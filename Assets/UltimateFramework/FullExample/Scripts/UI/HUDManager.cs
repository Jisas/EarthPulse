using UltimateFramework.CollisionsAndDamageSystem;
using UltimateFramework.StatisticsSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;
using MyBox;

public class HUDManager : MonoBehaviour
{
    #region PublicValues
    [Header("General")]
    public GameObject HUDCanvas;

    [Header("Health")]
    public GameObject barCanvas;
    public TagSelector healthTag;
    public Slider healthBarSlider;
    public Image healthSubBar;

    [Header("Stamina")]
    public bool useStamina = true;
    [ConditionalField(nameof(useStamina), false, true)] public TagSelector staminaTag;
    [ConditionalField(nameof(useStamina), false, true)] public Slider staminaBarSlider;
    [ConditionalField(nameof(useStamina), false, true)] public Image staminaSubBar;

    [Header("Visuals")]
    public bool useExtras = true;
    [ConditionalField(nameof(useExtras), false, true)] public Transform marker;
    [Range(0, 0.5f)] public float timeToWaitForEffect = 0.02f;
    [Range(0f, 0.5f)] public float speedOfEffect = 0.1f;
    #endregion

    #region PrivateValues
    private Statistic healthStat;
    private Statistic staminaStat;
    private StatisticsComponent characterStats;
    private CharacterDamageHandler characterDamageHandler;
    #endregion

    #region Mono
    private void Awake()
    {
        characterStats = transform.root.GetComponent<StatisticsComponent>();
        characterDamageHandler = transform.root.GetComponent<CharacterDamageHandler>();
    }
    private void OnEnable()
    {
        characterDamageHandler.OnUITakeDamage += ShakeHealthBar;
    }
    private void Start() => StartCoroutine(DelayedStart());
    private IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        healthStat = characterStats.FindStatistic(healthTag.tag);
        SetBar(healthBarSlider, healthSubBar, healthStat);

        if (useStamina)
        {
            staminaStat = characterStats.FindStatistic(staminaTag.tag);
            SetBar(staminaBarSlider, staminaSubBar, staminaStat);
        }
    }
    void Update()
    {
        if (!HUDCanvas.activeInHierarchy) return;
        if (healthStat != null) StartCoroutine(UpdateBar(healthBarSlider, healthSubBar, healthStat));
        if (useStamina && staminaStat != null) StartCoroutine(UpdateBar(staminaBarSlider, staminaSubBar, staminaStat));
    }
    private void OnDisable()
    {
        characterDamageHandler.OnUITakeDamage -= ShakeHealthBar;
    }
    #endregion

    #region Internal
    private void SetBar(Slider bar, Image subBar, Statistic stat)
    {
        bar.minValue = 0;
        bar.maxValue = stat.startMaxValue;
        subBar.fillAmount = 1;
    }
    private IEnumerator UpdateBar(Slider bar, Image subBar, Statistic stat)
    {
        bar.maxValue = stat.CurrentMaxValue;
        bar.value = stat.CurrentValue;
        yield return new WaitForEndOfFrame();

        float normalizedValue = (bar.value - bar.minValue) / (bar.maxValue - bar.minValue);

        // Espera un momento antes de empezar a actualizar subBar
        yield return new WaitForSeconds(timeToWaitForEffect);

        while (Mathf.Abs(subBar.fillAmount - normalizedValue) > 0.02f)
        {
            subBar.fillAmount = Mathf.MoveTowards(subBar.fillAmount, normalizedValue, speedOfEffect * Time.deltaTime);
            yield return null;
        }

        // Asegúrate de que subBar.fillAmount sea exactamente igual a normalizedValue al final
        subBar.fillAmount = normalizedValue;
    }
    #endregion

    #region Callbacks
    private void ShakeHealthBar()
    {
        barCanvas.transform.DOShakePosition(.2f, new Vector3(.6f, 0, 0), 20, randomnessMode: ShakeRandomnessMode.Full);
    }
    #endregion

    #region PublicMethods
    public IEnumerator UpdateMarker()
    {
        var time = 0f;
        marker.transform.DOScale(1.5f, 0f);

        while (time < 0.25f)
        {
            time += Time.deltaTime;
            marker.transform.DOScale(1f, 0.25f);
            yield return null;
        }
    }
    #endregion
}
