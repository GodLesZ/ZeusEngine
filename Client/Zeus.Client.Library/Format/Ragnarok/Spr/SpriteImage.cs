using System.Drawing;

namespace Zeus.Client.Library.Format.Ragnarok.Spr {

    public class SpriteImage {
        public byte[] Data;
        public ushort Height;

        public Bitmap Image;
        public ushort Width;

        public override string ToString() {
            return string.Format("{0}x{1}", Width, Height);
        }

    }

}