namespace Teleglib.Router {
    public class RouteCompletionData {
        public string HintText { get; }
        
        public string Path { get; }

        public RouteCompletionData(string hintText, string path) {
            HintText = hintText;
            Path = path;
        }
    }
}