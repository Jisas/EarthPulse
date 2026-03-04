using UltimateFramework.AI.BehaviourTree;
using UltimateFramework.StatisticsSystem;
using UltimateFramework.TempSyncSystem;
using UltimateFramework.Inputs;
using UltimateFramework.Utils;
using UnityEngine;

namespace UltimateFramework.AI.Task
{
    public class Attack : Node
    {
        private readonly AttackData _attackData;
        readonly TempoManager _tempoManager;
        readonly EntityActionInputs _inputs;
        readonly string _attackInput;

        public Attack(EntityActionInputs inputs, TempoManager tempoManager, AttackData data)
        {
            _tempoManager = tempoManager;
            _attackInput = data.inputName;
            _attackData = data;
            _inputs = inputs;
        }

        public override NodeState Evaluate()
        {
            Transform target = GetData("target") as Transform;
            var targetStats = target.GetComponent<StatisticsComponent>();

            if (target == null)
            {
                state = NodeState.Failure;
                return state;
            }

            if (targetStats != null && targetStats.FindStatistic("Stats.Health").CurrentValue <= 0)
            {
                state = NodeState.Failure;
                return state;
            }

            if (_tempoManager.IsBeat(2))
            {
                InputActionLogic attackInputAction = _inputs.FindInputAction(_attackInput);
                ActionsPriority actionPriority = attackInputAction.PrimaryAction.priority;
                string actionTag = attackInputAction.PrimaryAction.actionTag.tag;
                bool isBaseAction = attackInputAction.PrimaryAction.isBaseAction;
                attackInputAction.ExecuteAction(actionTag, actionPriority, isBaseAction);

                parent.parent.parent.SetData("initialBeatCount", _tempoManager.BeatCount);
                parent.parent.parent.SetData("beatsToWait", _attackData.beatsCooldown);

                state = NodeState.Success;
                return state;
            }

            state = NodeState.Running;
            return state;
        }
    }
}