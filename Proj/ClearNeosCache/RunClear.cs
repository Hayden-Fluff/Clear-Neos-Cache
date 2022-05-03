using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using CommandLine;


namespace ClearNeosCache
{
    public class RunClear
    {
        public class Options
        {

            [Option('a', "age", Required = true, HelpText = "<int> How long to look back for files to remove since last read")]
            public int FileAge { get; set; }
            [Option('d', "directoy", Required = false, HelpText = "[(Optional) <string> Overrides the directory to clean, otherwise default is used")]           
            public string TargetDirectory { get; set; }
        }

        public static readonly string defaultDirectory = System.IO.Path.GetTempPath() + @"Solirax\NeosVR\Cache";
        public static string runningDirectory = defaultDirectory;
        public static int fileAge;
        public static bool exceptionThrown;

        static void Main(string[] args)
        {

            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        static void RunOptions(Options opts)
        {
            if (opts.FileAge < 0)
            {
                RunClear.HandleException("valueOutOfRange");
            }

            else
            {
                fileAge = opts.FileAge;
            }

            if (!String.IsNullOrEmpty(opts.TargetDirectory))
            {
                //targetDirectory = opts.TargetDirectory;
                runningDirectory = opts.TargetDirectory;
                if (!Directory.Exists(opts.TargetDirectory))
                {
                    RunClear.HandleException("invalidDirectory");
                }
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                RunClear.HandleException("unsupportedOS");
            }

            if (exceptionThrown == false)
            {
            //  Console.WriteLine(opts.FileAge + "\n");
            //  Console.WriteLine(runningDirectory + "\n");
                RunClear.RunOpperation();
            }
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("\nUsage: {0} -a --age <int> [-d --directory <string>]\n", System.Diagnostics.Process.GetCurrentProcess().ProcessName);
        }

        public static DateTime TimeDelta(int delta)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeDelta = new TimeSpan(delta, 0, 0, 0);
            DateTime targetTime = currentTime - timeDelta;

            return targetTime;
        }

        public DateTime LastAccessTime { get; }

        public static int HandleException(string exceptionName)
        {
            Console.WriteLine("Exception: " + exceptionName);

            if (exceptionName == "valueOutOfRange")
            {
                Console.WriteLine("Error: The given value is out of range. Value must be greater than 0\n");
            }

            else if (exceptionName == "invalidDirectory")
            {
                Console.WriteLine("Error: {0}{1}{0} is not a valid folder path.\n", @"'", runningDirectory);
            }

            else if (exceptionName == "unsupportedOS")
            {
                Console.WriteLine("Error: The current OS is unsupported, must be using a Windows opperating system.");
            }

            else if (exceptionName.Contains("fileAccessException"))
            {
                if (exceptionName.Contains("security"))
                {
                    Console.WriteLine("Error: Current user does not have access remove the targeted file.");
                }
                
                else if (exceptionName.Contains("fileInUse"))
                {
                    Console.WriteLine("Error: Current file is in use, cannot delete");
                }

                else if (exceptionName.Contains("isNotFile"))
                {
                    Console.WriteLine("Error: The targeted file is a directory and not a file");
                }

                else
                {
                    Console.WriteLine("Error: Unhandled Exception");
                }
            }
            exceptionThrown = true;
            return 3;
        }

        public static long totalFileSize;
        public static long totalNumberOfFiles;
        public static long deletedFiles;

        public static int RunOpperation()
        {
            string[] fileList = Directory.GetFiles(runningDirectory);
            DateTime startTime = DateTime.Now;

            foreach (string File in fileList)
            {
                FileSystemInfo file = new FileInfo(File);
                FileInfo fileInfo = new FileInfo(File);

             //   Console.WriteLine("Processing file {0}", File);
                totalNumberOfFiles++;
           
                if (file.LastAccessTime < TimeDelta(fileAge))
                {
                 //   Console.WriteLine("File is Valid");
                    
                    try
                    {
                        totalFileSize += fileInfo.Length;
                        FileInfo currentFile = new FileInfo(Path.Combine(runningDirectory, File));
                        @currentFile.Delete();
                        Console.WriteLine("Removed {0}", File);
                        deletedFiles++;
                    }
                 
                    catch (Exception ex)
                    {
                  
                        if (ex.ToString() == "SecurityException")
                        {
                            HandleException("fileAccessException.secutiry");
                        }
                  
                        else if (ex.ToString() == "IOException")
                        {
                            HandleException("fileAccessException.fileInUse");
                        }
                  
                        else if (ex.ToString() == "UnauthorizedAccessException")
                        {
                            HandleException("fileAccessException.isNotFile");
                        }
                  
                        else
                        {
                            HandleException("fileAccessException: " + ex);
                        }
                    }
                }
            }

            DateTime endTime = DateTime.Now;
            TimeSpan totalTime = endTime - startTime;

            Console.WriteLine("\nOpperation completed in {0} hours, {1:mm} minutes, {1:ss} seconds\nprocessed {3} files\nRemoved {2} bytes of data in {4} files", Math.Floor(totalTime.TotalHours), totalTime, BytesToString(totalFileSize), totalNumberOfFiles, deletedFiles);
         //   Console.WriteLine("\n{0}\n\n{1}\n{2}\n", runningDirectory, fileAge, TimeDelta(fileAge));
            return 1;

            }
         // https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
            public static string BytesToString(long byteCount)
            {
                //Longs run out around EB
                string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; 
                if (byteCount == 0)
                    return "0" + suf[0];
                long bytes = Math.Abs(byteCount);
                int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                double num = Math.Round(bytes / Math.Pow(1024, place), 1);
                return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}
