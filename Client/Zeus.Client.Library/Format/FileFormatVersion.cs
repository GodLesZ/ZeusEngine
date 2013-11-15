using System.IO;

namespace Zeus.Client.Library.Format {

    /// <summary>
    ///     Represents a file version based on a major and minor byte number.
    /// </summary>
    public class FileFormatVersion {

        /// <summary>
        ///     Gets or sets the major version.
        /// </summary>
        public byte Major {
            get;
            protected set;
        }

        /// <summary>
        ///     Gets or sets the minor version.
        /// </summary>
        public byte Minor {
            get;
            protected set;
        }

        /// <summary>
        ///     Gets or sets the hex-representation as the combination of major and minor version.
        /// </summary>
        public ushort Version {
            get;
            protected set;
        }


        public FileFormatVersion(BinaryReader reader) {
            Major = reader.ReadByte();
            Minor = reader.ReadByte();
            reader.BaseStream.Seek(-2, SeekOrigin.Current);
            Version = reader.ReadUInt16();
        }


        /// <summary>
        ///     Returns the hex-presentation as the combination of major and minor version.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static implicit operator int(FileFormatVersion version) {
            return version.Version;
        }

        /// <summary>
        ///     Returns true, if the given major and minor version are equal or below to this version instance.
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <returns></returns>
        public bool IsCompatible(byte major, byte minor = 0) {
            return (Major > major || (Major == major && Minor >= minor));
        }


        /// <summary>
        ///     Return this version as a hex representation in a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Version.ToString("X2");
        }

    }

}