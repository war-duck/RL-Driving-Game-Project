using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathFromTerrainCollision : MonoBehaviour
{
    [SerializeField] private DriverAgent _driverAgent;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Terrain"))
        {
            _driverAgent.gotKilled();
        }
    }
}
