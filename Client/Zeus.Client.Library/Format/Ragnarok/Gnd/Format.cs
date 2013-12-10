using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zeus.Client.Library.Format.Ragnarok.Gnd {

    public class Format : FileFormat {

        public override char[] ExpectedMagicBytes {
            get { return new[] { 'G', 'R', 'G', 'N' }; }
        }

        public int Width {
            get;
            protected set;
        }

        public int Height {
            get;
            protected set;
        }

        public float TileScale {
            get;
            protected set;
        }

        public int TextureCount {
            get;
            protected set;
        }

        public List<TextureInfo> Textures {
            get;
            protected set;
        }


        protected Format() {
        }

        public Format(string filepath)
            : base(filepath) {
        }

        public Format(byte[] data)
            : base(data) {
        }

        public Format(Stream stream)
            : base(stream) {
        }


        public void Load(GraphicsDevice device, string basepath) {
            Textures.ForEach(tex => tex.Load(device, basepath));
        }


        protected override bool ReadInternal() {
            Textures = new List<TextureInfo>();

            if (base.ReadInternal() == false) {
                return false;
            }

            if (FileHeader.Version.Version > 0) {
                Width = Reader.ReadInt32();
                Height = Reader.ReadInt32();
                TileScale = Reader.ReadSingle();
                TextureCount = Reader.ReadInt32();
                Reader.ReadInt32(); // unknown
            } else {
                // @TODO: test this
                // gndFile->seek(6, InFile::CUR);
                Reader.BaseStream.Seek(6, SeekOrigin.Current);
                Width = Reader.ReadInt32();
                Height = Reader.ReadInt32();
                TextureCount = Reader.ReadInt32();
            }

            for (var i = 0; i < TextureCount; i++) {
                var tex = new TextureInfo(this);
                Textures.Add(tex);
            }

            // @TODO: Finish

            return true;
        }

    }

}