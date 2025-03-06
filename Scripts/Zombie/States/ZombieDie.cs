using System.Collections;
using UnityEngine;

public class ZombieDie : ZombieState
{
    private static readonly int IsIdle = Animator.StringToHash("IsDead");
    public ZombieDie(Zombie zombie) : base(zombie)
    {
    }

    public override void Enter()
    {
        _animator.SetBool(_zombie.IsDead, true);
        _zombie.Rigidbody2D.velocity = Vector2.zero;
        _zombie.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        _zombie.gameObject.layer = LayerMask.NameToLayer("Dead");
    }

    public override void Execute()
    {
        
    }

    public override void FixedExecute()
    {
        
    }

    public override void Exit()
    {
        
    }
}
