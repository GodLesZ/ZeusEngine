using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Zeus.Client.Library {

    public unsafe class FastBitmap : IDisposable {

        public struct Pixel {
            public byte Alpha;
            public byte Blue;
            public byte Green;
            public byte Red;

            public override string ToString() {
                return "(" + Alpha + ", " + Red + ", " + Green + ", " + Blue + ")";
            }
        }

        protected BitmapData _bitmapData = null;
        protected Pixel* _currentPixel = null;
        protected byte* _pointer = null;
        protected int _width = 0;
        protected Bitmap _workingBitmap = null;

        public byte* Pointer {
            get { return _pointer; }
        }


        public FastBitmap(Bitmap inputBitmap)
            : this(inputBitmap, false) {
        }

        public FastBitmap(Bitmap inputBitmap, bool lockImage) {
            _workingBitmap = inputBitmap;

            if (lockImage) {
                LockImage();
            }
        }


        public void Dispose() {
            UnlockImage();
        }


        public void LockImage() {
            var bounds = new Rectangle(Point.Empty, _workingBitmap.Size);

            _width = bounds.Width*sizeof (Pixel);
            if (_width%4 != 0) {
                _width = 4*(_width/4 + 1);
            }

            // Lock Image
            _bitmapData = _workingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            _pointer = (Byte*) _bitmapData.Scan0.ToPointer();
        }

        public void UnlockImage() {
            if (_bitmapData == null) {
                return;
            }

            _workingBitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _pointer = null;
        }


        public Color GetPixel(int x, int y) {
            _currentPixel = (Pixel*) (Pointer + y*_width + x*sizeof (Pixel));
            return Color.FromArgb(_currentPixel->Alpha, _currentPixel->Red, _currentPixel->Green, _currentPixel->Blue);
        }

        public Color GetPixelNext() {
            _currentPixel++;
            return Color.FromArgb(_currentPixel->Alpha, _currentPixel->Red, _currentPixel->Green, _currentPixel->Blue);
        }

        public void SetPixel(int x, int y, Color color) {
            var data = (Pixel*) (Pointer + (y*_width) + (x*sizeof (Pixel)));
            data->Alpha = color.A;
            data->Red = color.R;
            data->Green = color.G;
            data->Blue = color.B;
        }

    }

}