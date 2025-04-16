
using Mapbox.Utils;

[System.Serializable]
public class RouteData
{
    public string name;
    public string description;
    public float cost;
    public float durationHours;
    public Vector2d[] waypoints;
}
