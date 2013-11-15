using System;
using System.Globalization;
using System.IO;
using System.Text;
using Zeus.Client.Library.Format.Compression;

namespace Zeus.Client.Library.Format.Ragnarok.Grf {

	public class FileItem : ICloneable, IDisposable {
		protected string _nameHash = "";

		/// <summary>
		/// Gets or sets the path in the grf
		/// Starts with data/
		/// </summary>
		public string Filepath {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the compressed length of <see cref="FileData"/>
		/// </summary>
		public uint LengthCompressed {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the compressed length including DES
		/// </summary>
		public uint LengthCompressedAlign {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the uncompressed length of <see cref="FileData"/>
		/// </summary>
		public uint LengthUnCompressed {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the item flag
		/// </summary>
		public byte Flags {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="FileData"/> offset in the binary table
		/// </summary>
		public uint DataOffset {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the file table offste
		/// </summary>
		public uint TableOffset {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the file data
		/// </summary>
		public byte[] FileData {
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the cycle count, used in DES encryption
		/// <para>This is calculated</para>
		/// </summary>
		public int Cycle {
			get;
			internal set;
		}

		/// <summary>
		/// Gets or sets the index in the grf item list
		/// <para>This is calculated</para>
		/// </summary>
		public int Index {
			get;
			internal set;
		}

		public FileItemState State {
			get;
			internal set;
		}

		/// <summary>
		/// Gets or sets the real file system path for new added items
		/// </summary>
		public string NewFilepath {
			get;
			internal set;
		}

		/// <summary>
		/// Gets the item <see cref="Filepath"/> hash, used for faster accessing items from the list
		/// </summary>
		public string NameHash {
			get {
				if (_nameHash == "")
					_nameHash = BuildNameHash(Filepath);
				return _nameHash;
			}
		}

		public bool IsAdded {
			get { return (State & FileItemState.Added) != 0; }
		}

		public bool IsUpdated {
			get { return (State & FileItemState.Updated) != 0; }
		}

		public bool IsDeleted {
			get { return (State & FileItemState.Deleted) != 0; }
		}

		public bool IsFile {
			get { return Flags == 1 || Flags == 3 || Flags == 5; }
		}


		public FileItem() {
			Cycle = -1;
			State = FileItemState.None;
		}

		~FileItem() {
			Dispose();
		}


		public void Dispose() {
			Flush(true);
		}

		public void Flush() {
			Flush(false);
		}

		public void Flush(bool fromDispose) {
			// Don't flush data from updated files
			// - only on dispose call
			if (State == FileItemState.Updated && fromDispose == false) {
				return;
			}
			FileData = null;
		}


		public object Clone() {
			var item = new FileItem {
				Index = Index,
				Filepath = Filepath,
				LengthCompressed = LengthCompressed,
				LengthCompressedAlign = LengthCompressedAlign,
				LengthUnCompressed = LengthUnCompressed,
				Flags = Flags,
				DataOffset = DataOffset,
				Cycle = Cycle,
				State = State,
				NewFilepath = NewFilepath
			};

			if (FileData != null) {
				item.FileData = FileData.Clone() as byte[];
			}

			return item;
		}


		public void LoadFromFile(Format grf, string filepath) {
			var info = new FileInfo(filepath);
			Index = grf.Files.Count;
			Filepath = Tools.GetGrfPath(filepath);
			Flags = 3;
			Cycle = -1;
			LengthUnCompressed = (uint)info.Length;
			State = FileItemState.Added;
			NewFilepath = filepath;
			// TODO: load new items into memory?
		}





		/// <summary>
		/// Writes the binary data and file properties to te streams.
		/// </summary>
		/// <param name="grf"></param>
		/// <param name="writer"></param>
		/// <param name="tableWriter"></param>
		internal void WriteToStream(Format grf, BinaryWriter writer, BinaryWriter tableWriter) {
			// Skip deleted files
			if (IsDeleted) {
				return;
			}

			// Write binary data
			WriteToBinaryTable(grf, writer);

			// Write file properties
			WriteToFileTable(tableWriter);

			// Its saved, so remove updated and new flags
			State &= ~(FileItemState.Added | FileItemState.Updated);
		}

		/// <summary>
		/// Writes the binary data to the stream.
		/// </summary>
		/// <param name="grf"></param>
		/// <param name="writer"></param>
		internal void WriteToBinaryTable(Format grf, BinaryWriter writer) {
			// Skip deleted files
			if (IsDeleted) {
				return;
			}

			byte[] buf;

			// Update new offset
			DataOffset = (uint)writer.BaseStream.Position;

			// Either new or changed?
			if (IsUpdated == false && IsAdded == false) {
				// Auto-convert to 0x200 compression (deflate)
				if (grf.Version != 0x200) {
					// #1: Decompress buf and update length
					buf = grf.GetFileData(NameHash, true);
					LengthUnCompressed = (uint)buf.Length;
					// #2: Compress and update length
					buf = Deflate.Compress(buf, true);
					LengthCompressed = (uint)buf.Length;
					LengthCompressedAlign = (uint)buf.Length;
				} else {
					// Get compressed data
					buf = grf.GetFileData(NameHash, false);
				}
			} else {
				// Added or updated files, load data from origin filepath
				if (File.Exists(NewFilepath) == false) {
					throw new Exception("WriteItems(): File of new or updated item not found: " + NewFilepath);
				}

				buf = File.ReadAllBytes(NewFilepath);
				LengthUnCompressed = (uint)buf.Length;
				buf = Deflate.Compress(buf, true);
				LengthCompressed = LengthCompressedAlign = (uint)buf.Length;
			}

			try {
				// Check if the buf is compressed
				if (buf.Length != LengthCompressed && buf.Length != LengthCompressedAlign) {
					// The buf has to be compressed, so decompress it
					byte[] bufUncompressed = Deflate.Decompress(buf);
					// Update length, if decompression seems to be correct
					if (bufUncompressed.Length == 0 || bufUncompressed.Length != LengthUnCompressed) {
						// Narf, corrupt file or something like that
						// Just write it..
						//throw new Exception("WriteItems(): Item " + Filepath + ", DataLen missmatch");
					} else {
						// Decompression was succesfull, so update size
						LengthCompressed = (uint)Deflate.GetCompressedLength(bufUncompressed);
					}
				}

				// Seems like a valid buf, write it
				writer.Write(buf);
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine(e);
			}
		}

		/// <summary>
		/// Writes all file properties to the stream.
		/// </summary>
		/// <param name="tableWriter"></param>
		internal void WriteToFileTable(BinaryWriter tableWriter) {
			// Skip deleted files
			if (IsDeleted) {
				return;
			}

			byte[] filepathBuf = Encoding.Default.GetBytes(Filepath);
			tableWriter.Write(filepathBuf, 0, filepathBuf.Length);
			tableWriter.Write((byte)0); // \0 string terminator
			tableWriter.Write((int)LengthCompressed);
			tableWriter.Write((int)LengthCompressedAlign);
			tableWriter.Write((int)LengthUnCompressed);
			tableWriter.Write(Flags);
			tableWriter.Write((int)(DataOffset - Format.GrfHeaderLen));
		}


		public override string ToString() {
			return string.Format("Name={0},Length={1}({2}),Flags={3},Off={4}", Filepath, LengthCompressed, LengthUnCompressed, Flags, DataOffset);
		}


		public static string BuildNameHash(string filepath) {
			return filepath.GetHashCode().ToString(CultureInfo.InvariantCulture);
			//return Encoding.Default.GetBytes(filepath).Aggregate(string.Empty, (current, b) => current + b.ToString());
		}

	}

}
