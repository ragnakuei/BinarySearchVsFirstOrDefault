using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BinarySearchVsFirstOrDefault
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TestRunner>();
        }
    }

    [ClrJob, MonoJob, CoreJob] // 可以針對不同的 CLR 進行測試
    [MinColumn, MaxColumn]
    [MemoryDiagnoser]
    public class TestRunner
    {
        private readonly TestClass _test = new TestClass();

        public TestRunner()
        {
        }

        [Benchmark]
        public void BinarySearchFromIEnum() => _test.BinarySearchFromIEnum();

        [Benchmark]
        public void FirstOrDefault() => _test.FirstOrDefaultFromIEnum();

        [Benchmark]
        public void BinarySearchFromList() => _test.BinarySearchFromList();

        [Benchmark]
        public void FirstOrDefaultFromList() => _test.FirstOrDefaultFromList();
    }

    public class TestClass
    {
        private readonly IEnumerable<Test> _sampleDataIEnum
            = Enumerable.Range(0, 50)
                        .Select(i => new Test
                                     {
                                         Id   = i + 1
                                       , Name = ((char) (i + 97)).ToString()
                                     });

        private readonly List<Test> _sampleDataList
            = Enumerable.Range(0, 50)
                        .Select(i => new Test
                                     {
                                         Id = i + 1
                                        ,
                                         Name = ((char)(i + 97)).ToString()
                                     }).ToList();

        public Test BinarySearchFromIEnum()
        {
            var itemIndex = _sampleDataIEnum.ToList().BinarySearch(new Test {Id = 50}, new TestIdRelationalComparer());
            return _sampleDataIEnum.ToArray()[itemIndex];
        }

        public Test FirstOrDefaultFromIEnum()
        {
            return _sampleDataIEnum.FirstOrDefault(t=>t.Id == 50);
        }

        public Test BinarySearchFromList()
        {
            var itemIndex = _sampleDataList.BinarySearch(new Test { Id = 50 }, new TestIdRelationalComparer());
            return _sampleDataList[itemIndex];
        }

        public Test FirstOrDefaultFromList()
        {
            return _sampleDataList.FirstOrDefault(t => t.Id == 50);
        }
    }

    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

   public class TestIdRelationalComparer : IComparer<Test>
   {
       public int Compare(Test x, Test y)
       {
           if (ReferenceEquals(x, y)) return 0;
           if (ReferenceEquals(null, y)) return 1;
           if (ReferenceEquals(null, x)) return -1;
           return x.Id.CompareTo(y.Id);
       }
   }
}
