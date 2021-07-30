using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class EnemySpawnScript : MonoBehaviour
{
    [SerializeField] private EnemyController enemy;

    [SerializeField] private float spawnCooldown;
    private bool spawnSuppress = false;
    private float timer;

    public TowerScript tower { private get; set; }
    public int gridIndex { get; private set; }
    public int2 gridPos { get; private set; }

    public GridSystem grid { private get; set; }

    void Update()
    {
        if (spawnSuppress)
        {
            timer += Time.deltaTime;
            if (timer >= spawnCooldown)
            {
                timer = 0f;
                spawnSuppress = false;
            }
        }
        else
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (tower != null)
        {
            //spawn location y offset by 2.25
            EnemyController aEnemy = Instantiate(enemy, this.transform.position + new Vector3(0, 2.25f, 0), this.transform.rotation);
            aEnemy.tower = this.tower;
            aEnemy.grid = this.grid;
            aEnemy.currentPos = gridPos;
            spawnSuppress = true;
        }
    }

    public void SetGridInfo(int x, int z, int gridWidth, GridSystem grid)
    {
        gridIndex = x + z * gridWidth;
        gridPos = new int2(x, z);
        this.grid = grid;
    }
}
