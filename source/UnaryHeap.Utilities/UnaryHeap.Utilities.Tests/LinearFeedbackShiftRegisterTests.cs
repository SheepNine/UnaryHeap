using UnaryHeap.Utilities.Misc;
using NUnit.Framework;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class LinearFeedbackShiftRegisterTests
    {
        [Test]
        public void Length31Cycle()
        {
            var sut = new LinearFeedbackShiftRegister(15, new[] { 0, 6 });
            sut.RegisterValue = 0x737;
            Assert.AreEqual("1110110011100001101010010001011", sut.GetCyclePattern());
        }
    }
}
