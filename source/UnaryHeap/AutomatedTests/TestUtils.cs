using NUnit.Framework;
using System;
using System.Collections.Generic;

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
    }
}
