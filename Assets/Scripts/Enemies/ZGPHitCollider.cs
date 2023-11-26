using UnityEngine;

namespace ZGP.Game
{
    public class ZGPHitCollider : MonoBehaviour
    {
        private ZGPBaseZombie baseScript;

        void Awake()
        {
            baseScript = GetComponentInParent<ZGPBaseZombie>();
        }

        void OnTriggerStay(Collider hit)
        {
            if(hit.gameObject.CompareTag("Player")){
                baseScript.IsPlayerInsideHitCollider = true;
            }
        }

        void OnTriggerExit(Collider hit)
        {
            if(hit.gameObject.CompareTag("Player")){
                baseScript.IsPlayerInsideHitCollider = false;
            }
        }
    }
}