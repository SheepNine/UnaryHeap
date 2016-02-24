using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace UnaryHeap.Utilities.Doom
{
    /// <summary>
    /// Represents a WAD file (as used by DooM and DooM II), providing access to the
    /// lumps contained therein.
    /// </summary>
    public class WadFile
    {
        byte[] data;

        /// <summary>
        /// Initializes a new instance of the WadFile class.
        /// </summary>
        /// <param name="source">The stream from which to read the WAD file data.</param>
        /// <exception cref="System.ArgumentNullException">source is null.</exception>
        /// <exception cref="System.IO.InvalidDataException">The data in source is not
        /// correctly-formatted WAD data.</exception>
        public WadFile(Stream source)
        {
            if (null == source)
                throw new ArgumentNullException("source");

            using (var buffer = new MemoryStream())
            {
                source.CopyTo(buffer);
                Init(buffer.ToArray());
            }
        }

        /// <summary>
        /// Initializes a new instance of the WadFile class.
        /// </summary>
        /// <param name="fileName">The file from which to read the WAD file data.</param>
        /// <exception cref="System.ArgumentNullException">filename is null.</exception>
        /// <exception cref="System.IO.InvalidDataException">The data in the file specified
        /// is not correctly-formatted WAD data.</exception>
        public WadFile(string fileName)
        {
            if (null == fileName)
                throw new ArgumentNullException("fileName");

            Init(File.ReadAllBytes(fileName));
        }

        /// <summary>
        /// Initializes a new instance of the WadFile class.
        /// </summary>
        /// <param name="data">The WAD file data.</param>
        /// <exception cref="System.ArgumentNullException">data is null.</exception>
        /// <exception cref="System.IO.InvalidDataException">The data specified
        /// is not correctly-formatted WAD data.</exception>
        public WadFile(byte[] data)
        {
            if (null == data)
                throw new ArgumentNullException("data");
            if (data.Length < 12)
                throw new InvalidDataException(
                    "WAD files must be at least twelve bytes in size.");

            Init(data);
        }

        void Init(byte[] source)
        {
            data = new byte[source.Length];
            Array.Copy(source, data, source.Length);

            var identification = Encoding.ASCII.GetString(data, 0, 4);

            if (!identification.Equals("IWAD") && !identification.Equals("PWAD"))
                throw new InvalidDataException(
                    "Invalid WAD identifier. Valid values are 'IWAD' and 'PWAD'.");

            if (0 > LumpCount)
                throw new InvalidDataException("Negative lump count.");
            if (false == CheckDataRange(DirectoryOffset, LumpCount * 16))
                throw new InvalidDataException("WAD directory does not lie within file.");

            for (int i = 0; i < LumpCount; i++)
            {
                if (0 > GetLumpDataOffset(i))
                    throw new InvalidDataException(string.Format(
                        CultureInfo.InvariantCulture, "Negative offset at lump {0}.", i));
                if (0 > GetLumpSize(i))
                    throw new InvalidDataException(string.Format(
                        CultureInfo.InvariantCulture, "Negative size at lump {0}.", i));

                if (false == CheckDataRange(GetLumpDataOffset(i), GetLumpSize(i)))
                    throw new InvalidDataException(string.Format(
                        CultureInfo.InvariantCulture, "Lump {0} does not lie within file.", i));
            }
        }

        bool CheckDataRange(int offset, int size)
        {
            var start = offset;
            var end = offset + size;

            return start >= 0 && end <= data.Length;
        }

        /// <summary>
        /// Returns true if this WAD is a patch wad, or false if this wad is an initial wad.
        /// </summary>
        public bool IsPatchWad
        {
            get { return data[0] == 0x50; }
        }

        /// <summary>
        /// Gets the number of lumps in this WAD.
        /// </summary>
        public int LumpCount
        {
            get { return ReadLittleEndianInt32(data, 4); }
        }

        int DirectoryOffset
        {
            get { return ReadLittleEndianInt32(data, 8); }
        }

        /// <summary>
        /// Gets the name of the specified lump.
        /// </summary>
        /// <param name="index">The index of the lump for which to get the name.</param>
        /// <returns>The name of the specified lump.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is negative or not less than LumpCount.</exception>
        public string GetLumpName(int index)
        {
            if (false == LumpIndexInRange(index))
                throw new ArgumentOutOfRangeException("index");

            // Range check on directoryEntryStart computation to make Code Analysis happy
            if (index > (Int32.MaxValue - DirectoryOffset) / 16)
                throw new ArgumentOutOfRangeException("index", "WAD file too large.");

            int directoryEntryStart = DirectoryOffset + 16 * index;
            return ReadString(data, directoryEntryStart + 8);
        }

        int GetLumpDataOffset(int index)
        {
            int directoryEntryStart = DirectoryOffset + 16 * index;
            return ReadLittleEndianInt32(data, directoryEntryStart);
        }

        /// <summary>
        /// Gets the size of the specified lump.
        /// </summary>
        /// <param name="index">The index of the lump for which to get the size.</param>
        /// <returns>The size of the specified lump.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is negative or not less than LumpCount.</exception>
        public int GetLumpSize(int index)
        {
            if (false == LumpIndexInRange(index))
                throw new ArgumentOutOfRangeException("index");

            // Range check on directoryEntryStart computation to make Code Analysis happy
            if (index > (Int32.MaxValue - DirectoryOffset) / 16)
                throw new ArgumentOutOfRangeException("index", "WAD file too large.");

            int directoryEntryStart = DirectoryOffset + 16 * index;
            return ReadLittleEndianInt32(data, directoryEntryStart + 4);
        }

        /// <summary>
        /// Gets the data of the specified lump.
        /// </summary>
        /// <param name="index">The index of the lump for which to get the data.</param>
        /// <returns>The data of the specified lump.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index is negative or not less than LumpCount.</exception>
        public byte[] GetLumpData(int index)
        {
            if (false == LumpIndexInRange(index))
                throw new ArgumentOutOfRangeException("index");

            return Subset(GetLumpDataOffset(index), GetLumpSize(index));
        }

        byte[] Subset(int offset, int size)
        {
            if (0 == size)
                return new byte[0];

            var result = new byte[size];
            Array.Copy(data, offset, result, 0, size);
            return result;
        }

        /// <summary>
        /// Looks up the index of a lump by name.
        /// </summary>
        /// <param name="lumpName">The name of the lump to find.</param>
        /// <returns>The index of the first lump whose name matches lumpName,
        /// or -1 of no lump is found.</returns>
        public int FindLumpByName(string lumpName)
        {
            return FindLumpByName(lumpName, 0);
        }

        /// <summary>
        /// Looks up the index of a lump by name, skipping some indices.
        /// </summary>
        /// <param name="lumpName">The name of the lump to find.</param>
        /// <param name="searchStart">The first lump name to inspect.</param>
        /// <returns>The index of the first lump whose name matches lumpName and
        /// whose index is not less than searchStart, or -1 of no lump is found.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// searchStart is negative or not less than LumpCount.</exception>
        public int FindLumpByName(string lumpName, int searchStart)
        {
            if (null == lumpName)
                throw new ArgumentNullException("lumpName");
            if (8 < lumpName.Length)
                throw new ArgumentOutOfRangeException("lumpName",
                    "Lump names may not exceed eight characters.");

            if (0 == LumpCount)
                return -1;

            if (false == LumpIndexInRange(searchStart))
                throw new ArgumentOutOfRangeException("searchStart");

            for (int i = searchStart; i < LumpCount; i++)
            {
                if (lumpName == GetLumpName(i))
                    return i;
            }

            return -1;
        }

        bool LumpIndexInRange(int index)
        {
            return index >= 0 && index < LumpCount;
        }

        /// <summary>
        /// Reads a little-endian encoded 32-bit signed integer from a DooM WAD.
        /// </summary>
        /// <param name="bytes">The contents of the WAD file.</param>
        /// <param name="offset">The index of the first (lowest-order) byte
        /// of the value to be read.</param>
        /// <returns>The 32-bit signed integer at the specified index.</returns>
        public static int ReadLittleEndianInt32(byte[] bytes, int offset)
        {
            if (null == bytes)
                throw new ArgumentNullException("bytes");

            if (0 > offset || bytes.Length - 4 < offset)
                throw new ArgumentOutOfRangeException("offset");

            return (((((bytes[offset + 3] << 8)
                | bytes[offset + 2]) << 8)
                | bytes[offset + 1]) << 8)
                | bytes[offset + 0];
        }

        /// <summary>
        /// Reads a string from a DooM WAD.
        /// </summary>
        /// <param name="bytes">The contents of the WAD file.</param>
        /// <param name="offset">The index of the first (lowest-order) byte
        /// of the value to be read.</param>
        /// <returns>The string value of the WAD, with trailing nulls removed.</returns>
        public static string ReadString(byte[] bytes, int offset)
        {
            if (null == bytes)
                throw new ArgumentNullException("bytes");

            if (0 > offset || bytes.Length - 8 < offset)
                throw new ArgumentOutOfRangeException("offset");

            var result = Encoding.ASCII.GetString(bytes, offset, 8);
            var firstNullIndex = result.IndexOf((char)0);

            if (-1 == firstNullIndex)
                return result;
            else
                return result.Substring(0, firstNullIndex);
        }
    }
}
