using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.VectorTile;
using Mapbox.VectorTile.Geometry;
using System.Collections.Generic;

public class TrafficGeoJsonVisualizer : MonoBehaviour
{
    public AbstractMap map;
    public Color lightTrafficColor = Color.green;
    public Color mediumTrafficColor = Color.yellow;
    public Color heavyTrafficColor = Color.red;

    public Material lineMaterial;
    public float lineWidth = 3f;

    private List<GameObject> trafficLines = new List<GameObject>();

    void Start()
    {
        StartCoroutine(LoadTrafficData());
    }

    private IEnumerator LoadTrafficData()
    {
        Vector2d centerGeoCoord = map.CenterLatitudeLongitude;
        int zoom = (int)map.Zoom;

        double n = Math.Pow(2, zoom);
        int x = (int)((centerGeoCoord.x + 180.0) / 360.0 * n);
        int y = (int)((1.0 - Math.Log(Math.Tan(centerGeoCoord.y * Math.PI / 180.0) + 1.0 / Math.Cos(centerGeoCoord.y * Math.PI / 180.0)) / Math.PI) / 2.0 * n);

        string url = $"https://api.mapbox.com/v4/mapbox.mapbox-traffic-v1/{zoom}/{x}/{y}.vector.pbf?access_token=pk.eyJ1IjoiZHVzdHlrIiwiYSI6ImNtOWg5NDFnbDAzYnoyanM3ejZhYzgyaXoifQ.PTOiNf6j75uDhex7WvygKQ";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                byte[] data = webRequest.downloadHandler.data;
                ProcessVectorTileData(data, zoom, x, y);
            }
            else
            {
                Debug.LogError($"Failed to load traffic data: {webRequest.error}");
            }
        }
    }

    private void ProcessVectorTileData(byte[] data, int zoom, int tileX, int tileY)
    {
        var tile = new VectorTile(data);
        var trafficLayer = tile.GetLayer("traffic");

        if (trafficLayer == null)
        {
            Debug.LogError("Traffic layer not found.");
            return;
        }

        for (int i = 0; i < trafficLayer.FeatureCount(); i++)
        {
            var feature = trafficLayer.GetFeature(i);
            var geometry = feature.Geometry<Vector2>();

            string congestion = "unknown";
            var props = feature.GetProperties();
            if (props.ContainsKey("congestion"))
                congestion = props["congestion"].ToString().ToLower();

            Color trafficColor = GetTrafficColorFromString(congestion);

            foreach (var segment in geometry)
            {
                List<Vector3> worldPoints = new List<Vector3>();

                foreach (var point in segment)
                {
                    // Преобразуем координаты тайла в глобальные
                    Vector2d tileCoord = TileToGeo(point.X.x, point.Y.y, zoom, tileX, tileY);
                    Vector3 worldPos = map.GeoToWorldPosition(tileCoord, true);
                    worldPoints.Add(worldPos);
                }

                if (worldPoints.Count >= 2)
                    DrawTrafficLine(worldPoints, trafficColor);
            }
        }
    }

    private Vector2d TileToGeo(float x, float y, int zoom, int tileX, int tileY)
    {
        double scale = Math.Pow(2, zoom);
        double tileSize = 4096.0;

        double lon = (tileX + x / tileSize) / scale * 360.0 - 180.0;
        double latRad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * (tileY + y / tileSize) / scale)));
        double lat = latRad * 180.0 / Math.PI;

        return new Vector2d(lat, lon);
    }

    private Color GetTrafficColorFromString(string congestion)
    {
        return congestion switch
        {
            "low" => lightTrafficColor,
            "moderate" => mediumTrafficColor,
            "heavy" or "severe" => heavyTrafficColor,
            _ => Color.gray
        };
    }

    private void DrawTrafficLine(List<Vector3> points, Color color)
    {
        GameObject lineObj = new GameObject("TrafficLine");
        var lr = lineObj.AddComponent<LineRenderer>();

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = color;
        lr.startWidth = lr.endWidth = lineWidth;
        lr.useWorldSpace = true;

        trafficLines.Add(lineObj);
    }
}
