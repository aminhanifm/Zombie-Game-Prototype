using UnityEngine;
using System.Collections;

namespace ZGP.Game
{
    public class ZGPNormalZombie : ZGPBaseZombie, IPushable
    {
        private readonly float[] healthRandom = new float[] {100, 125, 150, 175};
        private readonly float[] attackRangeRandom = new float[] {0.4f, 0.6f};
        protected override void InitalizeZombie()
        {
            foreach (var model in models)
            {
                model.SetActive(false);
            }

            int randomIndexModel = Random.Range(0, models.Length);
            models[randomIndexModel].SetActive(true);
            
            int randomIndexHealth = Random.Range(0, healthRandom.Length);
            zombieClass.MaxHealth = healthRandom[randomIndexHealth];
            zombieClass.CurrentHealth = zombieClass.MaxHealth;
            zombieClass.Speed = zombieSettings.speed;
            agent.speed = zombieClass.Speed;

            int attackIndexRandom = Random.Range(0, attackRangeRandom.Length);
            zombieClass.AttackRange = attackRangeRandom[attackIndexRandom] + attackRangeOffset;

            ZombieMat = models[randomIndexModel].GetComponent<Renderer>().material;
        
            switch (GM.CurrentPhase)
            {
                case 2:
                    zombieClass.CurrentHealth += 50;
                break;
                case 4:
                    zombieClass.CurrentHealth += 100;
                break;
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
    }
}