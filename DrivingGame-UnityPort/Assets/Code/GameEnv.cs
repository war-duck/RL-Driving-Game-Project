using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameEnv : MonoBehaviour
{
    public static GameEnv instance;
    [SerializeField] private Rigidbody2D _player;
    [SerializeField] public Collider2D terrainCollider;
    [SerializeField] private Driver driver;
    public PlayerDataGetter playerDataGetter;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            playerDataGetter = new PlayerDataGetter(_player);
        }
    }

    public void GameOver()
    {
        playerDataGetter.HasDied = true;
    }
    public static void RestartGame()
    {
        instance.driver.isReturning = true;
    }
}
