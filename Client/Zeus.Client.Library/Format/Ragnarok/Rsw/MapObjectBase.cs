using Microsoft.Xna.Framework;

namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public abstract class MapObjectBase {
        protected Format _parentRsw;

        public MapObjectType Type {
            get;
            protected set;
        }

        public string Name {
            get;
            protected set;
        }

        public Vector3 Position {
            get;
            protected set;
        }


        protected MapObjectBase(Format rsw, MapObjectType type) {
            _parentRsw = rsw;
            Type = type;

            // Defaults
            Name = string.Empty;
            Position = Vector3.Zero;
        }
         
    }

}