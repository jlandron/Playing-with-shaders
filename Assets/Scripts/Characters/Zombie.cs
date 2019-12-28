using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Shooter.Characters
{
    [RequireComponent(typeof(FieldOfView))]
    [RequireComponent(typeof(NavMeshAgent))]

    public class Zombie : LivingEntity
    {
        public enum State
        {
            Wander,
            Chasing,
            Attacking
        }
        LivingEntity targetEntity;
        FieldOfView fov;
        NavMeshAgent navMeshAgent;
        [Header("Movement properties")]
        [SerializeField] float wanderSpeed = 1f;
        [SerializeField] float chaseSpeed = 2f;
        [SerializeField] float chaseDistence = 10f;
        [SerializeField] float wanderRadius = 10f;
        [SerializeField] float wanderTimerMax = 10f;
        [SerializeField] float wanderTimerMin = 1f;
        [SerializeField] float wanderTimer;

        [Header("Attack Properties")]
        [SerializeField] float attackDistanceThreashold = 1.5f;
        [SerializeField] float timeBetweenAttacks = 1f;
        float nextAttackTime;
        [SerializeField] float attackSpeed = 3;
        float myCollisionRadius;
        float targetCollisionRadius;
        [SerializeField] float attackPower = 2;

        [Header("State Machine")]
        [SerializeField] State currentState = State.Wander;

        private float _timer;

        Transform target;
        protected override void Start()
        {
            base.Start();
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        }
        void OnEnable()
        {
            fov = GetComponent<FieldOfView>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            wanderTimer = UnityEngine.Random.Range(wanderTimerMin, wanderTimerMax);
            _timer = wanderTimer;
            navMeshAgent.speed = wanderSpeed;
        }

        // Update is called once per frame
        protected override void Update()
        {
            CheckAttack();
            if (currentState != State.Attacking)
            {
                base.Update();
                AssignTargetThroughTakeDamage();
                if (target == null)
                {
                    AssignTargetOrWander();
                }
                else
                {
                    Chase();
                }
            }
            _timer += Time.deltaTime;
        }

        private void CheckAttack()
        {
            if (target != null && Time.time > nextAttackTime)
            {
                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreashold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
        IEnumerator Attack()
        {
            currentState = State.Attacking;
            navMeshAgent.enabled = false;
            if (target == null)
            {
                navMeshAgent.enabled = true;
                currentState = State.Wander;
                yield break;
            }
            Vector3 originalPosition = transform.position;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 attackPosition = target.position - directionToTarget * (myCollisionRadius);

            float percent = 0f;
            bool hasDamagedTarget = false;
            while (percent <= 1)
            {
                if (percent >= 0.5f && !hasDamagedTarget)
                {
                    print("Hitting player!");
                    hasDamagedTarget = true;
                    if (target != null)
                    {
                        target.GetComponent<LivingEntity>().TakeDamage(attackPower);
                    }
                }
                percent += Time.deltaTime * attackSpeed;
                float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
                transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
                yield return null;
            }

            navMeshAgent.enabled = true;
            currentState = State.Chasing;
        }
        private void AssignTargetThroughTakeDamage()
        {
            if (tookDamage && target == null)
            {
                currentState = State.Chasing;
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
                if (target != null)
                {
                    targetEntity = target.GetComponent<LivingEntity>();
                    targetEntity.OnDeath += OnTargetDeath;
                    targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
                    navMeshAgent.enabled = true;
                    navMeshAgent.SetDestination(target.position);
                    navMeshAgent.speed = chaseSpeed;
                }
            }
        }
        void OnTargetDeath()
        {
            target = null;
            currentState = State.Wander;
        }
        private void Chase()
        {
            if (!tookDamage && Vector3.Distance(target.position, gameObject.transform.position) > chaseDistence)
            {
                target = null;
                currentState = State.Wander;
                navMeshAgent.speed = wanderSpeed;
            }
            else
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (myCollisionRadius + targetCollisionRadius + (attackDistanceThreashold / 2));
                if (!dead)
                {
                    navMeshAgent.enabled = true;
                    navMeshAgent.SetDestination(targetPosition);
                }
            }
        }

        private void AssignTargetOrWander()
        {
            if (fov.visibleTargets.Count > 0)
            {
                currentState = State.Chasing;
                target = fov.visibleTargets[0];
                if (target != null)
                {
                    targetEntity = target.GetComponent<LivingEntity>();
                    targetEntity.OnDeath += OnTargetDeath;
                    targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
                    if (target != null)
                    {
                        navMeshAgent.enabled = true;
                        navMeshAgent.SetDestination(target.position);
                    }
                }
                navMeshAgent.speed = chaseSpeed;
            }
            else
            {
                currentState = State.Wander;
                Wander();
            }
        }

        private void Wander()
        {
            if (_timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(newPos);
                _timer = 0;
            }
        }

        private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
            randDirection += origin;
            NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
            return navHit.position;
        }
    }
}
