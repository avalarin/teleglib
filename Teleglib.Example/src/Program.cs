using System;
using Microsoft.Extensions.DependencyInjection;

namespace Teleglib.Example {
    internal class Program {
        public static void Main(string[] args) {
            var services = new ServiceCollection();
            var app = new Application(services);
            app.Start();
            Console.ReadLine();
        }
    }
}