using System.Collections;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner Instance { get; private set; }

    [Header("Prefabs (index 0 = level 1 ... index 4 = level 5, 투하 가능 과일)")]
    public GameObject[] spawnablePrefabs;   // 레벨 1~5

    [Header("Spawn Settings")]
    public float spawnY = 5f;
    public float dropDelay = 0.5f;

    [Header("Wall References (for preview clamp)")]
    public Transform leftWall;
    public Transform rightWall;

    GameObject currentFruit;
    int nextLevel;
    bool canDrop;

    void Awake() => Instance = this;

    void Start()
    {
        nextLevel = RandomLevel();
        SpawnPreview();
    }

    void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;
        if (!canDrop || currentFruit == null) return;

        // 프리뷰 과일 X 위치 = 마우스 X (벽 안쪽으로 Clamp)
        float radius  = currentFruit.GetComponent<CircleCollider2D>().radius
                        * currentFruit.transform.localScale.x;
        float minX    = leftWall.position.x  + radius;
        float maxX    = rightWall.position.x - radius;
        float worldX  = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

        currentFruit.transform.position = new Vector3(
            Mathf.Clamp(worldX, minX, maxX), spawnY, 0f);

        if (Input.GetMouseButtonDown(0))
            StartCoroutine(DropAndSpawnNext());
    }

    void SpawnPreview()
    {
        int level = nextLevel;
        nextLevel  = RandomLevel();

        var go    = Instantiate(spawnablePrefabs[level - 1],
                                new Vector3(0f, spawnY, 0f), Quaternion.identity);
        go.transform.localScale = Vector3.one * GameManager.Instance.GetFruitScale(level);
        var fruit = go.GetComponent<Fruit>();
        // Kinematic 상태 유지 — Drop() 호출 전까지 물리 비활성
        go.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        go.GetComponent<CircleCollider2D>().enabled = false;

        currentFruit = go;
        canDrop      = true;

        UIManager.Instance.UpdateNextFruit(nextLevel);
    }

    IEnumerator DropAndSpawnNext()
    {
        canDrop = false;
        currentFruit.GetComponent<Fruit>().Drop();
        currentFruit = null;

        yield return new WaitForSeconds(dropDelay);

        if (GameManager.Instance.State == GameState.Playing)
            SpawnPreview();
    }

    public void DestroyPreview()
    {
        if (currentFruit != null)
        {
            Destroy(currentFruit);
            currentFruit = null;
        }
    }

    static int RandomLevel() => Random.Range(1, 6); // 1~5
}
