using System;

namespace Zeus.Client.Library.Format.Ragnarok.Gat {

    public class GroundCell {

        public float LeftBottom { get; set; }

        public float LeftTop { get; set; }
        public float RightBottom { get; set; }

        public float RightTop { get; set; }

        public GroundCellType Type { get; set; }

        public GroundCell(float f1, float f2, float f3, float f4, byte t) {
            LeftBottom = f1;
            RightBottom = f2;
            LeftTop = f3;
            RightTop = f4;
            Type = (GroundCellType) Enum.Parse(typeof (GroundCellType), t.ToString());
        }

    }

}