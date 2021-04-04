using System;
using System.Collections.Generic;
using System.Text;
using TvdP.Collections;
using System.Collections;
using System.Threading;
using TvdP.Threading;

namespace Obtics.Collections.Transformations
{
    internal static class XLazySnapshotEnumerator
    {
        class VersionNode<TType>
        {
            internal TinyReaderWriterLock _Lock;

            //the actual content copy if created.
            internal IEnumerable<TType> _Materialized;

            //the number of active enumerators.
            internal Int32 _LiveEnumeratorCount;

            static WeakDictionary<object, VersionNumber, VersionNode<TType>> _ToVersionNodeMap = new WeakDictionary<object, VersionNumber, VersionNode<TType>>();

            static internal VersionNode<TType> Create(object trf, VersionNumber contentVersion)
            {
                //stateVersionNumber represents the sequence of inner enumerator state.
                //the inner enumerator state may progress even though the outwardVersionNumber remains the same.
                return _ToVersionNodeMap.GetOrAdd(trf, contentVersion, (_, __) => new VersionNode<TType>());
            }

            static internal VersionNode<TType> TryGet(object trf, VersionNumber contentVersion)
            { 
                VersionNode<TType> versionNode;
                return _ToVersionNodeMap.TryGetValue(trf, contentVersion, out versionNode) ? versionNode : null ;
            }
        }

        class Enumerator<TType> : IVersionedEnumerator<TType>
        {
            public Enumerator(VersionNumber contentVersion, IEnumerator<TType> source, VersionNode<TType> node)
            {
                _Source = source;
                _ContentVersion = contentVersion;
                _Node = node;

                Interlocked.Increment(ref node._LiveEnumeratorCount);
            }

            IEnumerator<TType> _Source;
            VersionNode<TType> _Node;
            long _Index;
            VersionNumber _ContentVersion;

            #region IVersionedEnumerator Members

            VersionNumber IVersionedEnumerator.ContentVersion
            { get{ return _ContentVersion; } }

            #endregion

            private VersionNode<TType> LockAndCheckVersionNode(VersionNode<TType> node)
            {
                if (node != null)
                {
                    node._Lock.LockForReading();

                    var materialized = node._Materialized;

                    if (materialized != null)
                    {
                        _Source.Dispose();
                        _Source = materialized.GetEnumerator();

                        node._Lock.ReleaseForReading();

                        _Node = node = null;

                        for (int i = 0; i < _Index; ++i)
                            _Source.MoveNext();
                    }
                }
                return node;
            }

            #region IEnumerator Members

            object IEnumerator.Current
            { get { return ((IEnumerator<TType>)this).Current; } }

            bool IEnumerator.MoveNext()
            {
                var node = _Node;

                try
                {
                    node = LockAndCheckVersionNode(node);

                    var res = _Source.MoveNext();
                    ++_Index;
                    return res;
                }
                finally
                {
                    if (node != null)
                        node._Lock.ReleaseForReading();
                }
            }

            void IEnumerator.Reset()
            {
                _Source.Reset();
                _Index = 0;
            }

            #endregion

            #region IEnumerator<TType> Members

            TType IEnumerator<TType>.Current
            {
                get 
                {
                    var node = _Node;

                    try
                    {
                        node = LockAndCheckVersionNode(node);
                        return _Source.Current;
                    }
                    finally
                    {
                        if (node != null)
                            node._Lock.ReleaseForReading();
                    }
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                var node = _Node;

                try
                {
                    node = LockAndCheckVersionNode(node);
                    _Source.Dispose();
                    if (node != null)
                        Interlocked.Decrement(ref node._LiveEnumeratorCount);
                }
                finally
                {
                    if (node != null)
                        node._Lock.ReleaseForReading();
                }
            }

            #endregion
        }

        /// <summary>
        /// Creates a new lazy snapshot enumerator.
        /// Not Concurrent with TakeSnapshot for same transformation!
        /// Transformation needs be locked when Create is called.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="transformation"></param>
        /// <param name="versionNumber"></param>
        /// <param name="innerEnumerator"></param>
        /// <returns></returns>
        internal static IVersionedEnumerator<TType> Create<TType>(object transformation, VersionNumber contentVersion, IEnumerator<TType> innerEnumerator)
        {
            var node = VersionNode<TType>.Create(transformation, contentVersion);
            return new Enumerator<TType>(contentVersion, innerEnumerator, node); 
        }

        /// <summary>
        /// A change to the source collection is about to happen. Create an actual copy if needed.
        /// Not Concurrent with Create for same transformation!
        /// Transformation needs be locked when TakeSnapshot is called.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="transformation">The collection that is about to change.</param>
        /// <param name="generateEnumerable">A function that creates a safe enumerable if an actual snapshot needs to be created.</param>
        internal static void TakeSnapshot<TType>(object transformation, VersionNumber contentVersion, Func<IEnumerable<TType>> generateEnumerable)
        {
            VersionNode<TType> versionNode = VersionNode<TType>.TryGet(transformation,contentVersion) ;

            if (versionNode != null && versionNode._LiveEnumeratorCount != 0 && versionNode._Materialized == null)
            {
                versionNode._Lock.LockForWriting();

                //TODO: Exception handling
                try
                { versionNode._Materialized = generateEnumerable(); }
                finally
                { versionNode._Lock.ReleaseForWriting(); }
            }
        }
    }

    //Shapshot enumerators are needed in an MT environment. It is possible that an underlying collection changes
    //while a client is still enumerating it. A solution is to take a snapshot (copy the collection contents)
    //when a client starts enumerating.
    //
    //This lazy snapshot enumerator works slightly differently; Clients start to enumerate in a normal fashion and
    //only whan a change to the source collection is about to happen and clients are still actively enumerating the
    //contents of the collection will be copied and clients will continue enumerating from there.
    //Also only one copy (snapshot) is made for all active enumerators.    
}
