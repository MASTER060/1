using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using RemoteFork.Log;
using RemoteFork.Settings;

namespace RemoteFork.Plugins {
    internal class PluginManager {
        private static readonly Logger Log = new Logger(typeof(PluginManager));

        public static readonly PluginManager Instance = new PluginManager();

        private readonly Dictionary<string, PluginInstance> _plugins = new Dictionary<string, PluginInstance>();

        private PluginManager() {
            LoadPlugins();
        }

        private void LoadPlugins() {
            string pathPlugins = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            if (Directory.Exists(pathPlugins)) {
                LoadAssemblies(pathPlugins);
                LoadScripts(pathPlugins);
            }
        }

        private void LoadScripts(string pathPlugins) {
            var dir = new DirectoryInfo(pathPlugins);

            foreach (var file in dir.GetFiles("*.cs")) {
                try {
                    LoadScript(file.FullName, GetChecksum(file.FullName));
                } catch (Exception exception) {
                    Log.LogError("LoadPlugins->{0}: {1}", file, exception.Message);
                }
            }
        }

        private static List<MetadataReference> CollectReferences() {
            // first, collect all assemblies
            var assemblies = new HashSet<Assembly>();

            Collect(Assembly.Load(new AssemblyName("netstandard")));
            Collect(Assembly.Load(new AssemblyName("RemoteForkCP")));

            //// add extra assemblies which are not part of netstandard.dll, for example:
            //Collect(typeof(Uri).Assembly);

            // second, build metadata references for these assemblies
            var result = new List<MetadataReference>(assemblies.Count);
            foreach (var assembly in assemblies) {
                result.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
            result.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            return result;

            // helper local function - add assembly and its referenced assemblies
            void Collect(Assembly assembly) {
                if (!assemblies.Add(assembly)) {
                    // already added
                    return;
                }

                var referencedAssemblyNames = assembly.GetReferencedAssemblies();

                foreach (var assemblyName in referencedAssemblyNames) {
                    var loadedAssembly = Assembly.Load(assemblyName);
                    assemblies.Add(loadedAssembly);
                }
            }

        }

        private void LoadScript(string fileName, string hash) {
            string codeToCompile = File.ReadAllText(fileName);
            
            var syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
            string assemblyName = Path.GetRandomFileName();
            var references = CollectReferences();
            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] {syntaxTree},
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var ms = new MemoryStream()) {
                EmitResult emitResult = compilation.Emit(ms);
                if (!emitResult.Success) {
                    // some errors
                    var failures = emitResult.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);
                    foreach (var failure in failures) {
                        Log.LogError(failure.GetMessage());
                    }
                } else {
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);

                    LoadAssembly(assembly, hash);
                }
            }
        }

        private void LoadAssemblies(string pathPlugins) {
            var dir = new DirectoryInfo(pathPlugins);

            foreach (var file in dir.GetFiles("*.dll")) {
                try {
                    LoadAssembly(Assembly.LoadFrom(file.FullName), GetChecksum(file.FullName));
                } catch (Exception exception) {
                    Log.LogError("LoadPlugins->{0}: {1}", file, exception.Message);
                }
            }
        }

        private void LoadAssembly(Assembly assembly, string hash) {
            foreach (var type in assembly.GetExportedTypes()) {
                if (typeof(IPlugin).IsAssignableFrom(type) && (type.IsAbstract == false)) {
                    var attribute = type.GetCustomAttribute<PluginAttribute>();

                    if (attribute != null) {
                        var plugin = new PluginInstance(hash, assembly, type, attribute);
                        if (!_plugins.ContainsKey(plugin.Id)) {
                            _plugins.Add(plugin.Id, plugin);

                            Log.LogDebug("Loaded plugin [id: {0}, name: {1}, type: {2}, version: {3}]", plugin.Id,
                                plugin.Name, type.FullName, plugin.Version);
                        }
                    }
                }
            }
        }

        private static string GetChecksum(string file) {
            using (var stream = File.OpenRead(file)) {
                var checksum = SHA256.Create().ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", string.Empty);
            }
        }

        public void ReimportPlugins() {
            _plugins.Clear();
            LoadPlugins();
        }

        public void RemovePlugin(string name) {
            if (_plugins.ContainsKey(name))
                _plugins.Remove(name);
        }

        public Dictionary<string, PluginInstance> GetPlugins(bool filtering = true) {
            if (filtering) {
                var dict = new Dictionary<string, PluginInstance>();

                if (ProgramSettings.Settings.Plugins) {
                    foreach (var plugin in _plugins) {
#if !DEBUG
                        if (ProgramSettings.Settings.DeveloperMode || ProgramSettings.Settings.EnablePlugins.Contains(plugin.Value.Key))
#endif
                        {
                            dict.Add(plugin.Key, plugin.Value);
                        }
                    }
                }

                return dict;
            }
            return _plugins;
        }

        public PluginInstance GetPlugin(string id) {
            if (_plugins.ContainsKey(id)) {
                if (ProgramSettings.Settings.Plugins) {
#if !DEBUG
                    if (ProgramSettings.Settings.DeveloperMode ||ProgramSettings.Settings.EnablePlugins.Contains(_plugins[id].Key))
#endif
                    {
                        return _plugins[id];
                    }
                }
            }
            return null;
        }
    }
}