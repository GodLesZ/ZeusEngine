using System;

namespace Zeus.Server.Library.Tools {

    /// <summary>
    ///     Console Color Array
    /// </summary>
    public enum ServerConsoleColor {
        /// <summary>
        ///     Default color
        /// </summary>
        None = ConsoleColor.Gray,

        /// <summary>
        ///     Basic gray
        /// </summary>
        Gray = ConsoleColor.Gray,

        /// <summary>
        ///     Lime green
        /// </summary>
        Status = ConsoleColor.Green,

        /// <summary>
        ///     Bright white
        /// </summary>
        Info = ConsoleColor.White,

        /// <summary>
        ///     Dark yellow
        /// </summary>
        Warning = ConsoleColor.Yellow,

        /// <summary>
        ///     Bright magenta
        /// </summary>
        Error = ConsoleColor.Magenta,

        /// <summary>
        ///     Bright cyan
        /// </summary>
        Debug = ConsoleColor.Cyan,


        // ConsoleColor orgins
        DarkBlue = ConsoleColor.DarkBlue,
        DarkGreen = ConsoleColor.DarkGreen,
        DarkCyan = ConsoleColor.DarkCyan,
        DarkRed = ConsoleColor.DarkRed,
        DarkMagenta = ConsoleColor.DarkMagenta,
        DarkYellow = ConsoleColor.DarkYellow,
        DarkGray = ConsoleColor.DarkGray,
        Blue = ConsoleColor.Blue,
        Red = ConsoleColor.Red,
        Yellow = ConsoleColor.Yellow,

        Black = ConsoleColor.Black,
    }

}