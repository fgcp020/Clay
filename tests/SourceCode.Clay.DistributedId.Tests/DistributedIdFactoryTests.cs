using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace SourceCode.Clay.Distributed
{
    public class DistributedIdFactoryTests
    {
        [Fact]
        public void DistributedIdFactory_Properties()
        {
            DateTime epoch = DateTime.Now;
            var factory = new DistributedIdFactory(epoch, 123);
            Assert.Equal(epoch.ToUniversalTime(), factory.Epoch);
            Assert.Equal(123, factory.MachineId);
        }

        [Fact]
        public void DistributedIdFactory_Create_Competing()
        {
            const int threadCount = 10;
            const int iterations = 10000;

            var factory = new DistributedIdFactory(DateTime.Now, 0);
            var threads = new Thread[threadCount];
            var bigHs = new HashSet<DistributedId>(threadCount * iterations);
            Exception ex = null;

            using (var mut = new ManualResetEvent(false))
            {
                for (var i = 0; i < threads.Length; i++)
                {
                    threads[i] = new Thread(() =>
                    {
                        try
                        {
                            mut.WaitOne();
                            var hs = new HashSet<DistributedId>();
                            for (var j = 0; j < iterations; j++)
                                hs.Add(factory.Create());
                            lock (bigHs) bigHs.UnionWith(hs);
                        }
                        catch (Exception e)
                        {
                            ex = e;
                        }
                    });
                    threads[i].Start();
                }

                mut.Set();

                for (var i = 0; i < threads.Length; i++)
                {
                    threads[i].Join();
                }

                Assert.Null(ex);
                Assert.Equal(threadCount * iterations, bigHs.Count);
            }
        }

        [Fact]
        public void DistributedIdFactory_Create_Exhaust()
        {
            DateTime old = DateTime.Now.AddYears(-400);
            var factory = new DistributedIdFactory(old, 0);
            InvalidOperationException e = Assert.Throws<InvalidOperationException>(() => factory.Create());
            Assert.Equal("All distributed identifiers have been exhausted for the epoch.", e.Message);
        }

        [Fact]
        public void DistributedIdFactory_Create_InvalidMachineID()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DistributedIdFactory(DateTime.Now, 16384));
        }
    }
}
