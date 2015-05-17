using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kursach
{
    public class _Class
    {
        public string _nameClass { set; get; }
        List<C_Variables> _variables = new List<C_Variables>();
        List<C_Methods> _methods = new List<C_Methods>();

        public _Class()
        {
            _nameClass = "";
        }
        public _Class(string _name)
        {
            _nameClass = _name;
        }
        public void AddVariable(C_Variables _var)
        {
            _variables.Add(_var);
        }
        public void AddMethod(C_Methods _meth)
        {
            _methods.Add(_meth);
        }
        public List<C_Variables> Variables
        {
            get { return _variables; }
        }
        public List<C_Methods> Methods
        {
            get { return _methods; }
        }
    }
    public class CodeGeneration_module
    {
        private bool SwapType(ref string _type, bool _cppFile)
        {
            string[] masTypeUser =
            {
                    "void",
                    "int","int[]","int[][]",
                    "float","float[]","float[][]",
                    "double","double[]","double[][]",
                    "char","char[]","char[][]",
                    "string","string[]","string[][]",
                    "bool","bool[]","bool[][]"
            };
            string[] masType = 
            {
                    "void",
                    "int","int*","int*",
                    "float","float*","float**",
                    "double","double*","double**",
                    "char","char*","char**",
                    "string","string*","string**",
                    "bool","bool*","bool**"
            };
            if (_cppFile)
            {
                for (int i = 0; i < masType.Length; i++)
                {
                    if (masType[i] == _type)
                    {
                        _type = masTypeUser[i];
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < masTypeUser.Length; i++)
                {
                    if (masTypeUser[i] == _type)
                    {
                        _type = masType[i];
                        return true;
                    }
                }
            }
            return false;
        }
        public  List<string> CodeH(string _NameClass,List<C_Variables> variables,List<C_Methods> methods)
        {
            List<string> temp = new List<string>();
            temp.Add("#include <iostream.h>");
            temp.Add("using namespace std;");
            temp.Add("class " + _NameClass + "\n{");
            string tempType = string.Empty;
            foreach (C_Variables arg in variables)
            {
                tempType = arg.Type;
                SwapType(ref tempType, false);
                arg.Type = tempType;

                temp.Add("\t" + arg.Type + " " + arg.Name);
            }
            temp.Add("public:");
            temp.Add(_NameClass + "();");
            temp.Add("~" + _NameClass + "();");
            foreach (C_Methods arg in methods)
            {
                string tempString = string.Empty;

                tempType = arg.Type;
                SwapType(ref tempType, false);
                arg.Type = tempType;

                tempString += "\t" + arg.Type + " " + arg.Name + "(";
                foreach (C_Variables var in arg.Variables)
                {
                    tempType = var.Type;
                    SwapType(ref tempType, false);
                    var.Type = tempType;

                    tempString += var.Type + " " + var.Name + ", ";
                }
                if (tempString[tempString.Length - 2] ==',')
                {
                    tempString = tempString.Remove(tempString.Length - 2);
                }
                tempString += ");";
                temp.Add(tempString);
            }
            temp.Add("}");
            return temp;
        }
        public List<string> CodeCpp(string _NameClass,List<C_Methods> methods)
        {
            List<string> temp = new List<string>();
            temp.Add("#include \"" + _NameClass + ".h\"");
            foreach (C_Methods arg in methods)
            {
                string tempString = string.Empty;

                string tempType = arg.Type;
                SwapType(ref tempType, false);
                arg.Type = tempType;

                tempString = arg.Type + " " + _NameClass + "::" + arg.Name + '(';
                foreach (C_Variables var in arg.Variables)
                {
                    tempType = var.Type;
                    SwapType(ref tempType, false);
                    var.Type = tempType;
                    tempString += var.Type + " " + var.Name + ", ";
                }
                if (tempString[tempString.Length - 2] == ',')
                {
                    tempString = tempString.Remove(tempString.Length - 2);
                }
                tempString += ")\n{}";
            }
            return temp;
        }

        public void CodeToFile(string _NameClass, List<C_Variables> variables, List<C_Methods> methods)
        {
            using (StreamWriter file = new System.IO.StreamWriter(_NameClass + ".h"))
            {
                foreach (string arg in CodeH(_NameClass,variables,methods))
                {
                    file.WriteLine(arg);
                }
                file.Close();
            }
            using (StreamWriter file = new System.IO.StreamWriter(_NameClass + ".cpp"))
            {
                foreach (string arg in CodeCpp(_NameClass, methods))
                {
                    file.WriteLine(arg);
                }
                file.Close();
            }
        }

        //----------------------------------------------------------------------------------------------------
        private void AddMethod(string[] fileLines, ref int index, _Class temp)
        {
            C_Methods tempM = new C_Methods();
            tempM.Type = fileLines[index];
            tempM.Name = fileLines[index + 1];
            index += 2;
            while (fileLines[index] != ")")
            {
                if (SwapType(ref fileLines[index],true))
                {
                    tempM.AddVariable(new C_Variables(fileLines[index], fileLines[index + 1]));
                    index += 2;
                }
            }
            temp.AddMethod(tempM);
            Stack<char> counter = new Stack<char>();
            index++;
            counter.Push(char.Parse(fileLines[index]));
            while (counter.Count!=0)
            {
                index++;
                if (fileLines[index] == "{")
                {
                    counter.Push(char.Parse(fileLines[index]));
                }
                if (fileLines[index] == "}")
                {
                    counter.Pop();
                }
            }
            index++;
        }

        public string CodeFromFile(string _pathToFile, List<_Class> listClass)
        {
            string name = string.Empty;
            using(StreamReader sr = new StreamReader(_pathToFile))
            {
                String allLines = sr.ReadToEnd();
                string[] fileLines = allLines.Split(new char[] { ' ','\n','\t','\r' }, StringSplitOptions.RemoveEmptyEntries);
                
                _Class temp = new _Class();
                int index = -1;

                for (int i = 0; i < fileLines.Length; i++)
                {
                    if (fileLines[i] == "class")
                    {
                        if (temp._nameClass!="")
                        {
                            listClass.Add(temp);
                        }
                        temp._nameClass = fileLines[i + 1];
                        index = i + 1;
                        break;
                    }
                }
                if (fileLines[index + 1] == ":")
                {
                    index += 4;
                }
                while (true)
                {
                    int checkIndex = index;
                    //Проверяем переменную
                    if (SwapType(ref fileLines[index], true) && (fileLines[index + 2] == ";" || fileLines[index + 2] == "="))
                    {
                        temp.AddVariable(new C_Variables(fileLines[index], fileLines[index + 1]));
                        index += 2;
                    }
                    //Проверяем метод
                    if (SwapType(ref fileLines[index], true) && fileLines[index + 2] == "(")
                    {
                        AddMethod(fileLines, ref index, temp);
                    }
                    //Виртуальный метод
                    if (fileLines[index] == "virtual" && SwapType(ref fileLines[index + 1], true) && fileLines[index + 3] == "(")
                    {
                        index++;
                        AddMethod(fileLines, ref index, temp);
                    }

                    //Условие перехода, если не распознано элементов
                    if (checkIndex == index)
                    {
                        index++;
                        if (index == fileLines.Length - 1)
                        {
                            listClass.Add(temp);
                            break;
                        }
                    }
                }
            }
            return name;
        }
    }
}
