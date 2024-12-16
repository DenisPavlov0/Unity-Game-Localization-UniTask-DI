using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerCamera : MonoBehaviour
{
    [Inject] private PlayerManager _player;
    [SerializeField] private Camera _camera;
    [Header("Camera Settings")] 
    [SerializeField] private float targetY;
    [SerializeField] private float targetZ;
    private Vector3 _cameraVelocity;
    private float _cameraSmoothSpeed = 1f;
    public Camera CameraObject
    {
        get
        {
            if (_camera == null)
            {
                Debug.LogError("Camera is not assigned!");
            }
            return _camera;
        }
    }
    public void HandleAllCameraActions()
    {
        if (_player != null)
        {
            FollowTarget();
        }
    }
    private void FollowTarget()
    {
        // Создаем вектор целевой позиции камеры, с учетом фиксированных значений по осям Y и Z
        Vector3 targetPosition = _player.transform.position;
        targetPosition.y = targetY; // Зафиксировать позицию по оси Y на уровне 5
        targetPosition.z = _player.transform.position.z - targetZ; 

        // Сглаживаем движение камеры, используя метод SmoothDamp
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, targetPosition, 
            ref _cameraVelocity, _cameraSmoothSpeed * Time.deltaTime);

        // Устанавливаем позицию камеры
        transform.position = targetCameraPosition;
    }
}
