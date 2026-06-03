using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public SaveData data;

    private string savePath;
    private const string SaveFileName = "save.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = GetSavePath();

        LoadGame();
    }

    public void SaveGame()
    {
        WriteStoredProgress(data);
    }

    public void ResetGame()
    {
        data = new SaveData();
        ClearStoredProgress();
    }

    public static void ClearStoredProgress()
    {
        string path = GetSavePath();

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        PlayerPrefs.DeleteKey("SpiderName");
        PlayerPrefs.DeleteKey("SpiderLastX");
        PlayerPrefs.DeleteKey("SpiderLastY");
        PlayerPrefs.DeleteKey("SpiderLastZ");
        PlayerPrefs.Save();
    }

    public static void WriteStoredProgress(SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(GetSavePath(), json);
    }

    private static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json =
                File.ReadAllText(savePath);

            data =
                JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            data = new SaveData();
        }
    }
}
