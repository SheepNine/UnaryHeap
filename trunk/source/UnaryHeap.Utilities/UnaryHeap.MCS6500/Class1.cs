using System;

namespace UnaryHeap.MCS6500
{
    public class CPU
    {
        BUS bus;
        byte A, X, Y, S;
        ushort PC;
        // negative, zero, carry, interrupt, decimal, overflow, break
        bool N, Z, C, I, D, V;

        public void DoInstruction()
        {
            byte opcode = Read_Immediate();

            switch (opcode)
            {
                case 0xA9: LDA(Read_Immediate); break;
                case 0xA5: LDA(Read_ZeroPage); break;
                case 0xB5: LDA(Read_ZeroPageXIndexed); break;
                case 0xAD: LDA(Read_Absolute); break;
                case 0xBD: LDA(Read_AbsoluteXIndexed); break;
                case 0xB9: LDA(Read_AbsoluteYIndexed); break;

                case 0xA2: LDX(Read_Immediate); break;
                case 0xA6: LDX(Read_ZeroPage); break;
                case 0xB6: LDX(Read_ZeroPageYIndexed); break;
                case 0xAE: LDX(Read_Absolute); break;
                case 0xBE: LDX(Read_AbsoluteYIndexed); break;

                case 0xA0: LDY(Read_Immediate); break;
                case 0xA4: LDY(Read_ZeroPage); break;
                case 0xB4: LDY(Read_ZeroPageXIndexed); break;
                case 0xAC: LDY(Read_Absolute); break;
                case 0xBC: LDY(Read_AbsoluteXIndexed); break;

                case 0x85: STA(Write_ZeroPage); break;
                case 0x95: STA(Write_ZeroPageXIndexed); break;
                case 0x8D: STA(Write_Absolute); break;
                case 0x9D: STA(Write_AbsoluteXIndexed); break;
                case 0x99: STA(Write_AbsoluteYIndexed); break;

                case 0x86: STX(Write_ZeroPage); break;
                case 0x96: STX(Write_ZeroPageYIndexed); break;
                case 0x8E: STX(Write_Absolute); break;

                case 0x84: STY(Write_ZeroPage); break;
                case 0x94: STY(Write_ZeroPageXIndexed); break;
                case 0x8C: STY(Write_Absolute); break;

                case 0xAA: TAX(); break;
                case 0x8A: TXA(); break;
                case 0xA8: TAY(); break;
                case 0x98: TYA(); break;

                case 0x18: CLC(); break;
                case 0x38: SEC(); break;
                case 0x58: CLI(); break;
                case 0x78: SEI(); break;
                case 0xB8: CLV(); break;

                case 0xD8: CLD(); break;
                case 0xF8: SED(); break;

                case 0x29: AND(Read_Immediate); break;
                case 0x25: AND(Read_ZeroPage); break;
                case 0x35: AND(Read_ZeroPageXIndexed); break;
                case 0x2D: AND(Read_Absolute); break;
                case 0x3D: AND(Read_AbsoluteXIndexed); break;
                case 0x39: AND(Read_AbsoluteYIndexed); break;

                case 0x09: ORA(Read_Immediate); break;
                case 0x05: ORA(Read_ZeroPage); break;
                case 0x15: ORA(Read_ZeroPageXIndexed); break;
                case 0x0D: ORA(Read_Absolute); break;
                case 0x1D: ORA(Read_AbsoluteXIndexed); break;
                case 0x19: ORA(Read_AbsoluteYIndexed); break;

                case 0x49: EOR(Read_Immediate); break;
                case 0x45: EOR(Read_ZeroPage); break;
                case 0x55: EOR(Read_ZeroPageXIndexed); break;
                case 0x4D: EOR(Read_Absolute); break;
                case 0x5D: EOR(Read_AbsoluteXIndexed); break;
                case 0x59: EOR(Read_AbsoluteYIndexed); break;

                case 0x24: BIT(Read_ZeroPage); break;
                case 0x2C: BIT(Read_Absolute); break;

                case 0xE8: INX(); break;
                case 0xCA: DEX(); break;
                case 0xC8: INY(); break;
                case 0x88: DEY(); break;

                case 0x90: BCC(); break;
                case 0xB0: BCS(); break;
                case 0xD0: BNE(); break;
                case 0xF0: BEQ(); break;
                case 0x10: BPL(); break;
                case 0x30: BMI(); break;
                case 0x50: BVC(); break;
                case 0x70: BVS(); break;

                case 0x69: ADC(Read_Immediate); break;
                case 0x65: ADC(Read_ZeroPage); break;
                case 0x75: ADC(Read_ZeroPageXIndexed); break;
                case 0x6D: ADC(Read_Absolute); break;
                case 0x7D: ADC(Read_AbsoluteXIndexed); break;
                case 0x79: ADC(Read_AbsoluteYIndexed); break;

                case 0xE9: SBC(Read_Immediate); break;
                case 0xE5: SBC(Read_ZeroPage); break;
                case 0xF5: SBC(Read_ZeroPageXIndexed); break;
                case 0xED: SBC(Read_Absolute); break;
                case 0xFD: SBC(Read_AbsoluteXIndexed); break;
                case 0xF9: SBC(Read_AbsoluteYIndexed); break;

                case 0xC9: CMP(Read_Immediate); break;
                case 0xC5: CMP(Read_ZeroPage); break;
                case 0xD5: CMP(Read_ZeroPageXIndexed); break;
                case 0xCD: CMP(Read_Absolute); break;
                case 0xDD: CMP(Read_AbsoluteXIndexed); break;
                case 0xD9: CMP(Read_AbsoluteYIndexed); break;

                case 0xE0: CPX(Read_Immediate); break;
                case 0xE4: CPX(Read_ZeroPage); break;
                case 0xEC: CPX(Read_Absolute); break;

                case 0xC0: CPY(Read_Immediate); break;
                case 0xC4: CPY(Read_ZeroPage); break;
                case 0xCC: CPY(Read_Absolute); break;

                case 0x2A: ReadWrite_Accumulator(ROL); break;
                case 0x26: ReadWrite_ZeroPage(ROL); break;
                case 0x36: ReadWrite_ZeroPageXIndexed(ROL); break;
                case 0x2E: ReadWrite_Absolute(ROL); break;
                case 0x3E: ReadWrite_AbsoluteXIndexed(ROL); break;

                case 0x6A: ReadWrite_Accumulator(ROR); break;
                case 0x66: ReadWrite_ZeroPage(ROR); break;
                case 0x76: ReadWrite_ZeroPageXIndexed(ROR); break;
                case 0x6E: ReadWrite_Absolute(ROR); break;
                case 0x7E: ReadWrite_AbsoluteXIndexed(ROR); break;

                case 0x4A: ReadWrite_Accumulator(LSR); break;
                case 0x46: ReadWrite_ZeroPage(LSR); break;
                case 0x56: ReadWrite_ZeroPageXIndexed(LSR); break;
                case 0x4E: ReadWrite_Absolute(LSR); break;
                case 0x5E: ReadWrite_AbsoluteXIndexed(LSR); break;

                case 0x0A: ReadWrite_Accumulator(ASL); break;
                case 0x06: ReadWrite_ZeroPage(ASL); break;
                case 0x16: ReadWrite_ZeroPageXIndexed(ASL); break;
                case 0x0E: ReadWrite_Absolute(ASL); break;
                case 0x1E: ReadWrite_AbsoluteXIndexed(ASL); break;
            }
        }

        void SEC() { C = true; }
        void SED() { D = true; }
        void SEI() { I = true; }
        void CLC() { C = false; }
        void CLD() { D = false; }
        void CLI() { I = false; }
        void CLV() { V = false; }

        void ADC(Func<byte> readMode)
        {
            // TODO: can this be done more cleanly?
            var B = readMode();
            var eightBits = A + B;
            var sevenBits = (A & 0x7F) + (B & 0x7F);

            if (C)
            {
                eightBits += 1;
                sevenBits += 1;
            }

            C = ((eightBits >> 8) == 1);
            V = ((eightBits >> 8) != (sevenBits >> 7));
            A = FlagSense((byte)eightBits);
        }

        void SBC(Func<byte> readMode)
        {
            // TODO: can this be done more cleanly?
            // TODO: is it even correct?
            var B = readMode();
            var eightBits = A - B;
            var sevenBits = (A & 0x7F) - (B & 0x7F);

            if (!C)
            {
                eightBits -= 1;
                sevenBits -= 1;
            }

            C = ((eightBits >> 8) == 1);
            V = ((eightBits >> 8) != (sevenBits >> 7));
            A = FlagSense((byte)eightBits);
        }

        void CMP(Func<byte> readMode) { Compare(A, readMode); }
        void CPX(Func<byte> readMode) { Compare(A, readMode); }
        void CPY(Func<byte> readMode) { Compare(A, readMode); }
        void Compare(byte a, Func<byte> readMode)
        {
            // TODO: can this be done more cleanly?
            // TODO: is it even correct?
            var b = readMode();
            var eightBits = a - b;
            var sevenBits = (a & 0x7F) - (b & 0x7F);

            if (!C)
            {
                eightBits -= 1;
                sevenBits -= 1;
            }

            C = ((eightBits >> 8) == 1);
            FlagSense((byte)eightBits);
        }

        void LDA(Func<byte> readMode) { A = FlagSense(readMode()); }
        void LDX(Func<byte> readMode) { X = FlagSense(readMode()); }
        void LDY(Func<byte> readMode) { Y = FlagSense(readMode()); }

        void STA(Action<byte> writeMode) { writeMode(A); }
        void STX(Action<byte> writeMode) { writeMode(X); }
        void STY(Action<byte> writeMode) { writeMode(Y); }

        void TAX() { Transfer(A, out X); }
        void TXA() { Transfer(X, out A); }
        void TAY() { Transfer(A, out Y); }
        void TYA() { Transfer(Y, out A); }
        void Transfer(byte src, out byte dest) { dest = FlagSense(src); }

        void AND(Func<byte> readMode) { A = FlagSense((byte)(A & readMode())); }
        void ORA(Func<byte> readMode) { A = FlagSense((byte)(A | readMode())); }
        void EOR(Func<byte> readMode) { A = FlagSense((byte)(A ^ readMode())); }
        void BIT(Func<byte> readMode)
        {
            var result = FlagSense((byte)(A & readMode()));
            V = (result & 0x40) == 0x40;
        }

        void INX() { Increment(ref X); }
        void INY() { Increment(ref X); }
        void Increment(ref byte register) { register = FlagSense(register == 0xFF ? (byte)0x00 : (byte)(register + 1)); }

        void DEX() { Increment(ref X); }
        void DEY() { Increment(ref X); }
        void Decrement(ref byte register) { register = FlagSense(register == 0x00 ? (byte)0xFF : (byte)(register - 1)); }

        void BCC() { Branch(!C); }
        void BCS() { Branch(C); }
        void BNE() { Branch(!Z); }
        void BEQ() { Branch(Z); }
        void BPL() { Branch(!N); }
        void BMI() { Branch(N); }
        void BVC() { Branch(!V); }
        void BVS() { Branch(V); }
        void Branch(bool branchTaken)
        {
            sbyte offset = (sbyte)Read_Immediate();
            if (branchTaken) { PC = (ushort)(PC + offset); }
        }

        void ASL()
        {

        }

        byte FlagSense(byte data)
        {
            N = (data > 0x7F);
            Z = (data == 0);
            return data;
        }

        byte Read_Immediate()
        {
            byte data = bus.Read(PC);
            PC += 1;
            return data;
        }

        byte Read_ZeroPage() { return Read_ZeroPageIndexed(0); }
        byte Read_ZeroPageXIndexed() { return Read_ZeroPageIndexed(X); }
        byte Read_ZeroPageYIndexed() { return Read_ZeroPageIndexed(Y); }
        byte Read_ZeroPageIndexed(byte indexValue)
        {
            byte addressLow = bus.Read(PC);
            PC += 1;
            return bus.Read((byte)(addressLow + indexValue), 0, indexValue);
        }

        byte Read_Absolute() { return Read_AbsoluteIndexed(0); }
        byte Read_AbsoluteXIndexed() { return Read_AbsoluteIndexed(X); }
        byte Read_AbsoluteYIndexed() { return Read_AbsoluteIndexed(Y); }
        byte Read_AbsoluteIndexed(byte indexValue)
        {
            byte addressLow = bus.Read(PC);
            PC += 1;
            byte addressHigh = bus.Read(PC);
            PC += 1;
            return bus.Read(addressLow, addressHigh, indexValue);
        }

        void Write_ZeroPage(byte data) { Write_ZeroPageIndexed(0, data); }
        void Write_ZeroPageXIndexed(byte data) { Write_ZeroPageIndexed(X, data); }
        void Write_ZeroPageYIndexed(byte data) { Write_ZeroPageIndexed(Y, data); }
        void Write_ZeroPageIndexed(byte indexValue, byte data)
        {
            byte addressLow = bus.Read(PC);
            PC += 1;
            bus.Write(addressLow, 0, indexValue, data);
        }

        void Write_Absolute(byte data) { Write_AbsoluteIndexed(0, data); }
        void Write_AbsoluteXIndexed(byte data) { Write_AbsoluteIndexed(X, data); }
        void Write_AbsoluteYIndexed(byte data) { Write_AbsoluteIndexed(Y, data); }
        void Write_AbsoluteIndexed(byte indexValue, byte data)
        {
            byte addressLow = bus.Read(PC);
            PC += 1;
            byte addressHigh = bus.Read(PC);
            PC += 1;
            bus.Write(addressLow, addressHigh, indexValue, data);
        }

        byte ASL(byte input)
        {
            C = (input & 0x80) == 0x80;
            return FlagSense((byte)(input << 1));
        }

        byte LSR(byte input)
        {
            C = (input & 0x1) == 0x1;
            return FlagSense((byte)(input >> 1)); // VERIFY: input zero-extended
        }

        byte ROL(byte input)
        {
            var carry = C ? 1 : 0;
            C = (input & 0x80) == 0x80;
            return FlagSense((byte)((input << 1) + carry));
        }

        byte ROR(byte input)
        {
            var carry = C ? 0x80 : 0;
            C = (input & 0x1) == 0x1;
            return FlagSense((byte)((input >> 1) + carry)); // VERIFY: input zero-extended
        }

        void ReadWrite_Accumulator(Func<byte, byte> instruction)
        {
            A = instruction(A);
        }

        void ReadWrite_ZeroPage(Func<byte, byte> instruction) { ReadWrite_ZeroPageIndexed(0, instruction); }
        void ReadWrite_ZeroPageXIndexed(Func<byte, byte> instruction) { ReadWrite_ZeroPageIndexed(X, instruction); }
        void ReadWrite_ZeroPageIndexed(byte indexValue, Func<byte, byte> instruction)
        {
            byte addressLow = bus.Read(PC);
            PC += 1;
            bus.Write(addressLow, 0, indexValue, instruction(bus.Read(addressLow, 0, indexValue)));
        }


        void ReadWrite_Absolute(Func<byte, byte> instruction) { ReadWrite_AbsoluteIndexed(0, instruction); }
        void ReadWrite_AbsoluteXIndexed(Func<byte, byte> instruction) { ReadWrite_AbsoluteIndexed(X, instruction); }
        void ReadWrite_AbsoluteIndexed(byte indexValue, Func<byte, byte> instruction)
        {
            byte addressLow = bus.Read(PC);
            PC += 1;
            byte addressHigh = bus.Read(PC);
            PC += 1;
            bus.Write(addressLow, addressHigh, indexValue, instruction(bus.Read(addressLow, addressHigh, indexValue)));
        }
    }

    public interface BUS
    {
        void Write(byte addressLow, byte addressHigh, byte index, byte data);
        byte Read(ushort address);
        byte Read(byte addressLow, byte addressHigh, byte index);
    }
}
