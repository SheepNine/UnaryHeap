#if INCLUDE_WORK_IN_PROGRESS_SNAKEOIL

using System;

namespace UnaryHeap.MCS6500
{
    class MMC1
    {
        RomBankSet chrRom;
        RomBankSet prgRom;

        byte controlRegister = 0x0C;
        byte chr0Register;
        byte chr1Register;
        byte prgRegister;

        byte shiftRegister;
        int bitsShifted;

        public MMC1(RomBankSet chrRom, RomBankSet prgRom)
        {
            this.chrRom = chrRom;
            this.prgRom = prgRom;
        }

        public void Write(ushort address, byte data)
        {
            if ((data & 0x80) == 0x80)
            {
                shiftRegister = 0;
                bitsShifted = 0;
                controlRegister |= 0x0C;
            }
            else
            {
                shiftRegister = (byte)(shiftRegister | ((data & 0x1) << bitsShifted));
                bitsShifted += 1;
                if (bitsShifted == 5)
                {
                    bitsShifted = 0;

                    switch (address & 0x6000)
                    {
                        case 0x0000:
                            controlRegister = shiftRegister;
                            break;
                        case 0x2000:
                            chr0Register = shiftRegister;
                            break;
                        case 0x4000:
                            chr1Register = shiftRegister;
                            break;
                        case 0x6000:
                            prgRegister = shiftRegister;
                            break;
                        default:
                            throw new ApplicationException("The world does not make sense.");
                    }
                }
            }
        }

        public byte CPURead(ushort address)
        {
            if (address < 0x8000)
                throw new NotImplementedException();

            var bankRelativeAddress = (ushort)(address & 0x3FFF);
            var accessingSecondBank = (address >= 0xC000);
            var prgBankIndex = prgRegister & 0xF; // high bit for RAM enable/disable

            switch ((controlRegister & 0xC) >> 2)
            {
                //0, 1: switch 32 KB at $8000, ignoring low bit of bank number
                case 0:
                case 1:
                    if (accessingSecondBank)
                        return prgRom[1 | (prgBankIndex << 1)].Read(bankRelativeAddress);
                    else
                        return prgRom[prgBankIndex << 1].Read(bankRelativeAddress);
                //2: fix first bank at $8000 and switch 16 KB bank at $C000
                case 2:
                    throw new NotImplementedException("Unverified");
                /*if (accessingSecondBank)
                    return PRG_ROM[prgBankIndex].Read(bankRelativeAddress);
                else
                    return PRG_ROM.First.Read(bankRelativeAddress);*/
                //3: fix last bank at $C000 and switch 16 KB bank at $8000
                case 3:
                    throw new NotImplementedException("Unverified");
                /*if (accessingSecondBank)
                    return PRG_ROM.Last.Read(bankRelativeAddress);
                else
                    return PRG_ROM[prgBankIndex].Read(bankRelativeAddress);*/
                default:
                    throw new ApplicationException("The world does not make sense.");
            }
        }

        public byte PPURead(ushort address)
        {
            var bankRelativeAddress = (ushort)(address & 0x0FFF);
            var accessingSecondBank = (address > 0x1000);

            if ((controlRegister & 0x10) == 0x10) //1: switch two separate 4 KB banks
            {
                if (accessingSecondBank)
                    return chrRom[chr1Register].Read(bankRelativeAddress);
                else
                    return chrRom[chr0Register].Read(bankRelativeAddress);
            }
            else //0: switch 8 KB at a time
            {
                throw new NotImplementedException("Unverified");
                /*if (accessingSecondBank)
                    return CHR_ROM[1 | (chr0Register << 1)].Read(bankRelativeAddress);
                else
                    return CHR_ROM[chr0Register << 1].Read(bankRelativeAddress);*/
            }
        }
    }

    class RomBank
    {
        byte[] data;

        public RomBank(byte[] data)
        {
            this.data = data;
        }

        public byte Read(ushort address)
        {
            return data[address];
        }
    }

    class RomBankSet
    {
        RomBank[] banks;

        public RomBankSet(RomBank[] banks)
        {
            this.banks = banks;
        }

        public RomBank First
        {
            get { return banks[0]; }
        }

        public RomBank Last
        {
            get { return banks[banks.Length - 1]; }
        }

        public RomBank this[int index]
        {
            get { return banks[index]; }
        }
    }
}

#endif