using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour
{
    private const float Damage = 20f;
    private const float Speed = 15f; // 총알 속도
    private Vector3 _direction;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetPosition">mousePosition</param>
    public void SetDirection(Vector3 targetPosition)
    {
        // 현재 총알 위치에서 목표 위치까지의 방향 벡터 계산
        Vector3 dir = (targetPosition - transform.position).normalized;

        // 랜덤한 -15도 ~ 15도 값 생성
        float randomAngle = Random.Range(-15f, 15f);

        // 방향 벡터를 회전 (Z축 기준)
        _direction = Quaternion.Euler(0, 0, randomAngle) * dir;
    }

    private void Update()
    {
        // 총알 이동
        transform.position += _direction * (Speed * Time.deltaTime);
        StartCoroutine(DestroyAfterSeconds(0.7f));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out Zombie zombie))
        {
            zombie.TakeDamage(Damage);
            BulletPool.Instance.Return(this);
        }
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        BulletPool.Instance.Return(this);
    }
}
