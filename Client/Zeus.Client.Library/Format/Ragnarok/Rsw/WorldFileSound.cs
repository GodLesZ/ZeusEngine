namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public class WorldFileSound : WorldFileObjectBase {
        
        public string WaveName { get; protected set; }
        public float[] Position { get; protected set; }
        public float Volume { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public float Range { get; protected set; }
        public float Cycle { get; protected set; }


        public WorldFileSound(WorldFile rsw) {
            Type = 3;
            Name = new string(rsw.Reader.ReadChars(80));
            WaveName = new string(rsw.Reader.ReadChars(80));
            Position = new[] {rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle()};
            Volume = rsw.Reader.ReadSingle();
            Width = rsw.Reader.ReadInt32();
            Height = rsw.Reader.ReadInt32();
            Range = rsw.Reader.ReadSingle();
            Cycle = 4.0f;
            if (rsw.Header.Version.IsCompatible(2, 0)) {
                Cycle = rsw.Reader.ReadSingle();
            }
        }

    }

}