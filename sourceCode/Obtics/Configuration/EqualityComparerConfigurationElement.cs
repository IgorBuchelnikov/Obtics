using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Obtics.Collections;
using System.ComponentModel;
using System.Collections;
using SL = System.Linq;

namespace Obtics.Configuration
{
#if !SILVERLIGHT
    /// <summary>
    /// A <see cref="ConfigurationElementCollection"/> of <see cref="EqualityComparerConfigurationElement"/> objects.
    /// </summary>
    public sealed class EqualityComparerConfigurationElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new element for this collection.
        /// </summary>
        /// <returns>A newly created element.</returns>
        protected override ConfigurationElement CreateNewElement()
        { return new EqualityComparerConfigurationElement(); }

        /// <summary>
        /// Gets a key that uniquely identifies the given element in this collection.
        /// </summary>
        /// <param name="element">The element to get the key from.</param>
        /// <returns>The key of the element.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        { return ((EqualityComparerConfigurationElement)element).EqualityComparerProvider; }
    }

    /// <summary>
    /// An <see cref="ConfigurationElement"/> that allows the specification of an <see cref="IEqualityComparerProvider"/>
    /// implementation in a configuration file.
    /// 
    /// The <see cref="IEqualityComparerProvider"/> should have a parameterless constructor. 
    /// </summary>
    public sealed class EqualityComparerConfigurationElement : ConfigurationElement
    {
        #region EqualityComparerProvider

        /// <summary>
        /// AttributeName of the name attribute.
        /// </summary>
        public const string EqualityComparerProviderAttributeName = "equalityComparerProvider";

        /// <summary>
        /// Type key of this ConfigurationElement instance.
        /// </summary>
        /// <remarks>
        /// This is a required attribute and the key of this information in the EqualityComparers collection.
        /// The matching attribute is 'type'.
        /// </remarks>
        [
            ConfigurationProperty(EqualityComparerProviderAttributeName,IsKey=true,IsRequired=true),
            TypeConverter(typeof(TypeTypeConverter))
        ]
        public Type EqualityComparerProvider
        { get{ return (Type)this[EqualityComparerProviderAttributeName]; } }


        IEqualityComparerProvider _EqualityComparerProviderInstance;

        /// <summary>
        /// Gets a new instance of the <see cref="ICollectionAdapterProvider"/>.
        /// </summary>
        /// <returns>A newly created instance of the <see cref="ICollectionAdapterProvider"/>.</returns>
        internal IEqualityComparerProvider EqualityComparerProviderInstance
        {
            get
            {
                return
                    _EqualityComparerProviderInstance ?? (_EqualityComparerProviderInstance =
                        (IEqualityComparerProvider)Activator.CreateInstance(EqualityComparerProvider)
                    );
            }
        }
        #endregion

        #region EqualityComparerType


        protected override void PostDeserialize()
        {
            //Check Type and Comparer attributes 
            var ecp = EqualityComparerProvider;

            if (ecp == null)
                throw new ConfigurationErrorsException(
                    String.Format(
                        "{0} can not be null.",
                        EqualityComparerConfigurationElement.EqualityComparerProviderAttributeName
                    )
                );


            if (!typeof(IEqualityComparerProvider).IsAssignableFrom(ecp))
                throw new ConfigurationErrorsException(
                    String.Format(
                        "{0} {1} should implement interface {2}.",
                        EqualityComparerConfigurationElement.EqualityComparerProviderAttributeName,
                        ecp.FullName,
                        typeof(IEqualityComparerProvider).FullName
                    )
                );

            var constructorInfo = ecp.GetConstructor(System.Type.EmptyTypes);

            if (constructorInfo == null)
                throw new ConfigurationErrorsException(
                    String.Format(
                        "{0} {1} should have a default constructor.",
                        EqualityComparerConfigurationElement.EqualityComparerProviderAttributeName,
                        ecp.FullName
                    )
                );

            base.PostDeserialize();
        }

        #endregion
    }
#endif
}
