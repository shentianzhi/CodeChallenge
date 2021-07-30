using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Mathematics;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int HP;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Pathfinding pathFinding;

    public GridSystem grid { private get; set; }
    public TowerScript tower { private get; set; }

    private MeshRenderer mRender;
    private bool isFlashing;
    private float flashDuration;

    public int2 currentPos { get; set; }
    public bool moveToNext { get; set; } = true;

    private List<int> path = new List<int>();
    private List<Vector3> waypoints = new List<Vector3>();
    private int pathIndex = -1;
    private Vector3 targetPos;

    private void Start()
    {
        mRender = this.GetComponent<MeshRenderer>();
        pathFinding.grid = this.grid;
        /*path = pathFinding.FindPath(currentPos, tower.gridPos, new int2(grid.gridWidth, grid.gridHeight));
        foreach(int i in path)
        {
            //For testing
            //grid.tiles[i].tileRender.material.color = Color.red;

            Vector3 pos = grid.tiles[i].transform.position;
            pos.y = transform.position.y;
            waypoints.Add(pos);
            Debug.Log(pos);
        }*/
        
    }

    private void Update()
    {
        /*if (!moveToNext)
        {
            moveToNext = HasReachedPosition(targetPos);
        }
        else if (path.Count > 0 && pathIndex < path.Count - 1)
        {
            moveToNext = false;
            pathIndex++;

            agent.SetDestination(waypoints[pathIndex]);
        }*/

        if (!moveToNext)
        {
            moveToNext = HasReachedPosition(targetPos);
        }
        else
        {
            moveToNext = false;
            UpdateDestination();
        }

    }

    private bool HasReachedPosition(Vector3 pos)
    {
        float dis = Mathf.Abs(Vector3.Distance(transform.position, pos));
        if (dis <= .1f)
        {
            return true;
        }

        return false;
    }

    public void OnDamaged(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            OnDeath();
        }
        else
        {
            if (!isFlashing)
            {
                isFlashing = true;
                StartCoroutine(DamagedFlash());
            }
            else
            {
                flashDuration = 0f;
            }          
        }
    }

    IEnumerator DamagedFlash()
    {
        mRender.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        mRender.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);

        flashDuration += 0.2f;
        if (flashDuration < 1f)
        {
            StartCoroutine(DamagedFlash());
        }
        else
        {
            flashDuration = 0;
        }
    }

    private void OnDeath()
    {
        this.gameObject.SetActive(false);
    }

    private void UpdateDestination()
    {
        path = pathFinding.FindPath(currentPos, tower.gridPos, new int2(grid.gridWidth, grid.gridHeight));
        if (path.Count > 0)
        {
            Vector3 pos = grid.tiles[path[0]].gameObject.transform.position;
            print(pos);
            pos.y = transform.position.y;
            agent.SetDestination(pos);

            currentPos = grid.tiles[0].gridPos;           
        }        
    }
}
