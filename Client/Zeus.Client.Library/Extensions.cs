using System.Drawing;
using System.IO;

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
                writer.Write((byte) (255 - c.A));
            } else {
                writer.Write(c.A);
            }
        }

    }

}