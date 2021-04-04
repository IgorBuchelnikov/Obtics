using System;
using System.Collections.Generic;
using SL = System.Linq;
using SLE = System.Linq.Enumerable;
using System.Text;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Unordered form of the <see cref="FilterTransformation{TType}"/>  class
    /// </summary>
    /// <typeparam name="TElement">Type of the elements of the sequence.</typeparam>
    /// <remarks>
    /// FilterTransformation maintains a buffer with element - index information. This class doesn't
    /// </remarks>
    internal class UnorderedFilterTransformation<TElement> : TranslucentTransformationBase<TElement,IInternalEnumerable<TElement>,Tuple<IInternalEnumerable<TElement>, Func<TElement, bool>>> 
    {
        //this class uses TranslucentTransformationBase
        //because in maintains its own content version
        //Not every event from the source gets passed to the client. (changes to elements that get filtered out)

        public static UnorderedFilterTransformation<TElement> Create(IEnumerable<TElement> s, Func<TElement, bool> predicate)
        {
            var source = s.PatchedUnordered();

            if (source == null || predicate == null)
                return null;

            return Carrousel.Get<UnorderedFilterTransformation<TElement>, IInternalEnumerable<TElement>, Func<TElement, bool>>(source, predicate);
        }

        public override bool IsMostUnordered
        { get { return true; } }

        protected override void SubscribeOnSources()
        { _Prms.First.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { _Prms.First.UnsubscribeINC(this); }

        #region IEnumerable<TType> Members

        protected override IEnumerator<TElement> GetEnumeratorDirect()
        { return SLE.Where(_Prms.First, _Prms.Second).GetEnumerator(); }

        IEnumerator<TElement> FilteredEnumerator(IEnumerator<TElement> source)
        {
            try
            {
                while (source.MoveNext())
                {
                    var c = source.Current;
                    if (_Prms.Second(c))
                        yield return c;
                }
            }
            finally
            {
                source.Dispose();
            }
        }

        protected override Tuple<bool, IEnumerator<TElement>> GetEnumeratorFromBuffer()
        {
            var enm = _Prms.First.GetEnumerator();
            return Tuple.Create(enm.ContentVersion == _SourceContentVersion, FilteredEnumerator(enm));
        }

        #endregion

        protected override VersionNumber InitializeBuffer(ref ObservableObjectBase<Tuple<IInternalEnumerable<TElement>, Func<TElement, bool>>>.FlagsState flags)
        {
            using(var enm = _Prms.First.GetEnumerator())
                return enm.ContentVersion; 
        }

        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs args, out INCEventArgs message, out INCEventArgs[] messages)
        {
            messages = null;
            message = null;
            var res = Change.None;

            if (args.IsCollectionEvent)
            {
                switch (args.Type)
                {
                    case INCEventArgsTypes.CollectionAdd:
                        {
                            var addArgs = (INCollectionAddEventArgs<TElement>)args;

                            if (_Prms.Second(addArgs.Item))
                            {
                                message = addArgs.Clone(AdvanceContentVersion());
                                res = Change.Controled;
                            }
                        }
                        break;
                    case INCEventArgsTypes.CollectionRemove:
                        {
                            var removeArgs = (INCollectionRemoveEventArgs<TElement>)args;

                            if (_Prms.Second(removeArgs.Item))
                            {
                                message = removeArgs.Clone(AdvanceContentVersion());
                                res = Change.Controled;
                            }
                        }
                        break;
                    case INCEventArgsTypes.CollectionReplace:
                        {
                            var replaceArgs = (INCollectionReplaceEventArgs<TElement>)args;
                            var oldItem = replaceArgs.OldItem;
                            var newItem = replaceArgs.NewItem;

                            var remove = _Prms.Second(oldItem);

                            if (_Prms.Second(newItem))
                            {
                                if (remove)
                                    message = replaceArgs.Clone(AdvanceContentVersion());
                                else
                                    message =
                                        INCEventArgs.CollectionAdd(
                                            AdvanceContentVersion(),
                                            -1,
                                            newItem
                                        );

                                res = Change.Controled;
                            }
                            else if (remove)
                            {
                                message =
                                    INCEventArgs.CollectionRemove(
                                        AdvanceContentVersion(),
                                        -1,
                                        oldItem
                                    );

                                res = Change.Controled;
                            }
                        }
                        break;
                    case INCEventArgsTypes.CollectionReset:
                        message = INCEventArgs.CollectionReset(AdvanceContentVersion());
                        res = Change.Destructive;
                        break;
                    default:
                        throw new Exception("Unexpected event type");
                }
            }

            return res;
        }

    }
}
