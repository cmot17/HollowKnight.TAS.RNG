// ReSharper disable All
using MonoMod;
using GlobalEnums;
using UnityEngine;
using Assembly_CSharp.TasInfo.mm.Source;

#pragma warning disable CS0649, CS0414

public class patch_GameManager : GameManager {
    private static readonly long TasInfoMark = 1234567890123456789;
    public static string TasInfo;


    [MonoModIgnore]
    public UIManager ui { get; private set; }

    [MonoModIgnore]
    private GameCameras gameCams;

    [MonoModIgnore]
    private extern void EnterHero(bool additiveGateSearch);

    private void BeginScene() {
        this.inputHandler.SceneInit();
        this.ui.SceneInit();
        if (this.hero_ctrl) {
            this.hero_ctrl.SceneInit();
        }
        this.gameCams.SceneInit();
        if (this.IsMenuScene()) {
            this.SetState(GameState.MAIN_MENU);
        } else if (this.IsGameplayScene()) {
            Debug.Log("About to call OnRoomTransition");
            RNGManager.OnRoomTransition(this);
            Debug.Log("OnRoomTransition finished");
            this.SetState(GameState.ENTERING_LEVEL);
            this.playerData.disablePause = false;
            this.inputHandler.AllowPause();
            this.inputHandler.StartAcceptingInput();
            this.EnterHero(true);
        } else if (this.IsNonGameplayScene()) {
            this.SetState(GameState.CUTSCENE);
        } else {
            Debug.LogError("GM - Scene type is not set to a standard scene type.");
        }
        if (this.ui != null) {
            this.ui.SetUIStartState(this.gameState);
        } else {
            this.ui = UnityEngine.Object.FindObjectOfType<UIManager>();
            if (this.ui != null) {
                this.ui.SetUIStartState(this.gameState);
            } else {
                Debug.LogError("GM: Could not find the UI manager in this scene.");
                Debug.LogError("GM: Could not find the UI manager in this scene.");
            }
        }
        Debug.Log("BeginScene finished");
    }

#if V1028
    [MonoModIgnore]
    private extern void orig_ManualLevelStart();
    private void ManualLevelStart() {
        orig_ManualLevelStart();
        Assembly_CSharp.TasInfo.mm.Source.TasInfo.AfterManualLevelStart();
    }
#endif
}