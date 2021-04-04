using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using ObticsUnitTest.Helpers;

namespace ObticsUnitTest.Obtics.Collections.Patches
{
    [TestClass]
    public class VersionedPatchTest
    {
        [TestMethod]
        public void ConcurrencyTest()
        {
            AsyncTestRunnerForCollectionTransformation.Run(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                s => global::Obtics.Collections.Patches.VersionedPatch<int>.Create(s),
                "global::Obtics.Collections.Patches.VersionedPatch<int>.Create(s)"
            );
        }
    }
}
