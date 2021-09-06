using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using System.Reflection;
using Assembly_CSharp.TasInfo.mm.Source.Utils;
using System.Runtime.CompilerServices;


namespace Assembly_CSharp.TasInfo.mm.Source {
    public static class RNGManager {
        public static List<string[]> SavedTransitions = new();

        public static List<string[]> Transitions = new();

        public static int TransitionNum = 0;

        public static string LastScene = "";

        public static string InfoOut = "";
        public static string GeneratorOut = "";
        public static string LastSceneOut = "";
        public static string NewSceneOut = "";
        public static string TransitionCountOut = "";
        public static bool SaveData = false;

        public struct PublicState {
            [SerializeField]
            public int s0;
            [SerializeField]
            public int s1;
            [SerializeField]
            public int s2;
            [SerializeField]
            public int s3;
        }

        public static PublicState Reinterpret(UnityEngine.Random.State st) {
            unsafe {
                return *(PublicState*)(void*)&st;
            }
        }

        public static UnityEngine.Random.State Reinterpret(PublicState st) {
            unsafe {
                return *(UnityEngine.Random.State*)(void*)&st;
            }
        }

        public static void OnInit() {
            var path = Path.GetDirectoryName(Application.dataPath) + "/tas_rng.csv";
            if (File.Exists(path)) {
                string[] fileData = File.ReadAllLines(path);
                foreach (string line in fileData) {
                    SavedTransitions.Add(line.Split(','));
                }
            }
        }

        public static void OnRoomTransition(GameManager gameManager) {
            TransitionNum += 1;
            InfoOut = $"Room transition: {TransitionNum}";
            //UnityEngine.Random.State st = UnityEngine.Random.state;
            PublicState currentState = Reinterpret(UnityEngine.Random.state);
            Debug.Log($"Savedtransitions count: {SavedTransitions.Count}");
            if (SavedTransitions.Count > 0) {
                if (SavedTransitions[TransitionNum - 1][0] == LastScene && SavedTransitions[TransitionNum - 1][1] == gameManager.sceneName) {
                    if (SavedTransitions[TransitionNum - 1].Length == 6) {
                        PublicState savedState = new PublicState {
                            s0 = Convert.ToInt32(SavedTransitions[TransitionNum - 1][2]),
                            s1 = Convert.ToInt32(SavedTransitions[TransitionNum - 1][3]),
                            s2 = Convert.ToInt32(SavedTransitions[TransitionNum - 1][4]),
                            s3 = Convert.ToInt32(SavedTransitions[TransitionNum - 1][5])
                        };
                        UnityEngine.Random.state = Reinterpret(savedState);
                    }
                }
            }

            //Debug.Log(Application.persistentDataPath + "test.csv");
            Transitions.Add(new string[] {
                    LastScene,
                    gameManager.sceneName,
                    currentState.s0.ToString(),
                    currentState.s1.ToString(),
                    currentState.s2.ToString(),
                    currentState.s3.ToString()
            });

            List<string[]> combinedTransitions = new();
            combinedTransitions.AddRange(Transitions);
            if (SavedTransitions.Count > TransitionNum) {
                combinedTransitions.AddRange(SavedTransitions.GetRange(TransitionNum, SavedTransitions.Count - TransitionNum - 1));
            }

            List<string> tempFile = new();

            foreach (string[] line in combinedTransitions) {
                string lineString = "";
                for (int i = 0; i < line.Length - 1; i++) {
                    lineString += line[i];
                    lineString += ",";
                }
                lineString += line[line.Length - 1];
                tempFile.Add(lineString);
            }


            LastSceneOut = $"LastScene={LastScene}";
            NewSceneOut = $"NewScene={gameManager.sceneName}";
            GeneratorOut = $"GeneratorState={currentState.s0.ToString()},{currentState.s1.ToString()},{currentState.s2.ToString()},{currentState.s3.ToString()}";
            TransitionCountOut = $"TransitionCount={TransitionNum}";
            SaveData = true;

            //File.WriteAllLines(Application.persistentDataPath + "/rng_state.csv", tempFile.ToArray());

            LastScene = gameManager.sceneName;
        }

        public static void OnPreRender(StringBuilder infoBuilder) {
            infoBuilder.Append(InfoOut);
            infoBuilder.Append(LastSceneOut);
            infoBuilder.Append(NewSceneOut);
            infoBuilder.Append(GeneratorOut);
            infoBuilder.Append(TransitionCountOut);
            if (SaveData) {
                infoBuilder.Append("SaveData=1");
            } 
            else
            {
                infoBuilder.Append("SaveData=0");
            }

            SaveData = false;
        }
    }
}