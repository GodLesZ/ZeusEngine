using System.Collections.Generic;

namespace Zeus.Library.Extensions {

    public static class ListExtensions {

        public static string Join(this IEnumerable<string> list, string glue = ",") {
            return string.Join(glue, list);
        }

    }

}