using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject buttonsPanel;

    private void Start()
    {
        buttonsPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }
    public void OnClickToControlsPanel()
    {
        controlsPanel.SetActive(true);
        buttonsPanel.SetActive(false);
    }

    public void OnClickToButtonsPanel()
    {
        buttonsPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Outdoor");
    }

}
