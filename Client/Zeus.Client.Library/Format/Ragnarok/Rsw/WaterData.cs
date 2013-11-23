namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public class WaterData {
        protected Format _parentRsw;

        public float Level {
            get;
            protected set;
        }
        public int Type {
            get;
            protected set;
        }
        public float WaveHeight {
            get;
            protected set;
        }
        public float WaveSpeed {
            get;
            protected set;
        }
        public float WavePitch {
            get;
            protected set;
        }
        public int AnimSpeed {
            get;
            protected set;
        }


        public WaterData(Format rsw) {
            _parentRsw = rsw;

            // Defaults
            Level = 0f;
            Type = 0;
            WaveHeight = 1.0f;
            WaveSpeed = 2.0f;
            WavePitch = 50.0f;
            AnimSpeed = 3;
            
            if (_parentRsw.FileHeader.Version.IsCompatible(1, 3)) {
                Level = _parentRsw.Reader.ReadSingle();
            }
            
            if (_parentRsw.FileHeader.Version.IsCompatible(1, 8)) {
                Type = _parentRsw.Reader.ReadInt32();
                WaveHeight = _parentRsw.Reader.ReadSingle();
                WaveSpeed = _parentRsw.Reader.ReadSingle();
                WavePitch = _parentRsw.Reader.ReadSingle();
            }

            if (_parentRsw.FileHeader.Version.IsCompatible(1, 9)) {
                AnimSpeed = _parentRsw.Reader.ReadInt32();
            }
        }
 

    }

}