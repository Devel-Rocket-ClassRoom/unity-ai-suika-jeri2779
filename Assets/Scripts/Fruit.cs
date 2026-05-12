using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Fruit Data")]
    public int fruitLevel;   // 1~11
    public int mergeScore;

    public bool IsDropped { get; private set; }

    Rigidbody2D rb;
    CircleCollider2D col;

    void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    void OnEnable()  => GameManager.Instance?.activeFruits.Add(this);
    void OnDisable() => GameManager.Instance?.activeFruits.Remove(this);

    public void Drop()
    {
        IsDropped = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsDropped) return;

        var other = collision.gameObject.GetComponent<Fruit>();
        if (other == null || !other.IsDropped) return;
        if (other.fruitLevel != fruitLevel) return;

        // 인스턴스 ID가 작은 쪽만 머지 시작 — 이중 발동 방지
        if (gameObject.GetInstanceID() < other.gameObject.GetInstanceID())
            MergeHandler.Instance.Merge(this, other);
    }
}
