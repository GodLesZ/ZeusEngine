namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public class GroundData {
        protected Format _parentRsw;

        public float Top {
            get;
            protected set;
        }

        public float Bottom {
            get;
            protected set;
        }

        public float Left {
            get;
            protected set;
        }

        public float Right {
            get;
            protected set;
        }


        public GroundData(Format rsw) {
            _parentRsw = rsw;

            // Defaults
            Top = -500;
            Bottom = 500;
            Left = -500;
            Right = 500;

            if (_parentRsw.FileHeader.Version.IsCompatible(1, 6)) {
                Top = _parentRsw.Reader.ReadInt32();
                Bottom = _parentRsw.Reader.ReadInt32();
                Left = _parentRsw.Reader.ReadInt32();
                Right = _parentRsw.Reader.ReadInt32();
            }
        }

    }

}