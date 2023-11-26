using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace ZGP.Game
{
    public class ZGPGameManager : MonoBehaviour
    {
        public static ZGPGameManager Instance {get; private set;}
        public ZGPCameraProperties CameraProperties {get; set;}
        public bool IsGameOver {get; set;}
        public bool BossSpawned {get; set;}
        public int CurrentPhase {get; set;}
        public int KillCount {get; set;}
        public ZGPCharacterStatus player;
        public GameObject gameOverGroup;
        public TMP_Text gameOverText, waveText;
        public ZGPSpawner[] spawners;
        public ZGPBaseZombie[] zombiePrefabs;
        private Coroutine spawner;

        [NonSerialized] public List<ZGPBaseZombie> zombies = new List<ZGPBaseZombie>();

        void Awake()
        {
            CameraProperties = FindObjectOfType<ZGPCameraProperties>();
            if(Instance != null && Instance != this){
                Destroy(Instance);
            } else {
                Instance = this;
            }
        }

        void Start()
        {
            ChangePhase(0, true);
        }

        public void GameOver(bool winning){
            FindObjectOfType<ZGPInputManager>().enabled = false;
            IsGameOver = true;
            gameOverGroup.SetActive(true);

            gameOverText.text = winning ? "You Win!" : "You Died!";
        }

        public void CheckPhase(){
            switch (CurrentPhase)
            {
                case 4:
                    if(BossSpawned){
                        if(zombies.Count == 0){
                            GameOver(true);
                        }
                    } else {
                        KillCount++;
                        // Debug.Log("Kill: " + KillCount);
                        if(KillCount >= 20){
                            StopCoroutine(spawner);
                            spawner = null;

                            // Debug.Log("Spawn Boss!");
                            SpawnZombie(ZombieSO.ZombieType.Boss);
                            SetWaveText("Wave\n<size=50><color=red>BOSS</color></size>");
                        }
                    }
                break;
                default:
                    // Debug.Log(zombies.Count + " / " + spawner);
                    if(zombies.Count == 0 && spawner == null){
                        ChangePhase(CurrentPhase + 1, true);
                    }
                break;
            }
        }

        public void ChangePhase(int phase, bool clearZombies){
            if(clearZombies){
                for (int i = zombies.Count - 1; i >= 0; i--)
                {
                    Destroy(zombies[i].gameObject);
                }
            }
            
            switch (phase)
            {
                case 0:
                    List<int> zombieType_0 = new List<int>(10){0};
                    SpawnZombieEveryXSeconds(5, 3, 10, 10, zombieType_0);
                    // SpawnZombieEveryXSeconds(0, 1f, 10, 10, zombieType_0);
                    SetWaveText("Wave\n<size=50>1</size>");
                break;
                case 1:
                    List<int> zombieType_1 = new List<int>(11){1,0,0,0,0,0,0,0,0,0,0};
                    zombieType_1.Shuffle();
                    SpawnZombieEveryXSeconds(0, 1, 11, 11, zombieType_1);
                    SetWaveText("Wave\n<size=50><color=yellow>Special!</color></size>");
                break;
                case 2:
                    List<int> zombieType_2 = new List<int>(10){0};
                    SpawnZombieEveryXSeconds(0, 2, 10, 15, zombieType_2);
                    SetWaveText("Wave\n<size=50>2</size>");
                break;
                case 3:
                    List<int> zombieType_3 = new List<int>(18){1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    zombieType_3.Shuffle();
                    SpawnZombieEveryXSeconds(0, 1, 10, 18, zombieType_3);
                    SetWaveText("Wave\n<size=50><color=yellow>Special!</color></size>");
                break;
                case 4:
                    List<int> zombieType_4 = new List<int>(){0};
                    SpawnZombieEveryXSeconds(0, 2, 10, 9999, zombieType_4);
                    // SpawnZombieEveryXSeconds(0, 0.1f, 20, 9999, zombieType_4);
                    SetWaveText("Wave\n<size=50>4</size>");
                break;
            }

            CurrentPhase = phase;
        }

        private void SetWaveText(string s){
            waveText.SetText(s);
        }

        public void SpawnZombieEveryXSeconds(float delay, float seconds, int maxZombiesIngame, int totalZombieSpawned, List<int> zombieType){
            spawner = StartCoroutine(SpawnZombieEveryXSecondsCoroutine(delay, seconds, maxZombiesIngame, totalZombieSpawned, zombieType));
        }

        private IEnumerator SpawnZombieEveryXSecondsCoroutine(float delay, float seconds, int maxZombiesIngame, int totalZombieSpawned, List<int> zombieType){
            yield return new WaitForSeconds(delay);
            
            int count = 0;

            while(count < totalZombieSpawned){
                
                yield return new WaitForSeconds(seconds);

                while (zombies.Count >= maxZombiesIngame)
                {
                    yield return null;
                }

                Debug.Log(count);

                try
                {
                    SpawnZombie((ZombieSO.ZombieType)zombieType.ElementAt(count));
                }
                catch (System.Exception)
                {
                    SpawnZombie(ZombieSO.ZombieType.Normal);
                }

                count++;

                yield return null;
            }

            spawner = null;
        }

        public void SpawnZombie(ZombieSO.ZombieType type){
            List<ZGPSpawner> spawnerList = spawners.ToList();
            int randomIndex;

            if(type == ZombieSO.ZombieType.Boss){
                randomIndex = 4;
            } else {
                randomIndex = Random.Range(0, 4);
            }
            
            ZGPSpawner sp = spawnerList[randomIndex];
            if(Vector3.SqrMagnitude(sp.transform.position - player.transform.position) < 5){
                SpawnZombie(type);
            } else {
                sp.Spawn((int)type);
            }

            if(type == ZombieSO.ZombieType.Boss){
                BossSpawned = true;
            }
        }

        public void Restart(){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Quit(){
            Application.Quit();
        }
    }
}