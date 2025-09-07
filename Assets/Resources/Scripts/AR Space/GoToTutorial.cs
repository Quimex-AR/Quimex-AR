using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoTOTutorial : MonoBehaviour, IPointerClickHandler
{
  private bool isLoading = false;

  public void OnPointerClick(PointerEventData eventData)
  {
    if (!isLoading)
    {
      StartCoroutine(StartGame());
    }
  }

  private IEnumerator StartGame()
  {
    isLoading = true;

    yield return new WaitForSeconds(1.5f);

    if (FadeCanvas.Instance != null)
    {
      yield return StartCoroutine(FadeCanvas.Instance.FadeToBlack());
    }

    LoadingScreenController.targetScene = "TFUE";
    SceneManager.LoadScene("Loading Scene");
  }
}
