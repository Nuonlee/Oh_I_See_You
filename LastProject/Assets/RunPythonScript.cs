using System.Diagnostics;
using UnityEngine;

public class RunPythonScript : MonoBehaviour
{
    // Python ���� ����
    public string pythonPath = "py";  // Python ���� ���� ��� (��: "python" �Ǵ� "py")
    public string scriptPath = "C:\\Users\\user\\nim-clients\\eye-contact\\scripts\\eye-contact.py"; // Python ��ũ��Ʈ ���
    public string arguments = "--use-ssl --target grpc.nvcf.nvidia.com:443 --function-id 15c6f1a0-3843-4cde-b5bc-803a4966fbb6 --api-key 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 --input \"C:\\Users\\user\\nim-clients\\eye-contact\\assets\\sample_input.mp4\" --output \"C:\\Users\\user\\nim-clients\\eye-contact\\assets\\output0_3.mp4";

    void Start()
    {
        RunPython();
    }

    void RunPython()
    {
        try
        {
            // Process ����
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonPath, // Python ���� ���� (��: "python" �Ǵ� "py")
                Arguments = $"{scriptPath} {arguments}", // ��ũ��Ʈ�� ���ڸ� ���� ����
                RedirectStandardOutput = true, // Python ��� �б�
                RedirectStandardError = true,  // Python ���� �б�
                UseShellExecute = false,       // �� ���� ��Ȱ��ȭ
                CreateNoWindow = true         // �� â ���� ����
            };

            // Process ����
            using (Process process = Process.Start(psi))
            {
                // Python ��� �б�
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();

                // ��� ���
                if (!string.IsNullOrEmpty(output))
                    UnityEngine.Debug.Log($"Python Output: {output}");
                if (!string.IsNullOrEmpty(errors))
                    UnityEngine.Debug.LogError($"Python Errors: {errors}");
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Error running Python script: {e.Message}");
        }
    }
}
