
using Microsoft.Xna.Framework;

namespace Zeus.Client.Library.Format.Ragnarok.Renderer {

    public class MapFogSettings {

        public bool Use {
            get;
            protected set;
        }

        public bool Exist {
            get;
            protected set;
        }

        public float Far {
            get;
            protected set;
        }

        public float Near {
            get;
            protected set;
        }

        public float Factor {
            get;
            protected set;
        }

        public Color Color {
            get;
            protected set;
        }


        public MapFogSettings() {
            Use = true;
            Exist = true;
            Far = 30;
            Near = 180;
            Factor = 1f;
            Color = Color.White;
        }

    }

}