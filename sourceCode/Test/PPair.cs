using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    /// <summary>
    /// Another collection item type.
    /// This type though is a static value type
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public struct PPair<TFirst, TSecond>
    {
        public readonly TFirst First;
        public readonly TSecond Second;

        public PPair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }

        public override string ToString()
        { return "(" + ((object)First == null ? "?" : First.ToString()) + "," + ((object)Second == null ? "?" : Second.ToString()) + ") "; }
    }
}
