using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;
using System.Data;

namespace Obtics.Values
{
    public static class ObticsToDataSetValueProvider
    {
        public static IEnumerable<TElement> Cascade<TElement>(this IValueProvider<EnumerableRowCollection<TElement>> source)
        { return ValueProvider.Cascade<EnumerableRowCollection<TElement>, TElement>(source); }

        public static IEnumerable<TElement> Cascade<TElement>(this IValueProvider<OrderedEnumerableRowCollection<TElement>> source)
        { return ValueProvider.Cascade<OrderedEnumerableRowCollection<TElement>, TElement>(source); }
    }
}
