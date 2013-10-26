using System;

namespace Zeus.Library.Scripting {

    /// <summary>
    ///     Script call priority attribute for methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CallPriorityAttribute : Attribute {

        public int Priority { get; set; }


        public CallPriorityAttribute(int priority) {
            Priority = priority;
        }

    }

}