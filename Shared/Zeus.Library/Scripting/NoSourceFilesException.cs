using System;

namespace Zeus.Library.Scripting {

    public class NoSourceFilesException : Exception {

        public NoSourceFilesException(string message)
            : base(message) {

        }

    }

}