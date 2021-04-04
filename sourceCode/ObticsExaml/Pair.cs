using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObticsExaml
{
    public struct Pair<TFirst, TSecond>
    {
        readonly TFirst _First;

        public TFirst First
        { get { return _First; } }

        readonly TSecond _Second;

        public TSecond Second
        { get { return _Second; } }

        public Pair(TFirst first, TSecond second)
        {
            _First = first;
            _Second = second;
        }
    }
}
