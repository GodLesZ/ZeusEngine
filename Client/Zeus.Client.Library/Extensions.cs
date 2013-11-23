using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using OpenTK;

namespace Zeus.Client.Library {

    public static class StreamExtensions {

        public static Color ReadSpriteColor(this BinaryReader bin) {
            return bin.ReadSpriteColor(true);
        }

        public static Color ReadSpriteColor(this BinaryReader bin, bool revertAlpha) {
            byte r = bin.ReadByte();
            byte g = bin.ReadByte();
            byte b = bin.ReadByte();
            byte a = bin.ReadByte();
            // Sprites using 255 for alpha, actions not..
            if (revertAlpha) {
                return Color.FromArgb(255 - a, r, g, b);
            }

            return Color.FromArgb(a, r, g, b);
        }

        public static void WriteSpriteColor(this BinaryWriter writer, Color c) {
            writer.WriteSpriteColor(c, true);
        }

        public static void WriteSpriteColor(this BinaryWriter writer, Color c, bool revertAlpha) {
            writer.Write(c.R);
            writer.Write(c.G);
            writer.Write(c.B);
            if (revertAlpha) {
                writer.Write((byte)(255 - c.A));
            } else {
                writer.Write(c.A);
            }
        }




        public static string ReadStringWithEncoding(this BinaryReader reader, int len, Encoding enc) {
            var buf = reader.ReadBytes(len);
            var i = 0;
            for (; i < buf.Length; i++) {
                if (buf[i] == 0) {
                    break;
                }
            }

            if (i < buf.Length - 1) {
                buf = buf.Take(i).ToArray();
            }

            return enc.GetString(buf).Trim();
        }

        public static string ReadStringIso(this BinaryReader reader, int len) {
            return reader.ReadStringWithEncoding(len, Encoding.GetEncoding("ISO-8859-1"));
        }

    }

    public static class Matrix4Extensions {
        
        public static Matrix4 Mult(this Matrix4 m, Matrix4 add) {
            var result = new Matrix4 {
                M11 = m.M11 * add.M11 + m.M21 * add.M12 + m.M31 * add.M13 + m.M41 * add.M14,
                M21 = m.M11 * add.M21 + m.M21 * add.M22 + m.M31 * add.M23 + m.M41 * add.M24,
                M31 = m.M11 * add.M31 + m.M21 * add.M32 + m.M31 * add.M33 + m.M41 * add.M34,
                M41 = m.M11 * add.M41 + m.M21 * add.M42 + m.M31 * add.M43 + m.M41 * add.M44,
                M12 = m.M12 * add.M11 + m.M22 * add.M12 + m.M32 * add.M13 + m.M42 * add.M14,
                M22 = m.M12 * add.M21 + m.M22 * add.M22 + m.M32 * add.M23 + m.M42 * add.M24,
                M32 = m.M12 * add.M31 + m.M22 * add.M32 + m.M32 * add.M33 + m.M42 * add.M34,
                M42 = m.M12 * add.M41 + m.M22 * add.M42 + m.M32 * add.M43 + m.M42 * add.M44,
                M13 = m.M13 * add.M11 + m.M23 * add.M12 + m.M33 * add.M13 + m.M43 * add.M14,
                M23 = m.M13 * add.M21 + m.M23 * add.M22 + m.M33 * add.M23 + m.M43 * add.M24,
                M33 = m.M13 * add.M31 + m.M23 * add.M32 + m.M33 * add.M33 + m.M43 * add.M34,
                M43 = m.M13 * add.M41 + m.M23 * add.M42 + m.M33 * add.M43 + m.M43 * add.M44,
                M14 = m.M14 * add.M11 + m.M24 * add.M12 + m.M34 * add.M13 + m.M44 * add.M14,
                M24 = m.M14 * add.M21 + m.M24 * add.M22 + m.M34 * add.M23 + m.M44 * add.M24,
                M34 = m.M14 * add.M31 + m.M24 * add.M32 + m.M34 * add.M33 + m.M44 * add.M34,
                M44 = m.M14 * add.M41 + m.M24 * add.M42 + m.M34 * add.M43 + m.M44 * add.M44
            };
            return result;
        }

    }

    public static class ImageExtensions {


        private static Texture2D ImageToTexture2D(Bitmap image, GraphicsDevice g) {
            Texture2D texture = null;
            try {
                using (var ms = new MemoryStream()) {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, 0);
                    texture = Texture2D.FromStream(g, ms);
                }
            } catch { }

            return texture;
        }

        public static Texture2D ToTexture2D(this Bitmap image, GraphicsDevice g) {
            if (image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb) {
                return ImageToTexture2D(image, g);
            }

            // Convert bitmap to 32bit ARGB
            var bitmap = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap)) {
                graphics.DrawImage(image, 0, 0, image.Width, image.Height);
            }
            return ImageToTexture2D(bitmap, g);
        }


}

}