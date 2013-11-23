using System.IO;
using Microsoft.Xna.Framework;

namespace Zeus.Client.Library.Format.Ragnarok.Rsm {

    public class ModelAnimationFrame {

        public float Time {
            get;
            protected set;
        }

        public Quaternion Quaternion {
            get;
            protected set;
        }


        public ModelAnimationFrame(BinaryReader reader) {
            Time = reader.ReadSingle();
            Quaternion = new Quaternion(
                reader.ReadSingle(), 
                reader.ReadSingle(), 
                reader.ReadSingle(), 
                reader.ReadSingle()
                );
        }

    }

}