using Teleglib.Controllers.Attributes;
using Teleglib.Controllers.Results;
using Teleglib.Features;
using Teleglib.Middlewares;

namespace Teleglib.Example.Controllers {
    [Route("/get")]
    public class MainController {

        [Route("name", Details = "Вывод полного имени")]
        public IActionResult Index(MiddlewareData data) {
            var user = data.Features.RequireOne<UpdateInfoFeature>().Update.Message.Chat;
            return new ResponseResult() { Text = $"Имя: {user.FirstName} {user.LastName}"};
        }

        [Route("nick", Details = "Вывод никнейма")]
        public IActionResult Subgroup(MiddlewareData data) {
            var user = data.Features.RequireOne<UpdateInfoFeature>().Update.Message.Chat;
            return new ResponseResult() { Text = $"Никнейм: {user.UserName}"};
         }

     }
 }