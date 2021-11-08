using System;
using System.IO;
using System.Security;
using System.Text;

namespace FileManagerHSE
{
    class FileManager
    {
        private DirectoryInfo workingDirectory;

        /// <summary>
        /// FileManager constructor
        /// </summary>
        /// <param name="path">Initial working directory name</param>
        /// <returns>Object of class FileManager</returns>
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

        /// <summary>
        /// FileManager constructor
        /// </summary>
        /// <param name="workingDirectory">Initial DirectoryInfo object</param>
        /// <returns>Object of class FileManager</returns>
        public FileManager(DirectoryInfo workingDirectory)
        {
            this.workingDirectory = workingDirectory;
        }

        public FileManager()
        {
            this.workingDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        }

        /// <summary>
        /// Changes current working directory.
        /// </summary>
        /// <param name="directory">DirectoryInfo object</param>
        /// <returns>true if directory changed; overwise, false.</returns>
        public bool changeDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
                return false;
            this.workingDirectory = directory;
            return true;
            
        }

        /// <summary>
        /// Changes current working directory.
        /// </summary>
        /// <param name="path">String directory path</param>
        /// <returns>true if directory changed; overwise, false.</returns>
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

        /// <summary>
        /// Returns a files list from current directory matching the given search pattern.
        /// </summary>
        /// <param name="searchPattern">Files mask</param>
        /// <returns>An array of type FileInfo.</returns>
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

        /// <summary>
        /// Returns directories list in current directory matching the given searchPattern.
        /// </summary>
        /// <param name="searchPattern">Directories mask</param>
        /// <returns>An array of type DirectoryInfo matching searchPattern.</returns>
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

        /// <summary>
        /// Return files and directories list in current directory.
        /// </summary>
        /// <returns>An array of FileSystemInfo type.</returns>
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

        /// <summary>
        /// Returns object of current working directory.
        /// </summary>
        /// <returns>Object of DirectoryInfo type.</returns>
        public DirectoryInfo getWorkingDirectory()
        {
            return workingDirectory;
        }

        /// <summary>
        /// Print system drives and allows to switch working directory to one of them.
        /// </summary>
        /// <returns>true if working directory changed; overwise, false</returns>
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


        /// <summary>
        /// Deletes specified file at the given path.
        /// </summary>
        /// <param name="filePath">Absolute or relative file path</param>
        /// <returns>true if file deleted; overwise, false.</returns>
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

        /// <summary>
        /// Copies an existing file to a new file. Overwriting if allowed.
        /// </summary>
        /// <param name="filePath">Absolute or relative file to copy path</param>
        /// <param name="copyDestPath">Absolute or relative new file path</param>
        /// <param name="overwrite">Is it allowed to overwrite file</param>
        /// <returns>true if file copied; overwise, false.</returns>
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

        /// <summary>
        /// Moves an existing file to specified path. Overwriting if allowed.
        /// </summary>
        /// <param name="filePath">Absolute or relative file to move path</param>
        /// <param name="moveDestPath">Absolute or relative new file path and name(optional)</param>
        /// <param name="overwrite">Is it allowed to overwrite file</param>
        /// <returns>true if file moved correct; overwise, false.</returns>
        public bool moveFile(string filePath, string moveDestPath, bool overwrite = true)
        {
            return copyFile(filePath, moveDestPath, overwrite) && deleteFile(filePath);
        }

        /// <summary>
        /// Creates a new file with specified encoding.
        /// </summary>
        /// <param name="filePath">Absolute or relative path of a new file</param>
        /// <param name="encoding">New file encoding (UTF8, ASCII or UNICODE)</param>
        /// <returns>true if file created; overwise, false.</returns>
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
                UI.PrintLine("Enter your text:");
                sw.WriteLine(Console.ReadLine());
                return true;
            };
            
        }

        /// <summary>
        /// Returns specified file.
        /// </summary>
        /// <param name="filePath">Absolute or relative file path</param>
        /// <returns>Object of FileInfo type</returns>
        public FileInfo GetFile(string filePath)
        {
            filePath = toAbsolutePath(filePath);
            return new FileInfo(filePath);
        }

        /// <summary>
        /// Copies an existing file to a new file. Overwriting if allowed.
        /// </summary>
        /// <param name="filesPath">List of paths to files to concatenate</param>
        /// <returns>true if file concatenate succesfull; overwise, false.</returns>
        public bool concatFiles(string[] filesPath)
        {
            try
            {
                string[] fileLines;
                foreach(var file in filesPath)
                {
                    UI.PrintFileText(new FileInfo(toAbsolutePath(file)));
                }
                return true;
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
