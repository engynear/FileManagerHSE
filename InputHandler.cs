using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileManagerHSE
{
    static class InputHandler
    {
        private static List<string> commandHistory = new();

        /// <summary>
        /// Returns whether the path is absolute
        /// </summary>
        /// <param name="fileManager">Object of FileManager class</param>
        /// <param name="waitCommandLine">String line with which the command input will start</param>
        /// <returns>Array of string type</returns>
        public static string[] inputCommand(FileManager fileManager, string waitCommandLine = "> ")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            ConsoleKeyInfo input;
            StringBuilder sb = new();
            Console.Write(waitCommandLine);
            int curPos = 0;
            int commandHistoryPosition = 0;
            string curCommandBuffer = "";

            do
            {
                input = Console.ReadKey();

                switch (input.Key)
                {
                    case ConsoleKey.Enter:
                        commandHistory.Insert(0, sb.ToString());
                        break;

                    case ConsoleKey.LeftArrow:
                        if (Console.CursorLeft > waitCommandLine.Length)
                            curPos--;
                        break;

                    case ConsoleKey.RightArrow:
                        if (Console.BufferWidth - 1 > Console.CursorLeft)
                            curPos++;
                        break;

                    case ConsoleKey.UpArrow:
                        if (commandHistoryPosition == 0)
                            curCommandBuffer = sb.ToString();

                        if (commandHistoryPosition >= commandHistory.Count)
                            break;

                        sb = new StringBuilder(commandHistory[commandHistoryPosition]);
                        curPos = sb.Length;
                        commandHistoryPosition++;
                        break;

                    case ConsoleKey.DownArrow:
                        if (commandHistoryPosition == 0)
                            break;
                        else if (commandHistoryPosition == 1)
                            sb = new StringBuilder(curCommandBuffer);
                        else
                            sb = new StringBuilder(commandHistory[commandHistoryPosition - 2]);

                        curPos = sb.Length;
                        commandHistoryPosition--;
                        break;

                    case ConsoleKey.Backspace:
                        if (curPos <= sb.Length && curPos > 0)
                            sb.Remove(--curPos, 1);
                        break;

                    case ConsoleKey.Delete:
                        if (curPos < sb.Length)
                            sb.Remove(curPos, 1);
                        break;

                    case ConsoleKey.Tab:
                        string[] args = getArguments(sb.ToString());
                        if (args.Length != 1)
                        {
                            string toComplete = args[args.Length - 1];
                            bool isInsideQuotes = toComplete.Contains("\"");
                            if (toComplete.EndsWith("\""))
                                sb.Remove(sb.Length - 1, 1);
                            toComplete = toComplete.Replace("\"", "");
                            sb = autoCompleteContent(fileManager, toComplete, sb, isInsideQuotes);


                            if (isInsideQuotes)
                                sb.Append("\"");
                        }
                        curPos = sb.Length;
                        break;

                    default:
                        if (curPos <= sb.Length)
                            sb.Insert(curPos, input.KeyChar);
                        else
                            sb.Append(input.KeyChar);
                        curPos++;

                        break;
                }
                if (curPos + waitCommandLine.Length < Console.BufferWidth - 1)
                    refreshInputLine(sb, curPos, waitCommandLine);
                else
                {
                    clearInputLine();
                    sb.Clear();
                    curPos = 0;
                    UI.PrintErrorMsg("Command too long, put the command on one line. (tip: showPath false)");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }


            } while (input.Key != ConsoleKey.Enter);

            Console.Write("\n");
            sb.Clear();
            UI.SetDefaultConsoleSettings();

            return getArguments(commandHistory[0]);

        }

        /// <summary>
        /// Completes live to file or directory name by part of it.
        /// </summary>
        /// <returns>Obejct of StringBuilder class</returns>
        private static StringBuilder autoCompleteContent(FileManager fileManager, string toComplete, StringBuilder sb, bool isInsideQuotes)
        {
            string dir = fileManager.GetWorkingDirectory().FullName;
            if (toComplete.Contains('/') || toComplete.Contains('\\'))
            {
                string toCompletePath = fileManager.CastToAbsolutePath(toComplete);
                toComplete = Path.GetFileName(toCompletePath);
                dir = toCompletePath.Substring(0,
                    toCompletePath.Length - toComplete.Length);
            }

            List<string> fitNames = new List<string>();
            foreach (var info in new FileManager(dir).GetContent())
            {
                if (info.Name.ToLower().StartsWith(toComplete.ToLower()))
                {
                    fitNames.Add(info.Name);
                }
            }

            return autoComplete(fitNames, toComplete, sb, fileManager, isInsideQuotes);
        }

        private static StringBuilder autoComplete(List<string> fitNames, string toComplete, StringBuilder sb, FileManager fileManager, bool isInsideQuotes)
        {
            if (fitNames.Count == 1)
            {
                sb.Remove(sb.Length - toComplete.Length, toComplete.Length);
                if (fitNames[0].Contains(" ") && !isInsideQuotes)
                    sb.Append("\"");
                sb.Append(fitNames[0]);
                string[] args = getArguments(sb.ToString());
                if (Directory.Exists(fileManager.CastToAbsolutePath(args[args.Length - 1])))
                    sb.Append('/');
                if (fitNames[0].Contains(" ") && !isInsideQuotes)
                    sb.Append("\"");





                return sb;


            }
            else if (fitNames.Count > 1)
            {
                for (int i = 0; i < fitNames.Count; i++)
                {
                    fitNames[i] = fitNames[i].Substring(toComplete.Length);
                }
                bool checking = true;
                string check = "";
                for (int i = 1; i < fitNames[0].Length + 1; i++)
                {
                    check = fitNames[0].Substring(0, i);
                    for (int j = 0; j < fitNames.Count; j++)
                    {
                        if (!fitNames[j].StartsWith(check))
                            checking = false;
                    }
                    if (!checking)
                    {
                        check = check.Substring(0, check.Length - 1);
                        break;
                    }
                }

                return sb;
            }
            return sb;
        }

        /// <summary>
        /// Reprints line of console command input.
        /// </summary>
        private static void refreshInputLine(StringBuilder sb, int curPos, string waitCommandLine)
        {
            clearInputLine();
            Console.Write(waitCommandLine);
            Console.Write(sb.ToString());
            Console.CursorLeft = curPos + waitCommandLine.Length;
        }

        /// <summary>
        /// Clears current console line.
        /// </summary>
        private static void clearInputLine()
        {
            Console.Write("\r");
            Console.Write(new String(' ', Console.BufferWidth - 1));
            Console.Write("\r");
        }

        /// <summary>
        /// Splits string by spaces, ignoring spaces inside quotes.
        /// </summary>
        /// <returns>Array of string type.</returns>
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