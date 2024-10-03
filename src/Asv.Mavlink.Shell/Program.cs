    using System;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Asv.IO;
    using ConsoleAppFramework;

    namespace Asv.Mavlink.Shell;

    class Program
    { 
        static async Task Main(string[] args)
        {
            Assembly.GetExecutingAssembly().PrintWelcomeToConsole();
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.Black;
          
            var app = ConsoleApp.Create();
            app.Add<ExampleCommand>();
            app.Add<FtpTreeDirectory>();
            app.Add<FtpBrowserDirectory>();
            app.Add<DevicesInfoCommand>();
            app.Add<GenerateCommand>();
            await app.RunAsync(args);
        }
    }

