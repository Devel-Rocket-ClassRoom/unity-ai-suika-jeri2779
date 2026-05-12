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
        var go = Instantiate(fruitPrefabs[nextLevel - 1], pos, Quaternion.identity);
        var fruit = go.GetComponent<Fruit>();
        var rb    = go.GetComponent<Rigidbody2D>();

        // 콜라이더 절대 비활성화 금지 — 중첩 방지
        // Kinematic으로 시작: 콜라이더는 활성 상태, 중력/외력만 차단
        rb.bodyType      = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        fruit.MarkDropped(); // IsDropped = true (머지 판정 활성화)

        // 물리 프레임 2회 대기
        // Kinematic 상태에서 주변 Dynamic 과일들이 자연스럽게 밀려나 안정화됨
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // Dynamic 전환 — 이후 정상 물리 작동
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
