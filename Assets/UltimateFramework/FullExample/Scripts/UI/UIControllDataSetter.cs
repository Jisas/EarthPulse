using UltimateFramework.SerializationSystem;
using UnityEngine.Rendering.Universal;
using UltimateFramework.SoundSystem;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using MyBox;

#region Enums
public enum UIControlType
{
    Slider,
    Selector
}
public enum SliderSettingsOptions
{
    RenderScale = 0,
    Brightness = 1,
    GeneralVolume = 2,
    MusicVolume = 3,
    AmbientalVolume = 4,
    EffectsVolume = 5,
    UIEffectsVolume = 6,
}
public enum SelectorSettingsOptions
{
    OverAllQuality = 0,
    ScreenResolution = 1,
    ScreenMode = 2,
    TextureResolution = 3,
    ShadowQuality = 4,
    ShadowResolution  = 5,
    FrameRate = 6,
    Antialiasing = 7,
}
public enum AntialiasingOptions
{
    Yes = 0,
    No = 1,
}
public enum FameRateOptions
{
    Rate30 = 0,
    Rate60 = 1,
    Rate120 = 2
}
#endregion

#region Other Class
[Serializable]
public class SettingsSelector
{
    public TMP_Dropdown dropdown;

    public SettingsSelector (TMP_Dropdown dropdown)
    {
        this.dropdown = dropdown;
    }
}

[Serializable]
public class SettingsSlider
{
    public Slider slider;
    public TextMeshProUGUI text;

    public SettingsSlider (Slider slider, TextMeshProUGUI text)
    {
        this.slider = slider;
        this.text = text;
    }
}
#endregion

public class UIControllDataSetter : MonoBehaviour
{
    [SerializeField] private UIControlType controlType;

    [ConditionalField("controlType", false, UIControlType.Slider), SerializeField]
    private SliderSettingsOptions sliderSettings;

    [ConditionalField("controlType", false, UIControlType.Selector), SerializeField]
    private SelectorSettingsOptions selectorSettings;

    [Space(5)]

    [ConditionalField("controlType", false, UIControlType.Selector), SerializeField]
    private SettingsSelector selector;

    [ConditionalField("controlType", false, UIControlType.Slider), SerializeField]
    private SettingsSlider slider;

    private DataGameManager m_GameData;
    private SoundManager m_SoundManager;

    void Start()
    {
        m_GameData = DataGameManager.Instance;
        m_SoundManager = SoundManager.Instance;

        switch (controlType)
        {
            case UIControlType.Slider:
                SetupSlider();
                break;

            case UIControlType.Selector:
                SetupSelector();
                break;
        }
    }
    void Update()
    {
        switch (controlType)
        {
            case UIControlType.Slider:
                SetSettingsBySlider(slider.slider.value);
                break;

            case UIControlType.Selector:
                SetSettingsBySelector();
                break;
        }

        m_GameData.SaveSettingsData();
    }

    private void SetupSlider()
    {
        slider.slider.minValue = 0.0f;
        slider.slider.maxValue = 100.0f;

        switch (sliderSettings)
        {
            case SliderSettingsOptions.RenderScale:
                slider.slider.maxValue = 200.0f;
                slider.slider.value = m_GameData.GetSettingsData().RenderScale * 100;
                break;

            case SliderSettingsOptions.Brightness:
                slider.slider.value = m_GameData.GetSettingsData().Brightness * 100;
                break;

            case SliderSettingsOptions.GeneralVolume:
                slider.slider.value = m_GameData.GetSettingsData().GeneralVolume * 100;
                break;

            case SliderSettingsOptions.MusicVolume:
                slider.slider.value = m_GameData.GetSettingsData().MusicVolume * 100;
                break;

            case SliderSettingsOptions.AmbientalVolume:
                slider.slider.value = m_GameData.GetSettingsData().AmbientalVolume * 100;
                break;

            case SliderSettingsOptions.EffectsVolume:
                slider.slider.value = m_GameData.GetSettingsData().EffectsVolume * 100;
                break;

            case SliderSettingsOptions.UIEffectsVolume:
                slider.slider.value = m_GameData.GetSettingsData().UIVolume * 100;
                break;
        }
    }
    private void SetSettingsBySlider(float value)
    {
        slider.text.text = $"{Math.Round(value, 0)}%";
        float realValue = value / 100;

        switch (sliderSettings)
        {
            case SliderSettingsOptions.RenderScale:
                UniversalRenderPipelineAsset urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
                m_GameData.SetRenderScale(realValue);
                urpAsset.renderScale = realValue;
                break;

            case SliderSettingsOptions.Brightness:
                m_GameData.SetBrightness(realValue);
                Screen.brightness = value;
                break;

            case SliderSettingsOptions.GeneralVolume:
                m_GameData.SetGeneralVolume(realValue);
                m_SoundManager.SetGeneralVolume();
                break;

            case SliderSettingsOptions.MusicVolume:
                m_GameData.SetMusicVolume(realValue);
                m_SoundManager.SetMusicVolume();
                break;

            case SliderSettingsOptions.EffectsVolume:
                m_GameData.SetEffectsVolume(realValue);
                m_SoundManager.SetEffectsVolume();
                break;

            case SliderSettingsOptions.UIEffectsVolume:
                m_GameData.SetUIVolume(realValue);
                m_SoundManager.SetUIEffectsVolume();
                break;
        }
    }

    private void SetupSelector()
    {
        List<TMP_Dropdown.OptionData> optionsData = new();

        switch (selectorSettings)
        {
            case SelectorSettingsOptions.OverAllQuality:

                for (int i = 0; i < QualitySettings.count; i++)
                {
                    var option = new TMP_Dropdown.OptionData(QualitySettings.names[i]);
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = m_GameData.GetSettingsData().Quality;
                break;

            case SelectorSettingsOptions.ScreenResolution:
                Resolution[] resolutions = Screen.resolutions;
                foreach (var res in resolutions)
                {
                    var option = new TMP_Dropdown.OptionData($"{res.width} x {res.height}");
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                Resolution currentResolution = Screen.currentResolution;
                int currentResolutionIndex = 0;

                foreach (var res in resolutions)
                {
                    if (res.width == currentResolution.width && res.height == currentResolution.height)
                        currentResolutionIndex = resolutions.IndexOfItem(res);
                }

                selector.dropdown.value = currentResolutionIndex;
                break;

            case SelectorSettingsOptions.ScreenMode:
                string[] screenModes = { "Full Screen", "Borderless Window", "Window" };
                foreach (var mode in screenModes)
                {
                    var option = new TMP_Dropdown.OptionData(mode);
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = m_GameData.GetSettingsData().ScreenMode;
                break;

            case SelectorSettingsOptions.TextureResolution:
                string[] textureResolutions = { "Full Resolution", "Half Resolution", "Quarter Resolution", "Eighth Resolution" };
                foreach (var resolution in textureResolutions)
                {
                    var option = new TMP_Dropdown.OptionData(resolution);
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = m_GameData.GetSettingsData().TextureResolution;
                break;

            case SelectorSettingsOptions.ShadowQuality:
                string[] shadowQualities = { "Deactivated", "Only Hard Shadows", "Hard and Soft Shadows" };
                foreach (var quality in shadowQualities)
                {
                    var option = new TMP_Dropdown.OptionData(quality);
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = m_GameData.GetSettingsData().ShadowQuality;
                break;

            case SelectorSettingsOptions.ShadowResolution:
                string[] shadowResolutions = { "Low", "Middle", "High", "Very High" };
                foreach (var resolution in shadowResolutions)
                {
                    var option = new TMP_Dropdown.OptionData(resolution);
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = m_GameData.GetSettingsData().ShadowResolution;
                break;

            case SelectorSettingsOptions.FrameRate:

                for (int i = 0; i < 3; i++)
                {
                    var temp = i == 0 ? 30 : i == 1 ? 60 : i == 2 ? 120 : 30;
                    var option = new TMP_Dropdown.OptionData($"{temp} FPS");
                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = m_GameData.GetSettingsData().FrameRate;
                break;

            case SelectorSettingsOptions.Antialiasing:

                for (int i = 0; i < 1; i++)
                {
                    var option = new TMP_Dropdown.OptionData(
                        System.Enum.GetName(typeof(AntialiasingOptions), i));

                    optionsData.Add(option);
                }

                selector.dropdown.ClearOptions();
                selector.dropdown.options = new(optionsData);
                selector.dropdown.value = m_GameData.GetSettingsData().AntiAliasing == true ? 0 : 1;
                break;
        }
    }
    private void SetSettingsBySelector()
    {
        switch (selectorSettings)
        {
            case SelectorSettingsOptions.OverAllQuality:
                m_GameData.SetQuality(selector.dropdown.value);
                QualitySettings.SetQualityLevel(selector.dropdown.value);
                break;

            case SelectorSettingsOptions.ScreenResolution:
                Resolution[] resolutions = Screen.resolutions;
                UnityEngine.Resolution selectedResolution = resolutions[selector.dropdown.value];
                m_GameData.SetScreenResolution(selector.dropdown.value);
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);
                break;

            case SelectorSettingsOptions.ScreenMode:
                UnityEngine.FullScreenMode selectedMode = FullScreenMode.FullScreenWindow;
                switch (selector.dropdown.value)
                {
                    case 0:
                        selectedMode = FullScreenMode.ExclusiveFullScreen;
                        break;
                    case 1:
                        selectedMode = FullScreenMode.FullScreenWindow;
                        break;
                    case 2:
                        selectedMode = FullScreenMode.Windowed;
                        break;
                }
                m_GameData.SetScreenMode(selector.dropdown.value);
                Screen.fullScreenMode = selectedMode;
                break;

            case SelectorSettingsOptions.TextureResolution:
                m_GameData.SetTextureResolution(selector.dropdown.value);
                QualitySettings.globalTextureMipmapLimit = selector.dropdown.value;
                break;

            case SelectorSettingsOptions.ShadowQuality:
                UnityEngine.ShadowQuality selectedShadowQuality = UnityEngine.ShadowQuality.Disable;
                switch (selector.dropdown.value)
                {
                    case 0:
                        selectedShadowQuality = UnityEngine.ShadowQuality.Disable;
                        break;
                    case 1:
                        selectedShadowQuality = UnityEngine.ShadowQuality.HardOnly;
                        break;
                    case 2:
                        selectedShadowQuality = UnityEngine.ShadowQuality.All;
                        break;
                }
                m_GameData.SetShadowQuality(selector.dropdown.value);
                QualitySettings.shadows = selectedShadowQuality;
                break;

            case SelectorSettingsOptions.ShadowResolution:
                UnityEngine.ShadowResolution selectedShadowResolution = UnityEngine.ShadowResolution.Low;
                switch (selector.dropdown.value)
                {
                    case 0:
                        selectedShadowResolution = UnityEngine.ShadowResolution.Low;
                        break;
                    case 1:
                        selectedShadowResolution = UnityEngine.ShadowResolution.Medium;
                        break;
                    case 2:
                        selectedShadowResolution = UnityEngine.ShadowResolution.High;
                        break;
                    case 3:
                        selectedShadowResolution = UnityEngine.ShadowResolution.VeryHigh;
                        break;
                }
                m_GameData.SetShadowResolution(selector.dropdown.value);
                QualitySettings.shadowResolution = selectedShadowResolution;
                break;

            case SelectorSettingsOptions.FrameRate:
                m_GameData.SetFrameRate(selector.dropdown.value);

                int value = 
                    selector.dropdown.value == 0 ? 30 : 
                    selector.dropdown.value == 1 ? 60 : 
                    selector.dropdown.value == 2 ? 120 : 30;

                Application.targetFrameRate = value;
                break;

            case SelectorSettingsOptions.Antialiasing:
                m_GameData.SetAntiAliasing(selector.dropdown.value == 0);
                QualitySettings.antiAliasing = selector.dropdown.value == 0 ? 0 : 2;
                break;
        }
    }
}