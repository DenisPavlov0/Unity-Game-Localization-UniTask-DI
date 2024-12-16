using System;
using UnityEngine;
using Zenject;

public class PlayerAnimationManager : MonoBehaviour
{
    // Константы для параметров анимации, которые используются в Animator
    private const string IsRunning = "isRunning"; // Параметр для анимации бега
    private const string IsIdle = "isIdle"; // Параметр для анимации покоя
    private const string IsJumping = "isJumping"; // Параметр для анимации прыжка
    private const string IsGround = "isGround"; // Параметр для проверки, находится ли игрок на земле

    [Inject] private PlayerInputManager _playerInputManager; // Менеджер ввода игрока для получения информации о движении
    [Inject] private PlayerManager _playerManager; // Менеджер игрока, чтобы проверять его состояние (например, на земле ли он)
    
    private Animator _animator; // Ссылка на компонент Animator, который управляет анимациями

    // Инициализация компонента Animator
    private void Start()
    {
        // Получаем ссылку на компонент Animator, который должен быть на том же объекте, что и этот скрипт
        _animator = GetComponent<Animator>();
    }

    // Метод для обработки анимаций в зависимости от состояния игрока
    public void HandleAnimations()
    {
        // Определяем, движется ли игрок
        bool isMoving = _playerInputManager.MoveAmount > 0.1f; 
        // Если игрок двигается, включаем анимацию бега
        _animator.SetBool(IsRunning, isMoving); 

        // Если игрок не двигается, включаем анимацию покоя (Idle)
        _animator.SetBool(IsIdle, !isMoving);

        // Включаем анимацию прыжка, если игрок в воздухе (не на земле)
        _animator.SetBool(IsJumping, !_playerManager.isGrounded);

        // Включаем параметр для проверки, находится ли игрок на земле
        _animator.SetBool(IsGround, _playerManager.isGrounded);
    }
}