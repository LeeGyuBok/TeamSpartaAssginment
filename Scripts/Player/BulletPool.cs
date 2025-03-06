using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;
    [SerializeField] private Bullet bulletPrefab;
    private const int Capacity = 500;
    
    public Stack<Bullet> Pool { get; }= new Stack<Bullet>(Capacity);
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<BulletPool>();
            if (Instance == null)
            {
                Instance = gameObject.AddComponent<BulletPool>();
            }
        }
        
        for (int i = 0; i < Capacity; i++)
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.gameObject.SetActive(false);
            Pool.Push(bullet);
        }
    }

    public void Return(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        Pool.Push(bullet);
    }

    public Bullet Spawn()
    {
        if (Pool.TryPop(out Bullet restoredBullet))
        {
            restoredBullet.gameObject.SetActive(true);
            return restoredBullet;
        }
        Bullet newBullet = Instantiate(bulletPrefab);
        newBullet.gameObject.SetActive(true);
        return newBullet;
    }
}
