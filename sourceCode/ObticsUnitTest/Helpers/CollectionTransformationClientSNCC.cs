using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Collections;

namespace ObticsUnitTest.Helpers
{
    internal class CollectionTransformationClientSNCC<TType> : CollectionTransformationClientNCC<TType>
    {
        VersionNumber? _SourceContentVersion;

        public virtual VersionNumber? SourceContentVersion
        { 
            get 
            { 
                lock(SyncRoot)
                    return _SourceContentVersion; 
            }
            set 
            {
                lock(SyncRoot)
                    _SourceContentVersion = value; 
            }
        }

        IVersionedEnumerator<TType> _SourceEnum;

        public override void Reset()
        {
            lock (SyncRoot)
            {
                Buffer.Clear();

                if (Source != null)
                {
                    IVersionedEnumerator<TType> sourceEnumerator = Source.GetEnumerator() as IVersionedEnumerator<TType>;

                    if (sourceEnumerator == null)
                        throw new Exception("Source must be IVersionedEnumerable");

                    _SourceContentVersion = sourceEnumerator.ContentVersion;
                    CollectionsHelper.Fill(Buffer, sourceEnumerator);

                    _SourceEnum = sourceEnumerator;
                }
            }
        }

        protected override void ncc_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            lock (SyncRoot)
            {
                if (object.ReferenceEquals(sender, Source))
                {
                    var newVersion = SIOrderedNotifyCollectionChanged.CollectionVersion(args);

                    if (newVersion.HasValue)
                    {
                        switch (newVersion.Value.IsInRelationTo(_SourceContentVersion))
                        {
                            case VersionRelation.Future:
                                args = SIOrderedNotifyCollectionChanged.ResetNotifyCollectionChangedEventArgs;
                                break;
                            case VersionRelation.Past:
                                return;
                        }

                        _SourceContentVersion = newVersion.Value;
                    }
                    else
                    {
                        args = SIOrderedNotifyCollectionChanged.ResetNotifyCollectionChangedEventArgs;
                        _SourceContentVersion = null;
                    }

                    base.ncc_CollectionChanged(sender, args);
                }
            }
        }
    }
}
