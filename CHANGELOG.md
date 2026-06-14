# Changelog

All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2026-06-14

### Added
- `SaveSystem`: cross-platform (PC & mobile) file save/load under
  `Application.persistentDataPath`. Every file is HMAC-SHA256 signed (tamper-evident)
  and can be AES-encrypted with a password. Writes are atomic (temp file + move).
  Payloads are serialized with Newtonsoft.Json (adds the
  `com.unity.nuget.newtonsoft-json` dependency).
- `LICENSE.md` (MIT), this `CHANGELOG.md`, and a real `README.md`.
- Editor assembly definition (`Coalballcat.Services.Editor`) so editor-only code
  no longer leaks into `Assembly-CSharp-Editor`.
- Declared package `dependencies` (`com.unity.ugui`, `com.unity.mathematics`).
- `MonoSingleton`: duplicate-instance destruction, opt-in `DontDestroyOnLoad`
  (`Persistent`), `HasInstance`, `OnSingletonAwake`, and static state reset that
  supports *Enter Play Mode* with Domain Reload disabled.
- `IndexBaseQueue<T>`: `Capacity`, `IsEmpty`, `TryDequeue`, `TryPeek`, and indexer.
- `Prefs`: typed default-value overloads, `Save()`, and `DeleteAll()`.
- `Timer`: working repeat support (`IsRepeated`).

### Changed (breaking)
- Moved previously global types into the `Coalballcat.Services` namespace:
  `NonAutoCreateSingletonAttribute`, `FastCollision`, `EventBus`, `EventChannel<T>`,
  `Timer`, `LayerAttribute`.
- Standardized the UI namespace on `Coalballcat.Services.UI`
  (`Panel`/`PanelAnimation` moved from `Coalballcat.Services.UIs`).
- Renamed misspelled public members: `FastMath.Chane` → `FastMath.Chance`,
  `Pre` → `Prefs`, `IPooler.DisposeWithoutUnitialize` → `DisposeWithoutUninitialize`.
- `MonoSingleton<T>` now uses the self-referencing constraint
  `where T : MonoSingleton<T>`.
- `Cryptor` migrated off the obsolete `RijndaelManaged`/`RNGCryptoServiceProvider`
  to `Aes` + `RandomNumberGenerator` with `Rfc2898DeriveBytes(SHA256)`.
  **The on-disk ciphertext format changed; previously encrypted strings cannot be
  decrypted by this version.**

### Fixed
- `FastMath.FastLerp` now clamps `t` (use `FastLerpUnclamped` for the old behavior).
- Removed `FastCollections.FastForEach(List<T>, Action<T>)`, which was ambiguous
  with `FastLoop.FastForEach(List<T>, ForAction<T>)` and broke lambda call sites.
- `FastCameraScreen` is now a `static` class and guards legacy `Input` usage behind
  `ENABLE_LEGACY_INPUT_MANAGER` so it compiles in new-Input-System-only projects.
- `ResponsiveGridLayoutGroupEditor` no longer hides every inspector field.
- `EventBus` clears its channels on play-mode reset.
- Clean-room rewrites of `ColorFader`, `ImageGradient`, `SelectionSlider`,
  `LoopableSelectionSlider`, and `SlicedFilledImage` (replacing third-party
  Asset-Store-licensed code that could not be redistributed).

### Notes
- `Panel`, `PanelAnimation`, and the `State`/`StateMachine` system require
  [UniTask](https://github.com/Cysharp/UniTask). UniTask is not on the Unity
  registry; install it via Git URL (see README).
