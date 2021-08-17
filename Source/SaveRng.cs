using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using Assembly_CSharp.TasInfo.mm.Source.Extensions;
using Assembly_CSharp.TasInfo.mm.Source.Utils;

namespace Assembly_CSharp.TasInfo.mm.Source {
    public static class SaveRng {
        public static List<string> transitions = new List<string>();
        public static void OnPreRender(GameManager gameManager, StringBuilder infoBuilder) 
        {
            bool exiting = false;

            if (gameManager.gameState == GlobalEnums.GameState.EXITING_LEVEL) 
            {
                exiting = true;
            }
            else if (gameManager.gameState == GlobalEnums.GameState.ENTERING_LEVEL && exiting) 
            {
                transitionNum += 1;
                string stateJson = JsonUtility.ToJson(UnityEngine.Random.state);
                int[] stateInds = new int[] { stateJson.IndexOf("s0"), stateJson.IndexOf("s1"), stateJson.IndexOf("s2"), stateJson.IndexOf("s3") };
                string[] stateStrings = new string[] { stateJson.Substring((stateInds[0] + 4), (stateInds[1] - stateInds[0] - 6)), stateJson.Substring((stateInds[1] + 4), (stateInds[2] - stateInds[1] - 6)), stateJson.Substring((stateInds[2] + 4), (stateInds[3] - stateInds[2] - 6)), stateJson.Substring((stateInds[3] + 4), (stateJson.Length - stateInds[3] - 5)) };

                string rngString = "\n" + stateStrings[0] + "\n" + stateStrings[1] + "\n" + stateStrings[2] + "\n" + stateStrings[3];
                infoBuilder.Append(rngString);
                exiting = false;
            }
        }
    }
}
