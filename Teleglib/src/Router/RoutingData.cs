namespace Teleglib.Router {
    public class RoutingData {

        public string Path { get; }

        public string[] PathParts { get; }

        public string UserCommand { get; }

        public string Content { get; }

        public RoutingData(string userCommand, string path, string[] pathParts, string content) {
            Path = path;
            PathParts = pathParts;
            Content = content;
            UserCommand = userCommand;
        }
    }
}