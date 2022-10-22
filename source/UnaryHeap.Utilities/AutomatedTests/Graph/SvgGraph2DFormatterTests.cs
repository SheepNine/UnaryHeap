using GraphRenderer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnaryHeap.Graph.Tests
{
    [TestFixture]
    public class SvgGraph2DFormatterTests
    {
        [Test]
        public void TestCase([ValueSource("TestCaseData")]string filename)
        {
            new FileGraphRenderApp(filename, Path.ChangeExtension(filename, "actual.svg"))
                .Run();

            Assert.AreEqual(
                File.ReadAllText(Path.ChangeExtension(filename, "expected.svg")),
                File.ReadAllText(Path.ChangeExtension(filename, "actual.svg")));

            new FileGraphRenderApp(filename).Run();

            Assert.AreEqual(
                File.ReadAllText(Path.ChangeExtension(filename, "expected.svg")),
                File.ReadAllText(Path.ChangeExtension(filename, "svg")));
        }

        public static IEnumerable<string> TestCaseData
        {
            get { return Directory.GetFiles(@"data\SvgGraph2DFormatterTests", "*.txt"); }
        }


        [Test]
        public void SimpleArgumentExceptions()
        {
            var tempFile = Path.ChangeExtension(GetRandomFilename(), "svg");
            File.WriteAllText(tempFile, "CONTENTS");

            try
            {
                Assert.Throws<ArgumentNullException>(
                    () => { GraphRendererApp.MainMethod(null); });

                Assert.Throws<ArgumentNullException>(
                    () => { new FileGraphRenderApp(null); });
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => { new FileGraphRenderApp(string.Empty); });
                Assert.Throws<ArgumentException>(
                    () => { new FileGraphRenderApp("non_existent.txt"); });
                Assert.Throws<ArgumentException>(
                    () => { new FileGraphRenderApp(tempFile); });

                Assert.Throws<ArgumentNullException>(
                    () => { new FileGraphRenderApp(null, "bacon.svg"); });
                Assert.Throws<ArgumentNullException>(
                    () => { new FileGraphRenderApp("bacon.txt", null); });
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => { new FileGraphRenderApp(string.Empty, "bacon.svg"); });
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => { new FileGraphRenderApp("bacon.txt", string.Empty); });
                Assert.Throws<ArgumentException>(
                    () => { new FileGraphRenderApp("non_existent.txt", "bacon.svg"); });
                Assert.Throws<ArgumentException>(
                    () => { new FileGraphRenderApp(tempFile, tempFile); });

                Assert.Throws<ArgumentNullException>(
                    () => { SvgGraph2DFormatter.Generate(null, new StringWriter()); });
                Assert.Throws<ArgumentNullException>(
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
