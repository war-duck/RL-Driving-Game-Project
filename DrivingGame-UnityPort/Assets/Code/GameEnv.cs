using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameEnv : MonoBehaviour
{
    public static GameEnv instance;
    [SerializeField] private Rigidbody2D _player;
    [SerializeField] public Collider2D terrainCollider;
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
    public void RestartGame()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
