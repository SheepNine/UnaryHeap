using NUnit.Framework;
using System;
using System.IO;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    class TextWriterIndenterTests
    {
        [Test]
        public void TypicalExample()
        {
            DoTestCase("First\r\n\tSecond\r\n\t\tThird\r\n\tFourth\r\nFifth", sut =>
            {
                sut.WriteLine("First");
                sut.IncreaseIndent();
                sut.WriteLine("Second");
                sut.IncreaseIndent();
                sut.WriteLine("Third");
                sut.DecreaseIndent();
                sut.WriteLine("Fourth");
                sut.DecreaseIndent();
                sut.Write("Fifth");
            });
        }

        [Test]
        public void CustomIndentation()
        {
            DoTestCase("First\r\n    Second\r\n        Third\r\n    Fourth\r\nFifth", sut =>
            {
                sut.IndentString = "    ";
                sut.WriteLine("First");
                sut.IncreaseIndent();
                sut.WriteLine("Second");
                sut.IncreaseIndent();
                sut.WriteLine("Third");
                sut.DecreaseIndent();
                sut.WriteLine("Fourth");
                sut.DecreaseIndent();
                sut.Write("Fifth");
            });
        }

        [Test]
        public void NoIndentForNoContent()
        {
            DoTestCase("First\r\n\tSecond\r\n\r\n\tFourth\r\nFifth", sut =>
            {
                sut.WriteLine("First");
                sut.IncreaseIndent();
                sut.WriteLine("Second");
                sut.IncreaseIndent();
                sut.WriteLine("");
                sut.DecreaseIndent();
                sut.Write("Fourth");
                sut.WriteLine();
                sut.DecreaseIndent();
                sut.Write("Fifth");
            });
        }

        [Test]
        public void WriteOverloads()
        {
            DoTestCase("->1.12.23.3True161289def", sut =>
            {
                sut.IndentString = "->";
                sut.IncreaseIndent();

                sut.Write(1.1);
                sut.Write(2.2m);
                sut.Write(3.3f);
                sut.Write(true);
                sut.Write(16);
                sut.Write((uint)12);
                sut.Write((ulong)8);
                sut.Write(9L);
                sut.Write('d');
                sut.Write(new[] { 'e', 'f' });
            });
        }

        [Test]
        public void NothingDoneForEmptyData()
        {
            DoTestCase("", sut =>
            {
                sut.IndentString = "FAIL";
                sut.IncreaseIndent();

                sut.Write("");
                sut.Write(new char[] { });
            });
        }

        [Test]
        public void NothingDoneForEmptyData2()
        {
            DoTestCase("\r\n\r\n\r\n", sut =>
            {
                sut.IndentString = "FAIL";
                sut.IncreaseIndent();

                sut.WriteLine("");
                sut.WriteLine(new char[] { });
                sut.WriteLine();
            });
        }

        [Test]
        public void WriteLineOverloads()
        {
            DoTestCase(
                "|1.1\r\n|2.2\r\n|3.3\r\n|True\r\n|16\r\n|12\r\n|8\r\n|9\r\n|d\r\n|ef\r\n",
                sut =>
            {
                sut.IndentString = "|";
                sut.IncreaseIndent();

                sut.WriteLine(1.1);
                sut.WriteLine(2.2m);
                sut.WriteLine(3.3f);
                sut.WriteLine(true);
                sut.WriteLine(16);
                sut.WriteLine((uint)12);
                sut.WriteLine((ulong)8);
                sut.WriteLine(9L);
                sut.WriteLine('d');
                sut.WriteLine(new[] { 'e', 'f' });
            });
        }

        static void DoTestCase(string expected, Action<TextWriterIndenter> actions)
        {
            var buffer = new StringWriter();
            var sut = new TextWriterIndenter(buffer);
            actions(sut);
            sut.Flush();
            Assert.AreEqual(expected, buffer.ToString());
        }
    }
}
