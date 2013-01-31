using ikvm.runtime;
using java.lang;

namespace Xpohama.Tikeria {
    public class SystemClassLoader : ClassLoader {
        public SystemClassLoader(ClassLoader parent)
            : base(new AppDomainAssemblyClassLoader(typeof(SystemClassLoader).Assembly)) {
        }
    }
}