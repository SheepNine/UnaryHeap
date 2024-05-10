using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace UnaryHeap.Quake
{
    /// <summary>
    /// Represents a BSP file.
    /// </summary>
    public class BspFile
    {
        const int BSP_VERSION = 29;

        /// <summary>
        /// The PAK file name of this BSP file.
        /// </summary>
        public string Name { get; private set; }
        readonly byte[] entities;
        readonly byte[] planes;
        readonly Texture[] textures;
        readonly byte[] vertexes;
        readonly byte[] visibility;
        readonly byte[] nodes;
        readonly byte[] texInfo;
        readonly byte[] faces;
        readonly byte[] lighting;
        readonly byte[] clipNodes;
        readonly byte[] leafs;
        readonly byte[] markSurfaces;
        readonly byte[] edges;
        readonly byte[] surfEdges;
        readonly byte[] models;

        /// <summary>
        /// Initializes a new instance of the BspFile class.
        /// </summary>
        /// <param name="name">The PAK file name of this BSP file.</param>
        /// <param name="dataStream">The BSP data to read.</param>
        /// <exception cref="ArgumentException">
        /// The data does not have the correct identifier</exception>
        public BspFile(string name, Stream dataStream)
        {
            Name = name;
            dataStream.Seek(0, SeekOrigin.Begin);
            int magic = ReadLeInt32(dataStream);
            if (magic != BSP_VERSION)
                throw new ArgumentException("Not a BSP file");
            entities = ReadLump(dataStream);
            planes = ReadLump(dataStream);
            textures = ReadTextures(dataStream);
            vertexes = ReadLump(dataStream);
            visibility = ReadLump(dataStream);
            nodes = ReadLump(dataStream);
            texInfo = ReadLump(dataStream);
            faces = ReadLump(dataStream);
            lighting = ReadLump(dataStream);
            clipNodes = ReadLump(dataStream);
            leafs = ReadLump(dataStream);
            markSurfaces = ReadLump(dataStream);
            edges = ReadLump(dataStream);
            surfEdges = ReadLump(dataStream);
            models = ReadLump(dataStream);
        }

        /// <summary>
        /// Gets the textures from the BSP.
        /// </summary>
        public IEnumerable<Texture> Textures
        {
            get { return textures; }
        }

        static Texture[] ReadTextures(Stream dataStream)
        {
            var lumpOffset = ReadLeInt32(dataStream);
            var lumpSize = ReadLeInt32(dataStream);
            var currentPosition = dataStream.Position;

            dataStream.Seek(lumpOffset, SeekOrigin.Begin);
            var textureCount = ReadLeInt32(dataStream);
            var result = new Texture[textureCount];
            for (int i = 0; i < textureCount; i++)
            {
                result[i] = ReadTexture(dataStream, lumpOffset);
            }

            dataStream.Seek(currentPosition, SeekOrigin.Begin);
            return result;
        }

        static Texture ReadTexture(Stream dataStream, int lumpOffset)
        {
            var textureOffset = ReadLeInt32(dataStream);
            var currentPosition = dataStream.Position;

            // Verify this hack with the Quake source code
            if (textureOffset == -1)
                return null;

            dataStream.Seek(lumpOffset + textureOffset, SeekOrigin.Begin);

            var name = ReadString(dataStream, 16);
            var width = ReadLeInt32(dataStream);
            var height = ReadLeInt32(dataStream);
            var offset0 = ReadLeInt32(dataStream);
            var offset1 = ReadLeInt32(dataStream);
            var offset2 = ReadLeInt32(dataStream);
            var offset3 = ReadLeInt32(dataStream);

            dataStream.Seek(lumpOffset + textureOffset + offset0, SeekOrigin.Begin);
            var mip0 = ReadBytes(dataStream, width * height);
            dataStream.Seek(lumpOffset + textureOffset + offset1, SeekOrigin.Begin);
            var mip1 = ReadBytes(dataStream, (width * height) >> 2);
            dataStream.Seek(lumpOffset + textureOffset + offset2, SeekOrigin.Begin);
            var mip2 = ReadBytes(dataStream, (width * height) >> 4);
            dataStream.Seek(lumpOffset + textureOffset + offset3, SeekOrigin.Begin);
            var mip3 = ReadBytes(dataStream, (width * height) >> 6);

            var result = new Texture(name, width, height, mip0, mip1, mip2, mip3);

            dataStream.Seek(currentPosition, SeekOrigin.Begin);
            return result;
        }

        static string ReadString(Stream dataStream, int size)
        {
            var result = Encoding.ASCII.GetString(ReadBytes(dataStream, size));
            var nullIndex = result.IndexOf('\0');
            if (nullIndex != -1)
                result = result.Substring(0, nullIndex);
            return result;
        }

        static byte[] ReadLump(Stream dataStream)
        {
            var lumpOffset = ReadLeInt32(dataStream);
            var lumpSize = ReadLeInt32(dataStream);
            var currentPosition = dataStream.Position;

            dataStream.Seek(lumpOffset, SeekOrigin.Begin);
            var result = ReadBytes(dataStream, lumpSize);

            dataStream.Seek(currentPosition, SeekOrigin.Begin);
            return result;
        }

        static int ReadLeInt32(Stream dataStream)
        {
            var buffer = ReadBytes(dataStream, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        static uint ReadLeUint32(Stream dataStream)
        {
            var buffer = ReadBytes(dataStream, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        static byte[] ReadBytes(Stream dataStream, int size)
        {
            var buffer = new byte[size];
            var bytesRead = dataStream.Read(buffer, 0, size);
            if (bytesRead != size)
                throw new InvalidDataException("Failed to read string");
            return buffer;
        }

        static byte ReadByte(Stream dataStream)
        {
            var result = dataStream.ReadByte();
            if (result == -1)
                throw new InvalidDataException("Expected a byte; found none");
            return (byte)result;
        }

        /// <summary>
        /// Represents a texture from a BSP file.
        /// </summary>
        public class Texture
        {
            /// <summary>
            /// The name of the texture.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// The width of the texture.
            /// </summary>
            public int Width { get; private set; }

            /// <summary>
            /// The height of the texture.
            /// </summary>
            public int Height { get; private set; }

            /// <summary>
            /// The data for the full-size mipmap.
            /// </summary>
            public byte[] Mip0 { get; private set; }

            /// <summary>
            /// The data for the half-size mipmap.
            /// </summary>
            public byte[] Mip1 { get; private set; }

            /// <summary>
            /// The data for the quarter-size mipmap.
            /// </summary>
            public byte[] Mip2 { get; private set; }

            /// <summary>
            /// The data for the eighth-size mipmap.
            /// </summary>
            public byte[] Mip3 { get; private set; }

            /// <summary>
            /// Initializes a new instance of the Texture file.
            /// </summary>
            /// <param name="name">The name of the texture.</param>
            /// <param name="width">The width of the texture.</param>
            /// <param name="height">The height of the texture.</param>
            /// <param name="mip0">The data for the full-size mipmap.</param>
            /// <param name="mip1">The data for the half-size mipmap.</param>
            /// <param name="mip2">The data for the quarter-size mipmap.</param>
            /// <param name="mip3">The data for the eighth-size mipmap.</param>
            public Texture(string name, int width, int height,
                byte[] mip0, byte[] mip1, byte[] mip2, byte[] mip3)
            {
                Name = name;
                Width = width;
                Height = height;
                Mip0 = mip0;
                Mip1 = mip1;
                Mip2 = mip2;
                Mip3 = mip3;
            }

            /// <summary>
            /// Write one of the texture's mips to a file.
            /// </summary>
            /// <param name="palette">The palette to use for the color indices.</param>
            /// <param name="filename">The output file to which to write.</param>
            /// <param name="format">The format to use when writing the file.</param>
            /// <param name="level">The mip level, from 0 (full size) to 3 (1/8 size).</param>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            public void SaveMip(Color[] palette, string filename,
                ImageFormat format, int level = 0)
            {
                if (level > 3 || level < 0)
                    throw new ArgumentOutOfRangeException(nameof(level));

                var width = Width >> level;
                var height = Height >> level;

                byte[] mip = level switch
                {
                    0 => Mip0,
                    1 => Mip0,
                    2 => Mip0,
                    _ => Mip0,
                };

                using (var bitmap = new Bitmap(width, height))
                {
                    foreach (var y in Enumerable.Range(0, height))
                        foreach (var x in Enumerable.Range(0, width))
                            bitmap.SetPixel(x, y, palette[mip[x + y * width]]);

                    bitmap.Save(filename, format);
                }
            }

            /// <summary>
            /// Returns a String which represents the object instance.
            /// </summary>
            /// <returns>The string representation of the object.</returns>
            public override string ToString()
            {
                return $"{Name}, {Width}x{Height}";
            }
        }
    }
}
