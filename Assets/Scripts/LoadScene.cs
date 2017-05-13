using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

    [SerializeField]
    string SceneNames;

    // Use this for initialization
    public void OnClick(string SceneName)
    {
        Debug.Log("bOOM");
        if (SceneName != string.Empty)
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
