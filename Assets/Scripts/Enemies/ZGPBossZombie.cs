using System.Collections;
using UnityEngine;

namespace ZGP.Game
{
    public class ZGPBossZombie : ZGPBaseZombie, IPushable
    {
        private readonly float[] attackRangeRandom = new float[] {1.5f, 2f, 1f, 1.3f, 0.6f, 0.85f};
        private readonly float[] moveSpeedPhase = new float[] { 11, 8, 5 };
        private readonly float[] scalingPhase = new float[] { 2.5f, 1.8f, 1f };
        private readonly float[] healthPhase = new float[] { 2500, 1600, 850 };
        private bool isDied;
        private int CurrentPhase {get; set;}

        protected override void InitalizeZombie()
        {    
            transform.localScale = Vector3.one * scalingPhase[CurrentPhase];
            zombieClass.MaxHealth = zombieSettings.health;
            zombieClass.CurrentHealth = zombieSettings.health;
            zombieClass.Speed = zombieSettings.speed;
            agent.speed = zombieClass.Speed;

            int attackIndexRandom = Random.Range(0, 1);
            zombieClass.AttackRange = attackRangeRandom[attackIndexRandom] + attackRangeOffset;

            ZombieMat = GetComponentInChildren<Renderer>().material;
        }

        protected override void Die()
        {
            if(isDied) return;

            switch (CurrentPhase)
            {
                case 0 :
                    Instantiate(killedParticle, transform.position + Vector3.up, Quaternion.identity);
                    ChangePhase(1);
                break;
                case 1 :
                    Instantiate(killedParticle, transform.position + Vector3.up, Quaternion.identity);
                    ChangePhase(2);
                break;
                default:
                    isDied = true;
                    GM.zombies.Remove(this);
                    GM.CheckPhase();
                    Instantiate(killedParticle, transform.position + Vector3.up, Quaternion.identity);
                    Destroy(gameObject);
                break;
            }
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

        private void ChangePhase(int phase){
            Debug.Log("Change Phase!");
            CurrentPhase = phase;

            zombieClass.Speed = moveSpeedPhase[phase];
            agent.speed = zombieClass.Speed;
            zombieClass.MaxHealth = zombieSettings.health;
            zombieClass.CurrentHealth = zombieSettings.health;
            transform.localScale = Vector3.one * scalingPhase[CurrentPhase];
            int attackIndexRandom = phase == 1 ? Random.Range(2, 3) : Random.Range(4, 5);
            zombieClass.AttackRange = attackRangeRandom[attackIndexRandom] + attackRangeOffset;
        }

    }
}