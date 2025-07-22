using UnityEngine;

public class LevelMapManager : MonoBehaviour
{
    public Transform mapRoot;
    public GameObject roomPrefab;
    public GameObject roomBossPrefab;
    public float roomOffsetX, roomOffsetY;
    public GameObject passPrefab;

    bool mapActive = true;
    GameObject[,] roomsMap;
    GameObject[,] passMapHorizontal; // проходы от комнат вниз
    GameObject[,] passMapVertical; // проходы от комнат вправо

    void Start() {
        GenerateMap();
    }

    void GenerateMap() {
        mapRoot.gameObject.SetActive(mapActive);
        LevelManager lm = FindFirstObjectByType<LevelManager>();
        roomsMap = new GameObject[lm.roomsX, lm.roomsY];
        passMapHorizontal = new GameObject[lm.roomsX - 1, lm.roomsY];
        passMapVertical = new GameObject[lm.roomsX, lm.roomsY - 1];
        for (int x = 0; x < lm.roomsX; ++x) {
            for (int y = 0; y < lm.roomsY; ++y) {
                if (lm.GetRoom(x, y) == null) continue;
                roomsMap[x, y] = Instantiate(x == lm.roomsX - 1 && y == lm.roomsY - 1 ? roomBossPrefab : roomPrefab, mapRoot, false);
                roomsMap[x, y].name = $"{roomPrefab.name} {x} {y}";
                roomsMap[x, y].transform.localPosition = new(x * roomOffsetX, -y * roomOffsetY, 0);
                if (x != lm.roomsX - 1) {
                    passMapHorizontal[x, y] = Instantiate(passPrefab, mapRoot, false);
                    passMapHorizontal[x, y].transform.localPosition = new((x + 0.5f) * roomOffsetX, -y * roomOffsetY, 0);
                    passMapHorizontal[x, y].SetActive(lm.GetRoom(x, y).roomInfo.rd.right == DirAvailability.Available);
                }
                if (y != lm.roomsY - 1) {
                    passMapVertical[x, y] = Instantiate(passPrefab, mapRoot, false);
                    passMapVertical[x, y].transform.localPosition = new(x * roomOffsetX, -(y + 0.5f) * roomOffsetY, 0);
                    passMapVertical[x, y].transform.localEulerAngles += new Vector3(0, 0, 90);
                    passMapVertical[x, y].SetActive(lm.GetRoom(x, y).roomInfo.rd.down == DirAvailability.Available);
                }
            }
        }
    }

    public void SetMapActive(bool active) {
        mapActive = active;
        mapRoot.gameObject.SetActive(active);
    }

    public void ChangeMapActive() {
        SetMapActive(!mapActive);
    }
}
