using UnityEngine;  // UnityEngine�� �߰��ؾ� Debug.LogError�� �۵��մϴ�.
using UnityEngine.UI; // UI �� RawImage�� ���� �ʿ�
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

    public RectTransform canvas; // ĵ���� RectTransform
    public RectTransform gazeIndicator; // �ü��� ǥ���� UI ���
    public GameObject targetUI_cor; // �ü��� ��ȣ�ۿ��� UI
    public RectTransform targetUI;
    public RectTransform targetUI_confirmed;
    public RectTransform targetUI2;
    public RectTransform targetUI2_confirmed;
    public VideoPlayer videoPlayer; // VideoPlayer ������Ʈ
    public VideoPlayer originPlayer; // ������ ���� Ŭ��
    public GameObject gazeMarker; // �ü� ��ġ�� ǥ���� ������Ʈ
    public GameObject samorigin; // �ü� ��ġ�� ǥ���� ������Ʈ
    public GameObject myorigin; // �ü� ��ġ�� ǥ���� ������Ʈ
    public GameObject Title;
    public GameObject processing;
    private GazePoint gazePoint;
    public GameObject target;

    // 27��ġ ������� ���� ũ�� �� �Ÿ�
    private const float monitorWidth = 59.4f; // cm
    private const float monitorHeight = 33.5f; // cm
    private const float viewerDistance = 20.0f; // cm

    public Vector2 world = new Vector2(0, 0);
    void Start()
    {
        // ���α׷� ���� �� �ڵ����� Python ��ũ��Ʈ�� ����
        videoPlayer.Stop();
        originPlayer.Stop();
        samorigin.SetActive(false);
        myorigin.SetActive(false);
    }

    void Update()
    {
        // Tobii API ������Ʈ
        TobiiGameIntegrationApi.Update();
        IntPtr handle = GetForegroundWindow();
        TobiiGameIntegrationApi.TrackWindow(handle);
        TobiiGameIntegrationApi.UpdateTrackerInfos();

        TobiiGameIntegrationApi.TryGetLatestGazePoint(out gazePoint);
        // �ֽ� �ü� ������ ��������
        UpdateGazeIndicator();
        UpdateGazeMarker();
        CheckGazeOnUI();

        Vector2 uicord = new Vector2(targetUI.transform.position.x - gazeMarker.transform.position.x, targetUI.transform.position.y - gazeMarker.transform.position.y);
        // ���� ������ ���� �Է� Ȯ�� (������)
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
            ProcessRegion(1, 1); // ���� ���
            target.transform.localPosition = new Vector3(-877, 307);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(1, 2);
            ProcessRegion(1, 2); // ���� ���
            target.transform.localPosition = new Vector3(-237, 307);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(1, 3);
            ProcessRegion(1, 3); // ���� ���
            target.transform.localPosition = new Vector3(417, 307);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(2, 1);
            ProcessRegion(2, 1); // ���� ���
            target.transform.localPosition = new Vector3(-877, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(2, 2);
            ProcessRegion(2, 2); // ���� ���
            target.transform.localPosition = new Vector3(-237, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(2, 3);
            ProcessRegion(2, 3); // ���� ���
            target.transform.localPosition = new Vector3(417, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(3, 1);
            ProcessRegion(3, 1); // ���� ���
            target.transform.localPosition = new Vector3(-877, -307);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(3, 2);
            ProcessRegion(3, 2); // ���� ���
            target.transform.localPosition = new Vector3(-237, -307);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            videoPlayer.Stop();
            originPlayer.Stop();
            samorigin.SetActive(false);
            myorigin.SetActive(false);
            world = new Vector2(3, 3);
            ProcessRegion(3, 3); // ���� ���
            target.transform.localPosition = new Vector3(417, -307);
        }
    }

    void ProcessRegion(int row, int col)
    {
        // yaw�� pitch ���
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
        // 9��� ���� ��ǥ
        float cellWidth = monitorWidth / 3;
        float cellHeight = monitorHeight / 3;

        // ������ �߽� ��ǥ (cm)
        float targetX = (col - 1) * cellWidth - monitorWidth / 2 + cellWidth / 2;
        float targetY = monitorHeight / 2 - (row - 1) * cellHeight - cellHeight / 2;

        // 3D ���ͷ� ��ȯ
        Vector3 targetVector = new Vector3(targetX, targetY, -viewerDistance);
        targetVector.Normalize();

        // yaw (���� ȸ��) �� pitch (���� ȸ��) ���
        float yaw = Mathf.Atan2(targetVector.x, -targetVector.z) * Mathf.Rad2Deg;
        float pitch = Mathf.Asin(targetVector.y) * Mathf.Rad2Deg;

        return new Vector2(yaw, pitch);
    }
    void UpdateGazeIndicator()
    {
        if (canvas == null || gazeIndicator == null) return;

        // ĵ������ RectTransform ��������
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // ĵ���� ũ��� �߽� ��ġ�� �������� ��ǥ ��ȯ
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // Tobii�� ����ȭ�� ��ǥ�� ĵ���� �������� ��ȯ
        Vector2 localPosition = new Vector2(
            (gazePoint.X - 0.5f) * canvasWidth,
            (gazePoint.Y - 0.5f) * canvasHeight
        );

        // Gaze Indicator�� ��ġ ����
        gazeIndicator.anchoredPosition = localPosition;
    }

    void UpdateGazeMarker()
    {
        if (gazeMarker == null) return;

        // Tobii ��ǥ�� ȭ�� ��ǥ�� ��ȯ
        Vector2 screenPosition = new Vector2(
            gazePoint.X * Screen.width,
            gazePoint.Y * Screen.height
        );

        // ȭ�� ��ǥ�� ĵ���� ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas,
            screenPosition,
            Camera.main,
            out Vector2 localCanvasPosition
        );

        // Gaze Marker ��ġ ����
        gazeMarker.transform.localPosition = new Vector3(gazePoint.X * 10.0f + 497, gazePoint.Y * 10.0f + 260, -427);
    }

    void CheckGazeOnUI()
    {
        Vector2 screenPosition = new Vector2(gazePoint.X * Screen.width, gazePoint.Y * Screen.height);

 //       UnityEngine.Debug.Log("point " + screenPosition);
        // �ü��� ���� ���� ���� ���� ��� ���� ����
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
        UnityEngine.Debug.Log("���� Ŭ���� ����Ǿ����ϴ�.");
    }
    async void RunPythonScript(string inputDir, string outputDir, string splitScriptPath, string processScriptPath, string mergeScriptPath, float yaw, float pitch)
    {
        Title.SetActive(false);
        processing.SetActive(true);
        UnityEngine.Debug.Log("Process start");

        try
        {
            string pythonExecutable = @"C:\Users\user\AppData\Local\Programs\Python\Python312\python.exe";  // Python ���

            // ù ��° ��ũ��Ʈ: ���� ���� (split_video.py)
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

            // �� ��° ��ũ��Ʈ: ���� ó�� (process_video.py)
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

            // �� ��° ��ũ��Ʈ: ���� ���� (merge_videos_opencv.py)
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

