using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class VideoFrameExtractor : MonoBehaviour
{
    public VideoPlayer videoPlayer;  
    public string videoFilePath;  
    public string outputFolder;     
    public bool extractAllFrames = true; 
    public int frameInterval = 1;   // 프레임 간격 (1 = 모든 프레임 저장)

    private int frameCount = 0;

    void Start()
    {
        if (string.IsNullOrEmpty(videoFilePath) || string.IsNullOrEmpty(outputFolder))
        {
            Debug.LogError("Video file path or output folder is not set!");
            return;
        }

        // 출력 폴더 생성
        Directory.CreateDirectory(outputFolder);

        // VideoPlayer 설정
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoFilePath;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Video prepared. Starting frame extraction...");
        videoPlayer.Play();
        StartCoroutine(ExtractFrames());
    }

    private System.Collections.IEnumerator ExtractFrames()
    {
        while (videoPlayer.isPlaying)
        {
            if ((frameCount % frameInterval == 0) || extractAllFrames)
            {
                // 텍스처에서 프레임 가져오기
                Texture2D texture = GetVideoFrameTexture(videoPlayer);

                if (texture != null)
                {
                    SaveFrameAsImage(texture, frameCount);
                }
            }

            frameCount++;
            yield return new WaitForEndOfFrame(); // 다음 프레임으로 이동
        }

        Debug.Log("Frame extraction completed!");
    }

    private Texture2D GetVideoFrameTexture(VideoPlayer vp)
    {
        // RenderTexture에서 Texture2D로 변환
        RenderTexture renderTexture = vp.texture as RenderTexture;
        if (renderTexture == null) return null;

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = currentRT; // 원래 상태로 복구
        return texture;
    }

    private void SaveFrameAsImage(Texture2D texture, int frameIndex)
    {
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(outputFolder, $"frame_{frameIndex:00000}.png");
        File.WriteAllBytes(filePath, bytes);
        Debug.Log($"Saved frame {frameIndex} to {filePath}");
        Destroy(texture); // 메모리 해제
    }
}
