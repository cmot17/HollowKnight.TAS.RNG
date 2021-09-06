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

        public static string FilePath = Path.GetDirectoryName(Application.dataPath) + "/tas_rng.csv";

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
            Debug.Log("OnInit Called");
            Debug.Log(FilePath);
        }

        public static void OnRoomTransition(GameManager gameManager) {
            Debug.Log("OnRoomTransition called");

            TransitionNum += 1;

            InfoOut = $"Room transition: {TransitionNum}";

            SavedTransitions.Clear();

            if (File.Exists(FilePath)) {
                string[] fileData = File.ReadAllLines(FilePath);
                foreach (string line in fileData) {
                    SavedTransitions.Add(line.Split(','));
                    Debug.Log("Savedtransitions input");
                    Debug.Log(line);
                }
            }

            Debug.Log($"Savedtransitions count: {SavedTransitions.Count}");

            PublicState currentState = Reinterpret(UnityEngine.Random.state);
            
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

            Debug.Log("RNG has been set to saved values");

            Transitions.Add(new string[] {
                    LastScene,
                    gameManager.sceneName,
                    currentState.s0.ToString(),
                    currentState.s1.ToString(),
                    currentState.s2.ToString(),
                    currentState.s3.ToString()
            });

            Debug.Log("Values added to internal list");

            LastSceneOut = $"LastScene={LastScene}";
            NewSceneOut = $"NewScene={gameManager.sceneName}";
            GeneratorOut = $"GeneratorState={currentState.s0.ToString()},{currentState.s1.ToString()},{currentState.s2.ToString()},{currentState.s3.ToString()}";
            TransitionCountOut = $"TransitionCount={TransitionNum}";

            SaveData = true;

            LastScene = gameManager.sceneName;

            Debug.Log("Transition info saved to output");
        }

        public static void OnPreRender(StringBuilder infoBuilder) {
            Debug.Log("OnPreRender called");
            infoBuilder.AppendLine(InfoOut);
            infoBuilder.AppendLine(LastSceneOut);
            infoBuilder.AppendLine(NewSceneOut);
            infoBuilder.AppendLine(GeneratorOut);
            infoBuilder.AppendLine(TransitionCountOut);
            if (SaveData) {
                infoBuilder.AppendLine("SaveData=1");
            } 
            else
            {
                infoBuilder.AppendLine("SaveData=0");
            }

            SaveData = false;
            Debug.Log("OnPreRender finished");
        }
    }
}