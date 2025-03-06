using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieRun : ZombieState
{
    private Rigidbody2D _rigidbody2D;
    private RaycastHit2D _raycastHit2D;
    private RaycastHit2D _hitInfo2D;
    private Vector3 _gap = new Vector3(-0.4f, 0.2f, 0);

    public ZombieRun(Zombie zombie) : base(zombie)
    {
        _rigidbody2D = zombie.Rigidbody2D;
    }

    public override void Enter()
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            _animator.SetBool(_zombie.IsIdle, true);
        }
    }

    public override void Execute()
    {
        Debug.DrawRay(_zombie.transform.position + _gap, Vector2.left * 0.1f);
        _raycastHit2D = Physics2D.Raycast(_zombie.transform.position + _gap, Vector2.left, 0.15f);
        if (_raycastHit2D)
        {
            if (_raycastHit2D.collider.gameObject.layer == _zombie.gameObject.layer)
            {
                _zombie.ChangeState(_zombie.JumpState);
                return;
            }
        }
    }

    public override void FixedExecute()
    {
        _rigidbody2D.velocity = Vector2.left;
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
        _rigidbody2D.velocity = Vector2.zero;
        _animator.SetBool(_zombie.IsIdle, false);
    }
    
    
}
