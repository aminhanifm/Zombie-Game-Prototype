using UnityEngine;

namespace ZGP.Game
{
    [CreateAssetMenu(menuName = "Zombie-Game-Prototype/WeaponSO")]
    public class WeaponSO : ScriptableObject
    {
        public int maxAmmo;
        public int baseDamage;
        public float bulletSpeed;
        public float bulletLifeTime;
        public float reloadTime;
        public float fireRate;
        public ZGPBullets bulletPrefab;
    }
}