using Shooter.Characters;
using Shooter.Map;
using System;
using System.Collections;
using UnityEngine;

namespace Shooter.Core
{
    public class Spawner : MonoBehaviour
    {
        [Header("Spawner properties")]
        [SerializeField] Wave[] waves;
        [SerializeField] Zombie[] zombiePrefabs; //support for more types of zombies
        [SerializeField] int zombieType = 0;
        [SerializeField] Color flashColor = Color.red;
        [SerializeField] float tileFlashesPerSecond = 4f;
        [SerializeField] float spawnDelay = 1f;

        int enemiesRemainingToSpawn;
        int enemiesRemainingAlive;
        float nextSpawnTime;
        Wave currentWave;
        int currentWaveNumber = 0;

        MapGenerator map;

        [Header("Anti-Camping properties")]
        LivingEntity player;
        [SerializeField] float timeBetweenCampChecks = 2f;
        [SerializeField] float campThreashold = 1.5f;
        [SerializeField] bool isCamping;
        float nextCampCheckTime;
        Vector3 playerPriorPosition;
        bool isDisabled;

        public event System.Action<int> OnNewWave;

        void Start()
        {
            SetupAntiCamping();
            map = FindObjectOfType<MapGenerator>();
            NextWave();
        }

        private void SetupAntiCamping()
        {
            player = FindObjectOfType<Player>();
            nextCampCheckTime = Time.time + timeBetweenCampChecks;
            playerPriorPosition = player.transform.position;
            player.OnDeath += OnPlayerdeath;
        }

        private void Update()
        {
            if (isDisabled) { return; }
            CheckPlayerCamping();
            CheckSpawning();
        }

        private void CheckPlayerCamping()
        {
            if(Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampChecks;
                isCamping = (Vector3.Distance(player.transform.position, playerPriorPosition) < campThreashold);
                playerPriorPosition = player.transform.position;
            }
        }
        private void CheckSpawning()
        {
            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine(AddZombieToMap());
            }
        }
        private IEnumerator AddZombieToMap()
        {
            Transform spawnTile;
            if (isCamping)
            {
                spawnTile = map.PositionToTile(player.transform.position);
            }
            else
            {
                spawnTile = map.GetRandomOpenTile();
            }
            Material tileMaterial = spawnTile.GetComponent<Renderer>().material;
            Color initialColor = tileMaterial.color;
            float spawnTimer = 0f;
            while (spawnTimer < spawnDelay)
            {
                tileMaterial.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashesPerSecond, 1));
                spawnTimer += Time.deltaTime;
                yield return null;
            }
            tileMaterial.color = initialColor;
            Zombie zombie = Instantiate(zombiePrefabs[zombieType], spawnTile.position + Vector3.up, Quaternion.identity);
            zombie.Revive();
            zombie.OnDeath += OnZombieDeath;
        }
        void OnZombieDeath()
        {
            print("Zombie died");
            enemiesRemainingAlive--;
            if (enemiesRemainingAlive <= 0)
            {
                print("Wave: " + currentWaveNumber);
                NextWave();
            }
        }
        void OnPlayerdeath()
        {
            isDisabled = true;
        }
        void NextWave()
        {
            currentWaveNumber++;
            if (currentWaveNumber - 1 < waves.Length)
            {
                currentWave = waves[currentWaveNumber - 1];
                enemiesRemainingToSpawn = currentWave.enemyCount;
                enemiesRemainingAlive = enemiesRemainingToSpawn;
                if(OnNewWave != null)
                {
                    OnNewWave(currentWaveNumber);
                }
                ResetPlayerPosition();
            }
        }
        void ResetPlayerPosition()
        {
            Transform t = map.PositionToTile(Vector3.zero);
            if (t != null)
            {
                player.transform.position = t.position + Vector3.up;
            }
            else {
                player.transform.position = Vector3.up;
            }
        }

        [System.Serializable]
        public class Wave
        {
            public int enemyCount;
            public float timeBetweenSpawns;
        }
    }
}
