using System;

namespace Example {
    public static class MyType {
        public static void Print ()
        {
#if V1
            Console.WriteLine ("v1");
#elif V2
            Console.WriteLine ("v2");
#else
            Console.WriteLine ("unknown");
#endif
        }
    }
}