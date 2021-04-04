using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Data;
using Obtics.Collections;
using System.Runtime.CompilerServices;

namespace Obtics.Data
{
    /// <summary>
    /// Adapt an untyped sequence with a CollectionChange event to a typed sequence with INotifyCollectionChanged
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <typeparam name="TTraits"></typeparam>
    /// <remarks>
    /// This adapater is not thread safe in principle. If the source is synchronized this should not be a problem.
    /// </remarks>
    abstract class CollectionAdapter<TType,TSource> : IVersionedEnumerable<TType>, ICollection, INotifyCollectionChanged where TSource : ICollection
    {
        readonly TSource _Source;
        List<TType> _Buffer;

        protected CollectionAdapter(TSource source)
        { _Source = source; }


        IEnumerator<TType> _GetEnumerator()
        {
            foreach (object item in _Source)
                yield return (TType)item;
        }

        #region IVersionedEnumerable<TType> Members

        public IVersionedEnumerator<TType> GetEnumerator()
        { return Obtics.Collections.VersionedEnumerator.WithContentVersion( _GetEnumerator(), _VersionNumber ); }

        #endregion

        #region IVersionedEnumerable Members

        VersionNumber _VersionNumber;

        public VersionNumber ContentVersion
        { get { return _VersionNumber; } }

        IVersionedEnumerator IVersionedEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region IEnumerable<TType> Members

        IEnumerator<TType> IEnumerable<TType>.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        #endregion

        #region INotifyCollectionChanged Members

        event NotifyCollectionChangedEventHandler _CollectionChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add 
            {
                if (_CollectionChanged == null)
                {
                    Register(_Source, CollectionChangeEventHandler);
                    _VersionNumber = _VersionNumber.Next;
                    _Buffer = new List<TType>(this);
                }

                _CollectionChanged += value;
            }

            remove 
            {
                _CollectionChanged -= value;

                if (_CollectionChanged == null)
                {
                    Unregister(_Source, CollectionChangeEventHandler);
                    _Buffer = null;
                }
            }
        }

        public abstract void Register(TSource source, CollectionChangeEventHandler handler);

        public abstract void Unregister(TSource source, CollectionChangeEventHandler handler);

        void CollectionChangeEventHandler(object sender, CollectionChangeEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = null;

            _VersionNumber = _VersionNumber.Next;

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    {
                        var ix = 0;
                        var element = e.Element;

                        foreach (object sElement in _Source)
                            if (sElement == element)
                                break;
                            else
                                ++ix;

                        args = new OrderedNotifyCollectionChangedEventArgs(_VersionNumber, NotifyCollectionChangedAction.Add, element, ix);

                        _Buffer.Insert(ix, (TType)element);
                    }
                    break;
                case CollectionChangeAction.Refresh:
                    args = new OrderedNotifyCollectionChangedEventArgs(_VersionNumber, NotifyCollectionChangedAction.Reset);
                    _Buffer = new List<TType>(this);
                    break;
                case CollectionChangeAction.Remove:
                    {  
                        var element = e.Element;
                        var ix = _Buffer.IndexOf((TType)element);

                        args = new OrderedNotifyCollectionChangedEventArgs(_VersionNumber, NotifyCollectionChangedAction.Remove, element, ix);

                        _Buffer.RemoveAt(ix);
                    }
                    break;
                default:
                    throw new Exception("Unexpected CollectionChangeAction value.");
            }

            _CollectionChanged(this, args);
        }
        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        { _Source.CopyTo(array, index); }

        public int Count
        { get { return _Source.Count; } }

        public bool IsSynchronized
        { get { return _Source.IsSynchronized; } }

        public object SyncRoot
        { get { return _Source.SyncRoot; } }

        #endregion

        internal abstract class AdapterClassBase : ICollectionAdapter
        {
            static ConditionalWeakTable<object, IVersionedEnumerable> _Carrousel = new ConditionalWeakTable<object, IVersionedEnumerable>();

            #region ICollectionAdapter Members

            public IVersionedEnumerable AdaptCollection(object collection)
            { return collection == null ? null : _Carrousel.GetOrAddUnderLock(collection, c => CreateAdapter(c)); }

            #endregion

            protected abstract IVersionedEnumerable CreateAdapter(object collection);
        }
    }

    sealed class DataTableCollectionAdapter : CollectionAdapter<DataTable,DataTableCollection>
    {
        internal class AdapterClass : AdapterClassBase
        {
            protected override IVersionedEnumerable CreateAdapter(object collection)
            { return new DataTableCollectionAdapter((DataTableCollection)collection); }
        }

        public override void Register(DataTableCollection source, CollectionChangeEventHandler handler)
        { source.CollectionChanged += handler; }

        public override void Unregister(DataTableCollection source, CollectionChangeEventHandler handler)
        { source.CollectionChanged -= handler; }

        public DataTableCollectionAdapter(DataTableCollection source)
            : base(source)
        { }
    }

    sealed class DataRelationCollectionAdapter : CollectionAdapter<DataRelation, DataRelationCollection>
    {
        internal class AdapterClass : AdapterClassBase
        {
            protected override IVersionedEnumerable CreateAdapter(object collection)
            { return new DataRelationCollectionAdapter((DataRelationCollection)collection); }
        }

        public override void Register(DataRelationCollection source, CollectionChangeEventHandler handler)
        { source.CollectionChanged += handler; }

        public override void Unregister(DataRelationCollection source, CollectionChangeEventHandler handler)
        { source.CollectionChanged -= handler; }

        public DataRelationCollectionAdapter(DataRelationCollection source)
            : base(source)
        { }
    }

    sealed class DataCollumnCollectionAdapter : CollectionAdapter<DataColumn, DataColumnCollection>
    {
        internal class AdapterClass : AdapterClassBase
        {
            protected override IVersionedEnumerable CreateAdapter(object collection)
            { return new DataCollumnCollectionAdapter((DataColumnCollection)collection); }
        }

        public override void Register(DataColumnCollection source, CollectionChangeEventHandler handler)
        { source.CollectionChanged += handler; }

        public override void Unregister(DataColumnCollection source, CollectionChangeEventHandler handler)
        { source.CollectionChanged -= handler; }

        public DataCollumnCollectionAdapter(DataColumnCollection source)
            : base(source)
        { }
    }

    sealed class ConstraintCollectionAdapter : CollectionAdapter<Constraint, ConstraintCollection>
    {
        internal class AdapterClass : AdapterClassBase
        {
            protected override IVersionedEnumerable CreateAdapter(object collection)
            { return new ConstraintCollectionAdapter((ConstraintCollection)collection); }
        }

        public override void Register(ConstraintCollection source, CollectionChangeEventHandler handler)
        { source.CollectionChanged += handler; }

        public override void Unregister(ConstraintCollection source, CollectionChangeEventHandler handler)
        { source.CollectionChanged -= handler; }

        public ConstraintCollectionAdapter(ConstraintCollection source)
            : base(source)
        { }
    }

    [ObticsRegistration]
    public sealed class ObticsToDataSetCollectionAdapterProvider : ICollectionAdapterProvider
    {
        #region ICollectionAdapterProvider Members

        public ICollectionAdapter GetCollectionAdapter(Type collectionType)
        {
            if ( typeof(ConstraintCollection).IsAssignableFrom(collectionType) )
                return new ConstraintCollectionAdapter.AdapterClass();

            if (typeof(DataColumnCollection).IsAssignableFrom(collectionType))
                return new DataCollumnCollectionAdapter.AdapterClass();

            if (typeof(DataRelationCollection).IsAssignableFrom(collectionType))
                return new DataRelationCollectionAdapter.AdapterClass();

            if (typeof(DataTableCollection).IsAssignableFrom(collectionType))
                return new DataTableCollectionAdapter.AdapterClass();

            if (typeof(DataRowCollection).IsAssignableFrom(collectionType))
                return new DataRowCollectionAdapter.AdapterClass();

            if (typeof(EnumerableRowCollection).IsAssignableFrom(collectionType))
            {
                var baseType = collectionType;

                do
                {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(EnumerableRowCollection<>))
                    {
                        return (ICollectionAdapter)
                            typeof(EnumerableRowCollectionAdapter<>)
                                .MakeGenericType(baseType.GetGenericArguments())
                                .GetConstructor(System.Type.EmptyTypes)
                                .Invoke(null);
                    }

                    baseType = baseType.BaseType;
                }
                while (baseType != typeof(object));

                return new EnumerableRowCollectionAdapter();
            }

            var baseTypeWalker = collectionType;

            while (typeof(object) != baseTypeWalker)
            {
                if (baseTypeWalker.IsGenericType && baseTypeWalker.GetGenericTypeDefinition() == typeof(global::System.Data.TypedTableBase<>))
                    return (ICollectionAdapter)
                        typeof(TypedTableAdapterClass<>)
                            .MakeGenericType(baseTypeWalker.GetGenericArguments())
                            .GetConstructor(Type.EmptyTypes)
                            .Invoke(null);

                baseTypeWalker = baseTypeWalker.BaseType;
            }

            return null;
        }

        #endregion
    }
}
