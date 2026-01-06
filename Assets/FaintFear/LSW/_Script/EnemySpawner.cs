using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private List<Transform> spawnerPositions = new List<Transform>();

    [SerializeField] private GameObject enemyPrefab; // 생성할 적 프리팹
    [SerializeField] private float spawnInterval = 60.0f;

    private void Awake()
    {
        // 자식 오브젝트들을 스폰 지점으로 등록
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnerPositions.Add(transform.GetChild(i));
        }
    }

    private void Start()
    {
        // 게임 시작 시 코루틴 실행
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // while(true)를 사용하여 무한 반복 (주기적 생성)
        while (true)
        {
            SpawnEnemy();

            // spawnInterval 만큼 대기
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (spawnerPositions.Count == 0 || enemyPrefab == null) return;

        // 랜덤한 스폰 위치 선택
        int randomIndex = Random.Range(0, spawnerPositions.Count);
        Transform spawnPoint = spawnerPositions[randomIndex];

        // 적 생성
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}