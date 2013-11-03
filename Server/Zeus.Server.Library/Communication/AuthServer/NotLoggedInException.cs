using System;

namespace Zeus.Server.Library.Communication.AuthServer {

    public class NotLoggedInException : Exception {

        public NotLoggedInException() 
            : base("Please log-in first.") {
        }

    }

}