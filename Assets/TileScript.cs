using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.AI;

public class TileScript : MonoBehaviour
{
    public bool isOccupied;
    public MeshRenderer tileRender;
    [SerializeField] private NavMeshSurface navMesh;
    public int2 gridPos;

    private void Start()
    {
        navMesh.BuildNavMesh();
    }

    public bool IsWalkable()
    {
        return !isOccupied;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<TowerScript>())
        {
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isOccupied = false;
    }

    public void ChangeTileMat(Material mat)
    {
        tileRender.material = mat;
    }
}
