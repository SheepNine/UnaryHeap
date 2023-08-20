using Pocotheosis.Tests.Pocos;
using Dataset = System.Collections.Generic.Dictionary<int, string>;

namespace Pocotheosis.Tests.Maps
{
    internal class NullableStringMapTests : PocoTestFixture<NullableStringMap>
    {
        public NullableStringMapTests()
        {
            AddSample(
                new NullableStringMap(new Dataset()),
                "df3f619804a92fdb4057192dc43dd748ea778adc52bc498ce80524c014b81119",
                @"{
                    MaybeStrings = ()
                }",
                @"{
                    ""MaybeStrings"": []
                }");
            AddSample(
                new NullableStringMap(new Dataset() { { 4, "bacon" } }),
                "e5d0fc4d3593b4c3e21d7e994e7f8ef9227a712c7fdf3e1c07e87e125653f6c4",
                @"{
                    MaybeStrings = (
                        4 -> 'bacon'
                    )
                }",
                @"{
                    ""MaybeStrings"": [{
                        ""k"":4,
                        ""v"":""bacon""
                    }]
                }");
            AddSample(
                new NullableStringMap(new Dataset() { { 4, null } }),
                "783d1d1e90c93463bd3bf41ce62f3e9464f3df6ed293f072b3273b3416975ae1",
                @"{
                    MaybeStrings = (
                        4 -> null
                    )
                }",
                @"{
                    ""MaybeStrings"": [{
                        ""k"":4,
                        ""v"":null
                    }]
                }");
            AddSample(
                new NullableStringMap(new Dataset() { { 7, "eggs" }, { 6, "sausage" } }),
                "024fcf25d8fc84be07bf4a6f5a3c39615a560987d94e33c88f0df7457ae3a817",
                @"{
                    MaybeStrings = (
                        6 -> 'sausage',
                        7 -> 'eggs'
                    )
                }",
                @"{
                    ""MaybeStrings"": [{
                        ""k"":6,
                        ""v"":""sausage""
                    },{
                        ""k"":7,
                        ""v"":""eggs""
                    }]
                }");
            AddSample(
                new NullableStringMap(new Dataset() { { 7, null }, { 6, "sausage" } }),
                "24f5459af9979214fea97b4c5672862df72faae1be9bb1ce9917adcb05317760",
                @"{
                    MaybeStrings = (
                        6 -> 'sausage',
                        7 -> null
                    )
                }",
                @"{
                    ""MaybeStrings"": [{
                        ""k"":6,
                        ""v"":""sausage""
                    },{
                        ""k"":7,
                        ""v"":null
                    }]
                }");
        }
    }
}
