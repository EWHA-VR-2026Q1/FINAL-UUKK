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

// 시험 임시 리셋용 함수
    public void ResetSave()
    {
        data = new SaveData();

        SaveGame();

        Debug.Log("Save Reset");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SaveManager.Instance.ResetSave();
        }
    }
    // R 버튼 누르면 임시 리셋
}