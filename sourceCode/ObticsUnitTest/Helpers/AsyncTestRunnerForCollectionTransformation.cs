﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics.Collections;
using Obtics.Collections.Transformations;
using Obtics.Values;

namespace ObticsUnitTest.Helpers
{
    internal class AsyncTestRunnerForCollectionTransformation
    {
        public static void Run<TSource, TResult>(
            TSource[] sourceItems,
            TResult[] resultItems,
            Func<IVersionedEnumerable<TSource>, IEnumerable<TResult>> builder,
            string prefix
        )
        {
            new AsyncTestRunnerForCollectionTransformation<TSource, TResult>(sourceItems, res => Enumerable.SequenceEqual(resultItems, res), builder, prefix).Run();
        }

        public static void Run<TSource, TResult>(
            TSource[] sourceItems,
            Func<IEnumerable<TResult>, bool> verifier,
            Func<IVersionedEnumerable<TSource>, IEnumerable<TResult>> builder,
            string prefix
        )
        {
            new AsyncTestRunnerForCollectionTransformation<TSource, TResult>(sourceItems, verifier, builder, prefix).Run();
        }
    }

    internal class AsyncTestRunnerForCollectionTransformation<TSource,TResult>
    {
        Func<IVersionedEnumerable<TSource>, IEnumerable<TResult>> _Builder;
        TSource[] _SourceItems;
        Func<IEnumerable<TResult>, bool> _Verifier;
        string _Prefix;

        internal AsyncTestRunnerForCollectionTransformation(TSource[] sourceItems, Func<IEnumerable<TResult>, bool> verifier, Func<IVersionedEnumerable<TSource>, IEnumerable<TResult>> builder, string prefix)
        {
            _SourceItems = sourceItems;
            _Builder = builder;
            _Verifier = verifier;
            _Prefix = prefix;
        }

        internal void Run()
        {
            {
                //NCC
                FrameIEnumerableNPCNCC<TSource> frame = new FrameIEnumerableNPCNCC<TSource>();
                AsyncFrameIEnumerableRunner<TSource> frameRunner = new AsyncFrameIEnumerableRunner<TSource>(_SourceItems, frame);
                frameRunner.Start();
                Thread.Sleep(0);
                var pipeline = _Builder(frame);
                var clientRunner = new AsynCollectionTransformationClientRunner<TResult>(
                    new CollectionTransformationClient<TResult>[] {
                    new CollectionTransformationClientNPC<TResult>(),
                    new CollectionTransformationClientSNCC<TResult>(),
                    new CollectionTransformationClientNPC<TResult>(),
                    new CollectionTransformationClientSNCC<TResult>(),
                },
                    pipeline
                );
                var clientRunner2 = new AsynCollectionTransformationClientRunner<TResult>(
                    new CollectionTransformationClient<TResult>[] {
                    new CollectionTransformationClientNPC<TResult>(),
                    new CollectionTransformationClientSNCC<TResult>(),
                    new CollectionTransformationClientNPC<TResult>(),
                    new CollectionTransformationClientSNCC<TResult>(),
                },
                    pipeline
                );
                clientRunner.Start();
                clientRunner2.Start();
                Thread.Sleep(1000);

                CollectionTransformationClientSNCC<TResult>[] clients = new CollectionTransformationClientSNCC<TResult>[20];

                for (int i = 0; i < clients.Length; ++i)
                {
                    clients[i] = new CollectionTransformationClientSNCC<TResult>();
                    clients[i].Source = pipeline;
                    Thread.Sleep(500 / clients.Length);
                }

                frameRunner.Stop();
                clientRunner.Stop();
                clientRunner2.Stop();

                for (int i = 0; i < 200 && (frameRunner.IsRunning || clientRunner.IsRunning || clientRunner2.IsRunning); ++i)
                    Thread.Sleep(100);

                if (frameRunner.IsRunning)
                    Assert.Fail(_Prefix + ": FrameRunner thread is still running on NCC test.");

                Assert.IsFalse(clientRunner.IsRunning, _Prefix + ": ClientRunner thread is still running on NCC test.");
                Assert.IsFalse(clientRunner2.IsRunning, _Prefix + ": ClientRunner2 thread is still running on NCC test.");

                for (int i = 0; i < clients.Length; ++i)
                    Assert.IsTrue(_Verifier(clients[i].Buffer), _Prefix + ": Client " + i.ToString() + " did not return the expected result on NCC test.");
            }

            {
                var sourceList = new List<TSource>(_SourceItems);

                AsyncTestRunnerForValueTransformation.Run(
                    new IVersionedEnumerable<TSource>[] { sourceList.GetRange(0, 3).Patched(), sourceList.GetRange(3, 1).Patched(), null, sourceList.GetRange(0, 2).Patched() },
                    new IVersionedEnumerable<TSource>[] { null, sourceList.GetRange(sourceList.Count - 7, 3).Patched(), sourceList.GetRange(6, 1).Patched(), sourceList.GetRange(2, 3).Patched() },
                    new IVersionedEnumerable<TSource>[] { sourceList.GetRange(7, 3).Patched(), null, sourceList.GetRange(sourceList.Count - 3, 2).Patched(), null, sourceList.GetRange(5, 5).Patched() },
                    new IVersionedEnumerable<TSource>[] { sourceList.GetRange(sourceList.Count - 2, 1).Patched(), sourceList.GetRange(5, 2).Patched(), sourceList.GetRange(9, sourceList.Count - 9).Patched(), null },
                    _Verifier,
                    (frame1, frame2, frame3, frame4) => ValueProvider.Static(_Builder(ObservableEnumerable.Where(ObservableEnumerable.Static(new IValueProvider<IVersionedEnumerable<TSource>>[] { frame1, frame2, frame3, frame4 }).Select<IValueProvider<IVersionedEnumerable<TSource>>, IVersionedEnumerable<TSource>>(f => f), s => s != null).Concat().Patched())),
                    "Continuously changing source."
                );
            }
        }
    }
}
