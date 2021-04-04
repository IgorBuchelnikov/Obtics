using System;
using System.Collections.Generic;
using Obtics.Collections;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Obtics.Values.Transformations
{
    /// <summary>
    /// AsCollectionNullableTransformation represents an IValueProvider as a Sequence (IEnumerable) with
    /// exactly one or no members. The result sequence will have no member if and only if 'predicate'
    /// returns false for the Value property of the source.
    /// </summary>
    /// <typeparam name="TType">Type of the Value property of the source and the members of the result sequence.</typeparam>
    internal sealed class AsCollectionNullableTransformation<TType> : NCSourcedObjectToVE<TType, Tuple<IInternalValueProvider<TType>, Func<TType, bool>>>
    {
        public static IEnumerable<TType> GeneralCreate(IInternalValueProvider<TType> source, Func<TType, bool> predicate)
        {
            var sourceAsStatic = source as StaticValueProvider<TType>;

            if (sourceAsStatic != null)
            {
                if (predicate == null)
                    return null;

                var val = sourceAsStatic.Value;
                return predicate(val) ? StaticEnumerable<TType>.Create(val) : StaticEnumerable<TType>.Create();
            }

            return Create(source, predicate);
        }

        internal override void Initialize(int hash, Tuple<IInternalValueProvider<TType>, Func<TType, bool>> prms)
        {
            base.Initialize(hash, prms);
            IsMostUnordered = true;
        }

        public static AsCollectionNullableTransformation<TType> Create(IInternalValueProvider<TType> source, Func<TType, bool> predicate)
        {
            if (source == null || predicate == null)
                return null;

            return Carrousel.Get<AsCollectionNullableTransformation<TType>, IInternalValueProvider<TType>, Func<TType, bool>>(source, predicate);
        }

        #region IVersionedEnumerable<TType> Members

        VersionNumber _ContentVersion;

        protected override VersionNumber ProtectedContentVersion
        { get { return _ContentVersion; } }


        #endregion

        #region SourcePropertyChangedEventHandler

        protected override object ProcessSourceChangedNotification(object sender, INCEventArgs args)
        {
            if (args.IsValueEvent)
                return INCEventArgs.CollectionReset(_ContentVersion = _ContentVersion.Next);

            return null;
        }

        #endregion

        internal protected override INotifyChanged GetSource(int i)
        { return i == 0 ? _Prms.First : null;  }

        IEnumerator<TType> BuildEnumerator( TType value )
        {
            if (_Prms.Second(value))
                yield return value;
        }

        protected override Obtics.Collections.IVersionedEnumerator<TType> ProtectedGetEnumerator()
        {
            return 
                Obtics.Collections.VersionedEnumerator.WithContentVersion(
                    BuildEnumerator(_Prms.First.Value),
                    _ContentVersion
                );
        }   
    }
}
