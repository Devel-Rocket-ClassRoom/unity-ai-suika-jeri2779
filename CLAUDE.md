# CLAUDE.md — 수박게임 프로젝트

## 프로젝트 개요

Unity 6.3 기반 수박게임(Suika Game) 클론. 과일을 떨어뜨려 같은 종류끼리 합쳐 점수를 올리는 물리 기반 2D 퍼즐 게임.

- **엔진**: Unity 6000.3.11f1
- **물리**: Unity 2D Physics (Rigidbody2D + CircleCollider2D)
- **렌더링**: 2D
- **씬**: `Assets/Scenes/`
- **스크립트**: `Assets/Scripts/`
- **프리팹**: `Assets/Prefabs/`
- **GDD**: `Docs/GDD.md` ← 모든 구현 의뢰 시 이 문서를 컨텍스트로 참고

---

## 필수 미션 구조

### 미션 1 — 메인 메커닉
- 마우스 입력으로 과일 투하
- 바닥·벽 충돌 (BoxCollider2D 3면)
- 동일 과일 접촉 시 다음 단계로 머지
- 게임오버 라인 위 2초 이상 → 게임오버

### 미션 2 — Unity AI Generators 애셋
- 11단계 과일 스프라이트 생성 (Unity AI Generators 사용)
- 512×512 px / 투명 PNG / Pivot 중앙 / PPU 100
- CircleCollider2D 크기 스프라이트에 맞춤
- 미션 1의 플레이스홀더 원 → 스프라이트 전량 교체

### 미션 3 — UI·마무리
- 좌측 상단: 현재 점수
- 우측 상단: 다음 과일 미리보기
- 게임오버 결과 화면: 최종 점수 + 다시 시작 버튼
- 전 사이클(시작→플레이→게임오버→재시작) 에디터/빌드에서 동작

---

## 과일 단계표 (11단계)

| 단계 | 이름 | 머지 점수 | 투하 가능 |
|------|------|-----------|-----------|
| 1 | 체리 | 1 pt | ✅ |
| 2 | 딸기 | 3 pt | ✅ |
| 3 | 포도 | 6 pt | ✅ |
| 4 | 데코폰 | 10 pt | ✅ |
| 5 | 감 | 15 pt | ✅ |
| 6 | 사과 | 21 pt | ❌ |
| 7 | 배 | 28 pt | ❌ |
| 8 | 복숭아 | 36 pt | ❌ |
| 9 | 파인애플 | 45 pt | ❌ |
| 10 | 멜론 | 55 pt | ❌ |
| 11 | 수박 | 66 pt | ❌ |

---

## 예정 스크립트 구조

```
Assets/Scripts/
├── GameManager.cs      # 게임 상태(Playing/GameOver), 점수 관리
├── FruitSpawner.cs     # 현재·다음 과일 생성, 투하 입력 처리
├── Fruit.cs            # 단계 정보, 머지 판정(OnCollisionEnter2D)
├── MergeHandler.cs     # 머지 실행, 다음 단계 과일 스폰
└── UIManager.cs        # 점수 표시, Next 과일 표시, 게임오버 화면
```

---

## 코딩 규칙

- **언어**: C# / Unity 6.3 API 기준
- **포매터**: CSharpier (`.claude/settings.json` 훅으로 자동 실행)
- 요청되지 않은 기능은 추가하지 않는다
- 200줄을 50줄로 줄일 수 있으면 다시 작성한다
- 기존 코드 스타일을 따른다, 불필요한 리팩토링 금지
- 모호한 부분은 구현 전 질문한다

---

## Unity MCP 연동

Claude Code ↔ Unity MCP 서버 연결 상태에서 작업.
`Unity_RunCommand` 로 에디터에서 직접 C# 실행 가능.
`Unity_GetConsoleLogs` 로 에러·경고 실시간 확인 가능.
