using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZGP.Game
{
    public class ZGPGameManager : MonoBehaviour
    {
        public static ZGPGameManager Instance {get; private set;}
        public ZGPCameraProperties CameraProperties {get; set;}
        public bool IsGameOver {get; set;}
        public ZGPCharacterStatus player;
        public GameObject gameOverGroup;
        public TMP_Text gameOverText;

        void Awake()
        {
            CameraProperties = FindObjectOfType<ZGPCameraProperties>();
            if(Instance != null && Instance != this){
                Destroy(Instance);
            } else {
                Instance = this;
            }
        }

        public void GameOver(bool winning){
            FindObjectOfType<ZGPInputManager>().enabled = false;
            IsGameOver = true;
            gameOverGroup.SetActive(true);

            gameOverText.text = winning ? "You Win!" : "You Died!";
        }

        public void Restart(){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Quit(){
            Application.Quit();
        }
    }
}