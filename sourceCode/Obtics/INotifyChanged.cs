using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Collections;
using System.Collections;
using System.Collections.Specialized;

namespace Obtics
{
    //A set of classes for the internal event mechanism.
    //Both INotifyPropertyChanged and INotifyCollectionChanged are represented
    //in this mechanism. This means there is one mechanism for both styles of events
    //and that simplifies the code.
    //
    //The internal mechanism works with interfaces instead of event handlers. Interfaces
    //can be registered directly and no Delegate objects need to be instantiated.
    //
    //Also the standard delegate lists of the MultiDelegate class is very inefficient
    //when working with large numbers of event consumers who regularly register and unregister.
    //NCObservableObjectBase has a custom consumer list mechanism for more scalable consumer
    //registration.

    /// <summary>
    /// Basic interface for change notification supplier
    /// </summary>
    internal interface INotifyChanged
    {
        /// <summary>
        /// Register for change notifications
        /// </summary>
        /// <param name="consumer">The consumer that is to receive change notifications</param>
        /// <remarks>
        /// A specific consumer can register only once. Calling SubscribeINC repeatedly with the
        /// same <paramref name="consumer"/> will lead to the consumer being registered only once.
        /// </remarks>
        void SubscribeINC(IReceiveChangeNotification consumer);

        /// <summary>
        /// Unregisters for change notifications
        /// </summary>
        /// <param name="consumer">The consumer to unregister.</param>
        /// <remarks>
        /// A consumer can have only one registration at a specific supplier. UnsubscribeINC will remove
        /// this one registration. Repeated calls to UnsubscribeINC without reregistering first will have
        /// no effect.
        /// </remarks>
        void UnsubscribeINC(IReceiveChangeNotification consumer);
    }

    /// <summary>
    /// Basic interface for change notification consumer
    /// </summary>
    internal interface IReceiveChangeNotification
    {
        void NotifyChanged(object sender, INCEventArgs changeArgs); 
    }

    /// <summary>
    /// Different types of change notification
    /// </summary>
    internal enum INCEventArgsTypes
    {
        /// <summary>
        /// A single item has been added to a collection
        /// </summary>
        CollectionAdd = 4,

        /// <summary>
        /// A single item has been removed from a collection
        /// </summary>
        CollectionRemove = 8,

        /// <summary>
        /// A single item in a collection has been replaced with another single item.
        /// </summary>
        CollectionReplace = 12,

        /// <summary>
        /// The entire contents of a collection has been changed.
        /// </summary>
        CollectionReset = 16,

        /// <summary>
        /// A property of the supplier has changed value.
        /// </summary>
        ValueChanged = 1,

        /// <summary>
        /// IsReadOnly property changed
        /// </summary>
        IsReadOnlyChanged = 2,

        /// <summary>
        /// Exception event
        /// </summary>
        Exception = 0
    } 

    /// <summary>
    /// Base class for change event argument objects.
    /// </summary>
    internal abstract class INCEventArgs : EventArgs
    {
        public abstract INCEventArgsTypes Type { get; }

        public bool IsCollectionEvent
        { get { return ((int)Type & 28) != 0; } }

        public bool IsValueEvent
        { get { return Type == INCEventArgsTypes.ValueChanged; } }

        public bool IsReadOnlyEvent
        { get { return Type == INCEventArgsTypes.IsReadOnlyChanged; } }

        public bool IsExeptionEvent
        { get { return Type == INCEventArgsTypes.Exception; } }

        static INPropertyChangedEventArgs _VCEA = new INPropertyChangedEventArgs();

        //PropertyChange
        public static INPropertyChangedEventArgs PropertyChanged()
        { return _VCEA; }

        static INIsReadOnlyChangedEventArgs _IROCEA = new INIsReadOnlyChangedEventArgs();

        //PropertyChange
        public static INIsReadOnlyChangedEventArgs IsReadOnlyChanged()
        { return _IROCEA; }

        public static INExceptionEventArgs Exception(Exception exception)
        { return new INExceptionEventArgs(exception); }

        public static VersionNumber FromNCC<TSource>(VersionNumber versionNumber, NotifyCollectionChangedEventArgs args, out INCollectionChangedEventArgs[] queue)
        {
            VersionNumber newSn = versionNumber;

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var items = args.NewItems;
                        var index = args.NewStartingIndex;
                        queue = new INCollectionChangedEventArgs[items.Count];

                        for(int i = 0, end = items.Count; i < end; ++i)
                            queue[i] = INCEventArgs.CollectionAdd<TSource>(newSn = newSn.Next, index+i, (TSource)items[i] ) ; 
                    }
                    break;
#if !SILVERLIGHT
                case NotifyCollectionChangedAction.Move:
                    {
                        var items = args.NewItems;
                        var oldIndex = args.OldStartingIndex;
                        var newIndex = args.NewStartingIndex;
                        var itemsCount = items.Count;
                        queue = new INCollectionChangedEventArgs[itemsCount << 1];

                        for (int i = 0; i < itemsCount; ++i)
                            queue[i] = INCEventArgs.CollectionRemove<TSource>(newSn = newSn.Next, oldIndex, (TSource)items[i]);

                        for (int i = 0; i < itemsCount; ++i)
                            queue[itemsCount + i] = INCEventArgs.CollectionAdd<TSource>(newSn = newSn.Next, newIndex + i, (TSource)items[i]);
                    }
                    break;
#endif
                case NotifyCollectionChangedAction.Remove:
                    {
                        var items = args.OldItems;
                        var index = args.OldStartingIndex;
                        var itemsCount = items.Count;
                        queue = new INCollectionChangedEventArgs[itemsCount];

                        for (int i = 0; i < itemsCount; ++i)
                            queue[i] = INCEventArgs.CollectionRemove<TSource>(newSn = newSn.Next, index, (TSource)items[i]);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        var oldItems = args.OldItems;
                        var newItems = args.NewItems;
                        var index = args.NewStartingIndex;
                        var oldItemsCount = oldItems.Count;
                        var newItemsCount = newItems.Count;
                        var minCount = Math.Min(oldItemsCount, newItemsCount);

                        queue = new INCollectionChangedEventArgs[oldItemsCount + newItemsCount - minCount];

                        int i = 0;

                        for (; i < minCount; ++i)
                            queue[i] = INCEventArgs.CollectionReplace<TSource>(newSn = newSn.Next, index++, (TSource)newItems[i], (TSource)oldItems[i]);

                        for (; i < oldItemsCount; ++i)
                            queue[i] = INCEventArgs.CollectionRemove<TSource>(newSn = newSn.Next, index, (TSource)oldItems[i]);

                        for (; i < newItemsCount; ++i)
                            queue[i] = INCEventArgs.CollectionAdd<TSource>(newSn = newSn.Next, index++, (TSource)newItems[i]);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    queue = new INCollectionChangedEventArgs[] { INCEventArgs.CollectionReset(newSn = newSn.Next) };
                    break;
                default:
                    throw new Exception("Unknown NotifyCollectionChangedAction");
            }

            return newSn;
        }

        //Reset
        public static INCollectionResetEventArgs CollectionReset(VersionNumber versionNumber)
        { return new INCollectionResetEventArgs(versionNumber); }

        //Add
        public static INCollectionAddEventArgs<TType> CollectionAdd<TType>(VersionNumber versionNumber, int index, TType item)
        { return new INCollectionAddEventArgs<TType>(versionNumber, item, index); }

        //Remove
        public static INCollectionRemoveEventArgs<TType> CollectionRemove<TType>(VersionNumber versionNumber, int index, TType item)
        { return new INCollectionRemoveEventArgs<TType>(versionNumber, item, index); }

        //Replace
        public static INCollectionReplaceEventArgs<TType> CollectionReplace<TType>(VersionNumber versionNumber, int index, TType newItem, TType oldItem)
        { return new INCollectionReplaceEventArgs<TType>(versionNumber, index, newItem, oldItem); }
    
    }

    internal sealed class INPropertyChangedEventArgs : INCEventArgs
    {
        internal INPropertyChangedEventArgs()
        { }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.ValueChanged; } }
    }

    internal sealed class INIsReadOnlyChangedEventArgs : INCEventArgs
    {
        internal INIsReadOnlyChangedEventArgs()
        { }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.IsReadOnlyChanged; } }
    }

    internal sealed class INExceptionEventArgs : INCEventArgs
    {
        internal INExceptionEventArgs(Exception exception)
        { _Exception = exception; }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.Exception; } }

        Exception _Exception;

        public new Exception Exception{ get{ return _Exception; } }
    }

    internal abstract class INCollectionChangedEventArgs : INCEventArgs
    {
        protected INCollectionChangedEventArgs(VersionNumber versionNumber)
        { _VersionNumber = versionNumber; }

        protected INCollectionChangedEventArgs(INCollectionChangedEventArgs other)
        { _VersionNumber = other._VersionNumber; }

        public abstract INCollectionChangedEventArgs Clone();

        public INCollectionChangedEventArgs Clone(VersionNumber versionNumber)
        {
            var res = Clone();
            res._VersionNumber = versionNumber;
            return res;
        }

        VersionNumber _VersionNumber;

        public VersionNumber VersionNumber 
        { 
            get { return _VersionNumber; }
        }
    }

    internal sealed class INCollectionResetEventArgs : INCollectionChangedEventArgs
    {
        internal INCollectionResetEventArgs(VersionNumber versionNumber)
            : base(versionNumber)
        { }

        internal INCollectionResetEventArgs(INCollectionResetEventArgs other)
            : base(other)
        { }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.CollectionReset; } }

        public override INCollectionChangedEventArgs Clone()
        { return new INCollectionResetEventArgs(this); }
    }

    internal abstract class SingleItemINCollectionEventArgs : INCollectionChangedEventArgs
    {        
        internal SingleItemINCollectionEventArgs(VersionNumber versionNumber, int index)
            : base(versionNumber)
        {
            Index = index;
        }

        internal SingleItemINCollectionEventArgs(SingleItemINCollectionEventArgs other, int index)
            : base(other)
        {
            Index = index;
        }

        public object Item { get { return UntypedItem; } }

        protected abstract object UntypedItem { get; }

        public int Index { get; private set; }
    }

    internal abstract class SingleItemINCollectionEventArgs<TType> : SingleItemINCollectionEventArgs
    {
        protected SingleItemINCollectionEventArgs(VersionNumber versionNumber, TType item, int index)
            : base(versionNumber, index)
        {
            Item = item;
        }

        protected SingleItemINCollectionEventArgs(SingleItemINCollectionEventArgs<TType> other)
            : base(other, other.Index)
        {
            Item = other.Item;
        }

        public new TType Item { get; private set; }

        protected override object UntypedItem { get { return this.Item; } }
    }

    internal sealed class INCollectionAddEventArgs<TType> : SingleItemINCollectionEventArgs<TType>
    {
        internal INCollectionAddEventArgs(VersionNumber versionNumber, TType item, int index)
            : base(versionNumber, item, index)
        {}

        internal INCollectionAddEventArgs(INCollectionAddEventArgs<TType> other)
            : base(other)
        {}

        public override INCollectionChangedEventArgs Clone()
        { return new INCollectionAddEventArgs<TType>(this); }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.CollectionAdd; } }
    }

    internal sealed class INCollectionRemoveEventArgs<TType> : SingleItemINCollectionEventArgs<TType>
    {
        internal INCollectionRemoveEventArgs(VersionNumber versionNumber, TType item, int index)
            : base(versionNumber, item, index)
        {}

        internal INCollectionRemoveEventArgs(INCollectionRemoveEventArgs<TType> other)
            : base(other)
        {}

        public override INCollectionChangedEventArgs Clone()
        { return new INCollectionRemoveEventArgs<TType>(this); }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.CollectionRemove; } }
    }

    internal abstract class INCollectionReplaceEventArgs : INCollectionChangedEventArgs
    {
        internal INCollectionReplaceEventArgs(VersionNumber versionNumber, int index)
            : base(versionNumber)
        {
            Index = index;
        }

        internal INCollectionReplaceEventArgs(INCollectionReplaceEventArgs other, int index)
            : base(other)
        {
            Index = index;
        }

        public object NewItem { get { return UntypedNewItem; } }

        protected abstract object UntypedNewItem { get; }

        public object OldItem { get { return UntypedOldItem; } }

        protected abstract object UntypedOldItem { get; }

        public int Index { get; private set; }
    }

    internal sealed class INCollectionReplaceEventArgs<TType> : INCollectionReplaceEventArgs
    {
        internal INCollectionReplaceEventArgs(VersionNumber versionNumber, int index, TType newItem, TType oldItem)
            : base(versionNumber, index)
        {
            NewItem = newItem;
            OldItem = oldItem;
        }

        internal INCollectionReplaceEventArgs(INCollectionReplaceEventArgs<TType> other)
            : base( other, other.Index )
        {
            NewItem = other.NewItem;
            OldItem = other.OldItem;
        }

        public override INCollectionChangedEventArgs Clone()
        { return new INCollectionReplaceEventArgs<TType>(this); }

        public override INCEventArgsTypes Type
        { get { return INCEventArgsTypes.CollectionReplace; } }

        public new TType NewItem { get; private set; }
        public new TType OldItem { get; private set; }

        protected override object UntypedNewItem
        { get { return this.NewItem; } }

        protected override object UntypedOldItem
        { get { return this.OldItem; } }
    }
}
