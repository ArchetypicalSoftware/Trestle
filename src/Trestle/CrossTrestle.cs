using System;
using Archetypical.Software.Trestle.Abstractions;

namespace Archetypical.Software.Trestle
{
    public static class CrossTrestle
    {
        static Lazy<ITrestle> TTS = new Lazy<ITrestle>(() => CreateBridge(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static ITrestle Current
        {
            get
            {
                var ret = TTS.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static ITrestle CreateBridge()
        {
#if PORTABLE
            return null;
#else
            return new Bridge();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the Archetypical.Software.Trestle NuGet package from your main application project in order to reference the platform-specific implementation.");
        }

    }
}
