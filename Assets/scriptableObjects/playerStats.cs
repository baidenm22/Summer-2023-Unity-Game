using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "persitanceStats")]
public class playerStats : ScriptableObject
{
    [SerializeField] int MaxHealth;
    [SerializeField] int health;
    [SerializeField] int damageBasic;
    [SerializeField] int damageStrong;

    private void OnEnable()
    {
       health = MaxHealth;
    }
    public int getMaxHealth()
    {
        return MaxHealth;
    }
    public int getHealth()
    {
        return health;
    }
    public int changeHealth(int HealthChange)
    {
        health += HealthChange;
        health = Mathf.Clamp(health, 0, MaxHealth);
        return health;
    }
    public int getBasicDamage()
    {
        return damageBasic;
    }
    public int getStrongDamage()
    {
        return damageStrong;
    }
}
