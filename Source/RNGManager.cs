using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;
using Assembly_CSharp.TasInfo.mm.Source.Extensions;
using Assembly_CSharp.TasInfo.mm.Source.Utils;

namespace Assembly_CSharp.TasInfo.mm.Source {
    public static class RNGManager {
        public static List<string> Transitions = new();

        private static bool exiting = false;

        public static int TransitionNum = 0;

        public static string InfoOut = "";

        public static void OnRoomTransition(GameManager gameManager) {
            TransitionNum += 1;
            InfoOut = $"Room transition: {TransitionNum}";
            
            if (gameManager.gameState == GlobalEnums.GameState.ENTERING_LEVEL && exiting) {
                
            }

            if (gameManager.gameState == GlobalEnums.GameState.EXITING_LEVEL) {
                exiting = true;
            } else {
                exiting = false;
            }
        }

        public static void OnPreRender(StringBuilder infoBuilder) {
            infoBuilder.Append(InfoOut);
        }
    }
}