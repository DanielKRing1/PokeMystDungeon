using System;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private MapManager mm;
    private int minAlive = 2;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (this.mm == null)
            this.mm = FindObjectOfType<MapManager>();

        this.MeetSpawnMin();
    }

    /**
    Spawns entities until the minimum number of entities are alive
    */
    private void MeetSpawnMin()
    {
        int aliveCount = this.GetAliveCount();

        while (aliveCount < this.minAlive)
        {
            this.Spawn();

            aliveCount++;
        }
    }

    private void Spawn()
    {
        // 1. Get a random cell to spawn GameObject at
        Tuple<int, int> spawnableCell = RandUtils.GetRandListElement(
            this.mm.GetMap().GetGroundCells()
        );
        int x = spawnableCell.Item1;
        int y = spawnableCell.Item2;

        // 2. Spawn GameObject at cell (convert to world coords)
        GameObject enemy = Resources.Load("Prefabs/Enemy") as GameObject;
        GameObject go = Instantiate(
            enemy,
            Vector3.zero,
            // this.mm.CellCoordsToWorldCoords(x, y),
            this.transform.rotation
        );
    }

    private int GetAliveCount()
    {
        return FindObjectsOfType<Brain>().Length;
    }
}
