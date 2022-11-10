using bomberman.classes.adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.server
{
    internal class LogsSingleton
    {
        private static LogsSingleton? instance;
        private List<string>? Logs { get; set; } = new List<string>();

        private LogsSingleton() {}

        public static LogsSingleton GetInstance()
        {
            if (instance == null)
            {
                instance = new LogsSingleton();
            }

            return instance;
        }

        public List<string>? All()
        {
            return Logs;
        }

        public void Add(string log)
        {
            Logs?.Add(log);
        }
    }
}
