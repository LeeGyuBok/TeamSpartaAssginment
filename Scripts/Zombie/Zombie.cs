using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Zombie : MonoBehaviour, IDamageable
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI dmgText;

    //데미지 표기 지속 시간
    private readonly WaitForSeconds _damageTextDelay = new (0.5f);

    //넉백에 의해 레이어변경 시, 변경 전 자신의 레이어
    private int _myLayerMask;

    //넉백 힘
    public readonly Vector2 PushVector = new Vector2(5f, 0f);

    // 자신의 위에 좀비가 있는지 없는지 확인할 떄 사용되는 Ray의 시작 위치 보간
    public readonly Vector3 DetectRayPosGap = new Vector3(-0.3f, 0.9f, 0f);

    private const float Damage = 10f;
    private Player _player;
    public ZombieState CurrentState { get; private set; }
    
    /// <summary>
    /// 기본 상태, 공격 상태, 사망 상태, 점프 상태
    /// </summary>
    public ZombieRun RunState { get; private set; }
    public ZombieAttack AttackState { get; private set; }
    public ZombieDie DieState { get; private set; }
    public ZombieJump JumpState { get; private set; }
    
    public readonly int IsIdle = Animator.StringToHash("IsIdle");
    public readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    public readonly int IsDead = Animator.StringToHash("IsDead");
    
    public Animator Animator { get; private set; }
    public Rigidbody2D Rigidbody2D{ get; private set; }

    //for debug
    private string _currentState;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        AttackState = new ZombieAttack(this);
        DieState = new ZombieDie(this);
        RunState = new ZombieRun(this);
        JumpState = new ZombieJump(this);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpSlider.maxValue = MaxHealthPoints;
        CurrentState = RunState;
        _myLayerMask = gameObject.layer;
        CurrentHealth = MaxHealthPoints;
        hpSlider.value = CurrentHealth;
    }
    
    public void ChangeState(ZombieState newState)
    {
        if (newState == null)
        {
            CurrentState = DieState;
            return;
        }
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
        _currentState = CurrentState.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState?.Execute();
    }

    private void FixedUpdate()
    {
        CurrentState?.FixedExecute();
    }

    //플레이어와 직접적으로 충돌해야 공격 상태로 전환
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
        {
            //Debug.Log("Attack");
            _player = other.gameObject.GetComponent<Player>();
            AttackState.SetPlayer(_player);
            ChangeState(AttackState);
            return;
        }
    }
    
    //만약 넉백 다하면 다시 이동 상태로 전환
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
        {
            AttackState.SetPlayer(null);
            ChangeState(RunState);
        }
    }

    /*private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        _hitInfo2D = Physics2D.Raycast(transform.position + DetectRayPosGap, Vector2.up * 0.05f);
        Gizmos.DrawRay(transform.position + DetectRayPosGap, Vector2.up * 0.05f);
        #endif
    }*/

    //넉백에 의해 변경된 레이어를 다시 자신의 레이어로 수정
    public IEnumerator SetLayerDelay()
    {
        yield return new WaitForSeconds(0.03f);
        gameObject.layer = _myLayerMask;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            Rigidbody2D.velocity += Vector2.left * 0.1f;    
        }
    }

    public float MaxHealthPoints { get; private set; } = 50f;
    public float CurrentHealth { get; private set; }
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        StartCoroutine(SetDmgString(damage));
        hpSlider.value = Mathf.Clamp(CurrentHealth, 0f, MaxHealthPoints);
        if (CurrentHealth <= 0)
        {
            ChangeState(DieState);
            StartCoroutine(DestroyZombie());
        }
    }
    private IEnumerator DestroyZombie()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }

    public void OnAttack()
    {
        _player.TakeDamage(Damage);
    }

    private IEnumerator SetDmgString(float damage)
    {
        dmgText.gameObject.SetActive(true);
        dmgText.text = damage.ToString(CultureInfo.CurrentCulture);
        yield return _damageTextDelay;
        dmgText.gameObject.SetActive(false);
    }
}
