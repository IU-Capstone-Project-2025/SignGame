using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public int roomsX = 4, roomsY = 4;
    public float roomOffsetX, roomOffsetY;
    public bool usePath = true;
    public bool removeUselessClusters = false;
    // public int minPathSize = 6, maxPathSize = 15;
    public List<RoomInfo> startRooms;
    public RoomInfo bossRoomInfo;
    public List<RoomCount> maxRoomCount;

    RoomInfoInstance[,] rooms;

    void Awake() {
        rooms = new RoomInfoInstance[roomsX, roomsY];
        GenerateLevel();
    }

    void Start() {
        LoadLevel();
    }

    void GenerateLevel() {
        List<RoomInfo> roomInfos = Resources.LoadAll<RoomInfo>("RoomInfos").ToList();
        // List<Vector2Int> path = new PathGenerator(roomsX, roomsY, minPathSize, maxPathSize).GenerateRandomPath(0, 0, roomsX - 1, roomsY - 1);
        List<Vector2Int> path = new PathGenerator(roomsX, roomsY).GenerateRandomPath(0, 0, roomsX - 1, roomsY - 1) ?? throw new Exception("Path cannot be generated");
        Debug.Log("Path: " + string.Join(" ", path.Select(p => $"[{p[0]} {p[1]}]")));
        RoomDirections[,] pathRooms = new RoomDirections[roomsX, roomsY];
        for (int x = 0; x < roomsX; ++x) for (int y = 0; y < roomsY; ++y) pathRooms[x, y] = RoomDirections.Any();
        for (int i = 0; i < path.Count - 1; ++i) {
            Vector2Int room1 = path[i], room2 = path[i + 1];
            if (room1[0] > room2[0]) {
                pathRooms[room1[0], room1[1]].left = DirAvailability.Available;
                pathRooms[room2[0], room2[1]].right = DirAvailability.Available;
            } else if (room1[0] < room2[0]) {
                pathRooms[room1[0], room1[1]].right = DirAvailability.Available;
                pathRooms[room2[0], room2[1]].left = DirAvailability.Available;
            } else if (room1[1] > room2[1]) {
                pathRooms[room1[0], room1[1]].up = DirAvailability.Available;
                pathRooms[room2[0], room2[1]].down = DirAvailability.Available;
            } else if (room1[1] < room2[1]) {
                pathRooms[room1[0], room1[1]].down = DirAvailability.Available;
                pathRooms[room2[0], room2[1]].up = DirAvailability.Available;
            }
        }
        rooms[roomsX - 1, roomsY - 1] = new RoomInfoInstance{roomInfo = bossRoomInfo};
        List<RoomCount> currentRoomCount = new(maxRoomCount);
        for (int x = 0; x < roomsX; ++x) {
            for (int y = 0; y < roomsY; ++y) {
                if (rooms[x, y] != null) continue;
                RoomDirections possibleRD = GetRoomDirectionsAvailable(x, y);
                if (usePath) possibleRD = possibleRD.ExtendAvailable(pathRooms[x, y]);
                List<RoomInfo> possibleRooms = (x == 0 && y == 0 ? startRooms : roomInfos)
                    .Where(ri => !currentRoomCount.Any(rc => rc.roomInfo == ri) || currentRoomCount.Find(rc => rc.roomInfo == ri).count > 0)
                    .Where(ri => possibleRD.CanFit(ri.rd)).ToList();
                if (possibleRooms.Count == 0) continue;
                RoomInfo ri = possibleRooms.OrderBy(_ => Random.value).First();
                if (currentRoomCount.Any(r => r.roomInfo == ri)) --currentRoomCount.Find(rc => rc.roomInfo == ri).count;
                // RoomInfo ri = roomInfos[Random.Range(0, roomInfos.Count - 1)];
                rooms[x, y] = new RoomInfoInstance{roomInfo = ri};
            }
        }
        if (removeUselessClusters) RemoveUselessClusters();
    }

    void LoadLevel() {
        for (int x = 0; x < roomsX; ++x) {
            for (int y = 0; y < roomsY; ++y) {
                RoomInfoInstance rii = rooms[x, y];
                if (rii == null) continue;
                rii.root = Instantiate(rii.roomInfo.roomPrefab, new(x * roomOffsetX, -y * roomOffsetY), Quaternion.identity).transform;
            }
        }
    }

    RoomDirections GetRoomDirectionsAvailable(int x, int y) {
        RoomDirections rd = new() {
            up = IsDirectionAvailable(x, y, x, y - 1),
            down = IsDirectionAvailable(x, y, x, y + 1),
            left = IsDirectionAvailable(x, y, x - 1, y),
            right = IsDirectionAvailable(x, y, x + 1, y)
        };
        return rd;
    }

    DirAvailability IsDirectionAvailable(int newX, int newY, int exX, int exY) {
        if (exX < 0 || exX >= roomsX || exY < 0 || exY >= roomsY) return DirAvailability.NotAvailable;
        if (rooms[exX, exY] == null) return DirAvailability.Any;
        if (exX > newX) return rooms[exX, exY].roomInfo.rd.left;
        if (exX < newX) return rooms[exX, exY].roomInfo.rd.right;
        if (exY > newY) return rooms[exX, exY].roomInfo.rd.up;
        if (exY < newY) return rooms[exX, exY].roomInfo.rd.down;
        return DirAvailability.Available;
    }

    public RoomInfoInstance GetRoom(int x, int y) {
        return rooms[x, y];
    }

    void RemoveUselessClusters() {
        List<HashSet<RoomInfoInstance>> clusters = new();
        for (int x = 0; x < roomsX; ++x) {
            for (int y = 0; y < roomsY; ++y) {
                RoomInfoInstance rii = rooms[x, y];
                if (rii == null) continue;
                if (clusters.Any(cluster => cluster.Contains(rii))) continue;
                HashSet<RoomInfoInstance> cluster = new() {rii};
                bool updated = true;
                while (updated) {
                    updated = false;
                    for (int x2 = 0; x2 < roomsX; ++x2) {
                        for (int y2 = 0; y2 < roomsY; ++y2) {
                            RoomInfoInstance rii2 = rooms[x2, y2];
                            if (cluster.Contains(rii2) || rii2 == null) continue;
                            bool add = false;
                            print($"{x2} {y2} {rii2.roomInfo.rd}");
                            if (rii2.roomInfo.rd.up == DirAvailability.Available && cluster.Contains(rooms[x2, y2 - 1])) add = true;
                            if (rii2.roomInfo.rd.down == DirAvailability.Available && cluster.Contains(rooms[x2, y2 + 1])) add = true;
                            if (rii2.roomInfo.rd.left == DirAvailability.Available && cluster.Contains(rooms[x2 - 1, y2])) add = true;
                            if (rii2.roomInfo.rd.right == DirAvailability.Available && cluster.Contains(rooms[x2 + 1, y2])) add = true;
                            if (add) {
                                updated = true;
                                cluster.Add(rii2);
                            }
                        }
                    }
                }
                clusters.Add(cluster);
            }
        }
        print(clusters.Count);
        clusters = clusters.Where(cluster => !cluster.Contains(rooms[0, 0])).ToList();
        if (clusters.Count > 0) {
            for (int x = 0; x < roomsX; ++x) {
                for (int y = 0; y < roomsY; ++y) {
                    if (clusters.Any(cluster => cluster.Contains(rooms[x, y]))) rooms[x, y] = null;
                }
            }
        }
    }
}

class PathGenerator {
    readonly int roomsX, roomsY;
    readonly int minPathSize, maxPathSize;

    static readonly List<Vector2Int> directions = new() {
        new(1, 0),
        new(-1, 0),
        new(0, 1),
        new(0, -1)
    };
    HashSet<Vector2Int> visited;

    public PathGenerator(int roomsX, int roomsY, int? minPathSize = null, int? maxPathSize = null) {
        this.roomsX = roomsX;
        this.roomsY = roomsY;
        this.minPathSize = minPathSize ?? 1;
        this.maxPathSize = maxPathSize ?? roomsX * roomsY;
    }

    public List<Vector2Int> GenerateRandomPath(int xFrom, int yFrom, int xTo, int yTo) {
        Vector2Int startPoint = new(xFrom, yFrom);
        Vector2Int endPoint = new(xTo, yTo);
        visited = new() {startPoint};
        IEnumerable<Vector2Int> dirs = directions.OrderBy(_ => Random.value);
        foreach (var dir in dirs) {
            List<Vector2Int> path = Extend(startPoint, dir, endPoint, minPathSize, maxPathSize);
            if (path == null) continue;
            path.Insert(0, startPoint);
            return path;
        }
        return null;
    }

    List<Vector2Int> Extend(Vector2Int startPoint, Vector2Int direction, Vector2Int endPoint, int minPathSize, int maxPathSize) {
        Vector2Int newPoint = startPoint + direction;
        if (!IsPointInBounds(newPoint) || visited.Contains(newPoint)) return null;
        if (newPoint == endPoint && minPathSize <= 1 && maxPathSize >= 1) return new() {newPoint};
        else if (maxPathSize < 1) return null;
        visited.Add(newPoint);
        IEnumerable<Vector2Int> dirs = directions.OrderBy(_ => Random.value);
        foreach (var dir in dirs) {
            List<Vector2Int> path = Extend(newPoint, dir, endPoint, minPathSize - 1, maxPathSize - 1);
            if (path == null) continue;
            path.Insert(0, newPoint);
            return path;
        }
        visited.Remove(newPoint);
        return null;
    }

    bool IsPointInBounds(Vector2Int point) {
        return point[0] >= 0 && point[0] < roomsX && point[1] >= 0 && point[1] < roomsY;
    }
}

[Serializable]
public class RoomCount {
    public RoomInfo roomInfo;
    public int count;
}
