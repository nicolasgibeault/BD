using System;
using System.Diagnostics;
using PFE.Framework.Transport;

namespace PFE.Server.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
            Serilog.Debugging.SelfLog.Enable(Console.Error);

            PFE.Framework.Transport.Server.GetServer.StartServer(707);
        }
    }
}
