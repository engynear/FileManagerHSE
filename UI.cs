using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerHSE
{
    static class UI
    {
        public static void SetDefaultConsoleSettings()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void PrintErrorMsg(string errorMsg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMsg);
        }
        public static void PrintErrorMsg(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
        }

        public static void PrintContent(FileSystemInfo[] list)
        {
            Console.ForegroundColor = ConsoleColor.White;
            printSeparator();
            bool is_dir;
            foreach(var element in list)
            {
                is_dir = Directory.Exists(element.FullName);
                if (is_dir)
                {
                    Console.WriteLine("[" + element.Name + "]");
                }
                else
                    Console.WriteLine(element.Name);
            }
            printSeparator();
        }

        

        public static void PrintLine(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg);
        }

        public static void Print(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(msg);
        }

        
        public static bool PrintFileText(FileInfo file, string encoding = "UTF-8")
        {
            string[] fileLines = File.ReadAllLines(file.FullName, Encoding.GetEncoding(encoding));
            foreach (var line in fileLines)
            {
                UI.PrintLine(line);
            }
            return true;
        }
        

        private static void printSeparator(char separator = '-', int length = 30)
        {
            StringBuilder sb = new();
            for (int i = 0; i < length; i++)
                sb.Append(separator);
            Console.WriteLine(sb.ToString());
        }
    }
}
