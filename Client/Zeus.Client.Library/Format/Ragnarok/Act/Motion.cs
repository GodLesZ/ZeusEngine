using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Zeus.Client.Library.Format.Ragnarok.Act {

    public class Motion {

        // left,top,right,bottom
        public int[] Range1 {
            get;
            protected set;
        }

        // left,top,right,bottom
        public int[] Range2 {
            get;
            protected set;
        }

        // -1 for none (default -1, v2.0+)
        public int EventId {
            get;
            protected set;
        }

        public List<SpriteClip> SpriteClips {
            get;
            protected set;
        }

        public List<AttachPoint> AttachPoints {
            get;
            protected set;
        }


        public Motion() {
            SpriteClips = new List<SpriteClip>();
            AttachPoints = new List<AttachPoint>();
            EventId = -1;
        }

        public Motion(BinaryReader reader, FileFormatVersion version)
            : this() {
            Range1 = new[] {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32()
            };
            Range2 = new[] {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32()
            };

            var spriteClipCount = reader.ReadInt32();
			for (var i = 0; i < spriteClipCount; i++) {
				SpriteClips.Add(new SpriteClip(reader, version));
			}

            // Events
            if (version.Version >= 0x200) {
			    EventId = reader.ReadInt32();
                // No array of events in this version
                if (version.Version == 0x200) {
                    EventId = -1;
                }
            }

			// Attach points
			if (version.Version >= 0x203) {
			    var attachPointCount = reader.ReadInt32();
			    for (var i = 0; i < attachPointCount; i++) {
			        AttachPoints.Add(new AttachPoint(reader, version));
			    }
			}


        }

    }

}