using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    public SpriteShapeController _spriteShapeController;
    [SerializeField, Range(3f, 300f)] public int _levelLength = 50;
    [SerializeField, Range(1f, 50f)] public float _xMultiplier = 2f;
    [SerializeField, Range(1f, 50f)] public float _yMultiplier = 2f;
    [SerializeField, Range(0f, 1f)] public float _curveSmoothness = 0.5f;
    [SerializeField, Range(0, 10)] public float _difficultyFactor = 5;

    [SerializeField, Range(0f, 30f)] public float _globalNoiseScale = 10f;
    [SerializeField, Range(0f, 30f)] public float _globalNoiseStrength = 5f;
    [SerializeField] public float _noiseStep = 0.5f;
    [SerializeField] public float _bottom = 10f;

    // Parameters for global noise

    private Vector3 _lastPos;

    private void OnValidate()
    {
        _spriteShapeController.spline.Clear();
        for (int i = 0; i < _levelLength; ++i)
        {
            float difficulty = 1 + _difficultyFactor * i / (float)_levelLength;

            // Global noise
            float globalNoise = Mathf.PerlinNoise(i / (float)_globalNoiseScale, 0) * _globalNoiseStrength * difficulty;

            // Local noise
            float currentYMultiplier = _yMultiplier * difficulty;
            float currentNoiseStep = _noiseStep * difficulty;
            float localNoise = Mathf.PerlinNoise(0, i * currentNoiseStep) * currentYMultiplier;

            // Combined height
            float height = globalNoise + localNoise;

            _lastPos = transform.position + new Vector3(i * _xMultiplier, height);
            _spriteShapeController.spline.InsertPointAt(i, _lastPos);

            if (i != 0 && i != _levelLength - 1)
            {
                _spriteShapeController.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                _spriteShapeController.spline.SetLeftTangent(i, Vector3.left * _xMultiplier * _curveSmoothness);
                _spriteShapeController.spline.SetRightTangent(i, Vector3.right * _xMultiplier * _curveSmoothness);
            }
        }

        _spriteShapeController.spline.InsertPointAt(_levelLength, new Vector3(_lastPos.x, transform.position.y - _bottom));
        _spriteShapeController.spline.InsertPointAt(_levelLength + 1, new Vector3(transform.position.x, transform.position.y - _bottom));
    }
}
