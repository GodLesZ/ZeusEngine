using Microsoft.Xna.Framework;

namespace Zeus.Client.Library.Format.Ragnarok.Rsw.Objects {

    public class Sound : MapObjectBase {

        public string WaveName {
            get;
            protected set;
        }

        public float Volume {
            get;
            protected set;
        }

        public int Width {
            get;
            protected set;
        }

        public int Height {
            get;
            protected set;
        }

        public float Range {
            get;
            protected set;
        }

        public float Cycle {
            get;
            protected set;
        }


        public Sound(Format rsw)
            : base(rsw, MapObjectType.Sound) {
            // Defaults
            Cycle = 4.0f;

            Name = _parentRsw.Reader.ReadStringIso(80);
            WaveName = _parentRsw.Reader.ReadStringIso(80);
            Position = new Vector3(_parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle());
            Volume = _parentRsw.Reader.ReadSingle();
            Width = _parentRsw.Reader.ReadInt32();
            Height = _parentRsw.Reader.ReadInt32();
            Range = _parentRsw.Reader.ReadSingle();
            if (_parentRsw.FileHeader.Version.IsCompatible(2)) {
                Cycle = _parentRsw.Reader.ReadSingle();
            }
        }

    }

}