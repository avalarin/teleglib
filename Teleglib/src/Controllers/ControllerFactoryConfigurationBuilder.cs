using System.Collections.Generic;
using System.Reflection;

namespace Teleglib.Controllers {
    public class ControllerFactoryConfigurationBuilder {

        private readonly List<string> _namespaces = new List<string>();
        private Assembly _assembly;
        private bool _generateRoutes;

        public ControllerFactoryConfigurationBuilder AddNamespace(string ns) {
            _namespaces.Add(ns);
            return this;
        }

        public ControllerFactoryConfigurationBuilder SetAssembly(Assembly assembly) {
            _assembly = assembly;
            return this;
        }

        public ControllerFactoryConfigurationBuilder GenerateRoutes() {
            _generateRoutes = true;
            return this;
        }

        public IControllerFactoryConfiguration Build() {
            return new Configuration(_namespaces.ToArray(), _assembly, _generateRoutes);
        }

        private class Configuration : IControllerFactoryConfiguration {
            public Assembly Assembly { get; }
            public string[] Namespaces { get; }
            public bool GenerateRoutes { get; }

            public Configuration(string[] namespaces, Assembly assembly, bool generateRoutes) {
                Namespaces = namespaces;
                Assembly = assembly;
                GenerateRoutes = generateRoutes;
            }
        }

    }
}