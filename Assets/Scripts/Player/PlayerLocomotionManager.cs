using UnityEngine;
using Zenject;

public class PlayerLocomotionManager : MonoBehaviour
{
    [Inject] private PlayerManager _playerManager; // Менеджер игрока, содержит информацию о состоянии игрока
    [Inject] private PlayerInputManager _playerInputManager; // Менеджер ввода игрока, отвечает за получение ввода от игрока
    [Inject] private PlayerCamera _playerCamera; // Камера игрока, используется для получения направления движения относительно камеры

    [Header("Movement")]
    private float _verticalMovement; // Вертикальное движение игрока (вперед/назад)
    private float _horizontalMovement; // Горизонтальное движение игрока (влево/вправо)
    private float _moveAmount; // Общее количество движения (величина)
    private Vector3 _moveDirection; // Направление движения игрока
    private Vector3 _targetRotationDirection; // Направление для вращения игрока

    // Настройки скорости для движения
    private float _walkingSpeed = 2f;
    private float _runningSpeed = 5f;
    private float _rotationSpeed = 15f;

    [Header("Jumping")] 
    [SerializeField] private LayerMask groundLayer; // Слой земли для проверки
    [SerializeField] private float groundCheckSphereRadius; // Радиус проверки земли
    [SerializeField] private float gravityForce = -9.55f; // Сила гравитации, используется для падения
    [SerializeField] private float jumpHeight = 2f; // Высота прыжка
    [SerializeField] private Vector3 yVelocity; // Вертикальная скорость
    [SerializeField] private float groundYVelocity = -40f; // Начальная вертикальная скорость при касании земли
    [SerializeField] private float fallStartVelocity = -5f; // Начальная скорость при падении (если игрок в воздухе)
    private bool _fallingVelocityHasBeenSet = false; // Флаг, указывающий, установлена ли скорость падения

    // Метод для получения вертикального и горизонтального ввода от игрока
    private void GetVerticalAndHorizontalInput()
    {
        _verticalMovement = _playerInputManager.VerticalInput; // Получаем вертикальный ввод (вперед/назад)
        _horizontalMovement = _playerInputManager.HorizontalInput; // Получаем горизонтальный ввод (влево/вправо)
    }

    // Основной метод, обрабатывающий все движения игрока
    public void HandleAllMovement()
    {
        // Обрабатываем движение по земле
        HandleGroundedMovement(); 
        // Обрабатываем вращение игрока
        HandleRotation(); 
        // Проверяем, находится ли игрок на земле или в воздухе
        HandleGroundCheck(); 

        // Если игрок на земле, обрабатываем вертикальную скорость
        if (_playerManager.isGrounded)
        {
            // Если вертикальная скорость отрицательная (игрок падает), сбрасываем ее на нормальное значение для нахождения на земле
            if (yVelocity.y < 0)
            {
                _fallingVelocityHasBeenSet = false;
                yVelocity.y = groundYVelocity;
            }
        }
        else
        {
            // Если игрок прыгнул и еще не установлен параметр падения, устанавливаем его
            if (_playerManager.isJumping && !_fallingVelocityHasBeenSet)
            {
                _fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartVelocity;
            }

            // Применяем гравитацию для уменьшения вертикальной скорости с течением времени
            yVelocity.y += gravityForce * Time.deltaTime;
        }

        // Двигаем персонажа с учетом вертикальной скорости
        _playerManager.CharacterController.Move(yVelocity * Time.deltaTime);
    }

    // Метод для обработки движения игрока по земле
    private void HandleGroundedMovement()
    {
        // Получаем ввод от игрока
        GetVerticalAndHorizontalInput();

        // Получаем позицию камеры игрока
        var playerCamera = _playerCamera.transform;

        // Определяем направление движения относительно камеры
        _moveDirection = playerCamera.forward * _verticalMovement;
        _moveDirection = _moveDirection + playerCamera.right * _horizontalMovement;
        _moveDirection.Normalize(); // Нормализуем направление для равномерного движения
        _moveDirection.y = 0; // Останавливаем вертикальное движение, чтобы игрок не поднимался или не опускался при движении

        // В зависимости от величины движения (бег или ходьба), устанавливаем скорость
        if (_playerInputManager.MoveAmount > 0.5f)
        {
            _playerManager.CharacterController.Move(_moveDirection * (_runningSpeed * Time.deltaTime)); // Двигаем игрока с беговой скоростью
        }
        else if (_playerInputManager.MoveAmount <= 0.5f)
        {
            _playerManager.CharacterController.Move(_moveDirection * (_walkingSpeed * Time.deltaTime)); // Двигаем игрока с ходьбой
        }
    }

    // Метод для обработки вращения игрока
    private void HandleRotation()
    {
        _targetRotationDirection = Vector3.zero; // Изначально не задаем направление

        // Получаем позицию камеры игрока
        var playerCamera = _playerCamera.CameraObject.transform;

        // Определяем направление вращения относительно камеры
        _targetRotationDirection = playerCamera.forward * _verticalMovement;
        _targetRotationDirection += _targetRotationDirection + playerCamera.right * _horizontalMovement;
        _targetRotationDirection.Normalize(); // Нормализуем направление для равномерного вращения
        _targetRotationDirection.y = 0; // Убираем вертикальную компоненту из направления вращения

        // Если направление вращения равно нулевому вектору, сохраняем текущее направление
        if (_targetRotationDirection == Vector3.zero)
        {
            _targetRotationDirection = transform.forward;
        }

        // Рассчитываем новое вращение на основе направления
        Quaternion newRotation = Quaternion.LookRotation(_targetRotationDirection);
        // Плавно вращаем игрока в нужное направление с заданной скоростью
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, _rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation; // Применяем вращение к игроку
    }

    // Метод для попытки прыжка
    public void AttemptToJump()
    {
        // Если игрок уже прыгает или не на земле, прыжок не выполняется
        if (_playerManager.isJumping) return;
        if (!_playerManager.isGrounded) return;

        // Если игрок может прыгнуть, применяем вертикальную скорость для прыжка
        ApplyJumpingVelocity();
    }

    // Метод для применения вертикальной скорости при прыжке
    public void ApplyJumpingVelocity()
    {
        // Вычисляем вертикальную скорость для прыжка с помощью уравнения кинематики
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }

    // Метод для проверки, находится ли игрок на земле
    private void HandleGroundCheck()
    {
        // Проверяем, есть ли земля в радиусе вокруг игрока
        _playerManager.isGrounded = Physics.CheckSphere(transform.position, groundCheckSphereRadius, groundLayer);
    }
}
