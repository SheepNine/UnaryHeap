using GeneratedTestPocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocotheosis.Tests.Values
{
    internal class PolymorphicValueTests : PocoTestFixture<PolymorphicValue>
    {
        public PolymorphicValueTests()
        {
            AddSample(
                new PolymorphicValue(P(1)),
                "",
                @"",
                @"");

            AddSample(
                new PolymorphicValue(new EnumValue(FNF)),
                "",
                @"",
                @"");

            AddSample(
                new PolymorphicValue(new StringValue("seven")),
                "",
                @"",
                @"");

            AddSample(
                new PolymorphicValue(new NullableStringValue(null)),
                "",
                @"",
                @"");

            AddSample(
                new PolymorphicValue(new ClassValue(P(1))),
                "",
                @"",
                @"");

            AddSample(
                new PolymorphicValue(new NullableClassValue(null)),
                "",
                @"",
                @"");

            AddSample(
                new PolymorphicValue(new PolymorphicValue(P(2))),
                "",
                @"",
                @"");
        }
    }
}
