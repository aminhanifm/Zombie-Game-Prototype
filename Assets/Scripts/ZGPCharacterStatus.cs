using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ZGP.Game
{
    public class ZGPCharacterStatus : MonoBehaviour, IPushable, IDamageable
    {
        private ZGPGameManager GM => ZGPGameManager.Instance;
        [Header("Text References")]
        public TMP_Text liveText;
        public TMP_Text ammoText;
        [Space(20)]
        public CanvasGroup hurtEffect;
        public WeaponSO weapon;
        public WeaponClass weaponClass;
        public Transform bulletHole;
        public RigBuilder rigBuilder;
        public float health = 5;
        private float maxHealth = 5;
        public bool IsReloading {get; set;}
        public bool IsShooting {get; set;}
        
        private Animator animator;
        private float curFireRate;
        private int animIDShoot;
        private int animIDReload;
        private int animIDDead;

        void Awake()
        {
            animator = GetComponent<Animator>();
            AssignAnimationIDs();
            
            weaponClass = new WeaponClass();
            weaponClass.maxAmmo = weapon.maxAmmo;
            weaponClass.currentAmmo = weapon.maxAmmo;
        }

        void Update()
        {
            if(GM.IsGameOver) return;

            UpdateTextStatus();

            if(IsShooting){
                if(weaponClass.currentAmmo <= 0){
                    StopShooting();
                    Reload();
                    return;
                }

                if(curFireRate < weapon.fireRate){
                    curFireRate += Time.deltaTime;
                } else if(curFireRate >= weapon.fireRate ){
                    ZGPBullets bullet = Instantiate(weapon.bulletPrefab, bulletHole.position, bulletHole.rotation);
                    bullet.rb.AddForce(bulletHole.forward * weapon.bulletSpeed, ForceMode.Impulse);
                    // bullet.rb.AddTorque(Random.insideUnitSphere * weapon.bulletSpeed, ForceMode.Impulse);
                    bullet.BaseDamage = weapon.baseDamage;
                    bullet.Owner = gameObject;
                    weaponClass.currentAmmo -= 1;
                    Destroy(bullet.gameObject, weapon.bulletLifeTime);
                    curFireRate = 0;
                }

            }
        }

        void AssignAnimationIDs()
        {
            animIDShoot = Animator.StringToHash("Shoot_b");
            animIDReload = Animator.StringToHash("Reload_b");
            animIDDead = Animator.StringToHash("Death_b");
        }

        public void UpdateTextStatus(){
            liveText.SetText($"Lives\n<size=50>{health}/{maxHealth}</size>");
            if(IsReloading) ammoText.SetText("Reloading...");
            else ammoText.SetText($"Ammo\n<size=50>{weaponClass.currentAmmo}/{weaponClass.maxAmmo}</size>");
        }

        public void Shoot(){
            if(IsReloading || GM.IsGameOver) return;

            if(weaponClass.currentAmmo > 0) {
                IsShooting = true;
                animator.SetBool(animIDShoot, true);
            }
        }

        public void StopShooting(){
            IsShooting = false;
            animator.SetBool(animIDShoot, false);
        }

        public void Reload(){
            if(IsReloading || GM.IsGameOver || weaponClass.currentAmmo >= weaponClass.maxAmmo) return;

            StartCoroutine(ReloadCoroutine());
        }

        private IEnumerator ReloadCoroutine(){
            IsReloading = true;
            animator.SetBool(animIDShoot, false);
            animator.SetBool(animIDReload, true);
            yield return new WaitForSeconds(weapon.reloadTime);
            IsReloading = false;
            weaponClass.currentAmmo = weapon.maxAmmo;
            animator.SetBool(animIDReload, false);
        }

        public void TakeDamage(float damage){
            if(health <= 0 || GM.IsGameOver) return;

            StartCoroutine(HurtEffect());
            ZGPDamageIndicator.Instance.ShowDamage(transform.position + (Vector3.up * 4), (int)damage);
            health -= damage;

            if(health <= 0){
                health = 0;
                rigBuilder.layers[0].active = false;
                animator.SetBool(animIDDead, true);
                GM.GameOver(false);
            }
        }

        private IEnumerator HurtEffect(){
            hurtEffect.alpha = 1;

            yield return new WaitForSeconds(0.15f);

            hurtEffect.alpha = 0;
        }

        public void ApplyPush(Vector3 pushDirection, float pushDistance, float pushDuration){
            StartCoroutine(PushTransform(pushDirection, pushDistance, pushDuration));
        }

        public IEnumerator PushTransform(Vector3 pushDirection, float pushDistance, float pushDuration){
            float elapsedTime = 0.0f;
            Vector3 pushVelocity = pushDirection * pushDistance;
            while (elapsedTime < pushDuration)
            {
                float t = elapsedTime / pushDuration;
                float forceMultiplier = 1 - t * t * t; // Cubic easing out function
                transform.Translate(pushVelocity * Time.deltaTime * forceMultiplier, Space.World);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}