using System.Collections;
using System.Runtime;
using System.Text;
using System.Xml.Linq;


namespace FileOperations
{
    public static class FileManager
    {

        static StringBuilder log = new StringBuilder();
        static StringBuilder message =  new StringBuilder();
        static char[] subFolderNamesForSortOperation = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
            'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 
            's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };


        public static bool CheckIfFolderExists(string pathString)
        {
            if (Directory.Exists(pathString)) return true;
            else return false;
        }

        public static string PerformPrintMessageOperation(string rootPath)
        {   
            // Clear strings from previous calls
            message.Clear();
            log.Clear();

            WalkDirectoryTreeToPrint(new System.IO.DirectoryInfo(rootPath));

            // Merge error messages and messages
            message.Append("\n\nERROR MESSAGES:\n");

            if (message.Length > 0) return message.ToString();
            else return "No messages";

        }


        public static string PerformCleanupOperation(string rootPath)
        {
            log.Clear();
            string pathString = System.IO.Path.Combine(rootPath, "_backup");
            System.IO.DirectoryInfo root = new System.IO.DirectoryInfo(rootPath);

            if (!System.IO.File.Exists(pathString))
            {
                System.IO.Directory.CreateDirectory(pathString);
                WalkDirectoryTreeToCleanup(root, rootPath);
            }
            else
            {
                WalkDirectoryTreeToCleanup(root ,rootPath);
            }

            return $"\nCleanup done!\nERROR MESSAGES: {log}";
        }

        public static string PerformSortOperation(string rootPath)
        {
            log.Clear();
            CreateSubFoldersForSortOperation(rootPath);
            System.IO.DirectoryInfo root = new System.IO.DirectoryInfo(rootPath);
            WalkDirectoryTreeToSort(root, rootPath);
            return $"\nSorting done!\nERROR MESSAGES: {log}";
        }

        private static void CreateSubFoldersForSortOperation(string rootPath)
        {
            string pathString = System.IO.Path.Combine(rootPath, "sorted");
            char[] subFoldersTocreate = subFolderNamesForSortOperation;

            // Check for sorted directory
            if (!System.IO.File.Exists(pathString))
            {
                System.IO.Directory.CreateDirectory(pathString);
            }

            // Check if there are sub dirs of sorted
            System.IO.DirectoryInfo sortedDirInfo = new System.IO.DirectoryInfo(pathString);
            System.IO.DirectoryInfo[] subDirsOfSorted = null;
            subDirsOfSorted = sortedDirInfo.GetDirectories();

            // Loop through sorted dir and test if all sub-dir are present, remove present from an array of names
            foreach (char name in subFolderNamesForSortOperation)
            {
                foreach(System.IO.DirectoryInfo dirInfo in subDirsOfSorted) 
                {
                    if (dirInfo.Name == name.ToString()) 
                    {
                        subFolderNamesForSortOperation = subFolderNamesForSortOperation.Where(val => val != name).ToArray();
                    }
                }
            }
            // Create folder from rest of missing names
            foreach (char name in subFolderNamesForSortOperation)
            {
                string dirName = System.IO.Path.Combine(pathString, name.ToString());
                System.IO.Directory.CreateDirectory(dirName);
            }
        }

        private static void WalkDirectoryTreeToSort(System.IO.DirectoryInfo root, string rootPath)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the backup files directly under this folder
            try
            {
                if (root.Name != "sorted")
                    files = root.GetFiles("*.backup");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                log.Append(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                // add to message
                log.Append(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    string destinationFolderName = fi.Name.Substring(0, 1).ToLower();

                    string sourceFile = @fi.FullName;
                    string destinationFile = $@"{rootPath}\sorted\{destinationFolderName}\{fi.Name}";

                    Console.WriteLine(destinationFile);

                    // To move a file or folder to a new location:
                    System.IO.File.Move(sourceFile, destinationFile);


                }
                // Now find all the subdirectories under this directory.
                if (root.Name != "sorted")
                    subDirs = root.GetDirectories();
                
                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    // Program do not sort already sorted files
                    WalkDirectoryTreeToSort(dirInfo, rootPath);
                }
            }
        }

        private static void WalkDirectoryTreeToCleanup(System.IO.DirectoryInfo root, string rootPath)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the backup files directly under this folder
            try
            {
                files = root.GetFiles("*.backup");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                log.Append(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                // add to message
                log.Append(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    string sourceFile = @fi.FullName;
                    string destinationFile = $@"{rootPath}\_backup\{fi.Name}";

                    // To move a file or folder to a new location:
                    System.IO.File.Move(sourceFile, destinationFile);


                }
                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTreeToCleanup(dirInfo, rootPath);
                }
            }
        }

        private static void WalkDirectoryTreeToPrint(System.IO.DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the txt files directly under this folder
            try
            {
                files = root.GetFiles("*.txt");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                log.Append(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {   
                // add to message
                log.Append(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    try
                    {
                        // Get the name of file and content
                        message.Append($"\n {Path.GetFileNameWithoutExtension(fi.Name).ToUpper()}");
                        message.Append("\n.\n");
                        message.Append(".\n");
                        message.Append(".\n");
                        message.Append("\n");
                        string text = System.IO.File.ReadAllText(fi.FullName);
                        message.Append(text);

                        // change extention to .backup
                        fi.MoveTo(Path.ChangeExtension(fi.FullName, ".backup"));
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        log.Append(e.Message);
                    }
                }

            }
                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTreeToPrint(dirInfo);
                }
            }
        }
    }
