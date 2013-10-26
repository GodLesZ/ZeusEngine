using System.IO;

namespace Zeus.Client.Library.Format {

    public class FileFormatVersion {

        public byte Major { get; protected set; }

        public byte Minor { get; protected set; }

        public ushort Version { get; protected set; }


        public FileFormatVersion(BinaryReader reader) {
            Major = reader.ReadByte();
            Minor = reader.ReadByte();
            reader.BaseStream.Seek(-2, SeekOrigin.Current);
            Version = reader.ReadUInt16();
        }


        public static implicit operator int(FileFormatVersion version) {
            return version.Version;
        }

        public bool IsCompatible(byte major, byte minor) {
            return (Major > major || (Major == major && Minor >= minor));
        }


        public override string ToString() {
            return Version.ToString("X2");
        }

    }

}