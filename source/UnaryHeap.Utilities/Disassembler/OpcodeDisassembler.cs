using System;
using System.IO;
using System.Linq;

namespace Disassembler
{
    class OpcodeDisassembler : IDisposable
    {
        private Stream source;

        public OpcodeDisassembler(Stream source)
        {
            this.source = source;
        }

        public void Dispose()
        {
            source.Close();
        }

        public void Disassemble(int baseAddress, int startAddress, int length,
            TextWriter output, LabelSet labels, Range[] dataRegions)
        {
            var instructionOutput = output;
            var dataOutput = output;
            var endAddress = startAddress + length - 1;

            source.Seek(startAddress, SeekOrigin.Begin);
            for (int i = startAddress; i <= endAddress;)
            {
                var dataRegion = dataRegions.FirstOrDefault(r => r.Start == baseAddress);

                if (dataRegion != null)
                {
                    foreach (var skip in Enumerable.Range(0, dataRegion.Length))
                        SafeReadByte();
                    dataOutput.WriteLine("{1:X4} Skipped {0} data bytes", dataRegion.Length, baseAddress);
                    baseAddress += dataRegion.Length;
                    i += dataRegion.Length;
                    continue;
                }

                var opcode = SafeReadByte();
                var instruction = GetInstruction(opcode);

                if (instruction.Mode.Length == 0)
                {
                    instructionOutput.WriteLine("{3:X4}\t{0}\t{1} {2}",
                        labels.GetLabel(baseAddress), instruction.Nmemonic,
                        instruction.Mode.FormatNoOperands(), baseAddress);
                    baseAddress += 1;
                    i += 1;
                }
                else if (instruction.Mode.Length == 1)
                {
                    var operand = SafeReadByte();

                    if (instruction.IsControlFlow)
                        labels.Record(instruction.Mode.GetAddress(baseAddress, operand));

                    instructionOutput.WriteLine("{3:X4}\t{0}\t{1} {2}",
                        labels.GetLabel(baseAddress), instruction.Nmemonic,
                        instruction.IsControlFlow ? 
                            labels.GetLabel(instruction.Mode.GetAddress(baseAddress, operand)) : 
                            instruction.Mode.FormatOneOperand(baseAddress, operand),
                        baseAddress);
                    baseAddress += 2;
                    i += 2;
                }
                else if (instruction.Mode.Length == 2)
                {
                    var operand1 = SafeReadByte();
                    var operand2 = SafeReadByte();

                    if (instruction.IsControlFlow)
                        labels.Record(instruction.Mode.GetAddress(operand1, operand2));

                    try
                    {
                        instructionOutput.WriteLine("{3:X4}\t{0}\t{1} {2}",
                            labels.GetLabel(baseAddress), instruction.Nmemonic,
                            instruction.IsControlFlow ? 
                                labels.GetLabel(instruction.Mode.GetAddress(operand1, operand2)) :
                                instruction.Mode.FormatTwoOperands(operand1, operand2),
                        baseAddress);
                    }
                    catch (NotImplementedException ex)
                    {
                        throw new NotImplementedException(
                            ex.Message +  string.Format(" at address {0:X4}", baseAddress));
                    }
                    baseAddress += 3;
                    i += 3;
                }
            }
        }

        public int[] ReadJumpVectorHiHiLoLo(int address, int length)
        {
            var result = new int[length];
            foreach (var i in Enumerable.Range(0, length))
            {
                source.Seek(address + i, SeekOrigin.Begin);
                int hi = SafeReadByte();
                source.Seek(address + i + length, SeekOrigin.Begin);
                int lo = SafeReadByte();
                result[i] = (hi << 8) | lo;
            }
            return result;
        }

        public int[] ReadJumpVectorLoHiLoHi(int address, int length)
        {
            var result = new int[length];
            foreach (var i in Enumerable.Range(0, length))
            {
                source.Seek(address + 2 * i, SeekOrigin.Begin);
                int lo = SafeReadByte();
                source.Seek(address + 2 * i + 1, SeekOrigin.Begin);
                int hi = SafeReadByte();
                result[i] = (hi << 8) | lo;
            }
            return result;
        }

        byte SafeReadByte()
        {
            int result = source.ReadByte();
            if (result == -1)
                throw new InvalidDataException("Unexpected end-of-stream reached");
            return (byte)result;
        }

        Instruction GetInstruction(byte opcode)
        {
            switch (opcode)
            {
                case 0xA9: return new LDA().Immediate;
                case 0xAD: return new LDA().Absolute;
                case 0xB9: return new LDA().AbsoluteYIndexed;
                case 0xBD: return new LDA().AbsoluteXIndexed;
                case 0xA5: return new LDA().ZeroPage;
                case 0xB5: return new LDA().ZeroPageXIndexed;
                case 0xB1: return new LDA().IndirectYIndexed;
                case 0xA2: return new LDX().Immediate;
                case 0xA6: return new LDX().ZeroPage;
                case 0xBE: return new LDX().AbsoluteYIndexed;
                case 0xAE: return new LDX().Absolute;
                case 0xA0: return new LDY().Immediate;
                case 0xA4: return new LDY().ZeroPage;
                case 0xAC: return new LDY().Absolute;
                case 0xBC: return new LDY().AbsoluteXIndexed;
                case 0xB4: return new LDY().ZeroPageXIndexed;
                case 0x8D: return new STA().Absolute;
                case 0x9D: return new STA().AbsoluteXIndexed;
                case 0x99: return new STA().AbsoluteYIndexed;
                case 0x85: return new STA().ZeroPage;
                case 0x95: return new STA().ZeroPageXIndexed;
                case 0x91: return new STA().IndirectYIndexed;
                case 0x8E: return new STX().Absolute;
                case 0x86: return new STX().ZeroPage;
                case 0x8C: return new STY().Absolute;
                case 0x84: return new STY().ZeroPage;
                case 0x94: return new STY().ZeroPageXIndexed;
                case 0x29: return new AND().Immediate;
                case 0x25: return new AND().ZeroPage;
                case 0x35: return new AND().ZeroPageXIndexed;
                case 0x3D: return new AND().AbsoluteXIndexed;
                case 0x09: return new ORA().Immediate;
                case 0x05: return new ORA().ZeroPage;
                case 0x1D: return new ORA().AbsoluteXIndexed;
                case 0x19: return new ORA().AbsoluteYIndexed;
                case 0x0D: return new ORA().Absolute;
                case 0x49: return new EOR().Immediate;
                case 0x45: return new EOR().ZeroPage;
                case 0x55: return new EOR().ZeroPageXIndexed;
                case 0x4D: return new EOR().Absolute;
                case 0x5D: return new EOR().AbsoluteXIndexed;
                case 0x59: return new EOR().AbsoluteYIndexed;
                case 0x4A: return new LSR().Accumulator;
                case 0x5E: return new LSR().AbsoluteXIndexed;
                case 0x46: return new LSR().ZeroPage;
                case 0x6A: return new ROR().Accumulator;
                case 0x66: return new ROR().ZeroPage;
                case 0x6E: return new ROR().Absolute;
                case 0x7E: return new ROR().AbsoluteXIndexed;
                case 0x0A: return new ASL().Accumulator;
                case 0x1E: return new ASL().AbsoluteXIndexed;
                case 0x06: return new ASL().ZeroPage;
                case 0x2A: return new ROL().Accumulator;
                case 0x26: return new ROL().ZeroPage;
                case 0x2E: return new ROL().Absolute;
                case 0x65: return new ADC().ZeroPage;
                case 0x7D: return new ADC().AbsoluteXIndexed;
                case 0x69: return new ADC().Immediate;
                case 0x79: return new ADC().AbsoluteYIndexed;
                case 0x6D: return new ADC().Absolute;
                case 0x71: return new ADC().IndirectYIndexed;
                case 0xE9: return new SBC().Immediate;
                case 0xED: return new SBC().Absolute;
                case 0xE5: return new SBC().ZeroPage;
                case 0xF5: return new SBC().ZeroPageXIndexed;
                case 0xF1: return new SBC().IndirectYIndexed;
                case 0xFD: return new SBC().AbsoluteXIndexed;
                case 0xF9: return new SBC().AbsoluteYIndexed;
                case 0xE6: return new INC().ZeroPage;
                case 0xEE: return new INC().Absolute;
                case 0xFE: return new INC().AbsoluteXIndexed;
                case 0xF6: return new INC().ZeroPageXIndexed;
                case 0xC6: return new DEC().ZeroPage;
                case 0xD6: return new DEC().ZeroPageXIndexed;
                case 0xDE: return new DEC().AbsoluteXIndexed;
                case 0xCE: return new DEC().Absolute;
                case 0xC9: return new CMP().Immediate;
                case 0xC5: return new CMP().ZeroPage;
                case 0xCD: return new CMP().Absolute;
                case 0xDD: return new CMP().AbsoluteXIndexed;
                case 0xD9: return new CMP().AbsoluteYIndexed;
                case 0xE0: return new CPX().Immediate;
                case 0xE4: return new CPX().ZeroPage;
                case 0xC0: return new CPY().Immediate;
                case 0xC4: return new CPY().ZeroPage;
                case 0xD0: return new BNE().Relative;
                case 0x90: return new BCC().Relative;
                case 0xF0: return new BEQ().Relative;
                case 0xB0: return new BCS().Relative;
                case 0x30: return new BMI().Relative;
                case 0x10: return new BPL().Relative;
                case 0x50: return new BVC().Relative;
                case 0x4C: return new JMP().Absolute;
                case 0x20: return new JSR().Absolute;
                case 0x6C: return new JMP().Indirect;
                case 0xCA: return Implied.DEX;
                case 0x88: return Implied.DEY;
                case 0xE8: return Implied.INX;
                case 0xC8: return Implied.INY;
                case 0x60: return Implied.RTS;
                case 0x18: return Implied.CLC;
                case 0x38: return Implied.SEC;
                case 0x8A: return Implied.TXA;
                case 0xAA: return Implied.TAX;
                case 0xA8: return Implied.TAY;
                case 0x98: return Implied.TYA;
                case 0x48: return Implied.PHA;
                case 0x68: return Implied.PLA;
                case 0x08: return Implied.PHP;
                case 0x28: return Implied.PLP;
                case 0xD8: return Implied.CLD;
                case 0x9A: return Implied.TXS;
                case 0xBA: return Implied.TSX;
                case 0x40: return Implied.RTI;
                case 0x78: return Implied.SEI;
                default: return new UnrecognizedInstruction(opcode);
            }
        }
    }
}
