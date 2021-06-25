using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpreter
{
    class string_concat
        : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            StringBuilder sb = new StringBuilder();
            Varible v;
            for (int i = 1; i <= numParams; ++i)
            {
                v = m.GetLocalByID(i);
                switch (v.type)
                {
                    case VarType.TYPE_NUMBER:
                        sb.Append(v.num);
                        break;
                    case VarType.TYPE_STRING:
                        sb.Append(v.str);
                        break;
                    default:
                        throw new RuntimeException("Invaild argument type");
                }
            }
            return new Varible(sb.ToString());
        }
    }

    class string_compare
        : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_STRING
                || m.GetLocalByID(2).type != VarType.TYPE_STRING)
                throw new RuntimeException("Invaild arguments");
            if (m.GetLocalByID(1).str == m.GetLocalByID(2).str)
                return new Varible(1.0);
            else
                return new Varible(0.0);
        }
    }

    class string_contain
        : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_STRING
                || m.GetLocalByID(2).type != VarType.TYPE_STRING)
                throw new RuntimeException("Invaild arguments");
            if (m.GetLocalByID(1).str.Contains(m.GetLocalByID(2).str))
                return new Varible(1.0);
            else
                return new Varible(0.0);
        }
    }

    class string_match
        : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_STRING
                || m.GetLocalByID(2).type != VarType.TYPE_STRING)
                throw new RuntimeException("Invaild arguments");
            // TODO : Match returns number of parrerns matched
            // All capture values are stored at a table optional at param 3
                return new Varible(0.0);
        }
    }

    class string_sub
        : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_STRING ||
                m.GetLocalByID(2).type != VarType.TYPE_NUMBER )
                throw new RuntimeException("Invaild argument");
            if (numParams > 2 && m.GetLocalByID(3).type == VarType.TYPE_NUMBER)
                return new Varible(m.GetLocalByID(1).str.Substring((int)m.GetLocalByID(2).num, (int)m.GetLocalByID(3).num));
            else
                return new Varible(m.GetLocalByID(1).str.Substring((int)m.GetLocalByID(2).num));
        }
    }


    class math_sin
: IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
                throw new RuntimeException("Invaild arugment");
            return new Varible(System.Math.Sin(m.GetLocalByID(1).num / 360 * (2 * System.Math.PI)));
        }
    }
    class math_cos
   : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
                throw new RuntimeException("Invaild arugment");
            return new Varible(System.Math.Cos(m.GetLocalByID(1).num / 360 * (2 * System.Math.PI)));
        }
    }
    class math_abs
   : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
                throw new RuntimeException("Invaild arugment");
            return new Varible(System.Math.Abs(m.GetLocalByID(1).num));
        }
    }
    class math_ceiling
   : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
                throw new RuntimeException("Invaild arugment");
            return new Varible(System.Math.Ceiling(m.GetLocalByID(1).num));
        }
    }
    class math_floor
   : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
                throw new RuntimeException("Invaild arugment");
            return new Varible(System.Math.Floor(m.GetLocalByID(1).num));
        }
    }
    class math_exp
   : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
                throw new RuntimeException("Invaild arugment");
            return new Varible(System.Math.Exp(m.GetLocalByID(1).num));
        }
    }
    class math_pow
   : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER || m.GetLocalByID(2).type != VarType.TYPE_NUMBER)
                throw new RuntimeException("Invaild arugment");
            return new Varible(System.Math.Pow(m.GetLocalByID(1).num, m.GetLocalByID(2).num));
        }
    }
    class math_log
   : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_NUMBER || m.GetLocalByID(2).type != VarType.TYPE_NUMBER)
                throw new RuntimeException("Invaild arugment");
            return new Varible(System.Math.Log(m.GetLocalByID(1).num, m.GetLocalByID(2).num));
        }
    }
    class math_random
: IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            Random random = new Random();
            if (numParams == 0)
            {
                return new Varible((double)random.Next());
            }
            else if (numParams == 1)
            {
                if (m.GetLocalByID(1).type != VarType.TYPE_NUMBER)
                    throw new RuntimeException("Invaild arugment");
                return new Varible((double)random.Next((int)m.GetLocalByID(1).num));
            }
            else if (numParams == 2)
            {
                if (m.GetLocalByID(1).type != VarType.TYPE_NUMBER || m.GetLocalByID(2).type != VarType.TYPE_NUMBER)
                    throw new RuntimeException("Invaild arugment");
                return new Varible((double)random.Next((int)m.GetLocalByID(1).num, (int)m.GetLocalByID(2).num));
            }
            else
                throw new RuntimeException("Invaild arugment");
        }
    }


    class table_copy
        : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 1 || m.GetLocalByID(1).type != VarType.TYPE_TABLE)
                throw new RuntimeException("Invaild argument");
            Varible newTable = new Varible(new Dictionary<Varible,Varible>());
            var table = newTable.table;
            foreach (var kvp in m.GetLocalByID(1).table)
            {
                table.Add(kvp.Key,kvp.Value);
            }
            return newTable;
        }
    }

    class table_insert
        : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 3 || m.GetLocalByID(1).type != VarType.TYPE_TABLE)
                throw new RuntimeException("Invaild argument");
            m.GetLocalByID(1).table[m.GetLocalByID(2)] = m.GetLocalByID(3);
            return new Varible(0.0);
        }
    }

    class table_contain_key
        : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_TABLE)
                throw new RuntimeException("Invaild argument");
            if (m.GetLocalByID(1).table.ContainsKey(m.GetLocalByID(2)))
                return new Varible(1.0);
            else
                return new Varible(0.0);
        }
    }

    class table_contain_value
        : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_TABLE)
                throw new RuntimeException("Invaild argument");
            if (m.GetLocalByID(1).table.ContainsValue(m.GetLocalByID(2)))
                return new Varible(1.0);
            else
                return new Varible(0.0);
        }
    }

    class table_remove
       : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_TABLE)
                throw new RuntimeException("Invaild argument");
            if (m.GetLocalByID(1).table.ContainsKey(m.GetLocalByID(2)))
            {
                m.GetLocalByID(1).table.Remove(m.GetLocalByID(2));
                return new Varible(1.0);
            }
            else
                return new Varible(0.0);
        }
    }

    class table_union
       : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_TABLE
                || m.GetLocalByID(2).type != VarType.TYPE_TABLE)
                throw new RuntimeException("Invaild argument");
            Varible newTable = new Varible(new Dictionary<Varible, Varible>());
            var table = newTable.table;
            foreach (var kvp in m.GetLocalByID(2).table)
            {
                table.Add(kvp.Key, kvp.Value);
            }
            foreach (var kvp in m.GetLocalByID(1).table)
            {
                table[kvp.Key] = kvp.Value;
            }
            return newTable;
        }
    }

    class table_intersect
       : IFunction
    {
        public Varible invoke(GcMachine m, int numParams)
        {
            if (numParams < 2 || m.GetLocalByID(1).type != VarType.TYPE_TABLE
                || m.GetLocalByID(2).type != VarType.TYPE_TABLE)
                throw new RuntimeException("Invaild argument");
            Varible newTable = new Varible(new Dictionary<Varible, Varible>());
            var table = newTable.table;
            var table2 = m.GetLocalByID(2).table;
            foreach (var kvp in m.GetLocalByID(1).table)
            {
                if (table2.ContainsKey(kvp.Key) && VarCompare.GetInstance().Equals(kvp.Value,table2[kvp.Key]))
                    table.Add(kvp.Key, kvp.Value);
            }
            return newTable;
        }
    }

    static class Library
    {
        static public void OpenLibrary(GcMachine m)
        {
            Varible v = new Varible(new Dictionary<Varible, Varible>(VarCompare.GetInstance()));

            v.table[new Varible("concat")] = new Varible(new string_concat());
            v.table[new Varible("compare")] = new Varible(new string_compare());
            v.table[new Varible("contain")] = new Varible(new string_contain());
            v.table[new Varible("match")] = new Varible(new string_match());
            v.table[new Varible("sub")] = new Varible(new string_sub());
            //String library
            m.SetGlobal("String", v);

            v = new Varible(new Dictionary<Varible, Varible>(VarCompare.GetInstance()));
            //Math library
            v.table[new Varible("sin")] = new Varible(new math_sin());
            v.table[new Varible("cos")] = new Varible(new math_cos());
            v.table[new Varible("abs")] = new Varible(new math_abs());
            v.table[new Varible("ceiling")] = new Varible(new math_ceiling());
            v.table[new Varible("floor")] = new Varible(new math_floor());
            v.table[new Varible("exp")] = new Varible(new math_exp());
            v.table[new Varible("pow")] = new Varible(new math_pow());
            v.table[new Varible("log")] = new Varible(new math_log());
            v.table[new Varible("random")] = new Varible(new math_random());
            m.SetGlobal("Math", v);

            v = new Varible(new Dictionary<Varible, Varible>(VarCompare.GetInstance()));
            //Table library
            v.table[new Varible("copy")] = new Varible(new table_copy());
            v.table[new Varible("contain_key")] = new Varible(new table_contain_key());
            v.table[new Varible("contain_value")] = new Varible(new table_contain_value());
            v.table[new Varible("intersect")] = new Varible(new table_intersect());
            v.table[new Varible("union")] = new Varible(new table_union());
            v.table[new Varible("insert")] = new Varible(new table_insert());
            v.table[new Varible("remove")] = new Varible(new table_remove());

            m.SetGlobal("Table", v);
        }

    }
}

