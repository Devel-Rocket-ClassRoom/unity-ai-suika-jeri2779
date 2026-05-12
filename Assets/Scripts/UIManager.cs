using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    public TextMeshProUGUI scoreText;
    public Image nextFruitImage;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public Button restartButton;

    [Header("Next Fruit Sprites (index 0 = level 1)")]
    public Sprite[] fruitSprites;

    void Awake()
    {
        Instance = this;
        gameOverPanel.SetActive(false);
        scoreText.text = "점수: 0";
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"점수: {score}";
    }

    public void UpdateNextFruit(int level)
    {
        if (fruitSprites != null && level - 1 < fruitSprites.Length)
            nextFruitImage.sprite = fruitSprites[level - 1];
    }

    public void ShowGameOver(int finalScore)
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = $"점수: {finalScore}";
    }

    public void OnRestartClicked()
    {
        restartButton.interactable = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
