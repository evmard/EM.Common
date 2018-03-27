using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public abstract class Singletone<T>
        where T : new()
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                            _instance = new T();
                    }
                }

                return _instance;
            }
        }
        private static object locker = new object();
        private static T _instance;
    }
}
