using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace ZGP.Game
{
    public class ZGPBaseZombie : MonoBehaviour, IDamageable
    {
        protected ZGPGameManager GM => ZGPGameManager.Instance;
        protected Transform playerTransform => GM.player.transform;
        protected NavMeshAgent agent;
        protected Animator animator;
        protected int animationIDWalk;
        protected int animationIDAttack;
        protected ZombieClass zombieClass;
        [SerializeField] protected ParticleSystem killedParticle;
        [SerializeField] protected ZombieSO zombieSettings;
        [SerializeField] protected GameObject[] models;
        protected bool IsAttacking {get; set;}
        protected Material ZombieMat {get; set;}

        private const float setPathDelay = 0.5f;
        protected float attackRangeOffset = 3f;
        private float pathTimer;
        public bool IsPlayerInsideHitCollider {get; set;}

        protected virtual void Awake()
        {
            zombieClass = new ZombieClass();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            AssignAnimationIDs();
            InitalizeZombie();
        }

        protected virtual void Start(){
            ZombieMat?.SetColor("_Color", Color.white);
        }

        private void AssignAnimationIDs(){
            animationIDWalk = Animator.StringToHash("IsWalking");
            animationIDAttack = Animator.StringToHash("IsAttacking");
        }

        protected virtual void Update(){
            if(GM.IsGameOver) return;

            if(!IsAttacking){
                pathTimer += Time.deltaTime;

                if(pathTimer >= setPathDelay){
                    pathTimer = 0;
                    SetPath(playerTransform.position);
                }

                animator.SetBool(animationIDWalk, agent.remainingDistance > agent.stoppingDistance
                    || agent.velocity.sqrMagnitude > 1);
            }

            CheckDistanceToPlayer();
        }

        protected virtual void CheckDistanceToPlayer(){
            if(GM == null || IsAttacking) return;
            
            Vector3 offset = playerTransform.position - transform.position;

            // Debug.Log(Vector3.SqrMagnitude(offset) + " / " + zombieClass.AttackRange);

            if(Vector3.SqrMagnitude(offset) <= zombieClass.AttackRange){
                Attack();
                // Debug.Log("Attack");
            }
        }

        protected virtual void InitalizeZombie(){
            
        }

        protected virtual void SetPath(Vector3 pos){
            agent.SetDestination(pos);
        }

        protected virtual void Attack(){
            IsAttacking = true;
            agent.ResetPath();
            transform.LookAt(playerTransform, Vector3.up);
            animator.SetTrigger(animationIDAttack);
        }

        protected virtual void CheckHit(){
            if(IsPlayerInsideHitCollider){
                GM.player.TakeDamage(1);
            }
        }

        protected virtual void EndAttack(){
            IsAttacking = false;
        }

        protected virtual void Die(){
            Instantiate(killedParticle, transform.position + Vector3.up, Quaternion.identity);
            Destroy(gameObject);
        }

        public virtual void TakeDamage(float damage){
            // Debug.Log(zombieClass.CurrentHealth + " / " + damage);
            if(zombieClass.CurrentHealth <= 0) return;

            zombieClass.CurrentHealth -= damage;
            ZGPDamageIndicator.Instance.ShowDamage(transform.position + (Vector3.up * 4), (int)damage);

            StartCoroutine(HurtEffect());

            if(zombieClass.CurrentHealth <= 0){
                Die();
            }
        }

        private IEnumerator HurtEffect(){
            ZombieMat.SetColor("_Color", Color.red);
            yield return new WaitForSeconds(0.1f);
            ZombieMat.SetColor("_Color", Color.white);
        }

        protected virtual void OnDestroy()
        {
            
        }

        void OnDrawGizmosSelected()
        {
            if(zombieClass == null) return;

            Color color = Color.red;
            color.a = 0.5f;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, zombieClass.AttackRange);
        }
    }
}