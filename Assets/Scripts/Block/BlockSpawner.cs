using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;  // Префаб блока
    [SerializeField] private float spawnDistance = 10f;  // Расстояние между блоками
    [SerializeField] private int blocksCount = 10;  // Количество блоков в группе
    private Queue<GameObject> _spawnedBlocks;  // Очередь для отслеживания блоков
    private Vector3 lastSpawnPosition;  // Последняя позиция для спавна
    private int blockIndex = 0;  // Индекс текущего блока

    [Inject] private DiContainer _diContainer;  // Инжектируем контейнер
    private void Start()
    {
        // Подписываемся на событие, которое инициирует спавн блоков
        Block.needNewSpawn += SpawnTenBlock;
        _spawnedBlocks = new Queue<GameObject>();
        lastSpawnPosition = transform.position;

        SpawnTenBlock();  // Изначально спавним 10 блоков
    }
    private void Update()
    {
        // Перезапуск индекса блока, если он превышает 10
        if (blockIndex >= 10)
        {
            blockIndex = 0;
        }
    }
    private void SpawnTenBlock()
    {
        // Если блоков больше, чем должно быть, удаляем старые
        if (_spawnedBlocks.Count >= blocksCount)
        {
            RemoveOldBlocks();
        }

        // Спавним новые блоки
        for (int i = 0; i < blocksCount; i++)
        {
            SpawnBlock();
        }
    }
    private void SpawnBlock()
    {
        // Генерация случайного значения по оси X в диапазоне от -10 до 10
        float randomX = Random.Range(-9f, 9f);
        
        // Генерация случайного значения по оси Y в диапазоне от -2 до 0
        float randomY = Random.Range(-2f, 1f);

        // Создаем позицию блока с случайными значениями по осям X и Y, фиксированное значение по оси Z
        Vector3 spawnPosition = new Vector3(randomX, randomY, lastSpawnPosition.z);

        // Спавним новый блок на случайной позиции
        GameObject newBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        _diContainer.Inject(newBlock);  // Инжектируем зависимости через Zenject

        // Добавляем компонент и устанавливаем индекс
        newBlock.AddComponent<Block>().SetIndex(blockIndex);

        // Добавляем блок в очередь
        _spawnedBlocks.Enqueue(newBlock);

        // Обновляем позицию для следующего блока, смещаем только по оси Z
        lastSpawnPosition += new Vector3(0, 0, spawnDistance);

        // Увеличиваем индекс
        blockIndex++;
    }
    // Метод для удаления старых блоков
    private void RemoveOldBlocks()
    {
        // Удаляем старые блоки, пока в очереди не останется нужное количество
        while (_spawnedBlocks.Count > blocksCount)
        {
            // Удаляем самый старый блок
            GameObject blockToRemove = _spawnedBlocks.Dequeue();
            Destroy(blockToRemove);  // Уничтожаем блок
        }
    }
}
