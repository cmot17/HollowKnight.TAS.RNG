using System.Text;
using UnityEngine;
using System.Reflection;
using Assembly_CSharp.TasInfo.mm.Source.Utils;

namespace Assembly_CSharp.TasInfo.mm.Source {
    public static class RngInfo {
        public static ulong rollTimes = 0;
        public static Random.State lastState;

        public static void OnInit() {
            lastState = Random.state;
        }

        public static void OnPreRender(StringBuilder infoBuilder) {
            Random.State origState = Random.state;
            Random.state = lastState;
            int increaseTimes = 0;
            //Debug.Log("Pre While Loop");
            while (!origState.Equals(Random.state)) {
                float _ = Random.value;
                rollTimes++;
                increaseTimes++;
            }
            lastState = Random.state;
            //Debug.Log("Post While Loop");

            if (ConfigManager.ShowRngCalls) {
                infoBuilder.AppendLine($"RNG: {rollTimes} +{increaseTimes}");

            }

            Random.state = origState;

            if (ConfigManager.ShowRngState) {
                //Random.State st = Random.state;
                RngManager.PublicState currentState = RngManager.Reinterpret(Random.state);
                infoBuilder.AppendLine($"S0: {currentState.s0}");
                infoBuilder.AppendLine($"S1: {currentState.s1}");
                infoBuilder.AppendLine($"S2: {currentState.s2}");
                infoBuilder.AppendLine($"S3: {currentState.s3}");
            }
        }

    }

}