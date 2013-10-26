using System.Collections.Generic;
using System.Reflection;

namespace Zeus.Library.Scripting {

    /// <summary>
    ///     Compares classes by the custom <see cref="CallPriorityAttribute" />, used for script call-prio
    /// </summary>
    public class CallPriorityComparer : IComparer<MethodInfo> {

        public int Compare(MethodInfo x, MethodInfo y) {
            if (x == null && y == null) {
                return 0;
            }
            if (x == null) {
                return 1;
            }
            if (y == null) {
                return -1;
            }
            return GetPriority(x) - GetPriority(y);
        }


        protected int GetPriority(MethodInfo mi) {
            var objs = mi.GetCustomAttributes(typeof(CallPriorityAttribute), true);
            if (objs.Length == 0) {
                return 0;
            }

            var attr = objs[0] as CallPriorityAttribute;
            return attr == null ? 0 : attr.Priority;

        }

    }

}