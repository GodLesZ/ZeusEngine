using System.IO;

namespace Zeus.Client.Library.Format.Ragnarok.Act {

    public class AttachPoint {

        public int X {
            get;
            protected set;
        }

        public int Y {
            get;
            protected set;
        }

        public int Attribute {
            get;
            protected set;
        }


        public AttachPoint(BinaryReader reader, FileFormatVersion version) {
            var ignored = reader.ReadInt32();

            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            Attribute = reader.ReadInt32();
        }

    }

}