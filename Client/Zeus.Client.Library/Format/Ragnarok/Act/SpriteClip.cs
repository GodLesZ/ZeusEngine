using System.Drawing;
using System.IO;

namespace Zeus.Client.Library.Format.Ragnarok.Act {

    public class SpriteClip {

        //< offset in frame (center is 0)
        public int X {
            get;
            protected set;
        }

        //< offset in frame (center is 0)
        public int Y {
            get;
            protected set;
        }

        //< number of the image in the spr file (-1 for none)
        public int SpriteNumber {
            get;
            protected set;
        }

        //< mirror image along the vertical axis if non-zero
        public bool MirrorOnVertical {
            get;
            protected set;
        }

        //< (uchar r,g,b,a;) (default 0xFFFFFFFF, v2.0+)
        public Color Color {
            get;
            protected set;
        }

        //< scale of X axis (default 1.0, v2.0+ zoom, v2.4+ xZoom)
        public float ZoomX {
            get;
            protected set;
        }

        //< scale of Y axis (default 1.0, v2.0+ zoom, v2.4+ yZoom)
        public float ZoomY {
            get;
            protected set;
        }

        //< angle/rotation (degrees) (default 0, v2.0+)
        public int Angle {
            get;
            protected set;
        }

        //< 0=palette image,1=rgba image (default 0, v2.0+)
        public int SpriteType {
            get;
            protected set;
        }

        //< (default 0, v2.5+)
        public int Width {
            get;
            protected set;
        }

        //< (default 0, v2.5+) 
        public int Height {
            get;
            protected set;
        }


        public SpriteClip() {
            X = 0;
            Y = 0;
            SpriteNumber = -1;
            Color = Color.FromArgb(255, 255, 255, 255);
            ZoomX = 1.0f;
            ZoomY = 1.0f;
            Angle = 0;
            SpriteType = 0;
            Width = 0;
            Height = 0;
        }

        public SpriteClip(BinaryReader reader, FileFormatVersion version)
            : this() {

            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            SpriteNumber = reader.ReadInt32();
            MirrorOnVertical = reader.ReadInt32() > 0;

            if (version.Version >= 0x200) {
                // RGBA
                var c = reader.ReadBytes(4);
                Color = Color.FromArgb(c[3], c[0], c[1], c[2]);
                if (version.Version >= 0x204) {
                    ZoomX = reader.ReadInt32();
                    ZoomY = reader.ReadInt32();
                } else {
                    ZoomX = ZoomY = reader.ReadInt32();
                }

                Angle = reader.ReadInt32();
                SpriteType = reader.ReadInt32();
                if (version.Version >= 0x205) {
                    Width = reader.ReadInt32();
                    Height = reader.ReadInt32();
                }
            }

        }

    }

}