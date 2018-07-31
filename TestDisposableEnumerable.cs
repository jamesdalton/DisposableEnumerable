using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;

namespace DisposableEnumerable
{
    public class TestDisposableEnumerable
    {
        private Mock<IDisposable> mockReader;
        
        public TestDisposableEnumerable()
        {
            mockReader = new Mock<IDisposable>();
            mockReader.Setup(x => x.Dispose()).Verifiable();
        }

        private IEnumerable<int> IterateList()
        {
            using (var reader = mockReader.Object)
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }
        }

        private IEnumerable<int> IterateAndTrow()
        {
            using (var reader = mockReader.Object)
            {
                yield return 4;
                throw new Exception();
            }
        }

        [Fact]
        public void BasicForEachDisposes()
        {
            foreach (var x in IterateList())
            {
                Console.WriteLine(x);
            }
            mockReader.Verify(x => x.Dispose());
        }

        [Fact]
        public void BasicForEachWithExceptionDisposes()
        {
            try
            {
                foreach(var x in IterateList())
                {
                    throw new Exception();
                }
            }
            catch {}
            mockReader.Verify(x => x.Dispose());
        }

        [Fact]
        public void BasicForEachIteratorThrows()
        {
            try
            {
                foreach(var x in IterateAndTrow())
                {
                    Console.WriteLine(x);
                }
            }
            catch { }
            mockReader.Verify(x => x.Dispose());
        }

        [Fact]
        public void BasicForEachExitEarly()
        {
            foreach(var x in IterateList())
            {
                Console.WriteLine(x);
                break;
            }
            mockReader.Verify(x => x.Dispose());
        }

        [Fact]
        public void LinqSelect()
        {
            var x = IterateList().Select(y => y).ToList();
            mockReader.Verify(z => z.Dispose());
        }

        [Fact]
        public void LinqFirstOrDefault()
        {
            var x = IterateList().FirstOrDefault();
            mockReader.Verify(y => y.Dispose());
        }

        [Fact]
        public void LinqSelectAndThrow()
        {
            try
            {
                var x = IterateAndTrow().Select(y => y).ToList();
            }
            catch { }
            mockReader.Verify(z => z.Dispose());
        }
    }
}
