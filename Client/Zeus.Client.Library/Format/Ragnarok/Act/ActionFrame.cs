using System.Collections.Generic;

namespace Zeus.Client.Library.Format.Ragnarok.Act {

    public class ActionFrame : List<ActionFrameImage> {

        public int Index { get; set; }

        public int EventIndex { get; set; }

        public List<ActionAttachmentPoint> AttachmentPointers { get; protected set; }


        public ActionFrame() {
            AttachmentPointers = new List<ActionAttachmentPoint>();
        }

    }

}