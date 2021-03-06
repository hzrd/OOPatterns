﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kursach
{
    public class CodeGeneration_module
    {
        //Функция меняет тип данных на C# если параметр _cppFile true, или на C++, если параметр false
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
        //Функция возвращает лист строк для записи в h файл
        public List<string> CodeH(ClassBox _cb)
        {
            List<string> temp = new List<string>();
            temp.Add("#include <iostream.h>");
            temp.Add("using namespace std;");
            string tempNameClass = "class " + _cb.Name + " ";
            if (_cb.ParentClasses.Count != 0)
            {
                tempNameClass += ": ";
                foreach (ClassBox cb in _cb.ParentClasses)
                {
                    tempNameClass += "public " + cb.Name + ", ";
                }
                tempNameClass = tempNameClass.Remove(tempNameClass.Length - 2);
            }
            tempNameClass += "\n{";
            temp.Add(tempNameClass);
            string tempType = string.Empty;
            foreach (C_Variables arg in _cb.Variables)
            {
                tempType = arg.Type;
                SwapType(ref tempType, false);

                temp.Add("\t" + tempType + " " + arg.Name + ";");
            }
            //Дописываем агрегацию и композицию
            foreach (ClassBox cb in _cb.AgregatedClasses)
            {
                temp.Add("\t" + cb.Name + " _" + cb.Name + ";");
            }
            foreach (ClassBox cb in _cb.CompositedClasses)
            {
                temp.Add("\t" + cb.Name + " _" + cb.Name + ";");
            }
            temp.Add("public:");
            temp.Add(_cb.Name + "();");
            temp.Add("~" + _cb.Name + "();");
            foreach (C_Methods arg in _cb.Methods)
            {
                string tempString = string.Empty;

                tempType = arg.Type;
                SwapType(ref tempType, false);

                tempString += "\t" + tempType + " " + arg.Name + "(";
                foreach (C_Variables var in arg.Variables)
                {
                    tempType = var.Type;
                    SwapType(ref tempType, false);

                    tempString += tempType + " " + var.Name + ", ";
                }
                if (tempString[tempString.Length - 2] == ',')
                {
                    tempString = tempString.Remove(tempString.Length - 2);
                }
                tempString += ");";
                temp.Add(tempString);
            }
            foreach (C_Methods vcm in _cb.VirtualMethods)
            {
                string tempString = string.Empty;

                tempType = vcm.Type;
                SwapType(ref tempType, false);

                tempString += "\t" + "virtual " + tempType + " " + vcm.Name + "(";
                foreach (C_Variables var in vcm.Variables)
                {
                    tempType = var.Type;
                    SwapType(ref tempType, false);

                    tempString += tempType + " " + var.Name + ", ";
                }
                if (tempString[tempString.Length - 2] == ',')
                {
                    tempString = tempString.Remove(tempString.Length - 2);
                }
                tempString += ") = 0;";
                temp.Add(tempString);
            }
            temp.Add("}");
            return temp;
        }
        //Функция возвращает лист строк для записи в cpp файл
        public List<string> CodeCpp(string _NameClass, List<C_Methods> methods)
        {
            List<string> temp = new List<string>();
            temp.Add("#include \"" + _NameClass + ".h\"");
            foreach (C_Methods arg in methods)
            {
                string tempString = string.Empty;

                string tempType = arg.Type;
                SwapType(ref tempType, false);

                tempString = tempType + " " + _NameClass + "::" + arg.Name + '(';
                foreach (C_Variables var in arg.Variables)
                {
                    tempType = var.Type;
                    SwapType(ref tempType, false);

                    tempString += tempType + " " + var.Name + ", ";
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
        //Функция которая записывает данные в файл, параметр _interface обозначает, нужно ли создавать cpp файл(для интерфейсов он не нужен)
        public void CodeToFile(ClassBox cb, string path, bool _interface)
        {
            using (StreamWriter file = new System.IO.StreamWriter(path + "\\" + cb.Name + ".h"))
            {
                foreach (string arg in CodeH(cb))
                {
                    file.WriteLine(arg);
                }
                file.Close();
            }
            if (!_interface)
            {
                using (StreamWriter file = new System.IO.StreamWriter(path + "\\" + cb.Name + ".cpp"))
                {
                    foreach (string arg in CodeCpp(cb.Name, cb.Methods))
                    {
                        file.WriteLine(arg);
                    }
                    file.Close();
                }
            }
        }
        //Функция добавления переменной(-ых) из строки, где первый параметр тип переменной, второй - имя(имена) переменной(-ых)
        private List<C_Variables> AddVariable(string _type, string[] _names)
        {
            //Функция принимает параметры: type - тип, names - "имя1","имя2" и т.д.
            List<C_Variables> _lcv = new List<C_Variables>();
            foreach (string _var in _names)
            {
                _lcv.Add(new C_Variables(_type, _var));
            }
            return _lcv;
        }
        //Функция добавления метода
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
                //Вырезаем скобочку, если встретилась и добавляем имя, тип переменной к массиву строк
                if (mas[i].LastIndexOf('(') != -1)
                {
                    tempValues += mas[i].Remove(mas[i].LastIndexOf('(')) + " ";
                    tempValues += mas[i].Remove(0, mas[i].LastIndexOf('(') + 1) + " ";
                    continue;
                }
                //Перегоняем новые значения
                if (mas[i] != "")
                {
                    tempValues += mas[i] + " ";
                }
            }
            string[] _masValues = tempValues.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //Находим тип и имя метода
            string type_method = _masValues[0];
            SwapType(ref type_method, true);
            temp.Type = type_method;
            temp.Name = _masValues[1];
            //Добавляем переменные
            for (int i = 2; i < _masValues.Length; i += 2)
            {
                temp.AddVariable(new C_Variables(_masValues[i], _masValues[i + 1]));
            }
            return temp;
        }
        //Функция чтения файла для добавления класса\интерфейса
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
                        //Получаем родителей, если они есть
                        int indexParent = 0;
                        if (_valueInLine.Count > 2)
                        {
                            if (_valueInLine[2][0] == ':' && _valueInLine[2].Length > 1)
                            {
                                indexParent = 3;
                            }
                            if (_valueInLine[2] == ":")
                            {
                                indexParent = 4;
                            }
                            string tempParents = string.Empty;
                            for (int i = indexParent; i < _valueInLine.Count; i++)
                            {
                                bool index = false;
                                if (_valueInLine[i].LastIndexOf(',') != -1 && _valueInLine[i].LastIndexOf(',') != _valueInLine[i].Length - 1)
                                {
                                    _valueInLine[i] = _valueInLine[i].Remove(_valueInLine[i].LastIndexOf('('));
                                }
                                if (_valueInLine[i].LastIndexOf(',') != -1)
                                {
                                    _valueInLine[i] = _valueInLine[i].Remove(_valueInLine[i].Length - 1);
                                    index = true;
                                }
                                tempParents += _valueInLine[i] + " ";
                                if (index)
                                {
                                    i++;
                                }
                            }
                            foreach (string parent in tempParents.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                temp.ParentClasses.Add(new ClassBox(parent));
                            }
                        }
                        //И идем дальше
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
                            if (!AddMethod(_valueInLine).Virtual)
                            {
                                temp.Methods.Add(AddMethod(_valueInLine));
                            }
                            else
                            {
                                temp.VirtualMethods.Add(AddMethod(_valueInLine));
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
