using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private Map map = new Map();

    // World space dimensions
    public int gridWidth,
        gridHeight,
        worldWidth,
        worldHeight;

    public void Start()
    {
        this.CreateMap();
    }

    private void CreateMap()
    {
        this.map.ComputeMap(this.gridHeight, this.gridWidth);

        GameObject wall = Resources.Load("Prefabs/Wall") as GameObject;
        float wallWidth = (float)this.worldWidth / (float)this.gridWidth;
        float wallHeight = (float)this.worldHeight / (float)this.gridHeight;
        float wallDepth = 1;
        wall.transform.localScale = new Vector3(wallWidth, wallDepth, wallHeight);
        GameObject plane = GameObject.Find("Plane");
        Action<int, int, Map.CellType> drawCell = (int x, int y, Map.CellType ct) =>
        {
            // Cavern
            if (ct == Map.CellType.Dead)
                return;
            // Wall
            else if (ct == Map.CellType.Alive)
            {
                // Start at top-left
                GameObject go = Instantiate(
                    wall,
                    this.CellCoordsToWorldCoords(x, y),
                    this.transform.rotation
                );
                go.transform.SetParent(plane.transform);
            }
            // Item
            else { }
        };
        this.map.DrawMap(drawCell);
    }

    /**
    Convert cell grid coordinates into world coordinates
    */
    public Vector3 CellCoordsToWorldCoords(int x, int y)
    {
        float wallWidth = (float)this.worldWidth / (float)this.gridWidth;
        float wallHeight = (float)this.worldHeight / (float)this.gridHeight;

        float worldX = x * wallWidth;
        float worldY = 0;
        float worldZ = (this.worldHeight) - (y * wallHeight);

        return new Vector3(worldX, worldY, worldZ);
    }

    public Map GetMap()
    {
        return this.map;
    }
}

public class Map
{
    public enum CellType
    {
        Alive,
        Dead,
        Item,
    }

    public int wallChance;

    public int birthRule,
        deathRule;

    private CellType[,] grid;
    private List<Tuple<int, int>> groundCells;
    private List<Tuple<int, int>> wallCells;

    public Map()
    {
        this.wallChance = 50;

        this.birthRule = 5;
        this.deathRule = 4;

        this.groundCells = new List<Tuple<int, int>>();
        this.wallCells = new List<Tuple<int, int>>();
    }

    public CellType GetCellType(int x, int y)
    {
        return this.grid[y, x];
    }

    public void ComputeMap(int height, int width)
    {
        this.grid = new CellType[height, width];

        this.InitNoise();
        // this.LogWallCount();
        this.SculptMap(20);
        // this.LogWallCount();
        this.EncloseMap();
        // this.LogWallCount();
        this.ConnectCaverns();
        // this.LogWallCount();

        this.RecordWallAndGroundCells();
    }

    public void DrawMap(Action<int, int, CellType> drawCell)
    {
        for (int y = 0; y < this.grid.GetLength(0); y++)
        {
            for (int x = 0; x < this.grid.GetLength(1); x++)
            {
                drawCell(x, y, this.grid[y, x]);
            }
        }
    }

    private void LogWallCount()
    {
        int wallCount = 0;
        int deadCount = 0;
        for (int y = 0; y < this.grid.GetLength(0); y++)
        {
            for (int x = 0; x < this.grid.GetLength(1); x++)
            {
                if (this.grid[y, x] == CellType.Alive)
                    wallCount++;
                else
                    deadCount++;
            }
        }

        Debug.Log(wallCount);
        Debug.Log(deadCount);
        Debug.Log("----");
    }

    // Initialize grid with noise -> 45% walls
    private void InitNoise()
    {
        bool IsAlive()
        {
            int rand = UnityEngine.Random.Range(0, 100);
            return (rand < this.wallChance);
        }

        for (int y = 0; y < this.grid.GetLength(0); y++)
        { // Each row
            for (int x = 0; x < this.grid.GetLength(1); x++)
            { // Each column
                this.grid[y, x] = IsAlive() ? CellType.Alive : CellType.Dead;
            }
        }
    }

    // Performas several iterations of CellularAutomata
    private void SculptMap(int iterations = 5)
    {
        for (int iter = 0; iter < iterations; iter++)
        {
            this.grid = CellularAutomata();
        }
    }

    // 'Births' and 'Kills' each Map cell
    // According to rules
    private CellType[,] CellularAutomata()
    {
        int height = this.grid.GetLength(0);
        int width = this.grid.GetLength(1);

        CellType[,] newGrid = new CellType[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int neighborCount = this.GetNeighborCount(x, y);

                if (neighborCount >= this.birthRule)
                {
                    newGrid[y, x] = CellType.Alive;
                }
                else if (neighborCount <= this.deathRule)
                {
                    newGrid[y, x] = CellType.Dead;
                }
            }
        }
        return newGrid;
    }

    private int GetNeighborCount(int x, int y)
    {
        int neighborCount = grid[y, x] == CellType.Dead ? 0 : 1;

        Vector2 dir = new Vector2(0, -1); // Necessary starting point (0, -1)

        float c = (Mathf.PI / 4);
        while (c < (2 * Mathf.PI))
        { // Each direction (8)
            try
            {
                dir.x = (int)Mathf.Clamp(Mathf.Round(dir.x + Mathf.Cos(c)), -1, 1);
                dir.y = (int)Mathf.Clamp(Mathf.Round(dir.y + Mathf.Sin(c)), -1, 1);

                if (
                    x + dir.x < 0
                    || x + dir.x >= this.grid.GetLength(1)
                    || y + dir.y < 0
                    || y + dir.y >= this.grid.GetLength(0)
                )
                {
                    neighborCount++;
                }
                else if (grid[x + (int)dir.x, y + (int)dir.y] == CellType.Alive)
                {
                    neighborCount++;
                }
            }
            catch (Exception e)
            {
                //print(e.Message);
            }

            c += (Mathf.PI / 4);
        }
        return neighborCount;
    }

    private void ConnectCaverns()
    {
        List<List<int>> caverns = new List<List<int>>();
        HashSet<int> seen = new HashSet<int>();

        this.FindCaverns(caverns, seen);
        this.BuildConnectingPaths(caverns);
    }

    private void FindCaverns(List<List<int>> caverns, HashSet<int> seen)
    {
        for (int y = 0; y < this.grid.GetLength(0); y++)
        {
            for (int x = 0; x < this.grid.GetLength(1); x++)
            {
                List<int> cavern = new List<int>();
                this.ExploreCavern(x, y, cavern, seen);

                // No cavern if count == 0
                if (cavern.Count > 0)
                    caverns.Add(cavern);
            }
        }

        Debug.Log(caverns.Count);
    }

    private void ExploreCavern(int x, int y, List<int> cavern, HashSet<int> seen)
    {
        int id = y * this.grid.GetLength(0) + x;

        // 1. Out-of-bounds or Wall or Already Seen
        if (
            y < 0
            || y >= this.grid.GetLength(0)
            || x < 0
            || x >= this.grid.GetLength(1)
            || this.grid[y, x] == CellType.Alive
            || seen.Contains(id)
        )
            return;

        // 2. Else in Cavern
        seen.Add(id);
        cavern.Add(id);

        // Go left/right
        ExploreCavern(x - 1, y, cavern, seen);
        ExploreCavern(x + 1, y, cavern, seen);
        // Go up/down
        ExploreCavern(x, y - 1, cavern, seen);
        ExploreCavern(x, y + 1, cavern, seen);
    }

    private void BuildConnectingPaths(List<List<int>> caverns)
    {
        for (int i = 0; i < caverns.Count - 1; i++)
        {
            // 1.1. Prev Cavern
            // Get random index from cavern
            int rand1 = UnityEngine.Random.Range(0, caverns[i].Count);
            // Get the cavern Point
            int cavernP1 = caverns[i][rand1];
            // Get the Point x and y
            int startX = cavernP1 % this.grid.GetLength(0);
            int startY = (int)Mathf.Floor((float)cavernP1 / (float)this.grid.GetLength(0));

            // 1.2. Next Cavern
            // Get random index from cavern
            int rand2 = UnityEngine.Random.Range(0, caverns[i + 1].Count);
            // Get the cavern Point
            int cavernP2 = caverns[i + 1][rand2];
            // Get the Point x and y
            int stopX = cavernP2 % this.grid.GetLength(0);
            int stopY = (int)Mathf.Floor((float)cavernP2 / (float)this.grid.GetLength(0));

            // 2. Connect the 2 points with 2 straight line components
            int x = startX;
            int y = startY;
            // 2.1. Straight line x component
            while (x != stopX)
            {
                this.grid[y, x] = CellType.Dead;

                if (x < stopX)
                    x++;
                else
                    x--;
            }

            // 2.2. Straight line y component
            while (y != stopY)
            {
                this.grid[y, x] = CellType.Dead;

                if (y < stopY)
                    y++;
                else
                    y--;
            }
        }
    }

    private void EncloseMap()
    {
        // Enclose top and bottom of map
        for (int x = 0; x < this.grid.GetLength(1); x++)
        {
            this.grid[0, x] = CellType.Alive;
            this.grid[this.grid.GetLength(0) - 1, x] = CellType.Alive;
        }

        // Enclose left and right of map
        for (int y = 0; y < this.grid.GetLength(0); y++)
        {
            this.grid[y, 0] = CellType.Alive;
            this.grid[y, this.grid.GetLength(1) - 1] = CellType.Alive;
        }
    }

    private void RecordWallAndGroundCells()
    {
        // 1. Reset wall and ground cells
        this.wallCells = new List<Tuple<int, int>>();
        this.groundCells = new List<Tuple<int, int>>();

        // 2. Traverse
        int height = this.grid.GetLength(0);
        int width = this.grid.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 3. Record wall and ground cells
                if (this.grid[y, x] == CellType.Alive)
                {
                    this.wallCells.Add(new Tuple<int, int>(y, x));
                }
                else
                {
                    this.groundCells.Add(new Tuple<int, int>(y, x));
                }
            }
        }
    }

    public List<Tuple<int, int>> GetWallCells()
    {
        return this.wallCells;
    }

    public List<Tuple<int, int>> GetGroundCells()
    {
        return this.groundCells;
    }
}
