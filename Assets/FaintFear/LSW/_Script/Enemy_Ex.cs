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
        private PlayerHealth playerHealth;

        [Header("Stats")]
        [SerializeField] private float moveSpeed = 4.0f;
        [SerializeField] private float rotSpeed = 5.0f;     // 추적 전투 시의 빠른 회전 속도
        [SerializeField] private float rayDistance = 15f;   // 시야 거리
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float gravity = 9.81f;

        // 공격 판정 관련 설정
        [Header("Attack Settings")]
        [SerializeField] private float attackImpactRadius = 1.0f;   // 실제 타격 범위 반경
        [SerializeField] private int damage = 30;
        [SerializeField] private Vector3 attackOffset = new Vector3(0, 1.0f, 1.0f); // 타격 위치 (캐릭터 앞)
        [SerializeField] private LayerMask targetLayer;             // 플레이어 레이어
        [SerializeField] private float attackCooldown = 2.0f;       // 공격 쿨타임

        [Header("Wander Settings")]
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private float minIdleTime = 2f;    // 배회 도착 후 최소 대기 시간
        [SerializeField] private float maxIdleTime = 5f;    // 배회 도착 후 최대 대기 시간
        [SerializeField] private float lookInterval = 1.5f; // 대기 중 두리번거리는 간격
        [SerializeField] private float idleRotSpeed = 2.0f; // 두리번거릴 때 회전 속도
        [SerializeField] private float walkStraightTime = 1.0f; // 배회 시작 시 무조건 직진하는 시간
        [SerializeField] private float wanderMoveRotSpeed = 1.5f; // 배회 이동 중의 부드러운 회전 속도

        // 장애물 감지 설정
        [Header("Obstacle Settings")]
        [SerializeField] private float obstacleCheckDistance = 1.5f; // 장애물 감지 거리
        [SerializeField] private float blockedWaitTime = 2.0f;       // 장애물 발견 시 대기 시간
        [SerializeField] private string obstacleTag = "Obstacle";    // 장애물 태그

        [Header("Vision")]
        [Range(0, 360)]
        [SerializeField] private float viewAngle = 120f;    // 시야각

        // 상태 변수
        private EnemyState currentState;
        private CharacterController controller;
        private Animator ani;
        private SphereCollider detectTrigger;
        private Transform target;
        private Vector3 lastKnownPos;

        // 이동 관련
        private Vector3 currentDestination;
        private float wanderTimer; // 이동 제한 시간 체크용
        private Vector3 startPos;

        // 배회 대기(Idle) 및 직진 관련 변수
        private bool isWanderIdle = false;  // 현재 멈춰서 두리번거리는 중인지 여부
        private float currentIdleTimer;     // 남은 대기 시간
        private float nextLookTimer;        // 다음 시선 변경까지 남은 시간
        private Quaternion targetIdleRot;   // 두리번거릴 때 목표 회전값
        private float currentWalkStraightTimer; // 이동 시작 후 남은 직진 시간

        // 공격 쿨타임 체크용 변수
        private float lastAttackTime;

        // 장애물로 인해 막혔는지 확인하는 변수
        private bool isBlocked = false;
        private float currentBlockedTimer;

        // + Enemy 전용 사운드 제어 스크립트 참조
        private EnemyAudio enemyAudio;

        //고유 ID
        [SerializeField] private string enemyId;
        public string GetEnemyId() => enemyId;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            detectTrigger = GetComponent<SphereCollider>();
            detectTrigger.isTrigger = true;
            startPos = transform.position;
            ani = GetComponent<Animator>();
            // + SoundManager를 직접 만지지 않고 EnemyAudio를 통해 사운드 제어
            enemyAudio = GetComponent<EnemyAudio>();
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

            //정신력
            UpdateMentalEffects();
        }

        // --- 상태별 로직 ---

        private void WanderUpdate()
        {
            // 1. 대기(Idle) 상태: 목적지 도착 후 멈춰서 주변을 두리번거림
            if (isWanderIdle)
            {
                // 멈춤 상태
                ani.SetInteger("State", 0);

                currentIdleTimer -= Time.deltaTime;
                nextLookTimer -= Time.deltaTime;

                // 두리번거리는 타이밍이 되면 랜덤한 방향 보기
                if (nextLookTimer <= 0)
                {
                    SetRandomLookRotation();
                    nextLookTimer = lookInterval; // 간격 초기화
                }

                // 하드코딩된 숫자 대신 idleRotSpeed 변수 사용 (천천히 회전)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetIdleRot, Time.deltaTime * idleRotSpeed);

                // 대기 시간이 끝나면 다시 이동 시작
                if (currentIdleTimer <= 0)
                {
                    isWanderIdle = false;
                    GetNewWanderPosition();
                    wanderTimer = 0;

                    // 이동 시작 시 일정 시간 동안은 회전 없이 직진만 하도록 타이머 설정
                    currentWalkStraightTimer = walkStraightTime;
                }
            }
            // 2. 이동 상태: 다음 배회 지점으로 이동
            else
            {
                // 이동 상태
                ani.SetInteger("State", 1);

                wanderTimer += Time.deltaTime;
                float dist = Vector3.Distance(transform.position, currentDestination);

                // -------------------------
                // + 적 의심 상태 BGM_Tense 재생
                if (target != null && CheckLineOfSight()) // 플레이어를 발견했지만 아직 Chase 아님
                {
                    if (SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlayBGM("BGM_Tense"); //+
                    }
                }

                // 직진 타이머가 남아있다면 방향을 틀지 않고 앞으로만 이동
                if (currentWalkStraightTimer > 0)
                {
                    currentWalkStraightTimer -= Time.deltaTime;
                    // MoveToTarget 대신 직접 Move 호출 (회전 로직 제외하고 정면 이동)
                    controller.Move(transform.forward * moveSpeed * Time.deltaTime);
                }
                else
                {
                    // 직진 시간이 끝났으면 목적지를 향해 부드럽게 회전하며 이동 (배회 전용 속도 적용)
                    MoveToTarget(currentDestination, wanderMoveRotSpeed);
                }

                // 도착했거나 이동 시간이 너무 오래 걸리면(끼임 방지) -> 대기 모드로 전환
                if (dist < 0.5f || wanderTimer > 8.0f)
                {
                    StartWanderIdle();
                }
            }
        }

        private void ChaseUpdate()
        {
            // 장애물에 막혔을 경우 처리 로직
            if (isBlocked)
            {
                HandleBlockedState();
                return; // 막혀서 대기 중이면 아래 이동 로직 실행 안함
            }

            // 추적 이동 상태
            ani.SetInteger("State", 2);

            // 이동 전 정면 장애물 체크
            if (CheckForwardObstacle())
            {
                isBlocked = true;
                currentBlockedTimer = blockedWaitTime;
                return;
            }

            // 추적 중일 때는 ignoreAngle: true를 전달하여 시야각을 무시
            // 즉, 플레이어가 등 뒤로 가더라도 사거리 내에 있고 벽이 없다면 계속 추적
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
                    // 추적 시에는 빠른 회전 속도 사용
                    MoveToTarget(currentDestination, rotSpeed);
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
            // 장애물에 막혔을 경우 처리 로직
            if (isBlocked)
            {
                HandleBlockedState();
                return;
            }

            // 수색 이동 상태
            ani.SetInteger("State", 1);

            // 이동 전 정면 장애물 체크
            if (CheckForwardObstacle())
            {
                isBlocked = true;
                currentBlockedTimer = blockedWaitTime;
                return;
            }

            // 수색 중 다시 발견하면 추적 재개
            // (기본값 false 사용)
            if (CheckLineOfSight())
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            // 수색 시에도 빠른 회전 속도 사용
            MoveToTarget(currentDestination, rotSpeed);

            // 마지막 위치에 도착했는데도 없으면 배회 복귀
            if (Vector3.Distance(transform.position, currentDestination) < 0.5f)
            {
                detectTrigger.enabled = true; // 트리거 다시 켜기
                ChangeState(EnemyState.Wander);
            }
        }

        private void AttackUpdate()
        {
            // 공격 대기/수행 중에는 제자리
            ani.SetInteger("State", 0);

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

            // 쿨타임 체크 후 공격 애니메이션 실행
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                // Animator에 "Attack" Trigger 파라미터가 있어야 함
                ani.SetTrigger("Attack");

                enemyAudio?.OnAttack();
                // + 적이 공격 애니메이션을 시작하는 순간 호출
                // + 공격 효과음(SFX_EnemyA_01 / SFX_EnemyA_02 중 랜덤) 재생
            }

        }

        // --- 기능 메서드 ---

        // 장애물 감지용 레이캐스트
        private bool CheckForwardObstacle()
        {
            // 발 밑이 아니라 허리쯤에서 쏘기 위해 y값 보정
            Vector3 origin = transform.position + Vector3.up * 1.0f;

            // 정면으로 레이 발사
            if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, obstacleCheckDistance))
            {
                // 부딪힌 물체의 태그가 Obstacle인지 확인
                if (hit.collider.CompareTag(obstacleTag))
                {
                    return true;
                }
            }
            return false;
        }

        // 막혔을 때 대기 및 상태 전환 처리
        private void HandleBlockedState()
        {
            // 멈춰서 대기하므로 애니메이션 State 0
            ani.SetInteger("State", 0);

            currentBlockedTimer -= Time.deltaTime;

            if (currentBlockedTimer <= 0)
            {
                // 대기 시간 끝나면 배회로 강제 전환
                Debug.Log("장애물 발견! 배회로 전환합니다.");
                detectTrigger.enabled = true; // 감지 트리거 다시 켜기
                ChangeState(EnemyState.Wander);
            }
        }

        // 애니메이션 이벤트에서 호출될 함수 (실제 피격 판정)
        public void OnAttackHit()
        {
            // 타격 위치 계산 (캐릭터 기준 앞쪽 오프셋 적용)
            Vector3 hitPoint = transform.position + transform.TransformDirection(attackOffset);

            // 구체 범위 내의 충돌체 검출
            Collider[] hitColliders = Physics.OverlapSphere(hitPoint, attackImpactRadius, targetLayer);

            foreach (var hit in hitColliders)
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);   // 한 번 데미지
                    enemyAudio?.OnHitPlayer();         // 타격 성공 사운드
                    break;
                }
            }
        }

        // 회전 속도를 인자로 받아 상황에 맞게 적용
        private void MoveToTarget(Vector3 targetPos, float turnSpeed)
        {
            Vector3 dir = targetPos - transform.position;
            dir.y = 0;
            if (dir.magnitude < 0.1f) return;

            Quaternion lookRot = Quaternion.LookRotation(dir.normalized);

            // 인자로 받은 turnSpeed를 사용하여 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
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

        // 배회 중 대기 상태 진입
        private void StartWanderIdle()
        {
            isWanderIdle = true;
            // 대기 시간 랜덤 설정
            currentIdleTimer = Random.Range(minIdleTime, maxIdleTime);

            // 즉시 한 번 시선 변경
            SetRandomLookRotation();
            nextLookTimer = lookInterval;
        }

        // 제자리에서 랜덤한 방향 설정
        private void SetRandomLookRotation()
        {
            // 완전 랜덤한 방향 (360도)
            float randomAngle = Random.Range(0f, 360f);
            targetIdleRot = Quaternion.Euler(0, randomAngle, 0);
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
            //적 추적에서 벗어나는 순간 정신력 회복
            if (currentState == EnemyState.SearchLastPos && newState == EnemyState.Wander)
            {
                if (playerHealth != null)
                {
                    playerHealth.HealInstant(20f);
                }
            }

            if (currentState != EnemyState.Chase && newState == EnemyState.Chase)
            {
                enemyAudio?.OnChaseStart();
                // + 배회 / 수색 상태에서 → 추적으로 처음 진입하는 순간
                // + 추적 시작 효과음(SFX_EnemyStart) + 추적 BGM(BGM_Chase) 재생
            }

            if (currentState == EnemyState.Chase && newState == EnemyState.SearchLastPos)
            {
                enemyAudio?.OnChaseEnd();
                // + 추적 중 플레이어를 놓쳐 수색 상태로 전환될 때
                // + 추적 종료 BGM(BGM_ChaseEnd) 재생
            }

            // + 추가: 의심 상태 진입 시마다 BGM_Tense 재생
            if (currentState == EnemyState.Wander && newState == EnemyState.Chase)
            {
                // 적이 배회 → Chase (의심/추적) 상태로 바뀔 때마다
                SoundManager.Instance.PlayBGM("BGM_Tense"); // 조건 만족할 때마다 재생
            }

            currentState = newState;

            // 상태 변경 후 런타임 기록
            RuntimeStateManager.RecordEnemyState(enemyId,CaptureRuntimeState());

            // 상태 변경 시 대기 관련 플래그 초기화
            isWanderIdle = false;

            // 상태 변경 시 막힘 상태 해제
            isBlocked = false;

            if (newState == EnemyState.Wander)
            {
                GetNewWanderPosition();
                wanderTimer = 0;
                // 배회 시작 시 직진 타이머 초기화
                currentWalkStraightTimer = walkStraightTime;
            }
            else if (newState == EnemyState.SearchLastPos)
            {
                currentDestination = lastKnownPos;
            }
        }

        //정신력 이벤트
        private void UpdateMentalEffects()
        {
            if (playerHealth == null) return;

            bool looking = CheckLineOfSight();
            bool chasing = currentState == EnemyState.Chase;

            playerHealth.IsEnemyLooking = looking;
            playerHealth.IsBeingChased = chasing;
        }

        // --- 유니티 이벤트 ---

        // 플레이어가 범위 내에 있을 때 계속 감지 시도 (등 뒤에 있다가 앞으로 오는 경우 등 대응)
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                target = other.transform;

                if (playerHealth == null)
                    playerHealth = other.GetComponent<PlayerHealth>();

                if (currentState == EnemyState.Wander && CheckLineOfSight())
                {
                    lastKnownPos = target.position;
                    detectTrigger.enabled = false;
                    ChangeState(EnemyState.Chase);
                }
            }

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

        //이너미 상태 런타임에 저장
        public EnemyRuntimeState CaptureRuntimeState()
        {
            return new EnemyRuntimeState
            {
                state = currentState,
                position = transform.position,
                lastKnownPlayerPos = lastKnownPos
            };
        }

        public void RestoreRuntimeState(EnemyRuntimeState data)
        {
            transform.position = data.position;
            lastKnownPos = data.lastKnownPlayerPos;
            ChangeState(data.state);
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

            // 공격 범위 확인용 기즈모 (빨간색 구체)
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Vector3 hitPoint = transform.position + transform.TransformDirection(attackOffset);
            Gizmos.DrawWireSphere(hitPoint, attackImpactRadius);

            // 장애물 감지 레이 기즈모 (파란색)
            Gizmos.color = Color.blue;
            Vector3 rayOrigin = transform.position + Vector3.up * 1.0f;
            Gizmos.DrawRay(rayOrigin, transform.forward * obstacleCheckDistance);
        }
    }
}