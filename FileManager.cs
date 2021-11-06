using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public FileInfo[] GetFiles(string searchPattern = "*.*")
        {
            return workingDirectory.GetFiles(searchPattern);
        }

        public DirectoryInfo[] GetDirectories(string searchPattern = "*")
        {
            return workingDirectory.GetDirectories(searchPattern);
        }

        public FileSystemInfo[] GetContent()
        {
            return workingDirectory.GetFileSystemInfos();
        }

        public DirectoryInfo getWorkingDirectory()
        {
            return workingDirectory;
        }

        public bool selectDisk()
        {
            Console.WriteLine("Select disk:");
            int i = 1;
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                UI.PrintLine((i++)+". " + drive.Name);
            }
            UI.Print("Type number of disk: ");
            int selection = Int32.Parse(Console.ReadLine());
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
            File.Delete(filePath);
            return true;
        }

        public bool copyFile(string filePath, string copyDestPath, bool overwrite = true)
        {
            filePath = toAbsolutePath(filePath);
            copyDestPath = toAbsolutePath(copyDestPath);
            if (Path.GetExtension(copyDestPath) == "")
                copyDestPath += "/" + Path.GetFileName(filePath);
            File.Copy(filePath, copyDestPath, overwrite);
            return true;
        }

        public bool moveFile(string filePath, string copyDestPath, bool overwrite = true)
        {
            return copyFile(filePath, copyDestPath, overwrite) && deleteFile(filePath);
        }

        public bool createFile(string filePath, string encoding = "UTF-8")
        {
            filePath = toAbsolutePath(filePath);
            using (StreamWriter sw = new StreamWriter(File.Open(filePath, FileMode.Create), Encoding.GetEncoding(encoding))) {
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
