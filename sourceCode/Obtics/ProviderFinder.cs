using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Obtics
{
    internal class ProviderFinder<TProvider,TKey,TValue> 
        where TProvider : class
        where TKey : class 
        where TValue : class
    {
        Func<TProvider, TKey, TValue> _valueExtractor;
        Func<TKey,TValue> _defaultValueCreator;
        TProvider[] _disambiguedProviders;
        bool _useReflection;

        public ProviderFinder(TProvider[] disambiguedProviders, Func<TProvider, TKey, TValue> valueExtractor, Func<TKey,TValue> defaultValueCreator, bool useReflection)
        {
            _valueExtractor = valueExtractor;
            _disambiguedProviders = disambiguedProviders;
            _defaultValueCreator = defaultValueCreator;
            _useReflection = useReflection;
        }

        ConditionalWeakTable<TKey, TValue> _KeyToValueMap = new ConditionalWeakTable<TKey, TValue>();

        ConditionalWeakTable<Assembly, IEnumerable<TProvider>> _AssemblyToProvidersMap = new ConditionalWeakTable<Assembly, IEnumerable<TProvider>>();

        IEnumerable<TProvider> GetProvidersForAssembly(Assembly assembly)
        {
            return 
                _AssemblyToProvidersMap.GetValue(
                    assembly, 
                    a =>
                        a.IsDynamic ? Enumerable.Empty<TProvider>() :
                        a.GetExportedTypes()
                            .Where(type => Attribute.GetCustomAttribute(type, typeof(ObticsRegistrationAttribute)) != null && typeof(TProvider).IsAssignableFrom(type))
                            .Select(type => (TProvider)Activator.CreateInstance(type))
                            .ToArray()
                )
            ;
        }

#if SILVERLIGHT

        static Assembly[] _Assemblies =
            System.Windows.Deployment.Current.Parts
                .Select(p => new Uri(p.Source, UriKind.Relative))
                .Concat(System.Windows.Deployment.Current.ExternalParts.Select(ep => ep.Source))
                .Select(uri => System.Windows.Application.GetResourceStream(uri))
                .Where(strInfo => strInfo != null)
                .Select(strInfo => new System.Windows.AssemblyPart().Load(strInfo.Stream))
                .Where(assembly => assembly != null)
                .ToArray()
        ;

#endif

        public TValue GetValue(TKey inkey)
        {
            //adapter will return the appropriate patch for the specific type of sequence.

            return
                _KeyToValueMap.GetValue(
                    inkey,
                    key =>
                    {
                        TValue value =
                            _disambiguedProviders
                                .Select(pr => _valueExtractor(pr, key))
                                .FirstOrDefault(val => val != null)
                        ;

                        if (value == null && _useReflection)
                        {
                            var cas =
#if SILVERLIGHT
                                _Assemblies
#else
                                System.AppDomain.CurrentDomain
                                    .GetAssemblies()
#endif
                                    .SelectMany(assembly => GetProvidersForAssembly(assembly))
                                    .Select(provider => Tuple.Create(provider, _valueExtractor(provider, key)))
                                    .Where(t => t.Second != null)
                                    .ToArray()
                            ;

                            //We should get only 1 solution this way.
                            //If we get more then raise an exception
                            if (cas.Length > 1)
                                throw new Exception(
                                    string.Format(
                                        "Multiple {0} found for {1} through providers {2} and {3}.",
                                        typeof(TValue).FullName,
                                        key.ToString(),
                                        cas[0].First.GetType().FullName,
                                        cas[1].First.GetType().FullName
                                    )
                                );
                            else if (cas.Length == 1)
                                value = cas[0].Second;
                        }


                        if (value == null)
                            value = _defaultValueCreator(key);

                        return value;
                    }
                )
            ;
        }
    }
}
