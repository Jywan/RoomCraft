# RoomCraft - 3D 가구 배치 시뮬레이터

> "내 가구가 이 방에 들어갈까?"를 3D로 확인하는 도구

**플레이**: [jywan.github.io/RoomCraft](https://jywan.github.io/RoomCraft/) (WebGL, PC 브라우저 권장)
> WebGL 빌드는 Unity의 한계로 한글 입력 시 자모가 조합되지 않고 낱자로 입력됩니다(예: "방" → "ㅂㅏㅇ"). 방/가구 이름은 영어로 입력해주세요.
> 저장/불러오기는 같은 브라우저 세션 안에서만 유지됩니다. 새로고침하거나 다시 접속하면 저장된 프로젝트가 사라집니다.

## 소개

실제 가구 치수(cm)를 입력해서 방 안에 배치해보는 3D 시뮬레이터입니다.
이사나 인테리어 전에 가구 배치를 미리 확인할 수 있습니다.

## 주요 기능

- **방 생성**: 가로 × 세로 × 높이(m) 입력으로 방 생성
- **가구 생성**: 카테고리별 기본 형태 + 사용자 치수(cm) 입력
- **드래그 배치**: 가구를 드래그로 방 안에 배치, 회전, 이동
- **시점 전환**: 탑뷰(평면도) ↔ 3D 자유시점 ↔ 1인칭 뷰
- **저장/관리**: 프로젝트 저장/불러오기
- **가구 프리셋**: 자주 쓰는 가구를 즐겨찾기처럼 저장해두고 다른 프로젝트에서도 바로 재사용

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
| T 키 | 탑뷰 → 3D 자유시점 → 1인칭 순환 전환 |
| 좌클릭 드래그 (자유시점/1인칭) | 카메라 시점 회전 |
| W / A / S / D (1인칭) | 이동 (벽 통과 불가, 가구는 통과됨) |
| 스크롤 (트랙패드 두 손가락, 탑뷰/자유시점) | 줌 인/아웃 |
| Q 키 | 선택된 가구 -45도 회전 |
| E / R 키 | 선택된 가구 +45도 회전 |
| Delete / Backspace | 선택된 가구 삭제 |
| G 키 | 그리드 스냅 ON/OFF 토글 |

## 프로젝트 구조

<details>
<summary>펼치기</summary>

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

</details>

## 개발 진행 상황

<details>
<summary>펼치기</summary>

- [x] Phase 1-1: 방 생성 시스템 (바닥 + 벽)
- [x] Phase 1-2: 카메라 시스템 (탑뷰 ↔ 3D 전환, 줌)
- [x] Phase 2-1: 가구 드래그 앤 드롭, 회전, 삭제
- [x] Phase 2-2: 충돌 감지 (벽 밖 배치 불가, 가구 겹침 경고)
- [x] Phase 3-1: 가구 생성 UI (카테고리/치수/이름/색상 입력)
- [x] Phase 3-2: 동적 모델 생성 (카테고리별 형태)
- [x] Phase 4-1: 프로젝트 저장/불러오기 (JSON 직렬화)
- [x] Phase 4-2: 그리드 스냅 + 치수 표시
- [x] Phase 4-3: 방 커스터마이징 UI (방 크기 입력)
- [x] Phase 4-4: 씬 분리 구조 (StartScene + EditorScene, 저장/불러오기 UI, 에디터 툴바)
- [x] Phase 4-5: 룸/가구 툴바 분리 + 색상 피커 (원형 휠 + RGB 직접 입력)
- [x] Phase 4-6: 가구 정보 패널 통합 (치수/미리보기/회전 슬라이더) + 버그 수정
- [x] Phase 5-1: 다각형 기반 방 생성 (꼭짓점 리스트로 바닥/벽 생성)
- [x] Phase 5-2: 프리셋 방 모양 선택 (사각형/L자형 등)
- [x] Phase 5-3: 커스텀 방 모양 편집 (점 클릭으로 다각형 직접 그리기)
- [x] Phase 6-1: WebGL 배포 (GitHub Pages)
- [ ] Phase 6-2: 모바일(Android/iOS) 빌드 배포
- [x] Phase 7: 1인칭 시점 추가 (WASD 이동, 마우스 시점 회전, 벽 충돌 처리)
- [x] Phase 8: 가구 프리셋 시스템 (전역 저장, 다른 프로젝트에서도 재사용)

</details>

## 개발 중 이슈 & 해결

<details>
<summary>펼치기</summary>

| 이슈 | 원인 | 해결 |
|------|------|------|
| `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package` | New Input System 패키지 설치 시 레거시 Input API가 비활성화됨 | **Project Settings > Player > Other Settings > Active Input Handling**을 **Both**로 변경 |
| Rider에서 자동완성(IntelliSense) 안 됨 | Rider가 .sln 없이 파일만 열린 상태 | Unity에서 스크립트 더블클릭으로 열거나, Rider에서 `RoomCraft_Unity.sln`을 직접 Open |
| 맥북 트랙패드에서 우클릭 드래그 불편 | 트랙패드로 우클릭 드래그가 어려움 | 좌클릭 드래그로 카메라 회전 변경 |
| 카메라 회전 감도가 너무 낮음 | `Time.deltaTime` 곱셈으로 입력값이 지나치게 작아짐 | 고정 배수(`0.1f`)로 변경 |
| UI 입력 중 게임 단축키 반응 | InputField 포커스 중에도 T/R키 등이 게임에 전달됨 | `EventSystem.currentSelectedGameObject` 체크로 무시 |
| 가구 생성 시 기존 가구 사라짐 | 같은 위치에 Collider가 겹치면 물리엔진이 오브젝트를 밀어냄 | Collider를 `isTrigger`로 변경 + Queries Hit Triggers 활성화 |
| 가구가 벽 속으로 파고듦 | IsInsideRoom에서 벽 두께 미보정 + SetRoomSize에 height 전달 | 벽 두께 보정 + depth로 수정 + 헤드보드/등받이 위치 조정 |
| Layout Group 중첩 시 자식 배치가 깨짐 | Horizontal/Vertical Layout Group은 자식을 "배치"만 하지, 자기 자신의 크기를 내용물에 맞춰 키워주진 않음 | Layout Group이 붙은 컨테이너가 또 다른 Layout Group의 자식이면, Content Size Fitter(Preferred Size)를 같이 붙여야 함 |
| Slider의 Direction 필드가 안 보임 | Handle Rect가 비어있으면 Unity가 Direction/Min/Max 필드를 숨김 | Handle Rect에 실제 Handle 오브젝트 연결 |
| Slider Handle의 Rect Transform 앵커를 못 건드림 | Handle Rect로 지정된 오브젝트는 Slider가 앵커 전체를 driven 상태로 잠금 | 부모(Handle Slide Area)의 크기를 대신 조절해서 우회 |
| 스크립트 리네임 후 씬 연결이 끊길 뻔함 | Finder 등에서 파일명만 바꾸면 `.meta`의 GUID 매칭이 깨짐 | 반드시 Unity 프로젝트 창 안에서 Rename |
| 방 크기를 바꿔도 가구가 예전 경계 기준으로 빨개짐 | `FurnitureBounds.SetRoomSize()`가 어디서도 호출되지 않아서 인스펙터 기본값(4×3)으로 계속 검사됨 | `EditorSceneLoader`가 방 생성/복원 직후 `SetRoomSize()` 호출하도록 수정 |
| 3D 뷰에서 가구 드래그 시 카메라도 같이 회전 | `CameraController`가 좌클릭을 UI 위 클릭인지만 체크하고, 가구(3D 오브젝트) 클릭인지는 체크 안 함 | 드래그 시작 전 Furniture 레이어로 Raycast해서 가구 위 클릭이면 카메라 드래그 무시 |
| 가구 선택 즉시 정보 텍스트가 한 프레임 안 보임 | `Content Size Fitter`가 텍스트 반영 전 크기를 계산해서 0으로 나옴 | 텍스트 설정 직후 `LayoutRebuilder.ForceRebuildLayoutImmediate()` 호출 |
| `PolygonUtils.cs`에서 `.x`/`.y` 컴파일 에러 | `using System.Numerics;`로 잘못 임포트되어 `Vector2`가 Unity 타입이 아닌 .NET 타입(필드명 `X`/`Y`)으로 잡힘 | `using UnityEngine;`으로 수정 |
| 다각형 바닥이 마젠타(핑크)로 보임 | `Floor Material`/`Wall Material`이 애초에 할당 안 되어 있었는데, 기존엔 `CreatePrimitive`가 자동으로 기본 머티리얼을 붙여줘서 안 드러났음. 커스텀 Mesh 방식은 그 자동 할당이 없어서 "머티리얼 없음"이 그대로 드러남 | URP Lit 머티리얼 새로 만들어서 `Floor Material`/`Wall Material`에 연결 |
| 바닥과 가구가 겹치는 부분에서 깜빡임(Z-파이팅) | 바닥 메쉬와 가구 밑면이 둘 다 Y=0에 정확히 겹쳐서 렌더링 순서가 매 프레임 흔들림 | 바닥을 Y=-0.01로 살짝 내려서 겹침 회피 |
| TMP Dropdown 목록 글자만 한글 깨짐 | Dropdown의 Template(펼침 목록) 쪽 Item Label이 한글 미지원 기본 폰트를 사용 | Item Label의 Font Asset을 NotoSansKR-Regular SDF로 교체 |
| 방 모양 Dropdown이 계속 기본값(Option A/B/C)으로 보임 | `SetupShapeDropdown()`을 작성만 하고 `Start()`에서 호출을 안 함 | `Start()`에 호출 추가 |
| 방 모양 정보 없을 때 가구가 항상 경계 밖으로 판정됨 | `FurnitureBounds.IsInsideRoom()`이 `roomOutline == null`일 때 `false`를 반환(의도는 `true`) | `true`로 수정 |
| 탭 구조로 재구성 후 `NewProjectPanel` 높이가 0으로 찌부러짐 | 입력 필드를 `NormalTabContent`(Layout Group 없는 컨테이너)로 재부모했는데, `NewProjectPanel`에 예전부터 붙어있던 `Vertical Layout Group`+`Content Size Fitter`가 새 자식의 선호 크기를 0으로 계산 | 두 컴포넌트 제거 후 Height 수동 복구 |
| 커스텀 방 그리기에서 방 크기가 크면 그리드 점이 패널 밖으로 넘침 | `GenerateGrid()`가 점 간격을 고정값(`cellPixelSize`)으로 써서 입력값이 클수록 전체 그리드가 `GridArea` 크기를 초과 | `GridArea` 실제 크기에 맞춰 간격을 동적으로 축소하는 `effectiveCellPx` 계산 추가 |
| 커스텀 탭을 처음 열 때 첫 클릭이 무반응, 두 번째 클릭에야 패널이 뜸 | `CustomRoomPanel`이 씬에 비활성 상태로 저장되어 있어 `Awake()`가 실행된 적이 없었고, 첫 클릭의 `SetActive(true)`가 오브젝트를 처음 활성화시키는 순간이라 Unity가 그 자리에서 `Awake()`를 실행 → `Awake()` 마지막 줄의 `SetActive(false)`가 방금 켠 걸 바로 꺼버림 | `CustomRoomPanel`을 씬에서 기본 활성화 상태로 저장 (런타임에 스스로 숨기는 방식으로 변경) |
| 커스텀 탭 열면 뒤의 메인 화면/탭 버튼이 비쳐 보임 | `CustomRoomPanel`이 형제 오브젝트보다 먼저 그려지고 배경 알파도 낮음(0.78) | 탭 전환 시 `SetAsLastSibling()`으로 맨 앞에 그리기 + 배경 알파 1로 조정 |
| 성능 최적화 중 방 경계 검사가 모서리 2개만 검사하게 됨 | `IsInsideRoom()`에서 매 프레임 배열 할당을 없애려고 재사용 버퍼(`cornerBuffer`)로 바꾸는 과정에서 복사 실수로 인덱스 2·3이 각각 인덱스 1·0과 동일한 값(`bounds.min.z`)을 사용 → 가구의 반대쪽 모서리 2개가 사실상 검사되지 않음 | 인덱스 2·3을 `bounds.max.z` 기준으로 수정 |
| GitHub Pages 배포 후 게임이 404로 안 뜸 (`build.loader.js` 로드 실패) | `.gitignore`의 `[Bb]uild/` 규칙이 Unity 자체 빌드 폴더뿐 아니라 경로 어디에 있든 이름이 Build/build인 폴더를 전부 무시함 — `docs/Build/`도 여기 걸려서 커밋 자체가 안 됨 | `.gitignore`에 `!docs/Build/` 예외 규칙 추가 |
| 1인칭에서 T키를 눌러도 영원히 못 빠져나옴 | `ToggleView()`의 3단 순환 조건 순서가 꼬여서 `FirstPerson` 상태일 때 `SwitchMode(FirstPerson)`(자기 자신)을 호출 → `SwitchMode()`의 "같은 모드면 무시" 가드에 걸려 아무 반응 없음 | `FreeView → FirstPerson`, `FirstPerson → TopView`로 분기 순서 수정 |
| `CharacterController` 연결해도 1인칭에서 계속 벽을 통과함 | Main Camera에 `Character Controller` 컴포넌트 자체를 추가하지 않은 채(또는 씬 저장 없이) 코드만 반영되어, `characterController != null` 체크가 조용히 충돌 없는 폴백 경로로 빠짐 | 컴포넌트 실제 추가 + 인스펙터 슬롯 연결 + 씬 저장 |
| `GridSnap` 캐싱 코드가 컴파일 에러 | 이전에 제안한 `gridSnap` 필드 캐싱을 적용할 때 필드 선언 없이 `Start()`의 대입문만 반영됨 | 필드 선언 추가, `HandleDrag()`가 실제로 캐싱된 필드를 쓰도록 완성 |

</details>

## 추후 최적화 예정

| 항목 | 내용 |
|------|------|
| 저장/불러오기 UI | EditorToolbar의 저장 버튼 → EditorSaveUI 팝업으로 동작. 불러오기는 StartScene에서 처리 |
