using System;
using UnityEngine;
using Zenject;

public class PlayerManager : MonoBehaviour
{
    [Inject] private PlayerLocomotionManager _playerLocomotionManager;
    [Inject] private PlayerAnimationManager _playerAnimationManager;
    [Inject] private PlayerCamera _playerCamera;
    

    private CharacterController _characterController;

    public bool isJumping = false;
    public bool isGrounded = true;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        _playerLocomotionManager.HandleAllMovement();
        _playerAnimationManager.HandleAnimations();
       
    }

    private void LateUpdate()
    {
        _playerCamera.HandleAllCameraActions();
    }
   
    public CharacterController CharacterController
    {
        get
        {
            if (_characterController == null)
            {
                Debug.LogError("CharacterController is not assigned!");
            }
            return _characterController;
        }
    }
}

