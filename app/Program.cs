using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

// See https://aka.ms/new-console-template for more information
public class App {
    public static void Main (string[] args)
    {
        foreach (var path in args) {
#if ALC
            var c = new AssemblyLoadContext (path, true);
            var a = c.LoadFromAssemblyPath (Path.GetFullPath (path));
#else
            var a = Assembly.LoadFrom (path);
#endif
            var t = a.GetType ("Example.MyType");
            var m = t.GetMethod ("Print");
            m.Invoke (null, null);
        }
    }
}
