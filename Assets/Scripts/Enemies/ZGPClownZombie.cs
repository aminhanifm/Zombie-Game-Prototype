using System.Collections;
using UnityEngine;

namespace ZGP.Game
{
    public class ZGPClownZombie : ZGPBaseZombie
    {
        [SerializeField] private ParticleSystem explosionPrefab;
        private readonly float[] attackRangeRandom = new float[] {0.4f, 0.6f};
        private bool isDied;
        private float explosionRadius = 2;
        private float pushDistance = 10f;
        private float pushDuration = .5f;
        protected override void InitalizeZombie()
        {     
            zombieClass.MaxHealth = zombieSettings.health;
            zombieClass.CurrentHealth = zombieSettings.health;
            zombieClass.Speed = zombieSettings.speed;
            agent.speed = zombieClass.Speed;

            int attackIndexRandom = Random.Range(0, attackRangeRandom.Length);
            zombieClass.AttackRange = attackRangeRandom[attackIndexRandom] + attackRangeOffset;

            ZombieMat = GetComponentInChildren<Renderer>().material;
        }

        protected override void Die()
        {
            if(isDied) return;

            isDied = true;
            zombieClass.Speed = 8;
            agent.speed = zombieClass.Speed;

            StartCoroutine(ExplosionCoroutine());
        }

        protected override void Attack()
        {
            if(zombieClass.CurrentHealth > 0){
                IsAttacking = true;
                agent.ResetPath();
                transform.LookAt(playerTransform, Vector3.up);
                animator.SetTrigger(animationIDAttack);
            }
        }

        private IEnumerator ExplosionCoroutine(){

            yield return new WaitForSeconds(0.1f);
            ZombieMat.SetColor("_Color", Color.red);

            yield return new WaitForSeconds(2.9f);

            Explosion();
        }

        private void Explosion(){
            Instantiate(explosionPrefab, transform.position + Vector3.up, Quaternion.identity);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up, explosionRadius);
            
            foreach (Collider hitCollider in hitColliders){
                if(hitCollider.TryGetComponent<IPushable>(out IPushable pushable)){
                    Vector3 pushDirection = (hitCollider.transform.position - transform.position).normalized;
                    pushDirection.Normalize();
                    pushable.ApplyPush(pushDirection, pushDistance, pushDuration);
                }

                if(hitCollider.TryGetComponent<IDamageable>(out IDamageable damageable)){
                    if(hitCollider.CompareTag("Player")){
                        damageable.TakeDamage(1);
                    } else {
                        damageable.TakeDamage(125);
                    }
                }
            }

            GM.zombies.Remove(this);
            GM.CheckPhase();

            Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            Color color = Color.red;
            color.a = 0.5f;
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position + Vector3.up, explosionRadius);
        }
    }
}