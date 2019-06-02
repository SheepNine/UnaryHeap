[assembly:NUnit.Framework.Parallelizable(NUnit.Framework.ParallelScope.Fixtures)]

namespace UnaryHeap.Utilities.Tests
{
    /// <summary>
    /// Defines NUnit category names for the UnaryHeap codebase.
    /// </summary>
    public static class Traits
    {
        /// <summary>
        /// The Slow category is for tests that take > 1s to run and can be skipped
        /// during 'quick' builds.
        /// </summary>
        public const string Slow = "slow";

        /// <summary>
        /// The New category is for tests/suites being newly-created so that development
        /// can be made quick by only running the new tests.
        /// </summary>
        public const string New = "new";
    }
}
