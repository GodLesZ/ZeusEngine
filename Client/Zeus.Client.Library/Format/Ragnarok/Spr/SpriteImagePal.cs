namespace Zeus.Client.Library.Format.Ragnarok.Spr {

    public class SpriteImagePal : SpriteImage {

        public bool Decoded;
        public ushort Size;

        public override string ToString() {
            return string.Format("{0}x{1} ({2} bytes)", Width, Height, Size);
        }

    }

}