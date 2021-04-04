using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Obtics.Collections;
using System.ComponentModel;
using System.Collections;

namespace Obtics.Configuration
{
#if !SILVERLIGHT
    /// <summary>
    /// A <see cref="ConfigurationElementCollection"/> of <see cref="MappingProviderConfigurationElement"/> objects.
    /// </summary>
    public sealed class MappingProviderConfigurationElementCollection : ConfigurationElementCollection, IEnumerable<MappingProviderConfigurationElement>
    {
        /// <summary>
        /// Creates a new element for this collection.
        /// </summary>
        /// <returns>A newly created element.</returns>
        protected override ConfigurationElement CreateNewElement()
        { return new MappingProviderConfigurationElement(); }

        /// <summary>
        /// Gets a key that uniquely identifies the given element in this collection.
        /// </summary>
        /// <param name="element">The element to get the key from.</param>
        /// <returns>The key of the element.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        { return ((MappingProviderConfigurationElement)element).Type; }

        #region IEnumerable<MappingProviderConfigurationElement> Members

        public new IEnumerator<MappingProviderConfigurationElement> GetEnumerator()
        {
            foreach (MappingProviderConfigurationElement elt in (IEnumerable)this)
                yield return elt;
        }

        #endregion
    }

    /// <summary>
    /// An <see cref="ConfigurationElement"/> that allows the specification of an <see cref="IMappingLoaderProvider"/>
    /// implementation in a configuration file.
    /// 
    /// The <see cref="IMappingLoaderProvider"/> should have a parameterless constructor. 
    /// </summary>
    public sealed class MappingProviderConfigurationElement : ConfigurationElement
    {
        #region MappingLoaderType

        /// <summary>
        /// Name of the Type configuration attribute.
        /// </summary>
        public const string TypeAttributeName = "type";

        /// <summary>
        /// Type of the <see cref="IMappingLoaderProvider"/> implementation. This type should provide a public parameterless constructor.
        /// </summary>
        [
            ConfigurationProperty(TypeAttributeName, IsRequired=true),
            TypeConverter(typeof(TypeTypeConverter))
        ]
        public Type Type
        { get { return (Type)this[TypeAttributeName]; } }


        Obtics.Values.IExpressionObserverMappingProvider _Instance;

        /// <summary>
        /// Gets a new instance of the <see cref="IMappingLoaderProvider"/>.
        /// </summary>
        /// <returns>A newly created instance of the <see cref="IMappingLoaderProvider"/>.</returns>
        internal Obtics.Values.IExpressionObserverMappingProvider Instance
        {
            get
            {
                return
                    _Instance ?? (_Instance =
                        (Obtics.Values.IExpressionObserverMappingProvider)Activator.CreateInstance(Type)
                    )
                ;
            }
        }

        #endregion

        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            //Validate Type
            var type = Type;
            
            if (type == null)
                throw new ConfigurationErrorsException(
                    String.Format(
                        "Attribute {0} should point to a valid type.",
                        MappingProviderConfigurationElement.TypeAttributeName
                    )
                );

            if (!typeof(Obtics.Values.IExpressionObserverMappingProvider).IsAssignableFrom(type))
                throw new ConfigurationErrorsException(
                    String.Format(
                        "Attribute {0} should point to a type that implements {1}.",
                        MappingProviderConfigurationElement.TypeAttributeName,
                        typeof(Obtics.Values.IExpressionObserverMappingProvider).FullName
                    )
                );

            var constructorInfo = type.GetConstructor(System.Type.EmptyTypes);

            if (constructorInfo == null)
                throw new ConfigurationErrorsException(
                    String.Format(
                        "Attribute {0} should point to a type with a default contructor.",
                        ObticsConfigurationSection.DefaultWorkQueueProviderAttributeName
                    )
                );

        }
    }
#endif
}
