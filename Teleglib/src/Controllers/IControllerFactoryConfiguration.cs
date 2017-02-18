using System.Reflection;

namespace Teleglib.Controllers {
    public interface IControllerFactoryConfiguration {
        Assembly Assembly { get; }

        string[] Namespaces { get; }

        bool GenerateRoutes { get; }
    }
}