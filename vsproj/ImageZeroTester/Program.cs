using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageZeroTester
{
    public static class ImageZero
    {
        [DllImport("ImageZeroLib.dll", EntryPoint = "IZ_Init")]
        public static extern void Init();

        [DllImport("ImageZeroLib.dll", EntryPoint = "IZ_Encode")]
        public static extern int Encode(byte[] src, int width, int height, byte[] dst, int dstOffset);

        [DllImport("ImageZeroLib.dll", EntryPoint = "IZ_ReadHeader")]
        public static extern int ReadHeader(byte[] src, int srcOffset, out int width, out int height, out int dataLength);

        [DllImport("ImageZeroLib.dll", EntryPoint = "IZ_Decode")]
        public static extern int Decode(byte[] src, int srcOffset, byte[] dst);
    }

    static class Program
    {
        public static byte[] BitmapToRaw(Bitmap bmp)
        {
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            var data = new byte[bmpData.Stride * bmpData.Height];
            Marshal.Copy(bmpData.Scan0, data, 0, data.Length);
            bmp.UnlockBits(bmpData);
            return data;
        }

        static void Main(string[] args)
        {
            // Make sure input exists and valid
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: ImageZeroTester <image-file>");
                return;
            }

            // Load image
            Bitmap bmp;
            byte[] rawIn;
            try
            {
                bmp = (Bitmap)Image.FromFile(args[0]);
                rawIn = BitmapToRaw(bmp);
                Console.WriteLine($"Loaded an {bmp.Width}x{bmp.Height} image");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load image: " + ex.Message);
                return;
            }

            // Build encode/decode tables
            ImageZero.Init();

            // Encode (raw->IZ)
            var compressed = new byte[bmp.Width * bmp.Height * 4];
            var outputSize = ImageZero.Encode(rawIn, bmp.Width, bmp.Height, compressed, 0);

            // Read IZ header 
            int outWidth; int outHeight; int outDataLenfth;
            ImageZero.ReadHeader(compressed, 0, out outWidth, out outHeight, out outDataLenfth);
            if (outWidth != bmp.Width || outHeight != bmp.Height || outDataLenfth != outputSize)
                throw new Exception("Error white reading compressed data header");

            // Decode (IZ->raw)
            var rawOut = new byte[bmp.Width * bmp.Height * 3];
            ImageZero.Decode(compressed, 0, rawOut);

            // Make sure output is the same is input
            if (!Enumerable.SequenceEqual(rawOut, rawIn))
                throw new Exception("Input and output are not the same");

            // Print debug
            Console.WriteLine($"Compression Rate (Output/Input): {outputSize}/{rawIn.Length} {outputSize * 100 / rawIn.Length:0.0}%");

            // Measure performance
            var encodeCounter = 0;
            var encodeTime = Stopwatch.StartNew();
            while (encodeTime.ElapsedMilliseconds < 1000)
            {
                ImageZero.Encode(rawIn, bmp.Width, bmp.Height, compressed, 0);
                encodeCounter++;
            }
            encodeTime.Stop();
            Console.WriteLine("Average Compresstion Time: " + (encodeTime.Elapsed.TotalMilliseconds / encodeCounter).ToString("0.00"));
            Console.WriteLine("MegaPixels / Second:       " + ((bmp.Width * bmp.Height * encodeCounter / encodeTime.Elapsed.TotalSeconds) / 1000000).ToString("0.0"));
            Console.WriteLine("MegaBytes  / Second:       " + ((3 * bmp.Width * bmp.Height * encodeCounter / encodeTime.Elapsed.TotalSeconds) / 1000000).ToString("0.0"));
        }
    }
}
