using System.IO;
using System.Linq;
using Tanks.Audio;
using Tanks.CameraSystem;
using Tanks.Combat;
using Tanks.Core;
using Tanks.Input;
using Tanks.Player;
using Tanks.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tanks.EditorTools
{
    /// <summary>Creates all first-prototype assets and a playable range from Tools > Tanks.</summary>
    public static class TanksTestRangeBuilder
    {
        private const string Root = "Assets/_Project";
        private const string InputPath = Root + "/Input/TankControls.inputactions";
        private const string WeaponPath = Root + "/ScriptableObjects/Weapons/StarterCannon.asset";
        private const string ProjectilePath = Root + "/Prefabs/Projectiles/Projectile.prefab";
        private const string EnemyTankPath = Root + "/Prefabs/Tanks/EnemyTank.prefab";
        private const string TankPath = Root + "/Prefabs/Tanks/PlayerAbrams.prefab";
        private const string AbramsModelPath = Root + "/Art/Tanks/AbramsM1/Source/M1A1.fbx";
        private const string AbramsBaseMaterialPath = Root + "/Art/Tanks/AbramsM1/AbramsBase.mat";
        private const string AbramsTrackMaterialPath = Root + "/Art/Tanks/AbramsM1/AbramsTrack.mat";
        private const string AbramsTurretMaterialPath = Root + "/Art/Tanks/AbramsM1/AbramsTurret.mat";
        private const string ScenePath = Root + "/Scenes/TestRange.unity";
        private const string ProjectileMaterialPath = Root + "/Art/VFX/ProjectileMetal.mat";
        private const string ProjectileTracerMaterialPath = Root + "/Art/VFX/ProjectileTracer.mat";
        private const string MuzzleFlashMaterialPath = Root + "/Art/VFX/MuzzleFlash.mat";
        private const string MuzzleSmokeMaterialPath = Root + "/Art/VFX/MuzzleSmoke.mat";
        private const string ImpactMetalMaterialPath = Root + "/Art/VFX/ImpactMetal.mat";
        private const string ImpactDustMaterialPath = Root + "/Art/VFX/ImpactDust.mat";
        private const string ExplosionFlashMaterialPath = Root + "/Art/VFX/ExplosionFlash.mat";
        private const string ExplosionSmokeMaterialPath = Root + "/Art/VFX/ExplosionSmoke.mat";
        private const string ImpactMetalPath = Root + "/Prefabs/Projectiles/ImpactMetal.prefab";
        private const string ImpactDustPath = Root + "/Prefabs/Projectiles/ImpactDust.prefab";
        private const string ExplosionPath = Root + "/Prefabs/Projectiles/TankExplosion.prefab";
        private const string TerrainDataPath = Root + "/Art/Environment/TestRangeTerrain.asset";
        private const string TerrainTextureFolder = Root + "/Art/Environment/Textures";
        private const string TerrainGrassTexturePath = TerrainTextureFolder + "/TerrainGrass.png";
        private const string TerrainDirtTexturePath = TerrainTextureFolder + "/TerrainDirt.png";
        private const string TerrainMudTexturePath = TerrainTextureFolder + "/TerrainMud.png";
        private const string TerrainGrassNormalPath = TerrainTextureFolder + "/TerrainGrassNormal.png";
        private const string TerrainDirtNormalPath = TerrainTextureFolder + "/TerrainDirtNormal.png";
        private const string TerrainMudNormalPath = TerrainTextureFolder + "/TerrainMudNormal.png";
        private const string TerrainGrassLayerPath = Root + "/Art/Environment/TerrainGrass.terrainlayer";
        private const string TerrainDirtLayerPath = Root + "/Art/Environment/TerrainDirt.terrainlayer";
        private const string TerrainMudLayerPath = Root + "/Art/Environment/TerrainMud.terrainlayer";
        private const string KenneyNatureRoot = Root + "/Art/Environment/KenneyNature";
        private const string KenneyRockLargeAPath = KenneyNatureRoot + "/rock_largeA.fbx";
        private const string KenneyRockLargeBPath = KenneyNatureRoot + "/rock_largeB.fbx";
        private const string KenneyRockSmallAPath = KenneyNatureRoot + "/rock_smallA.fbx";
        private const string KenneyGrassLargePath = KenneyNatureRoot + "/grass_large.fbx";
        private const string KenneyGrassLeafsPath = KenneyNatureRoot + "/grass_leafsLarge.fbx";
        private const string KenneyTreePineAPath = KenneyNatureRoot + "/tree_pineSmallA.fbx";
        private const string KenneyTreePineBPath = KenneyNatureRoot + "/tree_pineSmallB.fbx";
        private const string VolumeProfilePath = Root + "/Settings/TestRangeVolumeProfile.asset";
        private const string SkyboxMaterialPath = Root + "/Materials/TestRangeSkybox.mat";
        private const string GroundMaterialPath = Root + "/Materials/TestRangeGround.mat";
        private const string DirtMaterialPath = Root + "/Materials/TestRangeDirt.mat";
        private const string GrassMaterialPath = Root + "/Materials/TestRangeGrass.mat";
        private const string MudMaterialPath = Root + "/Materials/TestRangeMud.mat";
        private const string RockMaterialPath = Root + "/Materials/TestRangeRock.mat";
        private const string ConcreteMaterialPath = Root + "/Materials/TestRangeConcrete.mat";
        private const string MetalMaterialPath = Root + "/Materials/TestRangeMetal.mat";
        private const string CrateMaterialPath = Root + "/Materials/TestRangeCrate.mat";
        private const string SandbagMaterialPath = Root + "/Materials/TestRangeSandbag.mat";
        private const string UiSpriteAssetPath = Root + "/Settings/UiWhiteSprite.asset";
        private const string AudioGenericImpactPath = Root + "/Audio/Impacts/impactGeneric_light_000.ogg";
        private const string AudioMetalImpactPath = Root + "/Audio/Impacts/impactMetal_heavy_000.ogg";

        [MenuItem("Tools/Tanks/Build Test Range")]
        public static void BuildTestRange()
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
            EnsureFolders();
            InputActionAsset input = EnsureInputActions();
            WeaponConfig weapon = EnsureWeaponConfig();
            EnsureVfxAssets();
            GameObject projectilePrefab = EnsureProjectilePrefab();
            GameObject tankPrefab = EnsureTankPrefab(input, weapon, projectilePrefab);
            GameObject enemyPrefab = EnsureEnemyTankPrefab(weapon, projectilePrefab);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Terrain terrain = CreateEnvironment();
            CreateAudioManager();
            GameObject playerObject = (GameObject)PrefabUtility.InstantiatePrefab(tankPrefab, scene);
            playerObject.name = "Player Abrams";
            playerObject.transform.SetPositionAndRotation(new Vector3(0f, TerrainHeight(terrain, 0f, -12f) + 0.05f, -12f), Quaternion.identity);
            Tank player = playerObject.GetComponent<Tank>();
            CreateTargets(terrain, enemyPrefab);
            CreateLight();
            Camera playerCamera = CreateCamera(player.transform);
            SetProperty(player.GetComponent<TankAim>(), "aimingCamera", playerCamera);
            CreatePlayerUi(player.GetComponent<Health>(), player.GetComponent<Tank>().Cannon);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddToBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Tanks", "TestRange создан. Откройте Assets/_Project/Scenes/TestRange и нажмите Play.", "OK");
        }

        private static void EnsureFolders()
        {
            string[] directories =
            {
                "Scenes", "Scripts/Core", "Scripts/Player", "Scripts/Combat", "Scripts/Camera", "Scripts/Input", "Scripts/UI", "Scripts/Game", "Scripts/Audio",
                "Prefabs/Tanks", "Prefabs/Projectiles", "Prefabs/Environment", "ScriptableObjects/Tanks", "ScriptableObjects/Weapons", "Materials", "Settings", "Input",
                "Art/Tanks/AbramsM1/textures", "Art/Environment", "Art/Environment/Textures", "Art/Environment/KenneyNature", "Art/VFX"
            };
            foreach (string directory in directories) Directory.CreateDirectory(Path.Combine(Root, directory));
        }

        private static InputActionAsset EnsureInputActions()
        {
            InputActionAsset asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<InputActionAsset>();
                AssetDatabase.CreateAsset(asset, InputPath);
            }
            else
            {
                foreach (InputActionMap existingMap in asset.actionMaps.ToArray()) asset.RemoveActionMap(existingMap);
            }

            InputActionMap map = asset.AddActionMap("Tank");
            AddAxisAction(map, "Move", "<Keyboard>/w", "<Keyboard>/s", "<Keyboard>/upArrow", "<Keyboard>/downArrow");
            AddAxisAction(map, "Rotate", "<Keyboard>/d", "<Keyboard>/a", "<Keyboard>/rightArrow", "<Keyboard>/leftArrow");
            map.AddAction("Fire", InputActionType.Button, "<Mouse>/leftButton");
            map.AddAction("Aim", InputActionType.Value, "<Mouse>/position", expectedControlLayout: "Vector2");
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static void AddAxisAction(InputActionMap map, string name, string positive, string negative, string alternativePositive, string alternativeNegative)
        {
            InputAction action = map.AddAction(name, InputActionType.Value, expectedControlLayout: "Axis");
            AddAxisComposite(action, positive, negative);
            if (!string.IsNullOrEmpty(alternativePositive)) AddAxisComposite(action, alternativePositive, alternativeNegative);
        }

        private static void AddAxisComposite(InputAction action, string positive, string negative)
        {
            action.AddCompositeBinding("1DAxis")
                .With("Positive", positive)
                .With("Negative", negative);
        }

        private static WeaponConfig EnsureWeaponConfig()
        {
            WeaponConfig config = AssetDatabase.LoadAssetAtPath<WeaponConfig>(WeaponPath);
            if (config != null) return config;
            config = ScriptableObject.CreateInstance<WeaponConfig>();
            AssetDatabase.CreateAsset(config, WeaponPath);
            return config;
        }

        private static GameObject EnsureProjectilePrefab()
        {
            Material projectileMaterial = GetOrCreateMaterial(ProjectileMaterialPath, new Color(0.045f, 0.055f, 0.06f), "Universal Render Pipeline/Lit");
            projectileMaterial.SetFloat("_Metallic", 0.9f);
            projectileMaterial.SetFloat("_Smoothness", 0.78f);
            Material tracerMaterial = GetOrCreateMaterial(ProjectileTracerMaterialPath, new Color(1f, 0.18f, 0.015f, 0.85f), "Universal Render Pipeline/Unlit");
            SetTransparentMaterial(tracerMaterial, true);

            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "Projectile";
            projectile.transform.localScale = Vector3.one * 0.24f;
            projectile.GetComponent<Renderer>().sharedMaterial = projectileMaterial;
            Rigidbody body = projectile.AddComponent<Rigidbody>();
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            TrailRenderer trail = projectile.AddComponent<TrailRenderer>();
            trail.time = 0.16f;
            trail.startWidth = 0.065f;
            trail.endWidth = 0f;
            trail.minVertexDistance = 0.04f;
            trail.alignment = LineAlignment.View;
            trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            trail.receiveShadows = false;
            trail.material = tracerMaterial;

            Projectile projectileComponent = projectile.AddComponent<Projectile>();
            SetProperty(projectileComponent, "impactEffectPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(ImpactDustPath));
            SetProperty(projectileComponent, "metalImpactEffectPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(ImpactMetalPath));
            PrefabUtility.SaveAsPrefabAsset(projectile, ProjectilePath);
            Object.DestroyImmediate(projectile);
            return AssetDatabase.LoadAssetAtPath<GameObject>(ProjectilePath);
        }

        private static void EnsureVfxAssets()
        {
            Material muzzleFlashMaterial = GetOrCreateMaterial(MuzzleFlashMaterialPath, new Color(1f, 0.28f, 0.02f, 0.9f), "Universal Render Pipeline/Particles/Unlit");
            Material muzzleSmokeMaterial = GetOrCreateMaterial(MuzzleSmokeMaterialPath, new Color(0.16f, 0.18f, 0.19f, 0.58f), "Universal Render Pipeline/Particles/Unlit");
            Material impactMetalMaterial = GetOrCreateMaterial(ImpactMetalMaterialPath, new Color(1f, 0.48f, 0.04f, 0.95f), "Universal Render Pipeline/Particles/Unlit");
            Material impactDustMaterial = GetOrCreateMaterial(ImpactDustMaterialPath, new Color(0.28f, 0.2f, 0.12f, 0.72f), "Universal Render Pipeline/Particles/Unlit");
            Material explosionFlashMaterial = GetOrCreateMaterial(ExplosionFlashMaterialPath, new Color(1f, 0.19f, 0.015f, 0.95f), "Universal Render Pipeline/Particles/Unlit");
            Material explosionSmokeMaterial = GetOrCreateMaterial(ExplosionSmokeMaterialPath, new Color(0.12f, 0.11f, 0.1f, 0.72f), "Universal Render Pipeline/Particles/Unlit");
            SetTransparentMaterial(muzzleFlashMaterial, true);
            SetTransparentMaterial(muzzleSmokeMaterial, false);
            SetTransparentMaterial(impactMetalMaterial, true);
            SetTransparentMaterial(impactDustMaterial, false);
            SetTransparentMaterial(explosionFlashMaterial, true);
            SetTransparentMaterial(explosionSmokeMaterial, false);

            EnsureImpactPrefab(ImpactMetalPath, impactMetalMaterial, 14, 0.55f, 2.4f, 0.12f, new Color(1f, 0.5f, 0.04f, 1f));
            EnsureImpactPrefab(ImpactDustPath, impactDustMaterial, 20, 0.85f, 1.6f, 0.28f, new Color(0.3f, 0.22f, 0.14f, 0.75f));
            EnsureExplosionPrefab(explosionFlashMaterial, explosionSmokeMaterial);
        }

        private static void EnsureImpactPrefab(string path, Material material, int count, float lifetime, float speed, float size, Color color)
        {
            GameObject effect = new(Path.GetFileNameWithoutExtension(path));
            effect.AddComponent<AutoDestroyEffect>();
            ParticleSystem particles = CreateParticleSystem("Particles", effect.transform, material, count, lifetime, speed, size, color, 0.08f);
            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 25f;
            shape.radius = 0.04f;
            SaveEffectPrefab(effect, path, lifetime);
        }

        private static void EnsureExplosionPrefab(Material flashMaterial, Material smokeMaterial)
        {
            GameObject effect = new("Tank Explosion");
            effect.AddComponent<AutoDestroyEffect>();
            ParticleSystem flash = CreateParticleSystem("Flash", effect.transform, flashMaterial, 18, 0.22f, 3.5f, 1.25f, new Color(1f, 0.3f, 0.02f, 1f), 0.02f);
            ParticleSystem.ShapeModule flashShape = flash.shape;
            flashShape.shapeType = ParticleSystemShapeType.Sphere;
            flashShape.radius = 0.2f;
            ParticleSystem smoke = CreateParticleSystem("Smoke", effect.transform, smokeMaterial, 24, 2.8f, 1.2f, 0.85f, new Color(0.12f, 0.11f, 0.1f, 0.72f), 0.25f);
            ParticleSystem.ShapeModule smokeShape = smoke.shape;
            smokeShape.shapeType = ParticleSystemShapeType.Sphere;
            smokeShape.radius = 0.3f;
            SaveEffectPrefab(effect, ExplosionPath, 3.2f);
        }

        private static ParticleSystem CreateParticleSystem(string objectName, Transform parent, Material material, int count, float lifetime, float speed, float size, Color color, float radius)
        {
            GameObject effectObject = new(objectName);
            effectObject.transform.SetParent(parent, false);
            ParticleSystem particles = effectObject.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particles.main;
            main.loop = false;
            main.playOnAwake = false;
            main.duration = lifetime;
            main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime * 0.65f, lifetime);
            main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.65f, speed);
            main.startSize = new ParticleSystem.MinMaxCurve(size * 0.6f, size);
            main.startColor = color;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = count;
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });
            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = radius;
            ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.material = material;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            return particles;
        }

        private static void SaveEffectPrefab(GameObject effect, string path, float lifetime)
        {
            SetProperty(effect.GetComponent<AutoDestroyEffect>(), "lifetime", lifetime);
            PrefabUtility.SaveAsPrefabAsset(effect, path);
            Object.DestroyImmediate(effect);
        }

        private static GameObject EnsureTankPrefab(InputActionAsset input, WeaponConfig weapon, GameObject projectilePrefab)
        {
            AssetDatabase.ImportAsset(AbramsModelPath, ImportAssetOptions.ForceSynchronousImport);
            GameObject abramsAsset = AssetDatabase.LoadAssetAtPath<GameObject>(AbramsModelPath);
            if (abramsAsset == null)
                throw new FileNotFoundException($"Abrams model was not imported: {AbramsModelPath}");

            Material baseMaterial = EnsureAbramsMaterial(
                AbramsBaseMaterialPath,
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Base_BaseColor.png",
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Base_Normal.png",
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Base_Metallic.png",
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Base_Roughness.png",
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Base_Emissive.png",
                0.62f);
            Material trackMaterial = EnsureAbramsMaterial(
                AbramsTrackMaterialPath,
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Track_BaseColor.png",
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Track_Normal.png",
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Track_Metallic.png",
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Track_Roughness.png",
                null,
                0.55f);
            Material turretMaterial = EnsureAbramsMaterial(
                AbramsTurretMaterialPath,
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Turret_BaseColor.png",
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Turret_Normal.png",
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Turret_Metallic.png",
                "Assets/_Project/Art/Tanks/AbramsM1/textures/M1A1_LP_Turret_Roughness.png",
                null,
                0.58f);

            GameObject root = new("PlayerAbrams");
            root.name = "PlayerAbrams";
            BoxCollider hullCollider = root.AddComponent<BoxCollider>();
            hullCollider.center = new Vector3(0f, 1.15f, 0f);
            hullCollider.size = new Vector3(3.8f, 2.3f, 7.6f);
            Rigidbody body = root.AddComponent<Rigidbody>();
            body.mass = 2500f;
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;
            TankInput inputComponent = root.AddComponent<TankInput>();
            root.AddComponent<TankMovement>();
            Health health = root.AddComponent<Health>();
            AudioSource audioSource = root.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
            TankAudio tankAudio = root.AddComponent<TankAudio>();
            SetProperty(health, "destroyOnDeath", false);

            GameObject visualObject = (GameObject)PrefabUtility.InstantiatePrefab(abramsAsset);
            visualObject.name = "Abrams";
            visualObject.transform.SetParent(root.transform, false);
            PrefabUtility.UnpackPrefabInstance(visualObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            visualObject.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            visualObject.transform.localScale = Vector3.one;

            Transform importedHull = visualObject.transform.Find("Hull");
            Transform importedTurret = visualObject.transform.Find("Turret");
            Transform leftTrack = visualObject.transform.Find("L_Track");
            Transform rightTrack = visualObject.transform.Find("R_Track");
            if (importedHull == null || importedTurret == null || leftTrack == null || rightTrack == null)
                throw new InvalidDataException("M1A1.fbx must contain Hull, Turret, L_Track and R_Track nodes.");

            ApplyAbramsMaterials(visualObject.transform, importedHull, importedTurret, baseMaterial, turretMaterial, trackMaterial);

            Vector3 turretPivotPosition = importedTurret.localPosition;
            Transform turretTransform = new GameObject("Turret").transform;
            turretTransform.SetParent(visualObject.transform, false);
            turretTransform.localPosition = turretPivotPosition;
            turretTransform.localRotation = Quaternion.identity;
            importedTurret.SetParent(turretTransform, true);
            TankTurret turret = turretTransform.gameObject.AddComponent<TankTurret>();
            TankAim aim = root.AddComponent<TankAim>();

            Renderer turretRenderer = importedTurret.GetComponent<Renderer>();
            Vector3 muzzleWorldPosition = new(
                turretRenderer.bounds.center.x,
                turretRenderer.bounds.center.y,
                turretRenderer.bounds.max.z + 0.2f);
            Transform muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(turretTransform, true);
            muzzle.position = muzzleWorldPosition;
            muzzle.rotation = Quaternion.LookRotation(visualObject.transform.right, Vector3.up);
            TankCannon cannon = turretTransform.gameObject.AddComponent<TankCannon>();
            ParticleSystem muzzleFlash = CreateMuzzleParticles("Muzzle Flash", muzzle, new Color(1f, 0.7f, 0.12f, 1f), 9, 0.08f, 0.45f, AssetDatabase.LoadAssetAtPath<Material>(MuzzleFlashMaterialPath));
            ParticleSystem muzzleSmoke = CreateMuzzleParticles("Muzzle Smoke", muzzle, new Color(0.42f, 0.42f, 0.42f, 0.55f), 5, 0.5f, 0.75f, AssetDatabase.LoadAssetAtPath<Material>(MuzzleSmokeMaterialPath));

            Tank tank = root.AddComponent<Tank>();
            SetProperty(inputComponent, "inputActions", input);
            SetProperty(root.GetComponent<TankMovement>(), "input", inputComponent);
            SetProperty(aim, "input", inputComponent);
            SetProperty(aim, "turret", turret);
            SetProperty(cannon, "input", inputComponent);
            SetProperty(cannon, "muzzle", muzzle);
            SetProperty(cannon, "projectilePrefab", projectilePrefab.GetComponent<Projectile>());
            SetProperty(cannon, "weaponConfig", weapon);
            SetProperty(cannon, "muzzleFlash", muzzleFlash);
            SetProperty(cannon, "muzzleSmoke", muzzleSmoke);
            SetProperty(cannon, "audioEvents", tankAudio);
            SetProperty(tankAudio, "source", audioSource);
            SetProperty(tank, "hull", importedHull);
            SetProperty(tank, "leftTrack", leftTrack);
            SetProperty(tank, "rightTrack", rightTrack);
            SetProperty(tank, "turret", turret);
            SetProperty(tank, "cannon", cannon);

            PrefabUtility.SaveAsPrefabAsset(root, TankPath);
            Object.DestroyImmediate(root);
            return AssetDatabase.LoadAssetAtPath<GameObject>(TankPath);
        }

        private static GameObject EnsureEnemyTankPrefab(WeaponConfig weapon, GameObject projectilePrefab)
        {
            GameObject abramsAsset = AssetDatabase.LoadAssetAtPath<GameObject>(AbramsModelPath);
            GameObject root = new("EnemyTank");
            BoxCollider collider = root.AddComponent<BoxCollider>();
            collider.center = new Vector3(0f, 1.2f, 0f);
            collider.size = new Vector3(3.8f, 2.4f, 7.8f);
            root.AddComponent<ImpactSurface>();
            Health health = root.AddComponent<Health>();
            TargetStatus status = root.AddComponent<TargetStatus>();
            HealthDeathVfx deathVfx = root.AddComponent<HealthDeathVfx>();
            EnemyVisualVariant visualVariant = root.AddComponent<EnemyVisualVariant>();
            AudioSource audioSource = root.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
            TankAudio tankAudio = root.AddComponent<TankAudio>();

            GameObject visualObject = (GameObject)PrefabUtility.InstantiatePrefab(abramsAsset);
            visualObject.name = "Abrams Visual";
            visualObject.transform.SetParent(root.transform, false);
            PrefabUtility.UnpackPrefabInstance(visualObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            visualObject.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            visualObject.transform.localScale = Vector3.one;

            Transform hull = visualObject.transform.Find("Hull");
            Transform turret = visualObject.transform.Find("Turret");
            Transform leftTrack = visualObject.transform.Find("L_Track");
            Transform rightTrack = visualObject.transform.Find("R_Track");
            if (hull == null || turret == null || leftTrack == null || rightTrack == null)
                throw new InvalidDataException("Enemy visual requires Hull, Turret, L_Track and R_Track nodes.");

            ApplyAbramsMaterials(
                visualObject.transform,
                hull,
                turret,
                AssetDatabase.LoadAssetAtPath<Material>(AbramsBaseMaterialPath),
                AssetDatabase.LoadAssetAtPath<Material>(AbramsTurretMaterialPath),
                AssetDatabase.LoadAssetAtPath<Material>(AbramsTrackMaterialPath));
            Vector3 turretPivotPosition = turret.localPosition;
            Transform turretTransform = new GameObject("Turret").transform;
            turretTransform.SetParent(visualObject.transform, false);
            turretTransform.localPosition = turretPivotPosition;
            turretTransform.localRotation = Quaternion.identity;
            turret.SetParent(turretTransform, true);
            TankTurret turretComponent = turretTransform.gameObject.AddComponent<TankTurret>();
            Renderer turretRenderer = turret.GetComponent<Renderer>();
            Vector3 muzzleWorldPosition = new(turretRenderer.bounds.center.x, turretRenderer.bounds.center.y, turretRenderer.bounds.max.z + 0.2f);
            Transform muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(turretTransform, true);
            muzzle.position = muzzleWorldPosition;
            muzzle.rotation = Quaternion.LookRotation(visualObject.transform.right, Vector3.up);
            TankCannon cannon = turretTransform.gameObject.AddComponent<TankCannon>();
            ParticleSystem muzzleFlash = CreateMuzzleParticles("Muzzle Flash", muzzle, new Color(1f, 0.7f, 0.12f, 1f), 7, 0.08f, 0.32f, AssetDatabase.LoadAssetAtPath<Material>(MuzzleFlashMaterialPath));
            ParticleSystem muzzleSmoke = CreateMuzzleParticles("Muzzle Smoke", muzzle, new Color(0.42f, 0.42f, 0.42f, 0.55f), 4, 0.5f, 0.55f, AssetDatabase.LoadAssetAtPath<Material>(MuzzleSmokeMaterialPath));
            EnemyTankAI ai = root.AddComponent<EnemyTankAI>();
            SetProperty(deathVfx, "health", health);
            SetProperty(deathVfx, "effectPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(ExplosionPath));
            SetProperty(status, "displayName", "ENEMY ABRAMS");
            SetProperty(visualVariant, "tint", Color.white);
            SetProperty(cannon, "muzzle", muzzle);
            SetProperty(cannon, "projectilePrefab", projectilePrefab.GetComponent<Projectile>());
            SetProperty(cannon, "weaponConfig", weapon);
            SetProperty(cannon, "muzzleFlash", muzzleFlash);
            SetProperty(cannon, "muzzleSmoke", muzzleSmoke);
            SetProperty(ai, "turret", turretComponent);
            SetProperty(ai, "cannon", cannon);
            SetProperty(tankAudio, "source", audioSource);

            PrefabUtility.SaveAsPrefabAsset(root, EnemyTankPath);
            Object.DestroyImmediate(root);
            return AssetDatabase.LoadAssetAtPath<GameObject>(EnemyTankPath);
        }

        private static void ApplyAbramsMaterials(Transform visualRoot, Transform hull, Transform turret, Material baseMaterial, Material turretMaterial, Material trackMaterial)
        {
            foreach (Renderer renderer in visualRoot.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer.transform == hull)
                    renderer.sharedMaterial = baseMaterial;
                else if (renderer.transform == turret)
                    renderer.sharedMaterial = turretMaterial;
                else
                    renderer.sharedMaterial = trackMaterial;
            }
        }

        private static Material EnsureAbramsMaterial(string materialPath, string baseColorPath, string normalPath, string metallicPath, string roughnessPath, string emissionPath, float smoothness)
        {
            ConfigureTexture(baseColorPath, TextureImporterType.Default, true);
            ConfigureTexture(normalPath, TextureImporterType.NormalMap, false);
            ConfigureTexture(metallicPath, TextureImporterType.Default, false);
            ConfigureTexture(roughnessPath, TextureImporterType.Default, false);
            if (!string.IsNullOrEmpty(emissionPath)) ConfigureTexture(emissionPath, TextureImporterType.Default, true);

            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, materialPath);
            }

            Texture2D baseColor = AssetDatabase.LoadAssetAtPath<Texture2D>(baseColorPath);
            Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);
            Texture2D metallicGloss = EnsureMetallicGlossTexture(metallicPath, roughnessPath);
            material.SetTexture("_BaseMap", baseColor);
            material.SetTexture("_BumpMap", normal);
            material.SetTexture("_MetallicGlossMap", metallicGloss);
            material.SetFloat("_Metallic", 1f);
            material.SetFloat("_Smoothness", smoothness);
            material.SetFloat("_BumpScale", 1f);
            if (!string.IsNullOrEmpty(emissionPath))
            {
                material.SetTexture("_EmissionMap", AssetDatabase.LoadAssetAtPath<Texture2D>(emissionPath));
                material.SetColor("_EmissionColor", Color.white);
            }
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Texture2D EnsureMetallicGlossTexture(string metallicPath, string roughnessPath)
        {
            string glossPath = metallicPath.Replace("_Metallic.png", "_MetallicGloss.png");
            Texture2D existing = AssetDatabase.LoadAssetAtPath<Texture2D>(glossPath);
            if (existing != null) return existing;

            Texture2D metallic = LoadPng(metallicPath);
            Texture2D roughness = LoadPng(roughnessPath);
            if (metallic.width != roughness.width || metallic.height != roughness.height)
                throw new InvalidDataException($"Metallic and roughness dimensions differ: {metallicPath} / {roughnessPath}");

            Color32[] metallicPixels = metallic.GetPixels32();
            Color32[] roughnessPixels = roughness.GetPixels32();
            Color32[] glossPixels = new Color32[metallicPixels.Length];
            for (int i = 0; i < glossPixels.Length; i++)
            {
                Color32 metallicPixel = metallicPixels[i];
                glossPixels[i] = new Color32(metallicPixel.r, metallicPixel.g, metallicPixel.b, (byte)(255 - roughnessPixels[i].r));
            }

            Texture2D gloss = new(metallic.width, metallic.height, TextureFormat.RGBA32, false, true);
            gloss.SetPixels32(glossPixels);
            gloss.Apply(false, false);
            File.WriteAllBytes(glossPath, gloss.EncodeToPNG());
            Object.DestroyImmediate(gloss);
            Object.DestroyImmediate(metallic);
            Object.DestroyImmediate(roughness);
            AssetDatabase.ImportAsset(glossPath, ImportAssetOptions.ForceSynchronousImport);
            ConfigureTexture(glossPath, TextureImporterType.Default, false);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(glossPath);
        }

        private static Texture2D LoadPng(string path)
        {
            Texture2D texture = new(2, 2, TextureFormat.RGBA32, false, true);
            texture.LoadImage(File.ReadAllBytes(path), false);
            return texture;
        }

        private static void ConfigureTexture(string path, TextureImporterType type, bool srgb)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;
            bool changed = importer.textureType != type || importer.sRGBTexture != srgb;
            importer.textureType = type;
            importer.sRGBTexture = srgb;
            if (changed) importer.SaveAndReimport();
        }

        private static ParticleSystem CreateMuzzleParticles(string objectName, Transform parent, Color color, int count, float lifetime, float size, Material material)
        {
            GameObject effectObject = new(objectName);
            effectObject.transform.SetParent(parent, false);
            ParticleSystem particles = effectObject.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particles.main;
            main.loop = false;
            main.playOnAwake = false;
            main.duration = lifetime;
            main.startLifetime = lifetime;
            main.startSpeed = 1.8f;
            main.startSize = size;
            main.startColor = color;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = count;
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });
            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 12f;
            shape.radius = 0.05f;
            particles.GetComponent<ParticleSystemRenderer>().material = material;
            return particles;
        }

        private static Material GetOrCreateMaterial(string path, Color color, string shaderName = "Universal Render Pipeline/Lit")
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            Shader shader = Shader.Find(shaderName);
            if (shader == null) throw new System.InvalidOperationException($"Required URP shader was not found: {shaderName}");
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }
            else if (material.shader != shader)
            {
                material.shader = shader;
            }
            material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void SetTransparentMaterial(Material material, bool additive)
        {
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", additive ? 2f : 0f);
            material.SetFloat("_ZWrite", 0f);
            material.SetFloat("_SrcBlend", (float)(additive ? UnityEngine.Rendering.BlendMode.One : UnityEngine.Rendering.BlendMode.SrcAlpha));
            material.SetFloat("_DstBlend", (float)(additive ? UnityEngine.Rendering.BlendMode.One : UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha));
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            EditorUtility.SetDirty(material);
        }

        private static Material EnsureSkyboxMaterial()
        {
            Shader shader = Shader.Find("Skybox/Procedural");
            if (shader == null) throw new System.InvalidOperationException("Unity procedural skybox shader was not found.");
            Material material = AssetDatabase.LoadAssetAtPath<Material>(SkyboxMaterialPath);
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, SkyboxMaterialPath);
            }
            else if (material.shader != shader)
            {
                material.shader = shader;
            }

            material.SetColor("_SkyTint", new Color(0.38f, 0.5f, 0.65f));
            material.SetColor("_GroundColor", new Color(0.2f, 0.16f, 0.11f));
            material.SetFloat("_Exposure", 0.8f);
            material.SetFloat("_AtmosphereThickness", 0.8f);
            material.SetFloat("_SunSize", 0.04f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static TerrainLayer[] EnsureTerrainLayers()
        {
            Texture2D grassTexture = EnsureTerrainTexture(TerrainGrassTexturePath, new Color(0.16f, 0.25f, 0.1f), new Color(0.3f, 0.38f, 0.16f), 11, false);
            Texture2D dirtTexture = EnsureTerrainTexture(TerrainDirtTexturePath, new Color(0.23f, 0.14f, 0.075f), new Color(0.4f, 0.25f, 0.12f), 29, false);
            Texture2D mudTexture = EnsureTerrainTexture(TerrainMudTexturePath, new Color(0.095f, 0.07f, 0.045f), new Color(0.2f, 0.13f, 0.075f), 47, false);
            Texture2D grassNormal = EnsureTerrainTexture(TerrainGrassNormalPath, new Color(0.5f, 0.5f, 1f), new Color(0.54f, 0.48f, 0.92f), 11, true);
            Texture2D dirtNormal = EnsureTerrainTexture(TerrainDirtNormalPath, new Color(0.5f, 0.5f, 1f), new Color(0.46f, 0.55f, 0.9f), 29, true);
            Texture2D mudNormal = EnsureTerrainTexture(TerrainMudNormalPath, new Color(0.5f, 0.5f, 1f), new Color(0.55f, 0.46f, 0.9f), 47, true);

            TerrainLayer grassLayer = EnsureTerrainLayer(TerrainGrassLayerPath, "Terrain Grass", grassTexture, grassNormal, 26f, 0.15f);
            TerrainLayer dirtLayer = EnsureTerrainLayer(TerrainDirtLayerPath, "Terrain Dirt", dirtTexture, dirtNormal, 22f, 0.08f);
            TerrainLayer mudLayer = EnsureTerrainLayer(TerrainMudLayerPath, "Terrain Mud", mudTexture, mudNormal, 18f, 0.03f);
            return new[] { grassLayer, dirtLayer, mudLayer };
        }

        private static TerrainLayer EnsureTerrainLayer(string path, string layerName, Texture2D diffuse, Texture2D normal, float tileSize, float smoothness)
        {
            TerrainLayer layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);
            if (layer == null)
            {
                layer = new TerrainLayer();
                AssetDatabase.CreateAsset(layer, path);
            }
            layer.name = layerName;
            layer.diffuseTexture = diffuse;
            layer.normalMapTexture = normal;
            layer.tileSize = new Vector2(tileSize, tileSize);
            layer.tileOffset = Vector2.zero;
            layer.metallic = 0f;
            layer.smoothness = smoothness;
            EditorUtility.SetDirty(layer);
            return layer;
        }

        private static Texture2D EnsureTerrainTexture(string path, Color firstColor, Color secondColor, int seed, bool normalMap)
        {
            Texture2D existing = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (existing != null) return existing;

            const int size = 256;
            Texture2D texture = new(size, size, TextureFormat.RGBA32, false, normalMap);
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float coarse = Mathf.PerlinNoise((x + seed * 7f) * 0.035f, (y + seed * 3f) * 0.035f);
                    float fine = Mathf.PerlinNoise((x + seed) * 0.17f, (y - seed) * 0.17f);
                    float blend = Mathf.Clamp01(coarse * 0.72f + fine * 0.28f);
                    Color pixel = Color.Lerp(firstColor, secondColor, blend);
                    if (normalMap)
                        pixel = new Color(Mathf.Clamp01(0.5f + (blend - 0.5f) * 0.12f), Mathf.Clamp01(0.5f - (blend - 0.5f) * 0.1f), 1f, 1f);
                    pixels[y * size + x] = pixel;
                }
            }
            texture.SetPixels(pixels);
            texture.Apply(false, false);
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            ConfigureTexture(path, normalMap ? TextureImporterType.NormalMap : TextureImporterType.Default, !normalMap);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static float[,,] CreateTerrainAlphaMaps(int resolution)
        {
            float[,,] alphaMaps = new float[resolution, resolution, 3];
            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float worldX = x / (resolution - 1f) * 200f - 100f;
                    float worldZ = z / (resolution - 1f) * 200f - 100f;
                    float variation = Mathf.PerlinNoise((worldX + 100f) * 0.045f, (worldZ + 100f) * 0.045f);
                    float dirt = Mathf.Lerp(0.12f, 0.45f, variation);
                    float mud = Mathf.Lerp(0.02f, 0.14f, Mathf.PerlinNoise((worldX + 40f) * 0.08f, (worldZ - 20f) * 0.08f));
                    float road = 0f;
                    if (worldZ > -62f && worldZ < 98f) road = Mathf.Max(road, Mathf.Clamp01(1f - Mathf.Abs(worldX) / 7f));
                    if (worldX > -5f && worldX < 72f) road = Mathf.Max(road, Mathf.Clamp01(1f - Mathf.Abs(worldZ - 28f) / 6f));
                    mud = Mathf.Max(mud, Gaussian(worldX, worldZ, -42f, -18f, 11f, 0.7f));
                    mud = Mathf.Max(mud, Gaussian(worldX, worldZ, 24f, -25f, 13f, 0.65f));
                    dirt = Mathf.Clamp01(dirt + road * 0.75f);
                    mud = Mathf.Clamp01(mud + road * 0.1f);
                    float grass = Mathf.Max(0.02f, 1f - dirt - mud);
                    float total = grass + dirt + mud;
                    alphaMaps[z, x, 0] = grass / total;
                    alphaMaps[z, x, 1] = dirt / total;
                    alphaMaps[z, x, 2] = mud / total;
                }
            }
            return alphaMaps;
        }

        private static Terrain CreateEnvironment()
        {
            Material groundMaterial = GetOrCreateMaterial(GroundMaterialPath, new Color(0.19f, 0.23f, 0.13f), "Universal Render Pipeline/Terrain/Lit");
            Material dirtMaterial = GetOrCreateMaterial(DirtMaterialPath, new Color(0.29f, 0.19f, 0.11f));
            Material grassMaterial = GetOrCreateMaterial(GrassMaterialPath, new Color(0.16f, 0.27f, 0.1f));
            Material mudMaterial = GetOrCreateMaterial(MudMaterialPath, new Color(0.12f, 0.095f, 0.065f));
            Material rockMaterial = GetOrCreateMaterial(RockMaterialPath, new Color(0.22f, 0.23f, 0.2f));
            Material concreteMaterial = GetOrCreateMaterial(ConcreteMaterialPath, new Color(0.34f, 0.36f, 0.33f));
            Material metalMaterial = GetOrCreateMaterial(MetalMaterialPath, new Color(0.12f, 0.14f, 0.14f));
            Material crateMaterial = GetOrCreateMaterial(CrateMaterialPath, new Color(0.29f, 0.2f, 0.1f));
            Material sandbagMaterial = GetOrCreateMaterial(SandbagMaterialPath, new Color(0.5f, 0.4f, 0.24f));

            TerrainData data = AssetDatabase.LoadAssetAtPath<TerrainData>(TerrainDataPath);
            if (data == null)
            {
                data = new TerrainData();
                AssetDatabase.CreateAsset(data, TerrainDataPath);
            }

            const int heightmapResolution = 257;
            data.heightmapResolution = heightmapResolution;
            data.size = new Vector3(200f, 18f, 200f);
            float[,] heights = new float[heightmapResolution, heightmapResolution];
            for (int z = 0; z < heightmapResolution; z++)
            {
                for (int x = 0; x < heightmapResolution; x++)
                {
                    float worldX = x / (heightmapResolution - 1f) * 200f - 100f;
                    float worldZ = z / (heightmapResolution - 1f) * 200f - 100f;
                    float height = 0.008f;
                    height += Gaussian(worldX, worldZ, -56f, 44f, 18f, 0.32f);
                    height += Gaussian(worldX, worldZ, 62f, 50f, 22f, 0.24f);
                    height += Gaussian(worldX, worldZ, 55f, -50f, 18f, 0.18f);
                    height -= Gaussian(worldX, worldZ, 32f, 8f, 13f, 0.27f);
                    height -= Gaussian(worldX, worldZ, 32f, 25f, 10f, 0.18f);
                    heights[z, x] = Mathf.Clamp01(height);
                }
            }
            data.SetHeights(0, 0, heights);
            TerrainLayer[] terrainLayers = EnsureTerrainLayers();
            data.alphamapResolution = 256;
            data.baseMapResolution = 1024;
            data.terrainLayers = terrainLayers;
            float[,,] alphaMaps = CreateTerrainAlphaMaps(data.alphamapResolution);
            data.SetAlphamaps(0, 0, alphaMaps);
            EditorUtility.SetDirty(data);

            GameObject terrainObject = Terrain.CreateTerrainGameObject(data);
            terrainObject.name = "Terrain Ground 200m";
            terrainObject.transform.position = new Vector3(-100f, 0f, -100f);
            Terrain terrain = terrainObject.GetComponent<Terrain>();
            terrain.materialTemplate = groundMaterial;
            terrain.drawInstanced = true;

            CreateRoadStrip(terrain, dirtMaterial, mudMaterial, true, 0f, -55f, 150f);
            CreateRoadStrip(terrain, dirtMaterial, mudMaterial, false, 3f, 28f, 70f);
            CreatePrimitive("Road Shoulder", PrimitiveType.Cube, new Vector3(-38f, TerrainHeight(terrain, -38f, 20f) + 0.05f, 20f), new Vector3(34f, 0.1f, 11f), mudMaterial, false);

            float[,] mudPatches = { { -42f, -18f, 8f }, { 24f, -25f, 10f }, { -26f, 62f, 7f }, { 58f, 18f, 9f } };
            for (int i = 0; i < mudPatches.GetLength(0); i++)
            {
                float x = mudPatches[i, 0];
                float z = mudPatches[i, 1];
                float radius = mudPatches[i, 2];
                CreatePrimitive("Mud Patch", PrimitiveType.Cylinder, new Vector3(x, TerrainHeight(terrain, x, z) + 0.025f, z), new Vector3(radius, 0.035f, radius), mudMaterial, false);
            }

            Vector2[] concretePositions = { new(-72f, -38f), new(-65f, -25f), new(76f, 8f), new(70f, 20f), new(-70f, 63f), new(-58f, 70f) };
            for (int i = 0; i < concretePositions.Length; i++)
            {
                float x = concretePositions[i].x;
                float z = concretePositions[i].y;
                GameObject block = CreateBlockOnTerrain($"Concrete Barrier {i + 1}", terrain, x, z, 3.4f, 1.2f, 1.1f, concreteMaterial);
                block.transform.rotation = Quaternion.Euler(0f, i % 2 == 0 ? 18f : -18f, 0f);
            }

            CreateTankTrap(terrain, -46f, 30f, metalMaterial);
            CreateTankTrap(terrain, -35f, 33f, metalMaterial);
            CreateTankTrap(terrain, 48f, -8f, metalMaterial);
            CreateTankTrap(terrain, 58f, -4f, metalMaterial);

            for (int i = 0; i < 5; i++)
                CreateSandbagLine(terrain, -58f + i * 2.1f, 4f, sandbagMaterial, i % 2 == 0);
            for (int i = 0; i < 4; i++)
                CreateSandbagLine(terrain, 42f, 50f + i * 2.1f, sandbagMaterial, false);

            CreateCrateStack(terrain, -22f, 46f, crateMaterial);
            CreateCrateStack(terrain, 18f, 63f, crateMaterial);
            CreateCrateStack(terrain, 66f, -28f, crateMaterial);

            Vector2[] rockPositions = { new(-82f, -65f), new(-72f, 22f), new(-45f, 78f), new(-5f, 73f), new(12f, -65f), new(42f, -73f), new(83f, -45f), new(78f, 68f), new(8f, 48f), new(-80f, 54f), new(63f, 42f), new(-12f, -75f) };
            string[] rockAssets = { KenneyRockLargeAPath, KenneyRockLargeBPath, KenneyRockSmallAPath };
            for (int i = 0; i < rockPositions.Length; i++)
            {
                float x = rockPositions[i].x;
                float z = rockPositions[i].y;
                float size = 0.7f + (i % 3) * 0.35f;
                GameObject rock = CreateImportedEnvironmentModel(rockAssets[i % rockAssets.Length], "Kenney Training Range Rock", new Vector3(x, TerrainHeight(terrain, x, z) + 0.04f, z), Vector3.one * size, rockMaterial, false);
                if (rock == null)
                    rock = CreatePrimitive("Training Range Rock", PrimitiveType.Sphere, new Vector3(x, TerrainHeight(terrain, x, z) + size * 0.35f, z), new Vector3(size * 1.4f, size, size), rockMaterial, true);
                rock.transform.rotation = Quaternion.Euler(i * 13f, i * 29f, i * 7f);
            }

            CreateDestroyedWall(terrain, 73f, 58f, concreteMaterial);
            CreateRubblePile(terrain, -67f, -6f, concreteMaterial, rockMaterial);
            CreateRubblePile(terrain, 50f, 67f, concreteMaterial, rockMaterial);

            Vector2[] treePositions = { new(-88f, -42f), new(-82f, 42f), new(-48f, 87f), new(45f, 84f), new(87f, 48f), new(88f, -56f), new(34f, -86f), new(-42f, -86f) };
            string[] treeAssets = { KenneyTreePineAPath, KenneyTreePineBPath };
            for (int i = 0; i < treePositions.Length; i++)
            {
                float x = treePositions[i].x;
                float z = treePositions[i].y;
                GameObject tree = CreateImportedEnvironmentModel(treeAssets[i % treeAssets.Length], "Kenney Pine", new Vector3(x, TerrainHeight(terrain, x, z) + 0.04f, z), Vector3.one * (2.2f + (i % 3) * 0.3f), grassMaterial, false);
                if (tree != null) tree.transform.rotation = Quaternion.Euler(0f, i * 37f, 0f);
            }

            Random.State randomState = Random.state;
            Random.InitState(8107);
            for (int i = 0; i < 42; i++)
            {
                float x = Random.Range(-92f, 92f);
                float z = Random.Range(-88f, 88f);
                float y = TerrainHeight(terrain, x, z);
                string grassAsset = i % 2 == 0 ? KenneyGrassLargePath : KenneyGrassLeafsPath;
                GameObject grass = CreateImportedEnvironmentModel(grassAsset, "Kenney Grass Clump", new Vector3(x, y + 0.03f, z), Vector3.one * Random.Range(0.45f, 0.75f), grassMaterial, false);
                if (grass == null)
                    grass = CreatePrimitive("Grass Clump", PrimitiveType.Cube, new Vector3(x, y + 0.16f, z), new Vector3(0.08f, 0.32f, 0.35f), grassMaterial, false);
                grass.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 180f), Random.Range(-12f, 12f));
            }
            Random.state = randomState;
            NavMeshSurface navigationSurface = terrainObject.AddComponent<NavMeshSurface>();
            navigationSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            navigationSurface.BuildNavMesh();
            return terrain;
        }

        private static void CreateTargets(Terrain terrain, GameObject enemyPrefab)
        {
            Vector3[] positions = { new(-22f, 46f, 0f), new(62f, 50f, 180f), new(-68f, 63f, -18f) };
            Color[] tints = { new(0.78f, 0.88f, 0.66f, 1f), new(0.78f, 0.68f, 0.4f, 1f), new(0.54f, 0.68f, 0.5f, 1f) };
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject target = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab);
                target.name = $"Enemy Abrams {i + 1}";
                target.transform.SetPositionAndRotation(
                    new Vector3(positions[i].x, TerrainHeight(terrain, positions[i].x, positions[i].y) + 0.05f, positions[i].y),
                    Quaternion.Euler(0f, positions[i].z, 0f));
                target.transform.localScale = Vector3.one * (i == 1 ? 1.05f : 0.92f);
                SetProperty(target.GetComponent<TargetStatus>(), "displayName", $"ENEMY ABRAMS {i + 1}");
                SetProperty(target.GetComponent<EnemyVisualVariant>(), "tint", tints[i]);
            }
        }

        private static void CreateLight()
        {
            Material skyboxMaterial = EnsureSkyboxMaterial();
            RenderSettings.skybox = skyboxMaterial;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.34f, 0.42f, 0.52f);
            RenderSettings.ambientEquatorColor = new Color(0.28f, 0.3f, 0.23f);
            RenderSettings.ambientGroundColor = new Color(0.09f, 0.08f, 0.055f);
            RenderSettings.reflectionIntensity = 0.7f;

            GameObject lightObject = new("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(1f, 0.91f, 0.78f);
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.82f;
            light.shadowResolution = LightShadowResolution.High;
            lightObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);

            VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, VolumeProfilePath);
            }
            ColorAdjustments color = profile.components.OfType<ColorAdjustments>().FirstOrDefault() ?? profile.Add<ColorAdjustments>(true);
            color.postExposure.value = 0.15f;
            color.contrast.value = 12f;
            color.saturation.value = -6f;
            Tonemapping tonemapping = profile.components.OfType<Tonemapping>().FirstOrDefault() ?? profile.Add<Tonemapping>(true);
            tonemapping.mode.value = TonemappingMode.ACES;
            GameObject volumeObject = new("Range Atmosphere");
            Volume volume = volumeObject.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1f;
            volume.sharedProfile = profile;
        }

        private static void CreateAudioManager()
        {
            GameObject audioObject = new("AudioManager");
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            AudioManager manager = audioObject.AddComponent<AudioManager>();
            SetProperty(manager, "output", source);
            SetProperty(manager, "genericImpactClip", AssetDatabase.LoadAssetAtPath<AudioClip>(AudioGenericImpactPath));
            SetProperty(manager, "metalImpactClip", AssetDatabase.LoadAssetAtPath<AudioClip>(AudioMetalImpactPath));
        }

        private static float Gaussian(float x, float z, float centerX, float centerZ, float radius, float amplitude)
        {
            float distanceSquared = (x - centerX) * (x - centerX) + (z - centerZ) * (z - centerZ);
            return Mathf.Exp(-distanceSquared / (2f * radius * radius)) * amplitude;
        }

        private static float TerrainHeight(Terrain terrain, float x, float z)
        {
            return terrain.SampleHeight(new Vector3(x, 0f, z));
        }

        private static void CreateRoadStrip(Terrain terrain, Material roadMaterial, Material shoulderMaterial, bool alongZ, float fixedCoordinate, float startCoordinate, float length)
        {
            const int segments = 13;
            float segmentLength = length / segments;
            for (int i = 0; i < segments; i++)
            {
                float coordinate = startCoordinate + segmentLength * (i + 0.5f);
                float offset = Mathf.Sin(i * 1.73f) * 0.75f;
                float width = 8.4f + Mathf.Sin(i * 2.31f) * 0.8f;
                float x = alongZ ? fixedCoordinate + offset : coordinate;
                float z = alongZ ? coordinate : fixedCoordinate + offset;
                float y = TerrainHeight(terrain, x, z) + 0.045f;
                Vector3 scale = alongZ
                    ? new Vector3(width, 0.08f, segmentLength + 0.8f)
                    : new Vector3(segmentLength + 0.8f, 0.08f, width);
                GameObject road = CreatePrimitive("Uneven Dirt Road", PrimitiveType.Cube, new Vector3(x, y, z), scale, roadMaterial, false);
                float rotation = alongZ ? offset * 0.55f : 90f + offset * 0.55f;
                road.transform.rotation = Quaternion.Euler(0f, rotation, 0f);

                for (int track = -1; track <= 1; track += 2)
                {
                    float trackOffset = 2.15f * track;
                    float trackX = alongZ ? x + trackOffset : x;
                    float trackZ = alongZ ? z : z + trackOffset;
                    Vector3 trackScale = alongZ
                        ? new Vector3(0.22f, 0.082f, segmentLength + 0.55f)
                        : new Vector3(segmentLength + 0.55f, 0.082f, 0.22f);
                    GameObject trackMark = CreatePrimitive("Track Mark", PrimitiveType.Cube, new Vector3(trackX, TerrainHeight(terrain, trackX, trackZ) + 0.048f, trackZ), trackScale, shoulderMaterial, false);
                    trackMark.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
                }

                for (int side = -1; side <= 1; side += 2)
                {
                    float edgeOffset = width * 0.56f * side;
                    float edgeX = alongZ ? x + edgeOffset : x;
                    float edgeZ = alongZ ? z : z + edgeOffset;
                    CreatePrimitive("Road Mud Edge", PrimitiveType.Cylinder, new Vector3(edgeX, TerrainHeight(terrain, edgeX, edgeZ) + 0.03f, edgeZ), new Vector3(1.8f, 0.035f, 0.75f), shoulderMaterial, false);
                }
            }
        }

        private static GameObject CreatePrimitive(string objectName, PrimitiveType type, Vector3 position, Vector3 scale, Material material, bool withCollider)
        {
            GameObject objectInstance = GameObject.CreatePrimitive(type);
            objectInstance.name = objectName;
            objectInstance.transform.position = position;
            objectInstance.transform.localScale = scale;
            Renderer renderer = objectInstance.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = material;
            if (!withCollider)
            {
                Collider collider = objectInstance.GetComponent<Collider>();
                if (collider != null) Object.DestroyImmediate(collider);
            }
            objectInstance.isStatic = true;
            return objectInstance;
        }

        private static GameObject CreateImportedEnvironmentModel(string assetPath, string objectName, Vector3 position, Vector3 scale, Material material, bool withCollider)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (asset == null)
            {
                Debug.LogWarning($"Environment donor model not available, using fallback: {assetPath}");
                return null;
            }
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset);
            instance.name = objectName;
            instance.transform.position = position;
            instance.transform.localScale = scale;
            PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))
                renderer.sharedMaterial = material;
            if (!withCollider)
            {
                foreach (Collider collider in instance.GetComponentsInChildren<Collider>(true))
                    Object.DestroyImmediate(collider);
            }
            foreach (Transform child in instance.GetComponentsInChildren<Transform>(true))
                child.gameObject.isStatic = true;
            return instance;
        }

        private static GameObject CreateBlockOnTerrain(string objectName, Terrain terrain, float x, float z, float width, float height, float depth, Material material)
        {
            return CreatePrimitive(objectName, PrimitiveType.Cube, new Vector3(x, TerrainHeight(terrain, x, z) + height * 0.5f, z), new Vector3(width, height, depth), material, true);
        }

        private static void CreateTankTrap(Terrain terrain, float x, float z, Material material)
        {
            GameObject trap = new("Metal Tank Trap");
            trap.transform.position = new Vector3(x, TerrainHeight(terrain, x, z), z);
            trap.AddComponent<ImpactSurface>();
            for (int i = 0; i < 3; i++)
            {
                GameObject bar = CreatePrimitive("Steel Beam", PrimitiveType.Cube, trap.transform.position + Vector3.up * 1.05f, new Vector3(0.28f, 2.2f, 0.28f), material, true);
                bar.transform.SetParent(trap.transform, true);
                bar.transform.Rotate(0f, i * 60f, i == 1 ? 58f : -58f, Space.Self);
            }
            trap.isStatic = true;
        }

        private static void CreateSandbagLine(Terrain terrain, float x, float z, Material material, bool rotate)
        {
            float y = TerrainHeight(terrain, x, z) + 0.32f;
            GameObject sandbag = CreatePrimitive("Sandbag", PrimitiveType.Capsule, new Vector3(x, y, z), new Vector3(0.72f, 0.32f, 0.42f), material, true);
            sandbag.transform.rotation = Quaternion.Euler(0f, rotate ? 90f : 0f, 0f);
        }

        private static void CreateCrateStack(Terrain terrain, float x, float z, Material material)
        {
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < 2 - row; column++)
                {
                    float crateX = x + (column - (1f - row) * 0.5f) * 1.05f;
                    float crateZ = z + row * 0.9f;
                    float crateY = TerrainHeight(terrain, crateX, crateZ) + 0.55f + row * 1.1f;
                    GameObject crate = CreatePrimitive("Military Supply Crate", PrimitiveType.Cube, new Vector3(crateX, crateY, crateZ), new Vector3(1f, 1.1f, 1f), material, true);
                    crate.transform.rotation = Quaternion.Euler(0f, row * 12f, 0f);
                }
            }
        }

        private static void CreateDestroyedWall(Terrain terrain, float x, float z, Material material)
        {
            for (int i = 0; i < 5; i++)
            {
                float pieceX = x + i * 1.15f;
                float height = 1.1f + (i % 3) * 0.45f;
                GameObject piece = CreateBlockOnTerrain("Destroyed Concrete Wall", terrain, pieceX, z + (i % 2) * 0.5f, 1.15f, height, 0.7f, material);
                piece.transform.rotation = Quaternion.Euler(0f, i * 9f, i % 2 == 0 ? 8f : -10f);
            }
        }

        private static void CreateRubblePile(Terrain terrain, float x, float z, Material concreteMaterial, Material rockMaterial)
        {
            for (int i = 0; i < 8; i++)
            {
                float offsetX = (i % 4 - 1.5f) * 0.75f;
                float offsetZ = (i / 4 - 0.5f) * 1.1f;
                float size = 0.35f + (i % 3) * 0.16f;
                Material material = i % 2 == 0 ? concreteMaterial : rockMaterial;
                GameObject rubble = CreatePrimitive("Rubble", PrimitiveType.Cube, new Vector3(x + offsetX, TerrainHeight(terrain, x + offsetX, z + offsetZ) + size * 0.5f, z + offsetZ), new Vector3(size * 1.5f, size, size), material, true);
                rubble.transform.rotation = Quaternion.Euler(i * 17f, i * 31f, i * 11f);
            }
        }

        private static Camera CreateCamera(Transform target)
        {
            GameObject cameraObject = new("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            camera.fieldOfView = 62f;
            ThirdPersonCamera follow = cameraObject.AddComponent<ThirdPersonCamera>();
            SetProperty(follow, "target", target);
            SetProperty(follow, "distance", 6.5f);
            SetProperty(follow, "height", 3.6f);
            SetProperty(follow, "lookHeight", 1.8f);
            cameraObject.transform.position = target.position - target.forward * 6.5f + Vector3.up * 3.6f;
            cameraObject.transform.LookAt(target.position + target.forward * 1.6f + Vector3.up * 1.8f);
            return camera;
        }

        private static void CreatePlayerUi(Health health, TankCannon cannon)
        {
            GameObject canvasObject = new("Player HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

            Sprite sprite = GetOrCreateUiSprite();
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            RectTransform background = CreateUiImage("Health Background", canvasObject.transform, sprite, new Color(0.025f, 0.04f, 0.05f, 0.9f));
            background.anchorMin = new Vector2(0.02f, 0.93f);
            background.anchorMax = new Vector2(0.22f, 0.968f);
            background.offsetMin = background.offsetMax = Vector2.zero;
            RectTransform fill = CreateUiImage("Fill", background, sprite, new Color(0.22f, 0.72f, 0.3f, 1f));
            fill.anchorMin = Vector2.zero;
            fill.anchorMax = Vector2.one;
            fill.offsetMin = fill.offsetMax = new Vector2(4f, 4f);
            Image fillImage = fill.GetComponent<Image>();
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;

            GameObject label = new("Health Text", typeof(RectTransform), typeof(Text));
            label.transform.SetParent(background, false);
            RectTransform labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;
            Text text = label.GetComponent<Text>();
            text.font = font;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 13;

            PlayerHealthUI ui = canvasObject.AddComponent<PlayerHealthUI>();
            SetProperty(ui, "playerHealth", health);
            SetProperty(ui, "healthFill", fillImage);
            SetProperty(ui, "healthText", text);

            Text weaponText = CreateText("Weapon Status", canvasObject.transform, font, 13, TextAnchor.MiddleCenter);
            RectTransform weaponRect = weaponText.rectTransform;
            weaponRect.anchorMin = new Vector2(0.4f, 0.06f);
            weaponRect.anchorMax = new Vector2(0.6f, 0.11f);
            weaponRect.offsetMin = weaponRect.offsetMax = Vector2.zero;
            WeaponStatusUI weaponUi = canvasObject.AddComponent<WeaponStatusUI>();
            SetProperty(weaponUi, "cannon", cannon);
            SetProperty(weaponUi, "statusText", weaponText);

            CreateCrosshair(canvasObject.transform, sprite, new Color(0.45f, 0.9f, 0.92f, 0.9f));
        }

        private static RectTransform CreateUiImage(string name, Transform parent, Sprite sprite, Color color)
        {
            GameObject imageObject = new(name, typeof(RectTransform), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            return imageObject.GetComponent<RectTransform>();
        }

        private static Text CreateText(string name, Transform parent, Font font, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            return text;
        }

        private static void CreateCrosshair(Transform parent, Sprite sprite, Color color)
        {
            RectTransform horizontal = CreateUiImage("Crosshair Horizontal", parent, sprite, color);
            horizontal.anchorMin = horizontal.anchorMax = new Vector2(0.5f, 0.5f);
            horizontal.sizeDelta = new Vector2(18f, 1.5f);
            horizontal.anchoredPosition = Vector2.zero;

            RectTransform vertical = CreateUiImage("Crosshair Vertical", parent, sprite, color);
            vertical.anchorMin = vertical.anchorMax = new Vector2(0.5f, 0.5f);
            vertical.sizeDelta = new Vector2(1.5f, 18f);
            vertical.anchoredPosition = Vector2.zero;
        }

        private static Sprite GetOrCreateUiSprite()
        {
            Sprite existingSprite = AssetDatabase.LoadAllAssetsAtPath(UiSpriteAssetPath).OfType<Sprite>().FirstOrDefault();
            if (existingSprite != null) return existingSprite;

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(UiSpriteAssetPath);
            if (texture == null)
            {
                texture = new Texture2D(1, 1, TextureFormat.RGBA32, false, true)
                {
                    name = "UiWhiteTexture"
                };
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();
                AssetDatabase.CreateAsset(texture, UiSpriteAssetPath);
            }

            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            sprite.name = "UiWhiteSprite";
            AssetDatabase.AddObjectToAsset(sprite, texture);
            AssetDatabase.SaveAssets();
            return sprite;
        }

        private static void SetProperty(Object target, string propertyName, Object value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetProperty(Object target, string propertyName, bool value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetProperty(Object target, string propertyName, float value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).floatValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetProperty(Object target, string propertyName, string value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).stringValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetProperty(Object target, string propertyName, Color value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).colorValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AddToBuildSettings(string scenePath)
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                if (scene.path == scenePath) return;
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };
        }
    }
}
