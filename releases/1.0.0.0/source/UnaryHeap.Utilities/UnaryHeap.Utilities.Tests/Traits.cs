namespace UnaryHeap.Utilities.Tests
{
    /// <summary>
    /// Defines xUnit trait name/value pairs for the UnaryHeap codebase.
    /// </summary>
    public static class Traits
    {
        /// <summary>
        /// Defines a trait indicating how well done the code being tested is.
        /// </summary>
        public static class Status
        {
            /// <summary>
            /// The 'status' trait name.
            /// </summary>
            public const string Name = "status";

            /// <summary>
            /// <para>The code under test is considered stable. Tests marked as stable are not run during
            /// quick test builds, (only during full test builds), therefore:</para>
            /// <para>- this trait should only be added to a test once the code that it tests is not
            /// anticipated to change in the near future</para>
            /// <para>- this trait should be removed from a test that regularly breaks during active
            /// development</para>
            /// </summary>
            public const string Stable = "stable";
        }
    }
}
