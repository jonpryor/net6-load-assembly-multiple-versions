using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

public class App {
    public static void Main (string[] args)
    {
        AssemblyLoad load = AssemblyLoad.Reflection;
        int start = 0;

        if (args.Length > 0 && Enum.TryParse<AssemblyLoad>(args [0], ignoreCase:true, result:out load)) {
            start++;
        }
        for (int i = start; i < args.Length; ++i) {
            var path = args [i];

            Assembly? a = Load (load, Path.GetFullPath (path));
            var t = a.GetType ("Example.MyType");
            var m = t.GetMethod ("Print");
            m.Invoke (null, null);
        }
    }

    static Assembly? Load (AssemblyLoad how, string path)
    {
        switch (how) {
            case AssemblyLoad.Reflection:
                return Assembly.LoadFrom (path);
            case AssemblyLoad.Context:
                var c = new AssemblyLoadContext (path, true);
                return c.LoadFromAssemblyPath (path);
            case AssemblyLoad.DefaultContext:
                return AssemblyLoadContext.Default.LoadFromAssemblyPath (path);
        }
        throw new NotSupportedException ();
    }
}

enum AssemblyLoad {
    Reflection = 0,
    Context = 1,
    DefaultContext = 2,
}
