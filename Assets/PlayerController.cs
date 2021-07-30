using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TurretController turret;
    [SerializeField] private Camera mainCam;

    public LayerMask mouseIgnore;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if( Physics.Raycast(ray, out hit, 10000f, ~mouseIgnore))
            {
                TurretController aTurret;
                if (aTurret = hit.collider.gameObject.GetComponent<TurretController>())
                {
                    RemoveTurret(aTurret);
                }
                else if (hit.collider.gameObject.GetComponent<TurretSpawnPoint>())
                {
                    TileScript aTile = hit.collider.gameObject.GetComponent<TileScript>();
                    if (aTile != null)
                    {
                        if (!aTile.isOccupied)
                        {
                            aTile.isOccupied = true;
                            Vector3 spawnPos = aTile.gameObject.transform.position + new Vector3(0, 20f, 0f);
                            Instantiate(turret, spawnPos, new Quaternion(0f, 180f, 0f, 0f)).occupiedTile = aTile;
                        }
                    }                   
                }
            }
        }
    }

    private void RemoveTurret(TurretController aTurret)
    {
        aTurret.occupiedTile.isOccupied = false;
        aTurret.gameObject.SetActive(false);
    }
}
