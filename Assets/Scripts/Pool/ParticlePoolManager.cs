using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePoolManager : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject pickPrefab;
    private IObjectPool<ParticleSystem> _explosionPool;
    private IObjectPool<ParticleSystem> _pickPool;
    
    private void Start()
    {
        _explosionPool = new ObjectPool<ParticleSystem>(explosionPrefab);
        _pickPool = new ObjectPool<ParticleSystem>(pickPrefab);
    }
    
    public ParticleSystem GetExplosion()
    {
        return _explosionPool.GetObject();
    }
    
    public void ReturnExplosion(ParticleSystem particle)
    {
        _explosionPool.ReturnObject(particle);
    }
    
    public ParticleSystem GetPick()
    {
        return _pickPool.GetObject();
    }
    
    public void ReturnPick(ParticleSystem particle)
    {
        _pickPool.ReturnObject(particle);
    }
    
    private void ClearPools()
    {
        _explosionPool.ClearPool();
        _pickPool.ClearPool();
    }

    // Очистка пулов при выходе из игры
    private void OnApplicationQuit()
    {
        ClearPools();
    }
}
