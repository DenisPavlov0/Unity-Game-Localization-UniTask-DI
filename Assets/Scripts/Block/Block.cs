using System;
using UnityEngine;
using Zenject;

public class Block : MonoBehaviour
{
    [Inject] private BlockSpawner _blockSpawner;
    public int blockIndex;
    public static Action needNewSpawn;

    // Устанавливаем индекс блока
    public void SetIndex(int index)
    {
        blockIndex = index;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (blockIndex == 8 && other.CompareTag("Player"))
        {
            needNewSpawn.Invoke();
        }
    }

    // Получаем индекс блока
    public int GetBlockIndex()
    {
        return blockIndex;
    }
}
