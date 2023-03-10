using System;
using System.Diagnostics;

// ---- USE THIS TO OPEN PROCESS, A TCP-IP LISTENER IN PYTHON WITH DEFECT DETECTOR


public class Server
{
    public static void makeProcess()
    {
        var psi = new ProcessStartInfo(); // run the python script

        psi.FileName = "C:\\Users\\Administrator\\miniconda3\\envs\\tensorflow-gpu\\python.exe";

        // Provide script 
        var script = "C:\\Users\\Administrator\\source\\repos\\ML_server\\python_tcpip\\pythontcpip.py";

        Console.WriteLine("PREDICTION SEQUENCE - sending test image to python prediction...");

        psi.Arguments = $"\"{script}\" ";

        // process config

        psi.UseShellExecute = false;
        psi.CreateNoWindow = false;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;

        // execute process and get output

        var errors = "NA";

        while (true)
        {
            using (var p = Process.Start(psi))
            {
                errors = p.StandardError.ReadToEnd();
            }
        }
    }

    private static void Main(string[] args)
    {
        Console.WriteLine("building process for python!");
        makeProcess();
    }
}