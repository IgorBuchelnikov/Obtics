using System;
using Obtics.Values.Transformations;

namespace Obtics.Values
{
    public static partial class ValueProvider
    {
        /// <summary>
        /// Extends an <see cref="IValueProvider{TType}"/> with a value return path and allows control over the <see cref="IValueProvider.IsReadOnly"/> property.
        /// </summary>
        /// <typeparam name="TType">The type of the <see cref="IValueProvider{TType}.Value"/> property of the source and result <see cref="IValueProvider{TType}"/>.</typeparam>
        /// <param name="source">The <see cref="IValueProvider{TType}"/> that is going to be extended with a return path.</param>
        /// <param name="setValueAction">The action that needs to be taken when a client calls the <see cref="IValueProvider{TType}.Value"/> property setter.</param>
        /// <param name="isReadOnly">An <see cref="IValueProvider{Bool}"/> that determines the value of the <see cref="IValueProvider.IsReadOnly"/> property of the result <see cref="IValueProvider{TType}"/>.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/>, with an implementation for the <see cref="IValueProvider{TType}.Value"/> property setter and controled value for the <see cref="IValueProvider.IsReadOnly"/> property, or null when either <paramref name="source"/>, <paramref name="setValueAction"/> or <paramref name="isReadOnly"/> is null.</returns>
        /// <remarks>The <paramref name="setValueAction"/> delegate is called whenever a client calls the <see cref="IValueProvider{TType}.Value"/> property setter of the result, regardless of the value of its <see cref="IValueProvider.IsReadOnly"/> property.
        /// Any appropriate action needs to be taken by the delegate. Raising an <see cref="System.Data.ReadOnlyException"/> would be an appropriate response if <see cref="IValueProvider.IsReadOnly"/> is true.</remarks>
        public static IValueProvider<TType> ReturnPath<TType>(this IValueProvider<TType> source, Action<TType, bool> setValueAction, IValueProvider<bool> isReadOnly)
        { return _ReturnPath(source.Patched(), setValueAction, isReadOnly.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _ReturnPath<TType>(this IInternalValueProvider<TType> source, Action<TType, bool> setValueAction, IInternalValueProvider<bool> isReadOnly)
        {
            if (isReadOnly == null)
                return null;

            return ReturnPathTransformationExplicitReadOnlyCheck<TType>.Create(source, isReadOnly, setValueAction);
        }

        /// <summary>
        /// Extends an <see cref="IValueProvider{TType}"/> with a value return path and allows control over the <see cref="IValueProvider.IsReadOnly"/> property.
        /// </summary>
        /// <typeparam name="TType">The type of the <see cref="IValueProvider{TType}.Value"/> property of the source and result <see cref="IValueProvider{TType}"/>.</typeparam>
        /// <param name="source">The <see cref="IValueProvider{TType}"/> that is going to be extended with a return path.</param>
        /// <param name="setValueAction">The action that needs to be taken when a client calls the <see cref="IValueProvider{TType}.Value"/> property setter.</param>
        /// <param name="isReadOnly">An <see cref="IValueProvider{Bool}"/> that determines the value of the <see cref="IValueProvider.IsReadOnly"/> property of the result <see cref="IValueProvider{TType}"/>.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/>, with an implementation for the <see cref="IValueProvider{TType}.Value"/> property setter and controled value for the <see cref="IValueProvider.IsReadOnly"/> property, or null when either <paramref name="source"/>, <paramref name="setValueAction"/> or <paramref name="isReadOnly"/> is null.</returns>
        /// <remarks>The <paramref name="setValueAction"/> delegate is called whenever a client calls the <see cref="IValueProvider{TType}.Value"/> property setter of the result, regardless of the value of its <see cref="IValueProvider.IsReadOnly"/> property.
        /// Any appropriate action needs to be taken by the delegate. Raising an <see cref="System.Data.ReadOnlyException"/> would be an appropriate response if <see cref="IValueProvider.IsReadOnly"/> is true.</remarks>
        public static IValueProvider<TType> ReturnPath<TType>(this IValueProvider<TType> source, Action<TType> setValueAction, IValueProvider<bool> isReadOnly)
        { return _ReturnPath(source.Patched(), setValueAction, isReadOnly.Patched()).Concrete(); }

        internal static IInternalValueProvider<TType> _ReturnPath<TType>(this IInternalValueProvider<TType> source, Action<TType> setValueAction, IInternalValueProvider<bool> isReadOnly)
        {
            if (isReadOnly == null)
                return null;

            return ReturnPathTransformationImplicitReadOnlyCheck<TType>.Create(source, isReadOnly, setValueAction);
        }

        /// <summary>
        /// Extends an <see cref="IValueProvider{TType}"/> with a value return path.
        /// </summary>
        /// <typeparam name="TType">The type of the <see cref="IValueProvider{TType}.Value"/> property of the source and result <see cref="IValueProvider{TType}"/>.</typeparam>
        /// <param name="source">The <see cref="IValueProvider{TType}"/> that is going to be extended with a return path.</param>
        /// <param name="setValueAction">The action that needs to be taken when a client calls the <see cref="IValueProvider{TType}.Value"/> property setter.</param>
        /// <returns>An <see cref="IValueProvider{TType}"/>, with an implementation for the <see cref="IValueProvider{TType}.Value"/> property setter and an <see cref="IValueProvider.IsReadOnly"/> property whose value will always be false, or null when either <paramref name="source"/> or <paramref name="setValueAction"/> is null.</returns>
        /// <remarks>The value of the <see cref="IValueProvider.IsReadOnly"/> property of the result will always be false.</remarks>
        public static IValueProvider<TType> ReturnPath<TType>(this IValueProvider<TType> source, Action<TType> setValueAction)
        { return _ReturnPath(source.Patched(), setValueAction).Concrete(); }

        internal static IInternalValueProvider<TType> _ReturnPath<TType>(this IInternalValueProvider<TType> source, Action<TType> setValueAction)
        { return ReturnPathTransformationImplicitReadOnlyCheck<TType>.Create(source, null, setValueAction); }
    }
}
