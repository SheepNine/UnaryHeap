using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap
{
    public class TestUtils
    {
        public static void NullChecks(Dictionary<Type, IEnumerable<TestDelegate>> testCases)
        {
            foreach (var testCase in testCases)
                foreach (var action in testCase.Value)
                    Assert.Throws(testCase.Key, action);
        }

        public static IEnumerable<int[]> PermuteIndices(int count)
        {
            if (count == 1)
            {
                return new[] { new[] { 0 } };
            }
            else
            {
                return PermuteIndices(count - 1).SelectMany(p =>
                    Enumerable.Range(0, p.Length + 1).Select(i =>
                        p.Take(i).Append(count - 1).Concat(p.Skip(i)).ToArray()
                ));
            }
        }
    }
}
