using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace UnaryHeap.Utilities.Misc
{
    /// <summary>
    /// Encapsulates an algorithm for generating an animated GIF image stream from a
    /// collection of dynamically-generated image frames.
    /// </summary>
    public abstract class GifGenerator
    {
        #region Abstractions

        /// <summary>
        /// Callback to render a frame of animation.
        /// </summary>
        /// <param name="g">The target graphics context upon which to render the frame.</param>
        /// <param name="frameIndex">The frame index of the animation. Ranges from 0 to FrameCount - 1.</param>
        protected abstract void RenderFrame(Graphics g, int frameIndex);

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the GifGenerator class.
        /// </summary>
        /// <param name="width">The width, in pixels, of the output image.</param>
        /// <param name="height">The height, in pixels, of the output image.</param>
        /// <param name="frameCount">The numer of frames in the animation.</param>
        protected GifGenerator(int width, int height, int frameCount)
        {
            if (width < 1)
                throw new ArgumentOutOfRangeException("width");
            if (height < 1)
                throw new ArgumentOutOfRangeException("height");
            if (frameCount < 1)
                throw new ArgumentOutOfRangeException("frameCount");

            Width = width;
            Height = height;
            FrameCount = frameCount;
        }

        #endregion


        #region Properties

        /// <summary>
        /// The width, in pixels, of the output image.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height, in pixels, of the output image.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The numer of frames in the animation.
        /// </summary>
        public int FrameCount { get; private set; }

        #endregion


        #region Public methods

        /// <summary>
        /// Writes the GIF image stream of this GifGenerator to the specified filename.
        /// </summary>
        /// <param name="outputFileName">The filename of the resulting file.</param>
        public void Generate(string outputFileName)
        {
            using (var file = File.Create(outputFileName))
                Generate(file);
        }

        /// <summary>
        /// Writes the GIF image stream of this GifGenerator to the specified stream.
        /// </summary>
        /// <param name="output">The stream into which the result will be written.</param>
        public void Generate(Stream output)
        {
            if (FrameCount > 1)
                ProduceBitmapEncoder().Save(output);
            else
                ProduceSingleFrameImage(output);
        }

        #endregion


        #region Helper methods

        GifBitmapEncoder ProduceBitmapEncoder()
        {
            var result = new GifBitmapEncoder();

            using (var buffer = new Bitmap(Width, Height))
                foreach (var frameIndex in Enumerable.Range(0, FrameCount))
                {
                    RenderFrameToBuffer(buffer, frameIndex);
                    AppendBufferToEncoder(buffer, result);
                }

            return result;
        }

        void RenderFrameToBuffer(Bitmap buffer, int frameIndex)
        {
            using (var g = Graphics.FromImage(buffer))
                RenderFrame(g, frameIndex);
        }

        static void AppendBufferToEncoder(Bitmap buffer, BitmapEncoder encoder)
        {
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(buffer.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            var bitmapFrame = BitmapFrame.Create(bitmapSource);
            encoder.Frames.Add(bitmapFrame);
        }

        void ProduceSingleFrameImage(Stream output)
        {
            using (var buffer = new Bitmap(Width, Height))
            {
                RenderFrameToBuffer(buffer, 0);
                buffer.Save(output, ImageFormat.Gif);
            }
        }

        #endregion
    }
}