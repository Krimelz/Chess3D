using System.Collections.Generic;
using System;

public delegate void Callback();
public delegate void Callback<T>(T arg);
public delegate void Callback<T1, T2>(T1 arg1, T2 arg2);

public static class EventManager
{
    private static Dictionary<string, Delegate> _listeners = new Dictionary<string, Delegate>(); 
    public static void AddEvent(string name, Callback callback)
    {
        _listeners.Add(name, callback);
    }

    public static void RemoveEvent(string name)
    {
        _listeners.Remove(name);
    }

    public static void Broadcast(string name)
    {
        Delegate d;

        if (_listeners.TryGetValue(name, out d))
        {
            (d as Callback)?.Invoke();
        }
    }
}

public static class EventManager<T>
{
    private static Dictionary<string, Delegate> _listeners = new Dictionary<string, Delegate>();
    public static void AddEvent(string name, Callback<T> callback)
    {
        _listeners.Add(name, callback);
    }

    public static void RemoveEvent(string name)
    {
        _listeners.Remove(name);
    }

    public static void Broadcast(string name, T arg)
    {
        Delegate d;

        if (_listeners.TryGetValue(name, out d))
        {
            (d as Callback<T>)?.Invoke(arg);
        }
    }
}

public static class EventManager<T1, T2>
{
    private static Dictionary<string, Delegate> _listeners = new Dictionary<string, Delegate>();
    public static void AddEvent(string name, Callback<T1, T2> callback)
    {
        _listeners.Add(name, callback);
    }

    public static void RemoveEvent(string name)
    {
        _listeners.Remove(name);
    }

    public static void Broadcast(string name, T1 arg1, T2 arg2)
    {
        Delegate d;

        if (_listeners.TryGetValue(name, out d))
        {
            (d as Callback<T1, T2>)?.Invoke(arg1, arg2);
        }
    }
}
