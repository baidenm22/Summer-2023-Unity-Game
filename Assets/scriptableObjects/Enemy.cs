using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "enemyBase")]
public class Enemy : ScriptableObject
{
    public enum types { range, ram, bomb, boss, all };

    [SerializeField] string Enemy_Name;
    [SerializeField] types enemyType;
    [Space]
    [Header("Stats")]
    [SerializeField] int Basic_Damage;
    [SerializeField] int Strong_Damage;
    [SerializeField] int Health;
    [SerializeField] float Attack_Speed;
    [SerializeField] int movementSpeed;
    [Space]
    [Header("Vision")]
    [SerializeField] float maxSight;
    [SerializeField] float rangeAttackSight;
    [SerializeField] float closeAttackSight;
    [Space]
    [Header("Shooting")]
    [SerializeField] bool circleShootAttack;
    [SerializeField] bool straightShootAttack;
    [SerializeField] int numberOfBullets;
    [SerializeField] float timeBetweenFire;
    [SerializeField] [Range(0, (float)2 * Mathf.PI)] float angleAddRadians;
    [Space]
    [Header("Spawnables")]
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject tower;
    [SerializeField] GameObject slam;
    [SerializeField] GameObject death;

    [Space]
    [Header("Times: Slam, Ram")]
    [SerializeField] float[] time;

    public int getHealth()
    {
        return Health;
    }
    public int getBasicDamage()
    {
        return Basic_Damage;
    }
    public int getStrongDamage()
    {
        return Strong_Damage;
    }
    public float getAttack_Speed()
    {
        return Attack_Speed;
    }
    public string getType()
    {
        return enemyType.ToString();
    }
    public int getMovementSpeed()
    {
        return movementSpeed;
    }
    public float getVision(int number)
    {
        switch (number)
        {
            case 0:
                return maxSight;

            case 1:
                return rangeAttackSight;

            case 2:
                return closeAttackSight;
        }
        return 0;
    }
    public bool getCircleShoot()
    {
        return circleShootAttack;
    }
    public bool getStraightShoot()
    {
        return straightShootAttack;
    }
    public int getNumberOfBullets()
    {
        return numberOfBullets;
    }
    public float getTimeBetweenFire()
    {
        return timeBetweenFire;
    }
    public GameObject getBulletPrefab()
    {
        return bullet;
    }
    public GameObject getSlamPrefab()
    {
        return slam;
    }
    public GameObject getTowerPrefab()
    {
        return tower;
    }
    public float getAngleAdd()
    {
        return angleAddRadians;
    }
    public float times(int number)
    {
        return (time[number]);
    }
    public GameObject getDeathSpawn()
    {
        return death;
    }
}
