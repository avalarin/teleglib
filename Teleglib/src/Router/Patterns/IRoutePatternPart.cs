namespace Teleglib.Router.Patterns {
    public interface IRoutePatternPart {
        bool IsMatch(string part);
    }
}