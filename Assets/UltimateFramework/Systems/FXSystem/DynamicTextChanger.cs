using UnityEngine;
using CartoonFX;
using System;

[Serializable]
public struct VFXPresset
{
    public string text;
    public float size;
    public ParticleSystem particles; 
}

namespace Ultimateframework.FXSystem
{
    public class DynamicTextChanger : MonoBehaviour
    {
        [Space] public VFXPresset[] vfxPressets;

        public void SetRandomText()
        {
            var randomNum = UnityEngine.Random.Range(0, vfxPressets.Length);
            var presset = vfxPressets[randomNum];
            var dynamicParticleText = presset.particles.gameObject.GetComponent<CFXR_ParticleText>();
            dynamicParticleText.UpdateText(presset.text, presset.size);
            presset.particles.gameObject.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            presset.particles.Play(true);
        }
    }
}
