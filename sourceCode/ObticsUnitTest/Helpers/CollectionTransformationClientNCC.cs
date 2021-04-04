using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using O = Obtics;

namespace ObticsUnitTest.Helpers
{
    internal class CollectionTransformationClientNCC<TType> : CollectionTransformationClient<TType>
    {
        protected override void UpdateSource(bool before)
        {
            var ncc = Source as INotifyCollectionChanged;

            if (ncc != null)
                if (before)
                    ncc.CollectionChanged -= ncc_CollectionChanged;
                else
                    ncc.CollectionChanged += ncc_CollectionChanged;
            else
            {
                var nc = Source as O.Collections.IInternalEnumerable;

                if( nc != null )
                    if( before )
                        O.NCToNCC.Create(nc).CollectionChanged -= ncc_CollectionChanged;
                    else
                        O.NCToNCC.Create(nc).CollectionChanged += ncc_CollectionChanged;
            }

            base.UpdateSource(before);
        }

        public event NotifyCollectionChangedEventHandler SourceCollectionChangedReceived;

        protected virtual void OnSourceCollectionChangedReceived(NotifyCollectionChangedEventArgs e)
        {
            var sccr = SourceCollectionChangedReceived;

            if (sccr != null)
                sccr(this, e);
        }

        public event EventHandler AfterBufferUpdate;

        protected virtual void OnAfterBufferUpdate()
        {
            var abu = AfterBufferUpdate;

            if (abu != null)
                abu(this, EventArgs.Empty);
        }

        protected virtual void ncc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            lock (SyncRoot)
            {
                OnSourceCollectionChangedReceived(args);

                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        Buffer.InsertRange(args.NewStartingIndex, System.Linq.Enumerable.Cast<TType>(args.NewItems));
                        break;
#if !SILVERLIGHT
                    case NotifyCollectionChangedAction.Move:
                        Buffer.RemoveRange(args.OldStartingIndex, args.NewItems.Count);
                        Buffer.InsertRange(args.NewStartingIndex, System.Linq.Enumerable.Cast<TType>(args.NewItems));
                        break;
#endif
                    case NotifyCollectionChangedAction.Remove:
                        Buffer.RemoveRange(args.OldStartingIndex, args.OldItems.Count);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        Buffer.RemoveRange(args.NewStartingIndex, args.OldItems.Count);
                        Buffer.InsertRange(args.NewStartingIndex, System.Linq.Enumerable.Cast<TType>(args.NewItems));
                        break;
                    //case NotifyCollectionChangedAction.Reset:
                    default:
                        Reset();
                        break;
                }

                OnAfterBufferUpdate();
            }
        }
    }
}
