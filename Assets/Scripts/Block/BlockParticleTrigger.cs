
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BlockParticleTrigger : MonoBehaviour
{
    private ParticlePoolManager _particlePoolManager;
    [SerializeField] private Color[] possibleColors;
    private Renderer blockRenderer;

    private void Awake()
    {
        GameObject particlePoolObject = GameObject.FindGameObjectWithTag("ParticlePool");

        if (particlePoolObject != null)
        {
            // Получаем компонент ParticlePoolManager с найденного объекта
            _particlePoolManager = particlePoolObject.GetComponent<ParticlePoolManager>();

            if (_particlePoolManager == null)
            {
                Debug.LogError("ParticlePoolManager не найден на объекте с тегом ParticlePool");
            }
        }
        else
        {
            Debug.LogError("Объект с тегом 'ParticlePool' не найден на сцене");
        }
    }

    private void Start()
    {
        blockRenderer = GetComponent<Renderer>();
        if (possibleColors.Length == 0)
        {
            Debug.LogWarning("Массив цветов пуст! Нужно добавить хотя бы один цвет.");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerLocomotionManager = other.GetComponentInParent<PlayerLocomotionManager>();
            if (playerLocomotionManager != null && playerLocomotionManager.IsPlayerFalling())
            {
                TriggerParticleEffect(other);
                ChangeBlockColor();
            }
        }
    }
    private async void TriggerParticleEffect(Collider other)
    {
        if (_particlePoolManager == null)
        {
            Debug.LogError("ParticlePoolManager не был инжектирован!");
            return;  // Останавливаем выполнение, если компонент отсутствует
        }
        
        ParticleSystem particle = _particlePoolManager.GetExplosion();

        // Получаем точку столкновения (точка входа триггера)
        Vector3 triggerPosition = other.ClosestPointOnBounds(transform.position);

        // Устанавливаем позицию частицы в точку столкновения
        particle.transform.position = triggerPosition;

        // Запускаем эффект
        particle.Play();

        // Ждем завершения эффекта
        await WaitForParticleToFinish(particle);
    
        // Проверка на null перед возвратом в пул
        if (particle != null)
        {
            _particlePoolManager.ReturnExplosion(particle);
        }
        else
        {
            Debug.LogWarning("ParticleSystem was destroyed before return.");
        }
    }
    private void ChangeBlockColor()
    {
        // Проверяем, что массив цветов не пуст
        if (possibleColors.Length > 0)
        {
            // Генерация случайного индекса для массива цветов
            int randomIndex = Random.Range(0, possibleColors.Length);

            // Получаем случайный цвет из массива
            Color randomColor = possibleColors[randomIndex];

            // Устанавливаем случайный цвет для материала блока
            if (blockRenderer != null)
            {
                blockRenderer.material.color = randomColor;
            }
        }
        else
        {
            Debug.LogWarning("Массив цветов пуст! Невозможно изменить цвет блока.");
        }
    }
    private async UniTask WaitForParticleToFinish(ParticleSystem particle)
    {
        if (particle != null)
        {
            // Ожидаем завершения эффекта
            await UniTask.WaitUntil(() => !particle.isPlaying || particle == null);
        }
        else
        {
            Debug.LogWarning("ParticleSystem was destroyed before it finished.");
        }
    }
}
