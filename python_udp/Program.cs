using System;
using System.Diagnostics;
// ---- USE THIS TO OPEN PROCESS THAT IS A UDP LISTENER IN PYTHON


public class Program
{
    public static void makeProcess()
    {
        var psi = new ProcessStartInfo(); // run the python script

        psi.FileName = "C:\\Users\\Administrator\\miniconda3\\envs\\tensorflow-gpu\\python.exe";

        // Provide script 
        var script = "C:\\Users\\Administrator\\source\\repos\\ML_server\\python_udp\\pythonudp.py";
        //var defects = ML.defectsThreadCSVpath;
        //var mlModelPath = ML.mlModelPath;

        //if (isNose) { defects = ML.defectsNoseCSVpath; }

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

            if (errors != "NA")
            {
                string[] parsedout = errors.Split("*");
                Console.WriteLine($"{parsedout[1]}");
            }
        }
    }

    private static void Main(string[] args)
    {
        Console.WriteLine("building process for python!");
        makeProcess();
    }
}