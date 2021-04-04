using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Obtics;

namespace ObticsUnitTest.Helpers
{
    internal class CollectionTransformationClientNPC<TType> : CollectionTransformationClient<TType>
    {
        protected override void UpdateSource(bool before)
        {
            var npc = Source as INotifyPropertyChanged;

            if (npc != null)
                if (before)
                    npc.PropertyChanged -= npc_PropertyChanged;
                else
                    npc.PropertyChanged += npc_PropertyChanged;
            else
            {
                var nc = Source as INotifyChanged;

                if (nc != null)
                    if (before)
                        NCToNPC.Create(nc).PropertyChanged -= npc_PropertyChanged;
                    else
                        NCToNPC.Create(nc).PropertyChanged += npc_PropertyChanged;
            }

            base.UpdateSource(before);
        }

        public event PropertyChangedEventHandler SourcePropertyChangedReceived;

        protected virtual void OnSourcePropertyChangedReceived( PropertyChangedEventArgs e )
        {
            var spcr = SourcePropertyChangedReceived;

            if (spcr != null)
                spcr(this, e);
        }

        public event EventHandler AfterBufferUpdate;

        protected virtual void OnAfterBufferUpdate()
        {
            var abu = AfterBufferUpdate;
            if (abu != null)
                abu(this, EventArgs.Empty);
        }


        protected virtual void npc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock (SyncRoot)
            {
                OnSourcePropertyChangedReceived(e);

                if (e.PropertyName == SIList.ItemsIndexerPropertyName)
                {
                    Reset();

                    OnAfterBufferUpdate();
                }
            }
        }
    }
}
