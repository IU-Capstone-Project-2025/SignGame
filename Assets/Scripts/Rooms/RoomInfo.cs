using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomInfo", menuName = "Scriptable Objects/RoomInfo")]
public class RoomInfo : ScriptableObject
{
    public string sceneName;
    public RoomDirections rd;
}

[Serializable]
public class RoomDirections {
    public DirAvailability up;
    public DirAvailability down;
    public DirAvailability left;
    public DirAvailability right;

    public RoomDirections() {}

    public RoomDirections(RoomDirections roomDirections) {
        up = roomDirections.up;
        down = roomDirections.down;
        left = roomDirections.left;
        right = roomDirections.right;
    }

    public static RoomDirections Any() {
        return new RoomDirections{
            up = DirAvailability.Any,
            down = DirAvailability.Any,
            left = DirAvailability.Any,
            right = DirAvailability.Any
        };
    }

    public bool CanFit(RoomDirections other) {
        bool r = up == DirAvailability.Any || up == other.up;
        r &= down == DirAvailability.Any || down == other.down;
        r &= left == DirAvailability.Any || left == other.left;
        r &= right == DirAvailability.Any || right == other.right;
        return r;
    }

    public RoomDirections ExtendAvailable(RoomDirections other) {
        RoomDirections rd = new(this);
        rd.up = rd.up == DirAvailability.Available || other.up == DirAvailability.Available ? DirAvailability.Available : rd.up;
        rd.down = rd.down == DirAvailability.Available || other.down == DirAvailability.Available ? DirAvailability.Available : rd.down;
        rd.left = rd.left == DirAvailability.Available || other.left == DirAvailability.Available ? DirAvailability.Available : rd.left;
        rd.right = rd.right == DirAvailability.Available || other.right == DirAvailability.Available ? DirAvailability.Available : rd.right;
        return rd;
    }

    public override string ToString() {
        return $"{(up == DirAvailability.Available ? "U" : up == DirAvailability.Any ? "u" : "")}" + 
            $"{(down == DirAvailability.Available ? "D" : down == DirAvailability.Any ? "d" : "")}" + 
            $"{(left == DirAvailability.Available ? "L" : left == DirAvailability.Any ? "l" : "")}" + 
            $"{(right == DirAvailability.Available ? "R" : right == DirAvailability.Any ? "r" : "")}";
    }
}

public enum DirAvailability {
    Available,
    NotAvailable,
    Any
}

public class RoomInfoInstance
{
    public RoomInfo roomInfo;
    public bool visited = false;
    public bool cleared = false;
    public Transform root;
}
