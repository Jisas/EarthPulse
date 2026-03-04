using UnityEngine;

namespace UltimateFramework.AnimatorDataSystem
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorDataHandler : UFBaseComponent
    {
        [SerializeField] private AnimatorData animatorData;

        public AnimatorData GetData() => animatorData;
    }
}
