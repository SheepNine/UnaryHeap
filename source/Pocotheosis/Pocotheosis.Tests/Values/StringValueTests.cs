﻿using NUnit.Framework;
using GeneratedTestPocos;

namespace Pocotheosis.Tests.Values
{
    public class StringValueTests : PocoTestFixture<StringValue>
    {
        public StringValueTests()
        {
            AddSample(
                new StringValue(string.Empty),
                "98fdfcbc3fdc1804f196207e9b88616544580a6213fec7474b2a2ded56e6ca9e",
                @"{
                    Str = ''
                }",
                @"{
                    ""Str"": """"
                }");
            AddSample(
                new StringValue("a regular string"),
                "0b9103a8256f4e4866f7726a2cc8a2d5948e486dba0186c47d37ab4a69bd6ad4",
                @"{
                    Str = 'a regular string'
                }",
                @"{
                    ""Str"": ""a regular string""
                }");

            AddInvalidConstructions(
                () => { var a = new StringValue(null); },
                () => { var a = new StringValue.Builder(null); },
                () => { new StringValue.Builder("a").WithStr(null); },
                () => { new StringValue.Builder("a").Str = null; },
                () => { ReadTypedFromJson("{\"Str\":null}", false); }
            );
        }

        [Test]
        public override void Builder()
        {
            var sut = new StringValue("alpha").ToBuilder();
            Assert.AreEqual("alpha", sut.Str);
            sut.Str = "gamma";
            Assert.AreEqual("gamma", sut.Str);
            Assert.AreEqual(
                sut.WithStr("beta").Build(),
                new StringValue.Builder("beta").Build());
        }
    }
}
