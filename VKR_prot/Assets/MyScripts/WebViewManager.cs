using UnityEngine;

public class WebViewManager : MonoBehaviour
{
    private WebViewObject webViewObject;

    void Start()
    {
        // Проверка поддержки WebView на текущей платформе
        if (!IsWebViewSupported())
        {
            Debug.LogError("WebView is not supported on this platform.");
            return;
        }

        // Создание WebView
        webViewObject = gameObject.AddComponent<WebViewObject>();

        // Инициализация WebView
        webViewObject.Init(
            cb: (message) => Debug.Log($"Callback invoked with message: {message}"),
            err: (error) => Debug.LogError($"WebView error: {error}"),
            enableWKWebView: true
        );

        // Настройка размеров WebView
        webViewObject.SetMargins(0, 0, 0, 0);

        // Загрузка HTML-файла из StreamingAssets
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
        // Проверка поддерживаемых платформ
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