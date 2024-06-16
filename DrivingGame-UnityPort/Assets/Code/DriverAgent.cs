using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class DriverAgent : Agent
{
    [SerializeField] public Rigidbody2D player;
    [SerializeField] private Driver driver;
    public override void OnEpisodeBegin()
    {
        GameEnv.RestartGame();
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        driver._moveInput = actions.DiscreteActions[0] == 1 ? 1 : -1;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        PlayerData playerData = PlayerDataGetter.GetPlayerData(player, player.GetComponents<CircleCollider2D>());
        sensor.AddObservation(playerData.Rotation);
        sensor.AddObservation(playerData.Slope);
        sensor.AddObservation(playerData.DistToGround);
        sensor.AddObservation(playerData.AngularVelocity);
        sensor.AddObservation(playerData.IsTouchingGround);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;
        int input = (int)Input.GetAxis("Horizontal");
        discreteActionsOut[0] = input + 1;
    }
    public void gotKilled()
    {
        AddReward(-80f);
        Debug.Log("GotKilled");
        EndEpisode();
    }
    public void gotToGoal()
    {
        Debug.Log("gotToGoal");
        AddReward(10f);
    }
    public void gotToFinalGoal()
    {
        Debug.Log("gotToFinalGoal");
        AddReward(200f);
        EndEpisode();
    }
    public void waitedTooLong()
    {
        Debug.Log("waitedTooLong");
        AddReward(-20f);
    }
    public void backedUpTooMuch()
    {
        Debug.Log("backedUpTooMuch");
        AddReward(-50f);
    }
    public void gotBugged()
    {
        Debug.Log("gotBugged");
        EndEpisode();
    }
    public void sittingInTheCornerPenalty()
    {
        Debug.Log("SittingInTheCorner");
        AddReward(-10f);
    }
}
