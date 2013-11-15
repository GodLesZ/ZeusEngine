using System.Collections.Generic;
using System.IO;

namespace Zeus.Client.Library.Format.Ragnarok.Act {

    public class Format : FileFormat {
        public override char[] ExpectedMagicBytes {
            get { return new[] { 'A', 'C' }; }
        }

        public List<Action> Actions {
            get;
            protected set;
        }

        public List<float> Delays {
            get;
            protected set;
        }

        public List<string> Events {
            get;
            protected set;
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


        protected override bool ReadInternal() {
            Actions = new List<Action>();
            Delays = new List<float>();
            Events = new List<string>();

            if (base.ReadInternal() == false) {
                return false;
            }

            // @TODO: Validate against supported versions? 0x1?? ~ 0x200 - 0x205

            var actionCount = Reader.ReadUInt16();
            var reserved = Reader.ReadChars(10);

            for (var a = 0; a < actionCount; a++) {
                var action = new Action();
                var motionCount = Reader.ReadInt32();

                for (var m = 0; m < motionCount; m++) {
                    action.Add(new Motion(Reader, FileHeader.Version));
                }

                Actions.Add(action);
            }

            
	        // Events
	        if (FileHeader.Version.Version >= 0x201) {
	            var eventCount = Reader.ReadInt32();
                for (var i = 0; i < eventCount; i++) {
                    var eventName = new string(Reader.ReadChars(40));
                    Events.Add(eventName);
                }
	        }
	        
            // Delays
	        if (FileHeader.Version.Version >= 0x202) {
		        if (Actions.Count > 0) {
		            for (var i = 0; i < Actions.Count; i++) {
		                Delays.Add(Reader.ReadSingle());
		            }
		        }
	        }

            return true;
        }

    }

}