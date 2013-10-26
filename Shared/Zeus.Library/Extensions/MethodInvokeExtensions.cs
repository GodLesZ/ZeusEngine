namespace Zeus.Library.Extensions {

    public static class MethodInvokeExtensions {

        public static void Invoke<TObjType>(this object objInstance, string methodName, object[] methodArgs) {
            var info = typeof(TObjType).GetMethod(methodName);
            new MethodInvokeHelper(info, objInstance, methodArgs).Invoke();
        }

        public static void Invoke<TObjType>(this object objInstance, string methodName) {
            objInstance.Invoke<TObjType>(methodName, null);
        }

    }

}