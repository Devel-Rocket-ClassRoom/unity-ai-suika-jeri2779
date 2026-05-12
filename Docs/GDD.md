# GDD — 수박 게임 (Suika Game Clone)

> 작성일: 2026-05-12  
> 버전: 0.1 (초안)  
> 엔진: Unity 6.3 (6000.3.11f1)  
> 과제 기준: 필수 미션 1·2·3 완료 조건 전체 반영

---

## 1. 게임 개요

**한 줄 요약**  
과일을 떨어뜨려 쌓고, 같은 과일끼리 합쳐 더 큰 과일을 만들어 최고 점수를 노리는 물리 기반 퍼즐 게임.

**코어 루프**  
```
과일 선택(랜덤, 1턴 미리보기) → 마우스로 투하 위치 지정 → 투하
→ 물리 연산(중력·충돌) → 같은 과일 접촉 시 머지 → 점수 획득
→ 과일이 상단 라인 초과 유지 시 게임오버 → 결과 화면 → 재시작
```

---

## 2. 핵심 메커닉 (미션 1 기준)

> 미션 1 목표: 떨어뜨리기·충돌·머지·게임오버가 한 흐름으로 동작

### 2-1. 투하 (Drop)

| 항목 | 내용 |
|------|------|
| 입력 방식 | 마우스 좌클릭 (또는 키보드 좌우 이동 + Space 투하) |
| 투하 기준점 | 화면 상단 투하 가이드 라인 위에 현재 과일 표시 |
| 제한 조건 | 이전 과일이 완전히 바닥에 안착하기 전에는 투하 불가 (또는 일정 딜레이 후 투하 허용 — 구현 시 결정) |
| 투하 가능 과일 | 1~5단계 (체리·딸기·포도·데코폰·감) 중 랜덤 등장 |

### 2-2. 물리·충돌 (Physics & Collision)

| 항목 | 내용 |
|------|------|
| 물리 엔진 | Unity 2D Rigidbody + CircleCollider2D |
| 중력 | Unity 기본 중력 (조정 가능) |
| 바닥·벽 충돌 | 정적 BoxCollider2D 벽 3면 (좌·우·바닥) |
| 과일 간 충돌 | 물리 레이어 분리, 과일끼리 탄성 충돌로 굴러다님 |
| 콜라이더 형태 | 원형(Circle) — 단계별 반지름 비율은 섹션 3 참고 |

### 2-3. 머지 (Merge)

| 항목 | 내용 |
|------|------|
| 발동 조건 | 동일 단계 과일 2개의 콜라이더가 OnCollisionEnter2D 접촉 |
| 결과 | 접촉 지점 중앙에 다음 단계 과일 1개 스폰, 원본 2개 즉시 소멸 |
| 최종 단계(수박) 머지 | 수박 2개 접촉 → 66점 획득 후 두 수박 모두 소멸 (다음 단계 없음) |
| 연쇄 머지 | 허용 (스폰된 과일이 즉시 다른 동일 과일과 접촉하면 연쇄 발동) |
| 점수 지급 시점 | 머지 성공 즉시 |

### 2-4. 게임오버 판정

| 항목 | 내용 |
|------|------|
| 판정 라인 | 컨테이너 상단에 수평 빨간 선(게임오버 라인) 표시 |
| 판정 조건 | 어떤 과일이든 게임오버 라인 위에 **2초 이상** 머무르면 게임오버 |
| 유예 표시 | 라인 초과 중 경고 연출(예: 라인 색 변경 또는 경고 이펙트) |
| 즉시 게임오버 아님 | 과일이 일시적으로 라인 위로 튀어도 2초 이내 내려오면 계속 진행 |
| 게임오버 처리 | 입력 차단 → 결과 화면 전환 (미션 3 연계) |

---

## 3. 과일 단계 (미션 2 기준)

> 미션 2 완료 조건: **GDD에서 정한 단계 수만큼** 스프라이트 생성  
> → 본 GDD는 **11단계** 확정 (원작 동일)

### 3-1. 과일 단계표

| 단계 | 이름 | 머지 점수 | 상대 반지름 비율 | 투하 가능 |
|------|------|-----------|-----------------|-----------|
| 1 | 체리 🍒 | 1 pt | 1.0 (기준) | ✅ |
| 2 | 딸기 🍓 | 3 pt | 1.3 | ✅ |
| 3 | 포도 🍇 | 6 pt | 1.7 | ✅ |
| 4 | 데코폰 🍊 | 10 pt | 2.2 | ✅ |
| 5 | 감 🫑 | 15 pt | 2.6 | ✅ |
| 6 | 사과 🍎 | 21 pt | 3.0 | ❌ (머지 전용) |
| 7 | 배 🍐 | 28 pt | 3.5 | ❌ |
| 8 | 복숭아 🍑 | 36 pt | 4.0 | ❌ |
| 9 | 파인애플 🍍 | 45 pt | 4.6 | ❌ |
| 10 | 멜론 🍈 | 55 pt | 5.3 | ❌ |
| 11 | 수박 🍉 | 66 pt | 6.0 | ❌ |

> **반지름 비율 기준**: 체리 반지름 = 0.5 Unity Unit.  
> 수박 반지름 = 3.0 Unity Unit (체리의 6배).  
> 실제 수치는 컨테이너 크기에 맞게 조정 가능 — 이 비율을 유지하는 것이 핵심.

### 3-2. Unity AI Generators 스프라이트 명세

> 미션 2: 픽셀 단위 정렬·투명도·콜라이더 모양 맞춤 조건 반영

| 항목 | 규격 |
|------|------|
| 해상도 | 각 과일 512×512 px (정사각형) |
| 배경 | 투명 (PNG, Alpha 채널 포함) |
| 과일 위치 | 이미지 정중앙 배치 |
| Pivot | 중앙 (0.5, 0.5) |
| Pixels Per Unit | 100 |
| 콜라이더 형태 | CircleCollider2D — Radius는 스프라이트 반지름의 약 90% |
| 아트 방향 | 귀엽고 단순한 2D 일러스트 / 밝은 채도 / 그림자 없음 |
| 파일명 규칙 | `Fruit_01_Cherry.png` ~ `Fruit_11_Watermelon.png` |

---

## 4. 점수·승패 규칙

### 4-1. 점수 시스템

- 점수 = 머지 시 획득하는 값의 누적 합계
- 투하만으로는 점수 없음
- 연쇄 머지 시 각 머지마다 개별 점수 적용
- 이론적 최고 점수: 한 판에 획득 가능한 최대값 (무제한, 기록 경쟁)

### 4-2. 게임오버

- 게임오버 라인 위 2초 이상 유지 → 게임오버
- 게임오버 시 최종 점수 확정, 입력 차단

### 4-3. 클리어 없음

- 명시적 클리어 조건 없음 (얼마나 오래, 얼마나 높은 점수를 내느냐가 목표)
- 수박(11단계) 완성은 플레이어의 체감 목표로 존재

---

## 5. 화면·UI 구성 (미션 3 기준)

> 미션 3 완료 조건: 점수 표시 / 다음 과일 미리보기 / 게임오버 결과 화면 / 전 사이클 플레이 가능

### 5-1. 인게임 HUD 레이아웃

```
┌─────────────────────────────────┐
│  [점수: 0]          [다음 과일🍒] │  ← HUD 상단바
│─────────────────────────────────│
│       ↓ (투하 가이드 커서)       │
│                                 │
│   ══════ 게임오버 라인 ══════   │  ← 빨간 선
│                                 │
│  ┌─────────────────────────┐   │
│  │                         │   │
│  │      게임 컨테이너       │   │
│  │      (과일 낙하 공간)     │   │
│  │                         │   │
│  └─────────────────────────┘   │
└─────────────────────────────────┘
```

| UI 요소 | 위치 | 내용 |
|---------|------|------|
| 현재 점수 | **좌측 상단** | `점수: [숫자]` — 머지 시 즉시 갱신 |
| 다음 과일 미리보기 | **우측 상단** | `다음 과일:` + 과일 스프라이트 썸네일 표시 |
| 투하 가이드 | 컨테이너 상단, 마우스 X 추적 | 현재 투하할 과일 표시 + 수직 점선 |
| 게임오버 라인 | 컨테이너 상단 | 빨간 수평선 |

### 5-2. 게임오버 결과 화면

```
┌─────────────────────────────────┐
│                                 │
│         GAME OVER               │
│                                 │
│       최종 점수: 1234            │
│                                 │
│       [ 다시 시작 ]              │
│                                 │
└─────────────────────────────────┘
```

| UI 요소 | 내용 |
|---------|------|
| 타이틀 | "GAME OVER" 텍스트 |
| 최종 점수 | 인게임 획득 점수 표시 |
| 다시 시작 버튼 | 클릭 시 씬 재로드 또는 상태 초기화 |

### 5-3. 게임 상태 흐름 (State Flow)

```
[게임 시작]
    ↓
[Playing 상태]
  - 과일 투하 가능
  - 물리·머지 동작
  - HUD 갱신 중
    ↓ (게임오버 조건 충족)
[GameOver 상태]
  - 입력 차단
  - 결과 화면 표시
    ↓ (다시 시작 버튼 클릭)
[게임 시작] ← 씬 재로드 또는 GameManager 초기화
```

---

## 6. 톤 & 분위기

| 항목 | 방향 |
|------|------|
| 색감 | 밝고 채도 높은 파스텔 계열 — 흰색/연노랑 배경 |
| 아트 스타일 | 단순하고 귀여운 2D 아이콘 스타일 (Unity AI Generators 생성) |
| 컨테이너 | 나무 재질 느낌 또는 흰 사각 박스 |
| BGM | 경쾌하고 가벼운 루프 음악 (자유 구현 사항) |
| 효과음 | 투하음 / 머지음 / 게임오버음 (자유 구현 사항) |

---

## 7. 기술 스택 & 씬 구성

| 항목 | 내용 |
|------|------|
| 엔진 | Unity 6.3 (6000.3.11f1) |
| 물리 | Unity 2D Physics |
| 렌더링 | 2D URP 또는 Built-in |
| 씬 수 | 1개 (`GameScene`) — 재시작은 씬 리로드 방식 |
| 주요 스크립트 예정 | `GameManager`, `FruitSpawner`, `Fruit`, `MergeHandler`, `UIManager` |

---

## 8. 미션 완료 조건 체크리스트

### 미션 1 — 메인 메커닉

- [ ] 마우스(또는 키 입력)로 과일 1개 투하 가능
- [ ] 바닥·벽과 충돌 후 자연스럽게 멈춤
- [ ] 같은 종류 2개 접촉 → 다음 단계 1개로 합체
- [ ] 게임오버 라인 위 2초 이상 머무는 과일 → 게임 종료
- [ ] 시작 → 진행 → 게임오버 한 판 끊김 없이 플레이 가능

### 미션 2 — Unity AI Generators 애셋

- [ ] 11단계 과일 스프라이트 전부 생성 (Unity AI Generators 사용)
- [ ] 각 스프라이트 임포트: 투명 PNG, Pivot 중앙, PPU 100
- [ ] CircleCollider2D 크기 스프라이트에 맞춤
- [ ] 플레이스홀더 도형 → 생성 스프라이트 전량 교체
- [ ] 플레이 화면의 모든 과일이 직접 만든 아트로 표시

### 미션 3 — UI·마무리

- [ ] 현재 점수 화면에 실시간 표시
- [ ] 다음 투하 과일 미리보기 표시
- [ ] 게임오버 시 결과 화면 (최종 점수 + 다시 시작 버튼)
- [ ] 다시 시작 버튼으로 초기 상태 복귀
- [ ] 시작 → 플레이 → 게임오버 → 재시작 전 사이클 에디터/빌드에서 동작

---

## 9. 구현 결정 사항 & 주의점

> 설계 검토 중 발견된 버그 가능성·수치 충돌·구조 결정을 기록한다.  
> 구현 시 이 섹션을 먼저 확인할 것.

### 9-1. 오브젝트 관리 방식 — Instantiate/Destroy (풀링 없음)

- 과일 생성 빈도: 클릭 1회 = 1개 (초당 1~2개 수준)
- 동시 최대 과일 수: 물리 구조상 50~80개 내외, 그 이상이면 게임오버가 먼저
- 머지는 2 소멸 + 1 생성 → 오브젝트 수가 줄어드는 구조
- **결정**: 단순 `Instantiate / Destroy` 사용. 풀링은 복잡도만 올리고 체감 차이 없음.

---

### 9-2. 이중 머지 버그 방지 (Critical)

**원인**: `OnCollisionEnter2D`는 충돌하는 양쪽 오브젝트 모두에서 발동 → 같은 쌍이 2번 처리되어 없는 오브젝트 삭제 시도 또는 엉뚱한 과일 소멸.

**해결**: `Fruit.cs` 내에서 인스턴스 ID가 작은 쪽만 머지를 시작하도록 필터링.

```csharp
// Fruit.cs — OnCollisionEnter2D
if (other.fruitLevel == fruitLevel
    && gameObject.GetInstanceID() < other.gameObject.GetInstanceID())
    MergeHandler.Instance.Merge(this, other);
```

---

### 9-3. MergeHandler — 싱글턴으로 구성

**원인**: `Fruit`는 런타임에 동적 생성되므로 `MergeHandler`를 매 충돌마다 `FindObjectOfType`으로 찾으면 성능 낭비.

**해결**: MergeHandler를 싱글턴으로 구성, `Fruit`에서 `MergeHandler.Instance`로 직접 접근.

```csharp
public class MergeHandler : MonoBehaviour {
    public static MergeHandler Instance { get; private set; }
    void Awake() => Instance = this;
}
```

---

### 9-4. 게임오버 라인 감시 — 리스트 관리

**원인**: 매 프레임 `FindObjectsOfType<Fruit>()` 순회는 과일이 쌓일수록 비용 증가.

**해결**: 과일 생성·소멸 시점에 `GameManager`가 리스트로 직접 관리.

```csharp
// GameManager.cs
public List<Fruit> activeFruits = new();

// Fruit.cs
void OnEnable()  => GameManager.Instance.activeFruits.Add(this);
void OnDisable() => GameManager.Instance.activeFruits.Remove(this);
```

---

### 9-5. PPU 100 고정 시 반지름 비율 수치 충돌 (미션 2)

**원인**: `512px ÷ PPU 100 = 5.12 Units`. 수박 지름 = 반지름 3.0 × 2 = `6.0 Units` → 스프라이트(5.12)보다 콜라이더가 커지는 불일치.

**해결**: PPU는 100으로 고정, 각 과일 프리팹의 `Transform.localScale`로 단계별 크기 비율 적용. CircleCollider2D 반지름은 스케일 적용 후 수동으로 맞춤.

| 단계 | 상대 반지름 | localScale (참고값) |
|------|------------|---------------------|
| 1 체리 | 1.0 | 0.33 |
| 2 딸기 | 1.3 | 0.43 |
| 3 포도 | 1.7 | 0.56 |
| 4 데코폰 | 2.2 | 0.73 |
| 5 감 | 2.6 | 0.86 |
| 6 사과 | 3.0 | 1.00 |
| 7 배 | 3.5 | 1.17 |
| 8 복숭아 | 4.0 | 1.33 |
| 9 파인애플 | 4.6 | 1.53 |
| 10 멜론 | 5.3 | 1.77 |
| 11 수박 | 6.0 | 2.00 |

> localScale은 사과(단계 6)를 기준 1.0으로 잡아 역산한 참고값. 컨테이너 크기에 따라 전체 비율을 동일하게 조정 가능.

---

### 9-6. AI 생성 이미지 투명 여백 불균일 (미션 2)

**원인**: Unity AI Generators는 과일 영역 외 투명 여백 크기가 이미지마다 다를 수 있음 → 콜라이더가 보이는 과일보다 크거나 작게 설정됨.

**해결**: 임포트 후 `Sprite Editor`에서 실제 과일 영역 확인. 플레이 화면에서 Gizmos 켜고 콜라이더 경계 육안 검수 필수 (11개 전수).

---

### 9-7. 프리팹 교체 시 씬 인스턴스 미반영 방지 (미션 2)

**원인**: 씬에 드래그해 놓은 프리팹 인스턴스는 Prefab Override 상태가 되어 프리팹 수정이 자동 반영되지 않음.

**해결**: 미션 1부터 과일을 씬에 직접 배치하지 않는다. 전부 `FruitSpawner`가 런타임에 `Instantiate`로 생성. 프리팹만 수정하면 전량 반영됨.

---

### 9-8. 게임오버 후 과일 투하 차단 (미션 3, Critical)

**원인**: 게임오버 패널이 뜬 상태에서 씬 영역 클릭 시 FruitSpawner가 과일을 투하할 수 있음.

**해결**: `FruitSpawner.Update`에서 `GameManager.State == GameState.Playing` 조건 필수 체크.

```csharp
void Update() {
    if (GameManager.Instance.State != GameState.Playing) return;
    // 투하 입력 처리
}
```

---

### 9-9. 게임오버 패널 표시 딜레이 (미션 3)

**원인**: 게임오버 판정 즉시 패널이 뜨면 마지막 과일이 물리 연산 중인 상태에서 화면이 전환되어 어색함.

**해결**: 게임오버 판정 후 코루틴으로 `0.5초` 딜레이 후 패널 활성화.

```csharp
IEnumerator ShowGameOverPanel() {
    yield return new WaitForSeconds(0.5f);
    UIManager.Instance.ShowGameOver(score);
}
```

---

### 9-10. 다시 시작 버튼 중복 클릭 방지 (미션 3)

**원인**: 씬 로드 중 버튼 연속 클릭 시 `LoadScene` 중복 호출 가능.

**해결**: 클릭 즉시 버튼 비활성화.

```csharp
void OnRestartClicked() {
    restartButton.interactable = false;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
```

---

### 9-11. 다음 과일 미리보기 동기화 (미션 3)

**원인**: FruitSpawner가 "다음 과일" 데이터를 결정하는 시점과 UIManager가 표시하는 시점이 어긋나면 실제 투하 과일과 미리보기가 1턴 엇갈림.

**해결**: 투하 직후 다음 과일이 결정되는 즉시 `UIManager.UpdateNextFruit(nextFruitData)` 단방향 호출로 고정.

---

### 9-12. 프리뷰 과일 — 클릭 위치에서 낙하 보장

**동작**: 프리뷰 과일 X = 마우스 X → 클릭 시 `bodyType = Dynamic` 전환 → 전환 시점의 위치에서 그대로 낙하.  
클릭 후 마우스가 이동해도 이미 Dynamic 상태이므로 과일이 따라가지 않음. 별도 처리 불필요.

---

### 9-13. 과일 벽 막힘 판정 — 프리뷰·투하 후 모두 보장

과일은 프리뷰(Kinematic)와 투하 후(Dynamic) 두 상태 모두 벽에 막혀야 한다.  
상태마다 메커니즘이 다르므로 각각 명시한다.

| 상태 | 벽 막힘 방식 | 이유 |
|------|-------------|------|
| **프리뷰 (Kinematic)** | 코드 Clamp | Kinematic은 물리 충돌 무시 → BoxCollider2D 효과 없음 |
| **투하 후 (Dynamic)** | BoxCollider2D 물리 충돌 | 자동 처리, 별도 코드 불필요 |

**프리뷰 Clamp 구현** (FruitSpawner.Update):

```csharp
float radius   = currentFruit.GetComponent<CircleCollider2D>().radius
                 * currentFruit.transform.localScale.x;
float clampMin = leftWall.position.x  + radius;
float clampMax = rightWall.position.x - radius;
float worldX   = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

currentFruit.transform.position = new Vector3(
    Mathf.Clamp(worldX, clampMin, clampMax),
    spawnY,
    0f
);
```

**투하 후 벽 막힘** (자동):  
`rb.bodyType = Dynamic` 전환 즉시 좌·우·바닥 BoxCollider2D(Static)가 충돌을 처리한다.  
클램핑된 위치에서 투하되므로 벽과 겹침 없이 정상 낙하 보장.

---

### 9-14. 연쇄 머지 타이밍 — 스폰 직후 1프레임 콜라이더 비활성

**원인**: Unity 물리는 FixedUpdate 주기로 동작. Instantiate 직후 같은 프레임에서는 OnCollisionEnter2D가 발동되지 않아 연쇄 머지가 끊길 수 있음.

**해결**: 스폰 후 1프레임 뒤 콜라이더를 활성화해 물리 사이클을 맞춤.

```csharp
// MergeHandler.cs — 머지 후 스폰
col.enabled = false;
yield return null;
col.enabled = true;
```

---

### 9-15. activeFruits 리스트 null 방어

**원인**: Destroy는 해당 프레임 말에 실행되므로, 같은 프레임 내 게임오버 체크 시 소멸 예정 과일이 리스트에 남아 있을 수 있음.

**해결**: 게임오버 체크 루프에서 null 여부 선행 확인.

```csharp
// GameManager.cs
foreach (var fruit in activeFruits) {
    if (fruit == null) continue;
    if (fruit.isDropped && fruit.transform.position.y > gameOverLineY) ...
}
```

---

### 9-16. 게임오버 2초 타이머 — 과일별 개별 관리

**원인**: 단일 타이머로 구현하면 여러 과일이 동시에 라인을 초과하거나, 과일이 내려갔다 다시 올라올 때 타이머가 잘못 누적됨.

**해결**: 타이머를 GameManager 단일 변수가 아닌 "라인 위에 있는 과일이 1개라도 존재하는 동안" 누적하는 방식으로 구현. 라인 위 과일 수가 0이 되면 즉시 리셋.

```csharp
// GameManager.cs
float overLineTimer = 0f;

void Update() {
    bool anyOver = activeFruits.Any(f => f != null && f.isDropped
                                         && f.transform.position.y > gameOverLineY);
    overLineTimer = anyOver ? overLineTimer + Time.deltaTime : 0f;
    if (overLineTimer >= 2f) TriggerGameOver();
}
```

---

### 9-18. 투하 후 딜레이 중 게임오버 시 코루틴 차단

**원인**: 투하 후 다음 과일 스폰 딜레이(0.5초) 도중 게임오버가 발생하면 코루틴이 그대로 실행되어 게임오버 상태에서 과일이 스폰됨.

**해결**: GameManager가 게임오버 처리 시 FruitSpawner 코루틴을 명시적으로 중단.

```csharp
// GameManager.cs — 게임오버 처리
void TriggerGameOver() {
    state = GameState.GameOver;
    FruitSpawner.Instance.StopAllCoroutines();
    if (currentFruit != null) Destroy(currentFruit); // 프리뷰 과일도 제거
    StartCoroutine(ShowGameOverPanel());
}
```

투하 딜레이 흐름:
```
클릭 → currentFruit.Drop() → canDrop = false
     → StartCoroutine(SpawnNext(0.5f))
     → 0.5초 후: 다음 과일 Instantiate → canDrop = true
```

---

## 10. 씬 계층 구조 (에디터 기준)

```
Scene: GameScene
├── GameManager          ← GameManager.cs (싱글턴)
├── FruitSpawner         ← FruitSpawner.cs (싱글턴)
├── MergeHandler         ← MergeHandler.cs (싱글턴)
├── Container
│   ├── LeftWall         ← BoxCollider2D (Static)
│   ├── RightWall        ← BoxCollider2D (Static)
│   └── Bottom           ← BoxCollider2D (Static)
├── GameOverLine         ← Transform (Y 기준점, LineRenderer 시각 표시)
└── Canvas (Screen Space - Overlay)
    ├── ScoreText        ← TextMeshProUGUI
    ├── NextFruitPanel
    │   ├── Label        ← TextMeshProUGUI "다음 과일"
    │   └── NextFruitImage ← Image
    └── GameOverPanel    ← UIManager.cs 부착, 기본 비활성
        ├── TitleText    ← TextMeshProUGUI "GAME OVER"
        ├── FinalScoreText ← TextMeshProUGUI
        └── RestartButton ← Button

Prefabs/
├── Fruit_01_Cherry  ← Fruit.cs + Rigidbody2D + CircleCollider2D + SpriteRenderer
├── Fruit_02_Strawberry
│   ... (동일 구조)
└── Fruit_11_Watermelon
```

---

*이 GDD는 구현 과정에서 수치·세부 규칙을 다듬을 수 있으나, 미션 완료 조건 항목은 변경 없이 유지한다.*
