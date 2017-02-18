using Teleglib.Controllers.Actions;
using Teleglib.Controllers.Attributes;
using Teleglib.Middlewares;

namespace Teleglib.Example.Controllers {
    [Route("/main")]
    public class MainController {
        [Route("/index")]
        public IActionResult Index(MiddlewareData data) {
            return new ResponseResult() { Text = "Группа!"};
        }
        [Route("b")]
        public IActionResult Subgroup(MiddlewareData data) {
            return new ResponseResult() { Text = "Подгруппа!"};
        }
    }
}