using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LogoScriptIDE
{
    class Config
    {
        private Dictionary<string, string> dict
            = new Dictionary<string, string>();

        public void Save(TextWriter tw)
        {
            foreach (var pair in dict)
                tw.WriteLine(pair.Key + " = " + pair.Value);
        }

        public void Load(TextReader tr)
        {
            dict.Clear();
            while (true)
            {
                string s = tr.ReadLine();
                if (s == null) break;

                int i = s.IndexOf('=');
                if (i == -1) continue;

                string name = s.Substring(0, i).Trim();
                string data = s.Substring(i + 1).Trim();
                dict.Add(name, data);
            }
        }

        public void SetField(string name, string data)
        {
            if (dict.ContainsKey(name))
                dict[name] = data;
            else
                dict.Add(name, data);
        }

        public string GetField(string name)
        {
            if (dict.ContainsKey(name))
                return dict[name];
            else
                return "";
        }

        public double TryGetDouble(string name, double defaultValue)
        {
            try
            {
                string data = GetField(name);
                double answer = Double.Parse(data);
                return answer;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
