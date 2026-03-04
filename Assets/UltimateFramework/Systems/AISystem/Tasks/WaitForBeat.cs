using UltimateFramework.AI.BehaviourTree;
using UltimateFramework.TempSyncSystem;
using UltimateFramework.Utils;
using UnityEngine;

namespace UltimateFramework.AI.Task
{
    public class WaitForBeat : Node
    {
        private readonly TempoManager _tempoManager;
        private int? _beatsToWait;

        public WaitForBeat(TempoManager tempoManager)
        {
            _tempoManager = tempoManager;
        }

        public override NodeState Evaluate()
        {
            _beatsToWait = GetData<int>("beatsToWait");
            int? _initialBeatCount = GetData<int>("initialBeatCount");

            if (!_initialBeatCount.HasValue || !_beatsToWait.HasValue)
            {
                state = NodeState.Failure;
                return state;
            }

            if (_tempoManager.BeatCount >= _initialBeatCount + _beatsToWait)
            {
                _tempoManager.ResetBeatExecuted();
                state = NodeState.Success;
                return state;
            }

            state = NodeState.Running;
            return state;
        }
    }
}
