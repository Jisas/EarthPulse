using UltimateFramework.AI.BehaviourTree;
using UltimateFramework.LocomotionSystem;
using UltimateFramework.TempSyncSystem;
using System.Collections.Generic;
using UltimateFramework.AI.Task;
using UltimateFramework.Inputs;
using UnityEditor;
using UnityEngine;
using System;
using UltimateFramework.StatisticsSystem;

[Serializable]
public struct AttackData
{
    public string inputName;
    [Range(0, 100)] public float minRange;
    [Range(0, 100)] public float maxRange;
    public int beatsCooldown;
    public Color rangeColor;
}

[RequireComponent(typeof(AILocomotionCommponent))]
public class EnemyTree : BehaviourTree
{
    [Header("Detection")]
    [SerializeField] LayerMask enemiesLayer;
    [SerializeField] float visionRadius = 15.0f;
    [SerializeField] float stopDistance = 2.0f;

    [Header("Attack")]
    [SerializeField] AttackData[] attacksData;

    StatisticsComponent m_StatsAndAttributes;
    AILocomotionCommponent m_Locomotion;
    EntityActionInputs m_InputManager;
    TempoManager m_TempoManager;

    private void Awake()
    {
        m_StatsAndAttributes = GetComponent<StatisticsComponent>();
        m_InputManager = GetComponent<EntityActionInputs>();
        m_Locomotion = GetComponent<AILocomotionCommponent>();
        m_TempoManager = GetComponent<TempoManager>();
    }

    protected override Node SetupTree()
    {
        List<Node> attackNodes = new();

        foreach (var attack in attacksData)
        {
            attackNodes.Add(new Sequence(new List<Node>
            {
                new CheckEnemyInDobleAttackRange(transform, attack),
                new Attack(m_InputManager, m_TempoManager, attack),
            }));
        }

        Node root = new Sequence(new List<Node>
        {
            new DeadCheck(m_StatsAndAttributes, m_Locomotion),
            new CheckEnemyInFOVRange(transform, visionRadius, enemiesLayer, m_Locomotion),
            new LookAtTarget(transform, m_Locomotion),
            new ChaseTarget(transform, m_Locomotion, visionRadius, stopDistance),
            new DynamicSelector(attackNodes),
            new WaitForBeat(m_TempoManager)
        });

        return root;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.up, visionRadius, 3);

        Handles.color = Color.black;
        Handles.DrawWireDisc(transform.position, Vector3.up, stopDistance, 3);

        foreach (var attackData in attacksData)
        {
            Handles.color = attackData.rangeColor;
            Handles.DrawWireDisc(transform.position, Vector3.up, attackData.minRange, 3);
            Handles.DrawWireDisc(transform.position, Vector3.up, attackData.maxRange, 3);
        }
    }
#endif
}
