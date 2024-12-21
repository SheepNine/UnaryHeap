using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UnaryHeap.Utilities
{
    /// <summary>
    /// Represents an in-memory 8bpp RGB image that can be efficiently
    /// manipulated on a per-pixel basis.
    /// </summary>
    public class RawImage
    {
        #region Constants

        const int BytesPerPixel = 3;

        #endregion


        #region Member Variables

        byte[] data;
        int width;
        int height;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RawImage class.
        /// </summary>
        /// <param name="width">The width of the image, in pixels.</param>
        /// <param name="height">The height of the image, in pixels</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// width or height are less than one</exception>
        public RawImage(int width, int height)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);

            this.width = width;
            this.height = height;
            data = new byte[width * height * BytesPerPixel];
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the width of the image, in pixels.
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Gets the height of the image, in pixels.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Retrieves the color of a specified pixel.
        /// </summary>
        /// <param name="x">The X-coordinate of the pxiel to retrieve.</param>
        /// <param name="y">The Y-coordinate of the pixel to retrieve.</param>
        /// <param name="r">The red component of the specified pixel.</param>
        /// <param name="g">The green component of the specified pixel.</param>
        /// <param name="b">The blue component of the specified pixel.</param>
        /// <exception cref="ArgumentOutOfRangeException">x/y are less than zero
        /// or greater than or equal to Width/Height.</exception>
        public void GetPixel(int x, int y, out byte r, out byte g, out byte b)
        {
            var index = Index(x, y);
            r = data[index];
            index += 1;
            g = data[index];
            index += 1;
            b = data[index];
        }

        /// <summary>
        /// Updates the color of a specified pixel.
        /// </summary>
        /// <param name="x">The X-coordinate of the pixel to update.</param>
        /// <param name="y">The Y-coordinate of the pixel to update.</param>
        /// <param name="r">
        /// The value to which to set the red component of the pixel specified.</param>
        /// <param name="g">
        /// The value to which to set the green component of the pixel specified.</param>
        /// <param name="b">
        /// The value to which to set the blue component of the pixel specified.</param>
        /// <exception cref="ArgumentOutOfRangeException">x/y are less than zero
        /// or greater than or equal to Width/Height.</exception>
        public void SetPixel(int x, int y, byte r, byte g, byte b)
        {
            var index = Index(x, y);
            data[index] = r;
            index += 1;
            data[index] = g;
            index += 1;
            data[index] = b;
        }

        /// <summary>
        /// Updates the color of a specified pixel to be the average of its existing
        /// color and a new color.
        /// </summary>
        /// <param name="x">The X-coordinate of the pixel to update.</param>
        /// <param name="y">The Y-coordinate of the pixel to update.</param>
        /// <param name="r">
        /// The value to which to blend the red component of the pixel specified.</param>
        /// <param name="g">
        /// The value to which to blend the green component of the pixel specified.</param>
        /// <param name="b">
        /// The value to which to blend the blue component of the pixel specified.</param>
        /// <exception cref="ArgumentOutOfRangeException">x/y are less than zero
        /// or greater than or equal to Width/Height.</exception>
        public void BlendPixel(int x, int y, byte r, byte g, byte b)
        {
            var index = Index(x, y);
            data[index] = (byte)((data[index] + r) >> 1);
            index += 1;
            data[index] = (byte)((data[index] + g) >> 1);
            index += 1;
            data[index] = (byte)((data[index] + b) >> 1);
        }

        /// <summary>
        /// Creates a GDI Bitmap of this RawImage.
        /// </summary>
        /// <returns>A Bitmap containing the current color values of the RawImage.</returns>
        public Bitmap MakeBitmap()
        {
            var result = new Bitmap(width, height);

            try
            {
                var bitmapData = result.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
                result.UnlockBits(bitmapData);
                return result;
            }
            catch (Exception)
            {
                result.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Writes a binary representation of the current RawImage value to a stream.
        /// </summary>
        /// <param name="output">The stream to which to write the binary representation.</param>
        /// <exception cref="ArgumentNullException">output is null.</exception>
        public void Serialize(Stream output)
        {
            ArgumentNullException.ThrowIfNull(output);

            var writer = new BinaryWriter(output, Encoding.ASCII, true);
            writer.Write(width);
            writer.Write(height);
            output.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Reads the binary representation of a RawImage object from a stream.
        /// </summary>
        /// <param name="input">The stream from which to read the binary representation.</param>
        /// <returns>The RawImage value read.</returns>
        /// <exception cref="ArgumentNullException">input is null.</exception>
        public static RawImage Deserialize(Stream input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var reader = new BinaryReader(input, Encoding.ASCII, true);
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var result = new RawImage(width, height);
            reader.Read(result.data, 0, result.data.Length);
            return result;
        }

        #endregion


        #region Helper Methods

        int Index(int x, int y)
        {
            CheckRange(x, y);
            return (x + y * width) * BytesPerPixel;
        }

        void CheckRange(int x, int y)
        {
            if (x >= width || x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y >= height || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
        }

        #endregion
    }
}
