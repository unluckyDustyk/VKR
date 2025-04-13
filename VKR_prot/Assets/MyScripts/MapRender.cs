using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MapLoader : MonoBehaviour
{
    public RawImage mapImage; // ������ �� Raw Image
    public string apiKey = "8cdc4ef5-1cec-4c3a-8cb1-db7ad4e8d7cb"; // ��� API-����

    void Start()
    {
        LoadMap(37.617635f, 55.755814f, 10); // ������, ������� 10
    }

    public void LoadMap(float longitude, float latitude, int zoom)
    {
        StartCoroutine(LoadMapImage(longitude, latitude, zoom));
    }

    private IEnumerator LoadMapImage(float longitude, float latitude, int zoom)
    {
        string url = $"https://static-maps.yandex.ru/1.x/?apikey={apiKey}&ll={longitude},{latitude}&z={zoom}&l=map";

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            // ��������� ��������� User-Agent
            request.SetRequestHeader("User-Agent", "Mozilla/5.0");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                mapImage.texture = texture; // ��������� �������� Raw Image
            }
            else
            {
                Debug.LogError("Failed to load map: " + request.error);
                Debug.LogError("Response Code: " + request.responseCode); // �������� ��� ������
            }
        }
    }
}