using Godot;
using System;
using System.Linq;
using System.Security.Cryptography;

public class RLAPI
{
    private Player player;

    public RLAPI(Player player)
    {
        this.player = player;
    }

    public PlayerData GetPlayerData()
    {
        return player.playerData;   
    }

    public void ApplyModelInput(InputType input)
    {
        player.setCurrentInput(input);
    }
}