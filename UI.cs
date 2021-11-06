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
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void PrintErrorMsg(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintContent(FileSystemInfo[] list)
        {
            Console.ForegroundColor = ConsoleColor.White;
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
            string[] fileLines;
            try
            {
                fileLines = File.ReadAllLines(file.FullName, Encoding.GetEncoding(encoding));
            }catch(Exception e)
            {
                UI.PrintErrorMsg(e);
                return false;
            }

            foreach (var line in fileLines)
            {
                UI.PrintLine(line);
            }
            return true;
        }
        
        public static void PrintHelpMsg(string command = null)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            string message;
            switch (command)
            {
                case "cd":
                    message = "cd <dirPath> - Change to directory " +
                        "\n(Path can be absolute or relative).";
                    break;
                case "ls":
                    message = "ls - List current directory contents.";
                    break;
                case "printFile":
                case "cat":
                    message = "cat <fileDir> - Display file’s contents to the console.";
                    break;
                case "pwd":
                case "printWorkingDirectory":
                    message = "pwd - Display the pathname for the current directory.";
                    break;
                case "selDisk":
                case "selectDisk":
                case "disk":
                    message = "disk - Displays available drives " +
                        "and allows you to switch between them.";
                    break;
                case "help":
                    message = "help <command> - Display the help information for the specified command." +
                        "\nhelp - Display commands list.";
                    break;
                case "fileList":
                    message = "fileList <searchPattern> - List files in current dir matching the mask " +
                        "\n(* or None searchPattern displays all files in current directory).";
                    break;
                case "dirList":
                    message = "dirList <searchPattern> - List directories in current dir matching the mask" +
                        "\n (* or None searchPattern displays all directories in current directory).";
                    break;
                case "rm":
                case "deleteFile":
                    message = "rm <path> - Remove (delete) file(s) and/or directories.";
                    break;
                case "copy":
                    message = "copy <fileToCopy> <path> - Copy file to specified directory.";
                    break;
                case "mv":
                case "move":
                    message = "mv <fileToMove> <path> <overwrite> - Move file to specified directory.";
                    break;
                case "touch":
                case "createFile":
                    message = "touch <fileName> - Create an empty file with the specified name.";
                    break;
                case "concat":
                    message = "concat <file1Path> <file2Path> <newFilePath> " +
                        "\n- Concatenate two files to specified path (+ spec. name).";
                    break;
                case "exit":
                    message = "exit - Close application.";
                    break;
                case null:
                    try
                    {
                        message = File.ReadAllText("../../../help.txt");
                    }catch(Exception e)
                    {
                        message = "Type help <command> to see detailed info about command.\n" +
                            "Commands list:\n cd, ls, cat, pwd, disk, help,\n" +
                            "fileList, dirList, rm, copy, mv, touch, concat, exit";
                            
                    }
                    break;
                default:
                    message = "No such command, type help to see commands list";
                    break;
            }
            Console.WriteLine(message);
            SetDefaultConsoleSettings();
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
