using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace Zeus.Client.Library.Format.Ragnarok.Pal {

    /// <summary>
    ///     Contains a list of 256 colors, defined in a RGBA style.
    ///     The alpha channel is reversed, so 255 means visible and 0 invisible.
    /// <para>
    ///     Palette's were used in a Sprite and contains there basic color or 
    ///     as a single file to overlay a sprite base-palette.
    /// </para>     
    /// </summary>
    public class Format : FileFormat, IList<Color> {
        public const string FormatExtension = ".pal";
        public const int ColorCount = 256;

        private List<Color> _colors;
        public override char[] ExpectedMagicBytes {
            get { return null; }
        }

        public Color this[int index] {
            get { return _colors[index]; }
            set { _colors[index] = value; }
        }


        public Format(byte[] data)
            : base(data) {
        }

        public Format(Stream stream)
            : base(stream) {
        }

        public Format(string filepath)
            : base(filepath) {
        }

        public Format() {
        }


        public override bool Write(string destinationPath) {
            if (base.Write(destinationPath) == false) {
                return false;
            }

            for (var i = 0; i < ColorCount; i++) {
                Writer.Write(this[i].R);
                Writer.Write(this[i].G);
                Writer.Write(this[i].B);
                Writer.Write(this[i].A);
            }

            return true;
        }


        public void CreateColorChart(int rectSize, Stream stream) {
            var sizeX = (16 * rectSize);
            var sizeY = (16 * rectSize);
            var index = 0;
            using (var img = new Bitmap(sizeX, sizeY)) {
                using (var g = Graphics.FromImage(img)) {
                    var font = new Font(FontFamily.GenericSerif.Name, 8.0f);
                    var brush = Brushes.Black;
                    var brushAlt = Brushes.White;

                    for (var y = 0; y < sizeY; y += rectSize) {
                        for (var x = 0; x < sizeX; x += rectSize) {
                            var rect = new Rectangle(x, y, (x + rectSize), (y + rectSize));
                            var p = new Pen(this[index], 1);
                            index++;

                            g.FillRectangle(p.Brush, rect);

                            var indexName = index.ToString(CultureInfo.InvariantCulture);
                            var indexSize = g.MeasureString(indexName, font);
                            var sx = (x + (rectSize / 2) - (int)(indexSize.Width / 2f));
                            var sy = (y + (rectSize / 2) - (int)(indexSize.Height / 2f));
                            if (p.Color.R < 30 && p.Color.G < 30 && p.Color.B < 30) {
                                g.DrawString(indexName, font, brushAlt, new Point(sx, sy));
                            } else {
                                g.DrawString(indexName, font, brush, new Point(sx, sy));
                            }
                        }
                    }

                    img.Save(stream, ImageFormat.Png);
                }
            }
        }

        public void CreateColorChart(int rectSize, string filepath) {
            using (var fs = File.OpenWrite(filepath)) {
                CreateColorChart(rectSize, fs);
            }
        }

        public void ApplyPalette(string filePath) {
            var pal = new Format(filePath);
            Clear();
            AddRange(pal);
        }


        protected override bool ReadInternal() {
            // Skip to call base, because pal dont have a version or magic bytes

            if (_colors == null) {
                _colors = new List<Color>(ColorCount);
            }
            
            Clear();
            
            for (var i = 0; i < ColorCount; i++) {
                var r = Reader.ReadByte();
                var g = Reader.ReadByte();
                var b = Reader.ReadByte();
                var a = Reader.ReadByte();
                Add(Color.FromArgb(255 - a, r, g, b));
            }

            return true;
        }

        #region IList Implementation
        public int Count {
            get { return _colors.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public void Add(Color col) {
            _colors.Add(col);
        }

        public int IndexOf(Color item) {
            return _colors.IndexOf(item);
        }

        public void Insert(int index, Color item) {
            _colors.Insert(index, item);
        }

        public void RemoveAt(int index) {
            _colors.RemoveAt(index);
        }


        public void Clear() {
            _colors.Clear();
        }

        public bool Contains(Color item) {
            return _colors.Contains(item);
        }

        public void CopyTo(Color[] array, int arrayIndex) {
            _colors.CopyTo(array, arrayIndex);
        }

        public bool Remove(Color item) {
            return _colors.Remove(item);
        }

        public IEnumerator<Color> GetEnumerator() {
            return _colors.GetEnumerator();
        }

        public void AddRange(IEnumerable<Color> col) {
            _colors.AddRange(col);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _colors.GetEnumerator();
        }
        #endregion
    }

}