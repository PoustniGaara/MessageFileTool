using FileOperations;
using System;

namespace ConsoleApp
{
    static class ConsoleApp
    {
        private static string lastUsedPath;

        static void Main(string[] args)
        {
            InspectArgumentsAndPerformActions(args);
        }

        private static void InspectArgumentsAndPerformActions(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    WriteInstructions();
                    break;
                case 2:
                    PerformActions(args[0], args[1]);
                    break;
                default:
                    Console.WriteLine("Only two arguments are supported");
                    WriteInstructions();
                    break;
            }
        }

        private static void PerformActions(string rootPath, string operation)
        {    
            // Check if the rootPath exists
            if (FileManager.CheckIfFolderExists(rootPath))
            {
                // Save path for auto start
                lastUsedPath = rootPath;

                switch (operation)
                {
                    case "printmessages":
                        String printMessageRespond = FileManager.PerformPrintMessageOperation(rootPath);
                        Console.WriteLine(printMessageRespond);
                        Console.WriteLine("\n Next print of messages after 1 hour");
                        AutoPrintMessages();
                        break;
                    case "cleanup":
                        String printMessageRespond2 = FileManager.PerformCleanupOperation(rootPath);
                        Console.WriteLine(printMessageRespond2);
                        break;
                    case "sort":
                        String printMessageRespond3 = FileManager.PerformSortOperation(rootPath);
                        Console.WriteLine(printMessageRespond3);
                        break;
                    // If operation inputed by user was invalid
                    default:
                        Console.WriteLine("\nInvalid operation\n ");
                        WriteInstructions();
                        break;
                }
            }
            // If the rootPath doesnt exists
            else
            {
                Console.WriteLine("\nThe message folder path does not exists\n");
                WriteInstructions();
                return;
            }
            

        }

        private static void AutoPrintMessages()
        {
            Thread.Sleep(1000*60*60); // One Hour
            string[] arguments = { lastUsedPath, "printmessages" };
            Main(arguments);
        }

        private static void WriteInstructions()
        {
            Console.WriteLine("The console application accepts two arguments: \n" +
                "MessageHelper.exe [messagefolder path] [operation] \n " +
                "e.g. MessageHelper.exe c:\\temp\\messages printmessages \n" +
                "The message folder is inspected by the program and based on the operation argument, actions are performed on files in the root folder." +
                "Operations available: printmessages, cleanup, sort");
        }
    }

}


