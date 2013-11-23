using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Model = Zeus.Client.Library.Format.Ragnarok.Rsw.Objects.Model;

namespace Zeus.Client.Library.Format.Ragnarok.Rsw {

    public class Format : FileFormat {

        public override char[] ExpectedMagicBytes {
            get { return new[] { 'G', 'R', 'S', 'W' }; }
        }

        public string IniFilename {
            get;
            protected set;
        }

        public string GndFilename {
            get;
            protected set;
        }

        public string GatFilename {
            get;
            protected set;
        }

        public string ScrFilename {
            get;
            protected set;
        }

        public WaterData WaterData {
            get;
            protected set;
        }

        public LightData LightData {
            get;
            protected set;
        }

        public int GroundTop {
            get;
            protected set;
        }
        public int GroundBottom {
            get;
            protected set;
        }
        public int GroundLeft {
            get;
            protected set;
        }
        public int GroundRight {
            get;
            protected set;
        }


        public List<MapObjectBase> Objects {
            get;
            protected set;
        }


        protected Format() {
        }

        public Format(string filepath)
            : base(filepath) {
        }

        public Format(byte[] data)
            : base(data) {
        }

        public Format(Stream stream)
            : base(stream) {
        }


        public void Load(GraphicsDevice device, string dataPath) {
            foreach (var model in Objects.Where(obj => obj.Type == MapObjectType.Model).OfType<Model>()) {
                model.Load(device, dataPath);
            }
        }

        public void DrawAll(GraphicsDevice device, BasicEffect effect, Matrix view, Matrix projection, Matrix world) {
            foreach (var model in Objects.Where(obj => obj.Type == MapObjectType.Model).OfType<Model>()) {
                model.Draw(device, effect, view, projection, world);
            }
        }


        protected override bool ReadInternal() {
            GroundTop = -500;
            GroundBottom = 500;
            GroundLeft = -500;
            GroundRight = 500;
            Objects = new List<MapObjectBase>();

            if (base.ReadInternal() == false) {
                return false;
            }

            // @TODO: Test this, take-over from open-ro
            if (FileHeader.Version.Major == 1 && FileHeader.Version.Minor >= 2 && FileHeader.Version.Minor <= 9) {
                // supported [1.2 1.9]
            } else if (FileHeader.Version.Major == 2 && FileHeader.Version.Minor <= 2) {
                // supported [2.0 2.1]
            } else {
                Debug.WriteLine("[RSW] I don't know how to properly read rsw version {0}, but i'm gonna try...", FileHeader.Version);
            }

            IniFilename = Reader.ReadStringIso(40);
            GndFilename = Reader.ReadStringIso(40);
            // Gat filename sice 1.4
            if (FileHeader.Version.IsCompatible(1, 4)) {
                GatFilename = Reader.ReadStringIso(40);
            } else {
                GatFilename = string.Empty;
            }

            ScrFilename = Reader.ReadStringIso(40);

            WaterData = new WaterData(this);
            LightData = new LightData(this);

            if (FileHeader.Version.IsCompatible(1, 6)) {
                GroundTop = Reader.ReadInt32();
                GroundBottom = Reader.ReadInt32();
                GroundLeft = Reader.ReadInt32();
                GroundRight = Reader.ReadInt32();
            }

            var objectCount = Reader.ReadInt32();
            for (var i = 0; i < objectCount; i++) {
                var objectType = (MapObjectType)Reader.ReadInt32();
                switch (objectType) {
                    case MapObjectType.Model:
                        var modelObject = new Objects.Model(this);
                        Objects.Add(modelObject);
                        break;

                    case MapObjectType.Light:
                        var lightObject = new Objects.Light(this);
                        Objects.Add(lightObject);
                        break;

                    case MapObjectType.Sound:
                        var soundObject = new Objects.Sound(this);
                        Objects.Add(soundObject);
                        break;

                    case MapObjectType.Effect:
                        var effectObject = new Objects.Effect(this);
                        Objects.Add(effectObject);
                        break;

                    default:
                        Debug.WriteLine("[RSW] Unknown object with type {0}", objectType);
                        break;
                }
            }

            // @TODO: QuadTree

            return true;
        }

    }

}