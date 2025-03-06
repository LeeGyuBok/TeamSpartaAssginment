using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieJump : ZombieState
{
    // Start is called before the first frame update
    private Rigidbody2D _rigidbody2D;
    private readonly Vector2 _jumpVelocity = new Vector2(-1.5f, 6f);
    private RaycastHit2D _hitInfo2D;
    public ZombieJump(Zombie zombie) : base(zombie)
    {
        _rigidbody2D = zombie.Rigidbody2D;
    }

    public override void Enter()
    {
        _rigidbody2D.AddForce(_jumpVelocity, ForceMode2D.Impulse);
    }

    public override void Execute()
    {
        
    }

    public override void FixedExecute()
    {
        if (Mathf.Abs(_rigidbody2D.velocity.y) < 0.1f)
        {
            _hitInfo2D = Physics2D.Raycast(_zombie.transform.position, Vector2.up, 0.1f);
            if (_hitInfo2D.collider.gameObject.layer == _zombie.gameObject.layer)
            {
                return;
            }
            _zombie.ChangeState(_zombie.RunState);
        }
    }

    public override void Exit()
    {
        _rigidbody2D.velocity = Vector2.zero;
    }
}
