using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(FieldOfView))]
public class Zombie : MonoBehaviour
{
    FieldOfView fov;
    NavMeshAgent navMeshAgent;
    [SerializeField]
    float wanderSpeed = 1f;
    [SerializeField]
    float chaseSpeed = 2f;
    [SerializeField]
    float chaseDistence = 10f;
    [SerializeField]
    float wanderRadius = 10f;
    [SerializeField]
    float wanderTimerMax = 10f;
    [SerializeField]
    float wanderTimerMin = 1f;
    [SerializeField]
    float wanderTimer;
    [SerializeField]
    Transform wanderTarget;

    private float _timer;

    Transform target;
    void Start()
    {

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
    void Update()
    {

        if (target == null)
        {
            AssignTargetOrWander();
        }
        else
        {
            Chase();
        }
        _timer += Time.deltaTime;
    }

    private void Chase()
    {
        if (Vector3.Distance(target.position, gameObject.transform.position) > chaseDistence)
        {
            target = null;
            navMeshAgent.speed = wanderSpeed;
        }
        else
        {
            navMeshAgent.SetDestination(target.position);
            
        }
    }

    private void AssignTargetOrWander()
    {
        if (fov.visibleTargets.Count > 0)
        {
            target = fov.visibleTargets[0];
            navMeshAgent.SetDestination(target.position);
            navMeshAgent.speed = chaseSpeed;
        }
        else
        {
            Wander();
        }
    }

    private void Wander()
    {
        if (_timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
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
