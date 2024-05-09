using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Quake
{
    /// <summary>
    /// Class representing an idTech2 PAK file.
    /// </summary>
    public class Pak1File
    {
        class PakEntry
        {
            public string Name { get; private set; }
            public int FileOffset { get; private set; }
            public int DataSize { get; private set; }

            public PakEntry(string name, int offset, int size)
            {
                Name = name;
                FileOffset = offset;
                DataSize = size;
            }
        }

        readonly Stream data;
        readonly List<PakEntry> manifest = new();

        /// <summary>
        /// Creates a new instance of the Pak1File class.
        /// </summary>
        /// <param name="path">The filename of the .PAK file to open.</param>
        public Pak1File(string path)
            : this(File.OpenRead(path))
        {
        }

        /// <summary>
        /// Creates a new instance of the Pak1File class.
        /// </summary>
        /// <param name="dataStream">Stream containing the .PAK file data.</param>
        public Pak1File(Stream dataStream)
        {
            data = dataStream;
            data.Seek(0, SeekOrigin.Begin);
            var magic = ReadString(4);
            if (!magic.Equals("PACK", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Not a PACK file");
            var contentsOffset = ReadLeInt32();
            var contentCount = ReadLeInt32() / 64;
            data.Seek(contentsOffset, SeekOrigin.Begin);
            foreach (var i in Enumerable.Range(0, contentCount))
            {
                var name = ReadString(56);
                var offset = ReadLeInt32();
                var size = ReadLeInt32();
                manifest.Add(new PakEntry(name, offset, size));
            }
        }

        private int ReadLeInt32()
        {
            var buffer = ReadBytes(4);
            return BitConverter.ToInt32(buffer, 0);
        }

        private string ReadString(int size)
        {
            return Encoding.ASCII.GetString(ReadBytes(size)).TrimEnd('\0');
        }

        private byte[] ReadBytes(int size)
        {
            var buffer = new byte[size];
            var bytesRead = data.Read(buffer, 0, size);
            if (bytesRead != size)
                throw new InvalidDataException("Failed to read string");
            return buffer;
        }

        /// <summary>
        /// Read a file from the PAK file.
        /// </summary>
        /// <param name="filename">The name of the file to read.</param>
        /// <returns>The data of the file.</returns>
        /// <exception cref="ArgumentException">
        /// No file exists with the given name in the file</exception>
        public byte[] Read(string filename)
        {
            foreach (var entry in manifest)
            {
                if (entry.Name == filename)
                {
                    data.Seek(entry.FileOffset, SeekOrigin.Begin);
                    return ReadBytes(entry.DataSize);
                }
            }

            throw new ArgumentException("File not found in PAK file", nameof(filename));
        }

        /// <summary>
        /// Read a BSP file from the PAK file.
        /// </summary>
        /// <param name="mapName">The map name, e.g. 'e1m1'</param>
        /// <returns>The BSP file.</returns>
        public BspFile ReadMap(string mapName)
        {
            return new BspFile(new MemoryStream(Read($"maps/{mapName}.bsp")));
        }

        /// <summary>
        /// Gets the list of files in the PAK file.
        /// </summary>
        public IEnumerable<string> FileNames
        {
            get { return manifest.Select(m => m.Name); }
        }
    }
}
