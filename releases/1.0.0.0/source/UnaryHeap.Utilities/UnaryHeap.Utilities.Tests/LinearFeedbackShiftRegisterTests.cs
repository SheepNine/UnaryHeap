using UnaryHeap.Utilities.Misc;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class LinearFeedbackShiftRegisterTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Length31Cycle()
        {
            var sut = new LinearFeedbackShiftRegister(15, new[] { 0, 6 }) { RegisterValue = 0x737 };
            Assert.Equal("1110110011100001101010010001011", sut.GetCyclePattern());
        }
    }
}
