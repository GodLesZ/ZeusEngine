using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zeus.Client.Library.Format.Ragnarok.Grf;
using Zeus.Client.Tests.MonoGameClientTest.Formats;

namespace Zeus.Client.Tests.MonoGameClientTest {

    public class MaptestRoGrf : MapTestRO {
        protected RoGrfFile mGrf;
        protected BasicEffect mRsmEffect;

        public MaptestRoGrf() {

        }

        public override void Initialize(Game game, string Filename, string TextureRoot) {
            throw new NotSupportedException("Use .FromGrf()");
        }

        public void FromGrf(Game game, string mapname, string grfFilepath) {
            mClient = game;
            GNDFile.GraphicsDevice = mClient.GraphicsDevice;

            mGrf = new RoGrfFile(grfFilepath);

            //ROMap convMap = ROMapConverter.Convert( Filename, TextureRoot );
            //convMap.Save( Filename + ".gdmap" );

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            mGnd = new GNDFile(mapname + ".gnd", mGrf);
            mRsw = new RSWFile(mapname + ".rsw", mGrf);
            mGat = new GATFile(mapname + ".gat", mGrf);
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("ROMap: " + watch.ElapsedMilliseconds + "ms");

            if (mRsw.ModelData.Count > 0) {
                for (int i = 0; i < mRsw.ModelData.Count; i++) {
                    var texPath = "data/model/" + mRsw.ModelData[i].filename;
                    var rsm = new RSMFile(mClient.GraphicsDevice, texPath, mGrf, true);
                    mRsm.Add(rsm);
                }
            }

            mCenter = new Vector3(mGnd.Width * .5f, 0f, mGnd.Height * .5f);

            mBasicEffect = new BasicEffect(mClient.GraphicsDevice);
            mBasicEffect.TextureEnabled = true;

            mRsmEffect = new BasicEffect(mClient.GraphicsDevice);
            mRsmEffect.VertexColorEnabled = true;
        }


        protected override void DrawModels(Matrix view, Matrix projection) {
            for (int i = 0; i < mRsm.Count; i++) {
                mRsm[i].DrawSingle(mClient.GraphicsDevice, mRsmEffect, view, projection, mWorldMatrix);
            }
        }

    }

}