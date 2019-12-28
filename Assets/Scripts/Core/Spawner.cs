using Shooter.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Core
{
    public class Spawner : ObjectPooler
    {
        [Header("Spawner properties")]
        [SerializeField] Wave[] waves;
        [SerializeField] Zombie[] zombiePrefabs;

        int enemiesRemainingToSpawn;
        int enemiesRemainingAlive;
        float nextSpawnTime;
        Wave currentWave;
        int currentWaveNumber = 0;
        protected override void Start()
        {
            objectToPool = zombiePrefabs[0].gameObject;
            base.Start();
            NextWave();
        }

        private void Update()
        {
            if(enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                AddZombieToMap();
            }
        }

        private void AddZombieToMap()
        {
            //pooled object attempt, still broken due to events being called multiple times
            //GameObject zombieGo = GetPooledObject();
            //if (zombieGo != null)
            //{
            //    zombieGo.transform.position = Vector3.zero;
            //    zombieGo.transform.rotation = Quaternion.identity;
            //    Zombie zombie = zombieGo.GetComponent<Zombie>();
            //    if(zombie != null)
            //    {
            //        zombie.Revive();
            //        zombie.OnDeath += OnZombieDeath;
            //    }
            //}
            //----standard instantiation------
            Zombie zombie = Instantiate(zombiePrefabs[0], Vector3.zero, Quaternion.identity);
            zombie.Revive();
            zombie.OnDeath += OnZombieDeath;
        }
        void OnZombieDeath()
        {
            print("Zombie died");
            enemiesRemainingAlive--;
            if(enemiesRemainingAlive <= 0)
            {
                print("Wave: " + currentWaveNumber);
                NextWave();
            }
        }
        void NextWave()
        {
            currentWaveNumber++;
            if (currentWaveNumber - 1 < waves.Length)
            {
                currentWave = waves[currentWaveNumber - 1];
                enemiesRemainingToSpawn = currentWave.enemyCount;
                enemiesRemainingAlive = enemiesRemainingToSpawn;
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
