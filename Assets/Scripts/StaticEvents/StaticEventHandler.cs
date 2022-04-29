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
}

public class RoomChangedEventArgs : EventArgs
{
    public Room room;
}

public class RoomEnemiesDefeatedArs : EventArgs
{
    public Room room;
}