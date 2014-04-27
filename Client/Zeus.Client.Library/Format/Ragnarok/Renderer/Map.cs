using GatFormat = Zeus.Client.Library.Format.Ragnarok.Gat.Format;
using GndFormat = Zeus.Client.Library.Format.Ragnarok.Gnd.Format;
using RswFormat = Zeus.Client.Library.Format.Ragnarok.Rsw.Format;

namespace Zeus.Client.Library.Format.Ragnarok.Renderer {

    public class Map : IRenderer {


        public MapFogSettings Fog {
            get;
            protected set;
        }

        public string CurrentMap {
            get;
            protected set;
        }


        public RswFormat World {
            get;
            protected set;
        }

        public GndFormat Ground {
            get;
            protected set;
        }

        public GatFormat Altitude {
            get;
            protected set;
        }


        public Map(string mapname) {

        }

    }

}