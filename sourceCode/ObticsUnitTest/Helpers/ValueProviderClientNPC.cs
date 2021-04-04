using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Obtics;
using Obtics.Values;

namespace ObticsUnitTest.Helpers
{
    internal class ValueProviderClientNPC<TType> : ValueProviderClient<TType>
    {
        protected override void UpdateSource(bool before)
        {
            var npc = Source as INotifyPropertyChanged;

            if (npc == null)
            {
                var iivp = Source as IInternalValueProvider;

                if (iivp != null)
                    npc = (INotifyPropertyChanged)iivp.Concrete();
            }

            if (npc != null)
                if (before)
                    npc.PropertyChanged -= npc_PropertyChanged;
                else
                    npc.PropertyChanged += npc_PropertyChanged;

            base.UpdateSource(before);
        }

        public event PropertyChangedEventHandler SourcePropertyChangedReceived;

        protected void OnSourcePropertyChangedReceived(PropertyChangedEventArgs e)
        {
            var spcr = SourcePropertyChangedReceived;

            if (spcr != null)
                spcr(this, e);
        }

        public event EventHandler AfterBufferUpdate;

        protected void OnAfterBufferUpdate()
        {
            var abu = AfterBufferUpdate;

            if (abu != null)
                abu(this, EventArgs.Empty);
        }


        protected virtual void npc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock (this)
            {
                OnSourcePropertyChangedReceived(e);

                if (e.PropertyName == SIValueProvider.ValuePropertyName)
                {
                    Reset();
                    OnAfterBufferUpdate();
                }
            }
        }

    }
}
