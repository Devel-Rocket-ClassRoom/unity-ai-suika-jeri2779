using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Playing,
    GameOver,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = GameState.Playing;
    public int Score { get; private set; }

    public List<Fruit> activeFruits = new();

    [Header("Game Over")]
    public float gameOverLineY = 4f;
    public float gameOverDelay = 2f;

    float overLineTimer;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (State != GameState.Playing)
            return;

        bool anyOver = false;
        foreach (var fruit in activeFruits)
        {
            if (fruit == null)
                continue;
            var col = fruit.GetComponent<CircleCollider2D>();
            float top = fruit.transform.position.y + col.radius * fruit.transform.localScale.x;
            if (fruit.IsDropped && top > gameOverLineY)
            {
                anyOver = true;
                break;
            }
        }

        overLineTimer = anyOver ? overLineTimer + Time.deltaTime : 0f;
        if (overLineTimer >= gameOverDelay)
            TriggerGameOver();
    }

    [Header("Fruit Scale")]
    public float fruitMinScale = 0.3f;
    public float fruitMaxScale = 1.8f;

    public float GetFruitScale(int level)
    {
        float t = (level - 1) / 10f;
        return fruitMinScale * Mathf.Pow(fruitMaxScale / fruitMinScale, t);
    }

    public void AddScore(int points)
    {
        Score += points;
        UIManager.Instance.UpdateScore(Score);
    }

    void TriggerGameOver()
    {
        State = GameState.GameOver;
        FruitSpawner.Instance.StopAllCoroutines();
        FruitSpawner.Instance.DestroyPreview();
        StartCoroutine(ShowGameOverPanel());
    }

    IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.ShowGameOver(Score);
    }
}
