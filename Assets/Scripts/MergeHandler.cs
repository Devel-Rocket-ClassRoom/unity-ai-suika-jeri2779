using System.Collections;
using UnityEngine;

public class MergeHandler : MonoBehaviour
{
    public static MergeHandler Instance { get; private set; }

    [Header("Fruit Prefabs (index 0 = level 1 ... index 10 = level 11)")]
    public GameObject[] fruitPrefabs;

    void Awake() => Instance = this;

    public void Merge(Fruit a, Fruit b)
    {
        int level = a.fruitLevel;
        Vector3 midPoint = (a.transform.position + b.transform.position) * 0.5f;

        GameManager.Instance.AddScore(a.mergeScore);

        Destroy(a.gameObject);
        Destroy(b.gameObject);

        // 수박(11단계) 머지 — 다음 단계 없음
        if (level >= 11) return;

        StartCoroutine(SpawnNext(level + 1, midPoint));
    }

    IEnumerator SpawnNext(int nextLevel, Vector3 pos)
    {
        GameObject prefab = fruitPrefabs[nextLevel - 1];
        var go = Instantiate(prefab, pos, Quaternion.identity);

        var fruit = go.GetComponent<Fruit>();
        fruit.Drop();   // 머지 결과물은 즉시 Dynamic

        // 1프레임 후 콜라이더 활성 — 연쇄 머지 물리 사이클 보장
        var col = go.GetComponent<CircleCollider2D>();
        col.enabled = false;
        yield return null;
        col.enabled = true;
    }
}
