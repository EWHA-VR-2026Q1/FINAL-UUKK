using System;

[Serializable]
public class SaveData
{
    // 가장 높게 도달한 스테이지

    public int highestUnlockedStage = 1;

    // 이어하기용

    public string currentScene = "";

    // 마지막 저장 위치

    public float posX;
    public float posY;
    public float posZ;
}