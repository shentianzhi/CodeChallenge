using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretDetector : MonoBehaviour
{
    [SerializeField] private TurretController turret;

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            turret.AddToTargetList(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            turret.RemoveFromList(enemy);
        }
    }
}
