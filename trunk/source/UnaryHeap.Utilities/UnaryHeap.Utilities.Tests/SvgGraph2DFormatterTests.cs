using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class SvgGraph2DFormatterTests
    {
        [Theory]
        [MemberData("TestCaseData")]
        public void TestCase(string filename)
        {
            new FileGraphRenderApp(filename, Path.ChangeExtension(filename, "actual.svg")).Run();

            Assert.Equal(
                File.ReadAllText(Path.ChangeExtension(filename, "expected.svg")),
                File.ReadAllText(Path.ChangeExtension(filename, "actual.svg")));
        }

        public static IEnumerable<object[]> TestCaseData
        {
            get { return Directory.GetFiles(@"data\SvgGraph2DFormatterTests", "*.txt").Select(file => new object[] { file }); }
        }
    }
}
