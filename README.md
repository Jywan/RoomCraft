# RoomCraft - 3D 가구 배치 시뮬레이터

> "내 가구가 이 방에 들어갈까?"를 3D로 확인하는 도구

## 소개

실제 가구 치수(cm)를 입력해서 방 안에 배치해보는 3D 시뮬레이터입니다.
이사나 인테리어 전에 가구 배치를 미리 확인할 수 있습니다.

## 주요 기능

- **방 생성**: 가로 × 세로 × 높이(m) 입력으로 방 생성
- **가구 생성**: 카테고리별 기본 형태 + 사용자 치수(cm) 입력
- **드래그 배치**: 가구를 드래그로 방 안에 배치, 회전, 이동
- **시점 전환**: 탑뷰(평면도) ↔ 3D 자유시점 ↔ 1인칭 뷰
- **저장/관리**: 여러 레이아웃 비교, 가구 목록 재사용

## 기술 스택

| 구분 | 기술 |
|------|------|
| 엔진 | Unity 6 (6000.3.x) |
| 언어 | C# |
| 렌더링 | URP (Universal Render Pipeline) |
| 3D 모델링 | ProBuilder |
| 카메라 | Cinemachine |
| 입력 처리 | New Input System |
| 데이터 저장 | JSON |

## 플랫폼

- WebGL (웹 배포)
- Android / iOS

## 조작법

| 입력 | 동작 |
|------|------|
| T 키 | 탑뷰 ↔ 3D 뷰 전환 |
| 좌클릭 드래그 (3D 뷰) | 카메라 회전 |
| 스크롤 (트랙패드 두 손가락) | 줌 인/아웃 |
| R 키 | 선택된 가구 90도 회전 |
| Delete / Backspace | 선택된 가구 삭제 |
| G 키 | 그리드 스냅 ON/OFF 토글 |
| S 키 | 프로젝트 저장 |
| L 키 | 프로젝트 불러오기 |

## 프로젝트 구조

```
RoomCraft_Unity/Assets/
├── Scripts/
│   ├── Room/          ← 방 생성 시스템
│   ├── Furniture/     ← 가구 생성/관리
│   ├── Camera/        ← 카메라 시스템
│   ├── UI/            ← UI 컨트롤러
│   ├── Data/          ← 데이터 모델, JSON 직렬화
│   └── Input/         ← 입력 처리
├── Prefabs/
├── Materials/
├── Scenes/
└── Settings/
```

## 개발 진행 상황

- [x] Phase 1-1: 방 생성 시스템 (바닥 + 벽)
- [x] Phase 1-2: 카메라 시스템 (탑뷰 ↔ 3D 전환, 줌)
- [x] Phase 2-1: 가구 드래그 앤 드롭, 회전, 삭제
- [x] Phase 2-2: 충돌 감지 (벽 밖 배치 불가, 가구 겹침 경고)
- [x] Phase 3-1: 가구 생성 UI (카테고리/치수/이름/색상 입력)
- [x] Phase 3-2: 동적 모델 생성 (카테고리별 형태)
- [x] Phase 4-1: 프로젝트 저장/불러오기 (JSON 직렬화)
- [x] Phase 4-2: 그리드 스냅 + 치수 표시
- [x] Phase 4-3: 방 커스터마이징 UI (방 크기 입력)
- [ ] Phase 4-4: 저장/불러오기 UI (파일 목록 선택)
- [ ] Phase 5: WebGL/모바일 빌드 배포
- [ ] Phase 6: L자/다각형 방 지원 (꼭짓점 기반 벽 생성)

## 개발 중 이슈 & 해결

| 이슈 | 원인 | 해결 |
|------|------|------|
| `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package` | New Input System 패키지 설치 시 레거시 Input API가 비활성화됨 | **Project Settings > Player > Other Settings > Active Input Handling**을 **Both**로 변경 |
| Rider에서 자동완성(IntelliSense) 안 됨 | Rider가 .sln 없이 파일만 열린 상태 | Unity에서 스크립트 더블클릭으로 열거나, Rider에서 `RoomCraft_Unity.sln`을 직접 Open |
| 맥북 트랙패드에서 우클릭 드래그 불편 | 트랙패드로 우클릭 드래그가 어려움 | 좌클릭 드래그로 카메라 회전 변경 |
| 카메라 회전 감도가 너무 낮음 | `Time.deltaTime` 곱셈으로 입력값이 지나치게 작아짐 | 고정 배수(`0.1f`)로 변경 |
| UI 입력 중 게임 단축키 반응 | InputField 포커스 중에도 T/R키 등이 게임에 전달됨 | `EventSystem.currentSelectedGameObject` 체크로 무시 |
| 가구 생성 시 기존 가구 사라짐 | 같은 위치에 Collider가 겹치면 물리엔진이 오브젝트를 밀어냄 | Collider를 `isTrigger`로 변경 + Queries Hit Triggers 활성화 |
| 가구가 벽 속으로 파고듦 | IsInsideRoom에서 벽 두께 미보정 + SetRoomSize에 height 전달 | 벽 두께 보정 + depth로 수정 + 헤드보드/등받이 위치 조정 |

## 추후 최적화 예정

| 항목 | 내용 |
|------|------|
| Unity `== null` 비용 | Unity의 `== null`은 네이티브 오브젝트까지 확인하는 오버로딩 연산자라 비용이 높음. 매 프레임 호출되는 곳은 bool 플래그/캐싱으로 대체 예정 |
| 저장/불러오기 UI | 현재 S/L 키로 테스트 중. 추후 저장 파일 목록 UI + 선택해서 불러오기 기능 추가 예정 |
