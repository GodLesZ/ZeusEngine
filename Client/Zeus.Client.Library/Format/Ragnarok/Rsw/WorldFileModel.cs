namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public class WorldFileModel : WorldFileObjectBase {

        public int AnimationType { get; protected set; }
        public float AnimationSpeed { get; protected set; }
        public int BlockType { get; protected set; }
        public string ModelName { get; protected set; }
        public string NodeName { get; protected set; }
        public float[] Position { get; protected set; }
        public float[] Rotation { get; protected set; }
        public float[] Scale { get; protected set; }


        public WorldFileModel(WorldFile rsw) {
            Type = 1;
            if (rsw.Header.Version.IsCompatible(1, 6)) {
                Name = new string(rsw.Reader.ReadChars(40));
                AnimationType = rsw.Reader.ReadInt32();
                AnimationSpeed = rsw.Reader.ReadSingle();
                BlockType = rsw.Reader.ReadInt32();
            }

            ModelName = new string(rsw.Reader.ReadChars(80));
            NodeName = new string(rsw.Reader.ReadChars(80));
            Position = new[] { rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle() };
            Rotation = new[] { rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle() };
            Scale = new[] { rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle(), rsw.Reader.ReadSingle() };
        }

    }

}