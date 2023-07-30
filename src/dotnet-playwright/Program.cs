using Microsoft.Extensions.CommandLineUtils;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Playwright.Tools;

internal class Program
{
    static void Main(string[] args)
    {
        /* 
         * This should only wrap commands to the Microsoft.Playwright.dll
         * get current execution directory of command
         * then find the Microsoft.Playwright.dll file
         * then execute the command with the arguments and return the output from Microsoft.Playwright.dll
         * if the Microsoft.Playwright.dll is not found, then output an error message
         * to the user to run the command from the correct project
         */

        // Find a single Microsoft.Playwright.dll to wrap or error out
        String targetDll = GetPlaywrightCliDll();
        // get the arguments passed to the command
        StringBuilder sb = new StringBuilder();
        sb.Append("dotnet playwright ");
        foreach (var arg in args)
            sb.Append(arg + " ");
        Console.WriteLine(sb.ToString().TrimEnd());
        // Execute the command with the arguments and return the output from Microsoft.Playwright.dll
        CallPlaywrightCliDllWithArgs(targetDll, args);
    }

    private static void CallPlaywrightCliDllWithArgs(string targetDllFilePathway, string[] args)
    {
        /*
         * This is the heart of the program...
         * This creates a process command with the arguments passed in
         * and returns the output from the process - exit
         */
        var app = new CommandLineApplication(throwOnUnexpectedArg: false) { Name = "dotnet playwright" };
        // configure the app to wrap the Microsoft.Playwright.dll with the arguments
        app.OnExecute(() =>
        {
            var startInfo = new ProcessStartInfo(targetDllFilePathway)
            {
                Arguments = string.Join(" ", args),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                StandardOutputEncoding = Encoding.Default,
                StandardErrorEncoding = Encoding.Default,
                StandardInputEncoding = Encoding.Default
            };
            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                    throw new NullReferenceException("Process start did not occur for dotnet-playwright tool wrapping Microsoft.Playwright.dll.");
                // handle output and error output from the process asynchronously
                process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                return process.ExitCode;
            }
        });
        // execute wrapped dll process
        Console.WriteLine("Exit Code:" + app.Execute(args));
    }

    private static string GetPlaywrightCliDll()
    {
        // get currently executing directory
        var currentDirectory = Directory.GetCurrentDirectory();
        // Find 'Microsoft.Playwright.dll' file (ignoring filename cases) by crawling this and subdirectories...
        var playwrightDllFiles = Directory.GetFiles(currentDirectory, "Microsoft.Playwright.dll",
            new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = true });
        // If multiple files exists or no files found... throw an appropriate error for each condition
        if (playwrightDllFiles.Length == 0)
        {
            var lsb = new StringBuilder();
            lsb.AppendLine();
            lsb.AppendLine("ERROR!");
            lsb.AppendLine();
            lsb.AppendLine("Microsoft.Playwright.dll not found in current directory or subdirectories.");
            lsb.AppendLine("    Current directory is: " + currentDirectory);
            lsb.AppendLine("Either build the Playwright project again or move to a directory where the dll can be found.");
            lsb.AppendLine();
            throw new FileNotFoundException(lsb.ToString());
        }
        else if (playwrightDllFiles.Length > 1)
        {
            var lsb = new StringBuilder();
            lsb.AppendLine();
            lsb.AppendLine("ERROR!");
            lsb.AppendLine();
            lsb.AppendLine("Multiple Microsoft.Playwright.dll files found in current directory or subdirectories.");
            lsb.AppendLine("Locations are:");
            foreach (var file in playwrightDllFiles)
                lsb.AppendLine("    " + file);
            lsb.AppendLine();
            lsb.AppendLine("Suggestions for directory navigation are:");
            foreach (var file in playwrightDllFiles)
            {
                // capture in string file minus currentDirectory
                var fileMinusCurrentDirectory = file.Substring(currentDirectory.Length);
                var fileMinusCurrentDirectoryMinusFileName = fileMinusCurrentDirectory.Substring(0, fileMinusCurrentDirectory.Length - "Microsoft.Playwright.dll".Length);
                lsb.AppendLine("    cd ." + fileMinusCurrentDirectoryMinusFileName);
                // if file is not last file in the list, then add an or
                if (file != playwrightDllFiles[playwrightDllFiles.Length - 1])
                    lsb.AppendLine("    or");
            }
            throw new FileLoadException(lsb.ToString());
        }
        return playwrightDllFiles[0];
    }
}
