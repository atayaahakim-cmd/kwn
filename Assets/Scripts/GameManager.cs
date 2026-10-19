
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

        pausePanel.SetActive(false);
        quizPanel.SetActive(false);
        finishPanel.SetActive(false);

        UpdateHUD();
        SetupNextStageButton();
    }

    public void CollectMaterial(MaterialData data)
    {
        if (data == null || stageFinished ||
            collectedData.Contains(data))
            return;

        collectedData.Add(data);
        collectedMaterials++;
        score += 10;

        Time.timeScale = 0f;
        UpdateHUD();
        materialUI.Show(data);
    }

    public void ContinueAfterMaterial()
    {
        materialUI.Hide();
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        if (quizActive || stageFinished)
            return;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
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
                "Jumlah soal harus sama dengan nomor stage.");
            return;
        }

        quizActive = true;
        currentQuestion = 0;
        correctCount = 0;

        pausePanel.SetActive(false);
        quizPanel.SetActive(true);
        Time.timeScale = 0f;

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        answering = true;
        feedbackText.text = "";

        QuizQuestion q = questions[currentQuestion];
        questionText.text = q.question;
        questionCounterText.text =
            $"Soal {currentQuestion + 1}/{questions.Count}";

        for (int i = 0; i < optionButtons.Length; i++)
        {
            bool valid = i < q.options.Length;
            optionButtons[i].gameObject.SetActive(valid);
            optionButtons[i].interactable = valid;

            if (valid)
            {
                optionTexts[i].text = q.options[i];
                int answerIndex = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(
                    () => AnswerQuestion(answerIndex));
            }
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
            feedbackText.text = "Benar! +100 poin";
        }
        else
        {
            feedbackText.text = "Belum tepat.";
        }

        feedbackText.text += "\n" + q.explanation;
        UpdateHUD();

        // Matikan pilihan sampai pemain menekan Lanjut.
        foreach (Button button in optionButtons)
            button.interactable = false;
    }

    public void NextQuestion()
    {
        if (answering)
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

        quizPanel.SetActive(false);
        finishPanel.SetActive(true);

        finalScoreText.text = "Score: " + score;
        correctCountText.text =
            $"Jawaban benar: {correctCount}/{questions.Count}";

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
                : "Selesaikan Game";
        }

        nextStageButton.onClick.RemoveAllListeners();

        if (hasNext)
        {
            nextStageButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().buildIndex + 1);
            });
        }
        else
        {
            nextStageButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(0);
            });
        }
    }

    private void UpdateHUD()
    {
        if (scoreText != null)
            scoreText.text = "Skor: " + score;

        if (starText != null)
            starText.text =
                $"Bintang: {collectedMaterials}/{requiredMaterials}";
    }

    public bool AllMaterialsCollected()
    {
        return collectedMaterials >= requiredMaterials;
    }
}
