using System.Drawing;
using System.IO;

namespace Zeus.Client.Library.Format.Ragnarok.Gat {

    public sealed class Format : FileFormat {
        public const string FormatExtension = ".gat";

        public override char[] ExpectedMagicBytes {
            get { return new[] { 'G', 'R', 'A', 'T' }; }
        }

        public GroundCell[] Cells { get; private set; }
        public int Height { get; private set; }

        public char[] MagicHead { get; set; }
        public int Width { get; private set; }

        public Format(byte[] data) {
            using (var ms = new MemoryStream(data)) {
                Read(ms);
            }
        }


        public Bitmap DrawImage(Color[] colors) {
            if (Width < 1 || Height < 1) {
                return null;
            }

            var image = new Bitmap(Width, Height);
            var fastBmp = new FastBitmap(image);


            fastBmp.LockImage();
            for (int y = 0, i = 0; y < Height; y++) {
                for (var x = 0; x < Width; x++) {
                    fastBmp.SetPixel(x, y, colors[(int)Cells[i++].Type]);
                }
            }
            fastBmp.UnlockImage();

            return image;
        }

        protected override bool ReadInternal() {
            if (base.ReadInternal() == false) {
                return false;
            }
            
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
            Cells = new GroundCell[Width * Height];

            for (var i = 0; i < Cells.Length; i++) {
                Cells[i] = new GroundCell(
                    Reader.ReadSingle(),
                    Reader.ReadSingle(),
                    Reader.ReadSingle(),
                    Reader.ReadSingle(),
                    Reader.ReadByte()
                );
                Reader.BaseStream.Position += 3; // 3x unknown Char
            }

            return true;
        }

    }

}