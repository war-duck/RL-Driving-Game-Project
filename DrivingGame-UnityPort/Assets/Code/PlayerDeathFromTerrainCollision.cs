using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathFromTerrainCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Terrain"))
        {
            GameEnv.instance.GameOver();
        }
    }
}
