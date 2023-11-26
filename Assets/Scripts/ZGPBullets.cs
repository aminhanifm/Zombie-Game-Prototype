using UnityEngine;

namespace ZGP.Game
{
    public class ZGPBullets : MonoBehaviour
    {
        public Rigidbody rb;
        public int BaseDamage {private get; set;}
        public GameObject Owner {private get; set;}

        void OnCollisionEnter(Collision hit)
        {
            if(hit.transform.CompareTag("ZombieHead")){
                hit.transform.GetComponentInParent<ZGPBaseZombie>().TakeDamage(BaseDamage * 2);
            } else if (hit.transform.CompareTag("ZombieBody")){
                hit.transform.GetComponentInParent<ZGPBaseZombie>().TakeDamage(BaseDamage);
            } 

            Destroy(gameObject);
        }
    }
}