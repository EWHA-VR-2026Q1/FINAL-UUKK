using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public SaveData data;

    private string savePath;

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

        savePath =
            Application.persistentDataPath +
            "/save.json";

        LoadGame();
    }

    public void SaveGame()
    {
        string json =
            JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);
    }

    public void ResetGame()
    {
        data = new SaveData();

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        PlayerPrefs.DeleteKey("SpiderName");
        PlayerPrefs.Save();
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
