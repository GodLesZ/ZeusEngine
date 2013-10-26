using System.Diagnostics;

namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public class WorldHeader {

        public string Magic { get; set; }

        public FileFormatVersion Version { get; set; }

        public string IniFilename { get; set; }

        public string GndFilename { get; set; }

        public string GatFilename { get; set; }

        public string SrcFilename { get; set; }

        public float WaterLevel { get; set; }

        public int WaterType { get; set; }

        public float WaveHeight { get; set; }

        public float WaveSpeed { get; set; }

        public int WaterAnimationSpeed { get; set; }

        public float WavePitch { get; set; }

        public int LightLongitude { get; set; }

        public int LightLatitude { get; set; }

        public int GroundTop { get; set; }

        public int GroundBottom { get; set; }

        public int GroundLeft { get; set; }

        public int GroundRight { get; set; }

        public float Unknown1 { get; set; }

        public float[] DiffuseCol { get; set; }

        public float[] AmbientCol { get; set; }


        public WorldHeader() {
            WaterLevel = 10.0f;
            WaterType = 0;
            WaveHeight = 1.0f;
            WaveSpeed = 2.0f;
            WaterAnimationSpeed = 3;
            WavePitch = 50.0f;
            LightLongitude = 45;
            LightLatitude = 45;
            DiffuseCol = new[] { 1.0f, 1.0f, 1.0f };
            AmbientCol = new[] { 0.3f, 0.3f, 0.3f };
        }

    }

}