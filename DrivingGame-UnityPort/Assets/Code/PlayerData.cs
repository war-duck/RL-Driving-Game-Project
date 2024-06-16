using UnityEngine;
public class PlayerDataGetter
{
    public bool HasDied;
    private Rigidbody2D player;
    private CircleCollider2D[] wheelColliders;
    private static ContactFilter2D terrainFilter;
    private static Collider2D terrainCollider;
    private static readonly Vector2 offsetFront = new Vector2(3, 1000), offsetBack = new Vector2(4, 1000);
    public PlayerDataGetter(Rigidbody2D player)
    {
        this.player = player;
        this.wheelColliders = player.GetComponentsInChildren<CircleCollider2D>();
        LayerMask terrainLayer = LayerMask.GetMask("Terrain");
        terrainFilter = new ContactFilter2D();
        terrainFilter.SetLayerMask(terrainLayer);
        terrainCollider = GameEnv.instance.terrainCollider;

    }
    public static PlayerData GetPlayerData(Rigidbody2D player, CircleCollider2D[] wheelColliders)
    {
        PlayerData playerData = new PlayerData();
        playerData.GlobalPositionX = (int)player.position.x;
        playerData.Rotation = (int)player.rotation % 360;
        playerData.Slope = GetSlope(player);
        playerData.DistToGround = (int)(player.Distance(terrainCollider).distance*100);
        playerData.AngularVelocity = (int)player.angularVelocity;
        playerData.DistToGround = (int)(player.Distance(terrainCollider).distance * 100);
        playerData.AngularVelocity = (int)player.angularVelocity/10;
        playerData.IsTouchingGround = IsTouchingLayers(player, wheelColliders) ? 1 : 0;
        playerData.HasDied = 0;
        return playerData;
    }
    public PlayerData GetPlayerData()
    {
        PlayerData playerData = GetPlayerData(player, wheelColliders);
        playerData.HasDied = this.HasDied ? 1 : 0;
        return playerData;
    }
    private static bool IsTouchingLayers(Rigidbody2D player, CircleCollider2D[] wheelColliders)
    {
        foreach (CircleCollider2D wheelCollider in wheelColliders)
        {
            if (wheelCollider.IsTouchingLayers())
            {
                return true;
            }
        }
        return player.IsTouchingLayers();
    }
    public static int GetSlope(Rigidbody2D player)
    {
        Vector2 origin = player.position;
        Vector2 direction = Vector2.down;
        RaycastHit2D[] hits = new RaycastHit2D[1];
        if (Physics2D.Raycast(origin + offsetBack, direction, terrainFilter, hits) == 0)
        {
            return int.MaxValue;
        }
        RaycastHit2D backHit = hits[0];
        Debug.DrawRay(origin + offsetBack, direction * backHit.distance, Color.green, duration: 0.0f, depthTest: false);  
        if (Physics2D.Raycast(origin + offsetFront, direction, terrainFilter, hits) == 0)
        {
            return int.MaxValue;
        }
        RaycastHit2D frontHit = hits[0];
        Debug.DrawRay(origin + offsetFront, direction * frontHit.distance, Color.green, duration: 0.0f, depthTest: false);
        return (int)(-Mathf.Atan2(backHit.distance - frontHit.distance, offsetBack.x - offsetFront.x) * Mathf.Rad2Deg);
          
    }
}
public struct PlayerData
{
    public int GlobalPositionX;
    public int Rotation;
    public int Slope;
    public int DistToGround;
    public int AngularVelocity;
    public int IsTouchingGround;
    public int HasDied;
    public override string ToString()
    {
        return $"GlobalPositionX: {GlobalPositionX}\nRotation: {Rotation}\nSlope: {Slope}\nDistToGround: {DistToGround}\nAngularVelocity: {AngularVelocity}\nIsTouchingGround: {IsTouchingGround}\nHasDied: {HasDied}";
    }
}
