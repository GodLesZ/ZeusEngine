using System;
using System.Drawing;
using System.IO;

namespace Zeus.Client.Library.Format.Ragnarok.Act {

    public class ActionFile : FileFormatBase {
        public const string FormatExtension = ".act";

        public ActionList Actions { get; protected set; }

        public ActionIntervalList Intervals { get; protected set; }
        public byte MajorVersion { get; protected set; }

        public byte MinorVersion { get; protected set; }

        public ActionSoundList Sounds { get; protected set; }

        public ActionFile(string actionpath)
            : base(actionpath) {
        }

        public ActionFile(Stream stream)
            : base(stream) {
        }

        public ActionFile()
            : this("") {
        }


        public override bool Write(string filePath, bool overwrite) {
            base.Write(filePath, overwrite);

            Flush();

            return true;
        }

        public override string ToString() {
            return string.Format("0x{0}, {1}x actions, {2}x sounds", Version, Actions.Count, Sounds.Count);
        }

        protected override bool ReadInternal() {
            Actions = new ActionList();
            Sounds = new ActionSoundList();
            Intervals = new ActionIntervalList();

            Actions.Clear();
            Sounds.Clear();
            Intervals.Clear();

            Reader.ReadChars(2);
            MajorVersion = Reader.ReadByte();
            MinorVersion = Reader.ReadByte();
            Reader.BaseStream.Seek(-2, SeekOrigin.Current);
            Version = new FileFormatVersion(Reader);
            var versionString = "0x" + Version;

            if (Version > 0x205) {
                throw new Exception("Unsupported action format 0x" + versionString);
            }

            short animationCount = Reader.ReadInt16();
            Reader.BaseStream.Seek(10, SeekOrigin.Current);

            for (var a = 0; a < animationCount; a++) {
                var frameCount = Reader.ReadInt32();

                var ani = new Action();
                for (var f = 0; f < frameCount; f++) {
                    var frame = new ActionFrame {
                        Index = f,
                        EventIndex = 0
                    };

                    Reader.BaseStream.Seek(16, SeekOrigin.Current); // range1 RECT{left,top,right,bottom}
                    Reader.BaseStream.Seek(16, SeekOrigin.Current); // range1 RECT{left,top,right,bottom}

                    var imageCount = Reader.ReadInt32();

                    for (var i = 0; i < imageCount; i++) {
                        var img = new ActionFrameImage {
                            OffsetX = Reader.ReadInt32(),
                            OffsetY = Reader.ReadInt32(),
                            ImageIndex = Reader.ReadInt32(),
                            Direction = Reader.ReadInt32(),
                            Color = Color.White,
                            Rotation = 0,
                            ScaleX = 1,
                            ScaleY = 1,
                            Width = 0,
                            Height = 0
                        };

                        // Version >= 2
                        if (Version >= 0x200) {
                            img.Color = Reader.ReadSpriteColor(false);
                            // Version <= 2.3
                            if (Version <= 0x203) {
                                img.ScaleX = img.ScaleY = Reader.ReadSingle();
                            } else {
                                // Version > 2.3
                                img.ScaleX = Reader.ReadSingle();
                                img.ScaleY = Reader.ReadSingle();
                            }
                            
                            img.Rotation = Reader.ReadInt32();
                            img.RgbImage = (Reader.ReadInt32() == 1);

                            if (Version >= 0x205) {
                                img.Width = Reader.ReadInt32();
                                img.Height = Reader.ReadInt32();
                            }
                        }

                        frame.Add(img);
                    }

                    if (Version >= 0x200) {
                        frame.EventIndex = Reader.ReadInt32();

                        if (Version >= 0x203) {
                            var pointerCount = Reader.ReadInt32();
                            for (var i = 0; i < pointerCount; i++) {
                                // Skip 4 bytes "reserved"
                                Reader.BaseStream.Seek(4, SeekOrigin.Current);
                                frame.AttachmentPointers.Add(new ActionAttachmentPoint {
                                    X = Reader.ReadInt32(),
                                    Y = Reader.ReadInt32(),
                                    Attribute = Reader.ReadUInt32()
                                });
                            }
                        }
                    }

                    ani.Add(frame);
                }

                Actions.Add(ani);
            }

            // Sounds
            if (Version >= 0x0201) {
                var soundNum = Reader.ReadInt32();
                var soundNames = new string[soundNum];
                for (var i = 0; i < soundNames.Length; i++) {
                    soundNames[i] = new String(Reader.ReadChars(40));
                    soundNames[i] = soundNames[i].Replace("\0", "");
                }

                Sounds.AddRange(soundNames);
            }

            // Interval
            if (Version >= 0x0202) {
                var intervals = new float[animationCount];
                for (var i = 0; i < intervals.Length; i++) {
                    intervals[i] = Reader.ReadSingle()*25f;
                }

                Intervals.AddRange(intervals);
            } else {
                for (var i = 0; i < animationCount; i++) {
                    Intervals.Add(100f);
                }
            }

            Flush();
            return true;
        }
    }

}