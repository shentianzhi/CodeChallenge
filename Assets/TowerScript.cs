using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class TowerScript : MonoBehaviour
{
    public int gridIndex { get; private set; }
    public int2 gridPos { get; private set; }
    public GridSystem grid { get; private set; }

    private void Start()
    {
        EnemySpawnScript[] spawnPoints =  FindObjectsOfType<EnemySpawnScript>();
        foreach(EnemySpawnScript spawnPoint in spawnPoints)
        {
            spawnPoint.tower = this;
        }
        //Debug.Log("Tower position: " + transform.position) ;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        { 
            enemy.gameObject.SetActive(false);
        }
    }

    public void SetGridInfo(int x, int z, int gridWidth, GridSystem grid)
    {
        gridIndex = x + z * gridWidth;
        gridPos = new int2(x, z);
        this.grid = grid;
    }
}
