using System;
using System.Net.Sockets;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PythonServer : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        try
        {
            // Python 서버에 연결
            client = new TcpClient("127.0.0.1", 65432);
            stream = client.GetStream();
            Debug.Log("Python 서버에 연결되었습니다.");

            // Python 서버에 요청 보내기
            SendRequest("연산 요청");
        }
        catch (Exception e)
        {
            Debug.LogError("연결에 실패했습니다: " + e.Message);
        }
    }

    void SendRequest(string message)
    {
        try
        {
            // 요청 메시지를 UTF-8로 인코딩하여 전송
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("Python 서버에 요청을 보냈습니다.");

            // 응답 받기
            ReceiveData();
        }
        catch (Exception e)
        {
            Debug.LogError("요청 전송 실패: " + e.Message);
        }
    }

    void ReceiveData()
    {
        try
        {
            // 응답을 바이트 배열로 수신
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                // 수신된 데이터를 역직렬화하여 float 배열로 변환
                float[] tensorResult = Deserialize(buffer);
                Debug.Log("수신된 텐서 결과:");
                foreach (var value in tensorResult)
                {
                    Debug.Log(value);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("데이터 수신 실패: " + e.Message);
        }
    }

    float[] Deserialize(byte[] data)
    {
        // float 배열로 변환
        using (var ms = new System.IO.MemoryStream(data))
        {
            var formatter = new BinaryFormatter();
            return (float[])formatter.Deserialize(ms);
        }
    }

    void OnApplicationQuit()
    {
        // 연결 종료
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}
