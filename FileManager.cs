using System;
using System.IO;
using System.Security;
using System.Text;

namespace FileManagerHSE
{
    class FileManager
    {
        private DirectoryInfo workingDirectory;


        public FileManager(string path)
        {
            DirectoryInfo directory = new(path);
            if (directory.Exists)
                this.workingDirectory = directory;
            else
            {
                changeDirectory(path);
            }
        }

        public FileManager(DirectoryInfo workingDirectory)
        {
            this.workingDirectory = workingDirectory;
        }

        public FileManager()
        {
            this.workingDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        }

        public bool changeDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
                return false;
            this.workingDirectory = directory;
            return true;
            
        }

        public bool changeDirectory(string path)
        {
            DirectoryInfo directory;
            path = toAbsolutePath(path);

            directory = new(path);
            
            if (directory.Exists)
                return changeDirectory(directory);
            else
            {
                UI.PrintErrorMsg("Invalid directory");
            }
            return false;
        }

        public FileInfo[] GetFiles(string searchPattern = "*")
        {
            try
            {
                return workingDirectory.GetFiles(searchPattern);
            }catch(Exception e)
            {
                switch (e)
                {
                    case ArgumentException:
                        UI.PrintErrorMsg("Wrong searchOption argument");
                        break;
                    case DirectoryNotFoundException:
                        UI.PrintErrorMsg("The current working directory has been deleted or changed.\n" +
                            "Try to change working directory (cd).");
                        break;
                    case SecurityException:
                        UI.PrintErrorMsg("Directory access error.");
                        break;
                }
                return new FileInfo[0];
            }
            
        }

        public DirectoryInfo[] GetDirectories(string searchPattern = "*")
        {
            try
            {
                return workingDirectory.GetDirectories(searchPattern);
            }catch(Exception e)
            {
                switch (e)
                {
                    case DirectoryNotFoundException:
                        UI.PrintErrorMsg("The current working directory has been deleted or changed.\n" +
                            "Try to change working directory (cd).");
                        break;
                    case SecurityException:
                    case UnauthorizedAccessException:
                        UI.PrintErrorMsg("Directory access error.");
                        break;

                }
                return new DirectoryInfo[0];
            }

        }

        public FileSystemInfo[] GetContent()
        {
            try
            {
                return workingDirectory.GetFileSystemInfos();
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case DirectoryNotFoundException:
                        UI.PrintErrorMsg("The current working directory has been deleted or changed.\n" +
                            "Try to change working directory (cd).");
                        break;
                }
                return new FileSystemInfo[0];
            }
        }

        public DirectoryInfo getWorkingDirectory()
        {
            return workingDirectory;
        }

        public bool selectDisk()
        {
            UI.PrintLine("Select disk:");
            DriveInfo[] drives = DriveInfo.GetDrives();
            for(int i = 0; i<drives.Length; i++)
            {
                UI.PrintLine((i+1) + ". " + drives[i].Name);
            }
            UI.Print("Type number of disk: ");
            int selection = 0;
            try
            {
                selection = Int32.Parse(Console.ReadLine());
            }catch(Exception e)
            {

                return false;
            }
            if (selection - 1 >= drives.Length)
            {
                UI.PrintErrorMsg("Selection failed, index out of range");
                return false;
            }
            return changeDirectory(drives[selection-1].Name);
        }


        public bool deleteFile(string filePath)
        {
            filePath = toAbsolutePath(filePath);
            if (!File.Exists(filePath))
                return false;
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case UnauthorizedAccessException:
                        UI.PrintErrorMsg("Access exception.");
                        break;
                    case ArgumentException:
                        UI.PrintErrorMsg("Wrong arguments.");
                        break;
                    case PathTooLongException:
                        UI.PrintErrorMsg("Path too long.");
                        break;
                    case DirectoryNotFoundException:
                        UI.PrintErrorMsg("Working directory has been changed or deleted." +
                            "\n Or can`t find target directory");
                        break;
                    case FileNotFoundException:
                        UI.PrintErrorMsg("File not found.");
                        break;
                    case IOException:
                        UI.PrintErrorMsg("Input/Ouput exception.");
                        break;
                    case NotSupportedException:
                        UI.PrintErrorMsg("Not supported exception.");
                        break;
                }
                return false;
            }
            return true;
        }

        public bool copyFile(string filePath, string copyDestPath, bool overwrite = true)
        {
            filePath = toAbsolutePath(filePath);
            copyDestPath = toAbsolutePath(copyDestPath);
            if (Path.GetExtension(copyDestPath) == "")
                copyDestPath += "/" + Path.GetFileName(filePath);
            try
            {
                File.Copy(filePath, copyDestPath, overwrite);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case UnauthorizedAccessException:
                        UI.PrintErrorMsg("Access exception.");
                        break;
                    case ArgumentException:
                        UI.PrintErrorMsg("Wrong arguments.");
                        break;
                    case PathTooLongException:
                        UI.PrintErrorMsg("Path too long.");
                        break;
                    case DirectoryNotFoundException:
                        UI.PrintErrorMsg("Working directory has been changed or deleted." +
                            "\n Or can`t find target directory");
                        break;
                    case FileNotFoundException:
                        UI.PrintErrorMsg("File to copy not found.");
                        break;
                    case IOException:
                        UI.PrintErrorMsg("Input/Ouput exception.");
                        break;
                    case NotSupportedException:
                        UI.PrintErrorMsg("Not supported exception.");
                        break;
                }
                return false;
            }
            
            return true;
        }

        public bool moveFile(string filePath, string copyDestPath, bool overwrite = true)
        {
            return copyFile(filePath, copyDestPath, overwrite) && deleteFile(filePath);
        }

        public bool createFile(string filePath, string encoding = "UTF-8")
        {
            filePath = toAbsolutePath(filePath);
            FileStream fs;
            Encoding enc;
            try
            {
                fs = File.Open(filePath, FileMode.Create);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ArgumentException:
                        UI.PrintErrorMsg("Wrong argument exception.");
                        break;
                    case PathTooLongException:
                        UI.PrintErrorMsg("Path too long exception.");
                        break;
                    case DirectoryNotFoundException:
                        UI.PrintErrorMsg("Working directory has been changed or deleted." +
                            "\n Or can`t find target directory");
                        break;
                    case IOException:
                        UI.PrintErrorMsg("Input/output exception.");
                        break;
                    case UnauthorizedAccessException:
                        UI.PrintErrorMsg("Access exception.");
                        break;
                    case NotSupportedException:
                        UI.PrintErrorMsg("Not supported.");
                        break;
                           
                }
                return false;
            }
            try
            {
                enc = Encoding.GetEncoding(encoding);
            }catch(ArgumentException e)
            {
                UI.PrintErrorMsg("Wrong encoding argument. Set to UTF-8.");
                enc = Encoding.UTF8;
            }
            
            using (StreamWriter sw = new StreamWriter(fs, enc)) {
                sw.WriteLine();
                return true;
            };
            
        }

        public FileInfo GetFile(string filePath)
        {
            filePath = toAbsolutePath(filePath);
            return new FileInfo(filePath);
        }

        public bool concatFiles(string file1Path, string file2Path, string newFilePath)
        {
            file1Path = toAbsolutePath(file1Path);
            file2Path = toAbsolutePath(file2Path);

            newFilePath = toAbsolutePath(newFilePath);

            try
            {
                if (Path.GetExtension(newFilePath) == "")
                    newFilePath += "/" + Path.GetFileNameWithoutExtension(file1Path) +
                            "-" + Path.GetFileNameWithoutExtension(file2Path) +
                            Path.GetExtension(file1Path);

                string[] file1Lines = File.ReadAllLines(file1Path);
                string[] file2Lines = File.ReadAllLines(file2Path);

                using (StreamWriter sw = new StreamWriter(File.Open(newFilePath, FileMode.Create), Encoding.UTF8))
                {
                    foreach (var line in file1Lines)
                    {
                        sw.WriteLine(line);
                        UI.PrintLine(line);
                    }
                    foreach (var line in file2Lines)
                    {
                        sw.WriteLine(line);
                        UI.PrintLine(line);
                    }

                    return true;
                }
            }catch(Exception e)
            {
                UI.PrintErrorMsg(e);
                return false;
            }

        }

        private bool isAbsolutePath(string path)
        {
            return Path.IsPathRooted(path);
        }

        public string toAbsolutePath(string path)
        {
            if (!isAbsolutePath(path))
                path = workingDirectory + "/" + path;
            return path;
        }
    }
}
