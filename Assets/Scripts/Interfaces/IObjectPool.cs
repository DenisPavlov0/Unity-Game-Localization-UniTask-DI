public interface IObjectPool<T> where T : class
{
    T GetObject();  // Получить объект из пула
    void ReturnObject(T obj);  // Вернуть объект обратно в пул
    
    void ClearPool();
}