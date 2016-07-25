#if INCLUDE_WORK_IN_PROGRESS_SNAKEOIL

using System;
using System.IO;

namespace UnaryHeap.MCS6500
{
    public interface Cartridge
    {
        byte CpuRead(ushort address);
        void CpuWrite(ushort address, byte data);
        byte PpuRead(ushort address);
        void PpuWrite(ushort address, byte data);
    }

    public class MMC1 : Cartridge
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

        public void CpuWrite(ushort address, byte data)
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

                    shiftRegister = 0;
                    bitsShifted = 0;
                }
            }
        }

        public byte CpuRead(ushort address)
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
                if (accessingSecondBank)
                    return prgRom.Last.Read(bankRelativeAddress);
                else
                    return prgRom[prgBankIndex].Read(bankRelativeAddress);
                default:
                    throw new ApplicationException("The world does not make sense.");
            }
        }

        public byte PpuRead(ushort address)
        {
            if (address >= 0x2000)
                throw new NotImplementedException();

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
                if (accessingSecondBank)
                    return chrRom[1 | (chr0Register << 1)].Read(bankRelativeAddress);
                else
                    return chrRom[chr0Register << 1].Read(bankRelativeAddress);
            }
        }

        public void PpuWrite(ushort address, byte data)
        {
            throw new NotImplementedException();
        }
    }

    public class RomBank
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

    public class RomBankSet
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

    public class NES_CPU_BUS : BUS
    {
        RAM ram = new RAM();
        TwoA03 apuIO = new TwoA03();
        PPU ppu;
        Cartridge cartridge;

        public NES_CPU_BUS(Cartridge gamePack)
        {
            this.cartridge = gamePack;
            this.ppu = new PPU(gamePack);
        }

        public byte Read(ushort address)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
                return ram.Read((ushort)(address & 0x7FF));
            else if (address >= 0x2000 && address <= 0x3FFF)
                return ppu.Read((ushort)(address & 0x7));
            else if (address >= 0x4000 && address <= 0x401F)
                return apuIO.Read(address);
            else
                return cartridge.CpuRead(address);
        }

        public void Write(ushort address, byte data)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
                ram.Write((ushort)(address & 0x7FF), data);
            else if (address >= 0x2000 && address <= 0x3FFF)
                ppu.Write((ushort)(address & 0x7), data);
            else if (address >= 0x4000 && address <= 0x401F)
                apuIO.Write(address, data);
            else
                cartridge.CpuWrite(address, data);
        }
    }

    public class RAM : BUS
    {
        byte[] ram = new byte[0x800];
        public byte Read(ushort address) { return ram[address]; }
        public void Write(ushort address, byte data) { ram[address] = data; }
    }

    public class TwoA03 : BUS
    {
        TextWriter output = Console.Out;
        public byte Read(ushort address)
        {
            output.WriteLine("2a03 read {0:X4}", address);
            return 0;
        }

        public void Write(ushort address, byte data)
        {
            output.WriteLine("2a03 write {0:X4} <- {1:X2}", address, data);
        }
    }

    /*
    0: PPU CTRL (write)
    1: PPU MASK (write)
    2: PPU STATUS (read)
    3: OAM ADDR (write)
    4: OAM DATA (read/write)
    5: PPU SCROLL (write x2)
    6: PPU ADDR (write x2)
    7: PPU DATA (read/write)
    */
    public class PPU : BUS
    {
        Cartridge cartridge;
        ushort ppuAddress;
        byte? addressLatch;
        byte dataBuffer;

        public PPU(Cartridge gamePack)
        {
            this.cartridge = gamePack;
        }

        public byte Read(ushort address)
        {
            switch (address)
            {
                case 2:
                    return ReadStatus();
                case 4:
                    return ReadOamData();
                case 7:
                    return ReadPpuData();
                default:
                    throw new NotImplementedException();
            }
        }

        public void Write(ushort address, byte data)
        {
            switch (address)
            {
                case 0:
                    WriteControl(data); break;
                case 1:
                    WriteMask(data); break;
                case 3:
                    WriteOamAddr(data); break;
                case 4:
                    WriteOamData(data); break;
                case 5:
                    WriteScroll(data); break;
                case 6:
                    WritePpuAddr(data); break;
                case 7:
                    WritePpuData(data); break;
                default:
                    throw new NotImplementedException();
            }
        }

        byte ReadStatus() { return 0x80; } // Permanent VBLANK
        byte ReadOamData() { return 0; }
        byte ReadPpuData()
        {
            byte result = dataBuffer;
            dataBuffer = cartridge.PpuRead(ppuAddress);
            ppuAddress = (ushort)((ppuAddress + 1) & 0x3FFF);
            return result;
        }

        void WriteControl(byte data) { }
        void WriteMask(byte data) { }
        void WriteOamAddr(byte data) { }
        void WriteOamData(byte data) { }
        void WriteScroll(byte data) { }
        void WritePpuAddr(byte data)
        {
            if (addressLatch.HasValue)
            {
                ppuAddress = (ushort)((addressLatch.Value << 8) | data);
                addressLatch = null;
            }
            else
            {
                addressLatch = data;
            }
        }
        void WritePpuData(byte value) { }
    }
}

#endif