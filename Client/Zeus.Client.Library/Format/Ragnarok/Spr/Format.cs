using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Zeus.Client.Library.Format.Compression;
using PalFormat = Zeus.Client.Library.Format.Ragnarok.Pal.Format;

namespace Zeus.Client.Library.Format.Ragnarok.Spr {

    /// <summary>
    ///     A sprite contains a subset of images for one or more animations (.act).
    ///     A sprite contains also the instance of a <see cref="PalFormat"/>.
    /// <para>
    ///     Images are colored by the internal <see cref="PalFormat"/> or containing there own RGBA values.
    /// </para>
    /// </summary>
    public sealed class Format : FileFormat {
        public const string FormatExtension = ".spr";

        public override char[] ExpectedMagicBytes {
            get { return new[] { 'S', 'P' }; }
        }

        public List<SpriteImage> Images {
            get {
                var images = new List<SpriteImage>();
                if (ImagesPal != null) {
                    images.AddRange(ImagesPal);
                }
                if (ImagesRgba != null) {
                    images.AddRange(ImagesRgba);
                }
                return images;
            }
        }

        public List<SpriteImagePal> ImagesPal {
            get;
            protected set;
        }

        public List<SpriteImageRgba> ImagesRgba {
            get;
            protected set;
        }

        public SpriteImage this[int index, bool rgba] {
            get {
                if (rgba) {
                    if (index < 0 || index >= ImagesRgba.Count) {
                        throw new ArgumentOutOfRangeException("index", "RGBA Index is out of range: " + index);
                    }
                    return ImagesRgba[index];
                }
                if (index < 0 || index >= ImagesPal.Count) {
                    throw new ArgumentOutOfRangeException("index", "PAL Index is out of range: " + index);
                }
                return ImagesPal[index];
            }
        }

        public SpriteImage this[int index] {
            get {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException("index", "Index is out of range: " + index);
                }

                // Try to identify pal or rgba using index
                if (ImagesPal == null || index < ImagesPal.Count) {
                    return this[index, false];
                }
                index -= ImagesPal.Count;
                return this[index, true];

            }
        }

        public PalFormat Palette {
            get;
            protected set;
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


        public override bool Write(string filePath, bool overwrite) {
            return base.Write(filePath, overwrite);
        }


        public void ResetImages() {
            foreach (var entry in ImagesPal) {
                entry.Image = null;
            }
            foreach (var entry in ImagesRgba) {
                entry.Image = null;
            }
        }


        public bool DrawPalImage(int imageIndex) {
            if (ImagesPal == null || ImagesPal.Count <= imageIndex) {
                return false;
            }
            if (imageIndex < 0 || imageIndex >= ImagesPal.Count) {
                return false;
            }

            SpriteImagePal sprImg = ImagesPal[imageIndex];
            if (FileHeader.Version >= 0x201 && sprImg.Decoded == false) {
                sprImg.Data = RLE.Decode(sprImg.Data);
                sprImg.Decoded = true;
            }
            if (sprImg.Data == null || sprImg.Data.Length == 0 || sprImg.Width < 1 || sprImg.Height < 1) {
                return false;
            }

            sprImg.Image = new Bitmap(sprImg.Width, sprImg.Height);
            var fb = new FastBitmap(sprImg.Image);

            fb.LockImage();
            for (var x = 0; x < sprImg.Width; x++) {
                for (var y = 0; y < sprImg.Height; y++) {
                    var index = (x + (y * sprImg.Width));
                    if (index >= sprImg.Data.Length) {
                        fb.SetPixel(x, y, Color.Transparent);
                        continue;
                    }
                    fb.SetPixel(x, y, Palette[sprImg.Data[index]]);
                }
            }
            fb.UnlockImage();

            return true;
        }

        public bool DrawRgbaImage(int imageIndex) {
            if (ImagesRgba == null || ImagesRgba.Count <= imageIndex) {
                return false;
            }
            if (imageIndex < 0 || imageIndex >= ImagesRgba.Count) {
                return false;
            }

            SpriteImageRgba sprImg = ImagesRgba[imageIndex];
            if (sprImg == null || sprImg.Data == null || sprImg.Data.Length == 0 || sprImg.Width < 1 || sprImg.Height < 1) {
                return false;
            }

            sprImg.Image = new Bitmap(sprImg.Width, sprImg.Height);
            var fb = new FastBitmap(sprImg.Image);

            var index = 0;
            fb.LockImage();
            for (var y = 0; y < sprImg.Height; y++) {
                for (var x = 0; x < sprImg.Width; x++, index += 4) {
                    // A B G R
                    int alpha = sprImg.Data[index];
                    int blue = sprImg.Data[index + 1];
                    int green = sprImg.Data[index + 2];
                    int red = sprImg.Data[index + 3];
                    var col = Color.FromArgb(alpha, red, green, blue);
                    fb.SetPixel(x, y, col);
                }
            }
            fb.UnlockImage();

            return true;
        }

        public bool DrawImage(int imageIndex) {
            if (imageIndex < 0) {
                return false;
            }

            // Try to identify pal or rgba using index
            if (ImagesPal != null && imageIndex >= ImagesPal.Count) {
                imageIndex -= ImagesPal.Count;
                return DrawRgbaImage(imageIndex);
            }

            return DrawPalImage(imageIndex);
        }


        public bool IsDrawnPal(int imageIndex) {
            return (ImagesPal != null && ImagesPal.Count > imageIndex && ImagesPal[imageIndex].Image != null);
        }

        public bool IsDrawnRgba(int imageIndex) {
            return (ImagesRgba != null && ImagesRgba.Count > imageIndex && ImagesRgba[imageIndex].Image != null);
        }

        public bool IsDrawn(int imageIndex) {
            if (imageIndex < 0) {
                return false;
            }

            // Try to identify pal or rgba using index
            if (ImagesPal != null && imageIndex >= ImagesPal.Count) {
                imageIndex -= ImagesPal.Count;
                return IsDrawnRgba(imageIndex);
            }

            return IsDrawnPal(imageIndex);
        }


        public void AddImagePal(Bitmap image, int position) {
            var img = new SpriteImagePal {
                Width = (ushort)image.Width,
                Height = (ushort)image.Height,
                Image = image.Clone() as Bitmap,
                Decoded = true
            };
            img.Size = (ushort)(img.Width * img.Height);
            img.Data = new byte[img.Size];

            // build data
            for (var x = 0; x < img.Width; x++) {
                for (var y = 0; y < img.Height; y++) {
                    var c = image.GetPixel(x, y);
                    var i = Palette.IndexOf(c);
                    if (i == -1) {
                        continue; // @TODO: color not found?
                    }
                    img.Data[(x + (y * img.Width))] = (byte)i;
                }
            }

            if (position >= ImagesPal.Count) {
                ImagesPal.Add(img);
            } else {
                ImagesPal[position].Image = null; // force redraw
                ImagesPal.Insert(position, img);
            }
        }


        public void RemoveImagePal(int imageIndex) {
            if (imageIndex < 0 || imageIndex >= ImagesPal.Count) {
                return;
            }

            ImagesPal.RemoveAt(imageIndex);
        }

        public void RemoveImageRgba(int imageIndex) {
            if (imageIndex < 0 || imageIndex >= ImagesRgba.Count) {
                return;
            }

            ImagesRgba.RemoveAt(imageIndex);
        }

        public void RemoveImage(int imageIndex) {
            if (imageIndex < 0) {
                return;
            }

            // Try to identify pal or rgba using index
            if (ImagesPal != null && imageIndex >= ImagesPal.Count) {
                imageIndex -= ImagesPal.Count;
                RemoveImageRgba(imageIndex);
                return;
            }

            RemoveImagePal(imageIndex);
        }


        public Bitmap GetImageTransparentPal(int index) {
            if (ImagesPal == null || ImagesPal.Count <= index) {
                return null;
            }
            if (IsDrawnPal(index) == false) {
                if (DrawPalImage(index) == false) {
                    throw new Exception("Failed to draw pal-image on index #" + index);
                }
            }

            var img = ImagesPal[index].Image.Clone() as Bitmap;
            if (img == null) {
                throw new Exception("Invalid pal image on index #" + index);
            }
            var bg = Palette[0];
            var fb = new FastBitmap(img);

            fb.LockImage();
            for (var x = 0; x < img.Width; x++) {
                for (var y = 0; y < img.Height; y++) {
                    if (fb.GetPixel(x, y) == bg) {
                        fb.SetPixel(x, y, Color.Transparent);
                    }
                }
            }
            fb.UnlockImage();

            return img;
        }

        public Bitmap GetImageTransparentRgba(int index) {
            if (ImagesRgba == null || ImagesRgba.Count <= index) {
                return null;
            }
            if (IsDrawnRgba(index) == false) {
                DrawRgbaImage(index);
            }

            var img = ImagesRgba[index].Image.Clone() as Bitmap;
            if (img == null) {
                throw new Exception("Invalid rgba image on index #" + index);
            }
            var bg = Color.Fuchsia;
            var fb = new FastBitmap(img);

            fb.LockImage();
            for (var x = 0; x < img.Width; x++) {
                for (var y = 0; y < img.Height; y++) {
                    if (fb.GetPixel(x, y) == bg) {
                        fb.SetPixel(x, y, Color.Transparent);
                    }
                }
            }
            fb.UnlockImage();

            return img;
        }

        public Bitmap GetImageTransparent(int imageIndex, bool rgba) {
            if (rgba) {
                return GetImageTransparentRgba(imageIndex);
            }
            return GetImageTransparentPal(imageIndex);
        }

        public Bitmap GetImageTransparent(int imageIndex) {
            if (imageIndex < 0) {
                return null;
            }

            // Try to identify pal or rgba using index
            if (ImagesPal != null && imageIndex >= ImagesPal.Count) {
                imageIndex -= ImagesPal.Count;
                return GetImageTransparent(imageIndex, true);
            }

            return GetImageTransparent(imageIndex, false);
        }


        public override void Flush(bool onDestruct) {
            if (onDestruct == false) {
                // cleanup only on destruct!
                base.Flush(false);
                return;
            }

            if (Palette != null) {
                FreeSymbol(Palette);
            }
            if (ImagesPal != null) {
                foreach (var entry in ImagesPal) {
                    FreeSymbol(entry.Image);
                    FreeSymbol(entry.Data);
                }
            }
            if (ImagesRgba != null) {
                foreach (var entry in ImagesRgba) {
                    FreeSymbol(entry.Image);
                    FreeSymbol(entry.Data);
                }
            }

            base.Flush(true);
        }

        public override string ToString() {
            return string.Format("0x{0:X2}, {1}x pal, {2}x rgba", (int)FileHeader.Version, ImagesPal.Count, ImagesRgba.Count);
        }

        protected override bool ReadInternal() {
            if (base.ReadInternal() == false) {
                return false;
            }

            ImagesPal = new List<SpriteImagePal>();
            ImagesRgba = new List<SpriteImageRgba>();
            Palette = new PalFormat();

            if (FileHeader.Version.Major > 2) {
                // Unsupported version
                return false;
            }

            int imgPalCount = Reader.ReadUInt16();
            int imgRgbaCount = 0;
            if (FileHeader.Version >= 0x201) {
                imgRgbaCount = Reader.ReadUInt16();
            }

            // Images - Palette \\
            for (var i = 0; i < imgPalCount; i++) {
                var imgPal = new SpriteImagePal {
                    Width = Reader.ReadUInt16(),
                    Height = Reader.ReadUInt16()
                };
                if (FileHeader.Version >= 0x201) {
                    imgPal.Size = Reader.ReadUInt16();
                } else {
                    imgPal.Size = (ushort)(imgPal.Width * imgPal.Height);
                }
                imgPal.Data = Reader.ReadBytes(imgPal.Size);

                ImagesPal.Add(imgPal);
            }

            // Images - RGBA \\
            for (int i = 0; i < imgRgbaCount; i++) {
                var imgRgba = new SpriteImageRgba {
                    Width = Reader.ReadUInt16(),
                    Height = Reader.ReadUInt16()
                };

                int size = (imgRgba.Width * imgRgba.Height * 4);
                imgRgba.Data = Reader.ReadBytes(size);

                ImagesRgba.Add(imgRgba);
            }

            // Palette \\
            Reader.BaseStream.Position = (Reader.BaseStream.Length - (4 * PalFormat.ColorCount));

            Palette.Read(Reader.BaseStream);

            Flush();
            return true;
        }
    }

}