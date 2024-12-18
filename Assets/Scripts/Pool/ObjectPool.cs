using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IObjectPool<T> where T : class
{
    private readonly GameObject _prefab; // Префаб для создания объектов
    private readonly Queue<GameObject> _pool = new ();// Очередь для хранения объектов в пуле
    
    // Конструктор
    
    public ObjectPool(GameObject prefab, int initialCapacity = 5)
    {
        _prefab = prefab;
        for (int i = 0; i < initialCapacity; i++)
        {
            GameObject obj = Object.Instantiate(_prefab);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
    
    // Получение объекта из пула
    public T GetObject()
    {
        // Если пул не пустой то активируем объект из этой очереди,
        // Иначе инициализируем объект и активируем его
        if (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue(); // Берём объект из очереди
            obj.SetActive(true); // Делаем его активным
            return obj.GetComponent<T>(); 
        }
        else
        {
            // Если пул пустой, создаём новый объект
            GameObject obj = Object.Instantiate(_prefab);
            obj.SetActive(true);
            return obj.GetComponent<T>();  // Возвращаем компонент типа T
        }
        
    }
    // Возвращаем объект обратно в пул
    public void ReturnObject(T obj)
    {
        GameObject gameObject = (obj as Component)?.gameObject;
        if (gameObject != null)
        {
            gameObject.SetActive(false);  // Делаем объект неактивным
            _pool.Enqueue(gameObject);    // Возвращаем объект в пул
        }
    }
    
    public void ClearPool()
    {
        while (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue();
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }
    }
}
