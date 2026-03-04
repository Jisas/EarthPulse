using UltimateFramework.CollisionsAndDamageSystem;
using UltimateFramework.InventorySystem;
using System.Collections.Generic;
using UltimateFramework.UISystem;
using Ultimateframework.FXSystem;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using TMPro;

namespace UltimateFramework.TempSyncSystem
{
    public class InputPatternChecker : MonoBehaviour
    {
        public ComboVisualsManager visualsManager;
        [SerializeField] private DynamicTextChanger dynamicText;
        [SerializeField] private TextMeshProUGUI buffAmountText;
        [Space] public List<InputPattern> inputPatterns;
        [Space, SerializeField] private bool enableDebug;

        #region PrivateValues
        private bool collisionDetected;
        private readonly List<InputActionReference> recordedInputs = new();
        private readonly Stack<float> recordedBuffs = new();
        private readonly Stack<float> recordedPercentage = new();

        // Refereneces
        private TempoManager tempoManager;
        private WeaponDamageHandler weaponDamageHandler;
        private InventoryAndEquipmentComponent equipmentComponent;
        private int totalPercentage = 0;
        #endregion

        #region Mono
        private void Awake()
        {
            tempoManager = GetComponent<TempoManager>();
            equipmentComponent = GetComponent<InventoryAndEquipmentComponent>();
        }
        private void Start()
        {
            if (equipmentComponent.GetCurrentRightWeaponObject().TryGetComponent<WeaponDamageHandler>(out weaponDamageHandler))
                weaponDamageHandler.OnCollisionDetected += OnCollisionDetected;
        }
        private void OnEnable()
        {
            tempoManager.OnVerifyByTempo += StartChecking;
            tempoManager.OnVerifyByCompas += CheckPatterns;
        }
        private void OnDisable()
        {
            tempoManager.OnVerifyByTempo -= StartChecking;
            tempoManager.OnVerifyByCompas -= CheckPatterns;
            weaponDamageHandler.OnCollisionDetected -= OnCollisionDetected;
        }
        #endregion

        #region Internal
        private void StartChecking()
        {
            StartCoroutine(CheckInputCoroutine());
        }
        private void CheckPatterns()
        {
            bool patternMatched = false;
            if (enableDebug) Debug.Log("Check Patterns");

            foreach (var pattern in inputPatterns)
            {
                if (CheckPattern(pattern))
                {
                    if (enableDebug) Debug.Log("Check Success");
                    ExecutePatternBuff(pattern);
                    recordedInputs.Clear();
                    patternMatched = true;
                    break;
                }
            }

            if (!patternMatched && recordedInputs.Count > 0)
            {
                if (enableDebug) Debug.Log("Check Failure, Reset");
                recordedInputs.Clear();
                ResetPatternBuff(); 
            }
        }
        private void OnCollisionDetected()
        {
            collisionDetected = true;
        }
        private IEnumerator CheckInputCoroutine()
        {
            float endTime = Time.time + 0.5f;

            while (Time.time < endTime)
            {
                foreach (var inputAction in GetAllInputActions())
                {
                    if (inputAction.input.action.WasPressedThisFrame())
                    {
                        if (inputAction.requiresCollision)
                        {
                            if (collisionDetected)
                            {
                                if (tempoManager.CurrentBeatOnAction == 0)
                                {
                                    tempoManager.CurrentBeatOnAction = tempoManager.BeatCount;
                                    if (enableDebug) Debug.Log(tempoManager.CurrentBeatOnAction);
                                }

                                recordedInputs.Add(inputAction.input);
                                collisionDetected = false;
                                break;
                            }
                            else
                            {
                                if (enableDebug) Debug.Log("Action Failure, Reset");
                                tempoManager.CurrentBeatOnAction = 0;
                                recordedInputs.Clear();
                                ResetPatternBuff();
                            }
                        }
                        else recordedInputs.Add(inputAction.input);
                        break;
                    }
                }
                yield return null;
            }
        }
        private IEnumerable<InputStruct> GetAllInputActions()
        {
            foreach (var pattern in inputPatterns)
            {
                foreach (var inputAction in pattern.pattern)
                {
                    yield return inputAction;
                }
            }
        }
        private bool CheckPattern(InputPattern pattern)
        {
            if (recordedInputs.Count != pattern.pattern.Count)
            {
                if (enableDebug) Debug.Log($"Total recorded inputs: {recordedInputs.Count} not match with pattern {pattern.name} count: {pattern.pattern.Count}");
                return false;
            }

            for (int i = 0; i < pattern.pattern.Count; i++)
            {
                if (recordedInputs[i] != pattern.pattern[i].input)
                {
                    if (enableDebug) Debug.Log($"Eecorded input: {recordedInputs[i].name} not match with pattern input: {pattern.pattern[i].input.name}");
                    return false;
                }
            }

            return true;
        }
        private void ExecutePatternBuff(InputPattern pattern)
        {
            string[] texts = pattern.name.Split('+');
            visualsManager.InstantiateMessage(texts[0].Trim(), texts.Length > 1 ? texts[1].Trim() : null);

            dynamicText.SetRandomText();
            totalPercentage += (int)pattern.scalingPercentage;
            buffAmountText.text = totalPercentage.ToString();
            recordedPercentage.Push(pattern.scalingPercentage);

            var equipedWeapon = equipmentComponent.GetCurrentMainWeapon();
            var weaponItem = equipedWeapon.WeaponComponent.WeaponBehaviour.Item;

            foreach (var stat in weaponItem.Stats)
            {
                if (stat.statTag.Contains("Damage"))
                {
                    var amount = SumOP(stat.CurrentValue, pattern.scalingPercentage);
                    stat.SetCurrentValue(stat.CurrentValue + amount);
                    recordedBuffs.Push(amount);
                }
            }
        }
        private float SumOP(float a, float b)
        {
            var finalValue = a / CalculatePercentajeConvertion(a, b);
            return finalValue;
        }
        private float CalculatePercentajeConvertion(float inicialValue, float percentaje)
        {
            var percentageValue = (inicialValue * percentaje) / 100;
            var ajustedValue = inicialValue + percentageValue;
            var percentageConvertion = inicialValue / ajustedValue;
            return percentageConvertion;
        }
        private void ResetPatternBuff()
        {
            var equipedWeapon = equipmentComponent.GetCurrentMainWeapon();
            var weaponItem = equipedWeapon.WeaponComponent.WeaponBehaviour.Item;

            if (recordedPercentage.Count > 0)
            {
                totalPercentage -= (int)recordedPercentage.Pop();
                if (totalPercentage <= 0) totalPercentage = 0;
                buffAmountText.text = totalPercentage.ToString();
            }

            foreach (var stat in weaponItem.Stats)
            {
                if (stat.statTag.Contains("Damage") && recordedBuffs.Count > 0)
                    stat.SetCurrentValue(stat.CurrentValue - recordedBuffs.Pop());
            }
        }
        #endregion
    }
}
