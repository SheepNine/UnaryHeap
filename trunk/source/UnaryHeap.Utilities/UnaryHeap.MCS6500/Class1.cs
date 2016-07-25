﻿using System;
using System.Text;

namespace UnaryHeap.MCS6500
{
    public class CPU
    {
        //$FFFA-$FFFB = NMI vector
        //$FFFC-$FFFD = Reset vector
        //$FFFE-$FFFF = IRQ/BRK vector

        BUS bus;
        byte A, X, Y, S;
        ushort PC;
        // negative, zero, carry, interrupt, decimal, overflow, break
        bool N, Z, C, I, D, V;

        public CPU(BUS bus)
        {
            this.bus = bus;
        }

        public void PowerOn()
        {
            PC = _MakeAddress(bus.Read(0xFFFC), bus.Read(0xFFFD));

            int i = 0;
            while (true)
            {
                DoInstruction();
                i += 1;
                if (i == 200000)
                    throw new NotImplementedException("Execution count exceeded");
            }
        }

        public void DoInstruction()
        {
            Broadcast(EchoNextInstruction(), EmitCurrentState());

            byte opcode = Read_Immediate();

            switch (opcode)
            {
                case 0xA9: LDA(Read_Immediate); break;
                case 0xA5: LDA(Read_ZeroPage); break;
                case 0xB5: LDA(Read_ZeroPageXIndexed); break;
                case 0xAD: LDA(Read_Absolute); break;
                case 0xBD: LDA(Read_AbsoluteXIndexed); break;
                case 0xB9: LDA(Read_AbsoluteYIndexed); break;
                case 0xA1: LDA(Read_IndexedIndirect); break;
                case 0xB1: LDA(Read_IndirectIndexed); break;

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
                case 0x81: STA(Write_IndexedIndirect); break;
                case 0x91: STA(Write_IndirectIndexed); break;

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
                case 0x21: AND(Read_IndexedIndirect); break;
                case 0x31: AND(Read_IndirectIndexed); break;

                case 0x09: ORA(Read_Immediate); break;
                case 0x05: ORA(Read_ZeroPage); break;
                case 0x15: ORA(Read_ZeroPageXIndexed); break;
                case 0x0D: ORA(Read_Absolute); break;
                case 0x1D: ORA(Read_AbsoluteXIndexed); break;
                case 0x19: ORA(Read_AbsoluteYIndexed); break;
                case 0x01: ORA(Read_IndexedIndirect); break;
                case 0x11: ORA(Read_IndirectIndexed); break;

                case 0x49: EOR(Read_Immediate); break;
                case 0x45: EOR(Read_ZeroPage); break;
                case 0x55: EOR(Read_ZeroPageXIndexed); break;
                case 0x4D: EOR(Read_Absolute); break;
                case 0x5D: EOR(Read_AbsoluteXIndexed); break;
                case 0x59: EOR(Read_AbsoluteYIndexed); break;
                case 0x41: EOR(Read_IndexedIndirect); break;
                case 0x51: EOR(Read_IndirectIndexed); break;

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
                case 0x61: ADC(Read_IndexedIndirect); break;
                case 0x71: ADC(Read_IndirectIndexed); break;

                case 0xE9: SBC(Read_Immediate); break;
                case 0xE5: SBC(Read_ZeroPage); break;
                case 0xF5: SBC(Read_ZeroPageXIndexed); break;
                case 0xED: SBC(Read_Absolute); break;
                case 0xFD: SBC(Read_AbsoluteXIndexed); break;
                case 0xF9: SBC(Read_AbsoluteYIndexed); break;
                case 0xE1: SBC(Read_IndexedIndirect); break;
                case 0xF1: SBC(Read_IndirectIndexed); break;

                case 0xC9: CMP(Read_Immediate); break;
                case 0xC5: CMP(Read_ZeroPage); break;
                case 0xD5: CMP(Read_ZeroPageXIndexed); break;
                case 0xCD: CMP(Read_Absolute); break;
                case 0xDD: CMP(Read_AbsoluteXIndexed); break;
                case 0xD9: CMP(Read_AbsoluteYIndexed); break;
                case 0xC1: CMP(Read_IndexedIndirect); break;
                case 0xD1: CMP(Read_IndirectIndexed); break;

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

                case 0xC6: ReadWrite_ZeroPage(DEC); break;
                case 0xD6: ReadWrite_ZeroPageXIndexed(DEC); break;
                case 0xCE: ReadWrite_Absolute(DEC); break;
                case 0xDE: ReadWrite_AbsoluteXIndexed(DEC); break;

                case 0xE6: ReadWrite_ZeroPage(INC); break;
                case 0xF6: ReadWrite_ZeroPageXIndexed(INC); break;
                case 0xEE: ReadWrite_Absolute(INC); break;
                case 0xFE: ReadWrite_AbsoluteXIndexed(INC); break;

                case 0x9A: TXS(); break;
                case 0xBA: TSX(); break;

                case 0x4C: JMP_Abs(); break;
                case 0x6C: JMP_Indirect(); break;

                case 0xEA: NOP(); break;
                case 0x20: JSR(); break;
                case 0x60: RTS(); break;

                case 0x00: BRK(); break;
                case 0x40: RTI(); break;

                case 0x48: PHA(); break;
                case 0x68: PLA(); break;
                case 0x08: PHP(); break;
                case 0x28: PLP(); break;

                default:
                    throw new ApplicationException(
                        "Unrecognized opcode 0x" + opcode.ToString("X2"));
            }
        }

        public class StepEventArgs
        {
            public string NextInstruction { get; private set; }
            public string CurrentState { get; private set; }

            public StepEventArgs(string nextInstruction, string currentState)
            {
                NextInstruction = nextInstruction;
                CurrentState = currentState;
            }
        }

        public event EventHandler<StepEventArgs> Step;

        private void Broadcast(string nextInstruction, string currentState)
        {
            if (Step != null)
                Step(this, new StepEventArgs(nextInstruction, currentState));
        }

        private string EchoNextInstruction()
        {
            var result = new StringBuilder();

            result.AppendFormat("{0:X4}\t", PC);
            byte opcode = bus.Read(PC);

            switch (opcode)
            {
                case 0xA9:
                case 0xA5:
                case 0xB5:
                case 0xAD:
                case 0xBD:
                case 0xB9:
                case 0xA1:
                case 0xB1:
                    result.AppendFormat("LDA\t");
                    break;

                case 0xA2:
                case 0xA6:
                case 0xB6:
                case 0xAE:
                case 0xBE:
                    result.AppendFormat("LDX\t");
                    break;

                case 0xA0:
                case 0xA4:
                case 0xB4:
                case 0xAC:
                case 0xBC:
                    result.AppendFormat("LDY\t");
                    break;

                case 0x85:
                case 0x95:
                case 0x8D:
                case 0x9D:
                case 0x99:
                case 0x81:
                case 0x91:
                    result.AppendFormat("STA\t");
                    break;

                case 0x86:
                case 0x96:
                case 0x8E:
                    result.AppendFormat("STX\t");
                    break;

                case 0x84:
                case 0x94:
                case 0x8C:
                    result.AppendFormat("STY\t");
                    break;

                case 0x29:
                case 0x25:
                case 0x35:
                case 0x2D:
                case 0x3D:
                case 0x39:
                case 0x21:
                case 0x31:
                    result.AppendFormat("AND\t");
                    break;

                case 0x09:
                case 0x05:
                case 0x15:
                case 0x0D:
                case 0x1D:
                case 0x19:
                case 0x01:
                case 0x11:
                    result.AppendFormat("ORA\t");
                    break;

                case 0x49:
                case 0x45:
                case 0x55:
                case 0x4D:
                case 0x5D:
                case 0x59:
                case 0x41:
                case 0x51:
                    result.AppendFormat("EOR\t");
                    break;

                case 0x24:
                case 0x2C:
                    result.AppendFormat("BIT\t");
                    break;

                case 0x69:
                case 0x65:
                case 0x75:
                case 0x6D:
                case 0x7D:
                case 0x79:
                case 0x61:
                case 0x71:
                    result.AppendFormat("ADC\t");
                    break;

                case 0xE9:
                case 0xE5:
                case 0xF5:
                case 0xED:
                case 0xFD:
                case 0xF9:
                case 0xE1:
                case 0xF1:
                    result.AppendFormat("SBC\t");
                    break;

                case 0xC9:
                case 0xC5:
                case 0xD5:
                case 0xCD:
                case 0xDD:
                case 0xD9:
                case 0xC1:
                case 0xD1:
                    result.AppendFormat("CMP\t");
                    break;

                case 0xE0:
                case 0xE4:
                case 0xEC:
                    result.AppendFormat("CPX\t");
                    break;

                case 0xC0:
                case 0xC4:
                case 0xCC:
                    result.AppendFormat("CPY\t");
                    break;

                case 0x2A:
                case 0x26:
                case 0x36:
                case 0x2E:
                case 0x3E:
                    result.AppendFormat("ROL\t");
                    break;

                case 0x6A:
                case 0x66:
                case 0x76:
                case 0x6E:
                case 0x7E:
                    result.AppendFormat("ROR\t");
                    break;

                case 0x4A:
                case 0x46:
                case 0x56:
                case 0x4E:
                case 0x5E:
                    result.AppendFormat("LSR\t");
                    break;

                case 0x0A:
                case 0x06:
                case 0x16:
                case 0x0E:
                case 0x1E:
                    result.AppendFormat("ASL\t");
                    break;

                case 0xC6:
                case 0xD6:
                case 0xCE:
                case 0xDE:
                    result.AppendFormat("DEC\t");
                    break;

                case 0xE6:
                case 0xF6:
                case 0xEE:
                case 0xFE:
                    result.AppendFormat("INC\t");
                    break;

                case 0x4C:
                case 0x6C:
                    result.AppendFormat("JMP\t");
                    break;

                case 0xAA: result.AppendFormat("TAX\t"); break;
                case 0x8A: result.AppendFormat("TXA\t"); break;
                case 0xA8: result.AppendFormat("TAY\t"); break;
                case 0x98: result.AppendFormat("TYA\t"); break;
                case 0x18: result.AppendFormat("CLC\t"); break;
                case 0x38: result.AppendFormat("SEC\t"); break;
                case 0x58: result.AppendFormat("CLI\t"); break;
                case 0x78: result.AppendFormat("SEI\t"); break;
                case 0xB8: result.AppendFormat("CLV\t"); break;
                case 0xD8: result.AppendFormat("CLD\t"); break;
                case 0xF8: result.AppendFormat("SED\t"); break;
                case 0xE8: result.AppendFormat("INX\t"); break;
                case 0xCA: result.AppendFormat("DEX\t"); break;
                case 0xC8: result.AppendFormat("INY\t"); break;
                case 0x88: result.AppendFormat("DEY\t"); break;
                case 0x90: result.AppendFormat("BCC\t"); break;
                case 0xB0: result.AppendFormat("BCS\t"); break;
                case 0xD0: result.AppendFormat("BNE\t"); break;
                case 0xF0: result.AppendFormat("BEQ\t"); break;
                case 0x10: result.AppendFormat("BPL\t"); break;
                case 0x30: result.AppendFormat("BMI\t"); break;
                case 0x50: result.AppendFormat("BVC\t"); break;
                case 0x70: result.AppendFormat("BVS\t"); break;
                case 0x9A: result.AppendFormat("TXS\t"); break;
                case 0xBA: result.AppendFormat("TSX\t"); break;
                case 0xEA: result.AppendFormat("NOP\t"); break;
                case 0x20: result.AppendFormat("JSR\t"); break;
                case 0x60: result.AppendFormat("RTS\t"); break;
                case 0x00: result.AppendFormat("BRK\t"); break;
                case 0x40: result.AppendFormat("RTI\t"); break;
                case 0x48: result.AppendFormat("PHA\t"); break;
                case 0x68: result.AppendFormat("PLA\t"); break;
                case 0x08: result.AppendFormat("PHP\t"); break;
                case 0x28: result.AppendFormat("PLP\t"); break;
            }

            switch (opcode)
            {
                case 0xA9:
                case 0xA2:
                case 0xA0:
                case 0x29:
                case 0x09:
                case 0x49:
                case 0x69:
                case 0xE9:
                case 0xC9:
                case 0xE0:
                case 0xC0:
                    {
                        var operand = bus.Read((ushort)(PC + 1));
                        result.AppendFormat("#${0:X2}      ", operand);
                        break;
                    }

                case 0xA5:
                case 0xA6:
                case 0xA4:
                case 0x85:
                case 0x86:
                case 0x84:
                case 0x25:
                case 0x05:
                case 0x45:
                case 0x24:
                case 0x65:
                case 0xE5:
                case 0xC5:
                case 0xE4:
                case 0xC4:
                case 0x26:
                case 0x66:
                case 0x46:
                case 0x06:
                case 0xC6:
                case 0xE6:
                    {
                        var operand = bus.Read((ushort)(PC + 1));
                        result.AppendFormat("${0:X2}     ", operand);
                        break;
                    }

                case 0xB5:
                case 0xB4:
                case 0x95:
                case 0x94:
                case 0x35:
                case 0x15:
                case 0x55:
                case 0x75:
                case 0xF5:
                case 0xD5:
                case 0x36:
                case 0x76:
                case 0x56:
                case 0x16:
                case 0xD6:
                case 0xF6:
                    {
                        var operand = bus.Read((ushort)(PC + 1));
                        result.AppendFormat("${0:X2}, X  ", operand);
                        break;
                    }

                case 0xB6:
                case 0x96:
                    {
                        var operand = bus.Read((ushort)(PC + 1));
                        result.AppendFormat("${0:X2}, Y  ", operand);
                        break;
                    }

                case 0xAD:
                case 0xAE:
                case 0xAC:
                case 0x8D:
                case 0x8E:
                case 0x8C:
                case 0x2D:
                case 0x0D:
                case 0x4D:
                case 0x2C:
                case 0x6D:
                case 0xED:
                case 0xCD:
                case 0xEC:
                case 0xCC:
                case 0x2E:
                case 0x6E:
                case 0x4E:
                case 0x0E:
                case 0xCE:
                case 0xEE:
                case 0x4C:
                case 0x20:
                    {
                        var addrLow = bus.Read((ushort)(PC + 1));
                        var addrHi = bus.Read((ushort)(PC + 2));
                        result.AppendFormat("${0:X2}{1:X2}     ", addrHi, addrLow);
                        break;
                    }

                case 0x6C:
                    {
                        var addrLow = bus.Read((ushort)(PC + 1));
                        var addrHi = bus.Read((ushort)(PC + 2));
                        result.AppendFormat("(${0:X2}{1:X2})   ", addrHi, addrLow);
                        break;
                    }

                case 0xBD:
                case 0xBC:
                case 0x9D:
                case 0x3D:
                case 0x1D:
                case 0x5D:
                case 0x7D:
                case 0xFD:
                case 0xDD:
                case 0x3E:
                case 0x7E:
                case 0x5E:
                case 0x1E:
                case 0xDE:
                case 0xFE:
                    {
                        var addrLow = bus.Read((ushort)(PC + 1));
                        var addrHi = bus.Read((ushort)(PC + 2));
                        result.AppendFormat("${0:X2}{1:X2}, X  ", addrHi, addrLow);
                        break;
                    }

                case 0xB9:
                case 0xBE:
                case 0x99:
                case 0x39:
                case 0x19:
                case 0x59:
                case 0x79:
                case 0xF9:
                case 0xD9:
                    {
                        var addrLow = bus.Read((ushort)(PC + 1));
                        var addrHi = bus.Read((ushort)(PC + 2));
                        result.AppendFormat("${0:X2}{1:X2}, Y  ", addrHi, addrLow);
                        break;
                    }

                case 0xA1:
                case 0x81:
                case 0x21:
                case 0x01:
                case 0x41:
                case 0x61:
                case 0xE1:
                case 0xC1:
                    {
                        var operand = bus.Read((ushort)(PC + 1));
                        result.AppendFormat("(${0:X2}, X)", operand);
                        break;
                    }

                case 0xB1:
                case 0x91:
                case 0x31:
                case 0x11:
                case 0x51:
                case 0x71:
                case 0xF1:
                case 0xD1:
                    {
                        var operand = bus.Read((ushort)(PC + 1));
                        result.AppendFormat("(${0:X2}), Y", operand);
                        break;
                    }


                case 0x2A:
                case 0x6A:
                case 0x4A:
                case 0x0A:
                    {
                        result.AppendFormat("A        ");
                        break;
                    }

                case 0x90:
                case 0xB0:
                case 0xD0:
                case 0xF0:
                case 0x10:
                case 0x30:
                case 0x50:
                case 0x70:
                    {
                        var offset = bus.Read((ushort)(PC + 1));
                        result.AppendFormat("<{0:X4}>   ", (ushort)(PC + 2 + (sbyte)offset));
                        break;
                    }
                default:
                    result.AppendFormat("         ");
                    break;
            }
            return result.ToString();
        }

        public string EmitCurrentState()
        {
            var result = new StringBuilder();

            result.AppendFormat("\t{0:X2} {1:X2} {2:X2} {3:X2} ", A, X, Y, S);
            result.AppendFormat("{0}{1}{2}{3}{4}{5}",
                N ? "[N]" : " n ",
                Z ? "[Z]" : " z ",
                C ? "[C]" : " c ",
                I ? "[I]" : " i ",
                D ? "[D]" : " d ",
                V ? "[V]" : " v ");

            return result.ToString();
        }

        private void BRK()
        {
            throw new NotImplementedException();
        }

        private void RTI()
        {
            throw new NotImplementedException();
        }

        private void PHA()
        {
            bus.Write(_MakeAddress(S, 0x01), A);
            S = CyclicDecrement(S);
        }

        private void PLA()
        {
            S = CyclicIncrement(S);
            A = bus.Read(_MakeAddress(S, 0x01));
        }

        private void PHP()
        {
            throw new NotImplementedException();
        }

        private void PLP()
        {
            throw new NotImplementedException();
        }

        byte CyclicIncrement(byte value)
        {
            return (value == 0xFF) ? (byte)0x00 : (byte)(value + 1);
        }

        byte CyclicDecrement(byte value)
        {
            return (value == 0x00) ? (byte)0xFF : (byte)(value - 1);
        }

        void JSR()
        {
            /*When executing JSR (jump to subroutine) and RTS (return from subroutine)
            instructions, the return address pushed to the stack by JSR is that of the last byte
            of the JSR operand (that is, the most significant byte of the subroutine address),
            rather than the address of the following instruction. This is because the actual
            copy (from program counter to stack and then vice versa) takes place before the
            automatic increment of the program counter that occurs at the end of every
            instruction. This characteristic would go unnoticed unless the code examined the
            return address in order to retrieve parameters in the code stream (a 6502
            programming idiom documented in the ProDOS 8 Technical Reference Manual).
            It remains a characteristic of 6502 derivatives to this day.*/

            var addressLow = bus.Read(PC);
            PC += 1;
            var addressHigh = bus.Read(PC);
            // Taken care of in RTS

            bus.Write(_MakeAddress(S, 0x01), (byte)(PC >> 8));
            S = CyclicDecrement(S);
            bus.Write(_MakeAddress(S, 0x01), (byte)(PC));
            S = CyclicDecrement(S);

            PC = _MakeAddress(addressLow, addressHigh);
        }

        void RTS()
        {
            S = CyclicIncrement(S);
            var addressLow = bus.Read(_MakeAddress(S, 0x01));
            S = CyclicIncrement(S);
            var addressHigh = bus.Read(_MakeAddress(S, 0x01));

            PC = _MakeAddress(addressLow, addressHigh);
            PC += 1; // Omitted from JSR
        }

        void NOP() { }

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

            C = ((eightBits >> 8) != 0);
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

            C = ((eightBits >> 8) != 0);
            V = ((eightBits >> 8) != (sevenBits >> 7));
            A = FlagSense((byte)eightBits);
        }

        void CMP(Func<byte> readMode) { Compare(A, readMode); }
        void CPX(Func<byte> readMode) { Compare(X, readMode); }
        void CPY(Func<byte> readMode) { Compare(Y, readMode); }
        void Compare(byte a, Func<byte> readMode)
        {
            // TODO: can this be done more cleanly?
            // TODO: is it even correct?
            var b = readMode();
            var eightBits = a - b;
            var sevenBits = (a & 0x7F) - (b & 0x7F);

            C = ((eightBits >> 8) != 0);
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
        void TSX() { Transfer(S, out X); }
        void TXS() { S = X; } // NO FlagSense here
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
        void INY() { Increment(ref Y); }
        void Increment(ref byte register)
        {
            register = FlagSense(register == 0xFF ? (byte)0x00 : (byte)(register + 1));
        }

        void DEX() { Decrement(ref X); }
        void DEY() { Decrement(ref Y); }
        void Decrement(ref byte register)
        {
            register = FlagSense(register == 0x00 ? (byte)0xFF : (byte)(register - 1));
        }

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

        void JMP_Abs()
        {
            var addressLow = bus.Read(PC);
            PC += 1;
            var addressHigh = bus.Read(PC);
            PC += 1;
            PC = _MakeAddress(addressLow, addressHigh);
        }

        void JMP_Indirect()
        {
            var indirectLow = bus.Read(PC);
            PC += 1;
            var indirectHigh = bus.Read(PC);
            PC += 1;
            /*The 6502's memory indirect jump instruction, JMP (<address>), is partially broken.
            If <address> is hex xxFF (i.e., any word ending in FF), the processor will not jump
            to the address stored in xxFF and xxFF+1 as expected, but rather the one defined by
            xxFF and xx00 (for example, JMP ($10FF) would jump to the address stored in 10FF
            and 1000, instead of the one stored in 10FF and 1100). This defect continued
            through the entire NMOS line, but was corrected in the CMOS derivatives.*/
            // Implemented by page locking the address reads
            var addressLow = bus.Read(_MakePageLockedAddress(indirectLow, indirectHigh, 0));
            var addressHigh = bus.Read(_MakePageLockedAddress(indirectLow, indirectHigh, 1));
            PC = _MakeAddress(addressLow, addressHigh);
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
            return bus.Read(_MakePageLockedAddress(addressLow, 0x00, indexValue));
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
            return bus.Read(_MakePageCrossingAddress(addressLow, addressHigh, indexValue));
        }

        byte Read_IndexedIndirect() // (ADDR, X)
        {
            byte baseAddressLow = bus.Read(PC);
            PC += 1;
            byte indirectAddressLow = bus.Read(_MakePageLockedAddress(baseAddressLow, 0x00, X));
            byte indirectAddressHigh = bus.Read(_MakePageLockedAddress(baseAddressLow, 0x00, (byte)(X + 1)));
            return bus.Read(_MakeAddress(indirectAddressLow, indirectAddressHigh));
        }

        byte Read_IndirectIndexed() // (ADDR),Y
        {
            byte indirectAddressLow = bus.Read(PC);
            PC += 1;
            byte baseAddressLow = bus.Read(_MakePageLockedAddress(indirectAddressLow, 0x00, 0));
            byte baseAddressHigh = bus.Read(_MakePageLockedAddress(indirectAddressLow, 0x00, 1));
            return bus.Read(_MakePageCrossingAddress(baseAddressLow, baseAddressHigh, Y));
        }

        void Write_ZeroPage(byte data) { Write_ZeroPageIndexed(0, data); }
        void Write_ZeroPageXIndexed(byte data) { Write_ZeroPageIndexed(X, data); }
        void Write_ZeroPageYIndexed(byte data) { Write_ZeroPageIndexed(Y, data); }
        void Write_ZeroPageIndexed(byte indexValue, byte data)
        {
            byte addressLow = bus.Read(PC);
            PC += 1;
            bus.Write(_MakePageLockedAddress(addressLow, 0x00, indexValue), data);
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
            bus.Write(_MakePageCrossingAddress(addressLow, addressHigh, indexValue), data);
        }

        void Write_IndexedIndirect(byte data) // (ADDR, X)
        {
            byte baseAddressLow = bus.Read(PC);
            PC += 1;
            byte indirectAddressLow = bus.Read(_MakePageLockedAddress(baseAddressLow, 0x00, X));
            byte indirectAddressHigh = bus.Read(_MakePageLockedAddress(baseAddressLow, 0x00, (byte)(X + 1)));
            bus.Write(_MakeAddress(indirectAddressLow, indirectAddressHigh), data);
        }

        void Write_IndirectIndexed(byte data) // (ADDR),Y
        {
            byte indirectAddressLow = bus.Read(PC);
            PC += 1;
            byte baseAddressLow = bus.Read(_MakePageLockedAddress(indirectAddressLow, 0x00, 0));
            byte baseAddressHigh = bus.Read(_MakePageLockedAddress(indirectAddressLow, 0x00, 1));
            bus.Write(_MakePageCrossingAddress(baseAddressLow, baseAddressHigh, Y), data);
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

        byte INC(byte input)
        {
            return FlagSense(input == 0xFF ? (byte)0 : (byte)(input + 1));
        }

        byte DEC(byte input)
        {
            return FlagSense(input == 0x0 ? (byte)0xFF : (byte)(input - 1));
        }

        void ReadWrite_Accumulator(Func<byte, byte> instruction)
        {
            A = instruction(A);
        }

        void ReadWrite_ZeroPage(Func<byte, byte> instruction)
        {
            ReadWrite_ZeroPageIndexed(0, instruction);
        }
        void ReadWrite_ZeroPageXIndexed(Func<byte, byte> instruction)
        {
            ReadWrite_ZeroPageIndexed(X, instruction);
        }
        void ReadWrite_ZeroPageIndexed(byte indexValue, Func<byte, byte> instruction)
        {
            byte addressLow = bus.Read(PC);
            PC += 1;
            ushort address = _MakePageLockedAddress(addressLow, 0x00, indexValue);
            bus.Write(address, instruction(bus.Read(address)));
        }


        void ReadWrite_Absolute(Func<byte, byte> instruction)
        {
            ReadWrite_AbsoluteIndexed(0, instruction);
        }
        void ReadWrite_AbsoluteXIndexed(Func<byte, byte> instruction)
        {
            ReadWrite_AbsoluteIndexed(X, instruction);
        }
        void ReadWrite_AbsoluteIndexed(byte indexValue, Func<byte, byte> instruction)
        {
            byte addressLow = bus.Read(PC);
            PC += 1;
            byte addressHigh = bus.Read(PC);
            PC += 1;
            ushort address = _MakePageCrossingAddress(addressLow, addressHigh, indexValue);
            bus.Write(address, instruction(bus.Read(address)));
        }

        private ushort _MakeAddress(byte addressLow, byte addressHigh)
        {
            return (ushort)((addressHigh << 8) | addressLow);
        }

        private ushort _MakePageLockedAddress(byte addressLow, byte addressHigh, byte offset)
        {
            return _MakeAddress((byte)(addressLow + offset), addressHigh);
        }

        private ushort _MakePageCrossingAddress(byte addressLow, byte addressHigh, byte offset)
        {
            return (ushort)(((addressHigh << 8) | addressLow) + offset);
        }
    }

    public interface BUS
    {
        byte Read(ushort address);

        void Write(ushort address, byte data);
    }
}
