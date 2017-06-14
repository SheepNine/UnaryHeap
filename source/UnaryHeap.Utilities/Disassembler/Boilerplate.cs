using System;

namespace Disassembler
{
    abstract class InstructionSource
    {
        protected abstract string Nmemonic { get; }

        public Instruction Implied
        {
            get { return new RecognizedInstruction(Nmemonic, new ImpliedMode()); }
        }

        public Instruction Accumulator
        {
            get { return new RecognizedInstruction(Nmemonic, new AccumulatorMode()); }
        }

        public Instruction Immediate
        {
            get { return new RecognizedInstruction(Nmemonic, new ImmediateMode()); }
        }

        public Instruction Absolute
        {
            get { return new RecognizedInstruction(Nmemonic, new AbsoluteMode()); }
        }

        public Instruction AbsoluteXIndexed
        {
            get { return new RecognizedInstruction(Nmemonic, new AbsoluteXIndexedMode()); }
        }

        public Instruction AbsoluteYIndexed
        {
            get { return new RecognizedInstruction(Nmemonic, new AbsoluteYIndexedMode()); }
        }

        public Instruction ZeroPage
        {
            get { return new RecognizedInstruction(Nmemonic, new ZeroPageMode()); }
        }

        public Instruction ZeroPageXIndexed
        {
            get { return new RecognizedInstruction(Nmemonic, new ZeroPageXIndexedMode()); }
        }

        public Instruction ZeroPageYIndexed
        {
            get { return new RecognizedInstruction(Nmemonic, new ZeroPageYIndexedMode()); }
        }

        public Instruction IndirectYIndexed
        {
            get { return new RecognizedInstruction(Nmemonic, new IndirectYIndexedMode()); }
        }
    }

    class LDA : InstructionSource { protected override string Nmemonic { get { return "LDA"; } } }
    class STA : InstructionSource { protected override string Nmemonic { get { return "STA"; } } }
    class LDX : InstructionSource { protected override string Nmemonic { get { return "LDX"; } } }
    class STX : InstructionSource { protected override string Nmemonic { get { return "STX"; } } }
    class LDY : InstructionSource { protected override string Nmemonic { get { return "LDY"; } } }
    class STY : InstructionSource { protected override string Nmemonic { get { return "STY"; } } }
    class AND : InstructionSource { protected override string Nmemonic { get { return "AND"; } } }
    class ORA : InstructionSource { protected override string Nmemonic { get { return "ORA"; } } }
    class EOR : InstructionSource { protected override string Nmemonic { get { return "EOR"; } } }
    class LSR : InstructionSource { protected override string Nmemonic { get { return "LSR"; } } }
    class ROR : InstructionSource { protected override string Nmemonic { get { return "ROR"; } } }
    class ASL : InstructionSource { protected override string Nmemonic { get { return "ASL"; } } }
    class ROL : InstructionSource { protected override string Nmemonic { get { return "ROL"; } } }
    class INC : InstructionSource { protected override string Nmemonic { get { return "INC"; } } }
    class DEC : InstructionSource { protected override string Nmemonic { get { return "DEC"; } } }
    class ADC : InstructionSource { protected override string Nmemonic { get { return "ADC"; } } }
    class SBC : InstructionSource { protected override string Nmemonic { get { return "SBC"; } } }
    class CMP : InstructionSource { protected override string Nmemonic { get { return "CMP"; } } }
    class CPX : InstructionSource { protected override string Nmemonic { get { return "CPX"; } } }
    class CPY : InstructionSource { protected override string Nmemonic { get { return "CPY"; } } }

    class Implied
    {
        public static Instruction PHA
        {
            get { return new RecognizedInstruction("PHA", new ImpliedMode()); }
        }
        public static Instruction PLA
        {
            get { return new RecognizedInstruction("PLA", new ImpliedMode()); }
        }
        public static Instruction PHP
        {
            get { return new RecognizedInstruction("PHP", new ImpliedMode()); }
        }
        public static Instruction PLP
        {
            get { return new RecognizedInstruction("PLP", new ImpliedMode()); }
        }
        public static Instruction SEC
        {
            get { return new RecognizedInstruction("SEC", new ImpliedMode()); }
        }
        public static Instruction CLC
        {
            get { return new RecognizedInstruction("CLC", new ImpliedMode()); }
        }
        public static Instruction INX
        {
            get { return new RecognizedInstruction("INX", new ImpliedMode()); }
        }
        public static Instruction INY
        {
            get { return new RecognizedInstruction("INY", new ImpliedMode()); }
        }
        public static Instruction DEX
        {
            get { return new RecognizedInstruction("DEX", new ImpliedMode()); }
        }
        public static Instruction DEY
        {
            get { return new RecognizedInstruction("DEY", new ImpliedMode()); }
        }
        public static Instruction RTS
        {
            get { return new RecognizedInstruction("RTS", new ImpliedMode()); }
        }
        public static Instruction TXA
        {
            get { return new RecognizedInstruction("TXA", new ImpliedMode()); }
        }
        public static Instruction TAX
        {
            get { return new RecognizedInstruction("TAX", new ImpliedMode()); }
        }
        public static Instruction TYA
        {
            get { return new RecognizedInstruction("TYA", new ImpliedMode()); }
        }
        public static Instruction TAY
        {
            get { return new RecognizedInstruction("TAY", new ImpliedMode()); }
        }
        public static Instruction TSX
        {
            get { return new RecognizedInstruction("TSX", new ImpliedMode()); }
        }
        public static Instruction TXS
        {
            get { return new RecognizedInstruction("TXS", new ImpliedMode()); }
        }
        public static Instruction CLD
        {
            get { return new RecognizedInstruction("CLD", new ImpliedMode()); }
        }
        public static Instruction RTI
        {
            get { return new RecognizedInstruction("RTI", new ImpliedMode()); }
        }
        public static Instruction SEI
        {
            get { return new RecognizedInstruction("SEI", new ImpliedMode()); }
        }
    }

    class Branch
    {
        public static Instruction NE
        {
            get { return new RecognizedInstruction("BNE", new RelativeMode()); }
        }
        public static Instruction EQ
        {
            get { return new RecognizedInstruction("BEQ", new RelativeMode()); }
        }
        public static Instruction CC
        {
            get { return new RecognizedInstruction("BCC", new RelativeMode()); }
        }
        public static Instruction CS
        {
            get { return new RecognizedInstruction("BCS", new RelativeMode()); }
        }
        public static Instruction PL
        {
            get { return new RecognizedInstruction("BPL", new RelativeMode()); }
        }
        public static Instruction MI
        {
            get { return new RecognizedInstruction("BMI", new RelativeMode()); }
        }
        public static Instruction VC
        {
            get { return new RecognizedInstruction("BVC", new RelativeMode()); }
        }
    }


    class RecognizedInstruction : Instruction
    {
        string nmemonic;
        Mode mode;

        public string Nmemonic
        {
            get { return nmemonic; }
        }

        public Mode Mode
        {
            get { return mode; }
        }

        public RecognizedInstruction(string nmemonic, Mode mode)
        {
            this.nmemonic = nmemonic;
            this.mode = mode;
        }
    }

    class UnrecognizedInstruction : TwoByteOperandMode, Instruction
    {
        byte opcode;

        public UnrecognizedInstruction(byte opcode)
        {
            this.opcode = opcode;
        }

        public Mode Mode
        {
            get { return this; }
        }

        public string Nmemonic
        {
            get { return "???"; }
        }

        public override string FormatTwoOperands(byte operand1, byte operand2)
        {
            throw new NotImplementedException(string.Format(
                "Decode: {0:X2} {1:X2} {2:X2}", opcode, operand1, operand2));
        }
    }

    interface Instruction
    {
        string Nmemonic { get; }
        Mode Mode { get; }
    }

    interface Mode
    {
        int Length { get; }
        string FormatNoOperands();
        string FormatOneOperand(int baseAddress, byte operand);
        string FormatTwoOperands(byte operand1, byte operand2);
    }

    abstract class ZeroByteOperandMode : Mode
    {
        public int Length
        {
            get { return 0; }
        }

        public abstract string FormatNoOperands();

        public string FormatOneOperand(int baseAddress, byte operand)
        {
            throw new NotImplementedException();
        }

        public string FormatTwoOperands(byte operand1, byte operand2)
        {
            throw new NotImplementedException();
        }
    }

    abstract class OneByteOperandMode : Mode
    {
        public int Length
        {
            get { return 1; }
        }

        public string FormatNoOperands()
        {
            throw new InvalidOperationException();
        }

        public abstract string FormatOneOperand(int baseAddress, byte operand);

        public string FormatTwoOperands(byte operand1, byte operand2)
        {
            throw new InvalidOperationException();
        }
    }

    abstract class TwoByteOperandMode : Mode
    {
        public int Length
        {
            get { return 2; }
        }

        public string FormatNoOperands()
        {
            throw new InvalidOperationException();
        }

        public string FormatOneOperand(int baseAddress, byte operand)
        {
            throw new InvalidOperationException();
        }

        public abstract string FormatTwoOperands(byte operand1, byte operand2);
    }

    class ImpliedMode : ZeroByteOperandMode
    {
        public override string FormatNoOperands()
        {
            return string.Empty;
        }
    }

    class AccumulatorMode : ZeroByteOperandMode
    {
        public override string FormatNoOperands()
        {
            return "A";
        }
    }

    class AbsoluteMode : TwoByteOperandMode
    {
        public override string FormatTwoOperands(byte operand1, byte operand2)
        {
            return string.Format("${0:X2}{1:X2}", operand2, operand1);
        }
    }

    class AbsoluteXIndexedMode : TwoByteOperandMode
    {
        public override string FormatTwoOperands(byte operand1, byte operand2)
        {
            return string.Format("${0:X2}{1:X2},Y", operand2, operand1);
        }
    }

    class AbsoluteYIndexedMode : TwoByteOperandMode
    {
        public override string FormatTwoOperands(byte operand1, byte operand2)
        {
            return string.Format("${0:X2}{1:X2},Y", operand2, operand1);
        }
    }

    class ImmediateMode : OneByteOperandMode
    {
        public override string FormatOneOperand(int baseAddress, byte operand)
        {
            return string.Format("#${0:X2}", operand);
        }
    }

    class RelativeMode : OneByteOperandMode
    {
        public override string FormatOneOperand(int baseAddress, byte operand)
        {
            return string.Format("${0:X4}", baseAddress + (sbyte)operand + 2);
        }
    }

    class ZeroPageMode : OneByteOperandMode
    {
        public override string FormatOneOperand(int baseAddress, byte operand)
        {
            return string.Format("${0:X2}", operand);
        }
    }

    class ZeroPageXIndexedMode : OneByteOperandMode
    {
        public override string FormatOneOperand(int baseAddress, byte operand)
        {
            return string.Format("${0:X2},X", operand);
        }
    }

    class ZeroPageYIndexedMode : OneByteOperandMode
    {
        public override string FormatOneOperand(int baseAddress, byte operand)
        {
            return string.Format("${0:X2},Y", operand);
        }
    }

    class IndirectYIndexedMode : OneByteOperandMode
    {
        public override string FormatOneOperand(int baseAddress, byte operand)
        {
            return string.Format("(${0:X2}),Y", operand);
        }
    }

    class IndirectMode : TwoByteOperandMode
    {
        public override string FormatTwoOperands(byte operand1, byte operand2)
        {
            return string.Format("(${0:X2}{1:X2})", operand2, operand1);
        }
    }
}
