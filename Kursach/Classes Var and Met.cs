using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursach
{
    public class C_Variables
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public C_Variables()
        {}

        public C_Variables(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
    public class C_Methods
    {
        List<C_Variables> _variables = new List<C_Variables>();
        public string Type { set; get; }
        public string Name { set; get; }
        public bool Virtual { get; set; }

        public List<C_Variables> Variables
        {
            get { return _variables; }
        }

        public C_Methods()
        {
        }

        public C_Methods(string type, string name)
        {
            Name = name;
            Type = type;
        }

        public void AddVariable(C_Variables var)
        {
            _variables.Add(var);
        }

        public void ModifyVar(int number,string newType, string newName)
        {
            _variables[number].Type = newType;
            _variables[number].Name = newName;
        }

        public void DeleteVariable(C_Variables var)
        {
            for (int i = 0; i < _variables.Count; i++)
            {
                if (_variables[i].Name == var.Name)
                {
                    _variables.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
