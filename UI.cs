using System;
using System.IO;
using System.Text;


namespace FileManagerHSE
{
    static class UI
    {

        /// <summary>
        /// Sets default console settings.
        /// </summary>
        public static void SetDefaultConsoleSettings()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Prints an error message.
        /// </summary>
        /// <param name="errorMsg">String error message</param>
        public static void PrintErrorMsg(string errorMsg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMsg);
            SetDefaultConsoleSettings();
        }

        /// <summary>
        /// Prints an error message.
        /// </summary>
        /// <param name="e">Error object</param>
        public static void PrintErrorMsg(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            SetDefaultConsoleSettings();
        }

        /// <summary>
        /// Prints the content from array by element.
        /// </summary>
        /// <param name="list">List of content</param>
        public static void PrintContent(FileSystemInfo[] list)
        {
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

        /// <summary>
        /// Prints message with line break.
        /// </summary>
        /// <param name="msg">Message</param>
        public static void PrintLine(string msg)
        {
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Prints message without line break.
        /// </summary>
        /// <param name="msg">Message</param>
        public static void Print(string msg)
        {
            Console.Write(msg);
        }

        /// <summary>
        /// Prints the contents of a text file in the specified encoding
        /// </summary>
        /// <param name="encoding">Encoding (UTF8, ASCII or UNICODE)</param>
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

        /// <summary>
        /// Help messages handler
        /// </summary>
        /// <param name="command">Specified command to print help message</param>
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
                case "showPath":
                    message = "showPath <bool> - shows current working dir in console input line if true";
                    break;
                case null:
                    try
                    {
                        message = File.ReadAllText("../../../help.txt");
                    }catch(Exception e)
                    {
                        Console.WriteLine("Type help <command> to see detailed info about command.");
                        Console.WriteLine("Commands list:");
                        Console.WriteLine("cd <dirPath> - Change to directory.");
                        Console.WriteLine("ls - List current directory contents.");
                        Console.WriteLine("cat <fileDir> - Display file’s contents to the console.");
                        Console.WriteLine("pwd - Display the pathname for the current directory.");
                        Console.WriteLine("disk - Displays available drives " +
                            "\nand allows you to switch between them.");
                        Console.WriteLine("help <command> - Display the help information for the specified command.");
                        Console.WriteLine("fileList - List all files in current directory.");
                        Console.WriteLine("fileList <searchPattern> - List files in current dir matching the mask.");
                        Console.WriteLine("dirList - List all directories in current directory.");
                        Console.WriteLine("dirList <searchPattern> - List directories in current dir matching the mask.");
                        Console.WriteLine("rm <path> - Remove (delete) file(s) and/or directories.");
                        Console.WriteLine("copy <fileToCopy> <path> - Copy file to specified directory.");
                        Console.WriteLine("mv <fileToMove> <path> <overwrite> - Move file to specified directory.");
                        Console.WriteLine("touch <fileName> - Create an empty file with the specified name.");
                        Console.WriteLine("concat <file1Path> <file2Path> <newFilePath> - Concatenate two files.");
                        Console.WriteLine("showPath <bool> - Shows current working dir in console input line if true");
                        Console.WriteLine("exit - Close application.");
                        message = "";
                    }
                    break;
                default:
                    message = "No such command, type help to see commands list";
                    break;
            }
            Console.WriteLine(message);
            SetDefaultConsoleSettings();
            
        }
    }
}
