using System.Diagnostics;
using Unity;
using UnityEngine;
using System.Threading.Tasks;
using Tobii.GameIntegration.Net;
using System;
using System.Runtime.InteropServices;

public class CallPython : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    public string pythonPath = "py";  // Python 실행 경로
    public string splitScript = "C:/Users/user/nim-clients/eye-contact/scripts/split_video.py";
    public string processScript = "C:/Users/user/nim-clients/eye-contact/scripts/custom_multiprocessing.py";
    public string mergeScript = "C:/Users/user/nim-clients/eye-contact/scripts/merge_videos.py";

    private HeadPose head;

    public float yawLow = 10.0f;
    public float yawHigh = 20.0f;
    public float pitchLow = 15.0f;
    public float pitchHigh = 25.0f;

    async void Start()
    {
        // Tobii 초기화
        TobiiGameIntegrationApi.Update();
        IntPtr handle = GetForegroundWindow();
        TobiiGameIntegrationApi.TrackWindow(handle);
        TobiiGameIntegrationApi.UpdateTrackerInfos();

        // HeadPose 가져오기
        if (TobiiGameIntegrationApi.TryGetLatestHeadPose(out head))
        {
            // Yaw 제한 범위 조정
            head.Rotation.YawDegrees = Mathf.Clamp(head.Rotation.YawDegrees, 12.5f, 32.5f);

            // Pitch 제한 범위 조정
            head.Rotation.PitchDegrees = Mathf.Clamp(head.Rotation.PitchDegrees, 12.5f, 32.5f);

            yawLow = head.Rotation.YawDegrees - 2.5f;
            yawHigh = head.Rotation.YawDegrees - 2.5f;
            pitchLow = head.Rotation.PitchDegrees -2.5f;
            pitchHigh = head.Rotation.PitchDegrees - 2.5f; ;
        }

        // Python 스크립트 실행
        await RunPythonScriptAsync(splitScript, "");
        UnityEngine.Debug.Log("Splitting completed.");

        string processArgs = $"--yaw-low {yawLow} --yaw-high {yawHigh} --pitch-low {pitchLow} --pitch-high {pitchHigh}";
        await RunPythonScriptAsync(processScript, processArgs);
        UnityEngine.Debug.Log("Processing completed.");

        await RunPythonScriptAsync(mergeScript, "");
        UnityEngine.Debug.Log("Merging completed.");
    }

    async Task RunPythonScriptAsync(string script, string args)
    {
        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{script}\" {args}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(start))
        {
            string output = await Task.Run(() => process.StandardOutput.ReadToEnd());
            string error = await Task.Run(() => process.StandardError.ReadToEnd());

            if (!string.IsNullOrEmpty(output))
                UnityEngine.Debug.Log(output);

            if (!string.IsNullOrEmpty(error))
                UnityEngine.Debug.LogError(error);
        }
    }
}

