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

    private float _moveInput;

    private void Update()
    {
        _moveInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        if (_frontWheelRB.angularVelocity > -_maxWheelAngularVelocity && _frontWheelRB.angularVelocity < _maxWheelAngularVelocity)
            _frontWheelRB.AddTorque(-_moveInput * _speed * Time.fixedDeltaTime);
        if (_backWheelRB.angularVelocity > -_maxWheelAngularVelocity && _backWheelRB.angularVelocity < _maxWheelAngularVelocity)
            _backWheelRB.AddTorque(-_moveInput * _speed * Time.fixedDeltaTime);
        _carRB.AddTorque(_moveInput * _rotationSpeed * Time.fixedDeltaTime);
    }
}
