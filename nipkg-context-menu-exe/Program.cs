using System;
using System.Diagnostics;
using System.Text.Json;
using System.Reflection;
using System.Text.Json.Serialization;
using System.IO;


namespace Buildnipkg
{
    class Program
    {
        static string BuildCommandFromConfig(Config config)
        {
            var cmd = "cd " + config.nipkg_location + " && nipkg pack " + '\u0022' + config.source_location + '\u0022' + " "+  '\u0022'+ config.built_package_destination+ '\u0022';
            return cmd;
        }

        static string BuildFromArgsAndConfigCommand(string sourceLocationPath, Config config)
        {
            string cmd = string.Empty;
            if (Directory.Exists(sourceLocationPath))
            {
                cmd = "cd " + config.nipkg_location + " && nipkg pack " + '\u0022' + sourceLocationPath + '\u0022' + " " + '\u0022' + config.built_package_destination + '\u0022';
            }
            else
            {
                Console.WriteLine($"ERROR from nipkg-extension. PATH: {sourceLocationPath} does not exist.");
                Console.ReadKey();
            }
            
            return cmd;
        }

        static string BuildFromArgs(string[] args, Config config)
        {
            
            string sourceLocationPath = args[0];
            int lastFolderPosition = args[0].LastIndexOf("\\");
            string builtPackageDestination = sourceLocationPath.Substring(0,lastFolderPosition);
            string cmd = string.Empty;
            if (Directory.Exists(sourceLocationPath) && Directory.Exists(builtPackageDestination) && args[1] == "--here")
            {
                cmd = "cd " + config.nipkg_location + " && nipkg pack " + '\u0022' + sourceLocationPath + '\u0022' + " " + '\u0022' + builtPackageDestination + '\u0022';
            }
            else
            {
                if(args[1] != "--here")
                {
                    Console.WriteLine($"Unknown argument {args[1]} passed in. If two arguments passed, they must be first must be sourceLocationPath second must be --here");
                }
                else
                {
                    Console.WriteLine($"ERROR from nipkg-extension. PATH: {sourceLocationPath} or PATH: {builtPackageDestination} does not exist.");
                    Console.ReadKey();
                }
            }

            return cmd;
        }

        static void CheckOrBuildDestinationDirectory(Config c)
        {
            if (Directory.Exists(c.built_package_destination))
            {
                //do nothing
            }
            else if (c.built_package_destination == string.Empty)
            {
                //build in calling directory
                c.built_package_destination = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                //see https://stackoverflow.com/questions/422090/in-c-sharp-check-that-filename-is-possibly-valid-not-that-it-exists
                FileInfo fi = null;
                try
                {
                    fi = new FileInfo(c.built_package_destination);
                }
                catch (ArgumentException) { }
                catch (System.IO.PathTooLongException) { }
                catch (NotSupportedException) { }
                if (ReferenceEquals(fi, null))
                {
                    //file name is not valid
                }
                else
                {
                    //create directory
                    Directory.CreateDirectory(c.built_package_destination);
                }
            }
        }

        static void Main(string[] args)
        {
            string configFilePath = Assembly.GetEntryAssembly().Location.Replace("nipkg-context-menu.dll","") + "config.json";
            var jsonString = File.ReadAllText(configFilePath);
            var config = JsonSerializer.Deserialize<Config>(jsonString);
            CheckOrBuildDestinationDirectory(config);
            string command = string.Empty;
            if (args.Length == 0)
            {
                command = BuildCommandFromConfig(config);
            }
            else if (args.Length == 1)
            {
                command = BuildFromArgsAndConfigCommand(args[0], config);
            }
            else if (args.Length == 2)
            {
                command = BuildFromArgs(args, config);
            }
            else
            {
                Console.WriteLine("Unknown arguments passed. Exe takes at most two [source path] [empty or --here]. Args passed: " + string.Join(" ", args));
            }

            var proc = new ProcessStartInfo();
            //proc1.UseShellExecute = true; //this line enabled hides cmd window
            proc.WorkingDirectory = @"C:\Windows\System32";
            proc.FileName = @"C:\Windows\System32\cmd.exe";
            proc.Verb = "runas";
            proc.Arguments = "/c " + command;
            if(command != string.Empty)
            {
                Process.Start(proc);
            }
        }
    }
}
