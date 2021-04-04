using System;
using System.Collections.Generic;
using System.Text;
using Obtics.Collections.Transformations;

namespace Obtics.Collections
{
    public static partial class ObservableEnumerable
    {
        public static IEnumerable<TSource> Cache<TSource>(this IEnumerable<TSource> source)
        {
            var patched = source.Patched();
            return patched.HasSafeEnumerator ? patched : CacheTransformation<TSource>.Create(patched); 
        }
    }
}
