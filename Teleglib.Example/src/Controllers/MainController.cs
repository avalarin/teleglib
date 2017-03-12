using System;
using Teleglib.Controllers.Attributes;
using Teleglib.Controllers.Results;
using Teleglib.Renderers;
using Teleglib.Telegram.Models;

namespace Teleglib.Example.Controllers {
    [Route("/get")]
    public class MainController {

        [Route("name", Details = "Вывод полного имени")]
        public IActionResult Index(ChatInfo chat) {
            return new ResponseResult($"Имя: {chat.FirstName} {chat.LastName}");
        }

        [Route("nick", Details = "Вывод никнейма")]
        public IActionResult Subgroup(ChatInfo chat) {
           return new ResponseResult($"Никнейм: {chat.UserName}");
        }

        [Route("time", Details = "Вывод времени")]
        public IActionResult GetTime() {
            return new ResponseResult(MessageData.Builder()
                        .SetText($"Сейчас: {DateTime.Now.TimeOfDay}")
                        .SetInlineKeyboardMarkup(b => b.AddRow(r => r.AddItem(new InlineKeyboardButton("Обновить") { CallbackData = "/time" })))
                        .Build());
        }
     }
}