using System;
using System.Collections.Generic;
using SL = System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Obtics.Configuration;
using System.Security.Permissions;
using System.Security;
using TvdP.Collections;
using System.Collections.Specialized;
using Obtics.Collections.Patches;
using System.ComponentModel;
using System.Collections;

namespace Obtics.Collections
{
    /// <summary>
    /// Interface for objects that can provide <see cref="ICollectionAdapter"/> implementations for
    /// some types of unknown sequences.
    /// </summary>
    public interface ICollectionAdapterProvider
    {
        /// <summary>
        /// Gives an <see cref="ICollectionAdapter"/> implementation for a specific type of unknown sequence.
        /// </summary>
        /// <param name="collectionType">The unknown sequence type</param>
        /// <returns>An <see cref="ICollectionAdapter"/> that can adapt objects of type given by <paramref name="collectionType"/> or null of such an adapter can not be provided.</returns>
        ICollectionAdapter GetCollectionAdapter(Type collectionType);
    }

    /// <summary>
    /// Static class with static information about and helper methods for <see cref="ICollectionAdapterProvider"/>.
    /// </summary>
    public static class CollectionAdapterProvider
    {
        class NullAdapterClass : ICollectionAdapter
        {
            #region ICollectionAdapter Members

            public IVersionedEnumerable AdaptCollection(object collection)
            { return (IVersionedEnumerable)collection; }

            #endregion

            internal static readonly NotObservablePatchAdapterClass _Instance = new NotObservablePatchAdapterClass();
        }

        static bool SequenceTypeIsObservable(Type sourceType)
        { 
            return 
                typeof(INotifyCollectionChanged).IsAssignableFrom(sourceType) 
#if !SILVERLIGHT
                    || typeof(IBindingList).IsAssignableFrom(sourceType) 
#endif
                    || typeof(INotifyPropertyChanged).IsAssignableFrom(sourceType); 
        }

        static ICollectionAdapter CreateDefaultAdapter(Type sourceType)
        {


            if (typeof(IVersionedEnumerable).IsAssignableFrom(sourceType))
            {
                var specificVersionedElementType =
                    SL.Enumerable.FirstOrDefault(
                        sourceType.GetInterfaces()
                        , type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IVersionedEnumerable<>)
                            
                    );

                if (typeof(INotifyCollectionChanged).IsAssignableFrom(sourceType))
                {
                    return
                        specificVersionedElementType == null ? VersionedPatchAdapterClass._Instance :
                        (ICollectionAdapter)
                            (
                                    typeof(VersionedPatchAdapterClass<>)
                                    .MakeGenericType(specificVersionedElementType.GetGenericArguments())
                            )
                                .GetConstructor(Type.EmptyTypes)
                                .Invoke(null);
                }

                if (!SequenceTypeIsObservable(sourceType))
                    return NullAdapterClass._Instance;
            }

            var specificElementType =
                    SL.Enumerable.FirstOrDefault(
                        sourceType.GetInterfaces()
                        , type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)

                    );

            if (SequenceTypeIsObservable(sourceType))
            {
                return (ICollectionAdapter)
                    (
                        specificElementType == null
                        ?
                            typeof(StandardPatchAdapterClass<>)
                                .MakeGenericType(sourceType)

                        :
                            typeof(StandardPatchAdapterClass<,>)
                                .MakeGenericType(sourceType, specificElementType.GetGenericArguments()[0])
                    )
                        .GetConstructor(Type.EmptyTypes)
                        .Invoke(null);

            }
            else
            {
                return (ICollectionAdapter)
                    (
                        specificElementType == null
                        ?
                            NotObservablePatchAdapterClass._Instance
                        :
                            typeof(NotObservablePatchAdapterClass<>)
                                .MakeGenericType(specificElementType.GetGenericArguments()).GetConstructor(Type.EmptyTypes)
                                .Invoke(null)
                    );
            }
        }

#if !SILVERLIGHT
        static ProviderFinder<ICollectionAdapterProvider, Type, ICollectionAdapter> _ProviderFinder =
            new ProviderFinder<ICollectionAdapterProvider, Type, ICollectionAdapter>(
                SL.Enumerable.ToArray(
                    SL.Enumerable.Select(
                        SL.Enumerable.Cast<CollectionAdapterConfigurationElement>(
                            Obtics.Configuration.ObticsConfigurationSection.GetSection().CollectionAdapters
                        ), 
                        cace => cace.CollectionAdapterProviderInstance
                    )
                ),
                (provider, sourceType) => provider.GetCollectionAdapter(sourceType),
                CreateDefaultAdapter,
                Obtics.Configuration.ObticsConfigurationSection.GetSection().EnableRegistriationThroughReflection
            )
        ;
#else
        static ProviderFinder<ICollectionAdapterProvider, Type, ICollectionAdapter> _ProviderFinder =
            new ProviderFinder<ICollectionAdapterProvider, Type, ICollectionAdapter>(
                new ICollectionAdapterProvider[0],
                (provider, sourceType) => provider.GetCollectionAdapter(sourceType),
                 CreateDefaultAdapter,
                 true
            )
        ;
#endif

        internal static IVersionedEnumerable GetAdapter(IEnumerable source)
        { return _ProviderFinder.GetValue(source.GetType()).AdaptCollection(source); }
    }
}
