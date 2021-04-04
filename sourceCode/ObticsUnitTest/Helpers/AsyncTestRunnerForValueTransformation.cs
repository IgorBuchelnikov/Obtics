using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Values;
using System.Threading;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics;

namespace ObticsUnitTest.Helpers
{
    internal class AsyncTestRunnerForValueTransformation
    {
        public static void Run<TSource, TResult>(
            TSource[] sourceItems,
            TResult result,
            Func<IValueProvider<TSource>, IValueProvider<TResult>> builder,
            string prefix
        )
        {
            new AsyncTestRunnerForValueTransformation<TSource, TResult>(sourceItems, res => ObticsEqualityComparer<TResult>.Default.Equals(res, result), builder, prefix).Run();
        }

        public static void Run<TSource, TResult>(
            TSource[] sourceItems,
            Func<TResult, bool> verifier,
            Func<IValueProvider<TSource>, IValueProvider<TResult>> builder,
            string prefix
        )
        {
            new AsyncTestRunnerForValueTransformation<TSource, TResult>(sourceItems, verifier, builder, prefix).Run();
        }

        public static void Run<TSource, TSource2, TResult>(
            TSource[] sourceItems,
            TSource2[] source2Items,
            TResult result,
            Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TResult>> builder,
            string prefix
        )
        {
            new AsyncTestRunnerForValueTransformation<TSource, TSource2, TResult>(sourceItems, source2Items, res => ObticsEqualityComparer<TResult>.Default.Equals(res, result), builder, prefix).Run();
        }

        public static void Run<TSource, TSource2, TResult>(
            TSource[] sourceItems,
            TSource2[] source2Items,
            Func<TResult, bool> verifier,
            Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TResult>> builder,
            string prefix
        )
        {
            new AsyncTestRunnerForValueTransformation<TSource, TSource2, TResult>(sourceItems, source2Items, verifier, builder, prefix).Run();
        }

        public static void Run<TSource, TSource2, TSource3, TResult>(
            TSource[] sourceItems,
            TSource2[] source2Items,
            TSource3[] source3Items,
            TResult result,
            Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TResult>> builder,
            string prefix
        )
        {
            new AsyncTestRunnerForValueTransformation<TSource, TSource2, TSource3, TResult>(sourceItems, source2Items, source3Items, res => ObticsEqualityComparer<TResult>.Default.Equals(res, result), builder, prefix).Run();
        }

        public static void Run<TSource, TSource2, TSource3, TResult>(
            TSource[] sourceItems,
            TSource2[] source2Items,
            TSource3[] source3Items,
            Func<TResult, bool> verifier,
            Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TResult>> builder,
            string prefix
        )
        {
            new AsyncTestRunnerForValueTransformation<TSource, TSource2, TSource3, TResult>(sourceItems, source2Items, source3Items, verifier, builder, prefix).Run();
        }

        public static void Run<TSource, TSource2, TSource3, TSource4, TResult>(
            TSource[] sourceItems,
            TSource2[] source2Items,
            TSource3[] source3Items,
            TSource4[] source4Items,
            TResult result,
            Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TSource4>, IValueProvider<TResult>> builder,
            string prefix
        )
        {
            new AsyncTestRunnerForValueTransformation<TSource, TSource2, TSource3, TSource4, TResult>(sourceItems, source2Items, source3Items, source4Items, res => ObticsEqualityComparer<TResult>.Default.Equals(res, result), builder, prefix).Run();
        }

        public static void Run<TSource, TSource2, TSource3, TSource4, TResult>(
            TSource[] sourceItems,
            TSource2[] source2Items,
            TSource3[] source3Items,
            TSource4[] source4Items,
            Func<TResult, bool> verifier,
            Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TSource4>, IValueProvider<TResult>> builder,
            string prefix
        )
        {
            new AsyncTestRunnerForValueTransformation<TSource, TSource2, TSource3, TSource4, TResult>(sourceItems, source2Items, source3Items, source4Items, verifier, builder, prefix).Run();
        }
    }

    internal class AsyncTestRunnerForValueTransformation<TSource, TResult>
    {
        Func<IValueProvider<TSource>, IValueProvider<TResult>> _Builder;
        TSource[] _SourceItems;
        Func<TResult, bool> _Verifier;
        string _Prefix;

        internal AsyncTestRunnerForValueTransformation(TSource[] sourceItems, Func<TResult, bool> verifier, Func<IValueProvider<TSource>, IValueProvider<TResult>> builder, string prefix)
        {
            _SourceItems = sourceItems;
            _Builder = builder;
            _Verifier = verifier;
            _Prefix = prefix;
        }

        internal void Run()
        {
            {
                //NPC
                FrameIValueProviderNPC<TSource> frame = new FrameIValueProviderNPC<TSource>();
                AsyncFrameIValueProviderRunner<TSource> frameRunner = new AsyncFrameIValueProviderRunner<TSource>(_SourceItems, frame);
                frameRunner.Start();
                Thread.Sleep(0);
                var pipeline = _Builder(frame);
                var clientRunner = new AsynValueTransformationClientRunner<TResult>(
                    new ValueProviderClient<TResult>[] {
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                },
                    pipeline
                );
                var clientRunner2 = new AsynValueTransformationClientRunner<TResult>(
                    new ValueProviderClient<TResult>[] {
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                },
                    pipeline
                );
                clientRunner.Start();
                clientRunner2.Start();
                Thread.Sleep(1000);

                ValueProviderClientNPC<TResult>[] clients = new ValueProviderClientNPC<TResult>[5];

                for (int i = 0; i < clients.Length; ++i)
                {
                    clients[i] = new ValueProviderClientNPC<TResult>();
                    clients[i].Source = pipeline;
                    Thread.Sleep(500 / clients.Length);
                }

                frameRunner.Stop();
                clientRunner.Stop();
                clientRunner2.Stop();

                for (int i = 0; i < 50 && (frameRunner.IsRunning || clientRunner.IsRunning || clientRunner2.IsRunning); ++i)
                    Thread.Sleep(100);

                Assert.IsFalse(frameRunner.IsRunning, _Prefix + ": FrameRunner thread is still running on NPC test.");
                Assert.IsFalse(clientRunner.IsRunning, _Prefix + ": ClientRunner thread is still running on NPC test.");
                Assert.IsFalse(clientRunner2.IsRunning, _Prefix + ": ClientRunner2 thread is still running on NPC test.");

                for (int i = 0; i < clients.Length; ++i)
                    Assert.IsTrue(_Verifier(clients[i].Buffer), _Prefix + ": Client " + i.ToString() + " did not return the expected result on NPC test.");
            }
        }
    }

    internal class AsyncTestRunnerForValueTransformation<TSource, TSource2, TResult>
    {
        Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TResult>> _Builder;
        TSource[] _SourceItems;
        TSource2[] _Source2Items;
        Func<TResult, bool> _Verifier;
        string _Prefix;

        internal AsyncTestRunnerForValueTransformation(TSource[] sourceItems, TSource2[] source2Items, Func<TResult, bool> verifier, Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TResult>> builder, string prefix)
        {
            _SourceItems = sourceItems;
            _Source2Items = source2Items;
            _Builder = builder;
            _Verifier = verifier;
            _Prefix = prefix;
        }

        internal void Run()
        {
            {
                //NPC
                FrameIValueProviderNPC<TSource> frame = new FrameIValueProviderNPC<TSource>();
                AsyncFrameIValueProviderRunner<TSource> frameRunner = new AsyncFrameIValueProviderRunner<TSource>(_SourceItems, frame);
                frameRunner.Start();
                FrameIValueProviderNPC<TSource2> frame2 = new FrameIValueProviderNPC<TSource2>();
                AsyncFrameIValueProviderRunner<TSource2> frame2Runner = new AsyncFrameIValueProviderRunner<TSource2>(_Source2Items, frame2);
                frame2Runner.Start();
                Thread.Sleep(0);
                var pipeline = _Builder(frame, frame2);
                var clientRunner = new AsynValueTransformationClientRunner<TResult>(
                    new ValueProviderClient<TResult>[] {
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                },
                    pipeline
                );
                var clientRunner2 = new AsynValueTransformationClientRunner<TResult>(
                    new ValueProviderClient<TResult>[] {
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                },
                    pipeline
                );
                clientRunner.Start();
                clientRunner2.Start();
                Thread.Sleep(1000);

                ValueProviderClientNPC<TResult>[] clients = new ValueProviderClientNPC<TResult>[5];

                for (int i = 0; i < clients.Length; ++i)
                {
                    clients[i] = new ValueProviderClientNPC<TResult>();
                    clients[i].Source = pipeline;
                    Thread.Sleep(500 / clients.Length);
                }

                frameRunner.Stop();
                frame2Runner.Stop();
                clientRunner.Stop();
                clientRunner2.Stop();

                for (int i = 0; i < 50 && (frameRunner.IsRunning || frame2Runner.IsRunning || clientRunner.IsRunning || clientRunner2.IsRunning); ++i)
                    Thread.Sleep(100);

                Assert.IsFalse(frameRunner.IsRunning, _Prefix + ": FrameRunner thread is still running on NPC test.");
                Assert.IsFalse(frame2Runner.IsRunning, _Prefix + ": Frame2Runner thread is still running on NPC test.");
                Assert.IsFalse(clientRunner.IsRunning, _Prefix + ": ClientRunner thread is still running on NPC test.");
                Assert.IsFalse(clientRunner2.IsRunning, _Prefix + ": ClientRunner2 thread is still running on NPC test.");

                for (int i = 0; i < clients.Length; ++i)
                    Assert.IsTrue(_Verifier(clients[i].Buffer), _Prefix + ": Client " + i.ToString() + " did not return the expected result on NPC test.");
            }
        }
    }

    internal class AsyncTestRunnerForValueTransformation<TSource, TSource2, TSource3, TResult>
    {
        Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TResult>> _Builder;
        TSource[] _SourceItems;
        TSource2[] _Source2Items;
        TSource3[] _Source3Items;
        Func<TResult, bool> _Verifier;
        string _Prefix;

        internal AsyncTestRunnerForValueTransformation(TSource[] sourceItems, TSource2[] source2Items, TSource3[] source3Items, Func<TResult, bool> verifier, Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TResult>> builder, string prefix)
        {
            _SourceItems = sourceItems;
            _Source2Items = source2Items;
            _Source3Items = source3Items;
            _Builder = builder;
            _Verifier = verifier;
            _Prefix = prefix;
        }

        internal void Run()
        {
            {
                //NPC
                FrameIValueProviderNPC<TSource> frame = new FrameIValueProviderNPC<TSource>();
                AsyncFrameIValueProviderRunner<TSource> frameRunner = new AsyncFrameIValueProviderRunner<TSource>(_SourceItems, frame);
                frameRunner.Start();
                FrameIValueProviderNPC<TSource2> frame2 = new FrameIValueProviderNPC<TSource2>();
                AsyncFrameIValueProviderRunner<TSource2> frame2Runner = new AsyncFrameIValueProviderRunner<TSource2>(_Source2Items, frame2);
                frame2Runner.Start();
                FrameIValueProviderNPC<TSource3> frame3 = new FrameIValueProviderNPC<TSource3>();
                AsyncFrameIValueProviderRunner<TSource3> frame3Runner = new AsyncFrameIValueProviderRunner<TSource3>(_Source3Items, frame3);
                frame3Runner.Start();
                Thread.Sleep(0);
                var pipeline = _Builder(frame, frame2, frame3);
                var clientRunner = new AsynValueTransformationClientRunner<TResult>(
                    new ValueProviderClient<TResult>[] {
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                },
                    pipeline
                );
                var clientRunner2 = new AsynValueTransformationClientRunner<TResult>(
                    new ValueProviderClient<TResult>[] {
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                },
                    pipeline
                );
                clientRunner.Start();
                clientRunner2.Start();
                Thread.Sleep(1000);

                ValueProviderClientNPC<TResult>[] clients = new ValueProviderClientNPC<TResult>[5];

                for (int i = 0; i < clients.Length; ++i)
                {
                    clients[i] = new ValueProviderClientNPC<TResult>();
                    clients[i].Source = pipeline;
                    Thread.Sleep(500 / clients.Length);
                }

                frameRunner.Stop();
                frame2Runner.Stop();
                frame3Runner.Stop();
                clientRunner.Stop();
                clientRunner2.Stop();

                for (int i = 0; i < 50 && (frameRunner.IsRunning || frame2Runner.IsRunning || frame3Runner.IsRunning || clientRunner.IsRunning || clientRunner2.IsRunning); ++i)
                    Thread.Sleep(100);

                Assert.IsFalse(frameRunner.IsRunning, _Prefix + ": FrameRunner thread is still running on NPC test.");
                Assert.IsFalse(frame2Runner.IsRunning, _Prefix + ": Frame2Runner thread is still running on NPC test.");
                Assert.IsFalse(frame3Runner.IsRunning, _Prefix + ": Frame3Runner thread is still running on NPC test.");
                Assert.IsFalse(clientRunner.IsRunning, _Prefix + ": ClientRunner thread is still running on NPC test.");
                Assert.IsFalse(clientRunner2.IsRunning, _Prefix + ": ClientRunner2 thread is still running on NPC test.");

                for (int i = 0; i < clients.Length; ++i)
                    Assert.IsTrue(_Verifier(clients[i].Buffer), _Prefix + ": Client " + i.ToString() + " did not return the expected result on NPC test.");
            }
        }
    }

    internal class AsyncTestRunnerForValueTransformation<TSource, TSource2, TSource3, TSource4, TResult>
    {
        Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TSource4>, IValueProvider<TResult>> _Builder;
        TSource[] _SourceItems;
        TSource2[] _Source2Items;
        TSource3[] _Source3Items;
        TSource4[] _Source4Items;
        Func<TResult, bool> _Verifier;
        string _Prefix;

        internal AsyncTestRunnerForValueTransformation(TSource[] sourceItems, TSource2[] source2Items, TSource3[] source3Items, TSource4[] source4Items, Func<TResult, bool> verifier, Func<IValueProvider<TSource>, IValueProvider<TSource2>, IValueProvider<TSource3>, IValueProvider<TSource4>, IValueProvider<TResult>> builder, string prefix)
        {
            _SourceItems = sourceItems;
            _Source2Items = source2Items;
            _Source3Items = source3Items;
            _Source4Items = source4Items;
            _Builder = builder;
            _Verifier = verifier;
            _Prefix = prefix;
        }

        internal void Run()
        {
            {
                //NPC
                FrameIValueProviderNPC<TSource> frame = new FrameIValueProviderNPC<TSource>();
                AsyncFrameIValueProviderRunner<TSource> frameRunner = new AsyncFrameIValueProviderRunner<TSource>(_SourceItems, frame);
                frameRunner.Start();
                FrameIValueProviderNPC<TSource2> frame2 = new FrameIValueProviderNPC<TSource2>();
                AsyncFrameIValueProviderRunner<TSource2> frame2Runner = new AsyncFrameIValueProviderRunner<TSource2>(_Source2Items, frame2);
                frame2Runner.Start();
                FrameIValueProviderNPC<TSource3> frame3 = new FrameIValueProviderNPC<TSource3>();
                AsyncFrameIValueProviderRunner<TSource3> frame3Runner = new AsyncFrameIValueProviderRunner<TSource3>(_Source3Items, frame3);
                frame3Runner.Start();
                FrameIValueProviderNPC<TSource4> frame4 = new FrameIValueProviderNPC<TSource4>();
                AsyncFrameIValueProviderRunner<TSource4> frame4Runner = new AsyncFrameIValueProviderRunner<TSource4>(_Source4Items, frame4);
                frame4Runner.Start();
                Thread.Sleep(0);
                var pipeline = _Builder(frame, frame2, frame3, frame4);
                var clientRunner = new AsynValueTransformationClientRunner<TResult>(
                    new ValueProviderClient<TResult>[] {
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                },
                    pipeline
                );
                var clientRunner2 = new AsynValueTransformationClientRunner<TResult>(
                    new ValueProviderClient<TResult>[] {
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                    new ValueProviderClient<TResult>(),
                    new ValueProviderClientNPC<TResult>(),
                },
                    pipeline
                );
                clientRunner.Start();
                clientRunner2.Start();
                Thread.Sleep(1000);

                ValueProviderClientNPC<TResult>[] clients = new ValueProviderClientNPC<TResult>[5];

                for (int i = 0; i < clients.Length; ++i)
                {
                    clients[i] = new ValueProviderClientNPC<TResult>();
                    clients[i].Source = pipeline;
                    Thread.Sleep(500 / clients.Length);
                }

                frameRunner.Stop();
                frame2Runner.Stop();
                frame3Runner.Stop();
                frame4Runner.Stop();
                clientRunner.Stop();
                clientRunner2.Stop();

                for (int i = 0; i < 50 && (frameRunner.IsRunning || frame2Runner.IsRunning || frame3Runner.IsRunning || clientRunner.IsRunning || clientRunner2.IsRunning); ++i)
                    Thread.Sleep(100);

                Assert.IsFalse(frameRunner.IsRunning, _Prefix + ": FrameRunner thread is still running on NPC test.");
                Assert.IsFalse(frame2Runner.IsRunning, _Prefix + ": Frame2Runner thread is still running on NPC test.");
                Assert.IsFalse(frame3Runner.IsRunning, _Prefix + ": Frame3Runner thread is still running on NPC test.");
                Assert.IsFalse(frame4Runner.IsRunning, _Prefix + ": Frame4Runner thread is still running on NPC test.");
                Assert.IsFalse(clientRunner.IsRunning, _Prefix + ": ClientRunner thread is still running on NPC test.");
                Assert.IsFalse(clientRunner2.IsRunning, _Prefix + ": ClientRunner2 thread is still running on NPC test.");

                for (int i = 0; i < clients.Length; ++i)
                    Assert.IsTrue(_Verifier(clients[i].Buffer), _Prefix + ": Client " + i.ToString() + " did not return the expected result on NPC test.");
            }
        }
    }
}
