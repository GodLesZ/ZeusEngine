using Microsoft.Xna.Framework;

namespace Zeus.Client.Library.Format.Ragnarok.Rsw.Objects {

    public class Light : MapObjectBase {

        public Color Color {
            get;
            protected set;
        }

        public float Range {
            get;
            protected set;
        }


        public Light(Format rsw)
            : base(rsw, MapObjectType.Light) {
            Name = _parentRsw.Reader.ReadStringIso(80);
            Position = new Vector3(_parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle(), _parentRsw.Reader.ReadSingle());
            Color = new Color(_parentRsw.Reader.ReadInt32(), _parentRsw.Reader.ReadInt32(), _parentRsw.Reader.ReadInt32());
            Range = _parentRsw.Reader.ReadSingle();
        }

    }

}