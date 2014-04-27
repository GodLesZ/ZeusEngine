using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RsmModel = Zeus.Client.Library.Format.Ragnarok.Rsm.Format;

namespace Zeus.Client.Library.Format.Ragnarok.Rsw.Objects {

    public class Model : MapObjectBase {

        public int AnimationType {
            get;
            protected set;
        }

        public float AnimationSpeed {
            get;
            protected set;
        }

        public int BlockType {
            get;
            protected set;
        }

        public string Filename {
            get;
            protected set;
        }

        public RsmModel RsmModel {
            get;
            protected set;
        }

        public string NodeName {
            get;
            protected set;
        }

        public Vector3 Rotation {
            get;
            protected set;
        }

        public Vector3 Scale {
            get;
            protected set;
        }


        public Model(Format rsw)
            : base(rsw, MapObjectType.Model) {
            // Defaults
            RsmModel = null;
            AnimationType = 0;
            AnimationSpeed = 0;
            BlockType = 0;

            if (_parentRsw.FileHeader.Version.IsCompatible(1, 3)) {
                Name = _parentRsw.Reader.ReadStringIso(40);
                AnimationType = _parentRsw.Reader.ReadInt32();
                AnimationSpeed = _parentRsw.Reader.ReadSingle();
                BlockType = _parentRsw.Reader.ReadInt32();

                // Sanity settings
                if (AnimationSpeed < 0.0f || AnimationSpeed >= 100.0f) {
                    AnimationSpeed = 1.0f;
                }
            }

            Filename = _parentRsw.Reader.ReadStringIso(80);
            NodeName = _parentRsw.Reader.ReadStringIso(80);
            Position = new Vector3(
                _parentRsw.Reader.ReadSingle() / 5,
                _parentRsw.Reader.ReadSingle() / 5,
                _parentRsw.Reader.ReadSingle() / 5
            );
            Rotation = new Vector3(
                _parentRsw.Reader.ReadSingle(), 
                _parentRsw.Reader.ReadSingle(), 
                _parentRsw.Reader.ReadSingle()
            );
            Scale = new Vector3(
                _parentRsw.Reader.ReadSingle() / 5, 
                _parentRsw.Reader.ReadSingle() / 5, 
                _parentRsw.Reader.ReadSingle() / 5
            );
        }


        public void Load(GraphicsDevice device, string dataPath) {
            string filepath = string.Format("{0}model/{1}", dataPath, Filename);
            RsmModel = new RsmModel(filepath);
            RsmModel.CalculateMeshes(device);
        }

        public void Draw(GraphicsDevice device, BasicEffect effect, Matrix view, Matrix projection, Matrix world) {
            RsmModel.Draw(device, effect, view, projection, world);
        }

    }

}