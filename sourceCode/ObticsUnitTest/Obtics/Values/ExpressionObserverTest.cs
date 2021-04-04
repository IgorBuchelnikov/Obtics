using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Obtics.Values.Transformations;
using Obtics.Values;
using System.Linq.Expressions;

namespace ObticsUnitTest.Obtics.Values
{
    [TestClass]
    public class ExpressionObserverTest
    {
        void ValueProviderTest<TResult>( Expression<Func<TResult>> exp, string id)
        {
            Assert.AreEqual(
                exp.Compile()(),
                ExpressionObserver.Execute(exp).Value,
                id
            );
        }

        void SequenceTest<TResult>(Expression<Func<IEnumerable<TResult>>> exp, string id)
        {
            Assert.IsTrue(
                Enumerable.SequenceEqual(
                    exp.Compile()(),
                    ExpressionObserver.Execute(exp).Value
                ),
                id
            );
        }

        #region Aggregate

        [TestMethod]
        public void AggregateMapTest1()
        {
            ValueProviderTest(() => (new int[] { 1, 2, 3, 4 }).Aggregate((acc, v) => acc + v), "AggregateMapTest1");
        }

        [TestMethod]
        public void AggregateMapTest2()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3, 4 }).Aggregate(2, (acc, v) => acc + v),
                "AggregateMapTest2"
            );
        }

        [TestMethod]
        public void AggregateMapTest3()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3, 4 }).Aggregate(2, (acc, v) => acc + v, acc => acc + 10),
                "AggregateMapTest3"
            );
        }

        #endregion

        #region All

        [TestMethod]
        public void AllMapTest1()
        {
            ValueProviderTest(
                () => (new int[] { 2, 2, 2, 2 }).All(v => v == 2),
                "AllMapTest1"
            );
        }

        #endregion 
 
        #region Any

        [TestMethod]
        public void AnyMapTest1()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).Any(),
                "AnyMapTest1"
            );
        }
        
        [TestMethod]
        public void AnyMapTest2()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).Any( v => v == 4 ),
                "AnyMapTest2"
            );
        }
        
        #endregion 

        #region AsEnumerable


        [TestMethod]
        public void AsEnumerableMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3}).AsEnumerable(), 
                "AsEnumerableMapTest1" 
            );
        }

        #endregion 
 
        #region Average

        [TestMethod]
        public void AverageMapTest1()
        {
            ValueProviderTest(                
                () => (new decimal?[] { null, 1m, 2m, 3m }).Average(),
                "AverageMapTest1"
            );
        }

        [TestMethod]
        public void AverageMapTest2()
        {
            ValueProviderTest(                
                () => (new decimal[] { 1m, 2m, 3m }).Average(),
                "AverageMapTest2"
            );
        }

        [TestMethod]
        public void AverageMapTest3()
        {
            ValueProviderTest(                
                () => (new double?[] { null, 1.0, 2.0, 3.0 }).Average(),
                "AverageMapTest3"
            );
        }

        [TestMethod]
        public void AverageMapTest4()
        {
            ValueProviderTest(                
                () => (new double[] { 1.0, 2.0, 3.0 }).Average(),
                "AverageMapTest4"
            );
        }


        [TestMethod]
        public void AverageMapTest5()
        {
            ValueProviderTest(                
                () => (new float?[] { null, 1f, 2f, 3f }).Average(),
                "AverageMapTest5"
            );
        }

        [TestMethod]
        public void AverageMapTest6()
        {
            ValueProviderTest(                
                () => (new float[] { 1f, 2f, 3f }).Average(),
                "AverageMapTest6"
            );
        }


        [TestMethod]
        public void AverageMapTest7()
        {
            ValueProviderTest(                
                () => (new int?[] { null, 1, 2, 3 }).Average(),
                "AverageMapTest7"
            );
        }

        [TestMethod]
        public void AverageMapTest8()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).Average(),
                "AverageMapTest8"
            );
        }

        [TestMethod]
        public void AverageMapTest9()
        {
            ValueProviderTest(                
                () => (new long?[] { null, 1L, 2L, 3L }).Average(),
                "AverageMapTest9"
            );
        }

        [TestMethod]
        public void AverageMapTest10()
        {
            ValueProviderTest(                
                () => (new long[] { 1L, 2L, 3L }).Average(),
                "AverageMapTest10"
            );
        }




        
        [TestMethod]
        public void AverageMapTest11()
        {
            ValueProviderTest(                
                () => (new object[] { null, 1m, 2m, 3m }).Average( o => (decimal?)o ),
                "AverageMapTest11"
            );
        }

        [TestMethod]
        public void AverageMapTest12()
        {
            ValueProviderTest(                
                () => (new object[] { 1m, 2m, 3m }).Average( o => (decimal)o ),
                "AverageMapTest12"
            );
        }

        [TestMethod]
        public void AverageMapTest13()
        {
            ValueProviderTest(                
                () => (new object[] { null, 1.0, 2.0, 3.0 }).Average( o => (double?)o ),
                "AverageMapTest13"
            );
        }

        [TestMethod]
        public void AverageMapTest14()
        {
            ValueProviderTest(                
                () => (new object[] { 1.0, 2.0, 3.0 }).Average( o => (double)o ),
                "AverageMapTest14"
            );
        }


        [TestMethod]
        public void AverageMapTest15()
        {
            ValueProviderTest(                
                () => (new object[] { null, 1f, 2f, 3f }).Average( o => (float?)o ),
                "AverageMapTest15"
            );
        }

        [TestMethod]
        public void AverageMapTest16()
        {
            ValueProviderTest(                
                () => (new object[] { 1f, 2f, 3f }).Average( o => (float)o ),
                "AverageMapTest16"
            );
        }


        [TestMethod]
        public void AverageMapTest17()
        {
            ValueProviderTest(                
                () => (new object[] { null, 1, 2, 3 }).Average( o => (int?)o ),
                "AverageMapTest17"
            );
        }

        [TestMethod]
        public void AverageMapTest18()
        {
            ValueProviderTest(                
                () => (new object[] { 1, 2, 3 }).Average( o => (int)o ),
                "AverageMapTest18"
            );
        }

        [TestMethod]
        public void AverageMapTest19()
        {
            ValueProviderTest(                
                () => (new object[] { null, 1L, 2L, 3L }).Average( o => (long?)o ),
                "AverageMapTest19"
            );
        }

        [TestMethod]
        public void AverageMapTest20()
        {
            ValueProviderTest(                
                () => (new object[] { 1L, 2L, 3L }).Average( o => (long)o ),
                "AverageMapTest20"
            );
        }

        #endregion 
 
        #region Cast

        [TestMethod]
        public void CastMapTest1()
        {
            SequenceTest(
                () => (new object[] { 1, 2, 3}).Cast<int>(), 
                "CastMapTest1" 
            );
        }

        #endregion 

        #region Concat
        [TestMethod]
        public void ConcatMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3}).Concat(new int[] { 4, 5, 6}), 
                "ConcatMapTest1" 
            );
        }

        #endregion 
 
        #region Contains

        [TestMethod]
        public void ContainsMapTest1()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).Contains(2),
                "ContainsMapTest1"
            );
        }
                          
        [TestMethod]
        public void ContainsMapTest2()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).Contains(2, EqualityComparer<int>.Default),
                "ContainsMapTest2"
            );
        }
                          
        #endregion 
 
        #region Count
        [TestMethod]
        public void CountMapTest1()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).Count(),
                "CountMapTest1"
            );
        }
                          
        [TestMethod]
        public void CountMapTest2()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).Count( i => i < 3 ),
                "CountMapTest2"
            );
        }

        #endregion 
 
        #region DefaultIfEmpty

        [TestMethod]
        public void DefaultIfEmptyMapTest1()
        {
            SequenceTest(
                () => (new int[] {}).DefaultIfEmpty(), 
                "DefaultIfEmptyMapTest1" 
            );
        }

        [TestMethod]
        public void DefaultIfEmptyMapTest2()
        {
            SequenceTest(
                () => (new int[] {}).DefaultIfEmpty(7), 
                "DefaultIfEmptyMapTest2" 
            );
        }

        #endregion 
 
        #region Distinct

        [TestMethod]
        public void DistinctMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 1, 3 }).Distinct(), 
                "DistinctMapTest1" 
            );
        }

        [TestMethod]
        public void DistinctMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 1, 3 }).Distinct( EqualityComparer<int>.Default ), 
                "DistinctMapTest2" 
            );
        }

        #endregion 

        #region ElementAt

        [TestMethod]
        public void ElementAtTest1()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).ElementAt(1),
                "ElementAtTest1"
            );
        }

        #endregion 
 
        #region ElementAtOrDefault

        [TestMethod]
        public void ElementAtOrDefaultTest1()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).ElementAtOrDefault(4),
                "ElementAtOrDefaultTest1"
            );
        }

        #endregion 
 
        #region Empty

        #endregion 
 
        #region Except

        [TestMethod]
        public void ExceptMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).Except(new int[] { 2, 3 }), 
                "ExceptMapTest1" 
            );
        }

        [TestMethod]
        public void ExceptMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).Except(new int[] { 2, 3 }, EqualityComparer<int>.Default), 
                "ExceptMapTest2" 
            );
        }

        #endregion 
 
        #region First

        [TestMethod]
        public void FirstTest1()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).First(),
                "FirstTest1"
            );
        }

        [TestMethod]
        public void FirstTest2()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).First( i => i % 3 == 0),
                "FirstTest2"
            );
        }

        #endregion 
 
        #region FirstOrDefault

        [TestMethod]
        public void FirstOrDefaultTest1()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).FirstOrDefault(),
                "FirstOrDefaultTest1"
            );
        }

        [TestMethod]
        public void FirstOrDefaultTest2()
        {
            ValueProviderTest(                
                () => (new int[] { 1, 2, 3 }).FirstOrDefault( i => i % 7 == 0),
                "FirstOrDefaultTest2"
            );
        }

        #endregion 

        #region GroupBy


        [TestMethod]
        public void GroupByMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).GroupBy(i => i % 2).Select(g => g.Key),
                "GroupByMapTest1"
            );
        }

        [TestMethod]
        public void GroupByMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).GroupBy(i => i % 2, (i,g) => g.Count() + 1 ),
                "GroupByMapTest2"
            );
        }

        [TestMethod]
        public void GroupByMapTest3()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).GroupBy(i => i % 2, i => i * 2).Select(g => g.Key),
                "GroupByMapTest3"
            );
        }

        [TestMethod]
        public void GroupByMapTest4()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).GroupBy(i => i % 2, EqualityComparer<int>.Default).Select(g => g.Key),
                "GroupByMapTest4"
            );
        }

        [TestMethod]
        public void GroupByMapTest5()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).GroupBy(i => i % 2, (i, g) => g.Count() + 1, EqualityComparer<int>.Default),
                "GroupByMapTest5"
            );
        }

        [TestMethod]
        public void GroupByMapTest6()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).GroupBy(i => i % 2, i => i + 7, (i, g) => g.Count() + 1),
                "GroupByMapTest6"
            );
        }

        [TestMethod]
        public void GroupByMapTest7()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).GroupBy(i => i % 2, i => i * 2, EqualityComparer<int>.Default).Select(g => g.Key),
                "GroupByMapTest7"
            );
        }

        [TestMethod]
        public void GroupByMapTest8()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).GroupBy(i => i % 2, i => i + 7, (i, g) => g.Count() + 1, EqualityComparer<int>.Default),
                "GroupByMapTest8"
            );
        }




        #endregion 

        #region GroupJoin

        [TestMethod]
        public void GroupJoinMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).GroupJoin(new int[] { 1, 4, 6, 8 }, i => i % 3, i => i % 3, (o, ig) => ig.Sum() + o),
                "GroupJoinMapTest1"
            );
        }

        [TestMethod]
        public void GroupJoinMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).GroupJoin(new int[] { 1, 4, 6, 8 }, i => i % 3, i => i % 3, (o, ig) => ig.Sum() + o, EqualityComparer<int>.Default ),
                "GroupJoinMapTest2"
            );
        }
         
        #endregion 

        #region Intersect

        [TestMethod]
        public void IntersectMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).Intersect(new int[] { 2, 3 }),
                "IntersectMapTest1"
            );
        }

        [TestMethod]
        public void IntersectMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).Intersect(new int[] { 2, 3 }, EqualityComparer<int>.Default),
                "IntersectMapTest2"
            );
        }



        #endregion 

        #region Join

        [TestMethod]
        public void JoinMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).Join(new int[] { 1, 3, 5, 7 }, i => i % 2, o => o % 2, (o, i) => o + i),
                "JoinMapTest1"
            );
        }

        [TestMethod]
        public void JoinMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).Join(new int[] { 1, 3, 5, 7 }, i => i % 2, o => o % 2, (o, i) => o + i, EqualityComparer<int>.Default),
                "JoinMapTest2"
            );
        }

        #endregion

        #region Last

        [TestMethod]
        public void LastTest1()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).Last(),
                "LastTest1"
            );
        }

        [TestMethod]
        public void LastTest2()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).Last(i => i % 3 == 0),
                "LastTest2"
            );
        }

        #endregion

        #region LastOrDefault

        [TestMethod]
        public void LastOrDefaultTest1()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).LastOrDefault(),
                "LastOrDefaultTest1"
            );
        }

        [TestMethod]
        public void LastOrDefaultTest2()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).LastOrDefault(i => i % 7 == 0),
                "LastOrDefaultTest2"
            );
        }

        #endregion 

        #region LongCount
        [TestMethod]
        public void LongCountMapTest1()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).LongCount(),
                "LongCountMapTest1"
            );
        }

        [TestMethod]
        public void LongCountMapTest2()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).LongCount(i => i < 3),
                "LongCountMapTest2"
            );
        }

        #endregion 

        #region Max

        [TestMethod]
        public void MaxMapTest1()
        {
            ValueProviderTest(
                () => (new decimal?[] { null, 1m, 2m, 3m }).Max(),
                "MaxMapTest1"
            );
        }

        [TestMethod]
        public void MaxMapTest2()
        {
            ValueProviderTest(
                () => (new decimal[] { 1m, 2m, 3m }).Max(),
                "MaxMapTest2"
            );
        }

        [TestMethod]
        public void MaxMapTest3()
        {
            ValueProviderTest(
                () => (new double?[] { null, 1.0, 2.0, 3.0 }).Max(),
                "MaxMapTest3"
            );
        }

        [TestMethod]
        public void MaxMapTest4()
        {
            ValueProviderTest(
                () => (new double[] { 1.0, 2.0, 3.0 }).Max(),
                "MaxMapTest4"
            );
        }


        [TestMethod]
        public void MaxMapTest5()
        {
            ValueProviderTest(
                () => (new float?[] { null, 1f, 2f, 3f }).Max(),
                "MaxMapTest5"
            );
        }

        [TestMethod]
        public void MaxMapTest6()
        {
            ValueProviderTest(
                () => (new float[] { 1f, 2f, 3f }).Max(),
                "MaxMapTest6"
            );
        }


        [TestMethod]
        public void MaxMapTest7()
        {
            ValueProviderTest(
                () => (new int?[] { null, 1, 2, 3 }).Max(),
                "MaxMapTest7"
            );
        }

        [TestMethod]
        public void MaxMapTest8()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).Max(),
                "MaxMapTest8"
            );
        }

        [TestMethod]
        public void MaxMapTest9()
        {
            ValueProviderTest(
                () => (new long?[] { null, 1L, 2L, 3L }).Max(),
                "MaxMapTest9"
            );
        }

        [TestMethod]
        public void MaxMapTest10()
        {
            ValueProviderTest(
                () => (new long[] { 1L, 2L, 3L }).Max(),
                "MaxMapTest10"
            );
        }





        [TestMethod]
        public void MaxMapTest11()
        {
            ValueProviderTest(
                () => (new object[] { null, 1m, 2m, 3m }).Max(o => (decimal?)o),
                "MaxMapTest11"
            );
        }

        [TestMethod]
        public void MaxMapTest12()
        {
            ValueProviderTest(
                () => (new object[] { 1m, 2m, 3m }).Max(o => (decimal)o),
                "MaxMapTest12"
            );
        }

        [TestMethod]
        public void MaxMapTest13()
        {
            ValueProviderTest(
                () => (new object[] { null, 1.0, 2.0, 3.0 }).Max(o => (double?)o),
                "MaxMapTest13"
            );
        }

        [TestMethod]
        public void MaxMapTest14()
        {
            ValueProviderTest(
                () => (new object[] { 1.0, 2.0, 3.0 }).Max(o => (double)o),
                "MaxMapTest14"
            );
        }


        [TestMethod]
        public void MaxMapTest15()
        {
            ValueProviderTest(
                () => (new object[] { null, 1f, 2f, 3f }).Max(o => (float?)o),
                "MaxMapTest15"
            );
        }

        [TestMethod]
        public void MaxMapTest16()
        {
            ValueProviderTest(
                () => (new object[] { 1f, 2f, 3f }).Max(o => (float)o),
                "MaxMapTest16"
            );
        }


        [TestMethod]
        public void MaxMapTest17()
        {
            ValueProviderTest(
                () => (new object[] { null, 1, 2, 3 }).Max(o => (int?)o),
                "MaxMapTest17"
            );
        }

        [TestMethod]
        public void MaxMapTest18()
        {
            ValueProviderTest(
                () => (new object[] { 1, 2, 3 }).Max(o => (int)o),
                "MaxMapTest18"
            );
        }

        [TestMethod]
        public void MaxMapTest19()
        {
            ValueProviderTest(
                () => (new object[] { null, 1L, 2L, 3L }).Max(o => (long?)o),
                "MaxMapTest19"
            );
        }

        [TestMethod]
        public void MaxMapTest20()
        {
            ValueProviderTest(
                () => (new object[] { 1L, 2L, 3L }).Max(o => (long)o),
                "MaxMapTest20"
            );
        }

        [TestMethod]
        public void MaxMapTest21()
        {
            ValueProviderTest(
                () => (new object[] { 1L, 2L, 3L }).Max(),
                "MaxMapTest21"
            );
        }

        [TestMethod]
        public void MaxMapTest22()
        {
            ValueProviderTest(
                () => (new object[] { 1L, 2L, 3L }).Max(o => o),
                "MaxMapTest22"
            );
        }

        #endregion

        #region Min

        [TestMethod]
        public void MinMapTest1()
        {
            ValueProviderTest(
                () => (new decimal?[] { null, 1m, 2m, 3m }).Min(),
                "MinMapTest1"
            );
        }

        [TestMethod]
        public void MinMapTest2()
        {
            ValueProviderTest(
                () => (new decimal[] { 1m, 2m, 3m }).Min(),
                "MinMapTest2"
            );
        }

        [TestMethod]
        public void MinMapTest3()
        {
            ValueProviderTest(
                () => (new double?[] { null, 1.0, 2.0, 3.0 }).Min(),
                "MinMapTest3"
            );
        }

        [TestMethod]
        public void MinMapTest4()
        {
            ValueProviderTest(
                () => (new double[] { 1.0, 2.0, 3.0 }).Min(),
                "MinMapTest4"
            );
        }


        [TestMethod]
        public void MinMapTest5()
        {
            ValueProviderTest(
                () => (new float?[] { null, 1f, 2f, 3f }).Min(),
                "MinMapTest5"
            );
        }

        [TestMethod]
        public void MinMapTest6()
        {
            ValueProviderTest(
                () => (new float[] { 1f, 2f, 3f }).Min(),
                "MinMapTest6"
            );
        }


        [TestMethod]
        public void MinMapTest7()
        {
            ValueProviderTest(
                () => (new int?[] { null, 1, 2, 3 }).Min(),
                "MinMapTest7"
            );
        }

        [TestMethod]
        public void MinMapTest8()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).Min(),
                "MinMapTest8"
            );
        }

        [TestMethod]
        public void MinMapTest9()
        {
            ValueProviderTest(
                () => (new long?[] { null, 1L, 2L, 3L }).Min(),
                "MinMapTest9"
            );
        }

        [TestMethod]
        public void MinMapTest10()
        {
            ValueProviderTest(
                () => (new long[] { 1L, 2L, 3L }).Min(),
                "MinMapTest10"
            );
        }





        [TestMethod]
        public void MinMapTest11()
        {
            ValueProviderTest(
                () => (new object[] { null, 1m, 2m, 3m }).Min(o => (decimal?)o),
                "MinMapTest11"
            );
        }

        [TestMethod]
        public void MinMapTest12()
        {
            ValueProviderTest(
                () => (new object[] { 1m, 2m, 3m }).Min(o => (decimal)o),
                "MinMapTest12"
            );
        }

        [TestMethod]
        public void MinMapTest13()
        {
            ValueProviderTest(
                () => (new object[] { null, 1.0, 2.0, 3.0 }).Min(o => (double?)o),
                "MinMapTest13"
            );
        }

        [TestMethod]
        public void MinMapTest14()
        {
            ValueProviderTest(
                () => (new object[] { 1.0, 2.0, 3.0 }).Min(o => (double)o),
                "MinMapTest14"
            );
        }


        [TestMethod]
        public void MinMapTest15()
        {
            ValueProviderTest(
                () => (new object[] { null, 1f, 2f, 3f }).Min(o => (float?)o),
                "MinMapTest15"
            );
        }

        [TestMethod]
        public void MinMapTest16()
        {
            ValueProviderTest(
                () => (new object[] { 1f, 2f, 3f }).Min(o => (float)o),
                "MinMapTest16"
            );
        }


        [TestMethod]
        public void MinMapTest17()
        {
            ValueProviderTest(
                () => (new object[] { null, 1, 2, 3 }).Min(o => (int?)o),
                "MinMapTest17"
            );
        }

        [TestMethod]
        public void MinMapTest18()
        {
            ValueProviderTest(
                () => (new object[] { 1, 2, 3 }).Min(o => (int)o),
                "MinMapTest18"
            );
        }

        [TestMethod]
        public void MinMapTest19()
        {
            ValueProviderTest(
                () => (new object[] { null, 1L, 2L, 3L }).Min(o => (long?)o),
                "MinMapTest19"
            );
        }

        [TestMethod]
        public void MinMapTest20()
        {
            ValueProviderTest(
                () => (new object[] { 1L, 2L, 3L }).Min(o => (long)o),
                "MinMapTest20"
            );
        }

        [TestMethod]
        public void MinMapTest21()
        {
            ValueProviderTest(
                () => (new object[] { 1L, 2L, 3L }).Min(),
                "MinMapTest21"
            );
        }

        [TestMethod]
        public void MinMapTest22()
        {
            ValueProviderTest(
                () => (new object[] { 1L, 2L, 3L }).Min(o => o),
                "MinMapTest22"
            );
        }

        #endregion

        #region OfType

        [TestMethod]
        public void OfTypeMapTest1()
        {
            SequenceTest(
                () => (new object[] { 1, 2, 3 }).OfType<int>(),
                "OfTypeMapTest1"
            );
        }

        #endregion

        #region OrderBy

        [TestMethod]
        public void OrderByMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 4, 3 }).OrderBy(i => i),
                "OrderByMapTest1"
            );
        }

        [TestMethod]
        public void OrderByMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 4, 3 }).OrderBy(i => i, Comparer<int>.Default),
                "OrderByMapTest2"
            );
        }

        #endregion

        #region OrderByDescending

        [TestMethod]
        public void OrderByDescendingMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 4, 3 }).OrderByDescending(i => i),
                "OrderByDescendingMapTest1"
            );
        }

        [TestMethod]
        public void OrderByDescendingMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 4, 3 }).OrderByDescending(i => i, Comparer<int>.Default),
                "OrderByDescendingMapTest2"
            );
        }

        #endregion

        #region Range

        #endregion 
 
        #region Repeat

        #endregion 
 
        #region Reverse

        [TestMethod]
        public void ReverseMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).Reverse(),
                "ReverseMapTest1"
            );
        }

        #endregion 

        #region Select

        [TestMethod]
        public void SelectMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).Select( i => i + 3 ),
                "SelectMapTest1"
            );
        }
        
        [TestMethod]
        public void SelectMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).Select( (i,x) => i - x + 3 ),
                "SelectMapTest2"
            );
        }

        #endregion 

        #region SelectMany

        [TestMethod]
        public void SelectManyTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).SelectMany( (i,x) => Enumerable.Range(x,i) ),
                "SelectManyTest1"
            );
        }

        [TestMethod]
        public void SelectManyTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).SelectMany( i => Enumerable.Range(0,i) ),
                "SelectManyTest2"
            );
        }


        [TestMethod]
        public void SelectManyTest3()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).SelectMany( (i,x) => Enumerable.Range(x,i), (i,o) => i + o ),
                "SelectManyTest3"
            );
        }

        [TestMethod]
        public void SelectManyTest4()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).SelectMany( i => Enumerable.Range(0,i), (i,o) => i + o ),
                "SelectManyTest4"
            );
        }


        #endregion 

        #region SequenceEqual


        [TestMethod]
        public void SequenceEqualMapTest1()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).SequenceEqual(new int[] { 1, 2, 3 }),
                "SequenceEqualMapTest1"
            );
        }

        [TestMethod]
        public void SequenceEqualMapTest2()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).SequenceEqual(new int[] { 1, 2, 3 }, EqualityComparer<int>.Default),
                "SequenceEqualMapTest2"
            );
        }

        #endregion 
 
        #region Single

        [TestMethod]
        public void SingleTest1()
        {
            ValueProviderTest(
                () => (new int[] { 1 }).Single(),
                "SingleTest1"
            );
        }

        [TestMethod]
        public void SingleTest2()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).Single(i => i % 3 == 0),
                "SingleTest2"
            );
        }

        #endregion

        #region SingleOrDefault

        [TestMethod]
        public void SingleOrDefaultTest1()
        {
            ValueProviderTest(
                () => (new int[] { 1 }).SingleOrDefault(),
                "SingleOrDefaultTest1"
            );
        }

        [TestMethod]
        public void SingleOrDefaultTest2()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).SingleOrDefault(i => i % 3 == 0),
                "SingleOrDefaultTest2"
            );
        }

        #endregion
       
        #region Skip

        [TestMethod]
        public void SkipTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).Skip(2),
                "SkipTest1"
            );
        }

        #endregion 
 
        #region SkipWhile

        [TestMethod]
        public void SkipWhileTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).SkipWhile(i => i < 2),
                "SkipWhileTest1"
            );
        }

        [TestMethod]
        public void SkipWhileTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).SkipWhile( (i,x) => i+x < 3),
                "SkipWhileTest2"
            );
        }

        #endregion 

        #region Sum

        [TestMethod]
        public void SumMapTest1()
        {
            ValueProviderTest(
                () => (new decimal?[] { null, 1m, 2m, 3m }).Sum(),
                "SumMapTest1"
            );
        }

        [TestMethod]
        public void SumMapTest2()
        {
            ValueProviderTest(
                () => (new decimal[] { 1m, 2m, 3m }).Sum(),
                "SumMapTest2"
            );
        }

        [TestMethod]
        public void SumMapTest3()
        {
            ValueProviderTest(
                () => (new double?[] { null, 1.0, 2.0, 3.0 }).Sum(),
                "SumMapTest3"
            );
        }

        [TestMethod]
        public void SumMapTest4()
        {
            ValueProviderTest(
                () => (new double[] { 1.0, 2.0, 3.0 }).Sum(),
                "SumMapTest4"
            );
        }


        [TestMethod]
        public void SumMapTest5()
        {
            ValueProviderTest(
                () => (new float?[] { null, 1f, 2f, 3f }).Sum(),
                "SumMapTest5"
            );
        }

        [TestMethod]
        public void SumMapTest6()
        {
            ValueProviderTest(
                () => (new float[] { 1f, 2f, 3f }).Sum(),
                "SumMapTest6"
            );
        }


        [TestMethod]
        public void SumMapTest7()
        {
            ValueProviderTest(
                () => (new int?[] { null, 1, 2, 3 }).Sum(),
                "SumMapTest7"
            );
        }

        [TestMethod]
        public void SumMapTest8()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).Sum(),
                "SumMapTest8"
            );
        }

        [TestMethod]
        public void SumMapTest9()
        {
            ValueProviderTest(
                () => (new long?[] { null, 1L, 2L, 3L }).Sum(),
                "SumMapTest9"
            );
        }

        [TestMethod]
        public void SumMapTest10()
        {
            ValueProviderTest(
                () => (new long[] { 1L, 2L, 3L }).Sum(),
                "SumMapTest10"
            );
        }





        [TestMethod]
        public void SumMapTest11()
        {
            ValueProviderTest(
                () => (new object[] { null, 1m, 2m, 3m }).Sum(o => (decimal?)o),
                "SumMapTest11"
            );
        }

        [TestMethod]
        public void SumMapTest12()
        {
            ValueProviderTest(
                () => (new object[] { 1m, 2m, 3m }).Sum(o => (decimal)o),
                "SumMapTest12"
            );
        }

        [TestMethod]
        public void SumMapTest13()
        {
            ValueProviderTest(
                () => (new object[] { null, 1.0, 2.0, 3.0 }).Sum(o => (double?)o),
                "SumMapTest13"
            );
        }

        [TestMethod]
        public void SumMapTest14()
        {
            ValueProviderTest(
                () => (new object[] { 1.0, 2.0, 3.0 }).Sum(o => (double)o),
                "SumMapTest14"
            );
        }


        [TestMethod]
        public void SumMapTest15()
        {
            ValueProviderTest(
                () => (new object[] { null, 1f, 2f, 3f }).Sum(o => (float?)o),
                "SumMapTest15"
            );
        }

        [TestMethod]
        public void SumMapTest16()
        {
            ValueProviderTest(
                () => (new object[] { 1f, 2f, 3f }).Sum(o => (float)o),
                "SumMapTest16"
            );
        }


        [TestMethod]
        public void SumMapTest17()
        {
            ValueProviderTest(
                () => (new object[] { null, 1, 2, 3 }).Sum(o => (int?)o),
                "SumMapTest17"
            );
        }

        [TestMethod]
        public void SumMapTest18()
        {
            ValueProviderTest(
                () => (new object[] { 1, 2, 3 }).Sum(o => (int)o),
                "SumMapTest18"
            );
        }

        [TestMethod]
        public void SumMapTest19()
        {
            ValueProviderTest(
                () => (new object[] { null, 1L, 2L, 3L }).Sum(o => (long?)o),
                "SumMapTest19"
            );
        }

        [TestMethod]
        public void SumMapTest20()
        {
            ValueProviderTest(
                () => (new object[] { 1L, 2L, 3L }).Sum(o => (long)o),
                "SumMapTest20"
            );
        }

        #endregion 

        #region Take

        [TestMethod]
        public void TakeTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).Take(2),
                "TakeTest1"
            );
        }

        #endregion

        #region TakeWhile

        [TestMethod]
        public void TakeWhileTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).TakeWhile(i => i < 2),
                "TakeWhileTest1"
            );
        }

        [TestMethod]
        public void TakeWhileTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3 }).TakeWhile((i, x) => i + x < 3),
                "TakeWhileTest2"
            );
        }

        #endregion

        #region ThenBy

        [TestMethod]
        public void ThenByMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 4, 3 }).OrderBy(i => i % 2).ThenBy(i => i),
                "ThenByMapTest1"
            );
        }

        [TestMethod]
        public void ThenByMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 4, 3 }).OrderBy(i => i % 2).ThenBy(i => i, Comparer<int>.Default),
                "ThenByMapTest2"
            );
        }

        #endregion

        #region ThenByDescending

        [TestMethod]
        public void ThenByDescendingMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 4, 3 }).OrderBy(i => i % 2).ThenByDescending(i => i),
                "ThenByDescendingMapTest1"
            );
        }

        [TestMethod]
        public void ThenByDescendingMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 4, 3 }).OrderBy(i => i % 2).ThenByDescending(i => i, Comparer<int>.Default),
                "ThenByDescendingMapTest2"
            );
        }

        #endregion

        #region ToArray

        [TestMethod]
        public void ToArrayTest1()
        {
            ValueProviderTest(
                () => (new int[] { 1, 2, 3 }).ToArray().Sum(),
                "ToArrayTest1"
            );
        }

        #endregion
 
        #region ToDictionary

            //NOTE: These mappings are not fully compatible! It returns an IDictionary<> instead of a Dictionary<>
            //this means that a compile error will occur when the original Expression tree
            //contains a call to Enumerable.ToDictionary where a specifically Dictionary<> member is
            //being accessed on the result.

        [TestMethod]
        public void ToDictionaryTest1()
        {
            SequenceTest(
                () => (IDictionary<int,int>)(new int[] { 1, 2, 3, 4 }).ToDictionary( i => i ),
                "ToDictionaryTest1"
            );
        }

        [TestMethod]
        public void ToDictionaryTest2()
        {
            SequenceTest(
                () => (IDictionary<int,string>)(new int[] { 1, 2, 3, 4 }).ToDictionary( i => i, i => i.ToString() ),
                "ToDictionaryTest2"
            );
        }

        [TestMethod]
        public void ToDictionaryTest3()
        {
            SequenceTest(
                () => (IDictionary<int,int>)(new int[] { 1, 2, 3, 4 }).ToDictionary( i => i, EqualityComparer<int>.Default ),
                "ToDictionaryTest3"
            );
        }

        [TestMethod]
        public void ToDictionaryTest4()
        {
            SequenceTest(
                () => (IDictionary<int,string>)(new int[] { 1, 2, 3, 4 }).ToDictionary( i => i, i => i.ToString(), EqualityComparer<int>.Default ),
                "ToDictionaryTest4"
            );
        }

        #endregion

        #region ToList
            //NOTE: This mapping is not fully compatible! It returns an IList<> instead of a List<>
            //this means that a compile error will occur when the original Expression tree
            //contains a call to Enumerable.ToList where a specifically List<> member will
            //be accessed on the result.
        [TestMethod]
        public void ToListTest1()
        {
            SequenceTest(
                () => (IList<int>)(new int[] { 1, 2, 3, 4 }).ToList(),
                "ToListTest1"
            );
        }

        #endregion

        #region ToLookup

        [TestMethod]
        public void ToLookupTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).ToLookup(i => i).Select(g => g.Key),
                "ToLookupTest1"
            );
        }

        [TestMethod]
        public void ToLookupTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).ToLookup(i => i, i => i.ToString()).Select(g => g.Key),
                "ToLookupTest2"
            );
        }

        [TestMethod]
        public void ToLookupTest3()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).ToLookup(i => i, EqualityComparer<int>.Default).Select(g => g.Key),
                "ToLookupTest3"
            );
        }

        [TestMethod]
        public void ToLookupTest4()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).ToLookup(i => i, i => i.ToString(), EqualityComparer<int>.Default).Select(g => g.Key),
                "ToLookupTest4"
            );
        }

        #endregion

        #region Union

        [TestMethod]
        public void UnionMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).Union(new int[] { 2, 3 }),
                "UnionMapTest1"
            );
        }

        [TestMethod]
        public void UnionMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).Union(new int[] { 2, 3 }, EqualityComparer<int>.Default),
                "UnionMapTest2"
            );
        }

        #endregion 

        #region Where

        [TestMethod]
        public void WhereMapTest1()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).Where(i => i % 3 == 1),
                "WhereMapTest1"
            );
        }

        [TestMethod]
        public void WhereMapTest2()
        {
            SequenceTest(
                () => (new int[] { 1, 2, 3, 4 }).Where( (i,x) => (i + x) % 3 == 1),
                "WhereMapTest2"
            );
        }

        #endregion
    }
}
