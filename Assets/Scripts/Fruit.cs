using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Fruit Data")]
    public int fruitLevel;   // 1~11
    public int mergeScore;

    public bool IsDropped { get; private set; }

    bool isMerging;
    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void OnEnable()  => GameManager.Instance?.activeFruits.Add(this);
    void OnDisable() => GameManager.Instance?.activeFruits.Remove(this);

    // FruitSpawner용 — 즉시 Dynamic 전환
    public void Drop()
    {
        IsDropped = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    // MergeHandler용 — IsDropped만 설정, bodyType은 MergeHandler가 직접 제어
    public void MarkDropped() => IsDropped = true;

    public void SetMerging() => isMerging = true;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsDropped || isMerging) return;

        var other = collision.gameObject.GetComponent<Fruit>();
        if (other == null || !other.IsDropped || other.isMerging) return;
        if (other.fruitLevel != fruitLevel) return;

        if (gameObject.GetInstanceID() < other.gameObject.GetInstanceID())
        {
            // 양쪽 동시에 플래그 — 이 쌍의 재발동 + 제3 과일과의 중복 머지 모두 차단
            isMerging = true;
            other.SetMerging();
            MergeHandler.Instance.Merge(this, other);
        }
    }
}
