using System;

namespace Zeus.Library.Scripting {

    public class NoAssemblyFoundException : Exception {

        public NoAssemblyFoundException(string message)
            : base(message) {

        }

    }

}