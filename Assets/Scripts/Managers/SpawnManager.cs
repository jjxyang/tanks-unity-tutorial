using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {


    public EnemySpawnData[] enemySpawns;
    public GameObject player;

    [System.Serializable]
    public struct EnemySpawnData
    {
        public GameObject prefab;
        public float minSpawnRadius;  // how far from player will enemies spawn at minimum
        public float maxSpawnRadius;  // how far from player will enemies spawn at maximum
        public float minSpawnDelay;  // number of seconds between spawns at hardest difficulty
        public float maxSpawnDelay;  // number of seconds between spawns at the start
        public float timeUntilMaxSpawnDelay; // number of seconds between enemy beginning to spawn and the enemy spawning at its maximum rate
        public float firstSpawnTime;     // How many seconds to wait before this enemy type spawns
        public float offsetY;  // offset from the ground
        public float prefabRadius;  // collision radius for prefab


        [HideInInspector]
        public float spawnTimer;
        [HideInInspector]
        public float currentSpawnDelay;

        // While the constructor is not required if you are always setting it in the inspector it is good practice
        public EnemySpawnData(GameObject _prefab, float _minSpawnRadius, float _maxSpawnRadius,
            float _minSpawnDelay, float _maxSpawnDelay,
            float _timeUntilMaxSpawnDelay, float _firstSpawnTime, float _offsetY, float _prefabRadius)
        {
            prefab = _prefab;
            minSpawnRadius = _minSpawnRadius;
            maxSpawnRadius = _maxSpawnRadius;
            minSpawnDelay = _minSpawnDelay;
            maxSpawnDelay = _maxSpawnDelay;
            timeUntilMaxSpawnDelay = _timeUntilMaxSpawnDelay;
            firstSpawnTime = _firstSpawnTime;
            offsetY = _offsetY;
            spawnTimer = maxSpawnDelay;
            currentSpawnDelay = maxSpawnDelay;
            prefabRadius = _prefabRadius;
        }
    }

	void Update () {
        for ( int i = 0; i < enemySpawns.Length; i++) {
            enemySpawns[i].firstSpawnTime -= Time.deltaTime; 
            if(enemySpawns[i].firstSpawnTime > 0) // Wait firstSpawnTime seconds before you spawn this enemy type
            {
                continue;
            }

            float lerpX = Mathf.Abs(enemySpawns[i].firstSpawnTime) / enemySpawns[i].timeUntilMaxSpawnDelay;
            lerpX = Mathf.Clamp(lerpX, 0.0f, 1.0f);
            enemySpawns[i].currentSpawnDelay = Mathf.Lerp(enemySpawns[i].maxSpawnDelay, enemySpawns[i].minSpawnDelay, lerpX);
            enemySpawns[i].spawnTimer -= Time.deltaTime;

            if (enemySpawns[i].spawnTimer <= 0)
            {
                float enemyX = player.transform.position.x + Random.Range(enemySpawns[i].minSpawnRadius, enemySpawns[i].maxSpawnRadius);
                float enemyZ = player.transform.position.z + Random.Range(enemySpawns[i].minSpawnRadius, enemySpawns[i].maxSpawnRadius);
                Vector3 enemyPosition = new Vector3(enemyX, player.transform.position.y + enemySpawns[i].offsetY, enemyZ);

                Debug.Log("can i spawn? " + !Physics.CheckSphere(enemyPosition, enemySpawns[i].prefabRadius));
                Debug.Log("spawning at" + enemyPosition.ToString());

                if (!Physics.CheckSphere(enemyPosition, enemySpawns[i].prefabRadius))
                {
                    GameObject newEnemy = Instantiate(enemySpawns[i].prefab);
                    newEnemy.transform.position = enemyPosition;
                }
                enemySpawns[i].spawnTimer = enemySpawns[i].currentSpawnDelay;
            }
        }
	}    
}
