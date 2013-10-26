using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.CSharp;
using Zeus.Library.Extensions;
using Zeus.Library.IO;

namespace Zeus.Library.Scripting {

    /// <summary>
    ///     A base script compiler for csharp scripts.
    /// </summary>
    public class ScriptCompiler {

        protected readonly List<string> AdditionalReferences = new List<string>();

        public List<Assembly> CompiledAssemblies { get; protected set; }

        public string DestinationFilepath { get; protected set; }

        public string DestinationHashFilepath {
            get { return string.Format("{0}.hash", DestinationFilepath); }
        }

        public string Namespace { get; protected set; }

        public List<string> SourceAssemblies { get; protected set; }


        /// <summary>
        ///     Creates a new compiler for the given namespace and will store the compiled assembly in the given location.
        /// </summary>
        /// <param name="usedNamespace">Only scripts under this namespace will be used for compile</param>
        /// <param name="destination">Full destination path of final assembly</param>
        public ScriptCompiler(string usedNamespace = "", string destination = "Scripts/Output/Scripts.dll") {
            SourceAssemblies = new List<string>();
            CompiledAssemblies = new List<Assembly>();

            Namespace = usedNamespace;
            DestinationFilepath = destination;
        }


        public bool Compile(bool allowDebug = true, bool useCache = true) {
            // Ensure outoput exists
            Tools.EnsureDirectory(Path.GetDirectoryName(DestinationFilepath));

            // Clear old references
            AdditionalReferences.Clear();

            // Try to compile
            Assembly assembly;
            if (CompileScripts(out assembly, allowDebug, useCache) == false) {
                return false;
            }
            // We have to ensure to get a valid assembly
            if (assembly == null) {
                return false;
            }

            // Append the new assembly to the instance list
            CompiledAssemblies.Add(assembly);

            return true;
        }

        protected string GetCompilerDefines() {
            var defines = new[] {
                "/d:Framework_4_0"
            };

            return defines.Join(" ");
        }

        protected static byte[] GetHashCode(string compiledFile, IEnumerable<string> scriptFiles, bool debug = true) {
            using (var ms = new MemoryStream()) {
                using (var bin = new BinaryWriter(ms)) {
                    var fileInfo = new FileInfo(compiledFile);

                    bin.Write(fileInfo.LastWriteTimeUtc.Ticks);

                    foreach (var scriptFile in scriptFiles) {
                        fileInfo = new FileInfo(scriptFile);

                        bin.Write(fileInfo.LastWriteTimeUtc.Ticks);
                    }

                    bin.Write(debug);

                    ms.Position = 0;

                    using (var sha1 = SHA1.Create()) {
                        return sha1.ComputeHash(ms);
                    }
                }
            }
        }


        protected bool CompileScripts(out Assembly assembly, bool allowDebug = true, bool useCache = true) {

            if (SourceAssemblies.Count == 0) {
                throw new NoSourceFilesException("No source files to compile.");
            }

            // Ensure absolute paths
            /*
            var baseDir = Environment.CurrentDirectory;
            for (var i = 0; i < SourceAssemblies.Count; i++) {
                var sourceAssembly = SourceAssemblies[i];
                if (Path.IsPathRooted(sourceAssembly) == false) {
                    SourceAssemblies[i] = string.Format("{0}/{1}", baseDir, sourceAssembly);
                }
            }
            */

            if (useCache && GetCachedAssembly(out assembly, allowDebug)) {
                if (AdditionalReferences.Contains(assembly.Location) == false) {
                    AdditionalReferences.Add(assembly.Location);
                }

                return true;
            }

            // Remove existing files, if any
            var destinationDir = Path.GetDirectoryName(DestinationFilepath);
            if (string.IsNullOrEmpty(destinationDir)) {
                throw new ArgumentException("Invalid DestinationFilepath");
            }
            var existingFiles = Directory.GetFiles(destinationDir, GetFilenameMask(DestinationFilepath));
            foreach (var existingFile in existingFiles) {
                try {
                    File.Delete(existingFile);
                } catch {
                }
            }

            // Create compiler-provider Parameters
            var providerOptions = new Dictionary<string, string> {
                {
                    "CompilerVersion", "v4.0"
                }
            };

            // Use a charp provider
            using (var provider = new CSharpCodeProvider(providerOptions)) {
                // Find a path for our assembly
                var path = GetUnusedDestinationPath();
                var parms = new CompilerParameters(AdditionalReferences.ToArray(), path, allowDebug);
                var defines = GetCompilerDefines();
                if (defines != null) {
                    parms.CompilerOptions = defines;
                }

                // @TODO: Move params to config
                parms.CompilerOptions = GenerateCompilerOptions(parms.CompilerOptions, allowDebug);
                parms.IncludeDebugInformation = true;
                parms.WarningLevel = 4;

                var results = provider.CompileAssemblyFromFile(parms, SourceAssemblies.ToArray());
                Display(results);
                if (results.Errors.Cast<CompilerError>().Any(e => e.IsWarning == false)) {
                    assembly = null;
                    return false;
                }

                AdditionalReferences.Add(path);

                if (useCache && Path.GetFileName(path) == Path.GetFileName(DestinationFilepath)) {
                    try {
                        var hashCode = GetHashCode(path, SourceAssemblies, allowDebug);

                        using (var fs = File.OpenWrite(DestinationHashFilepath)) {
                            using (var bin = new BinaryWriter(fs)) {
                                bin.Write(hashCode, 0, hashCode.Length);
                            }
                        }
                    } catch {
                    }
                }

                assembly = results.CompiledAssembly;
                return true;
            }
        }

        protected bool GetCachedAssembly(out Assembly assembly, bool debug) {
            assembly = null;

            if (File.Exists(DestinationFilepath) == false) {
                return false;
            }

            var hashPath = string.Format("{0}.hash", DestinationFilepath);

            if (File.Exists(hashPath) == false) {
                return false;
            }

            try {
                var hashCode = GetHashCode(DestinationFilepath, SourceAssemblies, debug);

                using (var fs = File.OpenRead(DestinationHashFilepath)) {
                    using (var bin = new BinaryReader(fs)) {
                        var bytes = bin.ReadBytes(hashCode.Length);

                        if (bytes.Length != hashCode.Length) {
                            return false;
                        }
                        var valid = bytes.Where((t, i) => t != hashCode[i]).Any() == false;
                        if (valid == false) {
                            return false;
                        }

                        assembly = Assembly.LoadFrom(DestinationFilepath);
                        return true;
                    }
                }
            } catch {
            }

            return false;
        }

        protected string GenerateCompilerOptions(string compilerOptions, bool allowDebug) {
            var options = new List<string>(new[] {
                compilerOptions,
                "/nowarn:169,219,414"
            });
            if (allowDebug) {
                options.Add("/debug");
            }

            return string.Join(" ", options);
        }


        protected virtual void Display(CompilerResults results) {
            Debug.WriteLine(results.Errors);
            /*
            if (results.Errors.Count > 0) {
                var errors = new Dictionary<string, List<CompilerError>>(results.Errors.Count, StringComparer.OrdinalIgnoreCase);
                var warnings = new Dictionary<string, List<CompilerError>>(results.Errors.Count, StringComparer.OrdinalIgnoreCase);

                foreach (CompilerError e in results.Errors) {
                    string file = e.FileName;

                    if (string.IsNullOrEmpty(file)) {
                        ServerConsole.ErrorLine("\n# {0}: {1}", e.ErrorNumber, e.ErrorText);
                        continue;
                    }

                    Dictionary<string, List<CompilerError>> table = (e.IsWarning ? warnings : errors);

                    List<CompilerError> list = null;
                    table.TryGetValue(file, out list);

                    if (list == null)
                        table[file] = list = new List<CompilerError>();

                    list.Add(e);
                }

                if (errors.Count > 0)
                    ServerConsole.ErrorLine("failed ({0} errors, {1} warnings)", errors.Count, warnings.Count);
                else
                    ServerConsole.ErrorLine("done ({0} errors, {1} warnings)", errors.Count, warnings.Count);

                string scriptRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts" + Path.DirectorySeparatorChar));
                Uri scriptRootUri = new Uri(scriptRoot);

                if (warnings.Count > 0)
                    ServerConsole.WriteLine(EConsoleColor.DarkYellow, "Script Warnings:");

                foreach (KeyValuePair<string, List<CompilerError>> kvp in warnings) {
                    string fileName = kvp.Key;
                    List<CompilerError> list = kvp.Value;

                    string fullPath = Path.GetFullPath(fileName);
                    string usedPath = Uri.UnescapeDataString(scriptRootUri.MakeRelativeUri(new Uri(fullPath)).OriginalString);

                    ServerConsole.WriteLine(" + {0}:", usedPath);

                    foreach (CompilerError e in list)
                        ServerConsole.WriteLine("\t#{0}: Line {1}/ Col {2}: {3}", e.ErrorNumber, e.Line, e.Column, e.ErrorText);
                }

                if (errors.Count > 0)
                    ServerConsole.WriteLine(EConsoleColor.Error, "Script Errors:");

                foreach (KeyValuePair<string, List<CompilerError>> kvp in errors) {
                    string fileName = kvp.Key;
                    List<CompilerError> list = kvp.Value;

                    string fullPath = Path.GetFullPath(fileName);
                    string usedPath = Uri.UnescapeDataString(scriptRootUri.MakeRelativeUri(new Uri(fullPath)).OriginalString);

                    ServerConsole.WriteLine(" + {0}:", usedPath);

                    foreach (CompilerError e in list)
                        ServerConsole.WriteLine("\t#{0}: Line {1}/ Col {2}: {3}", e.ErrorNumber, e.Line, e.Column, e.ErrorText);
                }
            } else {
                ServerConsole.StatusLine("done (0 errors, 0 warnings)");
            }
            */
        }

        protected string GetUnusedDestinationPath() {
            // Found!
            if (File.Exists(DestinationFilepath) == false) {
                return DestinationFilepath;
            }

            // @TODO: Maybe just use a random number?
            var destinationPath = Path.GetDirectoryName(DestinationFilepath);
            var extension = Path.GetExtension(DestinationFilepath);
            var path = DestinationFilepath;

            for (var i = 2; File.Exists(path) && i <= 1000; i++) {
                path = string.Format("{0}.{1}{2}", destinationPath, i, extension);
            }

            return path;
        }

        protected string GetFilenameMask(string filepath) {
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var extension = Path.GetExtension(filepath);
            return string.Format("{0}*{1}", filename, extension);
        }


        /// <summary>
        ///     Calls a method on each registered <see cref="CompiledAssemblies" /> entry
        /// </summary>
        /// <param name="methodName">The searched Method/Function name</param>
        /// <param name="arguments">an <see cref="object" /> Array which represents the Arguments</param>
        /// <param name="silent">no Error Output?</param>
        public bool CallMethod(string methodName, object[] arguments = null, bool silent = true) {
            var invokeList = new List<MethodInfo>();
            var namespaceName = Namespace;

            // Any assembly to call from?
            if (CompiledAssemblies == null || CompiledAssemblies.Count < 1) {
                return false;
            }

            // Search for the exact name
            // @TODO: We need some sort of cache, maybe dictionary to call directly without searching
            foreach (var assembly in CompiledAssemblies) {
                if (assembly == null) {
                    continue;
                }

                // Catch only types in my namespace?
                var types = assembly.GetTypes().Where(type => string.IsNullOrEmpty(type.Namespace) == false);
                if (string.IsNullOrEmpty(namespaceName) == false) {
                    // ReSharper disable once PossibleNullReferenceException
                    types = types.Where(type => type.Namespace.StartsWith(namespaceName));
                }

                foreach (var type in types) {
                    try {
                        // Search a puplic and static method
                        var info = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
                        if (info != null) {
                            invokeList.Add(info);
                        }
                    } catch {
                    }
                }
            }

            // Found something?
            if (invokeList.Count == 0) {
                if (silent == false) {
                    var exMessage = string.Format("CallMethod \"{0}.{1}\" failed! Method was not found in {2} Assemblies!", namespaceName, methodName,
                        CompiledAssemblies.Count);
                    throw new NoAssemblyFoundException(exMessage);
                }

                return false;
            }

            // Sort by call priority (custom attribute)
            invokeList.Sort(new CallPriorityComparer());

            foreach (var methodInfo in invokeList) {
                var args = methodInfo.GetParameters();
                var inv = new MethodInvokeHelper(methodInfo);
                // Dynamic fill all arguments
                // Note: If more arguments needed then given, null will be used
                if (args.Length > 0) {
                    inv.MethodArguments = FillArgList(args, arguments);
                }

                // Start a thread for every execution
                // @TODO: We need some sort of logic here!
                //		  If we execute this for 100 player at a time, 100 threads are running - might by slow and unstable..
                var thread = new Thread(inv.Invoke) {
                    Name = inv.MethodInfo.Name
                };
                thread.Start();
            }

            // Garbage 
            invokeList.Clear();

            return true;
        }

        /// <summary>
        ///     Fills an object[] Array with Agruments, taken from <see cref="args" />
        ///     <para>Values will be taken from <see cref="userArguments" /></para>
        ///     <para>If <see cref="userArguments" /> has not enough Elements, the Value will be null</para>
        /// </summary>
        /// <param name="args"></param>
        /// <param name="userArguments"></param>
        /// <returns>The filled Argument List</returns>
        private static object[] FillArgList(ICollection<ParameterInfo> args, IList<object> userArguments) {
            // @TODO: this might crash a method using multiple parameters..
            if (args.Count == 0) {
                return null;
            }

            var methodArgs = new object[args.Count];
            if (userArguments == null || userArguments.Count == 0) {
                return methodArgs;
            }

            for (var a = 0; a < methodArgs.Length; a++) {
                // Fill missing arguments with null
                // @TODO: maybe check for Nullable types?
                if (a < userArguments.Count && userArguments[a] != null) {
                    methodArgs[a] = userArguments[a];
                } else {
                    methodArgs[a] = null;
                }
            }

            return methodArgs;
        }

    }

}