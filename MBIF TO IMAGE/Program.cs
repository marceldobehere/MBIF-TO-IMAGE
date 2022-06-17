using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace MBIF_TO_IMAGE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("No File provided!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine();

            if (Directory.Exists(args[0]))
            {
                string newFolder = $"{args[0]}-deconv";
                Directory.CreateDirectory(newFolder);
                string[] files = Directory.GetFiles(args[0]);
                foreach (string file in files)
                    DeconvImage(file, newFolder);
            }
            else if (File.Exists(args[0]))
            {
                DeconvImage(args[0], ".");
            }
            else
            {
                Console.WriteLine("No valid File provided!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\n\nEnd.");
            Console.ReadLine();
        }

        static void DeconvImage(string path, string resPath)
        {
            byte[] bytes = File.ReadAllBytes(path);


            //    writer.Write(image.Width);          // 4 byte width
            //    writer.Write(image.Height);         // 4 byte height
            //    writer.Write(xOff);                 // 4 byte x offset
            //    writer.Write(yOff);                 // 4 byte y offset
            //    writer.Write(imageData.LongLength); // 8 byte image lenght
            //    writer.Write(imageData);            // image data

            int width = BitConverter.ToInt32(bytes, 0);
            int height = BitConverter.ToInt32(bytes, 4);
            int xOff = BitConverter.ToInt32(bytes, 8);
            int yOff = BitConverter.ToInt32(bytes, 12);
            long dataLen = BitConverter.ToInt64(bytes, 16);
            int offset = 4+4+4+4+8;

            Console.WriteLine($"Image:");
            Console.WriteLine($" - Size:   {width}x{height}");
            Console.WriteLine($" - Offset: ({xOff}, {yOff})");
            Console.WriteLine($" - Data Lenght: {dataLen}");
            Console.WriteLine();
            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);



            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Console.WriteLine($"Image: {image.Width}x{image.Height} = {image.Width * image.Height} ({image.Width * image.Height * 4} bytes)");
            byte[] imageData = new byte[image.Height * image.Width * 4];
            try
            {
                unsafe
                {
                    byte* ptr = (byte*)bitmapData.Scan0;
                    for (int i = 0; i < image.Height * image.Width * 4; i++)
                        ptr[i] = bytes[i+offset];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Reading Image failed!");
            }
            Console.WriteLine("Image was read.");
            Console.WriteLine();
            image.Save($"{resPath}/{Path.GetFileNameWithoutExtension(path)}.bmp");


            //Console.WriteLine("Writing Image File.");
            //using (BinaryWriter writer = new BinaryWriter(new FileStream($"{resPath}/{Path.GetFileNameWithoutExtension(path)}.mbif", FileMode.Create)))
            //{
            //    writer.Write(image.Width);          // 4 byte width
            //    writer.Write(image.Height);         // 4 byte height
            //    writer.Write(xOff);                 // 4 byte x offset
            //    writer.Write(yOff);                 // 4 byte y offset
            //    writer.Write(imageData.LongLength); // 8 byte image lenght
            //    writer.Write(imageData);            // image data
            //}
            //Console.WriteLine("Done.");
        }
    }
}
