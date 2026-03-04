using UltimateFramework.TempSyncSystem;
using UnityEngine.Events;
using System.Collections;
using UnityEngine;
using TMPro;

public class Counter : MonoBehaviour
{
    [SerializeField] private TempoManager tempoManager;
    [Space] public UnityEvent OnStartCount;
    [Space] public UnityEvent OnFinishCount;

    int count = 4;
    TextMeshProUGUI countText;

    private void Awake()
    {
        tempoManager.OnVerifyByTempo += CountSequence;
        countText = GetComponent<TextMeshProUGUI>();
    }
    private void OnDisable()
    {
        countText.text = "";
        tempoManager.OnVerifyByTempo -= CountSequence;
    }
    private void CountSequence()
    {
        if (count == 4) OnStartCount?.Invoke();

        count--;

        if (count <= 0)
        {
            countText.text = "GO";
            tempoManager.OnVerifyByTempo -= CountSequence;
            OnFinishCount.Invoke();
            StartCoroutine(Wait());
        }
        else countText.text = count.ToString();
    }
    private IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(.5f);
        gameObject.SetActive(false);
        StopAllCoroutines();
    }

    public void ResetAndStartCounter()
    {
        count = 4;
        tempoManager.OnVerifyByTempo += CountSequence;
        gameObject.SetActive(true);
    }
}
