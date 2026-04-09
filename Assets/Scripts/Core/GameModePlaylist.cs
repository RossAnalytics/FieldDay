using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the ordered list of sport modes to play through.
/// Supports reset and sequential advancement.
/// Order: Darts → Bowling → Golf → Basketball → Soccer → Billiards
/// </summary>
public class GameModePlaylist
{
    private readonly List<GameObject> _prefabs;
    private int _currentIndex;

    public GameModePlaylist(List<GameObject> modePrefabs)
    {
        _prefabs = new List<GameObject>(modePrefabs);
        _currentIndex = 0;
    }

    /// <summary>Returns true if there are more modes left to play.</summary>
    public bool HasNext() => _currentIndex < _prefabs.Count;

    /// <summary>Returns the next mode prefab and advances the index.</summary>
    public GameObject Next()
    {
        if (!HasNext()) return null;
        return _prefabs[_currentIndex++];
    }

    /// <summary>Resets the playlist to the beginning.</summary>
    public void Reset() => _currentIndex = 0;

    /// <summary>Current zero-based index into the playlist.</summary>
    public int CurrentIndex => _currentIndex;

    /// <summary>Total number of modes in this playlist.</summary>
    public int Count => _prefabs.Count;
}
