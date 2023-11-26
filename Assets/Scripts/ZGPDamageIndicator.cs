using UnityEngine;
using TMPro;

namespace ZGP.Game
{
    public class ZGPDamageIndicator : MonoBehaviour
    {
        public static ZGPDamageIndicator Instance {get; private set;}
        public TMP_Text textPrefab;

        void Awake()
        {
            if(Instance != null && Instance != this){
                Destroy(Instance);
            } else {
                Instance = this;
            }
        }

        public void ShowDamage(Vector3 position, int damage){
            Vector3 modifiedPos = new Vector3(position.x + Random.Range(-1, 1), position.y + Random.Range(-1, 1), position.z + Random.Range(-1, 1));
            TMP_Text text = Instantiate(textPrefab, modifiedPos, Quaternion.identity);
            text.text = damage.ToString();

            Destroy(text.gameObject, 1f);
        }
    }
}