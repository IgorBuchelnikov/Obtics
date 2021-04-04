using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace ObticsUnitTest.Helpers
{
    public static class TestHelper
    {
        public static void ExpectException<TException>(Action action)
        {
            bool failed = false;
            try
            {
                action();
                failed = true;
            }
            catch (Exception ex)
            {
                var expected = typeof(TException);
                var actual = ex.GetType();
                if (actual != expected)
                    throw;
            }

            if (failed)
                throw new AssertFailedException("Expected exception of type " + typeof(TException).Name);
        }
    }
}
