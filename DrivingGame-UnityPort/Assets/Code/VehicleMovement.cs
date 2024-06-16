using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _frontWheelRB;
    [SerializeField] private Rigidbody2D _backWheelRB;
    [SerializeField] private Rigidbody2D _carRB;
    [SerializeField] private float _rotationSpeed = 1000f;
    [SerializeField] private float _maxWheelAngularVelocity = 1000f;
    [SerializeField] private float _speed = 800f;
    [SerializeField] private DriverAgent _driverAgent;
    private float nextGoalDistance = 6f;
    private float maxBackUpDist = 18f;
    private float nextGoal;
    public float _moveInput = 0f;
    private int timer = 0;
    private int timeoutCounter = 0;
    public bool isReturning = false;
    public int wait = 2;
    public LayerMask terrainLayer;
    private void Start()
    {
        nextGoal = _carRB.position.x + nextGoalDistance;
        terrainLayer = LayerMask.GetMask("Terrain");
    }

    private void Update()
    {
        ++timer;
        if (_carRB.position.x > nextGoal)
        {
            timer = 0;
            timeoutCounter = 0;
            nextGoal = _carRB.position.x + nextGoalDistance;
            _driverAgent.gotToGoal();
            if (nextGoal > 700)
            {
                _driverAgent.gotToFinalGoal();
            }
        }
        if (_carRB.position.x < nextGoal - maxBackUpDist)
        {
            nextGoal = _carRB.position.x + nextGoalDistance;
            _driverAgent.backedUpTooMuch();
        }
        if (timer > 90)
        {
            timer = 0;
            ++timeoutCounter;
            _driverAgent.waitedTooLong();
        }
        if (timeoutCounter > 4)
        {
            timeoutCounter = 0;
            _driverAgent.gotKilled();
        }
        if (
            PlayerDataGetter.GetPlayerData(_carRB, _carRB.GetComponents<CircleCollider2D>()).DistToGround > 500f
            || _backWheelRB.position.y < -10f
            || _frontWheelRB.position.y < -10f
            || _backWheelRB.position.x < -10f
            || _frontWheelRB.position.x < -10f
            )
        {
            _driverAgent.gotBugged();
        }
        if (PlayerDataGetter.GetPlayerData(_carRB, _carRB.GetComponents<CircleCollider2D>()).GlobalPositionX < -5)
            _driverAgent.sittingInTheCornerPenalty();
    }

    private void FixedUpdate()
    {
        if (isReturning)
        {
            _driverAgent.player.Sleep();
            _carRB.excludeLayers = terrainLayer;
            _backWheelRB.excludeLayers = terrainLayer;
            _frontWheelRB.excludeLayers = terrainLayer;
            if (_carRB.ClosestPoint(new Vector2(0, 0)).magnitude == 0)
            {
                if (--wait > 0)
                    return;
                isReturning = false;
                _carRB.excludeLayers = (LayerMask)16;
                _backWheelRB.excludeLayers = (LayerMask)16;
                _frontWheelRB.excludeLayers = (LayerMask)16;
                _driverAgent.player.WakeUp();
                wait = 2;
                return;
            }
            else
                Debug.Log(_carRB.ClosestPoint(new Vector2(5, 0)).magnitude);
            _carRB.MovePosition(new Vector2(0, 0));
            _carRB.MoveRotation(0);
            _frontWheelRB.MoveRotation(0);
            _backWheelRB.MoveRotation(0);
            _carRB.velocity = Vector2.zero;
            _backWheelRB.velocity = Vector2.zero;
            _frontWheelRB.velocity = Vector2.zero;
            _carRB.angularVelocity = 0;
            _backWheelRB.angularVelocity = 0;
            _frontWheelRB.angularVelocity = 0;
        return;
        }
        Debug.Log(_moveInput);
        if (_frontWheelRB.angularVelocity > -_maxWheelAngularVelocity && _frontWheelRB.angularVelocity < _maxWheelAngularVelocity)
            _frontWheelRB.AddTorque(-_moveInput * _speed * Time.fixedDeltaTime);
        if (_backWheelRB.angularVelocity > -_maxWheelAngularVelocity && _backWheelRB.angularVelocity < _maxWheelAngularVelocity)
            _backWheelRB.AddTorque(-_moveInput * _speed * Time.fixedDeltaTime);
        _carRB.AddTorque(_moveInput * _rotationSpeed * Time.fixedDeltaTime);
    }
}
