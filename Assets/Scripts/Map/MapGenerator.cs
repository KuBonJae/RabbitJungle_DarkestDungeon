using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    [SerializeField] private GameObject mapPrefab;
    [SerializeField] private GameObject bossMapPrefab;
    [SerializeField] private GameObject itemMapPrefab;
    [SerializeField] private GameObject specialJobUI;
    private int size = 20; // 그리드 크기
    private int roomCount = 15; // 생성할 방의 개수
    private int[,] stage;
    private System.Random rand = new System.Random();
    private Tuple<int, int> farthestRoom;
    private Tuple<int, int> itemRoom;
    private Tuple<int, int> itemRoomSub;
    private int[] dx = { 0, 1, 0, -1 }; // 오른쪽, 아래, 왼쪽, 위쪽
    private int[] dy = { 1, 0, -1, 0 };

    void Start()
    {
        GenerateStage();
        PrintStage();
        farthestRoom = FindFarthestRoom();
        if (itemRoom.Item1 == farthestRoom.Item1 && itemRoom.Item2 == farthestRoom.Item2)
        {
            itemRoom = itemRoomSub;
        }
        Debug.Log($"가장 먼 방: ({farthestRoom.Item1}, {farthestRoom.Item2})");
        Debug.Log($"아이템 방: ({itemRoom.Item1}, {itemRoom.Item2})");
        CreateStage();
    }
    /// <summary>
    /// 스테이지 생성
    /// </summary>
    private void GenerateStage()
    {
        // 스테이지 초기화
        stage = new int[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                stage[i, j] = -1; // 방이 없는 상태
            }
        }

        // 중심에서 시작
        int centerX = size / 2;
        int centerY = size / 2;
        stage[centerX, centerY] = 0;
        List<Tuple<int, int>> rooms = new List<Tuple<int, int>>();
        rooms.Add(new Tuple<int, int>(centerX, centerY));

        int currentRoom = 1;

        // 방 생성
        while (currentRoom < roomCount)
        {
            // 더 이상 방을 추가할 수 없는 경우 중단
            if (rooms.Count == 0)
            {
                Debug.Log("더 이상 방을 추가할 수 없습니다.");
                break;
            }

            // 랜덤한 방 선택
            var room = rooms[rand.Next(rooms.Count)];
            int x = room.Item1;
            int y = room.Item2;

            // 방이 있는 공간 확인
            List<int> directions = new List<int> { 0, 1, 2, 3 };
            bool createdChild = false;
            while (directions.Count > 0)
            {
                int dirIndex = rand.Next(directions.Count);
                int dir = directions[dirIndex];
                directions.RemoveAt(dirIndex);

                int nx = x + dx[dir];
                int ny = y + dy[dir];

                // 새로운 방이 범위 내에 있고, 인접한 칸이 다른 방과 접하지 않는지 확인
                if (IsInBounds(nx, ny) && stage[nx, ny] == -1 && !IsAdjacentToRoom(nx, ny))
                {
                    stage[nx, ny] = currentRoom;
                    rooms.Add(new Tuple<int, int>(nx, ny));
                    currentRoom++;
                    createdChild = true;
                    if (currentRoom == 11)
                    {
                        itemRoom = new Tuple<int, int>(nx, ny);
                    }
                    if (currentRoom == 12)
                    {
                        itemRoomSub = new Tuple<int, int>(nx, ny);
                    }
                    break;
                }
            }

            // 방을 생성하지 못했다면 리스트에서 제거
            if (!createdChild)
            {
                rooms.Remove(room);
            }
        }
    }
    /// <summary>
    /// 해당 좌표가 범위 내에 있는지 확인
    /// </summary>
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < size && y >= 0 && y < size;
    }
    /// <summary>
    /// 해당 공간이 다른 방과 인접한지 확인
    /// 이때 부모방인 경우는 제외
    /// </summary>
    private bool IsAdjacentToRoom(int x, int y)
    {
        int count = 0;
        for (int dir = 0; dir < 4; dir++)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];
            if (IsInBounds(nx, ny) && stage[nx, ny] != -1)
            {
                count++;
            }
        }
        return count != 1;
    }
    /// <summary>
    /// 스테이지 출력
    /// </summary>
    private void PrintStage()
    {
        string stageString = "";
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (stage[i, j] == -1)
                {
                    stageString += "██";
                }
                else
                {
                    stageString += $"{stage[i, j]:D2} ";
                }
            }
            stageString += "\n";
        }
        Debug.Log(stageString);
    }
    /// <summary>
    /// 실제 스테이지 생성
    /// </summary>
    private void CreateStage()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject currentMap = null;
                if (stage[i, j] != -1)
                {
                    //보스방일때
                    if (i == farthestRoom.Item1 && j == farthestRoom.Item2)
                    {
                        currentMap = Instantiate(bossMapPrefab, new Vector3((i - size / 2) * 75, (j - size / 2) * 75, 0), Quaternion.identity);
                        currentMap.GetComponent<RoomData>().RoomType = RoomType.Boss.ToString();
                    }
                    else if (i == itemRoom.Item1 && j == itemRoom.Item2)
                    {
                        currentMap = Instantiate(itemMapPrefab, new Vector3((i - size / 2) * 75, (j - size / 2) * 75, 0), Quaternion.identity);
                        currentMap.GetComponent<RoomData>().RoomType = RoomType.Item.ToString();
                        currentMap.transform.Find("Item").GetComponent<SpecialJobController>().specialJobsUI = specialJobUI;
                        if (DataManager.Instance.Weapon == WeaponType.Gun.ToString())
                        {
                            Transform item = currentMap.transform.Find("Item");
                            item.GetComponent<SpecialJobController>().jobButtons.Add(specialJobUI.transform.Find("Shotgun").GetComponent<UnityEngine.UI.Button>());
                            item.GetComponent<SpecialJobController>().jobButtons.Add(specialJobUI.transform.Find("Rifle").GetComponent<UnityEngine.UI.Button>());
                            item.GetComponent<SpecialJobController>().jobButtons.Add(specialJobUI.transform.Find("Sniper").GetComponent<UnityEngine.UI.Button>());

                        }
                        else if (DataManager.Instance.Weapon == WeaponType.Sword.ToString())
                        {
                            Transform item = currentMap.transform.Find("Item");
                            item.GetComponent<SpecialJobController>().jobButtons.Add(specialJobUI.transform.Find("LongSword").GetComponent<UnityEngine.UI.Button>());
                            item.GetComponent<SpecialJobController>().jobButtons.Add(specialJobUI.transform.Find("ShortSword").GetComponent<UnityEngine.UI.Button>());
                            item.GetComponent<SpecialJobController>().jobButtons.Add(specialJobUI.transform.Find("Axe").GetComponent<UnityEngine.UI.Button>());
                        }
                    }
                    else
                    {
                        currentMap = Instantiate(mapPrefab, new Vector3((i - size / 2) * 75, (j - size / 2) * 75, 0), Quaternion.identity);
                        currentMap.GetComponent<RoomData>().RoomType = RoomType.Battle.ToString();
                    }
                }
                //문활성화
                if (currentMap != null)
                {
                    for (int dir = 0; dir < 4; dir++)
                    {
                        int nx = i + dx[dir];
                        int ny = j + dy[dir];
                        if (IsInBounds(nx, ny) && stage[nx, ny] != -1)
                        {
                            Transform doorTransform = null;
                            switch (dir)
                            {
                                // 오른쪽 - 위쪽
                                case 0:
                                    doorTransform = currentMap.transform.GetChild(3).GetChild(0);
                                    if (doorTransform != null)
                                    {
                                        doorTransform.gameObject.SetActive(true);
                                        //Debug.Log("오른쪽 문 활성화");
                                    }
                                    else
                                    {
                                        //Debug.LogWarning("오른쪽 문을 찾을 수 없습니다.");
                                    }
                                    break;
                                // 아래 - 오른쪽
                                case 1:
                                    doorTransform = currentMap.transform.GetChild(2).GetChild(0);
                                    if (doorTransform != null)
                                    {
                                        doorTransform.gameObject.SetActive(true);
                                        //Debug.Log("아래 문 활성화");
                                    }
                                    else
                                    {
                                        //Debug.LogWarning("아래 문을 찾을 수 없습니다.");
                                    }
                                    break;
                                // 왼쪽 - 아래쪽
                                case 2:
                                    doorTransform = currentMap.transform.GetChild(4).GetChild(0);
                                    if (doorTransform != null)
                                    {
                                        doorTransform.gameObject.SetActive(true);
                                        //Debug.Log("왼쪽 문 활성화");
                                    }
                                    else
                                    {
                                        //Debug.LogWarning("왼쪽 문을 찾을 수 없습니다.");
                                    }
                                    break;
                                // 위쪽 - 왼쪽
                                case 3:
                                    doorTransform = currentMap.transform.GetChild(1).GetChild(0);
                                    if (doorTransform != null)
                                    {
                                        doorTransform.gameObject.SetActive(true);
                                        //Debug.Log("위쪽 문 활성화");
                                    }
                                    else
                                    {
                                        //Debug.LogWarning("위쪽 문을 찾을 수 없습니다.");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// 가장 먼 방 찾기
    /// </summary>
    private Tuple<int, int> FindFarthestRoom()
    {
        int centerX = size / 2;
        int centerY = size / 2;
        int[,] distances = new int[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                distances[i, j] = int.MaxValue;
            }
        }

        Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
        queue.Enqueue(new Tuple<int, int>(centerX, centerY));
        distances[centerX, centerY] = 0;

        Tuple<int, int> farthestRoom = new Tuple<int, int>(centerX, centerY);
        int maxDistance = 0;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int x = current.Item1;
            int y = current.Item2;
            int currentDistance = distances[x, y];

            for (int dir = 0; dir < 4; dir++)
            {
                int nx = x + dx[dir];
                int ny = y + dy[dir];

                if (IsInBounds(nx, ny) && stage[nx, ny] != -1 && distances[nx, ny] == int.MaxValue)
                {
                    distances[nx, ny] = currentDistance + 1;
                    queue.Enqueue(new Tuple<int, int>(nx, ny));

                    if (distances[nx, ny] > maxDistance)
                    {
                        maxDistance = distances[nx, ny];
                        farthestRoom = new Tuple<int, int>(nx, ny);
                    }
                }
            }
        }
        return farthestRoom;
    }

}
