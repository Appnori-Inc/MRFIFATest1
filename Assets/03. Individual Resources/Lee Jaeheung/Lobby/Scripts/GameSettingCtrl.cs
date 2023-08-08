using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using Photon.Voice.PUN;
using Photon.Voice.Unity.UtilityScripts;

public class GameSettingCtrl : MonoBehaviour
{
    [System.Serializable]
    public class JsonParsing
    {
        public List<LocalizationInfo> list_localizationInfo;

        public JsonParsing(string csvName)
        {
            string orizin_csv = File.ReadAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/" + csvName);
            string[] split_csv = orizin_csv.Split('\n');

            list_localizationInfo = new List<LocalizationInfo>();

            for (int i = 1; i < split_csv.Length; i++)
            {
                list_localizationInfo.Add((new LocalizationInfo(split_csv[i])));
            }
        }

        [System.Serializable]
        public class LocalizationInfo
        {
            public string id;
            public List<string> list_data;

            public LocalizationInfo(string csv)
            {
                string[] split_info = csv.Split(',');

                id = split_info[0];
                list_data = new List<string>();
                for (int i = 1; i < 14; i++)
                {
                    list_data.Add(split_info[i].Split('\r')[0]);
                }
            }
        }
    }

    private bool isInit = false;

    private static AudioMixer audioMixer;

    private Text text_sound;
    private Text text_bgm;
    private Text text_eff;
    private Slider slider_bgm;
    private Slider slider_eff;
    private Toggle toggle_bgm;
    private Toggle toggle_eff;

    private Text text_title_haptic;
    private Text text_info_haptic;
    private Slider slider_haptic;
    private Toggle toggle_haptic;

    private Text text_hand;
    private Text text_handL;
    private Text text_handR;
    private Toggle toggle_handL;
    private Toggle toggle_handR;

    private Text text_title_voice_chat;
    private Text text_info_voice_chat;
    private Slider slider_voice_chat;
    private Toggle toggle_voice_chat;

    private Text text_language;
    private Dropdown dropdown_language;

    private static bool isChangeData = false;

    public static LanguageState languageState;

    [System.Serializable]
    public class SettingInfo
    {
        public float value_bgm;
        public bool on_sound_bgm;
        public float value_eff;
        public bool on_sound_eff;

        public float value_haptic;
        public bool on_haptic;

        public bool on_right_handed;

        public float value_voice;
        public bool on_sound_voice;

        public string language;


        public SettingInfo()
        {
            value_bgm = 0.85f;
            on_sound_bgm = true;
            value_eff = 0.85f;
            on_sound_eff = true;

            value_haptic = 1f;
            on_haptic = true;

            on_right_handed = true;

            value_voice = 0.85f;
            on_sound_voice = true;

            language = "system";
        }
    }

    public static SettingInfo settingInfo;

    public static Dictionary<string, List<string>> localizationInfos;

    public static MicAmplifier micAmplifier;

    private static event Action<bool> action_hand;
    private static event Action action_localization;
    private static event Action<LanguageState> action_localization_put;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (isInit)
        {
            return;
        }

        isInit = true;

        audioMixer = Resources.Load<AudioMixer>("MasterAudioMixer");

        Transform findTr = transform.Find("Board/Scroll View/Viewport/Content/Slot (0)");

        text_sound = findTr.Find("Text_Title").GetComponent<Text>();
        text_bgm = findTr.Find("Text_BGM").GetComponent<Text>();
        text_eff = findTr.Find("Text_Effect").GetComponent<Text>();

        slider_bgm = findTr.Find("Slider_BGM").GetComponent<Slider>();
        slider_eff = findTr.Find("Slider_Effect").GetComponent<Slider>();
        toggle_bgm = findTr.Find("Toggle_BGM").GetComponent<Toggle>();
        toggle_eff = findTr.Find("Toggle_Effect").GetComponent<Toggle>();

        findTr = transform.Find("Board/Scroll View/Viewport/Content/Slot (1)");

        text_title_haptic = findTr.Find("Text_Title").GetComponent<Text>();
        text_info_haptic = findTr.Find("Text_Value").GetComponent<Text>();

        slider_haptic = findTr.Find("Slider_Haptic").GetComponent<Slider>();
        toggle_haptic = findTr.Find("Toggle_Haptic").GetComponent<Toggle>();

        findTr = transform.Find("Board/Scroll View/Viewport/Content/Slot (2)");

        text_hand = findTr.Find("Text_Title").GetComponent<Text>();
        text_handL = findTr.Find("Text_LeftHand").GetComponent<Text>();
        text_handR = findTr.Find("Text_RightHand").GetComponent<Text>();

        toggle_handL = findTr.Find("Toggles/Toggle_LeftHand").GetComponent<Toggle>();
        toggle_handR = findTr.Find("Toggles/Toggle_RightHand").GetComponent<Toggle>();

        findTr = transform.Find("Board/Scroll View/Viewport/Content/Slot (3)");

        text_title_voice_chat = findTr.Find("Text_Title").GetComponent<Text>();
        text_info_voice_chat = findTr.Find("Text_Value").GetComponent<Text>();

        slider_voice_chat = findTr.Find("Slider_Voice").GetComponent<Slider>();
        toggle_voice_chat = findTr.Find("Toggle_Voice").GetComponent<Toggle>();

        findTr = transform.Find("Board/Scroll View/Viewport/Content/Slot (4)");

        text_language = findTr.Find("Text_Title").GetComponent<Text>();

        dropdown_language = findTr.Find("Dropdown").GetComponent<Dropdown>();


        ////////////////////////////////////////////////

        toggle_bgm.onValueChanged.RemoveAllListeners();
        toggle_bgm.onValueChanged.AddListener(delegate { OnValueToggle(toggle_bgm); });
        slider_bgm.onValueChanged.RemoveAllListeners();
        slider_bgm.onValueChanged.AddListener(delegate { OnValueSlider(slider_bgm); });

        toggle_eff.onValueChanged.RemoveAllListeners();
        toggle_eff.onValueChanged.AddListener(delegate { OnValueToggle(toggle_eff); });
        slider_eff.onValueChanged.RemoveAllListeners();
        slider_eff.onValueChanged.AddListener(delegate { OnValueSlider(slider_eff); });

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(delegate { PublicGameUIManager.GetInstance.Play_Voice(7); });
        slider_eff.transform.GetComponent<EventTrigger>().triggers.Add(entry);


        toggle_haptic.onValueChanged.RemoveAllListeners();
        toggle_haptic.onValueChanged.AddListener(delegate { OnValueToggle(toggle_haptic); });
        slider_haptic.onValueChanged.RemoveAllListeners();
        slider_haptic.onValueChanged.AddListener(delegate { OnValueSlider(slider_haptic); });


        toggle_handL.onValueChanged.RemoveAllListeners();
        toggle_handL.onValueChanged.AddListener(delegate { OnValueToggle(toggle_handL); });
        toggle_handR.onValueChanged.RemoveAllListeners();
        toggle_handR.onValueChanged.AddListener(delegate { OnValueToggle(toggle_handR); });


        toggle_voice_chat.onValueChanged.RemoveAllListeners();
        toggle_voice_chat.onValueChanged.AddListener(delegate { OnValueToggle(toggle_voice_chat); });
        slider_voice_chat.onValueChanged.RemoveAllListeners();
        slider_voice_chat.onValueChanged.AddListener(delegate { OnValueSlider(slider_voice_chat); });

        List<Dropdown.OptionData> list_optionData = new List<Dropdown.OptionData>();

        for (int i = 0; i < 13; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("English");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 1:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("한국어");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 2:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("日本語");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 3:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("Español");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 4:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("Deutsch");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 5:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("Português");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 6:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("русский язык");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 7:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("Italiano");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 8:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("język polski");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 9:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("Türkçe");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 10:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("简体中文");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 11:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("繁體中文");
                        list_optionData.Add(optionData);
                    }
                    break;
                case 12:
                    {
                        Dropdown.OptionData optionData = new Dropdown.OptionData("Français");
                        list_optionData.Add(optionData);
                    }
                    break;
            }
        }

        dropdown_language.ClearOptions();
        dropdown_language.AddOptions(list_optionData);

        dropdown_language.onValueChanged.RemoveAllListeners();
        dropdown_language.onValueChanged.AddListener(delegate { OnValueDropdown(dropdown_language); });

        if (micAmplifier == null)
        {
            micAmplifier = PhotonVoiceNetwork.Instance.transform.GetComponent<MicAmplifier>();
        }

        InitSettingInfo();

        InitLocalizationInfo();

        languageState = GetChangeLanguageState(settingInfo.language);

        if (StaticLocalizationCtrl.IsInstance)
        {
            StaticLocalizationCtrl.GetInstance.SetLocaliztion();
        }
        if (StaticLocalizationCtrl_TextMesh.IsInstance)
        {
            StaticLocalizationCtrl_TextMesh.GetInstance.SetLocaliztion();
        }
        if (StaticResourcesChangeCtrl.IsInstance)
        {
            StaticResourcesChangeCtrl.GetInstance.SetData();
        }

        gameObject.SetActive(false);
    }

    private static void InitSettingInfo()
    {
        if (settingInfo == null)
        {
            if (File.Exists(Application.persistentDataPath + "/SettingData.json"))
            {
                try
                {
                    string jsonText = File.ReadAllText(Application.persistentDataPath + "/SettingData.json");
                    settingInfo = JsonUtility.FromJson<SettingInfo>(jsonText);
                }
                catch (System.Exception)
                {
                    settingInfo = new SettingInfo();
                }
            }
            else
            {
                settingInfo = new SettingInfo();
            }
        }
    }

    private static void InitLocalizationInfo()
    {
        if (localizationInfos == null)
        {
            localizationInfos = new Dictionary<string, List<string>>();

            TextAsset jsonText = Resources.Load("ItemInfos/LocalizationData") as TextAsset;
            List<JsonParsing.LocalizationInfo> list_localizations = JsonUtility.FromJson<JsonParsing>(jsonText.text).list_localizationInfo;
            for (int i = 0; i < list_localizations.Count; i++)
            {
                localizationInfos.Add(list_localizations[i].id, list_localizations[i].list_data);
            }

            languageState = GetChangeLanguageState(settingInfo.language);
        }
    }

    public void View()
    {
        Init();

        settingInfo.value_bgm = Mathf.Clamp01(settingInfo.value_bgm);
        settingInfo.value_eff = Mathf.Clamp01(settingInfo.value_eff);

        if (settingInfo.on_sound_bgm)
        {
            toggle_bgm.isOn = true;
            slider_bgm.value = settingInfo.value_bgm;
            audioMixer.SetFloat("BGM", LinearToDecibel(settingInfo.value_bgm));
        }
        else
        {
            toggle_bgm.isOn = false;
            slider_bgm.value = 0f;
            audioMixer.SetFloat("BGM", LinearToDecibel(0f));
        }

        if (settingInfo.on_sound_eff)
        {
            toggle_eff.isOn = true;
            slider_eff.value = settingInfo.value_eff;
            audioMixer.SetFloat("Effect", LinearToDecibel(settingInfo.value_eff));
        }
        else
        {
            toggle_eff.isOn = false;
            slider_eff.value = 0f;
            audioMixer.SetFloat("Effect", LinearToDecibel(0f));
        }


        settingInfo.value_haptic = Mathf.Clamp(settingInfo.value_haptic,0f,2f);

        if (settingInfo.on_haptic)
        {
            toggle_haptic.isOn = true;
            slider_haptic.value = settingInfo.value_haptic;
        }
        else
        {
            toggle_haptic.isOn = false;
            slider_haptic.value = 0f;
        }

        //UnityEngine.XR.Interaction.Toolkit.XRController.value_haptic = GetHapticValue();

        if (settingInfo.on_right_handed)
        {
            toggle_handR.isOn = true;
            toggle_handL.isOn = false;
        }
        else
        {
            toggle_handL.isOn = true;
            toggle_handR.isOn = false;
        }

        settingInfo.value_voice = Mathf.Clamp(settingInfo.value_voice, 0f, 1f);

        if (settingInfo.on_sound_voice)
        {
            toggle_voice_chat.isOn = true;
            slider_voice_chat.value = settingInfo.value_voice;
        }
        else
        {
            toggle_voice_chat.isOn = false;
            slider_voice_chat.value = 0f;
        }
        micAmplifier.AmplificationFactor = slider_voice_chat.value * 3f;

        languageState = GetChangeLanguageState(settingInfo.language);

        dropdown_language.value = (int)languageState;
        gameObject.SetActive(true);
    }

    public void OnValueSlider(Slider slider)
    {
        switch (slider.gameObject.name)
        {
            case "Slider_BGM":
                {
                    if (slider.value == 0f && !toggle_bgm.isOn)
                    {
                        return;
                    }

                    SetValueBGM(slider.value);
                }
                break;
            case "Slider_Effect":
                {
                    if (slider.value == 0f && !toggle_eff.isOn)
                    {
                        return;
                    }

                    SetValueEffect(slider.value);
                }
                break;
            case "Slider_Haptic":
                {
                    if (slider.value == 0f && !toggle_haptic.isOn)
                    {
                        return;
                    }

                    SetValueHaptic(slider.value);
                }
                break;
            case "Slider_Voice":
                {
                    if (slider.value == 0f && !toggle_voice_chat.isOn)
                    {
                        return;
                    }

                    SetValueVoiceChat(slider.value);
                }
                break;
        }
    }

    private float LinearToDecibel(float linear)
    {
        float dB;

        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
    }

    public void OnValueToggle(Toggle toggle)
    {
        //Debug.LogError("OnValueToggle");
        switch (toggle.gameObject.name)
        {
            case "Toggle_BGM":
                {
                    SetOnBGM(toggle.isOn);
                }
                break;
            case "Toggle_Effect":
                {
                    SetOnEffect(toggle.isOn);
                }
                break;
            case "Toggle_Haptic":
                {
                    SetOnHaptic(toggle.isOn);
                }
                break;
            case "Toggle_LeftHand":
                {
                    if (settingInfo.on_right_handed == !toggle.isOn)
                    {
                        return;
                    }

                    SetRightHanded(false);
                }
                break;
            case "Toggle_RightHand":
                {
                    if (settingInfo.on_right_handed == toggle.isOn)
                    {
                        return;
                    }

                    SetRightHanded(true);
                }
                break;
            case "Toggle_Voice":
                {
                    SetOnVoiceChat(toggle.isOn);
                }
                break;
        }
    }

    public void OnValueDropdown(Dropdown dropdown)
    {
        switch (dropdown.gameObject.name)
        {
            default:
                {
                    if ((int)languageState == dropdown.value)
                    {
                        return;
                    }

                    SetLanguageState(dropdown.value);
                }
                break;
        }
    }

    public static AudioMixerGroup GetAudioMixerGroup(string subPath)
    {
        if (audioMixer == null)
        {
            audioMixer = Resources.Load<AudioMixer>("MasterAudioMixer");
        }
        return audioMixer.FindMatchingGroups(subPath)[0];
    }

    public void SetOnBGM(bool isOn)
    {
        Init();

        if (settingInfo.on_sound_bgm == isOn)
        {
            return;
        }

        settingInfo.on_sound_bgm = isOn;
        if (settingInfo.on_sound_bgm)
        {
            slider_bgm.value = settingInfo.value_bgm;
            audioMixer.SetFloat("BGM", LinearToDecibel(settingInfo.value_bgm));
        }
        else
        {
            slider_bgm.value = 0f;
            audioMixer.SetFloat("BGM", LinearToDecibel(0f));
        }

        isChangeData = true;
        Debug.Log("SetOnBGM - " + isOn);
    }

    public void SetValueBGM(float value)
    {
        Init();

        if (settingInfo.value_bgm == value)
        {
            return;
        }

        value = Mathf.Clamp01(value);

        settingInfo.value_bgm = value;
        audioMixer.SetFloat("BGM", LinearToDecibel(settingInfo.value_bgm));

        settingInfo.on_sound_bgm = true;
        toggle_bgm.isOn = settingInfo.on_sound_bgm;

        isChangeData = true;
        Debug.Log("SetValueBGM - " + value);
    }

    public void SetOnEffect(bool isOn)
    {
        Init();

        if (settingInfo.on_sound_eff == isOn)
        {
            return;
        }

        settingInfo.on_sound_eff = isOn;
        if (settingInfo.on_sound_eff)
        {
            slider_eff.value = settingInfo.value_eff;
            audioMixer.SetFloat("Effect", LinearToDecibel(settingInfo.value_eff));
        }
        else
        {
            slider_eff.value = 0f;
            audioMixer.SetFloat("Effect", LinearToDecibel(0f));
        }

        isChangeData = true;
        Debug.Log("SetOnEffect - " + isOn);
    }

    public void SetValueEffect(float value)
    {
        Init();

        if (settingInfo.value_eff == value)
        {
            return;
        }

        value = Mathf.Clamp01(value);

        settingInfo.value_eff = value;
        audioMixer.SetFloat("Effect", LinearToDecibel(settingInfo.value_eff));

        settingInfo.on_sound_eff = true;
        toggle_eff.isOn = settingInfo.on_sound_eff;

        isChangeData = true;
        Debug.Log("SetValueEffect - " + value);
    }

    public void SetOnHaptic(bool isOn)
    {
        Init();

        if (settingInfo.on_haptic == isOn)
        {
            return;
        }

        settingInfo.on_haptic = isOn;
        if (settingInfo.on_haptic)
        {
            slider_haptic.value = settingInfo.value_haptic;
        }
        else
        {
            slider_haptic.value = 0f;
        }

        //UnityEngine.XR.Interaction.Toolkit.XRController.value_haptic = GetHapticValue();

        isChangeData = true;
        Debug.Log("SetOnHaptic - " + isOn);
    }

    public void SetValueHaptic(float value)
    {
        Init();

        if (settingInfo.value_haptic == value)
        {
            return;
        }

        settingInfo.value_haptic = value;

        settingInfo.on_haptic = true;
        toggle_haptic.isOn = settingInfo.on_haptic;

        //UnityEngine.XR.Interaction.Toolkit.XRController.value_haptic = GetHapticValue();
        PublicGameUIManager.GetInstance.SetHaptic(0.5f, 0.05f);

        isChangeData = true;
        Debug.Log("SetValueHaptic - " + value);
    }

    public float GetHapticValue()
    {
        Init();
        if (settingInfo.on_haptic)
        {
            return settingInfo.value_haptic;
        }
        else
        {
            return 0f;
        }
    }

    public void SetRightHanded(bool isRightHanded)
    {
        Init();

        settingInfo.on_right_handed = isRightHanded;

        if (action_hand != null)
        {
            action_hand.Invoke(settingInfo.on_right_handed);
        }

        isChangeData = true;
        Debug.Log("SetRightHanded - " + (settingInfo.on_right_handed ? "RightHanded" : "LeftHanded"));
    }

    public static bool IsRightHanded()
    {
        InitSettingInfo();

        return settingInfo.on_right_handed;
    }

    public static void AddHandChangedEvent(Action<bool> action)
    {
        //GameSettingCtrl.AddHandChangedEvent((b) => { Debug.Log("ClickEvent" + b); });
        //GameSettingCtrl.AddHandChangedEvent(TestEvent);
        action.Invoke(settingInfo.on_right_handed);
        action_hand += action;
    }

    public static void ClearHandChangedEvent()
    {
        action_hand = null;
    }

    public void SetOnVoiceChat(bool isOn)
    {
        Init();

        if (settingInfo.on_sound_voice == isOn)
        {
            return;
        }

        settingInfo.on_sound_voice = isOn;
        if (settingInfo.on_sound_voice)
        {
            slider_voice_chat.value = settingInfo.value_voice;
            audioMixer.SetFloat("VoiceChat", LinearToDecibel(settingInfo.value_voice));
        }
        else
        {
            slider_voice_chat.value = 0f;
            audioMixer.SetFloat("VoiceChat", LinearToDecibel(0f));
        }
        micAmplifier.AmplificationFactor = slider_voice_chat.value * 3f;

        isChangeData = true;
        Debug.Log("SetOnVoiceChat - " + isOn);
    }

    public void SetValueVoiceChat(float value)
    {
        Init();

        if (settingInfo.value_voice == value)
        {
            return;
        }

        settingInfo.value_voice = value;
        audioMixer.SetFloat("VoiceChat", LinearToDecibel(settingInfo.value_voice));
        micAmplifier.AmplificationFactor = settingInfo.value_voice * 3f;


        settingInfo.on_sound_voice = true;
        toggle_voice_chat.isOn = settingInfo.on_sound_voice;

        isChangeData = true;
        Debug.Log("SetValueVoiceChat - " + value);
    }

    public static void AddLocalizationChangedEvent(Action action)
    {
        //GameSettingCtrl.AddLocalizationChangedEvent(() => { Debug.Log("Change"); });
        //GameSettingCtrl.AddLocalizationChangedEvent(TestEvent);
        action.Invoke();
        action_localization += action;
    }

    public static void AddLocalizationChangedEvent(Action<LanguageState> action)
    {
        action.Invoke(languageState);
        action_localization_put += action;
    }

    public static void ClearLocalizationChangedEvent()
    {
        action_localization = null;
        action_localization_put = null;
    }

    public void SetLanguageState(int index)
    {
        Init();

        index = Mathf.Clamp(index, 0, Enum.GetNames(typeof(LanguageState)).Length - 1);

        languageState = (LanguageState)index;

        switch (languageState)
        {
            case LanguageState.english:
                {
                    settingInfo.language = "english";
                }
                break;
            case LanguageState.koreana:
                {
                    settingInfo.language = "koreana";
                }
                break;
            case LanguageState.japanese:
                {
                    settingInfo.language = "japanese";
                }
                break;
            case LanguageState.spanish:
                {
                    settingInfo.language = "spanish";
                }
                break;
            case LanguageState.german:
                {
                    settingInfo.language = "german";
                }
                break;
            case LanguageState.portuguese:
                {
                    settingInfo.language = "portuguese";
                }
                break;
            case LanguageState.russian:
                {
                    settingInfo.language = "russian";
                }
                break;
            case LanguageState.italian:
                {
                    settingInfo.language = "italian";
                }
                break;
            case LanguageState.polish:
                {
                    settingInfo.language = "polish";
                }
                break;
            case LanguageState.turkish:
                {
                    settingInfo.language = "turkish";
                }
                break;
            case LanguageState.schinese:
                {
                    settingInfo.language = "schinese";
                }
                break;
            case LanguageState.tchinese:
                {
                    settingInfo.language = "tchinese";
                }
                break;
            case LanguageState.french:
                {
                    settingInfo.language = "french";
                }
                break;
        }
        if (StaticLocalizationCtrl.IsInstance)
        {
            StaticLocalizationCtrl.GetInstance.SetLocaliztion();
        }
        if (StaticLocalizationCtrl_TextMesh.IsInstance)
        {
            StaticLocalizationCtrl_TextMesh.GetInstance.SetLocaliztion();
        }
        if (StaticResourcesChangeCtrl.IsInstance)
        {
            StaticResourcesChangeCtrl.GetInstance.SetData();
        }

        PublicGameUIManager.GetInstance.SetLocaliztion();

        if (LobbyUIManager.IsInstance)
        {
            LobbyUIManager.GetInstance.SetModeLocalization();
            GameDataManager.instance.StartGetAchieveList();
        }

        if (action_localization != null)
        {
            action_localization.Invoke();
        }

        if (action_localization_put != null)
        {
            action_localization_put.Invoke(languageState);
        }

        isChangeData = true;
        Debug.Log("SetLanguageState - " + languageState);
    }

    public static LanguageState GetLanguageState()
    {
        InitLocalizationInfo();

        return languageState;
    }

    private void OnDisable()
    {
        if (isChangeData)
        {
            isChangeData = false;
            SaveData();
        }
    }

    public void SaveData()
    {
        if (settingInfo != null)
        {
            //Debug.LogError("Save");
            File.WriteAllText(Application.persistentDataPath + "/SettingData.json", JsonUtility.ToJson(settingInfo));
        }
    }


    public void CSVToJson()
    {
        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_Localization.csv"))
        {
            string jsonString = JsonUtility.ToJson(new JsonParsing("Sheet_Localization.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/LocalizationData.json", jsonString);

            Debug.Log("Save JsonFile - LocalizationData");
        }
        else
        {
            Debug.LogError("Not find Sheet_Localization");
        }
    }

#if STEAMWORKS
    public LanguageState GetChangeLanguageState()
    {
        return GetChangeLanguageState(Steamworks.SteamUtils.GetSteamUILanguage());
    }
#endif

    public static LanguageState GetChangeLanguageState(string lang)
    {
        switch (lang)
        {
            case "english":
                {
                    return LanguageState.english;
                }
            case "koreana":
                {
                    return LanguageState.koreana;
                }
            case "japanese":
                {
                    return LanguageState.japanese;
                }
            case "spanish":
                {
                    return LanguageState.spanish;
                }
            case "german":
                {
                    return LanguageState.german;
                }
            case "portuguese":
                {
                    return LanguageState.portuguese;
                }
            case "russian":
                {
                    return LanguageState.russian;
                }
            case "italian":
                {
                    return LanguageState.italian;
                }
            case "polish":
                {
                    return LanguageState.polish;
                }
            case "turkish":
                {
                    return LanguageState.turkish;
                }
            case "schinese":
                {
                    return LanguageState.schinese;
                }
            case "tchinese":
                {
                    return LanguageState.tchinese;
                }
            case "french":
                {
                    return LanguageState.french;
                }
        }
#if SERVER_CHINA
        return LanguageState.schinese;
#else
        return LanguageState.english;
#endif
    }

    public static string GetLocalizationText(string id)
    {
        InitLocalizationInfo();

        try
        {
            return localizationInfos[id][(int)languageState];
        }
        catch (Exception)
        {
            return "None";
        }
    }

    public static string GetUILanguage()
    {
        InitSettingInfo();

        return settingInfo.language;
    }
}

public enum LanguageState
{
    english = 0, koreana = 1, japanese = 2, spanish = 3, german = 4, portuguese = 5, russian = 6, italian = 7, polish = 8, turkish = 9, schinese = 10, tchinese = 11, french = 12
}