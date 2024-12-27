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
            // Python ������ ����
            client = new TcpClient("127.0.0.1", 65432);
            stream = client.GetStream();
            Debug.Log("Python ������ ����Ǿ����ϴ�.");

            // Python ������ ��û ������
            SendRequest("���� ��û");
        }
        catch (Exception e)
        {
            Debug.LogError("���ῡ �����߽��ϴ�: " + e.Message);
        }
    }

    void SendRequest(string message)
    {
        try
        {
            // ��û �޽����� UTF-8�� ���ڵ��Ͽ� ����
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("Python ������ ��û�� ���½��ϴ�.");

            // ���� �ޱ�
            ReceiveData();
        }
        catch (Exception e)
        {
            Debug.LogError("��û ���� ����: " + e.Message);
        }
    }

    void ReceiveData()
    {
        try
        {
            // ������ ����Ʈ �迭�� ����
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                // ���ŵ� �����͸� ������ȭ�Ͽ� float �迭�� ��ȯ
                float[] tensorResult = Deserialize(buffer);
                Debug.Log("���ŵ� �ټ� ���:");
                foreach (var value in tensorResult)
                {
                    Debug.Log(value);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("������ ���� ����: " + e.Message);
        }
    }

    float[] Deserialize(byte[] data)
    {
        // float �迭�� ��ȯ
        using (var ms = new System.IO.MemoryStream(data))
        {
            var formatter = new BinaryFormatter();
            return (float[])formatter.Deserialize(ms);
        }
    }

    void OnApplicationQuit()
    {
        // ���� ����
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}
