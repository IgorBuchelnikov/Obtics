using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics;

#if TVDP_UNITTESTING
using TvdP.UnitTesting;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


using Obtics.Values;

namespace ObticsUnitTest.Helpers
{
    internal abstract class ValueProviderTestSequenceRunner<TType,TOut>
    {
        public abstract TType[] SourceValueSequence { get; }

        public abstract IValueProvider<TOut> BuildPipeline( IValueProvider<TType> source );

        //CheckResult will always be called with the last value of the SourceValueSequence in the Source ValueProvider
        public abstract bool CheckResult(object p);

        public abstract string Prefix { get; }

        public void Run()
        {
            TType[] svs = SourceValueSequence;

            if (svs.Length > 0)
            {
                var frame0 = new FrameIValueProvider<TType>();
                frame0.SetValue(svs[svs.Length - 1]);
                frame0.IsReadOnly = true;

                var client = new ValueProviderClientNPC<TOut>();
                client.Source = BuildPipeline(frame0);

                if (!CheckResult(client.Buffer))
                    Assert.Fail(Prefix + " (0) after initial setup. The result value is not correct.");
            }

            for (int i = 1; i < svs.Length; ++i)
            {
                var frame = new FrameIValueProviderNPC<TType>();
                frame.SetValue(svs[svs.Length - i - 1]);
                frame.IsReadOnly = true;

                var client = new ValueProviderClientNPC<TOut>();
                client.Source = BuildPipeline(frame);

                for (int j = svs.Length - i; j < svs.Length; ++j)
                    frame.SetValue(svs[j]);

                if (!CheckResult(client.Buffer))
                    Assert.Fail(Prefix + " (" + i.ToString() + ") after " + i.ToString() + " changes of source value. The result value is not correct.");
            }
        }
    }

}
