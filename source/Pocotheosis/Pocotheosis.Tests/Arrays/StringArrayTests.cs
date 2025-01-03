﻿using NUnit.Framework;
using GeneratedTestPocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    public class StringArrayTests : PocoTestFixture<StringArray>
    {
        static StringArray.Builder EmptyBuilder
        {
            get { return new StringArray.Builder(Enumerable.Empty<string>()); }
        }

        public StringArrayTests()
        {
            AddSample(
                new StringArray(Enumerable.Empty<string>()),
                "7f962dc8eb9ff177eb0a6f02349ec2883eaa1eac4ef3c55c346d2e3a508b0c4d",
                @"{
                    Strs = []
                }",
                @"{
                    ""Strs"": []
                }");
            AddSample(
                new StringArray(["alpha"]),
                "519565dcf6353cad06a6895d06f32f8f4155685d61434faad5f199e68c410d72",
                @"{
                    Strs = ['alpha']
                }",
                @"{
                    ""Strs"": [""alpha""]
                }");
            AddSample(
                new StringArray(["omega", "alpha"]),
                "0887efcd121b30dd297f5c376074727159810a1a8459a51aa3e869836e032051",
                @"{
                    Strs = ['omega', 'alpha']
                }",
                @"{
                    ""Strs"": [""omega"",""alpha""]
                }");

            AddInvalidConstructions(
                () => { var a = new StringArray(null); },
                () => { var a = new StringArray(new string[] { null }); },
                () => { var a = new StringArray.Builder(null); },
                () => { var a = new StringArray.Builder(new string[] { null }); },
                () => { EmptyBuilder.WithStrs(null); },
                () => { EmptyBuilder.WithStrs(new string[] { null }); },
                () => { EmptyBuilder.AppendStr(null); },
                () => { EmptyBuilder.InsertStrAt(0, null); },
                () => { EmptyBuilder.SetStr(0, null); }
            );
        }

        [Test]
        public override void Builder()
        {
            var A = "gamma";
            var B = "alpha";
            var C = "beta";

            var sut = new StringArray(new string[] { A, B, C }).ToBuilder();
            sut.SetStr(2, A);
            sut.RemoveStrAt(1);
            Assert.AreEqual(2, sut.NumStrs);
            Assert.AreEqual(A, sut.GetStr(1));
            Assert.AreEqual(new string[] { A, A }, sut.StrValues);

            sut.ClearStrs();
            sut.AppendStr(B);
            sut.InsertStrAt(0, C);
            sut.InsertStrAt(2, A);
            Assert.AreEqual(
                EmptyBuilder.WithStrs(new string[] { C, B, A }).Build(),
                sut.Build());
        }
    }
}
