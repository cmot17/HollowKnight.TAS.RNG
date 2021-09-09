using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using System.Reflection;
using Assembly_CSharp.TasInfo.mm.Source.Utils;
using System.Runtime.CompilerServices;


namespace Assembly_CSharp.TasInfo.mm.Source {
    public static class RngManager {
        public static List<string[]> SavedTransitions = new();

        public static string FilePath;



        public static int TransitionNum = 0;

        public static string LastScene = "";

        public static string InfoOut = "";
        public static string GeneratorOut = "";
        public static string LastSceneOut = "";
        public static string NewSceneOut = "";
        public static string TransitionCountOut = "";
        public static bool SaveData = false;

        public readonly struct PublicState {
            [SerializeField]
            public readonly int s0;
            [SerializeField]
            public readonly int s1;
            [SerializeField]
            public readonly int s2;
            [SerializeField]
            public readonly int s3;
            public PublicState(int s0, int s1, int s2, int s3) {
                this.s0 = s0;
                this.s1 = s1;
                this.s2 = s2;
                this.s3 = s3;
            }
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
            //Debug.Log("OnInit Called");
            //Debug.Log(FilePath);
        }

        public static void OnRoomTransition(GameManager gameManager) {
            
            //Debug.Log("OnRoomTransition called");

            TransitionNum += 1;

            InfoOut = $"Room transition: {TransitionNum}";

            SavedTransitions.Clear();
            
            if (File.Exists(FilePath)) {
                string[] fileData = File.ReadAllLines(FilePath);
                foreach (string line in fileData) {
                    SavedTransitions.Add(line.Split(','));
                    //Debug.Log("Savedtransitions input");
                    //Debug.Log(line);
                }
            }

            if (SavedTransitions.Count >= TransitionNum) {
                if (SavedTransitions[TransitionNum - 1][0] == LastScene && SavedTransitions[TransitionNum - 1][1] == gameManager.sceneName) {
                    if (SavedTransitions[TransitionNum - 1].Length == 6) {
                        if (ConfigManager.LoadRng) {
                            PublicState newState = new PublicState(
                                 Convert.ToInt32(SavedTransitions[TransitionNum - 1][2]),
                                 Convert.ToInt32(SavedTransitions[TransitionNum - 1][3]),
                                 Convert.ToInt32(SavedTransitions[TransitionNum - 1][4]),
                                 Convert.ToInt32(SavedTransitions[TransitionNum - 1][5])
                            );

                            UnityEngine.Random.state = Reinterpret(newState);
                        }
                    }
                    else {
                        SaveData = true;
                    }
                }
            }
            else {
                SaveData = true;
            }

            RngInfo.lastState = UnityEngine.Random.state;
            RngInfo.rollTimes = 0;

            PublicState currentState = Reinterpret(UnityEngine.Random.state);
            
            //Debug.Log("Values added to internal list");
            
            LastSceneOut = $"LastScene={LastScene}";
            NewSceneOut = $"NewScene={gameManager.sceneName}";
            GeneratorOut = $"GeneratorState={currentState.s0},{currentState.s1},{currentState.s2},{currentState.s3}";
            TransitionCountOut = $"TransitionCount={TransitionNum}";
            
                       

            LastScene = gameManager.sceneName;

            //Debug.Log("Transition info saved to output");
        }

        public static void OnPreRender(StringBuilder infoBuilder) {
            FilePath = Path.GetDirectoryName(Application.dataPath) + "/" + ConfigManager.SaveRngFile;
            //Debug.Log("OnPreRender called");
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

            //Debug.Log("OnPreRender finished");
        }
    }
}