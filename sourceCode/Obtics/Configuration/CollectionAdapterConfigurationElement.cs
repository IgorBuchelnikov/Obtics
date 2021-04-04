using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Obtics.Collections;
using System.ComponentModel;

namespace Obtics.Configuration
{
#if !SILVERLIGHT
    /// <summary>
    /// A <see cref="ConfigurationElementCollection"/> of <see cref="CollectionAdapterConfigurationElement"/> objects.
    /// </summary>
    public sealed class CollectionAdapterConfigurationElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new element for this collection.
        /// </summary>
        /// <returns>A newly created element.</returns>
        protected override ConfigurationElement CreateNewElement()
        { return new CollectionAdapterConfigurationElement(); }

        /// <summary>
        /// Gets a key that uniquely identifies the given element in this collection.
        /// </summary>
        /// <param name="element">The element to get the key from.</param>
        /// <returns>The key of the element.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        { return ((CollectionAdapterConfigurationElement)element).CollectionAdapterProvider; }
    }

    /// <summary>
    /// An <see cref="ConfigurationElement"/> that allows the specification of an <see cref="ICollectionAdapterProvider"/>
    /// implementation in a configuration file.
    /// 
    /// The <see cref="ICollectionAdapterProvider"/> should have a parameterless constructor. 
    /// </summary>
    public sealed class CollectionAdapterConfigurationElement : ConfigurationElement
    {
        #region CollectionAdapterType

        /// <summary>
        /// Name of the Type configuration attribute.
        /// </summary>
        public const string CollectionAdapterProviderAttributeName = "collectionAdapterProvider";

        /// <summary>
        /// Type of the <see cref="ICollectionAdapterProvider"/> implementation. This type should provide a public parameterless constructor.
        /// </summary>
        [
            ConfigurationProperty(CollectionAdapterProviderAttributeName, IsRequired=true),
            TypeConverter(typeof(TypeTypeConverter))
        ]
        public Type CollectionAdapterProvider
        { get { return (Type)this[CollectionAdapterProviderAttributeName]; } }


        ICollectionAdapterProvider _CollectionAdapterProviderInstance;

        /// <summary>
        /// Gets a new instance of the <see cref="ICollectionAdapterProvider"/>.
        /// </summary>
        /// <returns>A newly created instance of the <see cref="ICollectionAdapterProvider"/>.</returns>
        internal ICollectionAdapterProvider CollectionAdapterProviderInstance
        {
            get
            {
                return
                    _CollectionAdapterProviderInstance ?? (_CollectionAdapterProviderInstance =
                        (ICollectionAdapterProvider)Activator.CreateInstance(CollectionAdapterProvider)
                    );
            }
        }

        #endregion

        protected override void PostDeserialize()
        {
            var cap = CollectionAdapterProvider;

            if (cap == null)
                throw new ConfigurationErrorsException(
                    String.Format(
                        "{0} can not be null.",
                        CollectionAdapterConfigurationElement.CollectionAdapterProviderAttributeName
                    )
                );
            if (!typeof(ICollectionAdapterProvider).IsAssignableFrom(cap))
                throw new ConfigurationErrorsException(
                    String.Format(
                        "{0} {1} should implement interface {1}.",
                        CollectionAdapterConfigurationElement.CollectionAdapterProviderAttributeName,
                        cap.FullName,
                        typeof(ICollectionAdapterProvider).FullName
                    )
                );

            var constructorInfo = cap.GetConstructor(System.Type.EmptyTypes);

            if (constructorInfo == null)
                throw new ConfigurationErrorsException(
                    String.Format(
                        "{0} {1} should have a default constructor.",
                        CollectionAdapterConfigurationElement.CollectionAdapterProviderAttributeName,
                        cap.FullName
                    )
                );

            base.PostDeserialize();
        }
    }
#endif
}
