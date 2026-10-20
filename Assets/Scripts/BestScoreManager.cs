
using UnityEngine;
using TMPro;

public class BestScoreManager : MonoBehaviour
{
    [Header("UI Rekor Terbaik")]
    public TMP_Text bestScoreText;

    private const string BestScoreKey = "BestTotalScore";

    private void Start()
    {
        UpdateBestScoreUI();
    }

    public void SaveBestScore(int totalScore)
    {
        int bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        // Rekor hanya berubah jika skor baru lebih tinggi
        if (totalScore > bestScore)
        {
            PlayerPrefs.SetInt(BestScoreKey, totalScore);
            PlayerPrefs.Save();
        }

        UpdateBestScoreUI();
    }

    public void UpdateBestScoreUI()
    {
        int bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        if (bestScoreText != null)
        {
            bestScoreText.text = "Skor Terbaik : " + bestScore;
        }
    }

    // Opsional: reset rekor untuk pengujian
    public void ResetBestScore()
    {
        PlayerPrefs.DeleteKey(BestScoreKey);
        PlayerPrefs.Save();
        UpdateBestScoreUI();
    }
}
