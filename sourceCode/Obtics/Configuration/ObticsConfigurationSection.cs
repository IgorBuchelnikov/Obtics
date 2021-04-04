using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Obtics.Async;
using System.Security.Permissions;

namespace Obtics.Configuration
{
#if SILVERLIGHT
    internal sealed class ObticsConfigurationSection
    {
        internal IWorkQueueProvider GetDefaultWorkQueueProviderInstance()
        { return new WorkQueueOnDispatcherProvider(); }

        static ObticsConfigurationSection _ObticsConfigurationSection = new ObticsConfigurationSection();

        internal static ObticsConfigurationSection GetSection() { return _ObticsConfigurationSection; }
    }
#else
    /// <summary>
    /// Configuration section for Obtics.
    /// </summary>
    /// <remarks>The name of this configuration section is 'Obtics'.</remarks>
    public sealed class ObticsConfigurationSection :  ConfigurationSection
    {
        /// <summary>
        /// The name of the configuration section serviced by <see cref="ObticsConfigurationSection"/>.
        /// </summary>
        public const string ObticsConfigurationSectionName = "obtics";

        internal static ObticsConfigurationSection GetSection(System.Configuration.Configuration config)
        { return (ObticsConfigurationSection)config.GetSection(ObticsConfigurationSectionName) ?? new ObticsConfigurationSection(); }

        internal static ObticsConfigurationSection GetSection()
        { return (ObticsConfigurationSection)ConfigurationManager.GetSection(ObticsConfigurationSectionName) ?? new ObticsConfigurationSection(); }

        #region DefaultWorkQueueProvider

        /// <summary>
        /// Validator for the DefaultWorkQueueProvider configuration attribute.
        /// Expects the string given to represent a Type implementing <see cref="IWorkQueueProvider"/>
        /// and having a default constructor.
        /// </summary>
        public static class DefaultWorkQueueProviderValidatorClass
        {
            /// <summary>
            /// Name of the DefaultWorkQueueProviderValidator method.
            /// </summary>
            public const string DefaultWorkQueueProviderValidatorName = "DefaultWorkQueueProviderValidator";

            /// <summary>
            /// Verifies that the given type is a valid WorkQueueProvider
            /// </summary>
            /// <param name="typeObj">The <see cref="Type"/> object that needs to be verified.</param>
            public static void DefaultWorkQueueProviderValidator( object typeObj )
            {
                if (typeObj == null)
                    throw new ArgumentNullException("typeObj");

                var type = typeObj as Type ;

                if (type == null)
                    throw new ArgumentException("typeObj");

                if( !typeof(IWorkQueueProvider).IsAssignableFrom(type) )
                    throw new ConfigurationErrorsException(
                        String.Format(
                            "Configuration section {0} attribute {1} should point to a type that implements {2}.",
                            ObticsConfigurationSection.ObticsConfigurationSectionName,
                            ObticsConfigurationSection.DefaultWorkQueueProviderAttributeName,
                            typeof(IWorkQueueProvider).FullName
                        )
                    );

                var constructorInfo = type.GetConstructor(System.Type.EmptyTypes);

                if( constructorInfo == null )
                    throw new ConfigurationErrorsException( 
                        String.Format(
                            "Configuration section {0} attribute {1} should point to a type with a default contructor.", 
                            ObticsConfigurationSection.ObticsConfigurationSectionName, 
                            ObticsConfigurationSection.DefaultWorkQueueProviderAttributeName
                        ) 
                    );

            }        
        }

        /// <summary>
        /// Name of the DefaultWorkQueueProvider configuration attribute.
        /// </summary>
        public const string DefaultWorkQueueProviderAttributeName = "defaultWorkQueueProvider" ;

        /// <summary>
        /// DefaultWorkQueueProviderType, the type of the DefaultWorkQueueProvider.
        /// </summary>
        /// <remarks>The name of the configuration attribute of this property is 'DefaultWorkQueueProvider'</remarks>
        /// <seealso cref="Obtics.Async.WorkQueue.DefaultWorkQueueProvider"/>
        [
            ConfigurationProperty(DefaultWorkQueueProviderAttributeName, DefaultValue = typeof(WorkQueueOnThreadPoolProvider)),
            CallbackValidator(Type = typeof(DefaultWorkQueueProviderValidatorClass), CallbackMethodName = DefaultWorkQueueProviderValidatorClass.DefaultWorkQueueProviderValidatorName),
            TypeConverter(typeof(TypeTypeConverter))        
        ]
        public Type DefaultWorkQueueProviderType
        { get{ return (Type)this[DefaultWorkQueueProviderAttributeName]; } }

        /// <summary>
        /// Gets a new instance of the <see cref="DefaultWorkQueueProviderType"/>.
        /// </summary>
        /// <returns>A newly created instance of the <see cref="DefaultWorkQueueProviderType"/>.</returns>
        internal IWorkQueueProvider GetDefaultWorkQueueProviderInstance()
        {
            return (IWorkQueueProvider)
                DefaultWorkQueueProviderType
                    .GetConstructor(System.Type.EmptyTypes)
                    .Invoke(null);
        }

        #endregion


        #region enableRegistrationThroughReflection

        public const string EnableRegistriationThroughReflectionAttributeName = "enableRegistrationThroughReflection";

        /// <summary>
        /// Value telling obtics wether it should look for registrations through reflection and the <see cref="ObticsRegistrationAttribute"/>.
        /// </summary>
        [ConfigurationProperty(EnableRegistriationThroughReflectionAttributeName, DefaultValue = true)]
        public bool EnableRegistriationThroughReflection
        { get { return (bool)this[EnableRegistriationThroughReflectionAttributeName]; } }

        #endregion

        #region CollectionAdapters

        /// <summary>
        /// CollectionAdaptersElementName
        /// </summary>
        public const string CollectionAdaptersElementName = "collectionAdapters";

        /// <summary>
        /// CollectionAdapterConfigurationElementCollection
        /// </summary>
        [
            ConfigurationProperty(CollectionAdaptersElementName, IsDefaultCollection=false),
            ConfigurationCollection(typeof(CollectionAdapterConfigurationElementCollection))
        ]
        public CollectionAdapterConfigurationElementCollection CollectionAdapters
        { get { return (CollectionAdapterConfigurationElementCollection)this[CollectionAdaptersElementName]; } }

        #endregion

        #region RootMappings

        /// <summary>
        /// RootMappingsElementName
        /// </summary>
        public const string RootMappingsElementName = "rootMappings";

        /// <summary>
        /// Mapping loaders for the default expression observer object.
        /// </summary>
        [
            ConfigurationProperty(RootMappingsElementName, IsDefaultCollection = false),
            ConfigurationCollection(typeof(MappingProviderConfigurationElementCollection))
        ]
        public MappingProviderConfigurationElementCollection RootMappings
        { get { return (MappingProviderConfigurationElementCollection)this[RootMappingsElementName]; } }

        #endregion


        #region EqualityComparers

        /// <summary>
        /// EqualityComparersElementName
        /// </summary>
        public const string EqualityComparersElementName = "equalityComparers";

        /// <summary>
        /// EqualityComparerConfigurationElementCollection
        /// </summary>
        [
            ConfigurationProperty(EqualityComparersElementName, IsDefaultCollection = false),
            ConfigurationCollection(typeof(EqualityComparerConfigurationElementCollection))
        ]
        public EqualityComparerConfigurationElementCollection EqualityComparers
        { get { return (EqualityComparerConfigurationElementCollection)this[EqualityComparersElementName]; } }

        #endregion

        static internal ReflectionPermissionFlag? AssertReflectionPermissionFlag
        { get { return null; } }
    }
#endif
}
