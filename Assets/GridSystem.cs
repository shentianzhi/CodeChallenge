using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private TileScript tile;

    public int gridWidth;
    public int gridHeight;
    public float gap = 2;

    [SerializeField] private int2 turretSpawnXIndexRange;
    [SerializeField] private int2 turretSpawnZIndexRange;
    [SerializeField] private Material turretSpawnTileMat;

    [SerializeField] private EnemySpawnScript spawnPoint;
    [SerializeField] private int2[] enemySpawnCoords;

    [SerializeField] private TowerScript tower;
    [SerializeField] private int2 towerSpawnCoord;

    public List<TileScript> tiles { get; private set; } = new List<TileScript>();

    // Start is called before the first frame update
    void Start()
    {
        ConstructGrid();
    }

    private void ConstructGrid()
    {
        for(int z = 0; z < gridHeight; z++)
        {
            Vector3 pos = transform.position;
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 temp;
                temp.x = pos.x + gap * x;
                temp.z = pos.z + gap * z;
                temp.y = pos.y;
                TileScript aTile = Instantiate(tile, temp, Quaternion.identity);
                aTile.gridPos = new int2(x, z);

                tiles.Add(aTile);

                //For testing
                /*if (x == enemySpawnCoords[0].x && z == enemySpawnCoords[0].y)
                {
                    Instantiate(spawnPoint, temp + new Vector3(0f, .15f, 0f), quaternion.identity).SetGridInfo(x, z, gridWidth, this);
                }*/

                if (x >= turretSpawnXIndexRange.x && x <= turretSpawnXIndexRange.y 
                    && z >= turretSpawnZIndexRange.x && z <= turretSpawnZIndexRange.y)
                {
                    aTile.gameObject.AddComponent<TurretSpawnPoint>();
                    aTile.ChangeTileMat(turretSpawnTileMat);
                }
                else
                {
                    for (int i = 0; i < enemySpawnCoords.Length; i++)
                    {
                        if (x == towerSpawnCoord.x && z == towerSpawnCoord.y)
                        {
                            Instantiate(tower, temp + new Vector3(0f, .55f, 0f), quaternion.identity).SetGridInfo(x, z, gridWidth, this);
                        }
                        else if (x == enemySpawnCoords[i].x && z == enemySpawnCoords[i].y)
                        {
                            Instantiate(spawnPoint, temp + new Vector3(0f, .15f, 0f), quaternion.identity).SetGridInfo(x, z, gridWidth, this);
                        }
                    }
                }
            }
        }
    }

    public bool IsTileWalkable(int index)
    {
        return tiles[index].IsWalkable();
    }

    
    public Vector3 GetTileWorldPosition(int index)
    {
        //For some reason it returns an inaccurate world position. Modifies the position value so it returns the right world position.
        Vector3 temp = tiles[index].transform.position;
        temp.x *= -1;
        temp.y += 0.6f;
        temp.z *= -1;
        //print(temp);

        return temp;
    }
}
