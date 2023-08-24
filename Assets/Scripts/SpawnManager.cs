using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static readonly List<Type> ATTACK_BEHAVIOR_TYPES = new List<Type>()
    {
        typeof(BombAttack),
        typeof(LaserAttack),
        typeof(OrbitAttack),
        typeof(SpikeAttack)
    };

    private MapManager mm;
    private int minAlive = 20;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (this.mm == null)
            this.mm = FindObjectOfType<MapManager>();

        this.SpawnMinAlive();
    }

    /**
    Spawns entities until the minimum number of entities are alive
    */
    private void SpawnMinAlive()
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
        // TODO 8/21/2023: Create Enemy Prefab
        GameObject enemy = Resources.Load("Prefabs/Enemy") as GameObject;
        GameObject go = Instantiate(
            enemy,
            this.mm.CellCoordsToWorldCoords(x, y),
            this.transform.rotation
        );

        // 3. Add an AttackBehavior to GameObject
        go.AddComponent(this.GetRandAttackBehaviorType());
    }

    private int GetAliveCount()
    {
        return FindObjectsOfType<Brain>().Length;
    }

    private Type GetRandAttackBehaviorType()
    {
        return RandUtils.GetRandListElement(SpawnManager.ATTACK_BEHAVIOR_TYPES);
    }
}
