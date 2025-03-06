using UnityEngine;

public abstract class ZombieState
{
    /// <summary>
    /// 좀비의 상태를 구현하기 위해 사용
    /// </summary>
    public Animator _animator { get; protected set; }
    public Zombie _zombie { get; protected set; }
    public ZombieState(Zombie zombie)
    {
        _zombie = zombie;
        _animator = _zombie.Animator;
    }

    public abstract void Enter();
    
    public abstract void Execute();
    
    public abstract void FixedExecute();
    
    public abstract void Exit();
}
