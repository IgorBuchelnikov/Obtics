using System.Collections.Generic;
using System.Collections.Specialized;

namespace Obtics.Collections.Transformations
{
    /// <summary>
    /// Collection changed notification translating buffer for indexed tranformations.
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    internal class IndexedTranslator<TType> : List<TType>
    {
        public INCollectionChangedEventArgs[] Translate(INCollectionChangedEventArgs args)
        {
            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    return AddAction((INCollectionAddEventArgs<TType>)args);
                case INCEventArgsTypes.CollectionRemove:
                    return RemoveAction((INCollectionRemoveEventArgs<TType>)args);
                case INCEventArgsTypes.CollectionReplace:
                    return ReplaceAction((INCollectionReplaceEventArgs<TType>)args);
                //case NotifyCollectionChangedAction.Reset:
                default:
                    return new INCollectionChangedEventArgs[] { args };
            }
        }

        private static INCollectionChangedEventArgs[] BuildEventsList(int ix, List<TType> oldItems, List<TType> newItems)
        {
            var oldItemsCount = oldItems.Count;
            var newItemsCount = newItems.Count;

            INCollectionChangedEventArgs[] res = new INCollectionChangedEventArgs[oldItemsCount + newItemsCount];

            for (int i = 0; i < oldItemsCount; ++i)
                res[i] =
                    INCEventArgs.CollectionRemove<TType>(
                        default(VersionNumber),
                        ix,
                        oldItems[i]
                    );

            for (int i = 0; i < newItemsCount; ++i)
                res[oldItemsCount + i] =
                    INCEventArgs.CollectionAdd<TType>(
                        default(VersionNumber),
                        ix + i,
                        newItems[i]
                    );

            return res;
        }

        private INCollectionChangedEventArgs[] ReplaceAction(INCollectionReplaceEventArgs<TType> args)
        {
            this[args.Index] = args.NewItem;
            return new INCollectionChangedEventArgs[] { args };
        }

        private INCollectionChangedEventArgs[] RemoveAction(INCollectionRemoveEventArgs<TType> args)
        {
            var ix = args.Index;

            if (ix + 1 == Count)
            {
                RemoveAt(ix);
                return new INCollectionChangedEventArgs[] { args };
            }
            else
            {
                List<TType> oldItems = GetRange(ix, Count - ix);
                RemoveAt(ix);
                List<TType> newItems = GetRange(ix, Count - ix);

                return BuildEventsList(ix, oldItems, newItems);
            }
        }

        private INCollectionChangedEventArgs[] AddAction(INCollectionAddEventArgs<TType> args)
        {
            var ix = args.Index;
            var item = args.Item;

            if (ix == Count)
            {
                Add(item);
                return new INCollectionChangedEventArgs[] { args };
            }
            else
            {
                List<TType> oldItems = GetRange(ix, Count - ix);
                Insert(ix, item);
                List<TType> newItems = GetRange(ix, Count - ix);

                return BuildEventsList(ix, oldItems, newItems);
            }
        }
    }
}
