using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneAfterActivation : MonoBehaviour
{
    public string sceneToLoad;
    public float delay = 3f;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(ChangeSceneAfterDelay());
    }

    private System.Collections.IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No scene name specified for scene change!");
        }
    }
}