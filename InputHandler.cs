using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileManagerHSE
{
    static class InputHandler
    {
        private static string lastInput = "";
        public static string[] inputCommand(FileManager fileManager)
        {
            ConsoleKeyInfo input;
            StringBuilder sb = new();
            int cur_pose = 0;
            string cur_input = "";

            Console.ForegroundColor = ConsoleColor.Yellow;

            do
            {
                input = Console.ReadKey();
                switch (input.Key)
                {
                    case ConsoleKey.Backspace:
                        if (cur_pose <= sb.Length && cur_pose > 0)
                            sb.Remove(--cur_pose, 1);
                        else if (cur_pose > 0)
                            Console.CursorLeft -= 1;

                        refresh(sb);
                        break;

                    case ConsoleKey.Delete:
                        if (cur_pose < sb.Length)
                            sb.Remove(cur_pose, 1);
                        refresh(sb);
                        break;

                    case ConsoleKey.LeftArrow:
                        if (Console.CursorLeft > 0)
                        {
                            cur_pose--;
                            Console.CursorLeft -= 1;
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (Console.BufferWidth-1 > Console.CursorLeft)
                        {
                            cur_pose++;
                            Console.CursorLeft += 1;
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        cur_input = sb.ToString();
                        sb = new(lastInput);
                        cur_pose = lastInput.Length;
                        refresh(sb);
                        break;

                    case ConsoleKey.DownArrow:
                        sb = new(cur_input);
                        cur_pose = cur_input.Length;
                        refresh(sb);
                        break;

                    case ConsoleKey.Enter:
                    case ConsoleKey.Escape:
                        break;

                    case ConsoleKey.Tab:
                        Console.CursorLeft = sb.Length;
                        cur_pose = sb.Length;
                        string[] args = getArguments(sb.ToString());
                        string to_complete = args[args.Length - 1];

                        if (to_complete.Contains("\""))
                        {
                            to_complete = to_complete.Replace("\"", "");
                        }

                        string dir = fileManager.getWorkingDirectory().FullName;
                        if (to_complete.Contains('/') || to_complete.Contains('\\'))
                        {
                            string toCompletePath = fileManager.toAbsolutePath(to_complete);
                            to_complete = Path.GetFileName(toCompletePath);
                            dir = toCompletePath.Substring(0,
                                toCompletePath.Length - to_complete.Length); 
                        }

                        
                        List<string> fit_names = new List<string>();
                        foreach (var info in new FileManager(dir).GetContent())
                        {
                            if (info.Name.ToLower().StartsWith(to_complete.ToLower()))
                            {
                                fit_names.Add(info.Name);
                            }
                        }
                        if (fit_names.Count == 1)
                        {
                            cur_pose -= to_complete.Length;
                            sb.Remove(cur_pose, to_complete.Length);
                            sb.Append(fit_names[0]);
                            cur_pose += fit_names[0].Length;
                        }
                        else if (fit_names.Count > 1)
                        {
                            for (int i = 0; i < fit_names.Count; i++)
                            {
                                fit_names[i] = fit_names[i].Substring(to_complete.Length);
                            }
                            bool checking = true;
                            string check = "";
                            for (int i = 1; i < fit_names[0].Length + 1; i++)
                            {
                                check = fit_names[0].Substring(0, i);
                                for (int j = 0; j < fit_names.Count; j++)
                                {
                                    if (!fit_names[j].StartsWith(check))
                                        checking = false;
                                }
                                if (!checking)
                                {
                                    check = check.Substring(0, check.Length - 1);
                                    break;
                                }
                            }
                            sb.Append(check);
                            cur_pose += check.Length;
                        }
                        refresh(sb);
                        break;

                    default:
                        try
                        {
                            sb.Insert(cur_pose++, input.KeyChar);
                        }
                        catch (Exception)
                        {

                        }

                        refresh(sb);
                        break;
                }
                try
                {
                    Console.CursorLeft = cur_pose;
                }
                catch (Exception)
                {
                    Console.Clear();
                    sb.Clear();
                    cur_pose = 0;
                    UI.PrintErrorMsg("Command too long, put the command on one line");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

            } while (input.Key != ConsoleKey.Enter);
            lastInput = sb.ToString();
            Console.Write("\n");
            sb.Clear();
            UI.SetDefaultConsoleSettings();

            return getArguments(lastInput);
        }

        private static void refresh(StringBuilder sb)
        {
            clearConsole();
            Console.Write("\r");
            Console.Write(sb.ToString());
        }

        private static void clearConsole()
        {
            Console.Write("\r");
            Console.Write(new String(' ', Console.BufferWidth - 1));
        }

        private static string[] getArguments(string str)
        {
            char[] paramChars = str.ToCharArray();
            bool inQuote = false;
            for (int i = 0; i < paramChars.Length; i++)
            {
                if (paramChars[i] == '"')
                    inQuote = !inQuote;
                if (!inQuote && paramChars[i] == ' ')
                    paramChars[i] = '\n';
            }
            return (new string(paramChars)).Split('\n');
        }
    }
}
