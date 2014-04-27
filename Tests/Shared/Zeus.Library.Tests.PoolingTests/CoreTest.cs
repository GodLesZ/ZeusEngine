using System;
using System.Threading;
using NUnit.Framework;
using Zeus.Library.Pooling;

namespace Zeus.Library.Tests.PoolingTests {

    [TestFixture]
    public class CoreTest {
        const int PoolSize = 5;

        /// <summary>
        /// Test a EagerPool with <see cref="EPoolItemAccessMode.Fifo"/>
        /// </summary>
        [Test]
        public void TestPoolingEagerAndFifo() {
            // FIX: Reset global count due other tests running before
            Foo.GlobalCount = 0;

            using (var pool = new EagerPool<Foo>(PoolSize, p => new Foo(p))) {
                // All foo's sould be created and raised the number
                Assert.True(Foo.GlobalCount == PoolSize);

                // Fetch 2 objects
                var foo = pool.Acquire();
                var foo2 = pool.Acquire();
                // They should be initialized
                Assert.True(foo != null);
                Assert.True(foo2 != null);

                // Realease them - also in the pool
                foo.Dispose();
                foo2.Dispose();
            }
        }

        /// <summary>
        /// Test a LazyPool with <see cref="EPoolItemAccessMode.Fifo"/>
        /// </summary>
        [Test]
        public void TestPoolingLazyAndFifo() {
            // FIX: Reset global count due other tests running before
            Foo.GlobalCount = 0;

            using (var pool = new LazyPool<Foo>(PoolSize, p => new Foo(p))) {
                // All objects should be lazy loaded
                Assert.True(Foo.GlobalCount == 0);
                Assert.True(pool.LazyLoadedItemCount == 0);

                // Fetch 2 objects
                var foo = pool.Acquire();
                var foo2 = pool.Acquire();
                // They should be initialized
                Assert.True(Foo.GlobalCount == 2);
                Assert.True(pool.LazyLoadedItemCount == 2);
                Assert.True(foo != null);
                Assert.True(foo2 != null);

                // Release them
                foo.Dispose();
                foo2.Dispose();
            }
        }







        public class Foo : IDisposable {
            /// <summary>
            /// Counts all created objects of type <see cref="Foo"/>
            /// </summary>
            public static int GlobalCount;

            protected readonly IPool<Foo> _objectPool;

            /// <summary>
            /// Gets the id of this foo
            /// </summary>
            public int Id {
                get;
                protected set;
            }
            

            public Foo(IPool<Foo> pool) {
                _objectPool = pool;

                Id = Interlocked.Increment(ref GlobalCount);
            }

            public void Dispose() {
                if (_objectPool.IsDisposed == false) {
                    _objectPool.Release(this);
                }

                Console.WriteLine("Goodbye from Foo #{0}", Id);
            }

            public void Test() {
                Console.WriteLine("Hello from Foo #{0}", Id);
            }
        }

    }


}
