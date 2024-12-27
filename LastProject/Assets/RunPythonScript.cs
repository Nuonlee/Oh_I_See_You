using System.Diagnostics;
using UnityEngine;

public class RunPythonScript : MonoBehaviour
{
    // Python 실행 설정
    public string pythonPath = "py";  // Python 실행 파일 경로 (예: "python" 또는 "py")
    public string scriptPath = "C:\\Users\\user\\nim-clients\\eye-contact\\scripts\\eye-contact.py"; // Python 스크립트 경로
    public string arguments = "--use-ssl --target grpc.nvcf.nvidia.com:443 --function-id 15c6f1a0-3843-4cde-b5bc-803a4966fbb6 --api-key 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 --input \"C:\\Users\\user\\nim-clients\\eye-contact\\assets\\sample_input.mp4\" --output \"C:\\Users\\user\\nim-clients\\eye-contact\\assets\\output0_3.mp4";

    void Start()
    {
        RunPython();
    }

    void RunPython()
    {
        try
        {
            // Process 설정
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonPath, // Python 실행 파일 (예: "python" 또는 "py")
                Arguments = $"{scriptPath} {arguments}", // 스크립트와 인자를 합쳐 전달
                RedirectStandardOutput = true, // Python 출력 읽기
                RedirectStandardError = true,  // Python 에러 읽기
                UseShellExecute = false,       // 셸 실행 비활성화
                CreateNoWindow = true         // 새 창 생성 방지
            };

            // Process 실행
            using (Process process = Process.Start(psi))
            {
                // Python 출력 읽기
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();

                // 결과 출력
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
