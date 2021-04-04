using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using TvdP.Collections;

namespace Obtics.Collections
{
#if !SILVERLIGHT
    /// <summary>
    /// Extends the <see cref="NotifyCollectionChangedEventArgs"/> class with a property so that the order
    /// wherein different collection changed events are received can be checked.  
    /// </summary>
    public class OrderedNotifyCollectionChangedEventArgs : NotifyCollectionChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Reset"/>
        ///     change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This must be set to <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Reset"/>.</param>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action)
            : base(action)
        { _VersionNumber = versionNumber; }


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a multi-item change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This can be set to <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Reset"/>,
        ///     <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Add"/>, or <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Remove"/>.
        /// </param>
        /// <param name="changedItems"></param>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action, IList changedItems)
            : base(action, changedItems)
        { _VersionNumber = versionNumber; }


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a one-item change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This can be set to <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Reset"/>,
        ///     <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Add"/>, or <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Remove"/>.</param>
        /// <param name="changedItem">The item that is affected by the change.</param>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action, object changedItem)
            : base(action, changedItem)
        { _VersionNumber = versionNumber; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a multi-item <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Replace"/>
        ///     change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This can only be set to System.Collections.Specialized.NotifyCollectionChangedAction.Replace.</param>
        /// <param name="newItems">The new items that are replacing the original items.</param>
        /// <param name="oldItems">The original items that are replaced.</param>
        /// <exception cref="System.ArgumentException">If action is not Replace.</exception>
        /// <exception cref="System.ArgumentNullException">If oldItems or newItems is null.</exception>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action, IList newItems, IList oldItems)
            : base(action, newItems, oldItems)
        { _VersionNumber = versionNumber; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a multi-item change or a reset change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This can be set to <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Reset"/>,
        ///     <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Add"/>, or <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Remove"/>.</param>
        /// <param name="changedItems">The items affected by the change.</param>
        /// <param name="startingIndex">The index where the change occurred.</param>
        /// <exception cref="System.ArgumentException">If action is not Reset, Add, or Remove, if action is Reset and either changedItems
        ///     is not null or startingIndex is not -1, or if action is Add or Remove and
        ///     startingIndex is less than -1.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">If action is Add or Remove and changedItems is null.</exception>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
            : base(action, changedItems, startingIndex)
        { _VersionNumber = versionNumber; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a one-item change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This can be set to <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Reset"/>,
        ///     <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Add"/>, or <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Remove"/>.
        /// </param>
        /// <param name="changedItem">The item that is affected by the change.</param>
        /// <param name="index">The index where the change occurred.</param>
        /// <exception cref="System.ArgumentException">
        ///     If action is not Reset, Add, or Remove, or if action is Reset and either
        ///     changedItems is not null or index is not -1.
        /// </exception>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action, object changedItem, int index)
            : base(action, changedItem, index)
        { _VersionNumber = versionNumber; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a one-item <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Replace"/>
        ///     change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This can only be set to <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Replace"/>.</param>
        /// <param name="newItem">The new item that is replacing the original item.</param>
        /// <param name="oldItem">The original item that is replaced.</param>
        /// <exception cref="System.ArgumentException">If action is not Replace.</exception>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action, object newItem, object oldItem)
            : base(action, newItem, oldItem)
        { _VersionNumber = versionNumber; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a multi-item <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Replace"/>
        ///     change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This can only be set to <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Replace"/>.</param>
        /// <param name="newItems">The new items that are replacing the original items.</param>
        /// <param name="oldItems">The original items that are replaced.</param>
        /// <param name="startingIndex">The index of the begin item of the items that are being replaced.</param>
        /// <exception cref="System.ArgumentException">If action is not Replace.</exception>
        /// <exception cref="System.ArgumentNullException">If oldItems or newItems is null.</exception>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
            : base(action, newItems, oldItems, startingIndex)
        { _VersionNumber = versionNumber; }


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a multi-item <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Move"/>
        ///     change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This can only be set to <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Move"/>.</param>
        /// <param name="changedItems">The items affected by the change.</param>
        /// <param name="index">The new index for the changed items.</param>
        /// <param name="oldIndex">The old index for the changed items.</param>
        /// <exception cref="System.ArgumentException">If action is not Move or index is less than 0.</exception>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
            : base(action, changedItems, index, oldIndex)
        { _VersionNumber = versionNumber; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a multi-item <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Move"/>
        ///     change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This can only be set to <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Move"/>.</param>
        /// <param name="changedItem">The item affected by the change.</param>
        /// <param name="index">The new index for the changed item.</param>
        /// <param name="oldIndex">The old index for the changed item.</param>
        /// <exception cref="System.ArgumentException">If action is not Move or index is less than 0.</exception>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
            : base(action, changedItem, index, oldIndex)
        { _VersionNumber = versionNumber; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedNotifyCollectionChangedEventArgs"/>
        ///     class that describes a one-item <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Replace"/>
        ///     change.
        /// </summary>
        /// <param name="versionNumber">The sequence number of this event.</param>
        /// <param name="action">The action that caused the event. This can be set to <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Replace"/>.</param>
        /// <param name="newItem">The new item that is replacing the original item.</param>
        /// <param name="oldItem">The original item that is replaced.</param>
        /// <param name="index">The index of the item being replaced.</param>
        /// <exception cref="System.ArgumentException">If action is not Replace.</exception>
        public OrderedNotifyCollectionChangedEventArgs(VersionNumber versionNumber, NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
            : base(action, newItem, oldItem, index)
        { _VersionNumber = versionNumber; }

        readonly VersionNumber _VersionNumber;

        /// <summary>
        /// The versionNumber of the collection after processing of the information in this event.
        /// the event can be processed on a collection with ContentVersion == CollectionVersion.Previous
        /// </summary>
        public VersionNumber CollectionVersion
        { get { return _VersionNumber; } }
    }

#endif

    internal static class SIOrderedNotifyCollectionChanged
    {
        public static readonly NotifyCollectionChangedEventArgs ResetNotifyCollectionChangedEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

#if !SILVERLIGHT

        internal static VersionNumber? CollectionVersion(NotifyCollectionChangedEventArgs versionedSource)
        {
            var ordered = versionedSource as OrderedNotifyCollectionChangedEventArgs;
            return ordered != null ? (VersionNumber?)ordered.CollectionVersion : null ;
        }

        internal static  NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action); }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, object changedItem)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action, changedItem); }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, IList changedItems)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action, changedItems); }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, object changedItem, int index)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action, changedItem, index); }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, IList changedItems, int index)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action, changedItems, index); }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, object newItem, object oldItem)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action, newItem, oldItem); }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, IList newItems, IList oldItems)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action, newItems, oldItems); }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action, changedItem, index, oldIndex); }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action, changedItems, index, oldIndex); }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action, newItem, oldItem, index); }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, IList newItems, IList oldItems, int index)
        { return new OrderedNotifyCollectionChangedEventArgs(versionNumber, action, newItems, oldItems, index); }

#else
        static WeakKeyDictionary<NotifyCollectionChangedEventArgs, bool, VersionNumber> _VersionRetainer = new WeakKeyDictionary<NotifyCollectionChangedEventArgs,bool,VersionNumber>();

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action)
        { 
            var evt = new NotifyCollectionChangedEventArgs(action); 
            _VersionRetainer[evt,false] = versionNumber;
            return evt;
        }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, object changedItem, int index)
        { 
            var evt = new NotifyCollectionChangedEventArgs(action, changedItem, index);
            _VersionRetainer[evt, false] = versionNumber;
            return evt;
        }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
        {
            var evt = new NotifyCollectionChangedEventArgs(action, changedItem, index, oldIndex);
            _VersionRetainer[evt, false] = versionNumber;
            return evt;
        }

        internal static NotifyCollectionChangedEventArgs Create(VersionNumber versionNumber, NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            var evt = new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index);
            _VersionRetainer[evt, false] = versionNumber;
            return evt;
        }

        internal static VersionNumber? CollectionVersion(NotifyCollectionChangedEventArgs versionedSource)
        {
            VersionNumber versionNumber;
            return _VersionRetainer.TryGetValue(versionedSource, false, out versionNumber) ? (VersionNumber?)versionNumber : null ;
        }

#endif
    }
}
