
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class QuizQuestion
{
    [TextArea] public string question;
    public string[] options = new string[4];
    [Range(0, 3)] public int correctAnswer;
    [TextArea] public string explanation;
}

public class GameManager : MonoBehaviour
{
    [Header("Stage Settings")]
    [Range(1, 3)] public int stageNumber = 1;
    public int requiredMaterials = 3;
    public List<QuizQuestion> questions = new List<QuizQuestion>();

    [Header("Scene Names")]
    public string mainMenuSceneName = "Main Menu";
    public string[] stageSceneNames =
    {
        "Stage 1",
        "Stage 2",
        "Stage 3"
    };

    [Header("Audio")]
    public AudioClip stageBGM;
    public AudioClip materialSFX;
    public AudioClip quizSFX;
    public AudioClip correctAnswerSFX;
    public AudioClip wrongAnswerSFX;
    public AudioClip winSFX;

    [Header("Panels")]
    public MaterialPanelUI materialUI;
    public GameObject pausePanel;
    public GameObject quizPanel;
    public GameObject finishPanel;

    [Header("Quiz UI")]
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public TMP_Text questionCounterText;
    public Button[] optionButtons;
    public TMP_Text[] optionTexts;

    [Header("HUD")]
    public TMP_Text scoreText;
    public TMP_Text starText;

    [Header("Finish UI")]
    public TMP_Text finalScoreText;
    public TMP_Text correctCountText;
    public Button nextStageButton;
    public TMP_Text nextStageButtonText;

    private int score;
    private int collectedMaterials;
    private int currentQuestion;
    private int correctCount;

    private bool quizActive;
    private bool stageFinished;
    private bool answering;

    private readonly HashSet<MaterialData> collectedData =
        new HashSet<MaterialData>();

    private void Start()
    {
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (quizPanel != null)
            quizPanel.SetActive(false);

        if (finishPanel != null)
            finishPanel.SetActive(false);

        UpdateHUD();
        SetupNextStageButton();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(stageBGM);
    }

    public void CollectMaterial(MaterialData data)
    {
        if (data == null || stageFinished ||
            collectedData.Contains(data))
            return;

        collectedData.Add(data);
        collectedMaterials++;
        score += 10;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(materialSFX);

        Time.timeScale = 0f;
        UpdateHUD();

        if (materialUI != null)
            materialUI.Show(data);
        else
            Debug.LogWarning("MaterialPanelUI belum di-assign.");
    }

    public void ContinueAfterMaterial()
    {
        if (materialUI != null)
            materialUI.Hide();

        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        if (quizActive || stageFinished)
            return;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void LoadStage(int stageIndex)
    {
        if (stageSceneNames == null ||
            stageIndex < 0 ||
            stageIndex >= stageSceneNames.Length)
        {
            Debug.Log("Index stage tidak valid.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(stageSceneNames[stageIndex]);
    }

    public void StartQuiz()
    {
        if (stageFinished || quizActive)
            return;

        if (collectedMaterials < requiredMaterials)
            return;

        if (questions.Count != stageNumber)
        {
            Debug.LogError(
                "Jumlah soal harus sama dengan nomor stage."
            );
            return;
        }

        quizActive = true;
        currentQuestion = 0;
        correctCount = 0;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (quizPanel != null)
            quizPanel.SetActive(true);

        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(quizSFX);

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        if (currentQuestion < 0 ||
            currentQuestion >= questions.Count)
            return;

        answering = true;

        if (feedbackText != null)
            feedbackText.text = "";

        QuizQuestion q = questions[currentQuestion];

        if (questionText != null)
            questionText.text = q.question;

        if (questionCounterText != null)
        {
            questionCounterText.text =
                $"Soal {currentQuestion + 1}/{questions.Count}";
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (optionButtons[i] == null)
                continue;

            bool valid = q.options != null &&
                         i < q.options.Length;

            optionButtons[i].gameObject.SetActive(valid);
            optionButtons[i].interactable = valid;

            if (!valid)
                continue;

            if (optionTexts != null &&
                i < optionTexts.Length &&
                optionTexts[i] != null)
            {
                optionTexts[i].text = q.options[i];
            }

            int answerIndex = i;

            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(
                () => AnswerQuestion(answerIndex)
            );
        }
    }

    private void AnswerQuestion(int answer)
    {
        if (!answering)
            return;

        answering = false;

        QuizQuestion q = questions[currentQuestion];
        bool correct = answer == q.correctAnswer;

        if (correct)
        {
            correctCount++;
            score += 100;

            if (feedbackText != null)
                feedbackText.text = "Benar! +100 poin";

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(correctAnswerSFX);
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Belum tepat.";

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(wrongAnswerSFX);
        }

        if (feedbackText != null)
            feedbackText.text += "\n" + q.explanation;

        UpdateHUD();

        foreach (Button button in optionButtons)
        {
            if (button != null)
                button.interactable = false;
        }
    }

    public void NextQuestion()
    {
        if (answering || !quizActive)
            return;

        currentQuestion++;

        if (currentQuestion < questions.Count)
        {
            ShowQuestion();
        }
        else
        {
            EndStage();
        }
    }

    private void EndStage()
    {
        quizActive = false;
        stageFinished = true;
        answering = false;

        if (quizPanel != null)
            quizPanel.SetActive(false);

        if (finishPanel != null)
            finishPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + score;

        if (correctCountText != null)
        {
            correctCountText.text =
                $"Jawaban benar: {correctCount}/{questions.Count}";
        }
        // Tambahkan setelah stage dinyatakan selesai,
        // dan sebelum skor akhir ditampilkan.

        // Ambil total skor dari stage sebelumnya
        int totalScore = PlayerPrefs.GetInt("CurrentRunScore", 0);

        // Tambahkan skor stage yang baru selesai
        totalScore += score;

        // Simpan total skor perjalanan
        PlayerPrefs.SetInt("CurrentRunScore", totalScore);
        PlayerPrefs.Save();

        // Jika Stage 3 selesai, simpan rekor terbaik
        if (stageNumber >= 3)
        {
            int bestScore = PlayerPrefs.GetInt("BestTotalScore", 0);

            if (totalScore > bestScore)
            {
                PlayerPrefs.SetInt("BestTotalScore", totalScore);
                PlayerPrefs.Save();
            }
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(winSFX);

        SetupNextStageButton();
    }

    private void SetupNextStageButton()
    {
        if (nextStageButton == null)
            return;

        bool hasNext = stageNumber < 3;

        nextStageButton.gameObject.SetActive(true);

        if (nextStageButtonText != null)
        {
            nextStageButtonText.text = hasNext
                ? "Stage Berikutnya"
                : "Main Menu";
        }

        nextStageButton.onClick.RemoveAllListeners();

        if (hasNext)
        {
            nextStageButton.onClick.AddListener(
                () => LoadStage(stageNumber)
            );
        }
        else
        {
            nextStageButton.onClick.AddListener(LoadMainMenu);
        }
    }

    private void UpdateHUD()
    {
        if (scoreText != null)
            scoreText.text = "Skor: " + score;

        if (starText != null)
        {
            starText.text =
                $"Bintang: {collectedMaterials}/{requiredMaterials}";
        }
    }

    public bool AllMaterialsCollected()
    {
        return collectedMaterials >= requiredMaterials;
    }
}
