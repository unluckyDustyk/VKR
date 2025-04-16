using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Directions;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Unity;

public class SmartRouteManager : MonoBehaviour
{
    [Header("Map & Rendering")]
    public AbstractMap map;
    public LineRenderer lineRendererPrefab;
    public Transform lineParent;

    [Header("UI")]
    public TextMeshProUGUI resultText;

    [Header("Маршруты")]
    public List<RouteData> routes;

    public Button routeButton1;
    public Button routeButton2;
    public Button routeButton3;

    public GameObject routeSelectionPanel;   
    public GameObject resultPanel;          

    private Directions directions;
    private List<GameObject> lineInstances = new();

    private readonly Color[] routeColors = new Color[]
    {
        Color.red,
        new Color(0f, 0.5f, 0f), 
        Color.blue
    };

    void Start()
    {
        directions = MapboxAccess.Instance.Directions;

        routeButton1.onClick.AddListener(() => ShowRouteInfo(routes[0]));
        routeButton2.onClick.AddListener(() => ShowRouteInfo(routes[1]));
        routeButton3.onClick.AddListener(() => ShowRouteInfo(routes[2]));

        routeSelectionPanel.SetActive(true);
        resultPanel.SetActive(false);

        for (int i = 0; i < routes.Count; i++)
        {
            var route = routes[i];
            var color = routeColors[i % routeColors.Length];
            RequestRoute(route, color);
        }
    }

    void ShowRouteInfo(RouteData route)
    {
        routeSelectionPanel.SetActive(false);

        resultPanel.SetActive(true);

        string routeInfo = $"{route.name}\n{route.description}\nСтоимость: {route.cost}Р\nВремя: {route.durationHours} ч\n\n";

        if (route.durationHours < 8 && route.cost > 11500)
        {
            routeInfo += "Это быстрый, но дорогой маршрут. Подходит для тех, кто не хочет тратить время.";
        }
        else if (route.durationHours > 14)
        {
            routeInfo += "Этот маршрут не самый оптимальный, он занимает много времени и имеет больщую стоимость.";
        }
        else
        {
            routeInfo += "Этот маршрут оптимален по времени и стоимости.";
        }

        resultText.text = routeInfo;
    }

    // Запрос маршрута
    void RequestRoute(RouteData routeData, Color lineColor)
    {
        var wp = routeData.waypoints.Select(w => new Vector2d(w.x, w.y)).ToList();

        var directionResource = new DirectionResource(wp.ToArray(), RoutingProfile.Driving);

        directions.Query(directionResource, (res) =>
        {
            if (res.Routes == null || res.Routes.Count < 1)
            {
                Debug.LogError("Маршрут не найден");
                return;
            }

            var geometry = res.Routes[0].Geometry;
            var worldPositions = new List<Vector3>();

            foreach (var point in geometry)
            {
                worldPositions.Add(map.GeoToWorldPosition(point, true));
            }

            var line = Instantiate(lineRendererPrefab, lineParent);
            line.positionCount = worldPositions.Count;
            line.SetPositions(worldPositions.ToArray());

            line.startColor = line.endColor = lineColor;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.widthMultiplier = 5f;

            lineInstances.Add(line.gameObject);
        });
    }

    public void ReturnToRouteSelection()
    {
        resultPanel.SetActive(false);
        routeSelectionPanel.SetActive(true);
    }
}
