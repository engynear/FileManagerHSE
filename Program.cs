using System;
using System.Collections.Generic;

namespace FileManagerHSE
{
    class Program
    {
        static void Main(string[] args)
        {
            FileManager fileManager = new();
            bool running = true;
            bool showPath = true;
            while (running)
            {
                string waitCommandLine = showPath ? fileManager.GetWorkingDirectory().FullName + "> " : "> ";
                string[] input = InputHandler.inputCommand(fileManager, waitCommandLine);
                string command = input[0];
                List<string> arguments = new();
                for (int i = 1; i < input.Length; i++)
                    arguments.Add(input[i].Replace("\"",""));

                switch (command)
                {
                    case "cd":
                        if (arguments.Count == 1)
                            fileManager.ChangeDirectory(arguments[0]);
                        else
                            UI.PrintErrorMsg("Wrong arguments, type directory path");

                        break;

                    case "ls":
                        UI.PrintContent(fileManager.GetContent());
                        break;

                    case "printFile":
                    case "cat":
                        if (arguments.Count == 1)
                        {
                            UI.PrintFileText(fileManager.GetFile(arguments[0]));
                        }
                        else if (arguments.Count == 2)
                        {
                            UI.PrintFileText(fileManager.GetFile(arguments[0]), arguments[1]);
                        }
                        else
                        {
                            UI.PrintErrorMsg("Wrong arguments, type file name");
                        }
                        break;

                    case "pwd":
                    case "printWorkingDirectory":
                        UI.PrintLine(fileManager.GetWorkingDirectory().FullName);
                        break;

                    case "selDisk":
                    case "selectDisk":
                    case "disk":
                        fileManager.SelectDisk();
                        break;

                    case "help":
                        if (arguments.Count == 0)
                            UI.PrintHelpMsg();
                        else
                            UI.PrintHelpMsg(arguments[0]);
                        break;

                    case "fileList":
                        if (arguments.Count == 1)
                            UI.PrintContent(fileManager.GetFiles(arguments[0]));
                        else if (arguments.Count == 0)
                            UI.PrintContent(fileManager.GetFiles());
                        else
                            UI.PrintErrorMsg("Too much arguments, type files mask");
                        break;

                    case "dirList":
                        UI.PrintContent(fileManager.GetDirectories());
                        break;

                    case "rm":
                    case "deleteFile":
                        if (arguments.Count == 1)
                            fileManager.DeleteFile(arguments[0]);
                        else
                            UI.PrintErrorMsg("Wrong arguments, type file path");

                        break;
                    case "copy":
                        if (arguments.Count == 2)
                            fileManager.CopyFile(arguments[0], arguments[1]);
                        else if (arguments.Count == 3)
                            fileManager.CopyFile(arguments[0], arguments[1], Boolean.Parse(arguments[2]));
                        else
                            UI.PrintErrorMsg("Wrong arguments, type file path");

                        break;

                    case "mv":
                    case "move":
                        if (arguments.Count == 2)
                            fileManager.MoveFile(arguments[0], arguments[1]);
                        else if (arguments.Count == 3)
                            fileManager.MoveFile(arguments[0], arguments[1], Boolean.Parse(arguments[2]));
                        else
                            UI.PrintErrorMsg("Wrong arguments, type file path");
                        break;

                    case "touch":
                    case "createFile":
                        if (arguments.Count == 1)
                            fileManager.CreateFile(arguments[0]);
                        else if (arguments.Count == 2)
                            fileManager.CreateFile(arguments[0], arguments[1]);
                        else
                            UI.PrintErrorMsg("Wrong arguments, type file path");
                        break;

                    case "concat":
                        if (arguments.Count==0)
                            UI.PrintErrorMsg("No arguments. Type filePaths to concatenate.");
                        else
                            fileManager.ConcatFiles(arguments.ToArray());
                        break;

                    case "exit":
                        running = false;
                        break;

                    case "showPath":
                        if (arguments.Count == 1)
                            try
                            {
                                showPath = Boolean.Parse(arguments[0]);
                                UI.PrintLine(showPath.ToString());
                            }
                            catch(Exception e)
                            {
                                UI.PrintErrorMsg("You can only use true/false as an argument.");
                            }
                            
                        else
                            UI.PrintErrorMsg("Wrong arguments, type true/false as an argument.");
                        break;

                    default:
                        UI.PrintErrorMsg("Unknown command, type help to see available commands.");
                        break;
                }
            }

        }
    }
}
