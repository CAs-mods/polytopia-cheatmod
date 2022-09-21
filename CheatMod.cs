using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace CheatMod {
    [BepInPlugin("xyz.div72.CheatMod", "CheatMod", "1.0.0")]
    public class CheatMod : BaseUnityPlugin {
        private bool showMenu = false;

        private static bool enableAllTribes = false;
        private static bool revealAllTiles = false;

        private void Awake() {
            Harmony.CreateAndPatchAll(typeof(CheatMod));
            Logger.LogInfo("Loaded CheatMod!");
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F6)) {
                showMenu = !showMenu;
            }

            // Disable cheats in multiplayer games.
            if (GameManager.GameState != null) {
                if (GameManager.GameState.CurrentState == GameState.State.Started && (int)GameManager.GameState.Settings.GameType == 1) {
                    showMenu = false;
                    enableAllTribes = false;
                    revealAllTiles = false;
                }
            }
        }

        private void OnGUI() {
            if (showMenu) {
                if (GUILayout.Button("Unlock DLC Tribes")) {
                    enableAllTribes = true;
                }

                if (GUILayout.Button("Toggle Tile Visibility")) {
                    revealAllTiles = !revealAllTiles;
                }

                if (GUILayout.Button("Add 20 stars")) {
                    if (GameManager.LocalPlayer != null) {
                        GameManager.LocalPlayer.Currency += 20;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PurchaseManager), nameof(PurchaseManager.IsTribeUnlocked))]
        [HarmonyPrefix]
        static bool PatchTribeUnlockStatus(ref bool __result) {
            if (enableAllTribes) {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(TileData), nameof(TileData.GetExplored))]
        [HarmonyPrefix]
        static bool PatchTileVisibility(ref bool __result, byte playerId) {
            if (!revealAllTiles) {
                return true;
            }

            if (playerId != GameManager.LocalPlayer.Id) {
                return true;
            }

            __result = true;
            return false;
        }
    }
}
