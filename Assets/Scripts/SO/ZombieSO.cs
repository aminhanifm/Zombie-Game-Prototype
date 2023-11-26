using UnityEngine;

[CreateAssetMenu(menuName = "Zombie-Game-Prototype/ZombieSO")]
public class ZombieSO : ScriptableObject
{
    public enum ZombieType{
        Normal,
        Clown,
        Boss
    }

    public ZombieType zombieType;
    public int health;
    public float speed;
    public float attackRange;
}