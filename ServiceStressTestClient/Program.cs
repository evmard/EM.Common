using MicroServiceBase.Contract;
using Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStressTestClient
{
    class Program
    {
        static void Main(string[] args)
        {            
            var client = TestClient.Instance;
            var functs = new List<Func<string, StringResponce>>();
            functs.Add(client.Test);
            functs.Add(client.Test2);
            functs.Add(client.Test3);
            var ch = ' ';
            var i = 0;
            while (ch != 'q')
            {
                
                try
                {
                    Logger.Instance.Debug($"Send {i}");
                    var funct = functs[i % functs.Count];
                    var res = funct(i.ToString());
                    Logger.Instance.Debug($"Recived {res.Value}");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error(ex);
                }

                ++i;
                var key = Console.ReadKey();
                ch = key.KeyChar;
            }
        }
    }
}
