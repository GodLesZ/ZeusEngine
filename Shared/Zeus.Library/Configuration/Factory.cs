using System;

namespace Zeus.Library.Configuration {

    public class Factory {

        public static T Create<T>(string filepath) where T : IProvider {
            var ctor = typeof(T).GetConstructor(new[] {
                typeof(string)
            });
            if (ctor == null) {
                ctor = typeof(T).GetConstructor(Type.EmptyTypes);
                if (ctor == null) {
                    throw new Exception("No constructor found in \"" + typeof(T) + "\"");
                }
            }

            return (T)ctor.Invoke(new object[] {
                filepath
            });
        }

    }

}