using System;
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

        public float Opacity {
            get;
            protected set;
        }

        public Vector3 Direction {
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
            Opacity = 1;
            Direction = Vector3.Zero;

            if (_parentRsw.FileHeader.Version.IsCompatible(1, 5)) {
                Longitude = _parentRsw.Reader.ReadInt32();
                Latitude = _parentRsw.Reader.ReadInt32();
                DiffuseColor = new Color(_parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle());
                AmbientColor = new Color(_parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle());
            }

            if (_parentRsw.FileHeader.Version.IsCompatible(1, 7)) {
                Opacity = _parentRsw.Reader.ReadSingle();
            }


            // Calculate light direction
            var longitude = Longitude * Math.PI / 180;
            var latitude = Latitude * Math.PI / 180;

            Direction = new Vector3(
                (float)(-Math.Cos(longitude) * Math.Sin(latitude)),
                (float)-Math.Cos(latitude),
                (float)(-Math.Sin(longitude) * Math.Sin(latitude))
            );
        }

    }

}