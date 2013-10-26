using System;
using System.Globalization;

namespace Zeus.Server.Library.Tools {

    /// <summary>
    ///     <see cref="Console" /> wrapper to allow colored output
    /// </summary>
    public class ServerConsole {

        private static readonly object Lock = new object();
        protected static ServerConsoleColor _backgroundColor = ServerConsoleColor.Black;


        static ServerConsole() {
            DateFormatString = "[HH:mm:ss]";
            Prefix = "[";
            Suffix = "] ";
            UseColoredPrefixAndSuffix = true;
        }


        public static ServerConsoleColor BackgroundColor {
            get { return _backgroundColor; }
            set {
                _backgroundColor = value;
                Console.BackgroundColor = (ConsoleColor) value;
            }
        }

        /// <summary>
        ///     Status Prefix
        /// </summary>
        public static string Prefix { get; set; }

        /// <summary>
        ///     Status Suffix
        /// </summary>
        public static string Suffix { get; set; }

        /// <summary>
        ///     Using Prefix/Suffix
        /// </summary>
        public static bool UseColoredPrefixAndSuffix { get; set; }

        /// <summary>
        ///     String to Format the DateTime
        /// </summary>
        public static string DateFormatString { get; set; }
        
        protected static void WriteColored(string text, ServerConsoleColor colorPart) {
            if (DateFormatString != null) {
                string s = DateTime.Now.ToString(DateFormatString, CultureInfo.CreateSpecificCulture("de-DE"));
                Console.Write(s);
            }


            if (UseColoredPrefixAndSuffix) {
                text = String.Format("{0}{1}{2}", Prefix, text, Suffix);
            } else {
                Console.Write(Prefix);
            }

            Console.ForegroundColor = (ConsoleColor) colorPart;
            Console.Write(text);
            Console.ResetColor();
            Console.BackgroundColor = (ConsoleColor) _backgroundColor;

            if (UseColoredPrefixAndSuffix == false) {
                Console.Write(Suffix);
            }
        }

        #region base Method defines
        public static void Write(object Object) {
            Console.Write(Object);
        }

        public static void Write(string text) {
            Console.Write(text);
        }

        public static void Write(string text, params object[] args) {
            Write(string.Format(text, args));
        }

        public static void WriteLine(object obj) {
            Console.WriteLine(obj);
        }

        public static void WriteLine(string text) {
            // Check for \r at the end of line
            // IN that case, we cant just write the text + line feed or the \r will be ignored
            // WE have to use simple Write() without line feed!
            if (text.IndexOf('\r') != -1) {
                Write(text);
                return;
            }
            Console.WriteLine(text);
        }

        public static void WriteLine(string text, params object[] args) {
            WriteLine(string.Format(text, args));
        }

        public static int Read() {
            return Console.Read();
        }
        #endregion

        #region basic colored Write/WriteLine
        public static void Write(ServerConsoleColor colorPart, string text) {
            Console.ForegroundColor = (ConsoleColor) colorPart;
            Write(text);
            Console.ResetColor();
            Console.BackgroundColor = (ConsoleColor) _backgroundColor;
        }

        public static void Write(ServerConsoleColor colorPart, string text, params object[] args) {
            Console.ForegroundColor = (ConsoleColor) colorPart;
            Write(String.Format(text, args));
            Console.ResetColor();
            Console.BackgroundColor = (ConsoleColor) _backgroundColor;
        }

        public static void WriteLine(ServerConsoleColor colorPart, string text) {
            Console.ForegroundColor = (ConsoleColor) colorPart;
            WriteLine(text);
            Console.ResetColor();
            Console.BackgroundColor = (ConsoleColor) _backgroundColor;
        }

        public static void WriteLine(ServerConsoleColor colorPart, string text, params object[] args) {
            Console.ForegroundColor = (ConsoleColor) colorPart;
            WriteLine(String.Format(text, args));
            Console.ResetColor();
            Console.BackgroundColor = (ConsoleColor) _backgroundColor;
        }
        #endregion

        #region Status [EConsoleColor.Status]
        public static void Status(string text) {
            lock (Lock) {
                WriteColored("State", ServerConsoleColor.Status);
                Write(text);
            }
        }

        public static void Status(string text, params object[] arg) {
            lock (Lock) {
                WriteColored("State", ServerConsoleColor.Status);
                Write(String.Format(text, arg));
            }
        }

        public static void StatusLine(string text) {
            lock (Lock) {
                WriteColored("State", ServerConsoleColor.Status);
                WriteLine(text);
            }
        }

        public static void StatusLine(string text, params object[] arg) {
            lock (Lock) {
                WriteColored("State", ServerConsoleColor.Status);
                WriteLine(String.Format(text, arg));
            }
        }
        #endregion

        #region Info [EConsoleColor.Info]
        public static void Info(string text) {
            lock (Lock) {
                WriteColored("Info", ServerConsoleColor.Info);
                Write(text);
            }
        }

        public static void Info(string text, params object[] arg) {
            lock (Lock) {
                WriteColored("Info", ServerConsoleColor.Info);
                Write(String.Format(text, arg));
            }
        }

        public static void InfoLine(string text) {
            lock (Lock) {
                WriteColored("Info", ServerConsoleColor.Info);
                WriteLine(text);
            }
        }

        public static void InfoLine(string text, params object[] arg) {
            lock (Lock) {
                WriteColored("Info", ServerConsoleColor.Info);
                WriteLine(String.Format(text, arg));
            }
        }
        #endregion

        #region Warning [EConsoleColor.Warning]
        public static void Warning(string text) {
            lock (Lock) {
                WriteColored("Warning", ServerConsoleColor.Warning);
                Write(text);
            }
        }

        public static void Warning(string text, params object[] arg) {
            lock (Lock) {
                WriteColored("Warning", ServerConsoleColor.Warning);
                Write(String.Format(text, arg));
            }
        }

        public static void WarningLine(string text) {
            lock (Lock) {
                WriteColored("Warning", ServerConsoleColor.Warning);
                WriteLine(text);
            }
        }

        public static void WarningLine(string text, params object[] arg) {
            lock (Lock) {
                WriteColored("Error", ServerConsoleColor.Warning);
                WriteLine(String.Format(text, arg));
            }
        }
        #endregion

        #region Error [EConsoleColor.Error]
        public static void Error(string text) {
            lock (Lock) {
                WriteColored("Error", ServerConsoleColor.Error);
                Write(text);
            }
        }

        public static void Error(string text, params object[] arg) {
            lock (Lock) {
                WriteColored("Error", ServerConsoleColor.Error);
                Write(String.Format(text, arg));
            }
        }

        public static void ErrorLine(string text) {
            lock (Lock) {
                WriteColored("Error", ServerConsoleColor.Error);
                WriteLine(text);
            }
        }

        public static void ErrorLine(string text, params object[] arg) {
            lock (Lock) {
                WriteColored("Error", ServerConsoleColor.Error);
                WriteLine(String.Format(text, arg));
            }
        }
        #endregion

        #region Debug [EConsoleColor.Debug]
        public static void Debug(string text) {
            lock (Lock) {
                WriteColored("Debug", ServerConsoleColor.Debug);
                Write(text);
            }
        }

        public static void Debug(string text, params object[] arg) {
            lock (Lock) {
                WriteColored("Debug", ServerConsoleColor.Debug);
                Write(String.Format(text, arg));
            }
        }

        public static void DebugLine(string text) {
            lock (Lock) {
                WriteColored("Debug", ServerConsoleColor.Debug);
                WriteLine(text);
            }
        }

        public static void DebugLine(string text, params object[] arg) {
            lock (Lock) {
                WriteColored("Debug", ServerConsoleColor.Debug);
                WriteLine(String.Format(text, arg));
            }
        }
        #endregion

    }

}