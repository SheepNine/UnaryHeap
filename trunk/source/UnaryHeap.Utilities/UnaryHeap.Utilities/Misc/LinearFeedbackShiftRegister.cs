using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnaryHeap.Utilities.Misc
{
    /// <summary>
    /// Represents a shift register whose feedback is a linear function (XOR) of a subset of the bits
    /// in its current value, referred to as the tap bits.
    /// </summary>
    public class LinearFeedbackShiftRegister
    {
        #region Member variables

        int[] taps;
        int highBit;
        ulong registerValue;

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the current value of the shift register.
        /// </summary>
        public ulong RegisterValue
        {
            get
            {
                return registerValue;
            }
            set
            {
                if (value == 0)
                    throw new ArgumentOutOfRangeException("value", "RegisterValue cannot be set to zero.");

                // --- If the 63rd bit is the high bit, then any
                // --- input is valid. Otherwise, check that the input
                // --- doesn't set any bits beyond the register size

                if (highBit < 63 && (value >> (highBit + 1)) > 0)
                    throw new ArgumentOutOfRangeException("value", "Input value is larger than the shift register");

                registerValue = value;
            }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the LinearFeedbackShiftRegister class.
        /// </summary>
        /// <param name="numBits">The size, in bits, of the shift register.</param>
        /// <param name="tappedBits">A binary value where a bit value of 1 means that the corresponding bit of the register will be tapped.</param>
        public LinearFeedbackShiftRegister(int numBits, ulong tappedBits)
            : this(numBits, TapsFromBits(tappedBits))
        {
        }

        static int[] TapsFromBits(ulong taps)
        {
            return Enumerable.Range(0, 64).Where(i => (taps & (1ul << i)) == (1ul << i)).ToArray();
        }


        /// <summary>
        /// Initializes a new instance of the LinearFeedbackShiftRegister class.
        /// </summary>
        /// <param name="numBits">The size, in bits, of the shift register.</param>
        /// <param name="tappedBits">An array of integers specifying which bits of the register will be tapped.</param>
        public LinearFeedbackShiftRegister(int numBits, int[] tappedBits)
        {
            if (numBits < 2 || numBits > 64)
                throw new ArgumentOutOfRangeException("numBits", "NumBits must be between 2 and 64 inclusive.");
            if (tappedBits == null)
                throw new ArgumentNullException("tappedBits");
            if (tappedBits.Length == 0)
                throw new ArgumentException("No taps specified.", "tappedBits");
            if (tappedBits.Length != tappedBits.Distinct().Count())
                throw new ArgumentException("Duplicate taps specified.", "tappedBits");
            foreach (var tappedBit in tappedBits)
                if (tappedBit < 0 || tappedBit >= numBits)
                    throw new ArgumentOutOfRangeException("tappedBits", string.Format("{0} is not between 0 and {1} inclusive.", tappedBit, numBits - 1));

            taps = tappedBits.ToArray(); // Make a copy so caller can't modify the array from underneath us
            highBit = numBits - 1;
            registerValue = 1;
        }

        #endregion


        #region Public methods

        /// <summary>
        /// Advances the shift register to its next state.
        /// </summary>
        public void Shift()
        {
            if (CurrentRegisterValueProducesFeedback)
                registerValue = (registerValue >> 1) | (1ul << (highBit));
            else
                registerValue = (registerValue >> 1);
        }

        bool CurrentRegisterValueProducesFeedback
        {
            get { return (taps.Where(tap => IsRegisterBitSet(tap)).Count() & 1) == 1; }
        }

        bool IsRegisterBitSet(int index)
        {
            var mask = 1ul << index;
            return ((registerValue & mask) == mask);
        }

        /// <summary>
        /// Determines how many times the shift register can be shifted before it returns to its current value.
        /// </summary>
        public ulong GetCycleLength()
        {
            var result = 0ul;
            IterateCycle(val => result++);
            return result;
        }

        /// <summary>
        /// Computes the bit pattern that is produced by one iteration of the current cycle.
        /// </summary>
        public string GetCyclePattern()
        {
            var result = new StringBuilder();
            IterateCycle(val => result.Append((val & 1ul) == 1ul ? '1' : '0'));
            return result.ToString();
        }

        /// <summary>
        /// Determines the state in the current cycle of the shift register with the smallest binary value.
        /// </summary>
        public ulong GetSmallestValueInCycle()
        {
            var result = ulong.MaxValue;
            IterateCycle(val => result = Math.Min(result, val));
            return result;
        }

        /// <summary>
        /// Computes the shift register values possible in the current cycle.
        /// </summary>
        public List<ulong> GetCycleValues()
        {
            var result = new List<ulong>();
            IterateCycle(val => result.Add(val));
            return result;
        }

        /// <summary>
        /// Iterate through the shift register values in the current cycle and pass them to the input callback.
        /// </summary>
        /// <param name="callback">The callback to invoke on each shift register value.</param>
        public void IterateCycle(Action<ulong> callback)
        {
            var startingValue = registerValue;

            do
            {
                callback(registerValue);
                Shift();
            }
            while (registerValue != startingValue);
        }

        #endregion
    }
}