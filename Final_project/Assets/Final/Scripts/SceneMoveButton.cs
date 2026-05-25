using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoveButton : MonoBehaviour
{
    [SerializeField]
    private string targetSceneName;

    private void OnMouseDown()
    {
        Debug.Log("버튼 클릭 감지");
        SceneManager.LoadScene(targetSceneName);
    }
}

//Build Settings에서 사용할 씬을 모두 추가해야 함