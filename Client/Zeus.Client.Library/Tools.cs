using System.IO;
using System.Text;

namespace Zeus.Client.Library {

    public sealed class Tools {

        public static string UnifyPath(string filepath) {
            while (filepath.Contains("\\")) {
                filepath = filepath.Replace('\\', '/');
            }

            return filepath;
        }

        public static string GetGrfPath(string filepath) {
            var unifiedPath = UnifyPath(filepath);

            if (unifiedPath.StartsWith("data/")) {
                return unifiedPath;
            }

            // Skip until "data"
            var parts = unifiedPath.Split(new[] {'/'});
            var i = 0;
            while (parts[i++] != "data" && i < parts.Length) {
            }

            var path = new StringBuilder("data/"); // i is now 1 index above "data"
            for (; i < parts.Length; i++) {
                path.Append(parts[i]);
                if ((i + 1) < parts.Length) {
                    path.Append("/");
                }
            }

            // A filepath dosnt need a final slash!
            var indexOfDot = unifiedPath.LastIndexOf('.');
            if (indexOfDot == -1 && path.ToString() != "data/") {
                // Path dosnt contain a . (dot), so we assume its a folder
                // => folder needs a slash!
                path.Append("/");
            }


            // If no "data" was found, we have a blank "data/" in path variable
            // So we need to append the filename if a filename was given (. (dot) found..)
            if (path.ToString() == "data/" && unifiedPath.LastIndexOf('.') != -1) {
                path.Append(Path.GetFileName(filepath));
            }

            return path.ToString();
        }
    }

}