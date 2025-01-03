﻿using NUnit.Framework;
using GeneratedTestPocos;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    public class PrimitiveArrayTests: PocoTestFixture<PrimitiveArray>
    {
        static PrimitiveArray.Builder EmptyBuilder
        {
            get { return new PrimitiveArray.Builder(Enumerable.Empty<byte>()); }
        }

        public PrimitiveArrayTests()
        {
            AddSample(
                new PrimitiveArray(Enumerable.Empty<byte>()),
                "35be322d094f9d154a8aba4733b8497f180353bd7ae7b0a15f90b586b549f28b",
                @"{
                    Primitives = []
                }",
                @"{
                    ""Primitives"": []
                }");
            AddSample(
                new PrimitiveArray(new byte[] { 1 }),
                "86acb7e5dfa7228759e774443eedf64fbbace2ab942dde45340b356937ef4c0e",
                @"{
                    Primitives = [1]
                }",
                @"{
                    ""Primitives"": [1]
                }");
            AddSample(
                new PrimitiveArray(new byte[] { 205, 150, 79, 15 }),
                "3343ec2e0152367b5e0500fb683efe5884b026221e5eeeb85ef8d6e3fa11565e",
                @"{
                    Primitives = [205, 150, 79, 15]
                }",
                @"{
                    ""Primitives"": [205,150,79,15]
                }");

            AddInvalidConstructions(
                () => { var a = new PrimitiveArray(null); },
                () => { var a = new PrimitiveArray.Builder(null); },
                () => { EmptyBuilder.WithPrimitives(null); }
            );
        }

        [Test]
        public void CollectionWrapper()
        {
            var sut = new PrimitiveArray(new byte[] { 1, 3, 5 });
            Assert.AreEqual(3, sut.Primitives.Count);
            Assert.AreEqual(5, sut.Primitives[2]);
            Assert.AreEqual(new byte[] { 1, 3, 5}, sut.Primitives.OfType<byte>().ToArray());
        }

        [Test]
        public override void Builder()
        {
            byte A = 1;
            byte B = 5;
            byte C = 8;

            var sut = new PrimitiveArray(new[] { A, B, C }).ToBuilder();
            sut.SetPrimitive(2, A);
            sut.RemovePrimitiveAt(1);
            Assert.AreEqual(2, sut.NumPrimitives);
            Assert.AreEqual(A, sut.GetPrimitive(1));
            Assert.AreEqual(new[] { A, A }, sut.PrimitiveValues);

            sut.ClearPrimitives();
            sut.AppendPrimitive(B);
            sut.InsertPrimitiveAt(0, C);
            sut.InsertPrimitiveAt(2, A);
            Assert.AreEqual(
                EmptyBuilder.WithPrimitives(new[] { C, B, A }).Build(),
                sut.Build());
        }
    }
}
