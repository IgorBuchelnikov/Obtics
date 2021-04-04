using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    public class CObj
    {
        public Person V;
    }

    public class CObjEqualityComparer : IEqualityComparer<CObj>
    {
        #region IEqualityComparer<CObj> Members

        public bool Equals(CObj x, CObj y)
        {
            return object.Equals(x.V,y.V);
        }

        public int GetHashCode(CObj obj)
        {
            return obj.V.GetHashCode();
        }

        #endregion
    }
}
