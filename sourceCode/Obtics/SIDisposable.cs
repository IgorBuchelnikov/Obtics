using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obtics
{
    internal static class SIDisposable
    {
        public static void Dispose(object item)
        {
            var asDisposable = item as IDisposable;
            if (asDisposable != null)
                asDisposable.Dispose();
        }
    }
}
