using System;
using System.Diagnostics;


// ---- USE THIS TO OPEN PROCESS THAT IS A UDP LISTENER IN PYTHON
public class Program
{

    public static void makeProcess()
    {

        //System.Windows.MessageBox.Show("running ML defect detector. current wait is 40-60seconds.");

        var psi = new ProcessStartInfo(); // run the python script

        psi.FileName = "C:\\Users\\Administrator\\miniconda3\\envs\\tensorflow-gpu\\python.exe";

        // Provide script and arguments
        var script = "C:\\Users\\Administrator\\source\\repos\\ML_server\\python_udp\\pythonudp.py";
        //var defects = ML.defectsThreadCSVpath;
        //var mlModelPath = ML.mlModelPath;

        //if (isNose) { defects = ML.defectsNoseCSVpath; }

        Console.WriteLine("PREDICTION SEQUENCE - sending test image to python prediction...");

        //psi.Arguments = $"\"{script}\" \"{imagePath}\" \"{mlModelPath}\" \"{defects}\"";
        psi.Arguments = $"\"{script}\" ";

        // process config

        psi.UseShellExecute = false;
        psi.CreateNoWindow = false;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;

        // execute process and get output

        var errors = "NA";
        var results = "NA";

        while (true)
        {
            using (var process = Process.Start(psi))
            {
                errors = process.StandardError.ReadToEnd();
                results = process.StandardOutput.ReadToEnd();
            }

            //string[] rawMessage = errors.Split("b");

            if (errors != "NA")
            {
                string output = errors;
                Console.WriteLine($"{output}");
            }

        }

       


    }

    private static void Main(string[] args)
    {
        Console.WriteLine("building process for python!");
        makeProcess();
    }
}