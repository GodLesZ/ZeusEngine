using Microsoft.Xna.Framework;

namespace Zeus.Client.Library.Format.Ragnarok.Rsw.Objects {

    public class Effect : MapObjectBase {

        public int EffectType {
            get;
            protected set;
        }

        public float EmitSpeed {
            get;
            protected set;
        }

        public float[] Params {
            get;
            protected set;
        }


        public Effect(Format rsw)
            : base(rsw, MapObjectType.Effect) {
            Name = _parentRsw.Reader.ReadStringIso(80);
            Position = new Vector3(
                _parentRsw.Reader.ReadSingle() / 5, 
                _parentRsw.Reader.ReadSingle() / 5, 
                _parentRsw.Reader.ReadSingle() / 5
            );
            EffectType = _parentRsw.Reader.ReadInt32();
            EmitSpeed = _parentRsw.Reader.ReadSingle();
            Params = new[] {
                _parentRsw.Reader.ReadSingle(),
                _parentRsw.Reader.ReadSingle(),
                _parentRsw.Reader.ReadSingle(),
                _parentRsw.Reader.ReadSingle()
            };
        }

    }

}