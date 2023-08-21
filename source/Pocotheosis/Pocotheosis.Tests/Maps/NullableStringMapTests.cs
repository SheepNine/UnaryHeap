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
                "7ba37aa08ed85296a0b46d6dcb2bddda7000ad1696d27998a16a1e752dc75096",
                @"{
                    MaybeStrings = ()
                }",
                @"{
                    ""MaybeStrings"": []
                }");
            AddSample(
                new NullableStringMap(new Dataset() { { 4, "bacon" } }),
                "89775637108ae9b95ed59ea3b353f87434c639a0d1721404155b95273c97ccd2",
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
                "e3f9e9f724c3b1632a6e370f3b95352dafcbbe762aed9d893352408aee36f474",
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
                "3feff5c1c53ed32987d19deecd60d2b4a4c362256c0ab88ccf5f5066d75954c6",
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
                "7017db89709bf73851487110ea28218b0233d0756c5bbe93be0399bf2315746e",
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

            AddInvalidConstructions(
                () => { var a = new NullableStringMap(null); }
            );
        }
    }
}
