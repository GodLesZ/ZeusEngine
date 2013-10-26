namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public class WorldFileEffect : WorldFileObjectBase {

        public float[] Position { get; protected set; }
        public int EffectType { get; protected set; }
        public float EmitSpeed { get; protected set; }
        public float[] Param { get; protected set; }


        public WorldFileEffect(WorldFile rsw) {
            Type = 4;
            Name = new string(rsw.Reader.ReadChars(80));
            Position = new[] { rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle() };
            EffectType = rsw.Reader.ReadInt32();
            EmitSpeed = rsw.Reader.ReadSingle();
            Param = new[] { rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle() };
        }

    }

}