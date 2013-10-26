using System.Reflection;

namespace Zeus.Library {

    public class MethodInvokeHelper {

        public object[] MethodArguments { get; set; }

        public MethodInfo MethodInfo { get; protected set; }

        public object MethodObject { get; set; }


        public MethodInvokeHelper(MethodInfo info, object obj = null, object[] args = null) {
            MethodInfo = info;
            MethodObject = obj;
            MethodArguments = args;
        }


        /// <summary>
        ///     Wrapper for <see cref="MethodInfo.Invoke()" />
        /// </summary>
        public void Invoke() {
            MethodInfo.Invoke(MethodObject, MethodArguments);
        }

    }

}