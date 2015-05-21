using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kursach
{
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
        public List<string> CodeH(string _NameClass, List<C_Variables> variables, List<C_Methods> methods)
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
                if (tempString[tempString.Length - 2] == ',')
                {
                    tempString = tempString.Remove(tempString.Length - 2);
                }
                tempString += ");";
                temp.Add(tempString);
            }
            temp.Add("}");
            return temp;
        }
        public List<string> CodeCpp(string _NameClass, List<C_Methods> methods)
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
                temp.Add(tempString);
            }
            return temp;
        }

        public void CodeToFile(string _NameClass, List<C_Variables> variables, List<C_Methods> methods, string path)
        {
            using (StreamWriter file = new System.IO.StreamWriter(path + "\\" + _NameClass + ".h"))
            {
                foreach (string arg in CodeH(_NameClass, variables, methods))
                {
                    file.WriteLine(arg);
                }
                file.Close();
            }
            using (StreamWriter file = new System.IO.StreamWriter(path + "\\" + _NameClass + ".cpp"))
            {
                foreach (string arg in CodeCpp(_NameClass, methods))
                {
                    file.WriteLine(arg);
                }
                file.Close();
            }
        }

        public List<C_Variables> AddVariable(string _type, string[] _names)
        {
            //Функция принимает параметры: type - тип, names - "имя1","имя2" и т.д.
            List<C_Variables> _lcv = new List<C_Variables>();
            foreach (string _var in _names)
            {
                _lcv.Add(new C_Variables(_type, _var));
            }
            return _lcv;
        }

        private C_Methods AddMethod(List<string> mas)
        {
            C_Methods temp = new C_Methods();
            string tempValues = string.Empty;
            for (int i = 0; i < mas.Count; i++)
            {
                if (mas[i] == "=")
                {
                    break;
                }
                if (mas[i] == "virtual")
                {
                    temp.Virtual = true;
                    continue;
                }
                if (mas[i] == "friend")
                {
                    continue;
                }
                if (mas[i].Length > 2 && mas[i][mas[i].Length - 2] == '(' && mas[i][mas[i].Length - 1] == ')')
                {
                    string tempType = mas[i - 1];
                    SwapType(ref tempType, true);
                    temp.Type = tempType;
                    temp.Name = mas[i].Remove(mas[i].Length - 2);
                    return temp;
                }
                if (mas[i].Length > 3 && mas[i][mas[i].Length - 3] == '(' && mas[i][mas[i].Length - 2] == ')')
                {
                    string tempType = mas[i - 1];
                    SwapType(ref tempType, true);
                    temp.Type = tempType;
                    temp.Name = mas[i].Remove(mas[i].Length - 3);
                    return temp;
                }
                //Обрезаем скобочки, если есть
                if (mas[i][0] == '(')
                {
                    mas[i] = mas[i].Remove(0, 1);
                }
                if (mas[i][mas[i].Length - 1] == '(')
                {
                    mas[i] = mas[i].Remove(mas[i].LastIndexOf('('));
                }
                //Обрезаем концовки, если есть
                if (mas[i][mas[i].Length - 1] == ',')
                {
                    mas[i] = mas[i].Remove(mas[i].Length - 1);
                }
                if (mas[i][mas[i].Length - 1] == ';')
                {
                    mas[i] = mas[i].Remove(mas[i].Length - 2);
                }
                if (mas[i][mas[i].Length - 1] == ')')
                {
                    mas[i] = mas[i].Remove(mas[i].Length - 1);
                }
                //Перегоняем новые значения
                if (mas[i] != "")
                {
                    tempValues += mas[i] + " ";
                }
            }
            string[] _masValues = tempValues.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //Находим тип и имя метода
            int indexVirtual = 0;
            if (temp.Virtual)
            {
                indexVirtual++;
            }
            string type_method = _masValues[indexVirtual];
            indexVirtual++;
            SwapType(ref type_method, true);
            string name_method = string.Empty;
            bool find = false;
            int index = indexVirtual + 1;

            if (_masValues[indexVirtual].LastIndexOf('(') == -1)
            {
                name_method = _masValues[indexVirtual];
            }
            else
            {
                name_method = _masValues[indexVirtual].Remove(_masValues[indexVirtual].LastIndexOf('('));
                find = true;
            }
            temp.Type = type_method;
            temp.Name = name_method;

            string type_var = string.Empty;
            string name_var = string.Empty;
            if (find)
            {
                type_var = _masValues[indexVirtual].Remove(0, _masValues[indexVirtual].LastIndexOf('(') + 1);
                name_var = _masValues[indexVirtual + 1];
                temp.AddVariable(new C_Variables(type_var, name_var));
                index = indexVirtual += 2;
            }
            for (int i = index; i < _masValues.Length; i += 2)
            {
                temp.AddVariable(new C_Variables(_masValues[i], _masValues[i + 1]));
            }
            return temp;
        }

        public ClassBox ReadFile(string _path)
        {
            ClassBox temp = new ClassBox();
            using (StreamReader sr = new StreamReader(_path))
            {
                string[] lines = sr.ReadToEnd().Split(new char[] { '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                bool findclass = false;
                foreach (string line in lines)
                {
                    List<string> _valueInLine = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    //Находим название класса
                    if (_valueInLine[0] == "class")
                    {
                        temp.Name = _valueInLine[1];
                        findclass = true;
                        continue;
                    }
                    //После того как получили название класса - извлекаем переменные и методы
                    if (findclass)
                    {
                        //Проверяем расплитованную строку - переменная или метод? 
                        //У метода мы найдем пару '(' ')'
                        bool variable = true;
                        foreach (string _value in _valueInLine)
                        {
                            if (_value.LastIndexOf('(') != -1)
                            {
                                variable = false;
                                break;
                            }
                        }
                        //Если переменная
                        if (variable && _valueInLine.Count >= 2)
                        {
                            //Получаем тип переменной (-ых)
                            string _type = _valueInLine[0];
                            SwapType(ref _type, true);
                            //Удаляем его из текущей строки
                            _valueInLine.RemoveAt(0);
                            //Получаем список переменных, даже если она одна

                            string tempValue = string.Empty;
                            foreach (string val in _valueInLine)
                            {
                                tempValue += val + " ";
                            }
                            string[] _masName = tempValue.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            //Вырезаем последний символ ';' или ',', если такой имеется
                            for (int i = 0; i < _masName.Length; i++)
                            {
                                if (_masName[i].LastIndexOf(',') != -1 || _masName[i].LastIndexOf(';') != -1)
                                {
                                    _masName[i] = _masName[i].Remove(_masName[i].Length - 1);
                                }
                            }
                            //Добавляем переменные
                            temp.AddListVariables(AddVariable(_type, _masName));
                        }
                        //Если метод
                        if (!variable && _valueInLine.Count >= 2 && _valueInLine[1][0] != '~')
                        {
                            C_Methods cm = new C_Methods();
                            if (!AddMethod(_valueInLine).Virtual)
                            {
                                temp.Methods.Add(AddMethod(_valueInLine));
                            }
                            else
                            {
                                temp.MethodsVirtual.Add(AddMethod(_valueInLine));
                            }
                        }
                    }
                    else
                    //Пока не нашли название класса
                    {
                        continue;
                    }
                }
                sr.Close();
            }
            return temp;
        }
    }
}
