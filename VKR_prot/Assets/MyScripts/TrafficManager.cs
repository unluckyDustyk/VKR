using UnityEngine;
using Mapbox.Unity.Map;

public class TrafficManager : MonoBehaviour
{
    [Header("Ссылка на карту")]
    public AbstractMap map;

    [Header("ID стиля карты с трафиком")]
    [Tooltip("Например: mapbox://styles/mapbox/traffic-day-v2")]
    public string trafficStyleId = "mapbox://styles/mapbox/traffic-day-v2";

    void Start()
    {
        if (map == null)
        {
            Debug.LogError("Map (AbstractMap) не назначен в инспекторе!");
            return;
        }

        if (string.IsNullOrEmpty(trafficStyleId))
        {
            Debug.LogError("Не указан стиль трафика!");
            return;
        }

        ApplyTrafficStyle();
    }

    void ApplyTrafficStyle()
    {
        // Обновление карты с новым стилем
        map.UpdateMap();

        Debug.Log("Трафик включён, применён стиль: " + trafficStyleId);
    }
}
