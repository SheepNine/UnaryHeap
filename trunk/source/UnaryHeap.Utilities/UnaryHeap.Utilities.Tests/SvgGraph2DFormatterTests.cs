using System;
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

            new FileGraphRenderApp(filename).Run();

            Assert.Equal(
                File.ReadAllText(Path.ChangeExtension(filename, "expected.svg")),
                File.ReadAllText(Path.ChangeExtension(filename, "svg")));
        }

        public static IEnumerable<object[]> TestCaseData
        {
            get { return Directory.GetFiles(@"data\SvgGraph2DFormatterTests", "*.txt").Select(file => new object[] { file }); }
        }

        
        [Fact]
        public void SimpleArgumentExceptions()
        {
            var tempFile = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), "svg");
            File.WriteAllText(tempFile, "CONTENTS");

            try
            {
                Assert.Throws<ArgumentNullException>("inputJsonFile", () => { new FileGraphRenderApp(null); });
                Assert.Throws<ArgumentOutOfRangeException>("inputJsonFile", () => { new FileGraphRenderApp(string.Empty); });
                Assert.Throws<ArgumentException>("inputJsonFile", () => { new FileGraphRenderApp("non_existent.txt"); });
                Assert.Throws<ArgumentException>("inputJsonFile", () => { new FileGraphRenderApp(tempFile); });

                Assert.Throws<ArgumentNullException>("inputJsonFile", () => { new FileGraphRenderApp(null, "bacon.svg"); });
                Assert.Throws<ArgumentNullException>("outputSvgFile", () => { new FileGraphRenderApp("bacon.txt", null); });
                Assert.Throws<ArgumentOutOfRangeException>("inputJsonFile", () => { new FileGraphRenderApp(string.Empty, "bacon.svg"); });
                Assert.Throws<ArgumentOutOfRangeException>("outputSvgFile", () => { new FileGraphRenderApp("bacon.txt", string.Empty); });
                Assert.Throws<ArgumentException>("inputJsonFile", () => { new FileGraphRenderApp("non_existent.txt", "bacon.svg"); });
                Assert.Throws<ArgumentException>(() => { new FileGraphRenderApp(tempFile, tempFile); });
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}
