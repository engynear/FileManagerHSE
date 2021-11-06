using System;
using System.Collections.Generic;

namespace FileManagerHSE
{
    class Program
    {
        static void Main(string[] args)
        {
            FileManager fileManager = new("C://");
            bool running = true;
            while (running)
            {
                string[] input = InputHandler.inputCommand(fileManager);
                string command = input[0];
                List<string> arguments = new();
                for (int i = 1; i < input.Length; i++)
                    arguments.Add(input[i]);

                switch (command)
                {
                    case "cd":
                        if (!(arguments.Count == 1))
                        {
                            UI.PrintErrorMsg("Wrong arguments, type directory path");
                        }
                        fileManager.changeDirectory(arguments[0]);
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
                        UI.PrintLine(fileManager.getWorkingDirectory().FullName);
                        break;

                    case "selDisk":
                    case "selectDisk":
                    case "disk":
                        fileManager.selectDisk();
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
                        if (!(arguments.Count == 1))     
                            UI.PrintErrorMsg("Wrong arguments, type file path");
                        else 
                            fileManager.deleteFile(arguments[0]);
                        break;

                    case "copy":
                        if (!(arguments.Count == 2))
                            UI.PrintErrorMsg("Wrong arguments, type file path");
                        else
                            fileManager.copyFile(arguments[0], arguments[1]);
                        break;

                    case "mv":
                    case "move":
                        if (!(arguments.Count == 2))                       
                            UI.PrintErrorMsg("Wrong arguments, type file path");
                        else
                            fileManager.moveFile(arguments[0], arguments[1]);
                        break;

                    case "touch":
                    case "createFile":
                        if (arguments.Count == 1)
                            fileManager.createFile(arguments[0]);
                        else if (arguments.Count == 2)
                            fileManager.createFile(arguments[0], arguments[1]);
                        else
                            UI.PrintErrorMsg("Wrong arguments, type file path");
                        break;

                    case "concat":
                        if (!(arguments.Count == 3))
                            UI.PrintErrorMsg("Wrong arguments, type file path");
                        else
                            fileManager.concatFiles(arguments[0], arguments[1], arguments[2]);
                        break;

                    case "exit":
                        running = false;
                        break;

                    default:
                        UI.PrintErrorMsg("Unknown command, type help to see available commands.");
                        break;
                }
            }

        }
    }
}
