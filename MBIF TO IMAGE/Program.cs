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
            Console.WriteLine("If the image format is old then enter \"old\"");
            bool old = Console.ReadLine().Equals("old");

            if (Directory.Exists(args[0]))
            {
                string newFolder = $"{args[0]}-deconv";
                Directory.CreateDirectory(newFolder);
                string[] files = Directory.GetFiles(args[0]);
                foreach (string file in files)
                    DeconvImage(file, newFolder, old);
            }
            else if (File.Exists(args[0]))
            {
                DeconvImage(args[0], ".", old);
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

        static void DeconvImage(string path, string resPath, bool old)
        {
            byte[] bytes = File.ReadAllBytes(path);


            //    writer.Write(image.Width);          // 4 byte width
            //    writer.Write(image.Height);         // 4 byte height
            //    writer.Write(xOff);                 // 4 byte x offset (if not old)
            //    writer.Write(yOff);                 // 4 byte y offset (if not old)
            //    writer.Write(imageData.LongLength); // 8 byte image lenght
            //    writer.Write(imageData);            // image data

            int offset = 0;
            int width = BitConverter.ToInt32(bytes, offset);
            offset += 4;
            int height = BitConverter.ToInt32(bytes, offset);
            offset += 4;
            int xOff = 0, yOff = 0;
            long dataLen = 0;
            if (!old)
            {
                xOff = BitConverter.ToInt32(bytes, 8);
                offset += 4;
                yOff = BitConverter.ToInt32(bytes, 12);
                offset += 4;
                dataLen = BitConverter.ToInt64(bytes, offset);
                offset += 8;
            }
            else
            {
                dataLen = BitConverter.ToInt32(bytes, offset);
                offset += 4;
            }
            

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
