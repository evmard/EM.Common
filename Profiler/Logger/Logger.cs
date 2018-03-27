using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiling
{
    public static class Logger
    {
        private static IMultiLogger _instance;
        private static object locker = new object();
        public static IMultiLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock(locker)
                    {
                        if (_instance == null)
                        {
                            ILogDal logDal = new LogDal(new LogContextFactory());
                            _instance = new MultiLogger(logDal);
                        }
                    }
                }
                return _instance;
            }
        }

        public static void Dispose()
        {
            lock (locker)
            {
                if (_instance != null)
                    _instance.Dispose();

                _instance = null;
            }
        }
    }
}
