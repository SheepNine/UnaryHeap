#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace UnaryHeap.Utilities
{
    static class PortableExecutableInspector
    {
        static void Line08(StringBuilder result, string label, byte data)
        {
            result.AppendFormat(CultureInfo.InvariantCulture,
                "{0}: 0x{1:X2} ({1})", label, data);
        }

        static void Line16(StringBuilder result, string label, short data)
        {
            result.AppendFormat(CultureInfo.InvariantCulture,
                "{0}: 0x{1:X4} ({1})", label, data);
        }

        static void Line32(StringBuilder result, string label, int? data)
        {
            if (data == null) return;
            result.AppendFormat(CultureInfo.InvariantCulture,
                "{0}: 0x{1:X8} ({1})", label, data);
        }

        static void Line64(StringBuilder result, string label, long data)
        {
            result.AppendFormat(CultureInfo.InvariantCulture,
                "{0}: 0x{1:X16} ({1})", label, data);
        }

        static void Expect(Stream stream, string errorMessage, params byte[] values)
        {
            for (int i = 0; i < values.Length; i++)
                if (stream.ReadByte() != values[i])
                    throw new InvalidDataException(errorMessage);
        }

        static byte ReadLE08(Stream stream)
        {
            int byte0 = stream.ReadByte();
            if (byte0 == -1) throw new InvalidDataException("Unexpected end of stream");

            return (byte)byte0;
        }

        static short ReadLE16(Stream stream)
        {
            int byte0 = stream.ReadByte();
            if (byte0 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte1 = stream.ReadByte();
            if (byte1 == -1) throw new InvalidDataException("Unexpected end of stream");

            return (short)((byte1 << 8) | (byte0));
        }

        static int ReadLE32(Stream stream)
        {
            int byte0 = stream.ReadByte();
            if (byte0 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte1 = stream.ReadByte();
            if (byte1 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte2 = stream.ReadByte();
            if (byte2 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte3 = stream.ReadByte();
            if (byte3 == -1) throw new InvalidDataException("Unexpected end of stream");

            return (byte3 << 24) | (byte2 << 16) | (byte1 << 8) | (byte0);
        }

        static long ReadLE64(Stream stream)
        {
            int byte0 = stream.ReadByte();
            if (byte0 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte1 = stream.ReadByte();
            if (byte1 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte2 = stream.ReadByte();
            if (byte2 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte3 = stream.ReadByte();
            if (byte3 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte4 = stream.ReadByte();
            if (byte4 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte5 = stream.ReadByte();
            if (byte5 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte6 = stream.ReadByte();
            if (byte6 == -1) throw new InvalidDataException("Unexpected end of stream");
            int byte7 = stream.ReadByte();
            if (byte7 == -1) throw new InvalidDataException("Unexpected end of stream");

            return (byte7 << 56) | (byte6 << 48) | (byte5 << 40) | (byte4 << 32) |
                (byte3 << 24) | (byte2 << 16) | (byte1 << 8) | (byte0);
        }

        static string ReadString(int dataLength, Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                return TrimNulls(Encoding.ASCII.GetString(reader.ReadBytes(8)));
        }

        private static string TrimNulls(string value)
        {
            var index = value.IndexOf('\0');
            if (index != -1)
                return value.Substring(0, index);
            else
                return value;
        }


        class CoffHeader
        {
            enum MachineType : short
            {
                I386 = 0x14c,
                // TODO: fill out table
            }

            [Flags]
            enum Characteristics : short
            {
                EXECUTABLE_IMAGE = 0x2,
                _32BIT_MACHINE = 0x100,
                // TODO: fill out table
            }

            MachineType machine;
            public short numberOfSections;
            int timeDateStamp;
            int pointerToSymbolTable;
            int numberOfSymbols;
            short sizeOfOptionalHeader;
            Characteristics characteristics;

            public CoffHeader(Stream stream)
            {
                machine = (MachineType)ReadLE16(stream);
                numberOfSections = ReadLE16(stream); // NT limits numSections to 96
                timeDateStamp = ReadLE32(stream);
                pointerToSymbolTable = ReadLE32(stream);
                numberOfSymbols = ReadLE32(stream);
                sizeOfOptionalHeader = ReadLE16(stream);
                characteristics = (Characteristics)ReadLE16(stream);
            }

            public override string ToString()
            {
                var result = new StringBuilder();
                Line16(result, "machine", (short)machine);
                result.AppendLine();
                Line16(result, "numberOfSections", numberOfSections);
                result.AppendLine();
                Line32(result, "timeDateStamp", timeDateStamp);
                result.AppendLine();
                Line32(result, "pointerToSymbolTable", pointerToSymbolTable);
                result.AppendLine();
                Line32(result, "numberOfSymbols", numberOfSymbols);
                result.AppendLine();
                Line16(result, "sizeOfOptionalHeader", sizeOfOptionalHeader);
                result.AppendLine();
                Line16(result, "characteristics", (short)characteristics);
                return result.ToString();
            }
        }

        class OptionalHeaderStandardSection
        {
            public enum Magic : short
            {
                PE32Executable = 0x10b,
                PE32PlustExecutable = 0x20b,
                RomImage = 0x107
            }

            public Magic magic;
            byte majorLinkerVersion;
            byte minorLinkerVersion;
            int sizeOfCode;
            int sizeOfInitializedData;
            int sizeOfUninitializedData;
            int addressOfEntryPoint;
            int baseOfCode;
            int? baseOfData;

            public OptionalHeaderStandardSection(Stream stream)
            {
                magic = (Magic)ReadLE16(stream);
                majorLinkerVersion = ReadLE08(stream);
                minorLinkerVersion = ReadLE08(stream);
                sizeOfCode = ReadLE32(stream);
                sizeOfInitializedData = ReadLE32(stream);
                sizeOfUninitializedData = ReadLE32(stream);
                addressOfEntryPoint = ReadLE32(stream);
                baseOfCode = ReadLE32(stream);

                if (magic == Magic.PE32PlustExecutable)
                    baseOfData = null;
                else
                    baseOfData = ReadLE32(stream);
            }

            public override string ToString()
            {
                var result = new StringBuilder();
                Line16(result, "magic", (short)magic);
                result.AppendLine();
                Line08(result, "majorLinkerVersion", majorLinkerVersion);
                result.AppendLine();
                Line08(result, "minorLinkerVersion", minorLinkerVersion);
                result.AppendLine();
                Line32(result, "sizeOfCode", sizeOfCode);
                result.AppendLine();
                Line32(result, "sizeOfInitializedData", sizeOfInitializedData);
                result.AppendLine();
                Line32(result, "sizeOfUninitializedData", sizeOfUninitializedData);
                result.AppendLine();
                Line32(result, "addressOfEntryPoint", addressOfEntryPoint);
                result.AppendLine();
                Line32(result, "baseOfCode", baseOfCode);
                if (baseOfData.HasValue)
                {
                    result.AppendLine();
                    Line32(result, "baseOfData", baseOfData);
                }
                return result.ToString();
            }
        }

        class OptionalHeaderWindowsSection
        {
            long imageBase;
            int sectionAlignment;
            int fileAlignment;
            short majorOsVersion;
            short minorOsVersion;
            short majorImageVersion;
            short minorImageVersion;
            short majorSubsystemVersion;
            short minorSubsystemVersion;
            int sizeOfImage;
            int sizeOfHeaders;
            int checksum;
            short subsystem;
            short dllCharacteristics;
            long sizeOfStackReserve;
            long sizeOfStackCommit;
            long sizeOfHeapReserve;
            long sizeOfHeapCommit;
            int loaderFlags; // obsolete
            public int numberOfRVAAndSizes;

            public OptionalHeaderWindowsSection(Stream stream, bool plus)
            {
                if (plus)
                    imageBase = ReadLE64(stream);
                else
                    imageBase = ReadLE32(stream);

                sectionAlignment = ReadLE32(stream);
                fileAlignment = ReadLE32(stream);
                majorOsVersion = ReadLE16(stream);
                minorOsVersion = ReadLE16(stream);
                majorImageVersion = ReadLE16(stream);
                minorImageVersion = ReadLE16(stream);
                majorSubsystemVersion = ReadLE16(stream);
                minorSubsystemVersion = ReadLE16(stream);
                ReadLE32(stream); // RESERVED, not used
                sizeOfImage = ReadLE32(stream);
                sizeOfHeaders = ReadLE32(stream);
                checksum = ReadLE32(stream);
                subsystem = ReadLE16(stream);
                dllCharacteristics = ReadLE16(stream);

                if (plus)
                    sizeOfStackReserve = ReadLE64(stream);
                else
                    sizeOfStackReserve = ReadLE32(stream);

                if (plus)
                    sizeOfStackCommit = ReadLE64(stream);
                else
                    sizeOfStackCommit = ReadLE32(stream);

                if (plus)
                    sizeOfHeapReserve = ReadLE64(stream);
                else
                    sizeOfHeapReserve = ReadLE32(stream);

                if (plus)
                    sizeOfHeapCommit = ReadLE64(stream);
                else
                    sizeOfHeapCommit = ReadLE32(stream);

                loaderFlags = ReadLE32(stream); // obsolete
                numberOfRVAAndSizes = ReadLE32(stream);
            }

            public override string ToString()
            {
                var result = new StringBuilder();
                Line64(result, "imageBase", imageBase);
                result.AppendLine();
                Line32(result, "sectionAlignment", sectionAlignment);
                result.AppendLine();
                Line32(result, "fileAlignment", fileAlignment);
                result.AppendLine();
                Line16(result, "majorOsVersion", majorOsVersion);
                result.AppendLine();
                Line16(result, "minorOsVersion", minorOsVersion);
                result.AppendLine();
                Line16(result, "majorImageVersion", majorImageVersion);
                result.AppendLine();
                Line16(result, "minorImageVersion", minorImageVersion);
                result.AppendLine();
                Line16(result, "majorSubsystemVersion", majorSubsystemVersion);
                result.AppendLine();
                Line16(result, "minorSubsystemVersion", minorSubsystemVersion);
                result.AppendLine();
                Line32(result, "sizeOfImage", sizeOfImage);
                result.AppendLine();
                Line32(result, "sizeOfHeaders", sizeOfHeaders);
                result.AppendLine();
                Line32(result, "checksum", checksum);
                result.AppendLine();
                Line16(result, "subsystem", subsystem);
                result.AppendLine();
                Line16(result, "dllCharacteristics", dllCharacteristics);
                result.AppendLine();
                Line64(result, "sizeOfStackReserve", sizeOfStackReserve);
                result.AppendLine();
                Line64(result, "sizeOfStackCommit", sizeOfStackCommit);
                result.AppendLine();
                Line64(result, "sizeOfHeapReserve", sizeOfHeapReserve);
                result.AppendLine();
                Line64(result, "sizeOfHeapCommit", sizeOfHeapCommit);
                result.AppendLine();
                Line32(result, "loaderFlags", loaderFlags);
                result.AppendLine();
                Line32(result, "numberOfRVAAndSizes", numberOfRVAAndSizes);
                return result.ToString();
            }
        }

        public class DataDirectoryRecord
        {
            public int relativeVirtualAddress;
            public int size;

            public DataDirectoryRecord(Stream stream)
            {
                relativeVirtualAddress = ReadLE32(stream);
                size = ReadLE32(stream);
            }

            public override string ToString()
            {
                if (relativeVirtualAddress == 0 && size == 0)
                    return "<unspecified>";
                else
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "RVA 0x{0:X8} ({0}), Size 0x{1:X8} ({1})",
                        relativeVirtualAddress, size);
            }
        }

        public class SectionHeader
        {
            [Flags]
            public enum SectionFlags : uint
            {
                TYPE_NO_PAD = 0x00000008,
                CNT_CODE = 0x00000020,
                CNT_INITIALIZED_DATA = 0x00000040,
                CNT_UNINITIALIZED_DATA = 0x00000080,
                MEM_DISCARDABLE = 0x02000000,
                MEM_EXECUTE = 0x20000000,
                MEM_READ = 0x40000000,
                MEM_WRITE = 0x80000000,
            }

            public string name;
            public int virtualSize;
            public int virtualAddress;
            public int sizeOfRawData;
            public int pointerToRawData;
            public int pointerToRelocations;
            public int pointerToLineNumbers;
            public short numberOfRelocations;
            public short numberOfLineNumbers;
            public SectionFlags characteristics;

            public SectionHeader(Stream source)
            {
                name = ReadString(8, source);
                virtualSize = ReadLE32(source);
                virtualAddress = ReadLE32(source);
                sizeOfRawData = ReadLE32(source);
                pointerToRawData = ReadLE32(source);
                pointerToRelocations = ReadLE32(source);
                pointerToLineNumbers = ReadLE32(source);
                numberOfRelocations = ReadLE16(source);
                numberOfLineNumbers = ReadLE16(source);
                characteristics = (SectionFlags)ReadLE32(source);
            }

            public bool ContainsMappedAddress(int relativeVirtualAddress)
            {
                return relativeVirtualAddress >= virtualAddress &&
                    relativeVirtualAddress < virtualAddress + virtualSize;
            }

            public override string ToString()
            {
                var result = new StringBuilder();
                result.Append("name: " + name);
                result.AppendLine();
                Line32(result, "characteristics", (int)characteristics);
                result.Append(" [" + characteristics.ToString() + "]");
                result.AppendLine();
                Line32(result, "virtualSize", virtualSize);
                result.AppendLine();
                Line32(result, "virtualAddress", virtualAddress);
                result.AppendLine();
                Line32(result, "sizeOfRawData", sizeOfRawData);
                result.AppendLine();
                Line32(result, "pointerToRawData", pointerToRawData);
                result.AppendLine();
                Line16(result, "numberOfRelocations", numberOfRelocations);
                result.AppendLine();
                Line32(result, "pointerToRelocations", pointerToRelocations);
                result.AppendLine();
                Line16(result, "numberOfLineNumbers", numberOfLineNumbers);
                result.AppendLine();
                Line32(result, "pointerToLineNumbers", pointerToLineNumbers);
                return result.ToString();
            }
        }

        public class ExportDirectoryTable
        {
            public int flags;
            public int timeDateStamp;
            public short majorVersion;
            public short minorVersion;
            public int nameRva;
            public int ordinalBase;
            public int numberOfAddressTableEntries;
            public int numberOfNameOrdinalPointerEntries;
            public int addressTableRva;
            public int namePointerRva;
            public int ordinalRva;

            public ExportDirectoryTable(Stream stream)
            {
                flags = ReadLE32(stream);
                timeDateStamp = ReadLE32(stream);
                majorVersion = ReadLE16(stream);
                minorVersion = ReadLE16(stream);
                nameRva = ReadLE32(stream);
                ordinalBase = ReadLE32(stream);
                numberOfAddressTableEntries = ReadLE32(stream);
                numberOfNameOrdinalPointerEntries = ReadLE32(stream);
                addressTableRva = ReadLE32(stream);
                namePointerRva = ReadLE32(stream);
                ordinalRva = ReadLE32(stream);
            }

            public override string ToString()
            {
                var result = new StringBuilder();
                Line32(result, "flags", flags);
                result.AppendLine();
                Line32(result, "timeDateStamp", timeDateStamp);
                result.AppendLine();
                Line16(result, "majorVersion", majorVersion);
                result.AppendLine();
                Line16(result, "minorVersion", minorVersion);
                result.AppendLine();
                Line32(result, "nameRva", nameRva);
                result.AppendLine();
                Line32(result, "ordinalBase", ordinalBase);
                result.AppendLine();
                Line32(result, "numberOfAddressTableEntries", numberOfAddressTableEntries);
                result.AppendLine();
                Line32(result, "numberOfNameOrdinalPointerEntries",
                    numberOfNameOrdinalPointerEntries);
                result.AppendLine();
                Line32(result, "addressTableRva", addressTableRva);
                result.AppendLine();
                Line32(result, "namePointerRva", namePointerRva);
                result.AppendLine();
                Line32(result, "ordinalRva", ordinalRva);
                return result.ToString();
            }
        }
        class ImportDirectoryTableEntry
        {
            public int importLookupTableRva;
            public int timeDateStamp;
            public int forwarderChain;
            public int nameRva;
            public int importAddressTableRva;

            public ImportDirectoryTableEntry(Stream stream)
            {
                importLookupTableRva = ReadLE32(stream);
                timeDateStamp = ReadLE32(stream);
                forwarderChain = ReadLE32(stream);
                nameRva = ReadLE32(stream);
                importAddressTableRva = ReadLE32(stream);
            }

            public bool IsNull
            {
                get
                {
                    return importLookupTableRva == 0 &&
                      timeDateStamp == 0 &&
                      forwarderChain == 0 &&
                      nameRva == 0 &&
                      importAddressTableRva == 0;
                }
            }
        }

        public static void Inspect(string path, TextWriter output)
        {
            using (var stream = File.OpenRead(path))
                Inspect(stream, output);
        }

        static void SeekRVA(Stream stream, int rva, SectionHeader[] sections)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                if (sections[i].ContainsMappedAddress(rva))
                {
                    stream.Seek(sections[i].pointerToRawData +
                        (rva - sections[i].virtualAddress), SeekOrigin.Begin);
                    return;
                }
            }
            throw new InvalidDataException("Couldn't locate section data");
        }

        static string ReadAsciiString(Stream source)
        {
            var bytes = new List<byte>();
            while (true)
            {
                int b = source.ReadByte();
                if (b == -1)
                    throw new InvalidDataException("Unexpected end of file");
                if (b == 0)
                    break;
                bytes.Add((byte)b);
            }

            return Encoding.ASCII.GetString(bytes.ToArray());
        }

        static void DumpRawBytes(Stream source, int numBytes, TextWriter output)
        {
            for (int i = 0; i < numBytes; i++)
            {
                int b = source.ReadByte();
                output.Write("{0:X2}{1} ", b,
                    Encoding.ASCII.GetString(new byte[] { (byte)b }));
            }
        }

        public static void Inspect(Stream stream, TextWriter output)
        {
            stream.Seek(0, SeekOrigin.Begin);
            Expect(stream, "File identifier incorrect", 0x4D, 0x5A); // MZ

            stream.Seek(0x3C, SeekOrigin.Begin);
            int peHeaderOffset = ReadLE16(stream);

            stream.Seek(peHeaderOffset, SeekOrigin.Begin);
            Expect(stream, "PE identifier incorrect", 0x50, 0x45, 0x00, 0x00); // PE\0\0

            var coffHeader = new CoffHeader(stream);
            var standardOptionalHeader = new OptionalHeaderStandardSection(stream);
            var windowsOptionalHeader = new OptionalHeaderWindowsSection(stream,
                standardOptionalHeader.magic ==
                OptionalHeaderStandardSection.Magic.PE32PlustExecutable);
            var dataDirectories = new DataDirectoryRecord[
                windowsOptionalHeader.numberOfRVAAndSizes];
            for (int i = 0; i < windowsOptionalHeader.numberOfRVAAndSizes; i++)
                dataDirectories[i] = new DataDirectoryRecord(stream);
            var sectionHeaders = new SectionHeader[coffHeader.numberOfSections];
            for (int i = 0; i < coffHeader.numberOfSections; i++)
                sectionHeaders[i] = new SectionHeader(stream);

            SeekRVA(stream, dataDirectories[0].relativeVirtualAddress, sectionHeaders);
            var exportDirectory = new ExportDirectoryTable(stream);

            var nameRVAs = new List<int>();
            SeekRVA(stream, exportDirectory.namePointerRva, sectionHeaders);
            for (int i = 0; i < exportDirectory.numberOfNameOrdinalPointerEntries; i++)
                nameRVAs.Add(ReadLE32(stream));

            SeekRVA(stream, dataDirectories[1].relativeVirtualAddress, sectionHeaders);
            var importDirectoryTable = ReadImportDirectoryTable(stream);

            output.WriteLine();
            output.WriteLine("--- COFF header ---");
            output.WriteLine(coffHeader);
            output.WriteLine();
            output.WriteLine("--- Standard Optional Header ---");
            output.WriteLine(standardOptionalHeader);
            output.WriteLine();
            output.WriteLine("--- Windows Optional Header---");
            output.WriteLine(windowsOptionalHeader);

            output.WriteLine("");
            output.WriteLine("--- Optional Header Data Directories ---");
            var ddTypes = new string[]
            {
                "Export table",
                "Import table",
                "Resource table",
                "Exception table",
                "Certificate table",
                "Base Relocation table",
                "Debug",
                "Architecture",
                "Global pointer",
                "TLS table",
                "Load config table",
                "Bound import table",
                "Import address table",
                "Delay import deacriptor",
                "COM+ runtime header",
                "Reserved"
            };
            for (int i = 0; i < dataDirectories.Length; i++)
            {
                output.Write("{0:D2} ({1}): ", i, ddTypes[i]);
                output.Write(dataDirectories[i]);
                output.Write(" ");
                for (int j = 0; j < sectionHeaders.Length; j++)
                    if (sectionHeaders[j].ContainsMappedAddress(
                            dataDirectories[i].relativeVirtualAddress))
                        output.Write(sectionHeaders[j].name);
                output.WriteLine();
            }

            output.WriteLine("");
            output.WriteLine("--- Section Headers ---");
            for (int i = 0; i < sectionHeaders.Length; i++)
            {
                output.WriteLine("- - Section {0:D2} - -", i);
                output.WriteLine(sectionHeaders[i]);
            }

            output.WriteLine();
            output.WriteLine("--- Export Directory ---");
            output.WriteLine(exportDirectory);


            /*output.WriteLine();
            output.WriteLine("--- Exported Image Name ---");
            SeekRVA(stream, exportDirectory.nameRva, sectionHeaders);
            output.WriteLine(ReadAsciiString(stream));

            output.WriteLine();
            output.WriteLine("--- Exported Names ---");
            for (int i = 0; i < nameRVAs.Count; i++)
            {
                SeekRVA(stream, nameRVAs[i], sectionHeaders);
                output.Write("{0:D4}: ", i);
                output.WriteLine(ReadAsciiString(stream));
            }*/

            output.WriteLine();
            output.WriteLine("--- Imported Functions ---");
            for (int i = 0; i < importDirectoryTable.Count; i++)
            {
                output.Write("Import file name: ");
                SeekRVA(stream, importDirectoryTable[i].nameRva, sectionHeaders);
                output.WriteLine(ReadAsciiString(stream));
                output.WriteLine("Lookup RVA: 0x{0:x8}",
                    importDirectoryTable[i].importLookupTableRva);
                output.WriteLine("Address RVA: 0x{0:x8}",
                    importDirectoryTable[i].importAddressTableRva);

                SeekRVA(stream, importDirectoryTable[i].importLookupTableRva, sectionHeaders);
                while (true)
                {
                    if (standardOptionalHeader.magic ==
                        OptionalHeaderStandardSection.Magic.PE32PlustExecutable)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        var lutEntry = ReadLE32(stream);
                        if (lutEntry == 0)
                            break;

                        output.Write("\t0x{0:x8} ", lutEntry);

                        if ((lutEntry & 0x80000000) == 0)
                        {
                            long bookmark = stream.Position;
                            SeekRVA(stream, lutEntry, sectionHeaders);

                            {
                                short hint = ReadLE16(stream);
                                output.WriteLine(ReadAsciiString(stream) + " [" + hint + "]");
                            }


                            stream.Seek(bookmark, SeekOrigin.Begin);
                        }
                        else
                        {
                            output.WriteLine("\tOrdinal" + (lutEntry & 0x7FFFFFFF));
                        }
                    }
                }
            }
        }

        static List<ImportDirectoryTableEntry> ReadImportDirectoryTable(Stream stream)
        {
            var result = new List<ImportDirectoryTableEntry>();
            while (true)
            {
                var entry = new ImportDirectoryTableEntry(stream);
                if (entry.IsNull)
                    break;
                result.Add(entry);
            }
            return result;
        }
    }
}
#endif
