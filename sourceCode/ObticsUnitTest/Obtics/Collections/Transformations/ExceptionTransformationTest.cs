using System.Text;
using System.Collections.Generic;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using ObticsUnitTest.Helpers;
using Obtics.Collections.Transformations;
using Obtics.Async;
using System.Threading;
using Obtics;
using Obtics.Collections;
using System;

namespace ObticsUnitTest.Obtics.Collections.Transformations
{
    /// <summary>
    /// Summary description for BufferTransformationTest
    /// </summary>
    [TestClass]
    public class ExceptionTransformationTest
    {
        public ExceptionTransformationTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        class CorrectnessRunner : CollectionTestSequenceRunnerForTransformation<string, string>
        {
            static List<string> result = new List<string>(
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }
            );

            public override List<string> ExpectedResult
            {
                get { return result; }
            }

            static List<string> source = new List<string>(
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }
            );

            public override List<string> SourceSequence
            {
                get { return source; }
            }

            static List<string> filler = new List<string>(
                new string[] { "1111", "12", "13", "14", "15", "16", "17", "18", "19", "20" }
            );

            public override List<string> FillerItems
            {
                get { return filler; }
            }

            public override IEnumerable<string> CreatePipeline(FrameIEnumerable<string> frame)
            {
                return ExceptionTransformation<string,Exception>.Create(frame.Select(s => { if(s == "1111") throw new Exception(); return s; } ),e => true, e => new string[0]);
            }

            public override string Prefix
            {
                get { return "ExceptionTransformation<string,Exception>.Create(frame,e => new string[0])"; }
            }
        }


        [TestMethod]
        public void CorrectnessTest()
        {
            var runner = new CorrectnessRunner();

            runner.RunAllPrepPairs();
        }


        class DeterministicEventRegistrationRunner : CollectionDeterministicEventRegistrationRunnerForTransformation<string, string>
        {
            protected override IEnumerable<string> CreateTransformation(FrameIEnumerableNPCNCC<string> frame)
            {
                return ExceptionTransformation<string,Exception>.Create(frame, e => true, e => new string[0]);
            }

            public override string Prefix
            {
                get { return "ExceptionTransformation<string,Exception>.Create(frame, e => new string[0])"; }
            }
        }

        [TestMethod]
        public void DeterministicEventRegistrationTest()
        {
            var runner = new DeterministicEventRegistrationRunner();
            runner.Run();
        }

        [TestMethod]
        public void ArgumentsCheck_source()
        {
            Assert.IsNull(ExceptionTransformation<string, Exception>.Create(null, e => true, e => new string[0]), "Should return null when source is null.");
        }

        [TestMethod]
        public void ArgumentsCheck_handler()
        {
            Assert.IsNull(ExceptionTransformation<string, Exception>.Create(new string[0], null, e => new string[0]), "Should return null when handler is null.");
        }
        [TestMethod]
        public void ArgumentsCheck_fallbackCollectionGenerator()
        {
            Assert.IsNull(ExceptionTransformation<string, Exception>.Create(new string[0], e => true, null), "Should return null when fallbackCollectionGenerator is null.");
        }

        [TestMethod]
        public void EqualityTest()
        {
            EqualityRunner.Run(
                StaticEnumerable<string>.Create(new string[0]), StaticEnumerable<string>.Create(new string[] { "a" }),
                new Func<Exception, bool>(e => true), e => false,
                new Func<Exception, IEnumerable<string>>(e => new string[0]), e => new string[0],
                (s, h, fcg) => ExceptionTransformation<string, Exception>.Create(s, h, fcg),
                "ExceptionTransformation<string, Exception>"
            );
        }


        [TestMethod]
        public void ConcurrencyTest()
        {
            AsyncTestRunnerForCollectionTransformation.Run(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                s => ExceptionTransformation<int, Exception>.Create(s, e => true, e => new int[0]),
                "BufferTransformation<int>.Create(s, new MTWorkQueue())"
            );
        }
    }
}
