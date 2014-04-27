using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Zeus.Client.Library.Format.Ragnarok.Gnd {

    public class TextureInfo {
        protected readonly Format _parentGnd;

        public string Filename {
            get;
            protected set;
        }

        public string Name {
            get;
            protected set;
        }

        public Texture2D Texture {
            get;
            protected set;
        }


        public TextureInfo(Format gnd) {
            _parentGnd = gnd;

            Filename = _parentGnd.Reader.ReadStringIso(40);
            Name = _parentGnd.Reader.ReadStringIso(40);
        }


        public void Load(GraphicsDevice device, string basepath) {
            var filepath = string.Format("{0}/data/texture/{1}", basepath, Filename);
            Texture = ImageExtensions.FromFile(filepath, device);
        }

    }

}