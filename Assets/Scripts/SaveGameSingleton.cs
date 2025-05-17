using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

[System.Serializable]
public class SaveData
{
    public float mouseSensitivity = 2f;
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
}

public class SaveGameSingleton : MonoBehaviour
{
    private static SaveGameSingleton instance;
    public static SaveGameSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveGameSingleton>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("SaveGameManager");
                    instance = obj.AddComponent<SaveGameSingleton>();
                    DontDestroyOnLoad(obj);
                    instance.InitializeDefaultValues();
                }
            }
            return instance;
        }
    }

    public SaveData GameSettings { get; private set; } = new SaveData();
    public AudioMixer audioMixer; 
    public UnityEvent<SaveData> OnSettingsLoaded = new UnityEvent<SaveData>();

    private void InitializeDefaultValues()
    {
        string path = $"{Application.persistentDataPath}/GameSettings.json";
        if (File.Exists(path))
        {
            LoadSettings();
        }
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(GameSettings);
        File.WriteAllText($"{Application.persistentDataPath}/GameSettings.json", jsonData);
    }

    public void LoadSettings()
    {
        string path = $"{Application.persistentDataPath}/GameSettings.json";
        if (File.Exists(path))
        {
            string jsonText = File.ReadAllText(path);
            GameSettings = JsonUtility.FromJson<SaveData>(jsonText);
            ApplyAudioLevels(); 
            PlayerBehavior player = Object.FindObjectOfType<PlayerBehavior>();
            if (player != null) player.SetSensitivity(GameSettings.mouseSensitivity);
            OnSettingsLoaded?.Invoke(GameSettings);
        }
    }

    public void SetMouseSensitivity(float value)
    {
        GameSettings.mouseSensitivity = value;
        PlayerBehavior player = Object.FindObjectOfType<PlayerBehavior>();
        if (player != null) player.SetSensitivity(value);
        SaveSettings();
    }

    public void SetMasterVolume(float value)
    {
        GameSettings.masterVolume = value / 100f;
        ApplyAudioLevels();
        SaveSettings();
    }

    public void SetMusicVolume(float value)
    {
        GameSettings.musicVolume = value / 100f;
        ApplyAudioLevels();
        SaveSettings();
    }

    public void SetSFXVolume(float value)
    {
        GameSettings.sfxVolume = value / 100f;
        ApplyAudioLevels();
        SaveSettings();
    }

    private void ApplyAudioLevels()
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer not assigned!");
            return;
        }

        Debug.Log($"Setting volumes - Master: {GameSettings.masterVolume}, Music: {GameSettings.musicVolume}, SFX: {GameSettings.sfxVolume}");

        audioMixer.SetFloat("MasterVol", LinearToDecibel(GameSettings.masterVolume));
        audioMixer.SetFloat("MusicVol", LinearToDecibel(GameSettings.musicVolume));
        audioMixer.SetFloat("SFXVol", LinearToDecibel(GameSettings.sfxVolume));
    }

    private float LinearToDecibel(float linear)
    {
        return linear != 0 ? 20f * Mathf.Log10(linear) : -80f;
    }
}