using System.Diagnostics;
using UnityEngine;

public class eye_contact : MonoBehaviour
{
    // Path to Python executable (adjust this to your environment)
    public string pythonPath = @"C:\Python39\python.exe";

    // Path to the Python script
    public string scriptPath = @"path\to\eye-contact.py";

    // Command-line arguments
    public bool useSSL = true;
    public string target = "grpc.nvcf.nvidia.com:443";
    public string functionID = "15c6f1a0-3843-4cde-b5bc-803a4966fbb6";
    public string apiKey = "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";
    public string inputFilePath = @"path\to\input.mp4";
    public string outputFilePath = @"path\to\output.mp4";

    void Start()
    {
        RunScript();
    }

    public void RunScript()
    {
        // Build the arguments string
        string arguments = $"{scriptPath}";

        if (useSSL)
        {
            arguments += " --use-ssl";
        }

        arguments += $" --target {target}";
        arguments += $" --function-id {functionID}";
        arguments += $" --api-key {apiKey}";
        arguments += $" --input {inputFilePath}";
        arguments += $" --output {outputFilePath}";

        // Start the process
        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = Process.Start(start))
            {
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    UnityEngine.Debug.Log("Python Output: " + output);
                    if (!string.IsNullOrEmpty(errors))
                    {
                        UnityEngine.Debug.LogError("Python Errors: " + errors);
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("An error occurred while running the Python script: " + e.Message);
        }
    }
}
