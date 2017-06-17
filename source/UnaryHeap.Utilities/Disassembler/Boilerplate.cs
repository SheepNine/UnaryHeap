using System;

namespace Disassembler
{
    abstract class InstructionSource
    {
        protected abstract bool IsControlFlow { get; }

        protected abstract string Nmemonic { get; }

        public Instruction Implied
        {
            get { return new RecognizedInstruction(Nmemonic, new ImpliedMode(), false); }
        }

        public Instruction Accumulator
        {
            get { return new RecognizedInstruction(Nmemonic, new AccumulatorMode(), false); }
        }

        public Instruction Immediate
        {
            get { return new RecognizedInstruction(Nmemonic, new ImmediateMode(), false); }
        }

        public Instruction Absolute
        {
            get { return new RecognizedInstruction(Nmemonic, new AbsoluteMode(), IsControlFlow); }
        }

        public Instruction AbsoluteXIndexed
        {
            get { return new RecognizedInstruction(Nmemonic, new AbsoluteXIndexedMode(), false); }
        }

        public Instruction AbsoluteYIndexed
        {
            get { return new RecognizedInstruction(Nmemonic, new AbsoluteYIndexedMode(), false); }
        }

        public Instruction ZeroPage
        {
            get { return new RecognizedInstruction(Nmemonic, new ZeroPageMode(), false); }
        }

        public Instruction ZeroPageXIndexed
        {
            get { return new RecognizedInstruction(Nmemonic, new ZeroPageXIndexedMode(), false); }
        }

        public Instruction ZeroPageYIndexed
        {
            get { return new RecognizedInstruction(Nmemonic, new ZeroPageYIndexedMode(), false); }
        }

        public Instruction IndirectYIndexed
        {
            get { return new RecognizedInstruction(Nmemonic, new IndirectYIndexedMode(), false); }
        }

        public Instruction Indirect
        {
            get { return new RecognizedInstruction(Nmemonic, new IndirectMode(), false); }
        }

        public Instruction Relative
        {
            get { return new RecognizedInstruction(Nmemonic, new RelativeMode(), true); }
        }
    }

    class LDA : InstructionSource { protected override string Nmemonic { get { return "LDA"; } } protected override bool IsControlFlow { get { return false; } } }
    class STA : InstructionSource { protected override string Nmemonic { get { return "STA"; } } protected override bool IsControlFlow { get { return false; } } }
    class LDX : InstructionSource { protected override string Nmemonic { get { return "LDX"; } } protected override bool IsControlFlow { get { return false; } } }
    class STX : InstructionSource { protected override string Nmemonic { get { return "STX"; } } protected override bool IsControlFlow { get { return false; } } }
    class LDY : InstructionSource { protected override string Nmemonic { get { return "LDY"; } } protected override bool IsControlFlow { get { return false; } } }
    class STY : InstructionSource { protected override string Nmemonic { get { return "STY"; } } protected override bool IsControlFlow { get { return false; } } }
    class AND : InstructionSource { protected override string Nmemonic { get { return "AND"; } } protected override bool IsControlFlow { get { return false; } } }
    class ORA : InstructionSource { protected override string Nmemonic { get { return "ORA"; } } protected override bool IsControlFlow { get { return false; } } }
    class EOR : InstructionSource { protected override string Nmemonic { get { return "EOR"; } } protected override bool IsControlFlow { get { return false; } } }
    class LSR : InstructionSource { protected override string Nmemonic { get { return "LSR"; } } protected override bool IsControlFlow { get { return false; } } }
    class ROR : InstructionSource { protected override string Nmemonic { get { return "ROR"; } } protected override bool IsControlFlow { get { return false; } } }
    class ASL : InstructionSource { protected override string Nmemonic { get { return "ASL"; } } protected override bool IsControlFlow { get { return false; } } }
    class ROL : InstructionSource { protected override string Nmemonic { get { return "ROL"; } } protected override bool IsControlFlow { get { return false; } } }
    class INC : InstructionSource { protected override string Nmemonic { get { return "INC"; } } protected override bool IsControlFlow { get { return false; } } }
    class DEC : InstructionSource { protected override string Nmemonic { get { return "DEC"; } } protected override bool IsControlFlow { get { return false; } } }
    class ADC : InstructionSource { protected override string Nmemonic { get { return "ADC"; } } protected override bool IsControlFlow { get { return false; } } }
    class SBC : InstructionSource { protected override string Nmemonic { get { return "SBC"; } } protected override bool IsControlFlow { get { return false; } } }
    class CMP : InstructionSource { protected override string Nmemonic { get { return "CMP"; } } protected override bool IsControlFlow { get { return false; } } }
    class CPX : InstructionSource { protected override string Nmemonic { get { return "CPX"; } } protected override bool IsControlFlow { get { return false; } } }
    class CPY : InstructionSource { protected override string Nmemonic { get { return "CPY"; } } protected override bool IsControlFlow { get { return false; } } }
    class JMP : InstructionSource { protected override string Nmemonic { get { return "JMP"; } } protected override bool IsControlFlow { get { return true; } } }
    class JSR : InstructionSource { protected override string Nmemonic { get { return "JSR"; } } protected override bool IsControlFlow { get { return true; } } }
    class BNE : InstructionSource { protected override string Nmemonic { get { return "BNE"; } } protected override bool IsControlFlow { get { return false; } } }
    class BEQ : InstructionSource { protected override string Nmemonic { get { return "BEQ"; } } protected override bool IsControlFlow { get { return false; } } }
    class BMI : InstructionSource { protected override string Nmemonic { get { return "BMI"; } } protected override bool IsControlFlow { get { return false; } } }
    class BPL : InstructionSource { protected override string Nmemonic { get { return "BPL"; } } protected override bool IsControlFlow { get { return false; } } }
    class BCC : InstructionSource { protected override string Nmemonic { get { return "BCC"; } } protected override bool IsControlFlow { get { return false; } } }
    class BCS : InstructionSource { protected override string Nmemonic { get { return "BCS"; } } protected override bool IsControlFlow { get { return false; } } }
    class BVC : InstructionSource { protected override string Nmemonic { get { return "BVC"; } } protected override bool IsControlFlow { get { return false; } } }

    class Implied
    {
        public static Instruction PHA
        {
            get { return new RecognizedInstruction("PHA", new ImpliedMode(), false); }
        }
        public static Instruction PLA
        {
            get { return new RecognizedInstruction("PLA", new ImpliedMode(), false); }
        }
        public static Instruction PHP
        {
            get { return new RecognizedInstruction("PHP", new ImpliedMode(), false); }
        }
        public static Instruction PLP
        {
            get { return new RecognizedInstruction("PLP", new ImpliedMode(), false); }
        }
        public static Instruction SEC
        {
            get { return new RecognizedInstruction("SEC", new ImpliedMode(), false); }
        }
        public static Instruction CLC
        {
            get { return new RecognizedInstruction("CLC", new ImpliedMode(), false); }
        }
        public static Instruction INX
        {
            get { return new RecognizedInstruction("INX", new ImpliedMode(), false); }
        }
        public static Instruction INY
        {
            get { return new RecognizedInstruction("INY", new ImpliedMode(), false); }
        }
        public static Instruction DEX
        {
            get { return new RecognizedInstruction("DEX", new ImpliedMode(), false); }
        }
        public static Instruction DEY
        {
            get { return new RecognizedInstruction("DEY", new ImpliedMode(), false); }
        }
        public static Instruction RTS
        {
            get { return new RecognizedInstruction("RTS", new ImpliedMode(), false); }
        }
        public static Instruction TXA
        {
            get { return new RecognizedInstruction("TXA", new ImpliedMode(), false); }
        }
        public static Instruction TAX
        {
            get { return new RecognizedInstruction("TAX", new ImpliedMode(), false); }
        }
        public static Instruction TYA
        {
            get { return new RecognizedInstruction("TYA", new ImpliedMode(), false); }
        }
        public static Instruction TAY
        {
            get { return new RecognizedInstruction("TAY", new ImpliedMode(), false); }
        }
        public static Instruction TSX
        {
            get { return new RecognizedInstruction("TSX", new ImpliedMode(), false); }
        }
        public static Instruction TXS
        {
            get { return new RecognizedInstruction("TXS", new ImpliedMode(), false); }
        }
        public static Instruction CLD
        {
            get { return new RecognizedInstruction("CLD", new ImpliedMode(), false); }
        }
        public static Instruction RTI
        {
            get { return new RecognizedInstruction("RTI", new ImpliedMode(), false); }
        }
        public static Instruction SEI
        {
            get { return new RecognizedInstruction("SEI", new ImpliedMode(), false); }
        }
    }


    class RecognizedInstruction : Instruction
    {
        string nmemonic;
        Mode mode;
        bool isControlFlow;

        public string Nmemonic
        {
            get { return nmemonic; }
        }

        public Mode Mode
        {
            get { return mode; }
        }

        public bool IsControlFlow
        {
            get { return isControlFlow; }
        }

        public RecognizedInstruction(string nmemonic, Mode mode, bool isControlFlow)
        {
            this.nmemonic = nmemonic;
            this.mode = mode;
            this.isControlFlow = isControlFlow;
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

        public bool IsControlFlow
        {
            get { return false; }
        }

        public override string FormatTwoOperands(byte operand1, byte operand2)
        {
            throw new NotImplementedException(string.Format(
                "Decode: {0:X2} {1:X2} {2:X2}", opcode, operand1, operand2));
        }
    }

    interface Instruction
    {
        bool IsControlFlow { get; }
        string Nmemonic { get; }
        Mode Mode { get; }
    }

    interface Mode
    {
        int Length { get; }
        string FormatNoOperands();
        string FormatOneOperand(int baseAddress, byte operand);
        int GetAddress(int baseAddress, byte operand);
        string FormatTwoOperands(byte operand1, byte operand2);
        int GetAddress(byte operand1, byte operand2);
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

        public int GetAddress(int baseAddress, byte operand)
        {
            throw new NotImplementedException();
        }

        public string FormatTwoOperands(byte operand1, byte operand2)
        {
            throw new NotImplementedException();
        }

        public int GetAddress(byte operand1, byte operand2)
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

        public virtual int GetAddress(int baseAddress, byte operand)
        {
            throw new InvalidOperationException();
        }

        public string FormatTwoOperands(byte operand1, byte operand2)
        {
            throw new InvalidOperationException();
        }

        public int GetAddress(byte operand1, byte operand2)
        {
            throw new NotImplementedException();
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

        public int GetAddress(int baseAddress, byte operand)
        {
            throw new NotImplementedException();
        }

        public abstract string FormatTwoOperands(byte operand1, byte operand2);

        public int GetAddress(byte operand1, byte operand2)
        {
            return ((operand2 << 8) | operand1);
        }
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
            return string.Format("${0:X2}{1:X2},X", operand2, operand1);
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
            return string.Format("${0:X4}", GetAddress(baseAddress, operand));
        }

        public override int GetAddress(int baseAddress, byte operand)
        {
            return baseAddress + (sbyte)operand + 2;
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
