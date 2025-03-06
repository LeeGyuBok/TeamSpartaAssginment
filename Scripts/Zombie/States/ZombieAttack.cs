using UnityEngine;

public class ZombieAttack : ZombieState
{
    private Rigidbody2D _rigidbody2D;
    private RaycastHit2D _hitInfo2D;
    //private float _damage = 10f;
    private float _attackTimer;
    private float _delay;
    private Player _player;
    public ZombieAttack(Zombie zombie) : base(zombie)
    {
        _rigidbody2D = zombie.Rigidbody2D;
    }

    public override void Enter()
    {
        _animator.SetBool(_zombie.IsAttacking, true);
        _rigidbody2D.velocity = Vector2.zero;
        
        _attackTimer = _delay;
    }

    public override void Execute()
    {
        if (_animator.IsInTransition(0)) return;
        if (_delay == 0)
        {
            _delay = _animator.GetCurrentAnimatorStateInfo(0).length;
        }
        _attackTimer -= Time.deltaTime;
        if (_attackTimer > 0f) return;
        if (!_player)
        {
            _zombie.ChangeState(_zombie.RunState);
            return;
        } 
        //_player.TakeDamage(_damage);
        _attackTimer = _delay;

    }

    public override void FixedExecute()
    {
        _hitInfo2D = Physics2D.Raycast(_zombie.transform.position + _zombie.DetectRayPosGap, Vector2.up * 0.01f);
        if (_hitInfo2D)
        {
            GameObject hitObject = _hitInfo2D.collider.gameObject;
            if (hitObject.layer == _zombie.gameObject.layer)
            {
                _zombie.gameObject.layer = LayerMask.NameToLayer("Default");
                _rigidbody2D.velocity = Vector2.zero;
                _rigidbody2D.AddForce(_zombie.PushVector, ForceMode2D.Impulse);
                _zombie.StartCoroutine(_zombie.SetLayerDelay());
            }
        }
    }

    public override void Exit()
    {
        _animator.SetBool(_zombie.IsAttacking, false);
    }

    public void SetPlayer(Player player)
    {
        _player = player;
        /*if (_player)
        {
            _player.TakeDamage(_damage);
        }*/
    }
}
