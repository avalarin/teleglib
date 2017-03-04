using Teleglib.Controllers.Attributes;
using Teleglib.Controllers.Results;
using Teleglib.Telegram.Models;

namespace Teleglib.Example.Controllers {
    [Route("/get")]
    public class MainController {

        [Route("name", Details = "Вывод полного имени")]
        public IActionResult Index(ChatInfo chat) {
            return new ResponseResult() { Text = $"Имя: {chat.FirstName} {chat.LastName}"};
        }

        [Route("nick", Details = "Вывод никнейма")]
        public IActionResult Subgroup(ChatInfo chat) {
           return new ResponseResult() { Text = $"Никнейм: {chat.UserName}"};
         }
     }
}