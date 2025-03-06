using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour, IDamageable
{
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;

    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject hero;

    [SerializeField] private List<GameObject> boxes;
    [SerializeField] private List<Slider> boxHpSliders; 
    
    [SerializeField] private Camera mainCamera;

    [SerializeField] private Bullet bulletPrefab;
    
    private Dictionary<GameObject, (Slider, float)> _boxHpSliders = new Dictionary<GameObject, (Slider, float)>();

    private Vector3 _fireMousePosition;

    private void Awake()
    {
        CurrentHealth = MaxHealthPoints;
        _boxHpSliders = new Dictionary<GameObject, (Slider, float)>();
        for (int i = 0; i < boxes.Count; i++)
        {
            _boxHpSliders.Add(boxes[i], (boxHpSliders[i], CurrentHealth));
            boxHpSliders[i].maxValue = MaxHealthPoints;
            boxHpSliders[i].value = CurrentHealth;
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(Fire), 1f, 2f);
    }

    private void Update()
    {
        Aim();
    }

    public float MaxHealthPoints { get; private set; } = 1000;
    public float CurrentHealth { get; private set; }
    
    public void TakeDamage(float damage)
    {
        for (int i = 0; i < _boxHpSliders.Count; i++)
        {
            if (!boxes[i].gameObject.activeInHierarchy) continue;//이미 파괴된 박스이면 다시 순회
            float boxHp = _boxHpSliders[boxes[i]].Item2;//체력가져와서
            boxHp -= damage;//뺀다.
            _boxHpSliders[boxes[i]] = (boxHpSliders[i], boxHp);//재할당한다.
            //재할당된 값을 0부터 최대 체력까지의 비율로 변환해서 밸류를 바꾼다.
            _boxHpSliders[boxes[i]].Item1.value = Mathf.Clamp(_boxHpSliders[boxes[i]].Item2, 0f, MaxHealthPoints);
            //_boxHpSliders[boxes[i]].Item1.value -= _boxHpSliders[boxes[i]].Item2;// 이건 가끔 버그난다.
            //Debug.Log($"{_boxHpSliders[boxes[i]].Item2} / Clamped: {_boxHpSliders[boxes[i]].Item1.value}");
            if (boxHp <= 0)//체력이 0보다 작아지면
            {
                boxes[i].gameObject.SetActive(false);//박스를 파괴처리한다.
                
                //남은 박스들과 히어로를 내려주세요. 그들의 다음 위치로
                for (int j = boxes.Count-1; j > i; j--)
                {
                    boxes[j].transform.position = boxes[j - 1].transform.position;
                }
                Vector3 newHeroPosition = hero.gameObject.transform.position;
                newHeroPosition.y -= 1.5f;
                hero.gameObject.transform.position = newHeroPosition;
                return;
            }
            return;
        }
    }

    private void Aim()
    {
        _fireMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); // 마우스 위치를 월드 좌표로 변환
        Vector3 direction = _fireMousePosition - weapon.transform.position; // 현재 오브젝트와 마우스 위치 간의 방향 벡터
        // y/x 값으로 좌표평면에서 마우스의 현재 위치를 찾고 그에 대한 각도를 계산한다.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 라디안을 도(degree) 단위로 변환
        weapon.transform.rotation = Quaternion.Euler(0, 0, angle-35f); // Z축 기준 회전
    }

    private void Fire()
    {
        for (int i = 0; i < 5; i++)
        {
            Bullet bullet = BulletPool.Instance.Spawn();
            bullet.transform.position = hero.transform.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.SetDirection(_fireMousePosition);
        }
    }
}
