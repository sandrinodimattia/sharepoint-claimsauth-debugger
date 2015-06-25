using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FedAuthDebugger.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n FedAuth Debugger\n\n");

            using (var session = new TraceEventSession("FedAuthDebuggerApp"))
            {
                session.Source.Dynamic.All += delegate(TraceEvent data)
                {
                    if (!String.IsNullOrEmpty(data.FormattedMessage))
                    {
                        Console.WriteLine(" {0} - {1}", data.TimeStamp.ToString("HH:mm:ss"), data.FormattedMessage);
                    }
                };

                session.EnableProvider(
                    TraceEventProviders.GetEventSourceGuidFromName("FedAuthDebugger-Session"));
                session.EnableProvider(
                    TraceEventProviders.GetEventSourceGuidFromName("FedAuthDebugger-Federation"));
                session.Source.Process();
            }

            Console.ReadLine();
        }
    }
}
