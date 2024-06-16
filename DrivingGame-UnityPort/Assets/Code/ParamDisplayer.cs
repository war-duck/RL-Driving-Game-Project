using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ParamDisplayer : MonoBehaviour
{
    GameEnv _gameEnv;
    [SerializeField] private TMP_Text text;
    private PlayerDataGetter playerDataGetter;
    public void Start()
    {
        _gameEnv = GameEnv.instance;
        playerDataGetter = _gameEnv.playerDataGetter;
    }

    public void Update()
    {
        text.text = playerDataGetter.GetPlayerData().ToString();
    }
}
