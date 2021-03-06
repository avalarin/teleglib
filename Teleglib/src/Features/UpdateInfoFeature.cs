﻿using System.Collections.Generic;
using Teleglib.Parameters;
using Teleglib.Telegram.Models;

namespace Teleglib.Features {
    public class UpdateInfoFeature : IFeature, IParameterValuesSource {
        public UpdateInfo Update { get; }

        public UpdateInfoFeature(UpdateInfo update) {
            Update = update;
        }

        public MessageInfo GetAnyMessage() {
            return Update.Message ?? Update.EditedMessage ?? Update.ChannelPost ?? Update.EditedChannelPost ?? Update.CallbackQuery?.Message;
        }

        public IEnumerable<ParameterValue> GetValues() {
            var message = GetAnyMessage();
            return new List<ParameterValue>() {
                new ParameterValue("update", Update),
                new ParameterValue("message", message),
                new ParameterValue("chat", message.Chat)
            };
        }
    }
}