using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Apps;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class SvgGraph2DFormatterTests
    {
        [Theory]
        [MemberData("TestCaseData")]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
            get { return WrapInArray(Directory.GetFiles(@"data\SvgGraph2DFormatterTests", "*.txt")); }
        }

        private static IEnumerable<object[]> WrapInArray(string[] data)
        {
            return data.Select(datum => new object[] { datum });
        }


        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            var tempFile = Path.ChangeExtension(GetRandomFilename(), "svg");
            File.WriteAllText(tempFile, "CONTENTS");

            try
            {
                Assert.Throws<ArgumentNullException>("args", () => { GraphRendererApp.MainMethod(null); });

                Assert.Throws<ArgumentNullException>("inputJsonFile",
                    () => { new FileGraphRenderApp(null); });
                Assert.Throws<ArgumentOutOfRangeException>("inputJsonFile",
                    () => { new FileGraphRenderApp(string.Empty); });
                Assert.Throws<ArgumentException>("inputJsonFile",
                    () => { new FileGraphRenderApp("non_existent.txt"); });
                Assert.Throws<ArgumentException>("inputJsonFile",
                    () => { new FileGraphRenderApp(tempFile); });

                Assert.Throws<ArgumentNullException>("inputJsonFile",
                    () => { new FileGraphRenderApp(null, "bacon.svg"); });
                Assert.Throws<ArgumentNullException>("outputSvgFile",
                    () => { new FileGraphRenderApp("bacon.txt", null); });
                Assert.Throws<ArgumentOutOfRangeException>("inputJsonFile",
                    () => { new FileGraphRenderApp(string.Empty, "bacon.svg"); });
                Assert.Throws<ArgumentOutOfRangeException>("outputSvgFile",
                    () => { new FileGraphRenderApp("bacon.txt", string.Empty); });
                Assert.Throws<ArgumentException>("inputJsonFile",
                    () => { new FileGraphRenderApp("non_existent.txt", "bacon.svg"); });
                Assert.Throws<ArgumentException>(
                    () => { new FileGraphRenderApp(tempFile, tempFile); });

                Assert.Throws<ArgumentNullException>("graph",
                    () => { SvgGraph2DFormatter.Generate(null, new StringWriter()); });
                Assert.Throws<ArgumentNullException>("destination",
                    () => { SvgGraph2DFormatter.Generate(new Graph2D(false), null); });
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        static string GetRandomFilename()
        {
            return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }
    }
}
