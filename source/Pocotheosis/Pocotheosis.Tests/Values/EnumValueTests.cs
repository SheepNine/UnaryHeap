using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    internal class EnumValueTests : PocoTestFixture<EnumValue>
    {
        public EnumValueTests()
        {
            AddSample(
                new EnumValue(Tru),
                "49e8e3297545c15ab6a79471a7a34d43e24a8f1cb25ea3d8417c61f699267a3f",
                @"{
                    Enum = True
                }",
                @"{
                    ""Enum"": ""True""
                }");
            AddSample(
                new EnumValue(Fls),
                "0434a06771d6640ade6d09ab492879836f5f16e2a3b808c0638644d6ff4af09d",
                @"{
                    Enum = False
                }",
                @"{
                    ""Enum"": ""False""
                }");
            AddSample(
                new EnumValue(FNF),
                "f45a081dbf759b5d70fabc74fee5718a00de30926cda0066e694557227019eac",
                @"{
                    Enum = FileNotFound
                }",
                @"{
                    ""Enum"": ""FileNotFound""
                }");

            NoInvalidConstructions();
        }

        [Test]
        public override void Builder()
        {
            var sut = new EnumValue(Fls).ToBuilder();
            Assert.AreEqual(Fls, sut.Enum);
            Assert.AreEqual(
                sut.WithEnum(Tru).Build(),
                new EnumValue.Builder(Tru).Build());
        }
    }
}
