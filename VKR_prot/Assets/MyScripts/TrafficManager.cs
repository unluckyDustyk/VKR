using UnityEngine;
using Mapbox.Unity.Map;

public class TrafficManager : MonoBehaviour
{
    [Header("������ �� �����")]
    public AbstractMap map;

    [Header("ID ����� ����� � ��������")]
    [Tooltip("��������: mapbox://styles/mapbox/traffic-day-v2")]
    public string trafficStyleId = "mapbox://styles/mapbox/traffic-day-v2";

    void Start()
    {
        if (map == null)
        {
            Debug.LogError("Map (AbstractMap) �� �������� � ����������!");
            return;
        }

        if (string.IsNullOrEmpty(trafficStyleId))
        {
            Debug.LogError("�� ������ ����� �������!");
            return;
        }

        ApplyTrafficStyle();
    }

    void ApplyTrafficStyle()
    {
        // ���������� ����� � ����� ������
        map.UpdateMap();

        Debug.Log("������ �������, ������� �����: " + trafficStyleId);
    }
}
