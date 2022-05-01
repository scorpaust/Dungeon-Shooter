using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class StaticEventHandler 
{
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    public static void CallRoomChangedEvent(Room room)
	{
        OnRoomChanged?.Invoke(new RoomChangedEventArgs() { room = room });
	}

    // Room enemies defeated event
    public static event Action<RoomEnemiesDefeatedArs> OnRoomEnemiesDefeated;

    public static void CallRoomEnemiesDefeatedEvent(Room room)
	{
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedArs() { room = room });
	}

    // Points scored event
    public static event Action<PointsScoredArgs> OnPointsScored;

    public static void CallPointsScoredEvent(int points)
	{
        OnPointsScored?.Invoke(new PointsScoredArgs() { points = points });
	}

    // Score changed event
    public static event Action<ScoreChangedArgs> OnScoreChanged;

    public static void CallScoreChangedEvent(long score, int multiplier)
	{
        OnScoreChanged?.Invoke(new ScoreChangedArgs() { score = score, multiplier = multiplier });
	}

    // Multiplier event
    public static event Action<MultiplierArgs> OnMultiplier;

    public static void CallMultiplierEvent(bool multiplier)
	{
        OnMultiplier?.Invoke(new MultiplierArgs() { multiplier = multiplier });
	}
}

public class RoomChangedEventArgs : EventArgs
{
    public Room room;
}

public class RoomEnemiesDefeatedArs : EventArgs
{
    public Room room;
}

public class PointsScoredArgs : EventArgs
{
    public int points;
}

public class ScoreChangedArgs : EventArgs
{
    public long score;

    public int multiplier;
}

public class MultiplierArgs : EventArgs
{
    public bool multiplier;
}