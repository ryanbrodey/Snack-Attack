using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EggAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;                 // auto-find by tag if empty
    public Animator animator;                // Animator on THIS object (Egg root)
    [Tooltip("Pivot that should yaw with the model. Use Joint (preferred).")]
    public Transform model;                  // set this to the Joint object, not Eggy

    [Header("Animator Params (must match controller)")]
    public string speedParam = "Speed";      // optional (nice for debugging)
    public string inAttackParam = "InAttackRange";

    [Header("Behavior Distances (meters)")]
    public float chaseRange = 30f;
    public float attackRange = 1.6f;

    [Header("Rotation")]
    public float rotationSpeedDegPerSec = 720f; // how fast we turn
    [Tooltip("If the model looks sideways, try 90, -90, or 180.")]
    public float modelYawOffset = 0f;

    [Header("Smoothing")]
    public float speedDampTime = 0.15f;

    NavMeshAgent agent;
    int speedHash, inAttackHash;
    Quaternion modelBaseLocalRot;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        // Prefer Joint as the yaw pivot; fall back if missing
        if (model == null)
        {
            var j = transform.Find("Joint");
            if (j != null) model = j;
            else
            {
                // last resort - look for Eggy model
                var e = transform.Find("Eggy");
                if (e != null) model = e;
            }
        }

        if (model != null) modelBaseLocalRot = model.localRotation;

        speedHash = Animator.StringToHash(speedParam);
        inAttackHash = Animator.StringToHash(inAttackParam);
    }

    void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        // We rotate the root ourselves for full control
        agent.updatePosition = true;
        agent.updateRotation = false;
        agent.stoppingDistance = Mathf.Max(attackRange * 0.9f, 0.1f);

        // Apply one-time yaw fix to the pivot above the bones (so animation won't overwrite it)
        if (model != null)
            model.localRotation = modelBaseLocalRot * Quaternion.Euler(0f, modelYawOffset, 0f);
    }

    void Update()
    {
        if (!player || animator == null) return;

        // Always chase; stoppingDistance controls how close
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);
        bool inRange = dist <= attackRange;
        bool aware = dist <= chaseRange;

        // Face movement direction; if nearly stopped, face the player
        Vector3 dir = agent.desiredVelocity;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f)
        {
            dir = (player.position - transform.position);
            dir.y = 0f;
        }
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, look, rotationSpeedDegPerSec * Time.deltaTime);
        }

        // Drive the Animator (walk is default; attack gated by range)
        if (HasParam(speedHash, AnimatorControllerParameterType.Float))
            animator.SetFloat(speedHash, aware ? agent.velocity.magnitude : 0f, speedDampTime, Time.deltaTime);

        if (HasParam(inAttackHash, AnimatorControllerParameterType.Bool))
            animator.SetBool(inAttackHash, inRange);
    }

    bool HasParam(int hash, AnimatorControllerParameterType type)
    {
        foreach (var p in animator.parameters)
            if (p.type == type && p.nameHash == hash) return true;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
