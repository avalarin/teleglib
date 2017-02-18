using System;
using Teleglib.Telegram.Models;

namespace Teleglib.Polling {
    public class UpdateEventArgs : EventArgs {
        public UpdateInfo Update { get; }

        public UpdateEventArgs(UpdateInfo update) {
            Update = update;
        }
    }
}