using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kursach
{
    public enum Action
    {
        AddClass = 1,
        AddInterface = 2,
        AddAggregation = 3,
        AddComposition = 4,
        AddInheritance = 5,

        DeleteClass = 6,
        DeleteInterface = 7,
        DeleteAggregation = 8,
        DeleteComposition = 9,
        DeleteIntheritance = 10,

        CreateDiagram = 11,
        CreateCode = 12,

        OpenProject = 13,
        SaveFiles = 14

    }
    public class LoggerUserAction
    {
        Action _User_Action;
        DateTime _Date_Action;
        string _Name_Object;

        public LoggerUserAction(Action user_action, DateTime date_action, string name_object)
        {
            _User_Action = user_action;
            _Date_Action = date_action;
            _Name_Object = name_object;
            ToFile("logger.log");
        }
        public void ToFile(string sNameFile)
        {
            using (StreamWriter writer = File.AppendText(sNameFile))
            {
                writer.WriteLine(_Date_Action.ToString() + " " + _User_Action.ToString() + " " + _Name_Object);
                writer.Close();
            }
        }
        public void IntoFile(string sNameFile)
        {
            using (StreamReader sr = new StreamReader(sNameFile))
            {
                string[] str = sr.ReadToEnd().Split(new char[] { '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                int pos = 0;
                int count = 0;
                foreach (string log in str)
                {
                    for (int i = 0; i < log.Length; i++)
                    {
                        if (count == 2)
                            break;
                        if (log[i] == ' ')
                        {
                            count++;
                            pos = i;
                        }
                    }
                    _Date_Action = DateTime.Parse(log.Remove(pos));
                    _User_Action = (Action)Enum.Parse(typeof(Action), log.Substring(pos, log.LastIndexOf(' ') - pos));
                    _Name_Object = log.Remove(0, log.LastIndexOf(' ') + 1);
                }
            }
        }
        public string Info()
        {
            return _Date_Action + " " + _User_Action + " " + _Name_Object;
        }
    }
}
