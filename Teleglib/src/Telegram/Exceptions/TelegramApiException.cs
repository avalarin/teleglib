using System;

namespace Teleglib.Telegram.Exceptions {
    public class TelegramApiException : Exception {

        public int ErrorCode { get; }

        public TelegramApiException(string message, int errorCode)
            : base(message) {
            ErrorCode = errorCode;
        }

        public TelegramApiException(string message, int errorCode, Exception inner)
            : base(message, inner) {
            ErrorCode = errorCode;
        }
    }
}