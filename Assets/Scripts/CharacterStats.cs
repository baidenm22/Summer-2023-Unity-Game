using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] Enemy enemyData;
    [SerializeField] int MaxSplat;
    [SerializeField] int splat;
    [SerializeField] int damageBasic;
    [SerializeField] int damageStrong;
    [SerializeField] float despawnTime;
    void Start()
    {
        if (enemyData != null)
        {
            MaxSplat = enemyData.getHealth();
            splat = MaxSplat;
            damageBasic = enemyData.getBasicDamage();
            damageStrong = enemyData.getStrongDamage();
        }
        else
        {
            destroyOnTime(despawnTime);
            splat = MaxSplat;
        }
    }
    public void setMaxHealth(int health)
    {
        MaxSplat = health;
    }
    public int getMaxHealth()
    {
        return MaxSplat;
    }
    public int getHealth()
    {
        return splat;
    }
    public void replenishHealth()
    {
        splat = MaxSplat;
    }
    public int changeHealth(int HealthChange)
    {
        splat += HealthChange;
        splat = Mathf.Clamp(splat, 0, MaxSplat);
        return splat;
    }
    public int getBasicDamage()
    {
        return damageBasic; 
    }
    public int getStrongDamage()
    {
        return damageStrong;
    }

    public void destroyObject()
    {
        GameObject.Destroy(this.gameObject);
    }

    public void destroyOnTime(float time)
    {
        if (time != 0)
        {
            Invoke("destroyObject", time);
        }
    }
}
