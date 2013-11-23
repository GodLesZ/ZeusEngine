using Microsoft.Xna.Framework;

namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public class LightData {
        protected Format _parentRsw;

        public int Longitude {
            get;
            protected set;
        }

        public int Latitude {
            get;
            protected set;
        }

        public Color DiffuseColor {
            get;
            protected set;
        }

        public Color AmbientColor {
            get;
            protected set;
        }

        public float Ignored {
            get;
            protected set;
        }


        public LightData(Format rsw) {
            _parentRsw = rsw;

            // Defaults
            Longitude = 45;
            Latitude = 45;
            DiffuseColor = new Color(1, 1, 1);
            AmbientColor = new Color(0.3f, 0.3f, 0.3f);

            if (_parentRsw.FileHeader.Version.IsCompatible(1, 5)) {
                Longitude = _parentRsw.Reader.ReadInt32();
                Latitude = _parentRsw.Reader.ReadInt32();
                DiffuseColor = new Color(_parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle());
                AmbientColor = new Color(_parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle());
            }
            
            if (_parentRsw.FileHeader.Version.IsCompatible(1, 7)) {
                Ignored = _parentRsw.Reader.ReadSingle();
            }
        }

    }

}