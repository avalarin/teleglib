namespace Teleglib.Router {
    public class RoutingData {

        public string Path { get; }

        public string[] PathParts { get; }

        public string Content { get; }

        public RoutingData(string path, string[] pathParts, string content) {
            Path = path;
            PathParts = pathParts;
            Content = content;
        }
    }
}