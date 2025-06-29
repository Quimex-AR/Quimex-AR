using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void LoadDaltonGame()
    {
        SceneManager.LoadScene("DaltonGame");
    }
}
