using UnityEngine;

public class WebViewManager : MonoBehaviour
{
    private WebViewObject webViewObject;

    void Start()
    {
        // �������� ��������� WebView �� ������� ���������
        if (!IsWebViewSupported())
        {
            Debug.LogError("WebView is not supported on this platform.");
            return;
        }

        // �������� WebView
        webViewObject = gameObject.AddComponent<WebViewObject>();

        // ������������� WebView
        webViewObject.Init(
            cb: (message) => Debug.Log($"Callback invoked with message: {message}"),
            err: (error) => Debug.LogError($"WebView error: {error}"),
            enableWKWebView: true
        );

        // ��������� �������� WebView
        webViewObject.SetMargins(0, 0, 0, 0);

        // �������� HTML-����� �� StreamingAssets
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "map.html");
        if (System.IO.File.Exists(filePath))
        {
            string fileContent = System.IO.File.ReadAllText(filePath);
            webViewObject.LoadHTML(fileContent, "");
        }
        else
        {
            Debug.LogError("HTML file not found: " + filePath);
        }
    }

    private bool IsWebViewSupported()
    {
        // �������� �������������� ��������
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return true;
            default:
                return false;
        }
    }
}