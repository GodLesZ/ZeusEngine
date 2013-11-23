using System.IO;
using Microsoft.Xna.Framework;

namespace Zeus.Client.Library.Format.Ragnarok.Rsm {

    public class ModelMeshFace {

        public short[] Vertices {
            get;
            protected set;
        }

        public short[] TextureVertices {
            get;
            protected set;
        }

        public Vector3 Normal {
            get;
            protected set;
        }

        public short TextureIndex {
            get;
            protected set;
        }

        public int TwoSide {
            get;
            protected set;
        }

        public int SmoothGroup {
            get;
            protected set;
        }


        public ModelMeshFace(BinaryReader reader) {
            // Original: readWord()
            Vertices = new []{
                reader.ReadInt16(), 
                reader.ReadInt16(), 
                reader.ReadInt16()
            };
            TextureVertices = new[] {
                reader.ReadInt16(),
                reader.ReadInt16(),
                reader.ReadInt16()
            };
            TextureIndex = reader.ReadInt16();
            reader.ReadInt16(); // TODO?
            TwoSide = reader.ReadInt32();
            SmoothGroup = reader.ReadInt32();
        }

    }

}