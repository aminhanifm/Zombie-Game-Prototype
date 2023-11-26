using UnityEngine;

namespace ZGP.Game
{
    public class ZGPSpawner : MonoBehaviour
    {
        private ZGPGameManager GM => ZGPGameManager.Instance;
        public ZombieSO.ZombieType spawnerType;

        public void Spawn(int index){
            ZGPBaseZombie z = Instantiate(GM.zombiePrefabs[index], transform.position, Quaternion.identity);
            GM.zombies.Add(z);
        }
    }
}