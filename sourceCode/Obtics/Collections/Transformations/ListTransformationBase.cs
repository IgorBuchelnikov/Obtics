using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace Obtics.Collections.Transformations
{
    internal abstract class ListTransformationBase<TType, TSource, TPrms> : OpaqueTransformationBase<TType, TSource, TPrms>, IList<TType>, IList, INotifyPropertyChanged
        where TSource : IInternalEnumerable<TType>
    {
        List<TType> _Buffer;

        protected abstract TSource Source { get; }

        public override bool HasSafeEnumerator
        { get { return true; } }

        protected override void SubscribeOnSources()
        { Source.SubscribeINC(this); }

        protected override void UnsubscribeFromSources()
        { Source.UnsubscribeINC(this); }

        protected override void ClearBuffer(ref ObservableObjectBase<TPrms>.FlagsState flags)
        { _Buffer = null; }

        protected override VersionNumber InitializeBuffer(ref ObservableObjectBase<TPrms>.FlagsState flags)
        {
            using (var sourceEnumerator = Source.GetEnumerator())
            {
                _Buffer = new List<TType>();

                CollectionsHelper.Fill(_Buffer, sourceEnumerator);

                return sourceEnumerator.ContentVersion;
            }
        }

        List<TType> GetBuffer()
        {
            if (_Buffer == null)
            {
                using (var sourceEnumerator = Source.GetEnumerator())
                {
                    if (sourceEnumerator.ContentVersion != _SourceContentVersion)
                        return null;

                    _Buffer = new List<TType>();

                    CollectionsHelper.Fill(_Buffer, sourceEnumerator);
                }
            }

            return _Buffer;
        }

        #region ProcessCollectionChange


        protected override Change ProcessIncrementalChangeEvent(ref FlagsState flags, INCollectionChangedEventArgs args, out INCEventArgs message, out INCEventArgs[] messages)
        {
            var res = Change.Controled;
            messages = null;

            switch (args.Type)
            {
                case INCEventArgsTypes.CollectionAdd:
                    {
                        var addArgs = (INCollectionAddEventArgs<TType>)args;
                        _Buffer.Insert(addArgs.Index, addArgs.Item);
                    }
                    break;
                case INCEventArgsTypes.CollectionRemove:
                    {
                        var removeArgs = (INCollectionRemoveEventArgs<TType>)args;
                        _Buffer.RemoveAt(removeArgs.Index);
                    }
                    break;
                case INCEventArgsTypes.CollectionReplace:
                    {
                        var replaceArgs = (INCollectionReplaceEventArgs<TType>)args;
                        _Buffer[replaceArgs.Index] = replaceArgs.NewItem;
                    }
                    break;
                case INCEventArgsTypes.CollectionReset:
                    res = Change.Destructive;
                    break;

                default:
                    throw new Exception("Unexpected event type.");
            }

            message = args.Clone(AdvanceContentVersion());
            return res;
        }


        #endregion

        #region IEnumerable

        protected override IEnumerator<TType> GetEnumeratorDirect()
        { return SL.Enumerable.ToList(Source).GetEnumerator(); }

        protected override IEnumerator<TType> GetEnumeratorFromBuffer()
        { return _Buffer.GetEnumerator(); }

        protected override IEnumerable<TType> GetSnapshotEnumerable()
        { return _Buffer.ToArray(); }

        #endregion

        static void RaiseNotSupportedException()
        { throw new NotSupportedException(); }

        protected TResult DoAction<TPrm,TResult>(TPrm prm, Func<TPrm,TResult> onBuffered, Func<TPrm,TResult> onUnbuffered)
        {
            FlagsState flags;

           retry:

            while (!GetFlags(out flags)) ;

            var state = GetState(ref flags);

            switch (state)
            {
                case State.Excepted :
                case State.Initial :
                    return onUnbuffered(prm);

                default:
                    if (!Lock(ref flags))
                        goto retry;

                    break;
            }

            try
            {
                try
                {
                    switch (state)
                    {
                        case State.Clients:
                            SubscribeOnSources();
                            _ContentVersion = _ContentVersion.Next;
                            SetState(ref flags, state = State.Hidden);
                            goto case State.Hidden;

                        case State.Hidden:
                            CallInitializeBuffer(ref flags);
                            SetState(ref flags, state = State.Visible);
                            break;
                    }
                }
                catch (Exception)
                {
                    SetState(ref flags, State.Excepted);
                    throw;
                }

                return onBuffered(prm);
            }
            finally
            { Commit(ref flags); }
        }

        #region IList<TType> Members

        public int IndexOf(TType item)
        {
            return
                DoAction(
                    item,
                    itm => _Buffer.IndexOf(itm),
                    itm =>
                    {
                        int ix = 0;
                        var comparer = ObticsEqualityComparer<TType>.Default;
                        foreach (var sItem in Source)
                            if (comparer.Equals(itm, sItem))
                                return ix;
                            else
                                ++ix;

                        return -1;
                    }
                )
            ;
        }

        public virtual void Insert(int index, TType item)
        { RaiseNotSupportedException(); }

        public virtual void RemoveAt(int index)
        { RaiseNotSupportedException(); }

        public virtual void ReplaceAt(int index, TType item)
        { RaiseNotSupportedException(); }

        public const string ItemsIndexerPropertyName = SIList.ItemsIndexerPropertyName;

        public TType this[int index]
        {
            get
            {
                return
                    DoAction(
                        index,
                        ix => GetBuffer()[ix],
                        ix => SL.Enumerable.ElementAt(Source, ix)
                    )
                ;
            }
            set { ReplaceAt(index,value); }
        }

        #endregion

        #region ICollection<TType> Members

        public virtual void Add(TType item)
        { RaiseNotSupportedException(); }

        public virtual void Clear()
        { RaiseNotSupportedException(); }

        public bool Contains(TType item)
        {
            return
                DoAction(
                    item,
                    itm => _Buffer.Contains(itm),
                    itm =>
                    {
                        var comparer = ObticsEqualityComparer<TType>.Default;
                        foreach(var sItm in Source)
                            if(comparer.Equals(itm,sItm))
                                return true;

                        return false;
                    }
                )
            ;
        }

        public void CopyTo(TType[] array, int arrayIndex)
        {
            //GetEnumerator is public safe method only using local variables.. no locking.
            int i = arrayIndex;

            using (IEnumerator<TType> x = GetEnumerator())
            {
                while (x.MoveNext())
                    array[i++] = x.Current;
            }
        }

        public const string CountPropertyName = SICollection.CountPropertyName;

        public int Count
        {
            get
            {
                return
                    DoAction(
                        (object)null,
                        n => _Buffer.Count,
                        n => SL.Enumerable.Count(Source)
                    )
                ;
            }
        }

        public virtual bool Remove(TType item)
        { RaiseNotSupportedException(); return false; }

        public virtual bool IsReadOnly
        { get { return true; } }


        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            this.Add((TType)value);
            return -1;
        }

        void IList.Clear()
        { this.Clear(); }

        bool IList.Contains(object value)
        { return value is TType && this.Contains((TType)value); }

        int IList.IndexOf(object value)
        { return value is TType ? this.IndexOf((TType)value) : -1; }

        void IList.Insert(int index, object value)
        { this.Insert(index, (TType)value); }

        bool IList.IsFixedSize
        { get { return false; } }

        bool IList.IsReadOnly
        { get { return this.IsReadOnly; } }

        void IList.Remove(object value)
        {
            if (value is TType)
                this.Remove((TType)value);
        }

        void IList.RemoveAt(int index)
        { this.RemoveAt(index); }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (TType)value; }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        { this.CopyTo((TType[])array, index); }

        int ICollection.Count
        { get { return this.Count; } }

        public virtual bool IsSynchronized
        { get { return true; } }

        public virtual object SyncRoot
        { get { return null; } }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { Obtics.NCToNPC.Create(this).PropertyChanged += value; }
            remove { Obtics.NCToNPC.Create(this).PropertyChanged -= value; }
        }

        #endregion

    }
}
