using UnityEngine;

namespace ZGP.Game
{
    public class ZGPGameManager : MonoBehaviour
    {
        public static ZGPGameManager Instance;
        public bool IsGameOver {get; set;}

        void Awake()
        {
            if(Instance != null && Instance != this){
                Destroy(Instance);
            } else {
                Instance = this;
            }
        }

        public void GameOver(){
            IsGameOver = true;
        }
    }
}