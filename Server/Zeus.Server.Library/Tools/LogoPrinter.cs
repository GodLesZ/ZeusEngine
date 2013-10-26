using System;
using System.Collections.Generic;
using System.Linq;

namespace Zeus.Server.Library.Tools {

    /// <summary>
    /// A simple logo printer for display logos in ascii art
    /// </summary>
    public class LogoPrinter {
        protected readonly List<string> lines = new List<string>();
        protected string logoStart = string.Empty;
        protected string logoEnd = string.Empty;

        public ServerConsoleColor TextColor {
            get;
            set;
        }

        public ServerConsoleColor PrefixColor {
            get;
            set;
        }

        public ServerConsoleColor SufixColor {
            get;
            set;
        }

        public ServerConsoleColor StartEndColor {
            get;
            set;
        }

        public ServerConsoleColor CopyrightColor {
            get;
            set;
        }

        public string PrefixSign {
            get;
            set;
        }

        public string SufixSign {
            get;
            set;
        }



        public LogoPrinter(string prefix = "(", string sufix = ")", string[] textLines = null) {
            if (textLines == null || textLines.Any() == false) {
                throw new ArgumentNullException("textLines");
            }
            PrefixSign = prefix;
            SufixSign = sufix;

            lines.AddRange(textLines);

            var len = lines[0].Length;
            logoStart = "";
            for (var i = 0; i < len; i += 2) {
                logoStart += "-=";
            }

            if (logoStart.Length < len) {
                logoStart += "-";
            } else if (logoStart.Length > len) {
                logoStart = logoStart.Substring(0, len);
            }
            logoEnd = logoStart;
        }


        public void PrintLogo() {
            PrintColoredLine(logoStart, PrefixColor);

            for (var i = 0; i < lines.Count - 1; i++) {
                PrintColoredLine(lines[i], TextColor);
            }

            PrintColoredLine(lines[lines.Count - 1], CopyrightColor);

            PrintColoredLine(logoEnd, PrefixColor);
        }


        protected void PrintColoredLine(string text, ServerConsoleColor color) {
            ServerConsole.Write(PrefixColor, PrefixSign);
            ServerConsole.Write(color, text);
            ServerConsole.WriteLine(SufixColor, SufixSign);
        }

    }

}
