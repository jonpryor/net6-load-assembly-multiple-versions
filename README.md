# .NET 6 and loading different versions of the same assembly

Can different versions of the "same" assembly be loaded at the same time in .NET 6?

[`app`](app) is the app, which uses either `Assembly.LoadFrom()` or
`AssemblyLoadContext` to load an assembly.  It attempts to load each assembly
specified on the command-line, then find and execute the method
`Example.MyType.Print()` from the assembly:

```
% dotnet app/bin/Debug/net6.0/multi-versions.dll …
```

[`Lib.cs`](Lib.cs) is the source code to the library to load, specifying a public
static method `Example.MyType.Print()`.

[`lib-v1`](lib-v1) builds `Lib.cs` with `V1` defined, and `$(Version)`=1.0.0.0.

```
% monodis --assembly lib-v1/bin/Debug/net6.0/lib.dll
Assembly Table
Name:          lib
Hash Algoritm: 0x00008004
Version:       1.0.0.0
Flags:         0x00000000
PublicKey:     BlobPtr (0x00000000)
	Zero sized public key
Culture:       
```

[`lib-v2`](lib-v2) builds `Lib.cs` with `V2` defined, and `$(Version)`=2.0.0.0.

```
% monodis --assembly lib-v2/bin/Debug/net6.0/lib.dll
Assembly Table
Name:          lib
Hash Algoritm: 0x00008004
Version:       2.0.0.0
Flags:         0x00000000
PublicKey:     BlobPtr (0x00000000)
	Zero sized public key
Culture:       
```

Can It Run?  Not with `Assembly.LoadFrom()`:

```
% make all
% make run
dotnet app/bin/Debug/net6.0/app.dll  lib-v1/bin/Debug/net6.0/lib.dll lib-v2/bin/Debug/net6.0/lib.dll
v1
Unhandled exception. System.IO.FileLoadException: Assembly with same name is already loaded
   at System.Runtime.Loader.AssemblyLoadContext.LoadFromPath(IntPtr ptrNativeAssemblyLoadContext, String ilPath, String niPath, ObjectHandleOnStack retAssembly) in System.Private.CoreLib.dll:token 0x6003a0b+0x2e
   at System.Runtime.Loader.AssemblyLoadContext.LoadFromAssemblyPath(String assemblyPath) in System.Private.CoreLib.dll:token 0x6003a45+0x48
   at System.Reflection.Assembly.LoadFrom(String assemblyFile) in System.Private.CoreLib.dll:token 0x6004ffa+0x74
   at App.Main(String[] args) in /Users/jon/Documents/Developer/tmp/multi-versions/app/Program.cs:line 25
make: *** [run] Abort trap: 6
```

It *can* run with `AssemblyLoadContext`:

```
% make run STYLE=Context
dotnet app/bin/Debug/net6.0/app.dll  true lib-v1/bin/Debug/net6.0/lib.dll lib-v2/bin/Debug/net6.0/lib.dll
v1
v2
```

It fails when using `AssemblyLoadContext.Default.LoadFromAssemblyPath()`:

```
% make run STYLE=DefaultContext
dotnet app/bin/Debug/net6.0/app.dll DefaultContext lib-v1/bin/Debug/net6.0/lib.dll lib-v2/bin/Debug/net6.0/lib.dll
v1
Unhandled exception. System.IO.FileLoadException: Assembly with same name is already loaded
   at System.Runtime.Loader.AssemblyLoadContext.LoadFromPath(IntPtr ptrNativeAssemblyLoadContext, String ilPath, String niPath, ObjectHandleOnStack retAssembly) in System.Private.CoreLib.dll:token 0x6003a0b+0x2e
   at System.Runtime.Loader.AssemblyLoadContext.LoadFromAssemblyPath(String assemblyPath) in System.Private.CoreLib.dll:token 0x6003a45+0x48
   at App.Load(AssemblyLoad how, String path) in /Users/jon/Documents/Developer/tmp/multi-versions/app/Program.cs:line 35
   at App.Main(String[] args) in /Users/jon/Documents/Developer/tmp/multi-versions/app/Program.cs:line 19
make: *** [run] Abort trap: 6
```
