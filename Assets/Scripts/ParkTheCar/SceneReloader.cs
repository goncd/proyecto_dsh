using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    public void Reload()
    {
        if (Game.Instance == null || Game.Instance.IsRestarting())
            return;
            
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
