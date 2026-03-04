using UltimateFramework.AI.BehaviourTree;
using UltimateFramework.Utils;
using UnityEngine;

namespace UltimateFramework.AI.Task
{
    public class CheckEnemyInDobleAttackRange : Node
    {
        readonly Transform _transform;
        readonly float _minRange;
        readonly float _maxRange;

        public CheckEnemyInDobleAttackRange(Transform transform, AttackData data)
        {
            _transform = transform;
            _minRange = data.minRange;
            _maxRange = data.maxRange;
        }

        public override NodeState Evaluate()
        {
            object t = GetData("target");

            if (t == null)
            {
                state = NodeState.Failure;
                return state;
            }

            Transform target = t as Transform;
            var distance = Vector3.Distance(_transform.position, target.position);

            if (distance < _maxRange && distance > _minRange)
            {
                state = NodeState.Success;
                return state;
            }

            state = NodeState.Failure;
            return state;
        }
    }
}
