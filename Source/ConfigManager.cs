using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assembly_CSharp.TasInfo.mm.Source {
    internal static class ConfigManager {
        private const string ConfigFile = "./HollowKnightTasInfo.config";
        private static string defaultContent = @"
[Settings]
Enabled = true

ShowKnightInfo = true
ShowCustomInfo = true
ShowSceneName = true
ShowTime = true
ShowRng = true
ShowRngState = true

ShowEnemyHp = true
ShowEnemyPosition = true
ShowEnemyVelocity = true

ShowHitbox = true
ShowOtherHitbox = false

PositionPrecision = 5
VelocityPrecision = 3

SetRngFrame = -1
SetRngS0 = 1
SetRngS1 = 1812433254
SetRngS2 = 1900727103
SetRngS3 = -603986212

# 碰撞箱颜色 ARGB 格式，注释或删除则不显示该类 hitbox
KnightHitbox = 0xFF00FF00
AttackHitbox = 0xFF00FFFF
EnemyHitbox = 0xFFFF0000
HarmlessHitbox = 0xFFFFFF00
TriggerHitbox = 0xFFBB99FF
TerrainHitbox = 0xFFFF8844
OtherHitbox = 0xFFFFFFFF

# 默认为 1，数值越大视野越广
CameraZoom = 1
CameraFollow = false
DisableCameraShake = false

[CustomInfoTemplate]
# 该配置用于定制附加显示的数据，需要注意如果调用属性或者方法有可能会造成 desync
# 例如 HeroController.CanJump() 会修改 ledgeBufferSteps 字段，请查看源码确认是否安全。定制数据格式如下：
# {UnityObject子类名.字段/属性/方法.字段/属性/方法……}
# {GameObjectName.字段/属性/方法.字段/属性/方法……}
# 只支持无参方法以及字符串作为唯一参数的方法
# 常用的类型 PlayerData 和 HeroControllerStates 可以简写
# 支持配置多行，并且同一行可以存在多个 {}
# paused: {GameManager.isPaused}
# canAttack: {HeroController.CanAttack()}
# geo: {HeroController.playerData.geo}
# geo: {PlayerData.geo}
# jumping: {HeroControllerStates.jumping}
# component: {Crawler Fixed.GetComponentInChildren(BoxCollider2D)}
# crawler hp: {Crawler Fixed.LocateMyFSM(health_manager_enemy).FsmVariables.FindFsmInt(HP)}
";

        private static DateTime lastWriteTime;
        private static readonly Dictionary<string, string> Settings = new();
        public static string CustomInfoTemplate { get; private set; } = string.Empty;
        public static bool Enabled => GetSettingValue<bool>(nameof(Enabled));
        public static bool ShowCustomInfo => Enabled && GetSettingValue<bool>(nameof(ShowCustomInfo));
        public static bool ShowKnightInfo => Enabled && GetSettingValue<bool>(nameof(ShowKnightInfo));
        public static bool ShowSceneName => Enabled && GetSettingValue<bool>(nameof(ShowSceneName));
        public static bool ShowTime => Enabled && GetSettingValue<bool>(nameof(ShowTime));
        public static bool ShowRng => Enabled && GetSettingValue<bool>(nameof(ShowRng));
        public static bool ShowRngState => Enabled && GetSettingValue<bool>(nameof(ShowRngState));
        public static bool ShowEnemyHp => Enabled && GetSettingValue<bool>(nameof(ShowEnemyHp));
        public static bool ShowEnemyPosition => Enabled && GetSettingValue<bool>(nameof(ShowEnemyPosition));
        public static bool ShowEnemyVelocity => Enabled && GetSettingValue<bool>(nameof(ShowEnemyVelocity));
        public static bool ShowHitbox => Enabled && GetSettingValue<bool>(nameof(ShowHitbox));
        public static bool ShowOtherHitbox => Enabled && GetSettingValue<bool>(nameof(ShowOtherHitbox));
        public static int PositionPrecision => GetSettingValue(nameof(PositionPrecision), 5);
        public static int VelocityPrecision => GetSettingValue(nameof(VelocityPrecision), 3);
        public static long SetRngFrame => GetSettingValue(nameof(SetRngFrame), -1);
        public static long SetRngS0 => GetSettingValue(nameof(SetRngS0), 1);
        public static long SetRngS1 => GetSettingValue(nameof(SetRngS1), 1812433254);
        public static long SetRngS2 => GetSettingValue(nameof(SetRngS2), 1900727103);
        public static long SetRngS3 => GetSettingValue(nameof(SetRngS3), -603986212);
        public static float CameraZoom => Enabled ? GetSettingValue(nameof(CameraZoom), 1f) : 1f;
        public static bool CameraFollow => Enabled && GetSettingValue<bool>(nameof(CameraFollow));
        public static bool DisableCameraShake => Enabled && GetSettingValue<bool>(nameof(DisableCameraShake));
        public static bool IsCameraZoom => CameraZoom > 0f && Math.Abs(CameraZoom - 1f) > 0.001;

        public static string GetHitboxColorValue(HitboxInfo.HitboxType hitboxType) {
            return GetSettingValue($"{hitboxType}Hitbox", string.Empty);
        }

        public static void OnPreRender() {
            TryParseConfigFile();
        }

        private static T GetSettingValue<T>(string settingName, T defaultValue = default) {
            if (Settings.ContainsKey(settingName)) {
                string value = Settings[settingName];
                if (string.IsNullOrEmpty(value)) {
                    return defaultValue;
                }

                try {
                    return (T) (typeof(T).IsEnum ? Enum.Parse(typeof(T), value, true) : Convert.ChangeType(value, typeof(T)));
                } catch {
                    return defaultValue;
                }
            } else {
                return defaultValue;
            }
        }

        private static void TryParseConfigFile() {
            if (!File.Exists(ConfigFile)) {
                File.WriteAllText(ConfigFile, defaultContent);
            }

            DateTime writeTime = File.GetLastWriteTime(ConfigFile);
            if (lastWriteTime != writeTime) {
                lastWriteTime = writeTime;
                CustomInfoTemplate = string.Empty;
                Settings.Clear();

                IEnumerable<string> contents = File.ReadAllLines(ConfigFile)
                    .Select(s => s.Trim()).Where(line => !line.StartsWith("#") && !string.IsNullOrEmpty(line));

                bool customInfoSection = false;
                bool settingsSection = false;
                foreach (string content in contents) {
                    switch (content) {
                        case "[CustomInfoTemplate]":
                            customInfoSection = true;
                            settingsSection = false;
                            continue;
                        case "[Settings]":
                            customInfoSection = false;
                            settingsSection = true;
                            continue;
                    }

                    if (customInfoSection) {
                        if (string.IsNullOrEmpty(CustomInfoTemplate)) {
                            CustomInfoTemplate = content;
                        } else {
                            CustomInfoTemplate += $"\n{content}";
                        }
                    } else if (settingsSection) {
                        string[] keyValue = content.Split('=').Select(s => s.Trim()).ToArray();
                        if (keyValue.Length != 2) {
                            continue;
                        }
                        Settings[keyValue[0]] = keyValue[1];
                    }
                }
            }
        }
    }
}