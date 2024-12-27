using UnityEngine;  // UnityEngine을 추가해야 Debug.LogError가 작동합니다.
using UnityEngine.UI; // UI 및 RawImage를 위해 필요
using UnityEngine.Video;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tobii.GameIntegration.Net;
using System.Threading.Tasks;

public class Data : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    public RectTransform canvas; // 캔버스 RectTransform
    public RectTransform gazeIndicator; // 시선을 표시할 UI 요소
    public GameObject targetUI_cor; // 시선과 상호작용할 UI
    public RectTransform targetUI;
    public RectTransform targetUI_confirmed;
    public RectTransform targetUI2;
    public RectTransform targetUI2_confirmed;
    public VideoPlayer videoPlayer; // VideoPlayer 컴포넌트
    public VideoPlayer originPlayer; // 변경할 비디오 클립
    public GameObject gazeMarker; // 시선 위치를 표시할 오브젝트
    public GameObject samorigin; // 시선 위치를 표시할 오브젝트
    public GameObject myorigin; // 시선 위치를 표시할 오브젝트
    public GameObject Title;
    public GameObject processing;
    private GazePoint gazePoint;
    public GameObject target;

    // 27인치 모니터의 가정 크기 및 거리
    private const float monitorWidth = 59.4f; // cm
    private const float monitorHeight = 33.5f; // cm
    private const float viewerDistance = 20.0f; // cm

    public Vector2 world = new Vector2(0, 0);
    void Start()
    {
        // 프로그램 실행 시 자동으로 Python 스크립트를 실행
        videoPlayer.Stop();
        originPlayer.Stop();
        samorigin.SetActive(false);
        myorigin.SetActive(false);
    }

    void Update()
    {
        // Tobii API 업데이트
        TobiiGameIntegrationApi.Update();
        IntPtr handle = GetForegroundWindow();
        TobiiGameIntegrationApi.TrackWindow(handle);
        TobiiGameIntegrationApi.UpdateTrackerInfos();

        TobiiGameIntegrationApi.TryGetLatestGazePoint(out gazePoint);
        // 최신 시선 데이터 가져오기
        UpdateGazeIndicator();
        UpdateGazeMarker();
        CheckGazeOnUI();

        Vector2 uicord = new Vector2(targetUI.transform.position.x - gazeMarker.transform.position.x, targetUI.transform.position.y - gazeMarker.transform.position.y);
        // 비디오 변경을 위한 입력 확인 (디버깅용)
        if (Vector2.SqrMagnitude(uicord) < 250)
        {
            //targetUI_cor.SetActive(false);
        }
        else
        {
            //targetUI_cor.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            samorigin.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            myorigin.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            samorigin.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            myorigin.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(1, 1);
            ProcessRegion(1, 1); // 좌측 상단
            target.transform.localPosition = new Vector3(-877, 307);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(1, 2);
            ProcessRegion(1, 2); // 좌측 상단
            target.transform.localPosition = new Vector3(-237, 307);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(1, 3);
            ProcessRegion(1, 3); // 좌측 상단
            target.transform.localPosition = new Vector3(417, 307);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(2, 1);
            ProcessRegion(2, 1); // 좌측 상단
            target.transform.localPosition = new Vector3(-877, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(2, 2);
            ProcessRegion(2, 2); // 좌측 상단
            target.transform.localPosition = new Vector3(-237, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(2, 3);
            ProcessRegion(2, 3); // 좌측 상단
            target.transform.localPosition = new Vector3(417, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(3, 1);
            ProcessRegion(3, 1); // 좌측 상단
            target.transform.localPosition = new Vector3(-877, -307);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(3, 2);
            ProcessRegion(3, 2); // 좌측 상단
            target.transform.localPosition = new Vector3(-237, -307);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(3, 3);
            ProcessRegion(3, 3); // 좌측 상단
            target.transform.localPosition = new Vector3(417, -307);
        }
    }

    void ProcessRegion(int row, int col)
    {
        // yaw와 pitch 계산
        Vector2 yawPitch = CalculateYawPitchForRegion(row, col);
        RunPythonScript(@"D:\Github\LastProject\Assets\VIdeo\input\sample_input.mp4",
                 @"D:/Github/LastProject/Assets/VIdeo/output/final_output.mp4",
                 @"D:\Github\LastProject\nim-clients\eye-contact\scripts\split_video.py",
                 @"D:\Github\LastProject\nim-clients\eye-contact\scripts\custom_multiprocessing.py",
                 @"D:\Github\LastProject\nim-clients\eye-contact\scripts\merge_videos.py",
                 yawPitch.x, yawPitch.y);
    }
    Vector2 CalculateYawPitchForRegion(int row, int col)
    {
        // 9등분 기준 좌표
        float cellWidth = monitorWidth / 3;
        float cellHeight = monitorHeight / 3;

        // 영역의 중심 좌표 (cm)
        float targetX = (col - 1) * cellWidth - monitorWidth / 2 + cellWidth / 2;
        float targetY = monitorHeight / 2 - (row - 1) * cellHeight - cellHeight / 2;

        // 3D 벡터로 변환
        Vector3 targetVector = new Vector3(targetX, targetY, -viewerDistance);
        targetVector.Normalize();

        // yaw (수평 회전) 및 pitch (수직 회전) 계산
        float yaw = Mathf.Atan2(targetVector.x, -targetVector.z) * Mathf.Rad2Deg;
        float pitch = Mathf.Asin(targetVector.y) * Mathf.Rad2Deg;

        return new Vector2(yaw, pitch);
    }
    void UpdateGazeIndicator()
    {
        if (canvas == null || gazeIndicator == null) return;

        // 캔버스의 RectTransform 가져오기
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // 캔버스 크기와 중심 위치를 기준으로 좌표 변환
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // Tobii의 정규화된 좌표를 캔버스 공간으로 변환
        Vector2 localPosition = new Vector2(
            (gazePoint.X - 0.5f) * canvasWidth,
            (gazePoint.Y - 0.5f) * canvasHeight
        );

        // Gaze Indicator의 위치 갱신
        gazeIndicator.anchoredPosition = localPosition;
    }

    void UpdateGazeMarker()
    {
        if (gazeMarker == null) return;

        // Tobii 좌표를 화면 좌표로 변환
        Vector2 screenPosition = new Vector2(
            gazePoint.X * Screen.width,
            gazePoint.Y * Screen.height
        );

        // 화면 좌표를 캔버스 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas,
            screenPosition,
            Camera.main,
            out Vector2 localCanvasPosition
        );

        // Gaze Marker 위치 갱신
        gazeMarker.transform.localPosition = new Vector3(gazePoint.X * 10.0f + 497, gazePoint.Y * 10.0f + 260, -427);
    }

    void CheckGazeOnUI()
    {
        Vector2 screenPosition = new Vector2(gazePoint.X * Screen.width, gazePoint.Y * Screen.height);

 //       UnityEngine.Debug.Log("point " + screenPosition);
        // 시선이 비디오 영역 내에 있을 경우 비디오 변경
        if (screenPosition.x < (300 + (world.y - 2)*700)&& screenPosition.x > (-400 + (world.y - 2) * 700))
        {
            if (screenPosition.y < (350 - (world.x - 2) * 220) && screenPosition.y > (-130 - (world.x - 2) * 220))
            {
                UnityEngine.Debug.Log("touched");
                samorigin.SetActive(false);
            }
            else
            {
                samorigin.SetActive(true);
            }
        }
        else
        {
            samorigin.SetActive(true);
        }

    }

    void ChangeVideoClip()
    {
        videoPlayer.url = @"D:\Github\LastProject\Assets\VIdeo\output\final_output.mp4";
        videoPlayer.Play();
        originPlayer.Play();
        UnityEngine.Debug.Log("비디오 클립이 변경되었습니다.");
    }
    async void RunPythonScript(string inputDir, string outputDir, string splitScriptPath, string processScriptPath, string mergeScriptPath, float yaw, float pitch)
    {
        Title.SetActive(false);
        processing.SetActive(true);
        UnityEngine.Debug.Log("Process start");

        try
        {
            string pythonExecutable = @"C:\Users\user\AppData\Local\Programs\Python\Python312\python.exe";  // Python 경로

            // 첫 번째 스크립트: 비디오 분할 (split_video.py)
            string argumentsForSplitting = $"\"{splitScriptPath}\" --input-dir \"{inputDir}\" --output-dir \"{outputDir}\"";
            ProcessStartInfo startInfoForSplitting = new ProcessStartInfo
            {
                FileName = pythonExecutable,
                Arguments = argumentsForSplitting,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfoForSplitting })
            {
                process.Start();

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                var timeoutTask = Task.Delay(TimeSpan.FromMinutes(5));
                var completedTask = await Task.WhenAny(Task.Run(() => process.WaitForExit()), timeoutTask);

                if (completedTask == timeoutTask)
                {
                    process.Kill();
                    throw new Exception("Python script timed out.");
                }

                string output = await outputTask;
                string error = await errorTask;

                if (!string.IsNullOrEmpty(output))
                    UnityEngine.Debug.Log($"Python Script Output: {output}");
                if (!string.IsNullOrEmpty(error))
                    UnityEngine.Debug.LogError($"Python Script Error: {error}");

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Python script exited with code {process.ExitCode}. See error log for details.");
                }
            }

            UnityEngine.Debug.Log("Video splitting completed");

            // 두 번째 스크립트: 비디오 처리 (process_video.py)
            string argumentsForProcessing = $"\"{processScriptPath}\" --input \"{inputDir}\" --output \"{outputDir}\" --yaw {yaw} --pitch {pitch}";
            ProcessStartInfo startInfoForProcessing = new ProcessStartInfo
            {
                FileName = pythonExecutable,
                Arguments = argumentsForProcessing,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfoForProcessing })
            {
                process.Start();

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                var timeoutTask = Task.Delay(TimeSpan.FromMinutes(5));
                var completedTask = await Task.WhenAny(Task.Run(() => process.WaitForExit()), timeoutTask);

                if (completedTask == timeoutTask)
                {
                    process.Kill();
                    throw new Exception("Python script timed out.");
                }

                string output = await outputTask;
                string error = await errorTask;

                if (!string.IsNullOrEmpty(output))
                    UnityEngine.Debug.Log($"Python Script Output: {output}");
                if (!string.IsNullOrEmpty(error))
                    UnityEngine.Debug.LogError($"Python Script Error: {error}");

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Python script exited with code {process.ExitCode}. See error log for details.");
                }
            }

            UnityEngine.Debug.Log("Video processing completed");

            // 세 번째 스크립트: 비디오 병합 (merge_videos_opencv.py)
            string tempAvi = @"D:/Github/LastProject/Assets/VIdeo/output/temp_merged.avi";
            string finalOutput = @"D:/Github/LastProject/Assets/VIdeo/output/final_output.mp4";

            string argumentsForMerging = $"\"{mergeScriptPath}\" --input-dir \"{outputDir}\" --temp-output \"{tempAvi}\" --final-output \"{finalOutput}\"";
            ProcessStartInfo startInfoForMerging = new ProcessStartInfo
            {
                FileName = pythonExecutable,
                Arguments = argumentsForMerging,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfoForMerging })
            {
                process.Start();

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                var timeoutTask = Task.Delay(TimeSpan.FromMinutes(5));
                var completedTask = await Task.WhenAny(Task.Run(() => process.WaitForExit()), timeoutTask);

                if (completedTask == timeoutTask)
                {
                    process.Kill();
                    throw new Exception("Python script timed out.");
                }

                string output = await outputTask;
                string error = await errorTask;

                if (!string.IsNullOrEmpty(output))
                    UnityEngine.Debug.Log($"Python Script Output: {output}");
                if (!string.IsNullOrEmpty(error))
                    UnityEngine.Debug.LogError($"Python Script Error: {error}");

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Python script exited with code {process.ExitCode}. See error log for details.");
                }
            }

            UnityEngine.Debug.Log("Video merging completed");
            processing.SetActive(false);
            samorigin.SetActive(true);
            myorigin.SetActive(true);
            ChangeVideoClip();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Python script execution failed: {ex.Message}");
        }
    }
}

