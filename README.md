# Unity Game Project: Learning UniTask, Zenject & Object Pooling

Этот проект был создан с целью изучения работы с такими концепциями, как асинхронное программирование с использованием **UniTask**, внедрение зависимостей (DI) через контейнер **Zenject**, а также паттерн "Object Pool" для оптимизации работы с игровыми объектами. В ходе разработки были реализованы основы этих технологий в контексте игрового процесса.

## Основные цели проекта:
- Изучение и внедрение UniTask для асинхронных операций в Unity.
- Использование Zenject для внедрения зависимостей (Dependency Injection).
- Реализация Object Pooling для эффективного управления ресурсами, такими как игровые объекты и эффекты.

## Структура проекта

### Использованные технологии:
- **Zenject** — DI контейнер для удобного управления зависимостями между компонентами.
- **UniTask** — библиотека для асинхронных задач, используемая в Unity для упрощения асинхронного программирования.
- **Object Pooling** — паттерн для управления памятью и повторным использованием объектов.

### Описание классов и интерфейсов:
- **GameInstaller** — Конфигурация контейнера Zenject для привязки зависимостей.
- **PlayerAnimationManager** — Обрабатывает анимации персонажа, связанные с движением и состоянием игрока.
- **PlayerCamera** — Управление камерой для следования за игроком и корректировки ее положения.
- **PlayerInputManager** — Обрабатывает ввод от игрока с использованием новой системы ввода Unity (PlayerInput).
- **PlayerLocomotionManager** — Управление движением персонажа, включая бег, ходьбу и прыжки.
- **PlayerManager** — Управление состоянием персонажа (проверка на землю, состояние прыжка и анимации).
- **ObjectPool<T>** — Пул объектов для повторного использования игровых объектов и эффектов.
- **ParticlePoolManager** — Менеджер пулов для частиц (например, взрывов и эффектов подбора).

### Особенности:
- Реализация пулов для частиц (например, для взрывов и эффектов подбора), что помогает избежать излишних затрат на создание и уничтожение объектов в ходе игры.
- Внедрение зависимостей с использованием Zenject для упрощения работы с компонентами и их связями.
- Асинхронное управление задачами с помощью UniTask, что позволяет повысить производительность и плавность игры.
- Модульная архитектура, где каждый класс отвечает за свою часть игры (ввод, анимация, движение, камера), что способствует легкости в поддержке и расширении проекта.

## Установка и запуск

1. Клонируйте репозиторий:

   ```bash
   git clone https://github.com/yourusername/yourproject.git

2. Откройте проект в Unity.
3. Убедитесь, что у вас установлен пакет **Zenject** через **Unity Package Manager**.
4. Импортируйте необходимые пакеты **UniTask** и другие зависимости.


## Использование

### Основные моменты:
- В проекте используется **Zenject** для инъекции зависимостей между компонентами. Это упрощает взаимодействие между классами и позволяет легко тестировать компоненты независимо друг от друга.
- **UniTask** используется для выполнения асинхронных операций, таких как загрузка ресурсов или задержки между событиями, что делает игру более отзывчивой и плавной.
- **Object Pooling** помогает эффективно управлять ресурсами (например, частицами взрывов), избегая их создания и уничтожения в каждый момент времени, что значительно повышает производительность.

### Пример использования UniTask, Object Pool, Zenject:

```csharp
using Cysharp.Threading.Tasks;

public class SomeManager : MonoBehaviour
{
    private async void Start()
    {
        await SomeAsyncMethod();
    }

    private async UniTask SomeAsyncMethod()
    {
        // Имитация асинхронной операции (например, загрузка ресурса)
        await UniTask.Delay(1000);  // Задержка на 1 секунду
        Debug.Log("Асинхронная операция завершена");
    }
}

public class ParticleEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    private IObjectPool<ParticleSystem> _explosionPool;

    private void Start()
    {
        _explosionPool = new ObjectPool<ParticleSystem>(explosionPrefab);
    }

    public void CreateExplosion(Vector3 position)
    {
        ParticleSystem explosion = _explosionPool.GetObject();
        explosion.transform.position = position;
        explosion.Play();
    }

    public void ReturnExplosion(ParticleSystem particle)
    {
        _explosionPool.ReturnObject(particle);
    }
}

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerManager>().AsSingle();
        Container.Bind<PlayerInputManager>().AsSingle();
    }
}


