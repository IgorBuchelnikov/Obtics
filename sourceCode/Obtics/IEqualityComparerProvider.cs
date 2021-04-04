using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Obtics
{
    public interface IEqualityComparerProvider
    {
        IEqualityComparer GetEqualityComparer(Type compareeType);
    }
}
