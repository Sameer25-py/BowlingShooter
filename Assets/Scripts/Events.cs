using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public static class Events
{
    public static UnityEvent<Vector2> TouchBegan = new UnityEvent<Vector2>();
    public static UnityEvent<Vector2> TouchMoved = new UnityEvent<Vector2>();
    public static UnityEvent TouchEnded = new UnityEvent();
    public static UnityEvent<Vector2> TouchedBubble = new UnityEvent<Vector2>();
    public static UnityEvent<string> ConvertBubble = new UnityEvent<string>();

    public static UnityEvent<List<Sprite>> StartGame = new();
}