using System;
using System.IO;
using Deucarian.TemplateGameIdleAutoDefense;
using UnityEngine;

namespace Moss.IdleAutoDefense
{
    public sealed class MossIdleAutoDefenseGameBootstrap : IdleAutoDefenseTemplateController
    {
        private const string PresentationRootName = "Moss Gameplay Presentation";
        private const string PresentationAudioName = "Moss Feedback Audio";
        private const string CorePulseName = "Moss Core Upgrade Pulse";
        private const string SpawnPulseName = "Moss Enemy Spawn Spores";
        private const string KillPulseName = "Moss Kill Spore Burst";
        private const string HitPulseName = "Moss Objective Damage Spores";

        private const float ArenaRadius = 8.5f;
        private const float HudPulseSeconds = 1.2f;

        private string _lastSaveMessage = "Moss save has not been written yet.";
        private GameObject _presentationRoot;
        private Material _groundMaterial;
        private Material _mossMaterial;
        private Material _coreMaterial;
        private Material _dangerMaterial;
        private Material _weaponMaterial;
        private ParticleSystem _spawnPulse;
        private ParticleSystem _killPulse;
        private ParticleSystem _upgradePulse;
        private ParticleSystem _objectiveHitPulse;
        private AudioSource _audioSource;
        private AudioClip _spawnClip;
        private AudioClip _fireClip;
        private AudioClip _killClip;
        private AudioClip _upgradeClip;
        private AudioClip _objectiveHitClip;
        private int _lastSpawnedCount;
        private int _lastProjectileCount;
        private int _lastKillCount;
        private int _lastUpgradeCount;
        private int _lastObjectiveHitCount;
        private float _runSeconds;
        private float _lastSpawnPulseTime = -999f;
        private float _lastFirePulseTime = -999f;
        private float _lastKillPulseTime = -999f;
        private float _lastUpgradePulseTime = -999f;
        private float _lastObjectiveHitPulseTime = -999f;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _smallStyle;
        private GUIStyle _buttonStyle;

        private void Start()
        {
            EnsurePresentationScene();
            WriteMossSnapshot("play-started");
        }

        private void LateUpdate()
        {
            EnsurePresentationScene();
            _runSeconds += Time.deltaTime;
            RefreshPresentationFeedback();
            AnimatePresentation();
        }

        private void OnGUI()
        {
            EnsureGuiStyles();

            GUILayout.BeginArea(new Rect(14f, 14f, 392f, 312f), GUI.skin.box);
            GUILayout.Label("Moss Idle Auto Defense", _titleStyle);
            GUILayout.Label("Core: " + ResolveCoreStatus() + "    Run: " + FormatSeconds(_runSeconds), _labelStyle);
            DrawProgressBar("Wave pressure", CalculatePressure01(), new Color(0.37f, 0.84f, 0.41f));
            DrawProgressBar("Upgrade bloom", Mathf.Clamp01((SelectedUpgradeCount % 4) / 4f + RecentPulse01(_lastUpgradePulseTime) * 0.25f), new Color(0.95f, 0.82f, 0.36f));

            GUILayout.Space(4f);
            GUILayout.Label("Spawned " + SpawnedCount + "   Kills " + TotalKills + "   Shots " + ProjectileLaunchCount + "   Upgrades " + SelectedUpgradeCount, _labelStyle);
            GUILayout.Label("Credits " + (OfflineRewardCredits + EncounterRewardCredits) + "   Parts " + (OfflineRewardParts + EncounterRewardParts), _labelStyle);
            GUILayout.Label(UsingAssignedContentPack ? "Starter pack online" : "Fallback content warning", UsingAssignedContentPack ? _smallStyle : _labelStyle);
            GUILayout.Label(UsingAssignedContentSet ? "Moss run set active" : "No authored set active", UsingAssignedContentSet ? _smallStyle : _labelStyle);

            GUILayout.Space(8f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save", _buttonStyle))
                WriteMossSnapshot("manual");
            if (GUILayout.Button("Restart", _buttonStyle))
            {
                RestartRun();
                ResetPresentationCounters();
                _lastSaveMessage = "run restarted";
            }
            if (GUILayout.Button("Reset Save", _buttonStyle))
                ResetMossSave();
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);
            GUILayout.Label("First 2 minutes: hold the core while moss defenses auto-fire and bloom upgrades.", _smallStyle);
            GUILayout.Label("Save: " + _lastSaveMessage, _smallStyle);
            GUILayout.EndArea();
        }

        private int TotalKills => DirectOrCombatKillCount + ProjectileAdapterKillCount;

        private void EnsurePresentationScene()
        {
            if (_presentationRoot != null) return;

            _presentationRoot = new GameObject(PresentationRootName);
            _groundMaterial = CreateMaterial("Moss ground", new Color(0.08f, 0.12f, 0.1f));
            _mossMaterial = CreateMaterial("Moss lane", new Color(0.18f, 0.55f, 0.23f));
            _coreMaterial = CreateMaterial("Moss core", new Color(0.12f, 0.9f, 0.64f));
            _dangerMaterial = CreateMaterial("Moss danger", new Color(0.95f, 0.2f, 0.18f));
            _weaponMaterial = CreateMaterial("Moss weapon", new Color(0.85f, 0.72f, 0.28f));

            CreatePrimitive("Moss Keyboard Arena", PrimitiveType.Cylinder, Vector3.zero, new Vector3(ArenaRadius * 2f, 0.08f, ArenaRadius * 2f), _groundMaterial);
            CreatePrimitive("Moss Core - Defended Objective", PrimitiveType.Sphere, new Vector3(0f, 0.45f, 0f), new Vector3(1.35f, 0.9f, 1.35f), _coreMaterial);
            CreatePrimitive("Moss Growth Ring", PrimitiveType.Cylinder, Vector3.zero, new Vector3(2.4f, 0.05f, 2.4f), _mossMaterial);

            CreateWeaponPad("Spore Wand Pad", new Vector3(-1.65f, 0.18f, 1.25f));
            CreateWeaponPad("Cursor Beam Pad", new Vector3(1.65f, 0.18f, 1.25f));
            CreateWeaponPad("Moss Seeker Pad", new Vector3(-1.65f, 0.18f, -1.25f));
            CreateWeaponPad("Sticky Bloom Pad", new Vector3(1.65f, 0.18f, -1.25f));

            CreateSpawnZone("North Spawn Sporeline", new Vector3(0f, 0.1f, ArenaRadius - 0.8f), new Vector3(3.2f, 0.05f, 0.45f));
            CreateSpawnZone("East Spawn Sporeline", new Vector3(ArenaRadius - 0.8f, 0.1f, 0f), new Vector3(0.45f, 0.05f, 3.2f));
            CreateSpawnZone("South Spawn Sporeline", new Vector3(0f, 0.1f, -ArenaRadius + 0.8f), new Vector3(3.2f, 0.05f, 0.45f));
            CreateSpawnZone("West Spawn Sporeline", new Vector3(-ArenaRadius + 0.8f, 0.1f, 0f), new Vector3(0.45f, 0.05f, 3.2f));

            for (int i = 0; i < 10; i++)
            {
                float angle = i * 36f * Mathf.Deg2Rad;
                Vector3 position = new Vector3(Mathf.Cos(angle) * 4.8f, 0.16f, Mathf.Sin(angle) * 4.8f);
                Vector3 scale = i % 2 == 0 ? new Vector3(0.9f, 0.12f, 0.55f) : new Vector3(0.55f, 0.12f, 0.9f);
                CreatePrimitive("Low Keyboard Prop " + (i + 1).ToString(), PrimitiveType.Cube, position, scale, _mossMaterial);
            }

            _spawnPulse = CreatePulse(SpawnPulseName, new Color(0.42f, 1f, 0.45f), 0.3f, 2.2f, 0.7f);
            _killPulse = CreatePulse(KillPulseName, new Color(0.9f, 1f, 0.35f), 0.2f, 3.0f, 0.55f);
            _upgradePulse = CreatePulse(CorePulseName, new Color(0.98f, 0.82f, 0.28f), 0.28f, 1.7f, 0.9f);
            _objectiveHitPulse = CreatePulse(HitPulseName, new Color(1f, 0.22f, 0.12f), 0.22f, 2.4f, 0.55f);

            GameObject audioObject = new GameObject(PresentationAudioName);
            audioObject.transform.SetParent(_presentationRoot.transform, false);
            _audioSource = audioObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
            _audioSource.volume = 0.32f;
            _spawnClip = CreateTone("moss-spawn-cue", 180f, 0.12f, 0.18f);
            _fireClip = CreateTone("moss-fire-cue", 480f, 0.08f, 0.16f);
            _killClip = CreateTone("moss-kill-cue", 700f, 0.1f, 0.2f);
            _upgradeClip = CreateTone("moss-upgrade-cue", 920f, 0.18f, 0.22f);
            _objectiveHitClip = CreateTone("moss-core-hit-cue", 120f, 0.18f, 0.2f);

            ConfigureCamera();
            ResetPresentationCounters();
        }

        private void RefreshPresentationFeedback()
        {
            int totalKills = TotalKills;
            if (SpawnedCount > _lastSpawnedCount)
            {
                PlayPulse(_spawnPulse, OrbitPosition(SpawnedCount, ArenaRadius - 1.1f), 18, _spawnClip);
                _lastSpawnPulseTime = Time.time;
            }

            if (ProjectileLaunchCount > _lastProjectileCount)
            {
                PlayPulse(_upgradePulse, OrbitPosition(ProjectileLaunchCount, 1.65f), 6, _fireClip);
                _lastFirePulseTime = Time.time;
            }

            if (totalKills > _lastKillCount)
            {
                PlayPulse(_killPulse, OrbitPosition(totalKills + 9, 4.4f), 22, _killClip);
                _lastKillPulseTime = Time.time;
            }

            if (SelectedUpgradeCount > _lastUpgradeCount)
            {
                PlayPulse(_upgradePulse, new Vector3(0f, 0.7f, 0f), 34, _upgradeClip);
                _lastUpgradePulseTime = Time.time;
            }

            if (ObjectiveDamageEvents > _lastObjectiveHitCount)
            {
                PlayPulse(_objectiveHitPulse, new Vector3(0f, 0.65f, 0f), 24, _objectiveHitClip);
                _lastObjectiveHitPulseTime = Time.time;
            }

            _lastSpawnedCount = SpawnedCount;
            _lastProjectileCount = ProjectileLaunchCount;
            _lastKillCount = totalKills;
            _lastUpgradeCount = SelectedUpgradeCount;
            _lastObjectiveHitCount = ObjectiveDamageEvents;
        }

        private void AnimatePresentation()
        {
            if (_presentationRoot == null) return;
            float pulse = 1f + RecentPulse01(_lastUpgradePulseTime) * 0.08f - RecentPulse01(_lastObjectiveHitPulseTime) * 0.05f;
            Transform core = _presentationRoot.transform.Find("Moss Core - Defended Objective");
            if (core != null)
                core.localScale = new Vector3(1.35f * pulse, 0.9f * pulse, 1.35f * pulse);
        }

        private void ResetPresentationCounters()
        {
            _lastSpawnedCount = SpawnedCount;
            _lastProjectileCount = ProjectileLaunchCount;
            _lastKillCount = TotalKills;
            _lastUpgradeCount = SelectedUpgradeCount;
            _lastObjectiveHitCount = ObjectiveDamageEvents;
            _runSeconds = 0f;
        }

        private void CreateWeaponPad(string name, Vector3 position)
        {
            CreatePrimitive(name, PrimitiveType.Cylinder, position, new Vector3(0.9f, 0.16f, 0.9f), _weaponMaterial);
            CreatePrimitive(name + " Sprout", PrimitiveType.Sphere, position + new Vector3(0f, 0.36f, 0f), new Vector3(0.38f, 0.46f, 0.38f), _coreMaterial);
        }

        private void CreateSpawnZone(string name, Vector3 position, Vector3 scale)
        {
            CreatePrimitive(name, PrimitiveType.Cube, position, scale, _dangerMaterial);
        }

        private GameObject CreatePrimitive(string name, PrimitiveType primitiveType, Vector3 position, Vector3 scale, Material material)
        {
            GameObject instance = GameObject.CreatePrimitive(primitiveType);
            instance.name = name;
            instance.transform.SetParent(_presentationRoot.transform, false);
            instance.transform.localPosition = position;
            instance.transform.localScale = scale;
            Renderer renderer = instance.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;
            Collider collider = instance.GetComponent<Collider>();
            if (collider != null)
                collider.enabled = false;
            return instance;
        }

        private ParticleSystem CreatePulse(string name, Color color, float startSize, float startSpeed, float lifetime)
        {
            GameObject instance = new GameObject(name);
            instance.transform.SetParent(_presentationRoot.transform, false);
            ParticleSystem particles = instance.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particles.main;
            main.loop = false;
            main.playOnAwake = false;
            main.startLifetime = lifetime;
            main.startSpeed = startSpeed;
            main.startSize = startSize;
            main.startColor = color;
            main.maxParticles = 90;
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = false;
            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.35f;
            return particles;
        }

        private static Material CreateMaterial(string name, Color color)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = name;
            material.color = color;
            return material;
        }

        private static AudioClip CreateTone(string name, float frequency, float durationSeconds, float volume)
        {
            const int sampleRate = 22050;
            int sampleCount = Mathf.Max(1, Mathf.CeilToInt(sampleRate * durationSeconds));
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float fade = Mathf.Clamp01(1f - i / (float)sampleCount);
                samples[i] = Mathf.Sin(Mathf.PI * 2f * frequency * t) * volume * fade;
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private void PlayPulse(ParticleSystem particles, Vector3 position, int count, AudioClip clip)
        {
            if (particles != null)
            {
                particles.transform.position = position;
                particles.Emit(count);
            }

            if (_audioSource != null && clip != null)
                _audioSource.PlayOneShot(clip);
        }

        private static Vector3 OrbitPosition(int seed, float radius)
        {
            float angle = seed * 137.5f * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angle) * radius, 0.55f, Mathf.Sin(angle) * radius);
        }

        private void ConfigureCamera()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }

            camera.transform.position = new Vector3(0f, 9.5f, -9.5f);
            camera.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
            camera.orthographic = true;
            camera.orthographicSize = 7.3f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.025f, 0.035f, 0.032f);
        }

        private void EnsureGuiStyles()
        {
            if (_titleStyle != null) return;
            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, normal = { textColor = new Color(0.8f, 1f, 0.74f) } };
            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, normal = { textColor = Color.white } };
            _smallStyle = new GUIStyle(GUI.skin.label) { fontSize = 11, normal = { textColor = new Color(0.78f, 0.9f, 0.78f) }, wordWrap = true };
            _buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 12, fixedHeight = 28f };
        }

        private void DrawProgressBar(string label, float value, Color fill)
        {
            Rect rect = GUILayoutUtility.GetRect(340f, 18f);
            GUI.Box(rect, GUIContent.none);
            Rect fillRect = new Rect(rect.x + 2f, rect.y + 2f, Mathf.Max(0f, rect.width - 4f) * Mathf.Clamp01(value), rect.height - 4f);
            Color oldColor = GUI.color;
            GUI.color = fill;
            GUI.DrawTexture(fillRect, Texture2D.whiteTexture);
            GUI.color = oldColor;
            GUI.Label(rect, label + "  " + Mathf.RoundToInt(Mathf.Clamp01(value) * 100f).ToString() + "%", _smallStyle);
        }

        private string ResolveCoreStatus()
        {
            if (EncounterCompleted) return "Secured";
            if (EncounterFailed) return "Overrun";
            if (RecentPulse01(_lastObjectiveHitPulseTime) > 0f) return "Under attack";
            if (ObjectiveDamageEvents > 0) return "Holding";
            return "Stable";
        }

        private float CalculatePressure01()
        {
            float spawnPressure = Mathf.Clamp01(SpawnedCount / 24f);
            float hitPressure = Mathf.Clamp01(ObjectiveDamageEvents / 6f);
            float eventPressure = Mathf.Max(RecentPulse01(_lastSpawnPulseTime), RecentPulse01(_lastFirePulseTime) * 0.5f, RecentPulse01(_lastKillPulseTime) * 0.4f);
            return Mathf.Clamp01(Mathf.Max(spawnPressure, hitPressure, eventPressure));
        }

        private float RecentPulse01(float lastTime)
        {
            if (lastTime < 0f) return 0f;
            return Mathf.Clamp01(1f - (Time.time - lastTime) / HudPulseSeconds);
        }

        private static string FormatSeconds(float seconds)
        {
            int total = Mathf.Max(0, Mathf.FloorToInt(seconds));
            return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
        }

        private void WriteMossSnapshot(string reason)
        {
            MossIdleAutoDefenseSave.WriteSnapshot(reason, this);
            _lastSaveMessage = "written to " + MossIdleAutoDefenseSave.SaveFilePath;
        }

        private void ResetMossSave()
        {
            bool deleted = MossIdleAutoDefenseSave.Reset();
            _lastSaveMessage = deleted ? "reset complete" : "nothing to reset";
            Debug.Log("[Moss Idle Auto Defense] " + _lastSaveMessage + ".");
        }
    }

    public static class MossIdleAutoDefenseSave
    {
        private const string SampleFileName = "sample-state.json";

        public static string SaveDirectoryPath => Path.Combine(Application.persistentDataPath, "Moss", "IdleAutoDefense");
        public static string SaveFilePath => Path.Combine(SaveDirectoryPath, SampleFileName);
        public static bool HasSave => File.Exists(SaveFilePath);

        public static void WriteSnapshot(string reason, IdleAutoDefenseTemplateController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            Directory.CreateDirectory(SaveDirectoryPath);
            File.WriteAllText(SaveFilePath, CreateSnapshotJson(reason, controller));
        }

        public static bool Reset()
        {
            bool existed = Directory.Exists(SaveDirectoryPath);
            if (existed)
                Directory.Delete(SaveDirectoryPath, true);
            return existed;
        }

        private static string CreateSnapshotJson(string reason, IdleAutoDefenseTemplateController controller)
        {
            return "{\n" +
                   "  \"game\": \"Moss Idle Auto Defense\",\n" +
                   "  \"reason\": \"" + Escape(reason) + "\",\n" +
                   "  \"savedUtc\": \"" + DateTimeOffset.UtcNow.ToString("O") + "\",\n" +
                   "  \"runtimeState\": \"" + controller.RuntimeStateName + "\",\n" +
                   "  \"spawned\": " + controller.SpawnedCount + ",\n" +
                   "  \"kills\": " + (controller.DirectOrCombatKillCount + controller.ProjectileAdapterKillCount) + ",\n" +
                   "  \"projectileLaunches\": " + controller.ProjectileLaunchCount + ",\n" +
                   "  \"selectedUpgrades\": " + controller.SelectedUpgradeCount + ",\n" +
                   "  \"objectiveHits\": " + controller.ObjectiveDamageEvents + "\n" +
                   "}\n";
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
