using System;
using System.Collections.Generic;

namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public class WorldFile : FileFormatBase {

        public WorldHeader Header { get; protected set; }
        public List<WorldFileObjectBase> Objects { get; protected set; }


        protected override bool ReadInternal() {
            Objects = new List<WorldFileObjectBase>();
            Header = new WorldHeader {
                Magic = new string(Reader.ReadChars(4))
            };
            if (Header.Magic != "GRSW") {
                return false;
            }
            Header.Version = new FileFormatVersion(Reader);

            Header.IniFilename = new string(Reader.ReadChars(40));
            Header.GndFilename = new string(Reader.ReadChars(40));
            if (Header.Version.IsCompatible(1, 4)) {
                Header.GatFilename = new string(Reader.ReadChars(40));
            }

            Header.SrcFilename = new string(Reader.ReadChars(40));
            if (Header.Version.IsCompatible(1, 3)) {
                Header.WaterLevel = Reader.ReadSingle();
            }

            if (Header.Version.IsCompatible(1, 8)) {
                Header.WaterType = Reader.ReadInt32();
                Header.WaveHeight = Reader.ReadSingle();
                Header.WaveSpeed = Reader.ReadSingle();
                Header.WavePitch = Reader.ReadSingle();
            }

            if (Header.Version.IsCompatible(1, 9)) {
                Header.WaterAnimationSpeed = Reader.ReadInt32();
            }

            if (Header.Version.IsCompatible(1, 5)) {
                Header.LightLongitude = Reader.ReadInt32();
                Header.LightLatitude = Reader.ReadInt32();
                Header.DiffuseCol = new[] {
                    Reader.ReadSingle(),
                    Reader.ReadSingle(),
                    Reader.ReadSingle()
                };
                Header.AmbientCol = new[] {
                    Reader.ReadSingle(),
                    Reader.ReadSingle(),
                    Reader.ReadSingle()
                };
            }

            if (Header.Version.IsCompatible(1, 7)) {
                Header.Unknown1 = Reader.ReadSingle();
            }

            if (Header.Version.IsCompatible(1, 6)) {
                Header.GroundTop = Reader.ReadInt32();
                Header.GroundBottom = Reader.ReadInt32();
                Header.GroundLeft = Reader.ReadInt32();
                Header.GroundRight = Reader.ReadInt32();
            }

            var objectCount = Reader.ReadInt32();
            for (var i = 0; i < objectCount; i++) {
                var objectType = Reader.ReadInt32();
                switch (objectType) {
                    case 1:
                        Objects.Add(new WorldFileModel(this));
                        break;
                    case 2:
                        Objects.Add(new WorldFileLight(this));
                        break;
                    case 3:
                        Objects.Add(new WorldFileSound(this));
                        break;
                    case 4:
                        Objects.Add(new WorldFileEffect(this));
                        break;
                    default:
                        throw new Exception("Unknown rsw object of type #" + objectType);
                }
            }

            if (Header.Version.IsCompatible(2, 1)) {
                //{ * 1365 (4^0 + 4^1 + 4^2 + 4^3 + 4^4 + 4^5, quadtree with 6 levels, depth-first ordering)
                //	[ QuadTreeNode ]
                //	Field			Size		Comment
                //	-----			----		-------
                //	max				12			(vector3d)
                //	min				12			(vector3d)
                //	halfSize		12			(vector3d)
                //	center			12			(vector3d)
                //}
            }

            return true;
        }

    }

}