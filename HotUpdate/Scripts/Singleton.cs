using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

  public  class Singleton<T>where T: new()
    {
        protected Singleton() { }
        private static T instance;
        private static Object obj = new Object();
        public static T GetSingleton()
        {
            if(instance == null)
            {
                lock (obj)
                {
                    if (instance == null) instance = new T();
                }
            }
            return instance;
        }
    }
