using System.Text;
using UnityEngine;
using System.Reflection;
using Assembly_CSharp.TasInfo.mm.Source.Extensions;

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

            if (ConfigManager.ShowRngState) {
                infoBuilder.AppendLine($"S0: {typeof(Random.State).GetField("s0", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Random.state)}");
                infoBuilder.AppendLine($"S1: {typeof(Random.State).GetField("s1", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Random.state)}");
                infoBuilder.AppendLine($"S2: {typeof(Random.State).GetField("s2", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Random.state)}");
                infoBuilder.AppendLine($"S3: {typeof(Random.State).GetField("s3", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Random.state)}");
            }
        }

    }

}