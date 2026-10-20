
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour
{
    [Header("Scene Names")]
    public string mainMenuSceneName = "Main Menu";
    public string firstStageSceneName = "Stage 1";

    [Header("Main Menu Panels")]
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    // =========================
    // SCENE NAVIGATION
    // =========================

    public void PlayGame()
    {
        Time.timeScale = 1f;

        // Mulai perjalanan skor baru
        PlayerPrefs.SetInt("CurrentRunScore", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene(firstStageSceneName);
    }

    public void OpenMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

        Debug.Log("Game ditutup.");
    }

    // =========================
    // SETTINGS PANEL
    // =========================

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // =========================
    // CREDITS PANEL
    // =========================

    public void OpenCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}
