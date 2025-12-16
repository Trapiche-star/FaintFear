using System.Collections;
using UnityEngine;

namespace FaintFear
{
    public enum EnemyState
    {
        Wander,         // 배회
        Chase,          // 추적
        SearchLastPos,  // 수색 (놓친 위치로 이동)
        Attack          // 공격
    }

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(SphereCollider))]
    public class Enemy_Ex : Enemy
    {
        [Header("Settings")]
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private LayerMask visionLayer;

        [Header("Stats")]
        [SerializeField] private float moveSpeed = 4.0f;
        [SerializeField] private float rotSpeed = 10.0f;
        [SerializeField] private float rayDistance = 15f;   // 시야 거리
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private float wanderDelay = 2f;
        [SerializeField] private float gravity = 9.81f;

        [Header("Vision")]
        [Range(0, 360)]
        [SerializeField] private float viewAngle = 120f;    // 시야각

        // 상태 변수
        private EnemyState currentState;
        private CharacterController controller;
        private SphereCollider detectTrigger;
        private Transform target;
        private Vector3 lastKnownPos;

        // 이동 관련
        private Vector3 currentDestination;
        private float wanderTimer;
        private Vector3 startPos;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            detectTrigger = GetComponent<SphereCollider>();
            detectTrigger.isTrigger = true;
            startPos = transform.position;
        }

        private void Start()
        {
            ChangeState(EnemyState.Wander);
        }

        private void Update()
        {
            ApplyGravity();

            switch (currentState)
            {
                case EnemyState.Wander:
                    WanderUpdate();
                    break;
                case EnemyState.Chase:
                    ChaseUpdate();
                    break;
                case EnemyState.SearchLastPos:
                    SearchUpdate();
                    break;
                case EnemyState.Attack:
                    AttackUpdate();
                    break;
            }
        }

        // --- 상태별 로직 ---

        private void WanderUpdate()
        {
            wanderTimer += Time.deltaTime;
            float dist = Vector3.Distance(transform.position, currentDestination);

            // 도착했거나 대기 시간이 지났으면 새로운 배회 위치 선정
            if (wanderTimer >= wanderDelay || dist < 0.5f)
            {
                GetNewWanderPosition();
                wanderTimer = 0;
            }

            MoveToTarget(currentDestination);
        }

        private void ChaseUpdate()
        {
            // 추적 중일 때는 ignoreAngle: true를 전달하여 시야각을 무시.
            // 즉, 플레이어가 등 뒤로 가더라도 사거리 내에 있고 벽이 없다면 계속 추적.
            if (CheckLineOfSight(ignoreAngle: true))
            {
                // 플레이어가 감지됨 (시야각 무시, 거리/장애물 통과)
                lastKnownPos = target.position;
                currentDestination = lastKnownPos;

                float dist = Vector3.Distance(transform.position, target.position);
                if (dist <= attackRange)
                {
                    ChangeState(EnemyState.Attack);
                }
                else
                {
                    MoveToTarget(currentDestination);
                }
            }
            else
            {
                // 벽에 가려지거나, 시야 거리(rayDistance) 밖으로 나감
                // 놓침 -> 수색 상태로 전환
                ChangeState(EnemyState.SearchLastPos);
            }
        }

        private void SearchUpdate()
        {
            // 수색 중 다시 발견하면 추적 재개
            if (CheckLineOfSight())
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            MoveToTarget(currentDestination);

            // 마지막 위치에 도착했는데도 없으면 배회 복귀
            if (Vector3.Distance(transform.position, currentDestination) < 0.5f)
            {
                detectTrigger.enabled = true; // 트리거 다시 켜기
                ChangeState(EnemyState.Wander);
            }
        }

        private void AttackUpdate()
        {
            float dist = Vector3.Distance(transform.position, target.position);

            // 추적과 마찬가지로 시야각을 무시(true)하고 거리와 장애물만 체크.
            bool isVisible = CheckLineOfSight(true);

            // 사거리 밖이거나 아예 안 보이면(벽 뒤/거리 밖) 추적 복귀
            if (dist > attackRange || !isVisible)
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            LookAtTarget(target.position);

            // 공격 로직 (애니메이션 실행 등)
        }

        // --- 기능 메서드 ---

        private void MoveToTarget(Vector3 targetPos)
        {
            Vector3 dir = targetPos - transform.position;
            dir.y = 0;
            if (dir.magnitude < 0.1f) return;

            Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotSpeed);
            controller.Move(transform.forward * moveSpeed * Time.deltaTime);
        }

        private void LookAtTarget(Vector3 targetPos)
        {
            Vector3 dir = targetPos - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotSpeed);
            }
        }

        private void ApplyGravity()
        {
            if (!controller.isGrounded)
            {
                controller.Move(Vector3.down * gravity * Time.deltaTime);
            }
        }

        private void GetNewWanderPosition()
        {
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            Vector3 randomPos = startPos + new Vector3(randomCircle.x, 0, randomCircle.y);
            randomPos.y = transform.position.y;
            currentDestination = randomPos;
        }

        // ignoreAngle 매개변수 추가 (기본값 false)
        // true일 경우 시야각 계산(Vector3.Angle)을 건너뜀.
        private bool CheckLineOfSight(bool ignoreAngle = false)
        {
            if (target == null) return false;

            // 눈 위치 (높이 1.5f)
            Vector3 origin = transform.position + Vector3.up * 1.5f;
            // 타겟 위치
            Vector3 targetCenter = target.position;

            Vector3 targetDir = (targetCenter - origin).normalized;
            float distToTarget = Vector3.Distance(origin, targetCenter);

            // 1. 거리 체크 (거리는 항상 체크)
            if (distToTarget > rayDistance) return false;

            // 2. 각도 체크 (ignoreAngle이 false일 때만 체크)
            if (!ignoreAngle)
            {
                if (Vector3.Angle(transform.forward, targetDir) > viewAngle * 0.5f) return false;
            }

            // 3. 레이캐스트 (장애물은 항상 체크)
            if (Physics.Raycast(origin, targetDir, out RaycastHit hit, rayDistance, visionLayer))
            {
                // 타겟 본인 혹은 부모 오브젝트의 태그 확인
                if (hit.transform.CompareTag(playerTag) || (hit.transform.root != null && hit.transform.root.CompareTag(playerTag)))
                {
                    return true;
                }
            }

            return false;
        }

        private void ChangeState(EnemyState newState)
        {
            currentState = newState;
            if (newState == EnemyState.Wander) GetNewWanderPosition();
            else if (newState == EnemyState.SearchLastPos) currentDestination = lastKnownPos;
        }

        // --- 유니티 이벤트 ---

        // 플레이어가 범위 내에 있을 때 계속 감지 시도 (등 뒤에 있다가 앞으로 오는 경우 등 대응)
        private void OnTriggerStay(Collider other)
        {
            if (currentState == EnemyState.Wander && other.CompareTag(playerTag))
            {
                target = other.transform;

                // 최초 발견 시에는 시야각을 체크해야 하므로 기본값(false) 사용
                if (CheckLineOfSight())
                {
                    lastKnownPos = target.position;
                    detectTrigger.enabled = false;
                    ChangeState(EnemyState.Chase);
                }
            }
        }

        // 디버깅용 기즈모 (Scene 뷰 확인용)
        private void OnDrawGizmos()
        {
            // 시야각
            Gizmos.color = Color.yellow;
            Vector3 origin = transform.position + Vector3.up * 1.5f;
            Vector3 leftDir = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
            Vector3 rightDir = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;
            Gizmos.DrawRay(origin, leftDir * rayDistance);
            Gizmos.DrawRay(origin, rightDir * rayDistance);

            // 타겟 연결선
            if (target != null)
            {
                // 기즈모는 기본 시야각 기준으로 표시
                Gizmos.color = CheckLineOfSight() ? Color.green : Color.red;
                Gizmos.DrawLine(origin, target.position);
            }
        }
    }
}