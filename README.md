# Coal Ball Cat Service

General-purpose runtime helper utilities for Unity games (PC & mobile). A grab-bag
of battle-tested building blocks you can drop into any project: singletons, object
pooling, a state machine, an event bus, fast math/collection helpers, simple data
persistence, and a handful of UI components.

- **Unity:** 6000.0 (Unity 6) or newer
- **Namespace:** `Coalballcat.Services` (UI components live in `Coalballcat.Services.UI`)
- **License:** MIT (see [LICENSE.md](LICENSE.md))

## Installation

Add the package via **Package Manager → Add package from git URL…**:

```
https://github.com/II-STUDIO/com.coalballcat.service.git
```

### Dependencies

Declared automatically and resolved by the Package Manager:

- `com.unity.ugui` (uGUI + TextMeshPro)
- `com.unity.mathematics`
- `com.unity.nuget.newtonsoft-json` (used by `SaveSystem`)

**Optional / manual:** The `Panel`, `PanelAnimation`, `State`, and `StateMachine`
APIs use [UniTask](https://github.com/Cysharp/UniTask), which is **not** on the
Unity registry. If you use those types, install UniTask first:

```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

> If you don't need async panels/states, you can ignore UniTask — the rest of the
> package does not depend on it.

## What's inside

| Area | Types |
|------|-------|
| **Singleton** | `MonoSingleton<T>`, `NonAutoCreateSingletonAttribute` |
| **Pooling** | `GameObjectPooler`, `MonoPooler<T>`, `GameObjectPoolerGroup`, `MonoPoolerGroup<T>`, `PoolableObject`, `PoolManager` |
| **State** | `StateMachine<T>`, `IStateSync`, `IStateAsync`, `IStateTick`/`FixedTick`/`LateTick` |
| **Events** | `EventBus`, `EventChannel<T>` |
| **Collections** | `IndexBaseQueue<T>` |
| **Fast utils** | `FastMath`, `FastVector2/3`, `FastQuaternion`, `FastTransform`, `FastLoop`, `FastCollections`, `FastCameraScreen`, `FastUI`, `FastMath2D/3D`, `FastCollision` |
| **Data** | `SaveSystem`, `Prefs`, `Cryptor`, `XMLConvert` |
| **Time** | `Timer` |
| **UI** | `SnapScrollRect`, `RadialLayout`, `ResponsiveGridLayoutGroup`, `SlicedFilledImage`, `SelectionSlider`/`LoopableSelectionSlider`, `ImageGradient`, `ColorFader`, `Panel` |
| **Attributes** | `[Layer]` |

## Quick start

### Singleton

```csharp
using Coalballcat.Services;

public class AudioManager : MonoSingleton<AudioManager>
{
    // Survive scene loads:
    protected override bool Persistent => true;

    protected override void OnSingletonAwake() { /* init */ }

    public void Play(AudioClip clip) { /* ... */ }
}

// Anywhere:
AudioManager.Instance.Play(clip);
if (AudioManager.HasInstance) { /* ... */ }
```

If a type should *not* be auto-created when missing, mark it with
`[NonAutoCreateSingleton]`.

### Object pooling

```csharp
// Plain GameObjects (particles, decals, …)
var pool = new GameObjectPooler(prefab, preSpawnCount: 16);
GameObject go = pool.Pool(position, rotation);
pool.Release(go);

// Typed components that need lifecycle hooks
public class Bullet : PoolableObject
{
    public override void OnPoolGet()     { /* reset */ }
    public override void OnPoolRelease() { /* cleanup */ }
}

var bullets = new MonoPooler<Bullet>(bulletPrefab, preSpawnCount: 64);
Bullet b = bullets.Pool(muzzle.position);
b.ReturnPool(); // returns itself to its pool
```

### State machine

```csharp
enum PlayerStateType { Idle, Run }

var sm = new StateMachine<PlayerStateType>();
sm.Register(PlayerStateType.Idle, new IdleState());
sm.Register(PlayerStateType.Run,  new RunState());
sm.SetInitialState(PlayerStateType.Idle);

void Update() => sm.Tick();
sm.ChangeState(PlayerStateType.Run);
```

### Event bus

```csharp
struct PlayerDied { public int score; }

EventBus.On<PlayerDied>(e => Debug.Log(e.score));
EventBus.Emit(new PlayerDied { score = 100 });
EventBus.Off<PlayerDied>(handler);
```

### Data persistence

```csharp
Prefs.SaveIntData("coins", 250);
int coins = Prefs.FindIntData("coins");
Prefs.Save();

string cipher = Cryptor.Encrypt(json, "pass-phrase");
string json   = Cryptor.Decrypt(cipher, "pass-phrase");
```

### Tamper-evident file saves (PC & mobile)

`SaveSystem` writes to `Application.persistentDataPath`, signs every file with an
HMAC (so player edits are detected), and can optionally AES-encrypt it.

```csharp
[System.Serializable]
public class PlayerData
{
    public int level;
    public int coins;
}

// Save (signed; pass a password to also encrypt)
SaveSystem.Save("player.dat", new PlayerData { level = 5, coins = 250 });
SaveSystem.Save("player.dat", data, encryptionPassword: "secret"); // signed + encrypted

// Load — TryLoad returns false if the file is missing, corrupted, or tampered with
if (SaveSystem.TryLoad("player.dat", out PlayerData loaded))
    Debug.Log(loaded.coins);

// Or with a fallback
PlayerData d = SaveSystem.Load("player.dat", fallback: new PlayerData());

SaveSystem.Exists("player.dat");
SaveSystem.Delete("player.dat");
```

> Payloads are serialized with **Newtonsoft.Json**, so dictionaries, lists/arrays at
> the top level, properties, and polymorphic types all work. On-device saves are
> tamper-*evident*, not tamper-*proof* — don't rely on them for anything
> security-critical.

## Notes & caveats

- **Legacy input:** `FastCameraScreen` mouse helpers are compiled only when the
  legacy Input Manager is enabled (`ENABLE_LEGACY_INPUT_MANAGER`). The
  screen-position overloads always work — pass a pointer position from the new
  Input System yourself.
- **Cryptor** is convenience obfuscation for local save data, not a substitute for
  server-side validation. The 1.1.0 ciphertext format is not compatible with 1.0.x.

## License

MIT — see [LICENSE.md](LICENSE.md). All bundled code is original to Coal Ball Cat
Studio or derived from MIT/Unity-licensed sources.
