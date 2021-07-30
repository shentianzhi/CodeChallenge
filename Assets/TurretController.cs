using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private GameObject laserSpawn1;
    [SerializeField] private GameObject laserSpawn2;
    [SerializeField] private GameObject weapon;
    [SerializeField] private LaserController laser;
    [SerializeField] private float fireCooldown;
    [SerializeField] private float rotationSpeed;

    private float timer;
    private bool fireSuppress;

    private List<EnemyController> enemies = new List<EnemyController>();

    public TileScript occupiedTile { get; set; }

    private void Start()
    {
        this.GetComponent<Rigidbody>().velocity = Vector3.down * 20f;
    }


    // Update is called once per frame
    void Update()
    {
        Aim();

        if (fireSuppress)
        {
            timer += Time.deltaTime;
            if (timer >= fireCooldown)
            {
                timer = 0f;
                fireSuppress = false;
            }
        }
        else if (enemies.Count > 0)
        {
            Shoot();
        }     
    }

    private void Shoot()
    {
        //Shoot 2 lasers
        Instantiate(laser, laserSpawn1.transform.position, laserSpawn1.transform.rotation);
        Instantiate(laser, laserSpawn2.transform.position, laserSpawn2.transform.rotation);
        fireSuppress = true;
    }

    public void AddToTargetList(EnemyController aEnemy)
    {
        enemies.Add(aEnemy);
    }

    public void RemoveFromList(EnemyController aEnemy)
    {
        enemies.Remove(aEnemy);
    }

    private void Aim()
    {
        if (enemies.Count > 0)
        {
            if (enemies[0].isActiveAndEnabled)
            {
                Vector3 lookDir = Vector3.Normalize(enemies[0].transform.position - this.transform.position);
                Quaternion lookRot = Quaternion.LookRotation(lookDir, Vector3.up);
                lookRot.x = 0;
                lookRot.z = 0;

                weapon.transform.rotation = lookRot;
            }
            else
            {
                enemies.RemoveAt(0);
            }
        }
        else
        {
            weapon.transform.localRotation = Quaternion.Lerp(weapon.transform.localRotation, Quaternion.identity, Time.deltaTime * rotationSpeed);
        }
    }
}
