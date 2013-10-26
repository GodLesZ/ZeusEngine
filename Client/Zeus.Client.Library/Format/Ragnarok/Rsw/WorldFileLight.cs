namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public class WorldFileLight : WorldFileObjectBase {

        public float[] Position { get; protected set; }
        public int Red { get; protected set; }
        public int Green { get; protected set; }
        public int Blue { get; protected set; }
        public float Range { get; protected set; }


        public WorldFileLight(WorldFile rsw) {
            Type = 2;
            Name = new string(rsw.Reader.ReadChars(80));
            Position = new[] {rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle()};
            Red = rsw.Reader.ReadInt32();
            Green = rsw.Reader.ReadInt32();
            Blue = rsw.Reader.ReadInt32();
            Range = rsw.Reader.ReadSingle();
        }

    }

}