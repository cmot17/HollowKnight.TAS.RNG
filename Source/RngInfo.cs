using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

namespace Assembly_CSharp.TasInfo.mm.Source {
    public static class RngInfo {
        private static ulong rollTimes = 0;
        private static Random.State lastState;

        public static void OnInit() {
            lastState = Random.state;
        }

        public static void OnPreRender(StringBuilder infoBuilder) {
            Random.State origState = Random.state;
            Random.state = lastState;
            int increaseTimes = 0;
            while (!origState.Equals(Random.state)) {
                float _ = Random.value;
                rollTimes++;
                increaseTimes++;
            }
            lastState = Random.state;

            
            if (ConfigManager.ShowRng) {
                infoBuilder.AppendLine($"RNG: {rollTimes} +{increaseTimes}");
                
            }

            Random.state = origState;
            int setFrame = -1;

            if (ConfigManager.SetRngFrame == Time.frameCount && setFrame != Time.frameCount) 
            {
                setFrame = Time.frameCount;
                Random.state = JsonUtility.FromJson<Random.State>("{\"s0\":" + ConfigManager.SetRngS0 + ",\"s1\":" + ConfigManager.SetRngS1 + ",\"s2\":" + ConfigManager.SetRngS2 + ",\"s3\":" + ConfigManager.SetRngS3 + "}");
                lastState = JsonUtility.FromJson<Random.State>("{\"s0\":" + ConfigManager.SetRngS0 + ",\"s1\":" + ConfigManager.SetRngS1 + ",\"s2\":" + ConfigManager.SetRngS2 + ",\"s3\":" + ConfigManager.SetRngS3 + "}");
                rollTimes = 0;
            }
            string stateJson = JsonUtility.ToJson(Random.state);
            int[] stateInds = new int[] { stateJson.IndexOf("s0"), stateJson.IndexOf("s1"), stateJson.IndexOf("s2"), stateJson.IndexOf("s3") };
            string[] stateStrings = new string[] { stateJson.Substring((stateInds[0] + 4), (stateInds[1] - stateInds[0] - 6)), stateJson.Substring((stateInds[1] + 4), (stateInds[2] - stateInds[1] - 6)), stateJson.Substring((stateInds[2] + 4), (stateInds[3] - stateInds[2] - 6)), stateJson.Substring((stateInds[3] + 4), (stateJson.Length - stateInds[3] - 5)) };

            if (ConfigManager.ShowRngState) 
            {
                infoBuilder.AppendLine($"S0: {stateStrings[0]}");
                infoBuilder.AppendLine($"S1: {stateStrings[1]}");
                infoBuilder.AppendLine($"S2: {stateStrings[2]}");
                infoBuilder.AppendLine($"S3: {stateStrings[3]}");
            }
        }

    }
   
}