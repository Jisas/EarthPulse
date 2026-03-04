using UnityEngine.InputSystem;
using UnityEngine;
using MyBox;

namespace UltimateFramework.TempoSyncSystem
{
    public class InputPracticeListener : MonoBehaviour
    {
        public TempoVerifier TempoVerifier;
        [ReadOnly] public InputActionReference inputRef;
        public bool Verifier() => inputRef.action.WasPressedThisFrame();
    }
}