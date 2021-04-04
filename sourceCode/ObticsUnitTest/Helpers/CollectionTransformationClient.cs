using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Obtics.Collections;

namespace ObticsUnitTest.Helpers
{
    internal class CollectionTransformationClient<TType>
    {
        IEnumerable<TType> _Source;

        protected object SyncRoot
        {
            get
            {
                return ((ICollection)_Buffer).SyncRoot;
            }
        }

        public virtual IEnumerable<TType> Source
        {
            get 
            {
                lock (SyncRoot)
                    return _Source; 
            }
            set 
            {
                lock (SyncRoot)
                    if (_Source != value)
                    {
                        UpdateSource(true);
                        _Source = value;
                        UpdateSource(false);
                    }
            }
        }

        protected virtual void UpdateSource(bool before)
        {
            if (!before)
                Reset();
        }

        List<TType> _Buffer = new List<TType>();

        public List<TType> Buffer
        { get { return _Buffer; } }


        public virtual void Reset()
        {
            lock (SyncRoot)
            {
                _Buffer.Clear();

                if (_Source != null)
                {
                    var sourceEnumerator = _Source.GetEnumerator();
                    CollectionsHelper.Fill(Buffer, sourceEnumerator);
                }
            }
        }
    }
}
