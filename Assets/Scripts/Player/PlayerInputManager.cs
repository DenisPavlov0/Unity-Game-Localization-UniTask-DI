using UnityEngine;
using Zenject;

public class PlayerInputManager : MonoBehaviour
{
    [Inject] private PlayerLocomotionManager _playerLocomotionManager;
    [Inject] private PlayerManager _playerManager;
    // Объявление переменных для обработки ввода игрока
    private PlayerInput _playerInput;  // Экземпляр класса для работы с вводом игрока (класс PlayerInput из нового Input System)
    private Vector2 _movementInput;    // Переменная для хранения вектора ввода движения (X и Y)
   
    // Приватные поля
    private float verticalInput;   // Вертикальный ввод (вперед/назад)
    private float horizontalInput; // Горизонтальный ввод (влево/вправо)
    private float moveAmount;      // Суммарное движение

    // Свойства для получения данных из других классов
    public float VerticalInput => verticalInput;  
    public float HorizontalInput => horizontalInput;  
    public float MoveAmount => moveAmount;

    [Header("PlayerActionInput")] 
    [SerializeField] private bool jumpInput = false;
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);  
    }
    
    private void OnEnable()
    {
        if (_playerInput == null)
        {
            _playerInput = new PlayerInput();  // Создаем новый экземпляр PlayerInput
            // Подписываемся на событие, которое будет срабатывать при каждом изменении ввода
            _playerInput.PlayerMovement.Movement.performed += i => _movementInput = i.ReadValue<Vector2>();
            _playerInput.PlayerAction.Jump.performed += i => jumpInput = true;
        }
        _playerInput.Enable();  // Включаем систему ввода
    }

   
    private void Update()
    {
        HandleMovementInput();
        HandleJump();
    }
    
    private void HandleMovementInput()
    {
        // Сохраняем значения по осям X и Y из _movementInput в отдельные переменные
        verticalInput = _movementInput.y;  // Получаем вертикальный ввод (вперед/назад)
        horizontalInput = _movementInput.x;  // Получаем горизонтальный ввод (влево/вправ)
        
        // Вычисляем общее количество движения
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));  // Ограничиваем сумму движения значениями от 0 до 1
        
        // Если движение невелико, но есть, ставим минимальное значение
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;  // Устанавливаем moveAmount равным 0.5 для минимального движения
        }
        // Если движение достаточно сильно, устанавливаем максимальное значение
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;  // Устанавливаем moveAmount равным 1 для максимального движения
        }
    }

    private void HandleJump()
    {
        if (jumpInput)
        {
            jumpInput = false;
            _playerLocomotionManager.AttemptToJump();
        }
    }
    
    
}
