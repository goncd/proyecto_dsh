using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    // Singleton instance.
    public static GameState Instance { get; private set; }

    // The name of the player.
    public string playerName;

    // The scene that is currently being run.
    public int currentScene;

    // The last scene, useful for going back when a minigame ends.
    public int lastScene;

    // Dynamic data storage.
    private readonly Dictionary<string, object> data = new(); // dynamic data store

    // Handle the singleton by ensuring only one instance of this class
    // is active at all times.
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Generic Set and Get methods for loading and storing data
    // that is persistent across the scenes.
    public void Set<T>(string key, T value)
    {
        data[key] = value;
    }

    public T Get<T>(string key)
    {
        if (data.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        Debug.LogWarning($"Key '{key}' not found or not of type {typeof(T)}");
        return default;
    }

    public bool Get<T>(string key, out T var)
    {
        if (data.TryGetValue(key, out var value) && value is T typedValue)
        {
            var = typedValue;
            return true;
        }

        var = default;
        return false;
    }

    public bool HasKey(string key) => data.ContainsKey(key);
    public void Clear() => data.Clear();
}