using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FPLibrary;
using UFE3D;

public class GlobalEditorWindow : EditorWindow {
	public static GlobalEditorWindow globalEditorWindow;
	private GlobalInfo globalInfo;
	private Vector2 scrollPos;

	private bool advancedOptions;
	private bool preloadOptions;
	private bool cameraOptions;
	private bool characterRotationOptions;
	private bool languageOptions;
	private bool guiScreensOptions;
	private bool screenOptions;
	private bool roundOptions;
	private bool bounceOptions;
	private bool counterHitOptions;
	private bool comboOptions;
	private bool debugOptions;
	private bool aiOptions;
	private bool blockOptions;
	private bool knockDownOptions;
	private bool hitOptions;
	private bool inputsOptions;
	private bool networkOptions;
	private bool player1InputOptions;
	private bool player2InputOptions;
	private bool cInputOptions;
	private bool touchControllerOptions;
	private bool stageOptions;
    private bool storyModeOptions;
    private bool trainingModeOptions;
    private bool challengeModeOptions;
	private bool storyModeSelectableCharactersInStoryModeOptions;
	private bool storyModeSelectableCharactersInVersusModeOptions;
	private bool characterOptions;
    private GameObject canvasPreview;
    private GameObject eventSystemPreview;
    private UFEScreen screenPreview;
	
	private string titleStyle;
	private string addButtonStyle;
	private string borderBarStyle;
	private string rootGroupStyle;
	private string fillBarStyle1;
	private string subGroupStyle;
	private string arrayElementStyle;
	private string subArrayElementStyle;
	private string foldStyle;
	private string enumStyle;
	private GUIStyle labelStyle;
	private GUIContent helpGUIContent = new GUIContent();
	private string pName;


	[MenuItem("Window/U.F.E./Global Editor")]
	public static void Init(){
		globalEditorWindow = EditorWindow.GetWindow<GlobalEditorWindow>(false, "Global", true);
		globalEditorWindow.Show();
		globalEditorWindow.Populate();
	}
	
	void OnSelectionChange(){
		Populate();
		Repaint();
	}
	
	void OnEnable(){
		Populate();
	}
	
	void OnFocus(){
		Populate();
	}
	
	void OnDisable(){
		Clear();
	}
	
	void OnDestroy(){
		Clear();
	}
	
	void OnLostFocus(){
		//Clear();
	}
	
	void Update(){
		if (EditorApplication.isPlayingOrWillChangePlaymode) {
			Clear();
		}
	}

	void Clear(){
		if (globalInfo != null){
            CloseGUICanvas();
		}
	}

	void helpButton(string page){
		if (GUILayout.Button("?", GUILayout.Width(18), GUILayout.Height(18))) 
			Application.OpenURL("http://www.ufe3d.com/doku.php/"+ page);
	}

    void Populate() {
        this.titleContent = new GUIContent("Global", (Texture)Resources.Load("Icons/Global"));

		// Style Definitions
		titleStyle = "MeTransOffRight";
		borderBarStyle = "ProgressBarBack";
		addButtonStyle = "CN CountBadge";
		rootGroupStyle = "GroupBox";
		subGroupStyle = "ObjectFieldThumb";
		arrayElementStyle = "flow overlay box";
		fillBarStyle1 = "ProgressBarBar";
		subArrayElementStyle = "HelpBox";
		foldStyle = "Foldout";
		enumStyle = "MiniPopup";
		
		labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.fontStyle = FontStyle.Bold;
		labelStyle.normal.textColor = Color.white;


		helpGUIContent.text = "";
		helpGUIContent.tooltip = "Open Live Docs";
		
		UnityEngine.Object[] selection = Selection.GetFiltered(typeof(GlobalInfo), SelectionMode.Assets);
		if (selection.Length > 0){
			if (selection[0] == null) return;
			globalInfo = (GlobalInfo) selection[0];
        }

        UFE.isControlFreak1Installed = UFE.IsInstalled("TouchController");				// [DGT]
        UFE.isControlFreak2Installed = UFE.IsInstalled("ControlFreak2.UFEBridge");
        UFE.isControlFreakInstalled = UFE.isControlFreak1Installed || UFE.isControlFreak2Installed;
        UFE.isRewiredInstalled = UFE.IsInstalled("Rewired.Integration.UniversalFightingEngine.RewiredUFEInputManager");
		UFE.isCInputInstalled = UFE.IsInstalled("cInput");

#if UFE_LITE
		UFE.isAiAddonInstalled = false;
#else
        UFE.isAiAddonInstalled = UFE.IsInstalled("RuleBasedAI");
#endif

#if UFE_LITE || UFE_BASIC
		UFE.isNetworkAddonInstalled = false;
		UFE.isPhotonInstalled = false;
        UFE.isBluetoothAddonInstalled = false;
#else
        UFE.isNetworkAddonInstalled = UFE.IsInstalled("UnetHighLevelMultiplayerAPI");
        UFE.isPhotonInstalled = UFE.IsInstalled("PhotonMultiplayerAPI");
        UFE.isBluetoothAddonInstalled = UFE.IsInstalled("BluetoothMultiplayerAPI");
#endif

        //versionUpdate();

    }

        public void OnGUI(){
		if (globalInfo == null){
			GUILayout.BeginHorizontal("GroupBox");
            GUILayout.Label("Select a Global Configuration file\nor create a new one.", "CN EntryInfo");
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button("Create new Global Configuration"))
				ScriptableObjectUtility.CreateAsset<UFE3D.CharacterInfo> ();
			return;
		}


		GUIStyle fontStyle = new GUIStyle();
        //fontStyle.font = (Font)EditorGUIUtility.Load("EditorFont.TTF");
        fontStyle.font = (Font)Resources.Load("EditorFont");
		fontStyle.fontSize = 30;
		fontStyle.alignment = TextAnchor.UpperCenter;
		fontStyle.normal.textColor = Color.white;
		fontStyle.hover.textColor = Color.white;
		EditorGUILayout.BeginVertical(titleStyle);{
			EditorGUILayout.BeginHorizontal();{
				EditorGUILayout.LabelField("", (globalInfo.gameName == ""? "Universal Fighting Engine":globalInfo.gameName) , fontStyle, GUILayout.Height(32));
				helpButton("global:start");
			}EditorGUILayout.EndHorizontal();
		}EditorGUILayout.EndVertical();
		
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);{
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUIUtility.labelWidth = 120;
				globalInfo.gameName = EditorGUILayout.TextField("Project Name:", globalInfo.gameName);
				EditorGUILayout.Space();

				EditorGUIUtility.labelWidth = 200;

				EditorGUILayout.Space();


				EditorGUIUtility.labelWidth = 150;
			}EditorGUILayout.EndVertical();

			
			// Debug Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					debugOptions = EditorGUILayout.Foldout(debugOptions, "Debug Options", foldStyle);
					helpButton("global:debugoptions");
				}EditorGUILayout.EndHorizontal();
				
				if (debugOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						EditorGUIUtility.labelWidth = 220;

                        EditorGUILayout.Space();
                        SubGroupTitle("Debug Info");

                        globalInfo.debugOptions.preloadedObjects = EditorGUILayout.Toggle("Preload Info (console)", globalInfo.debugOptions.preloadedObjects);
                        globalInfo.debugOptions.debugMode = EditorGUILayout.Toggle("Display Character Debug", globalInfo.debugOptions.debugMode);
                        
                        EditorGUI.BeginDisabledGroup(!globalInfo.debugOptions.debugMode);{
                            globalInfo.debugOptions.trainingModeDebugger = EditorGUILayout.Toggle("Display in Training Mode Only", globalInfo.debugOptions.trainingModeDebugger);
                            CharacterDebugOptions(globalInfo.debugOptions.p1DebugInfo, "Player 1 Debugger");
                            CharacterDebugOptions(globalInfo.debugOptions.p2DebugInfo, "Player 2 Debugger");

                            EditorGUI.BeginDisabledGroup(!UFE.isNetworkAddonInstalled);{
                                globalInfo.debugOptions.networkToggle = EditorGUILayout.Foldout(globalInfo.debugOptions.networkToggle, "Network Info", foldStyle);
                                if (globalInfo.debugOptions.networkToggle) {
                                    EditorGUILayout.BeginVertical(subGroupStyle);
                                    {
                                        EditorGUI.indentLevel += 1;

                                        globalInfo.debugOptions.ping = EditorGUILayout.Toggle("Ping", globalInfo.debugOptions.ping);
                                        globalInfo.debugOptions.frameDelay = EditorGUILayout.Toggle("Frame Delay", globalInfo.debugOptions.frameDelay);
                                        globalInfo.debugOptions.currentLocalFrame = EditorGUILayout.Toggle("Current Local Frame", globalInfo.debugOptions.currentLocalFrame);
                                        globalInfo.debugOptions.currentNetworkFrame = EditorGUILayout.Toggle("Current Network Frame", globalInfo.debugOptions.currentNetworkFrame);
                                        globalInfo.debugOptions.connectionLog = EditorGUILayout.Toggle("Connection Log (console)", globalInfo.debugOptions.connectionLog);
                                        globalInfo.debugOptions.desyncErrorLog = EditorGUILayout.Toggle("Desync Error Log (console)", globalInfo.debugOptions.desyncErrorLog);
                                        globalInfo.debugOptions.stateTrackerTest = EditorGUILayout.Toggle("State Tracker Test", globalInfo.debugOptions.stateTrackerTest);

                                        EditorGUI.indentLevel -= 1;
                                        EditorGUILayout.Space();
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }EditorGUI.EndDisabledGroup();

                        }EditorGUI.EndDisabledGroup();

                        EditorGUILayout.Space();
                        SubGroupTitle("Debug Mode");
                        
                        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
						globalInfo.debugOptions.startGameImmediately = EditorGUILayout.Toggle("Start Game Immediately", globalInfo.debugOptions.startGameImmediately);
						EditorGUI.BeginDisabledGroup(!globalInfo.debugOptions.startGameImmediately);{
                            globalInfo.debugOptions.skipLoadingScreen = EditorGUILayout.Toggle("Skip Loading Screen", globalInfo.debugOptions.skipLoadingScreen);
                            if (globalInfo.stages.Length > 0) globalInfo.selectedStage = globalInfo.stages[0];

                            globalInfo.debugOptions.matchType = (MatchType)EditorGUILayout.EnumPopup("Match Type:", globalInfo.debugOptions.matchType, enumStyle);
                            if (globalInfo.debugOptions.matchType == MatchType.Challenge) {
                                if (globalInfo.challengeModeOptions.Length > 0) {
                                    int arraySize = globalInfo.challengeModeOptions.Length;
                                    string[] challengeSelect = new string[arraySize];
                                    for (int i = 0; i < globalInfo.challengeModeOptions.Length; i++) {
                                        if (globalInfo.challengeModeOptions[i].challengeName != "") {
                                            challengeSelect[i] = globalInfo.challengeModeOptions[i].challengeName;
                                        } else {
                                            challengeSelect[i] = "No name";
                                        }
                                    }
                                    globalInfo.selectedChallenge = EditorGUILayout.Popup("Challenge Selection:", globalInfo.selectedChallenge, challengeSelect);
                                } else {
							        GUILayout.BeginHorizontal("GroupBox");
							        GUILayout.Label("No challenges found", "CN EntryWarn");
							        GUILayout.EndHorizontal();
                                }
                            }

							globalInfo.p1CharStorage = (UFE3D.CharacterInfo) EditorGUILayout.ObjectField("Player 1 Character:", globalInfo.p1CharStorage, typeof(UFE3D.CharacterInfo), false);
							globalInfo.p2CharStorage = (UFE3D.CharacterInfo) EditorGUILayout.ObjectField("Player 2 Character:", globalInfo.p2CharStorage, typeof(UFE3D.CharacterInfo), false);
							globalInfo.p1CPUControl = EditorGUILayout.Toggle("Player 1 CPU Controlled", globalInfo.p1CPUControl);
							globalInfo.p2CPUControl = EditorGUILayout.Toggle("Player 2 CPU Controlled", globalInfo.p2CPUControl);
                            globalInfo.debugOptions.emulateNetwork = EditorGUILayout.Toggle("Emulate Network Game", globalInfo.debugOptions.emulateNetwork);
						}EditorGUI.EndDisabledGroup();
                        
                        if (!globalInfo.debugOptions.startGameImmediately){
                            globalInfo.selectedStage = null;
							globalInfo.player1Character = null;
							globalInfo.player2Character = null;
                        }

                        EditorGUILayout.Space();
						
						EditorGUIUtility.labelWidth = 150;
						EditorGUI.indentLevel -= 1;

					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// AI Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					aiOptions = EditorGUILayout.Foldout(aiOptions, "AI Options", foldStyle);
					helpButton("global:aioptions");
				}EditorGUILayout.EndHorizontal();
				
				if (aiOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 150;
						globalInfo.aiOptions.engine = (AIEngine) EditorGUILayout.EnumPopup("AI Engine:", globalInfo.aiOptions.engine, enumStyle);

						if (globalInfo.aiOptions.engine == AIEngine.RandomAI){
							EditorGUIUtility.labelWidth = 200;
							globalInfo.aiOptions.attackWhenEnemyIsDown = EditorGUILayout.Toggle("Attack When Enemy is Down", globalInfo.aiOptions.attackWhenEnemyIsDown);
							globalInfo.aiOptions.moveWhenEnemyIsDown = EditorGUILayout.Toggle("Move When Enemy is Down", globalInfo.aiOptions.moveWhenEnemyIsDown);
							
							EditorGUILayout.Space();
							globalInfo.aiOptions.inputFrequency = Mathf.Max(EditorGUILayout.FloatField("Input Frequency (seconds);", globalInfo.aiOptions.inputFrequency), 0f);
							EditorGUILayout.Space();
							
							EditorGUIUtility.labelWidth = 150;
							
							globalInfo.aiOptions.behaviourToggle = EditorGUILayout.Foldout(globalInfo.aiOptions.behaviourToggle, "Distance Behaviours ("+ globalInfo.aiOptions.distanceBehaviour.Length +")", foldStyle);
							if (globalInfo.aiOptions.behaviourToggle){
								EditorGUILayout.BeginVertical(subGroupStyle);{
									EditorGUILayout.Space();
									EditorGUI.indentLevel += 1;
									
									for (int i = 0; i < globalInfo.aiOptions.distanceBehaviour.Length; i ++){
										EditorGUILayout.Space();
										EditorGUILayout.BeginVertical(subArrayElementStyle);{
											EditorGUILayout.Space();
											
											EditorGUIUtility.labelWidth = 160;
											EditorGUILayout.BeginHorizontal();{
												globalInfo.aiOptions.distanceBehaviour[i].characterDistance = (CharacterDistance)EditorGUILayout.EnumPopup("Opponent Distance:", globalInfo.aiOptions.distanceBehaviour[i].characterDistance, enumStyle);
												Vector2 newRange = ReturnRange(globalInfo.aiOptions.distanceBehaviour[i].characterDistance, globalInfo.aiOptions.distanceBehaviour.Length);
												if (newRange != Vector2.zero){
													globalInfo.aiOptions.distanceBehaviour[i].proximityRangeBegins = (int)newRange.x;
													globalInfo.aiOptions.distanceBehaviour[i].proximityRangeEnds = (int)newRange.y;
												}
												
												//GUILayout.Label(DistanceToString(i, globalInfo.aiOptions.distanceBehaviour.Length) + " Distance");
												if (GUILayout.Button("", "PaneOptions")){
													PaneOptions<AIDistanceBehaviour>(globalInfo.aiOptions.distanceBehaviour, globalInfo.aiOptions.distanceBehaviour[i], delegate (AIDistanceBehaviour[] newElement) { globalInfo.aiOptions.distanceBehaviour = newElement; });
												}
											}EditorGUILayout.EndHorizontal();
											GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
											
											int pArcBeginsTemp = globalInfo.aiOptions.distanceBehaviour[i].proximityRangeBegins;
											int pArcEndsTemp = globalInfo.aiOptions.distanceBehaviour[i].proximityRangeEnds;
											EditorGUI.indentLevel += 2;
											StyledMinMaxSlider("Proximity", ref globalInfo.aiOptions.distanceBehaviour[i].proximityRangeBegins, ref globalInfo.aiOptions.distanceBehaviour[i].proximityRangeEnds, 0, 100, EditorGUI.indentLevel);
											EditorGUI.indentLevel -= 2;
											if (globalInfo.aiOptions.distanceBehaviour[i].proximityRangeBegins != pArcBeginsTemp ||
											    globalInfo.aiOptions.distanceBehaviour[i].proximityRangeEnds != pArcEndsTemp){
												globalInfo.aiOptions.distanceBehaviour[i].characterDistance = CharacterDistance.Other;
											}
											
											EditorGUILayout.Space();
											
											EditorGUIUtility.labelWidth = 200;

											globalInfo.aiOptions.distanceBehaviour[i].movingForwardProbability = EditorGUILayout.Slider("Move Forward Probability:", globalInfo.aiOptions.distanceBehaviour[i].movingForwardProbability, 0, 1);
											globalInfo.aiOptions.distanceBehaviour[i].movingBackProbability = EditorGUILayout.Slider("Move Back Probability:", globalInfo.aiOptions.distanceBehaviour[i].movingBackProbability, 0, 1);
											globalInfo.aiOptions.distanceBehaviour[i].jumpingProbability = EditorGUILayout.Slider("Jump Probability:", globalInfo.aiOptions.distanceBehaviour[i].jumpingProbability, 0, 1);
											globalInfo.aiOptions.distanceBehaviour[i].crouchProbability = EditorGUILayout.Slider("Crouch Probability:", globalInfo.aiOptions.distanceBehaviour[i].crouchProbability, 0, 1);
											globalInfo.aiOptions.distanceBehaviour[i].attackProbability = EditorGUILayout.Slider("Attack Probability:", globalInfo.aiOptions.distanceBehaviour[i].attackProbability, 0, 1);

											EditorGUILayout.Space();
										}EditorGUILayout.EndVertical();
									}
									EditorGUILayout.Space();
									if (StyledButton("New Distance Behaviour"))
										globalInfo.aiOptions.distanceBehaviour = AddElement<AIDistanceBehaviour>(globalInfo.aiOptions.distanceBehaviour, null);
									
									EditorGUILayout.Space();
									EditorGUI.indentLevel -= 1;
								}EditorGUILayout.EndVertical();
							}


						}else if (globalInfo.aiOptions.engine == AIEngine.FuzzyAI && UFE.isAiAddonInstalled){
							
							EditorGUIUtility.labelWidth = 180;
							globalInfo.aiOptions.multiCoreSupport = EditorGUILayout.Toggle("Multi Core Support", globalInfo.aiOptions.multiCoreSupport);
							globalInfo.aiOptions.persistentBehavior = EditorGUILayout.Toggle("Persistent Behavior", globalInfo.aiOptions.persistentBehavior);

							globalInfo.aiOptions.selectedDifficultyLevel = (AIDifficultyLevel) EditorGUILayout.EnumPopup("Default Difficulty:", globalInfo.aiOptions.selectedDifficultyLevel, enumStyle);

							globalInfo.aiOptions.difficultyToggle = EditorGUILayout.Foldout(globalInfo.aiOptions.difficultyToggle, "Difficulty Settings ("+ globalInfo.aiOptions.difficultySettings.Length +")", foldStyle);
							if (globalInfo.aiOptions.difficultyToggle){
								EditorGUILayout.BeginVertical(subGroupStyle);{
									EditorGUILayout.Space();
									//EditorGUI.indentLevel += 1;
									
									for (int i = 0; i < globalInfo.aiOptions.difficultySettings.Length; i ++){
										EditorGUILayout.Space();
										EditorGUILayout.BeginVertical(subArrayElementStyle);{
											EditorGUILayout.Space();
											
											EditorGUIUtility.labelWidth = 160;
											EditorGUILayout.BeginHorizontal();{
												globalInfo.aiOptions.difficultySettings[i].difficultyLevel = (AIDifficultyLevel)EditorGUILayout.EnumPopup("Difficulty Level:", globalInfo.aiOptions.difficultySettings[i].difficultyLevel, enumStyle);

												if (GUILayout.Button("", "PaneOptions")){
													PaneOptions<AIDifficultySettings>(globalInfo.aiOptions.difficultySettings, globalInfo.aiOptions.difficultySettings[i], delegate (AIDifficultySettings[] newElement) { globalInfo.aiOptions.difficultySettings = newElement; });
												}
											}EditorGUILayout.EndHorizontal();

											
											SubGroupTitle("Override Instructions");
											
											EditorGUILayout.Space();
											
											EditorGUIUtility.labelWidth = 176;

											globalInfo.aiOptions.difficultySettings[i].startupBehavior = (AIBehavior)EditorGUILayout.EnumPopup("Startup Behavior:", globalInfo.aiOptions.difficultySettings[i].startupBehavior, enumStyle);

											DisableableSlider("Time Between Decisions:",
											                  ref globalInfo.aiOptions.difficultySettings[i].overrideTimeBetweenDecisions,
											                  ref globalInfo.aiOptions.difficultySettings[i].timeBetweenDecisions, 
											                  0f,
											                  .5f);

											DisableableSlider("Time Between Actions:",
											                  ref globalInfo.aiOptions.difficultySettings[i].overrideTimeBetweenActions,
											                  ref globalInfo.aiOptions.difficultySettings[i].timeBetweenActions, 
											                  0f,
											                  .5f);

											DisableableSlider("Rule Compliance:",
											                  ref globalInfo.aiOptions.difficultySettings[i].overrideRuleCompliance,
											                  ref globalInfo.aiOptions.difficultySettings[i].ruleCompliance, 
											                  0f,
											                  1f);

											DisableableSlider("Aggressiveness:",
											                  ref globalInfo.aiOptions.difficultySettings[i].overrideAggressiveness,
											                  ref globalInfo.aiOptions.difficultySettings[i].aggressiveness, 
											                  .1f,
											                  .9f);

											DisableableSlider("Combo Efficiency:",
											                  ref globalInfo.aiOptions.difficultySettings[i].overrideComboEfficiency,
											                  ref globalInfo.aiOptions.difficultySettings[i].comboEfficiency, 
											                  0f,
											                  1f);

											/*globalInfo.aiOptions.difficultySetup[i].timeBetweenActions = EditorGUILayout.Slider("Time Between Actions:", globalInfo.aiOptions.difficultySetup[i].timeBetweenActions, 0f, 0.5f);
											
											EditorGUILayout.Space();
											EditorGUIUtility.labelWidth = 160;
											globalInfo.aiOptions.difficultySetup[i].ruleCompliance = EditorGUILayout.Slider("Rule Compliance:", globalInfo.aiOptions.difficultySetup[i].ruleCompliance, 0f, 1f);
											globalInfo.aiOptions.difficultySetup[i].aggressiveness = EditorGUILayout.Slider("Aggressiveness:", globalInfo.aiOptions.difficultySetup[i].aggressiveness, 0.1f, 0.9f);
											globalInfo.aiOptions.difficultySetup[i].comboEfficiency = EditorGUILayout.Slider("Combo Efficiency:", globalInfo.aiOptions.difficultySetup[i].comboEfficiency, 0f, 1f);
											*/
											EditorGUIUtility.labelWidth = 150;

											EditorGUILayout.Space();

											EditorGUILayout.Space();
										}EditorGUILayout.EndVertical();
									}
									EditorGUILayout.Space();
									if (StyledButton("New Difficulty Setup"))
										globalInfo.aiOptions.difficultySettings = AddElement<AIDifficultySettings>(globalInfo.aiOptions.difficultySettings, null);
									
									EditorGUILayout.Space();
									//EditorGUI.indentLevel -= 1;
								}EditorGUILayout.EndVertical();
							}
						}else{
							GUILayout.BeginHorizontal("GroupBox");
							GUILayout.Label("You must have Fuzzy AI installed\n in order to use this option", "CN EntryWarn");
							GUILayout.EndHorizontal();
						}
						EditorGUIUtility.labelWidth = 150;
						
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

			// Language Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					languageOptions = EditorGUILayout.Foldout(languageOptions, "Languages ("+ globalInfo.languages.Length +")", foldStyle);
					helpButton("global:languages");
				}EditorGUILayout.EndHorizontal();

				if (languageOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 160;
						for (int i = 0; i < globalInfo.languages.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									globalInfo.languages[i].languageName = EditorGUILayout.TextField("Language:", globalInfo.languages[i].languageName);
									if (GUILayout.Button("", "PaneOptions")){
										PaneOptions<LanguageOptions>(globalInfo.languages, globalInfo.languages[i], delegate (LanguageOptions[] newElement) { globalInfo.languages = newElement; });
									}
								}EditorGUILayout.EndHorizontal();
								
								bool defaultTemp = globalInfo.languages[i].defaultSelection;
								globalInfo.languages[i].defaultSelection = EditorGUILayout.Toggle("Default", globalInfo.languages[i].defaultSelection);
								if (defaultTemp != globalInfo.languages[i].defaultSelection){
									for (int t = 0; t < globalInfo.languages.Length; t ++){
										if (t != i) globalInfo.languages[t].defaultSelection = false;
									}
									globalInfo.languages[i].defaultSelection = true;
								}
								EditorGUILayout.Space();

								globalInfo.languages[i].start = EditorGUILayout.TextField("Start:", globalInfo.languages[i].start);
								globalInfo.languages[i].options = EditorGUILayout.TextField("Options:", globalInfo.languages[i].options);
								globalInfo.languages[i].credits = EditorGUILayout.TextField("Credits:", globalInfo.languages[i].credits);
								globalInfo.languages[i].selectYourCharacter = EditorGUILayout.TextField("Select Your Character:", globalInfo.languages[i].selectYourCharacter);
								globalInfo.languages[i].selectYourStage = EditorGUILayout.TextField("Select Your Stage:", globalInfo.languages[i].selectYourStage);
								globalInfo.languages[i].round = EditorGUILayout.TextField("Round:", globalInfo.languages[i].round);
								globalInfo.languages[i].finalRound = EditorGUILayout.TextField("Final Round:", globalInfo.languages[i].finalRound);
								globalInfo.languages[i].fight = EditorGUILayout.TextField("Fight:", globalInfo.languages[i].fight);
								globalInfo.languages[i].firstHit = EditorGUILayout.TextField("First Hit:", globalInfo.languages[i].firstHit);
								globalInfo.languages[i].combo = EditorGUILayout.TextField("Combo:", globalInfo.languages[i].combo);
								globalInfo.languages[i].parry = EditorGUILayout.TextField("Parry:", globalInfo.languages[i].parry);
								globalInfo.languages[i].counterHit = EditorGUILayout.TextField("Counter Hit:", globalInfo.languages[i].counterHit);
								globalInfo.languages[i].victory = EditorGUILayout.TextField("Victory:", globalInfo.languages[i].victory);
                                globalInfo.languages[i].challengeBegins = EditorGUILayout.TextField("Challenge Begins:", globalInfo.languages[i].challengeBegins);
                                globalInfo.languages[i].challengeEnds = EditorGUILayout.TextField("Challenge Ends:", globalInfo.languages[i].challengeEnds);
								globalInfo.languages[i].timeOver = EditorGUILayout.TextField("Time Over:", globalInfo.languages[i].timeOver);
								globalInfo.languages[i].perfect = EditorGUILayout.TextField("Perfect:", globalInfo.languages[i].perfect);
								globalInfo.languages[i].rematch = EditorGUILayout.TextField("Rematch:", globalInfo.languages[i].rematch);
								globalInfo.languages[i].quit = EditorGUILayout.TextField("Quit:", globalInfo.languages[i].quit);
								globalInfo.languages[i].ko = EditorGUILayout.TextField("K.O.:", globalInfo.languages[i].ko);
								globalInfo.languages[i].draw = EditorGUILayout.TextField("Draw:", globalInfo.languages[i].draw);
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
						}
						
						if (StyledButton("New Language"))
							globalInfo.languages = AddElement<LanguageOptions>(globalInfo.languages, new LanguageOptions());
						
						EditorGUIUtility.labelWidth = 150;
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Camera Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					cameraOptions = EditorGUILayout.Foldout(cameraOptions, "Camera Options", foldStyle);
					helpButton("global:camera");
				}EditorGUILayout.EndHorizontal();

				if (cameraOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 200;
						globalInfo.cameraOptions.initialFieldOfView = EditorGUILayout.Slider("Field of View:", globalInfo.cameraOptions.initialFieldOfView, 1, 179);
						globalInfo.cameraOptions.initialDistance = EditorGUILayout.Vector3Field("Initial Distance:", globalInfo.cameraOptions.initialDistance);
						//globalInfo.cameraOptions.initialDistance.x = 0;
						globalInfo.cameraOptions.initialRotation = EditorGUILayout.Vector3Field("Initial Rotation:", globalInfo.cameraOptions.initialRotation);
						globalInfo.cameraOptions.movementSpeed = EditorGUILayout.FloatField("Movement Speed:", globalInfo.cameraOptions.movementSpeed);
						globalInfo.cameraOptions.minZoom = EditorGUILayout.FloatField("Minimum Zoom:", globalInfo.cameraOptions.minZoom);
						globalInfo.cameraOptions.maxZoom = EditorGUILayout.FloatField("Maximum Zoom:", globalInfo.cameraOptions.maxZoom);
						globalInfo.cameraOptions._maxDistance = EditorGUILayout.FloatField("Maximum Players Distance:", (float)globalInfo.cameraOptions._maxDistance);
						globalInfo.cameraOptions.followJumpingCharacter = EditorGUILayout.Toggle("Follow Jumping Characters", globalInfo.cameraOptions.followJumpingCharacter);
						globalInfo.cameraOptions.enableLookAt = EditorGUILayout.Toggle("Enable LookAt", globalInfo.cameraOptions.enableLookAt);
                        if (globalInfo.cameraOptions.enableLookAt) {
                            globalInfo.cameraOptions.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed:", globalInfo.cameraOptions.rotationSpeed);
                            //globalInfo.cameraOptions.heightOffSet = EditorGUILayout.FloatField("LookAt Height Offset:", globalInfo.cameraOptions.heightOffSet);
                            globalInfo.cameraOptions.rotationOffSet = EditorGUILayout.Vector3Field("Rotation Offset:", globalInfo.cameraOptions.rotationOffSet);
                            globalInfo.cameraOptions.motionSensor = (MotionSensor)EditorGUILayout.EnumPopup("Motion Sensor:", globalInfo.cameraOptions.motionSensor);
                            if (globalInfo.cameraOptions.motionSensor != MotionSensor.None)
                                globalInfo.cameraOptions.motionSensibility = EditorGUILayout.FloatField("Sensibility", globalInfo.cameraOptions.motionSensibility);
                        }
						EditorGUIUtility.labelWidth = 150;

						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

			
			// Character Rotation Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					characterRotationOptions = EditorGUILayout.Foldout(characterRotationOptions, "Character Rotation Options", foldStyle);
					helpButton("global:rotation");
				}EditorGUILayout.EndHorizontal();
				
				if (characterRotationOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 210;
						globalInfo.characterRotationOptions.autoMirror = EditorGUILayout.Toggle("Auto Mirror", globalInfo.characterRotationOptions.autoMirror);
						globalInfo.characterRotationOptions.rotateWhileJumping = EditorGUILayout.Toggle("Rotate While Jumping", globalInfo.characterRotationOptions.rotateWhileJumping);
						globalInfo.characterRotationOptions.rotateOnMoveOnly = EditorGUILayout.Toggle("Rotate On Move Only", globalInfo.characterRotationOptions.rotateOnMoveOnly);
						globalInfo.characterRotationOptions.fixRotationWhenStunned = EditorGUILayout.Toggle("Fix Rotation When Stunned", globalInfo.characterRotationOptions.fixRotationWhenStunned);
						globalInfo.characterRotationOptions.fixRotationWhenBlocking = EditorGUILayout.Toggle("Fix Rotation When Blocking", globalInfo.characterRotationOptions.fixRotationWhenBlocking);
						globalInfo.characterRotationOptions.fixRotationOnHit = EditorGUILayout.Toggle("Fix Rotation On Hit", globalInfo.characterRotationOptions.fixRotationOnHit);
						globalInfo.characterRotationOptions._rotationSpeed = EditorGUILayout.FloatField("Rotation Speed:", (float)globalInfo.characterRotationOptions._rotationSpeed);
						globalInfo.characterRotationOptions._mirrorBlending = EditorGUILayout.FloatField("Mirror Blending (Mecanim only):", (float)globalInfo.characterRotationOptions._mirrorBlending);
						EditorGUIUtility.labelWidth = 150;
						
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Round Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					roundOptions = EditorGUILayout.Foldout(roundOptions, "Round Options", foldStyle);
					helpButton("global:round");
				}EditorGUILayout.EndHorizontal();

				if (roundOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 200;
						globalInfo.roundOptions.totalRounds = EditorGUILayout.IntField("Total Rounds (Best of):", globalInfo.roundOptions.totalRounds);
						globalInfo.roundOptions._p1XPosition = EditorGUILayout.FloatField("Initial Spawn Position (P1):", (float)globalInfo.roundOptions._p1XPosition);
						globalInfo.roundOptions._p2XPosition = EditorGUILayout.FloatField("Initial Spawn Position (P2):", (float)globalInfo.roundOptions._p2XPosition);
						globalInfo.roundOptions._newRoundDelay = EditorGUILayout.FloatField("New Round Delay (seconds):", (float)globalInfo.roundOptions._newRoundDelay);
						globalInfo.roundOptions._endGameDelay = EditorGUILayout.FloatField("End Game Delay (seconds):", (float)globalInfo.roundOptions._endGameDelay);
						globalInfo.roundOptions.victoryMusic = (AudioClip) EditorGUILayout.ObjectField("Victory Music:", globalInfo.roundOptions.victoryMusic, typeof(UnityEngine.AudioClip), false);
						globalInfo.roundOptions.hasTimer = EditorGUILayout.Toggle("Has Timer", globalInfo.roundOptions.hasTimer);
						if (globalInfo.roundOptions.hasTimer){
							globalInfo.roundOptions._timer = EditorGUILayout.FloatField("Round Timer (seconds):", (float)globalInfo.roundOptions._timer);
							globalInfo.roundOptions._timerSpeed = EditorGUILayout.FloatField("Timer Speed (%):", (float)globalInfo.roundOptions._timerSpeed);
						}
						globalInfo.roundOptions.resetLifePoints = EditorGUILayout.Toggle("Reset life points", globalInfo.roundOptions.resetLifePoints);
						globalInfo.roundOptions.resetPositions = EditorGUILayout.Toggle("Reset positions", globalInfo.roundOptions.resetPositions);
						globalInfo.roundOptions.allowMovementStart = EditorGUILayout.Toggle("Allow movement before battle", globalInfo.roundOptions.allowMovementStart);
						globalInfo.roundOptions.slowMotionKO = EditorGUILayout.Toggle("Slow motion K.O.", globalInfo.roundOptions.slowMotionKO);
                        if (globalInfo.roundOptions.slowMotionKO) {
                            globalInfo.roundOptions._slowMoTimer = EditorGUILayout.FloatField("- Slow-mo Timer (seconds):", (float)globalInfo.roundOptions._slowMoTimer);
                            globalInfo.roundOptions._slowMoSpeed = EditorGUILayout.Slider("- Game Speed:", (float)globalInfo.roundOptions._slowMoSpeed, .01f, 1);
                        }
                        globalInfo.roundOptions.allowMovementEnd = EditorGUILayout.Toggle("Allow movement after K.O", globalInfo.roundOptions.allowMovementEnd);
                        globalInfo.roundOptions.inhibitGaugeGain = EditorGUILayout.Toggle("Inhibit gauge after K.O", globalInfo.roundOptions.inhibitGaugeGain);
                        globalInfo.roundOptions.rotateBodyKO = EditorGUILayout.Toggle("Rotate body after K.O", globalInfo.roundOptions.rotateBodyKO);
						//globalInfo.roundOptions.cameraZoomKO = EditorGUILayout.Toggle("Camera Zoom K.O.", globalInfo.roundOptions.cameraZoomKO);
						globalInfo.roundOptions.freezeCamAfterOutro = EditorGUILayout.Toggle("Freeze camera after outro", globalInfo.roundOptions.freezeCamAfterOutro);
						EditorGUIUtility.labelWidth = 150;
						
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

			// Counter Hit Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					counterHitOptions = EditorGUILayout.Foldout(counterHitOptions, "Counter Hit Options", foldStyle);
					helpButton("global:counterhit");
				}EditorGUILayout.EndHorizontal();
				
				if (counterHitOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 180;
						globalInfo.counterHitOptions.startUpFrames = EditorGUILayout.Toggle("Start Up Frames", globalInfo.counterHitOptions.startUpFrames);
						globalInfo.counterHitOptions.activeFrames = EditorGUILayout.Toggle("Active Frames", globalInfo.counterHitOptions.activeFrames);
						globalInfo.counterHitOptions.recoveryFrames = EditorGUILayout.Toggle("Recovery Frames", globalInfo.counterHitOptions.recoveryFrames);
						globalInfo.counterHitOptions._damageIncrease = EditorGUILayout.FloatField("Damage Increase (%):", (float)globalInfo.counterHitOptions._damageIncrease);
						globalInfo.counterHitOptions._hitStunIncrease = EditorGUILayout.FloatField("Hit Stun Increase (%):", (float)globalInfo.counterHitOptions._hitStunIncrease);
						globalInfo.counterHitOptions.sound = (AudioClip) EditorGUILayout.ObjectField("Sound File:", globalInfo.counterHitOptions.sound, typeof(UnityEngine.AudioClip), false);
						EditorGUIUtility.labelWidth = 150;
						
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

			// Combo Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					comboOptions = EditorGUILayout.Foldout(comboOptions, "Combo Options", foldStyle);
					helpButton("global:combo");
				}EditorGUILayout.EndHorizontal();

				if (comboOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;

                        SubGroupTitle("General");
						EditorGUIUtility.labelWidth = 210;
						globalInfo.comboOptions.comboDisplayMode = (ComboDisplayMode) EditorGUILayout.EnumPopup("Display Mode:", globalInfo.comboOptions.comboDisplayMode);
						globalInfo.comboOptions.maxCombo = EditorGUILayout.IntField("Maximum Hits:", globalInfo.comboOptions.maxCombo);

						globalInfo.comboOptions.hitStunDeterioration = (Sizes) EditorGUILayout.EnumPopup("Hit Stun Deterioration:", globalInfo.comboOptions.hitStunDeterioration, enumStyle);
						EditorGUI.BeginDisabledGroup(globalInfo.comboOptions.hitStunDeterioration == Sizes.None);{
							globalInfo.comboOptions._minHitStun = EditorGUILayout.IntField("Minimum Hit Stun (frames):", globalInfo.comboOptions._minHitStun);
						}EditorGUI.EndDisabledGroup();

						globalInfo.comboOptions.damageDeterioration = (Sizes) EditorGUILayout.EnumPopup("Damage Deterioration:", globalInfo.comboOptions.damageDeterioration, enumStyle);
						EditorGUI.BeginDisabledGroup(globalInfo.comboOptions.damageDeterioration == Sizes.None);{
							globalInfo.comboOptions._minDamage = EditorGUILayout.FloatField("Minimum Damage:", (float)globalInfo.comboOptions._minDamage);
						}EditorGUI.EndDisabledGroup();

                        globalInfo.comboOptions.maxConsecutiveCrumple = EditorGUILayout.IntField("Max Crumple Hits:", globalInfo.comboOptions.maxConsecutiveCrumple);

                        EditorGUILayout.Space();

                        SubGroupTitle("Air Combos");
						globalInfo.comboOptions.airJuggleDeterioration = (Sizes) EditorGUILayout.EnumPopup("Air-Juggle Deterioration:", globalInfo.comboOptions.airJuggleDeterioration, enumStyle);
						globalInfo.comboOptions.airJuggleDeteriorationType = (AirJuggleDeteriorationType) EditorGUILayout.EnumPopup("Air-Juggle Deterioration Type:", globalInfo.comboOptions.airJuggleDeteriorationType, enumStyle);
						EditorGUI.BeginDisabledGroup(globalInfo.comboOptions.airJuggleDeterioration == Sizes.None);{
							globalInfo.comboOptions._minPushForce = EditorGUILayout.FloatField("Minimum Juggle Force (Y):", (float)globalInfo.comboOptions._minPushForce);
						}EditorGUI.EndDisabledGroup();

                        globalInfo.comboOptions._knockBackMinForce = EditorGUILayout.FloatField("Mininum Knock Back Force (X):", (float)globalInfo.comboOptions._knockBackMinForce);
                        globalInfo.comboOptions.airRecoveryType = (AirRecoveryType)EditorGUILayout.EnumPopup("Air Recovery Type:", globalInfo.comboOptions.airRecoveryType, enumStyle);
                        globalInfo.comboOptions.resetFallingForceOnHit = EditorGUILayout.Toggle("Reset Falling Force On Hit", globalInfo.comboOptions.resetFallingForceOnHit);
                        globalInfo.comboOptions.neverCornerPush = EditorGUILayout.Toggle("Never Corner Push", globalInfo.comboOptions.neverCornerPush);
						
                        //globalInfo.comboOptions.neverAirRecover = EditorGUILayout.Toggle("Never Air-Recover", globalInfo.comboOptions.neverAirRecover);

						globalInfo.comboOptions.fixJuggleWeight = EditorGUILayout.Toggle("Fixed Juggle Weight", globalInfo.comboOptions.fixJuggleWeight);
						globalInfo.comboOptions._juggleWeight = EditorGUILayout.FloatField("Juggle Weight:", (float)globalInfo.comboOptions._juggleWeight);

						EditorGUIUtility.labelWidth = 150;
						
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

            // Bounce Options
            EditorGUILayout.BeginVertical(rootGroupStyle);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    bounceOptions = EditorGUILayout.Foldout(bounceOptions, "Bounce Options", foldStyle);
                    helpButton("global:bounce");
                } EditorGUILayout.EndHorizontal();

                if (bounceOptions) {
                    EditorGUILayout.BeginVertical(subGroupStyle);
                    {
                        EditorGUILayout.Space();
                        EditorGUI.indentLevel += 1;

                        SubGroupTitle("Ground Bounce");

                        EditorGUIUtility.labelWidth = 180;
                        globalInfo.groundBounceOptions.bounceForce = (Sizes)EditorGUILayout.EnumPopup("Bounce Back Force:", globalInfo.groundBounceOptions.bounceForce, enumStyle);
                        EditorGUI.BeginDisabledGroup(globalInfo.groundBounceOptions.bounceForce == Sizes.None);
                        {
                            globalInfo.groundBounceOptions.bouncePrefab = (GameObject)EditorGUILayout.ObjectField("Bounce Effect:", globalInfo.groundBounceOptions.bouncePrefab, typeof(UnityEngine.GameObject), true);
                            globalInfo.groundBounceOptions.bounceKillTime = EditorGUILayout.FloatField("Effect Duration:", globalInfo.groundBounceOptions.bounceKillTime);
                            globalInfo.groundBounceOptions.bounceSound = (AudioClip)EditorGUILayout.ObjectField("Bounce Sound:", globalInfo.groundBounceOptions.bounceSound, typeof(UnityEngine.AudioClip), false);
                            globalInfo.groundBounceOptions._minimumBounceForce = EditorGUILayout.FloatField("Minimum Bounce Force:", (float)globalInfo.groundBounceOptions._minimumBounceForce);
                            globalInfo.groundBounceOptions._maximumBounces = EditorGUILayout.FloatField("Maximum Bounces:", (float)globalInfo.groundBounceOptions._maximumBounces);
                            //globalInfo.groundBounceOptions.bounceHitBoxes = EditorGUILayout.Toggle("Bounce Hit Boxes", globalInfo.groundBounceOptions.bounceHitBoxes);
                            globalInfo.groundBounceOptions.sticky = EditorGUILayout.Toggle("Stick to Character", globalInfo.groundBounceOptions.sticky);
                            globalInfo.groundBounceOptions.shakeCamOnBounce = EditorGUILayout.Toggle("Shake Camera On Bounce", globalInfo.groundBounceOptions.shakeCamOnBounce);
                            if (globalInfo.groundBounceOptions.shakeCamOnBounce) {
                                globalInfo.groundBounceOptions._shakeDensity = EditorGUILayout.FloatField("Shake Density:", (float)globalInfo.groundBounceOptions._shakeDensity);
                            }
                        }
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.Space();

                        SubGroupTitle("Wall Bounce");

                        globalInfo.wallBounceOptions.bounceForce = (Sizes)EditorGUILayout.EnumPopup("Bounce Back Force:", globalInfo.wallBounceOptions.bounceForce, enumStyle);
                        EditorGUI.BeginDisabledGroup(globalInfo.wallBounceOptions.bounceForce == Sizes.None);
                        {
                            globalInfo.wallBounceOptions.bouncePrefab = (GameObject)EditorGUILayout.ObjectField("Bounce Effect:", globalInfo.wallBounceOptions.bouncePrefab, typeof(UnityEngine.GameObject), true);
                            globalInfo.wallBounceOptions.bounceKillTime = EditorGUILayout.FloatField("Effect Duration:", globalInfo.wallBounceOptions.bounceKillTime);
                            globalInfo.wallBounceOptions.bounceSound = (AudioClip)EditorGUILayout.ObjectField("Bounce Sound:", globalInfo.wallBounceOptions.bounceSound, typeof(UnityEngine.AudioClip), false);
                            globalInfo.wallBounceOptions._minimumBounceForce = EditorGUILayout.FloatField("Minimum Bounce Force:", (float)globalInfo.wallBounceOptions._minimumBounceForce);
                            globalInfo.wallBounceOptions._maximumBounces = EditorGUILayout.FloatField("Maximum Bounces:", (float)globalInfo.wallBounceOptions._maximumBounces);
                            //globalInfo.wallBounceOptions.bounceHitBoxes = EditorGUILayout.Toggle("Bounce Hit Boxes", globalInfo.wallBounceOptions.bounceHitBoxes);
                            globalInfo.wallBounceOptions.sticky = EditorGUILayout.Toggle("Stick to Character", globalInfo.wallBounceOptions.sticky);
                            globalInfo.wallBounceOptions.shakeCamOnBounce = EditorGUILayout.Toggle("Shake Camera On Bounce", globalInfo.wallBounceOptions.shakeCamOnBounce);
                            if (globalInfo.wallBounceOptions.shakeCamOnBounce) {
                                globalInfo.wallBounceOptions._shakeDensity = EditorGUILayout.FloatField("Shake Density:", (float)globalInfo.wallBounceOptions._shakeDensity);
                            }
                        }
                        EditorGUI.EndDisabledGroup();

                        EditorGUIUtility.labelWidth = 150;

                        EditorGUI.indentLevel -= 1;
                        EditorGUILayout.Space();

                    } EditorGUILayout.EndVertical();
                }
            } EditorGUILayout.EndVertical();
			
			// Block Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					blockOptions = EditorGUILayout.Foldout(blockOptions, "Block Options", foldStyle);
					helpButton("global:block");
				}EditorGUILayout.EndHorizontal();

				if (blockOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						EditorGUIUtility.labelWidth = 200;

						SubGroupTitle("Block");

						globalInfo.blockOptions.blockType = (BlockType) EditorGUILayout.EnumPopup("Block Input:", globalInfo.blockOptions.blockType, enumStyle);
						EditorGUI.BeginDisabledGroup(globalInfo.blockOptions.blockType == BlockType.None);{
                            globalInfo.blockOptions.allowAirBlock = EditorGUILayout.Toggle("Allow Air Block", globalInfo.blockOptions.allowAirBlock);
                            globalInfo.blockOptions.ignoreAppliedForceBlock = EditorGUILayout.Toggle("Ignore Applied Forces", globalInfo.blockOptions.ignoreAppliedForceBlock);
                            globalInfo.blockOptions.allowMoveCancel = EditorGUILayout.Toggle("Allow Move Canceling", globalInfo.blockOptions.allowMoveCancel);

						}EditorGUI.EndDisabledGroup();

						EditorGUILayout.Space();
						
						SubGroupTitle("Parry");

						globalInfo.blockOptions.parryType = (ParryType) EditorGUILayout.EnumPopup("Parry Input:", globalInfo.blockOptions.parryType, enumStyle);
                        EditorGUI.BeginDisabledGroup(globalInfo.blockOptions.parryType == ParryType.None);
                        {
                            globalInfo.blockOptions._parryTiming = EditorGUILayout.FloatField("Parry Timing:", (float)globalInfo.blockOptions._parryTiming);
                            globalInfo.blockOptions.parryStunType = (ParryStunType)EditorGUILayout.EnumPopup("Parry Stun Type:", globalInfo.blockOptions.parryStunType, enumStyle);
                            if (globalInfo.blockOptions.parryStunType == ParryStunType.Fixed) {
                                globalInfo.blockOptions.parryStunFrames = EditorGUILayout.IntField("Parry Stun (Frames):", globalInfo.blockOptions.parryStunFrames);
                            } else {
                                globalInfo.blockOptions.parryStunFrames = EditorGUILayout.IntSlider("Parry Stun Percentage:", globalInfo.blockOptions.parryStunFrames, 1, 100);
                            }

                            globalInfo.blockOptions.highlightWhenParry = EditorGUILayout.Toggle("Highlight When Parry", globalInfo.blockOptions.highlightWhenParry);
                            EditorGUI.BeginDisabledGroup(!globalInfo.blockOptions.highlightWhenParry);{
                                globalInfo.blockOptions.parryColor = EditorGUILayout.ColorField("Parry Color Mask:", globalInfo.blockOptions.parryColor);
                            } EditorGUI.EndDisabledGroup();

                            globalInfo.blockOptions.allowAirParry = EditorGUILayout.Toggle("Allow Air Parry", globalInfo.blockOptions.allowAirParry);
                            globalInfo.blockOptions.ignoreAppliedForceParry = EditorGUILayout.Toggle("Ignore Applied Forces", globalInfo.blockOptions.ignoreAppliedForceParry);
                            globalInfo.blockOptions.resetButtonSequence = EditorGUILayout.Toggle("Reset Button Sequence", globalInfo.blockOptions.resetButtonSequence);
                            globalInfo.blockOptions.easyParry = EditorGUILayout.Toggle("Enable Easy Parry", globalInfo.blockOptions.easyParry);


                        } EditorGUI.EndDisabledGroup();
						
						EditorGUIUtility.labelWidth = 150;

						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Knock Down Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					knockDownOptions = EditorGUILayout.Foldout(knockDownOptions, "Knock Down Options", foldStyle);
					helpButton("global:knockdown");
				}EditorGUILayout.EndHorizontal();

				if (knockDownOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 180;
						KnockdownOptionsBlock("Default Knockdown", globalInfo.knockDownOptions.air);
						KnockdownOptionsBlock("High Knockdown", globalInfo.knockDownOptions.high);
						KnockdownOptionsBlock("Mid Knockdown", globalInfo.knockDownOptions.highLow);
						KnockdownOptionsBlock("Sweep Knockdown", globalInfo.knockDownOptions.sweep);
                        KnockdownOptionsBlock("Crumple Knockdown", globalInfo.knockDownOptions.crumple);
                        KnockdownOptionsBlock("Wall Bounce Knockdown", globalInfo.knockDownOptions.wallbounce);
						EditorGUIUtility.labelWidth = 150;


						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

			
			// Hit Effects Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					hitOptions = EditorGUILayout.Foldout(hitOptions, "Hit Effect Options", foldStyle);
					helpButton("global:hitEffects");
				}EditorGUILayout.EndHorizontal();

				if (hitOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;

						EditorGUIUtility.labelWidth = 200;
						HitOptionBlock("Weak Hit Options", globalInfo.hitOptions.weakHit);
						HitOptionBlock("Medium Hit Options", globalInfo.hitOptions.mediumHit);
						HitOptionBlock("Heavy Hit Options", globalInfo.hitOptions.heavyHit);
						HitOptionBlock("Crumple Hit Options", globalInfo.hitOptions.crumpleHit);

						EditorGUILayout.Space();
						
						HitOptionBlock("Block Hit Options", globalInfo.blockOptions.blockHitEffects, true);
						HitOptionBlock("Parry Hit Options", globalInfo.blockOptions.parryHitEffects, true);

						EditorGUILayout.Space();

						HitOptionBlock("Custom Hit 1 Options", globalInfo.hitOptions.customHit1);
						HitOptionBlock("Custom Hit 2 Options", globalInfo.hitOptions.customHit2);
						HitOptionBlock("Custom Hit 3 Options", globalInfo.hitOptions.customHit3);

						EditorGUILayout.Space();

						globalInfo.hitOptions.resetAnimationOnHit = EditorGUILayout.Toggle("Restart Animation on Hit", globalInfo.hitOptions.resetAnimationOnHit);
						EditorGUIUtility.labelWidth = 150;
						
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Inputs
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					inputsOptions = EditorGUILayout.Foldout(inputsOptions, "Input Options", foldStyle);
					helpButton("global:input");
				}EditorGUILayout.EndHorizontal();

				if (inputsOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;

						string errorMsg = null;

						globalInfo.inputOptions.inputManagerType = (InputManagerType) EditorGUILayout.EnumPopup("Input Manager:", globalInfo.inputOptions.inputManagerType, enumStyle);
						if (globalInfo.inputOptions.inputManagerType == InputManagerType.cInput && !UFE.isCInputInstalled){
							errorMsg = "You must have cInput installed\n in order to use this option.";
						}else if (globalInfo.inputOptions.inputManagerType == InputManagerType.ControlFreak && !UFE.isControlFreakInstalled){
							errorMsg = "You must have Control Freak installed\n in order to use this option.";
                        } else if (globalInfo.inputOptions.inputManagerType == InputManagerType.Rewired && !UFE.isRewiredInstalled){
                            errorMsg = "You must have Rewired installed\n in order to use this option.";
						}


						if (errorMsg != null){
							GUILayout.BeginHorizontal("GroupBox");
							GUILayout.Label(errorMsg, "CN EntryWarn");
							GUILayout.EndHorizontal();
						}else{
							player1InputOptions = EditorGUILayout.Foldout(player1InputOptions, "Player 1 Inputs ("+ globalInfo.player1_Inputs.Length +")", foldStyle);
							if (player1InputOptions) globalInfo.player1_Inputs = PlayerInputsBlock(globalInfo.player1_Inputs);
							
							player2InputOptions = EditorGUILayout.Foldout(player2InputOptions, "Player 2 Inputs ("+ globalInfo.player2_Inputs.Length +")", foldStyle);
							if (player2InputOptions) globalInfo.player2_Inputs = PlayerInputsBlock(globalInfo.player2_Inputs);

                            if (globalInfo.inputOptions.inputManagerType == InputManagerType.cInput) {
                                EditorGUILayout.Space();
                                cInputOptions = EditorGUILayout.Foldout(cInputOptions, "cInput Preferences", foldStyle);
                                if (cInputOptions) CInputPreferences();
							} else if (globalInfo.inputOptions.inputManagerType == InputManagerType.ControlFreak){
								EditorGUILayout.Space();
								touchControllerOptions = EditorGUILayout.Foldout(touchControllerOptions, "Control Freak Preferences", foldStyle);
								if (touchControllerOptions) ControlFreakPreferences();
							}
						}
						
						EditorGUILayout.Space();
                        EditorGUIUtility.labelWidth = 180;

                        //if (globalInfo.inputOptions.inputManagerType == InputManagerType.UnityInputManager) {
                            globalInfo.inputOptions.forceDigitalInput = EditorGUILayout.Toggle("Force Digital Input", globalInfo.inputOptions.forceDigitalInput);
                        //}

						globalInfo.inputOptions.confirmButton = (ButtonPress) EditorGUILayout.EnumPopup("Confirm Button:", globalInfo.inputOptions.confirmButton, enumStyle);
						globalInfo.inputOptions.cancelButton = (ButtonPress) EditorGUILayout.EnumPopup("Cancel Button:", globalInfo.inputOptions.cancelButton, enumStyle);
						EditorGUIUtility.labelWidth = 150;



						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

			
			// Stages
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					stageOptions = EditorGUILayout.Foldout(stageOptions, "Stages ("+ globalInfo.stages.Length +")", foldStyle);
					helpButton("global:stages");
				}EditorGUILayout.EndHorizontal();

				if (stageOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
                        EditorGUI.indentLevel += 1;
                        SubGroupTitle("Loading Method");

                        StorageMode newStagePrefabStorage = (StorageMode)EditorGUILayout.EnumPopup("Stage Prefab:", globalInfo.stagePrefabStorage);
                        if (globalInfo.stagePrefabStorage != newStagePrefabStorage) {
                            globalInfo.stagePrefabStorage = newStagePrefabStorage;

                            if (newStagePrefabStorage == StorageMode.ResourcesFolder) {
                                for (int i = 0; i < globalInfo.stages.Length; i++) {
                                    globalInfo.stages[i].prefab = null;
                                }
                            }
                        }


                        StorageMode newStageMusicStorage = (StorageMode)EditorGUILayout.EnumPopup("Stage Music:", globalInfo.stageMusicStorage);
                        if (globalInfo.stageMusicStorage != newStageMusicStorage) {
                            globalInfo.stageMusicStorage = newStageMusicStorage;

                            if (newStageMusicStorage == StorageMode.ResourcesFolder) {
                                for (int i = 0; i < globalInfo.stages.Length; i++) {
                                    globalInfo.stages[i].music = null;
                                }
                            }
                        }

                        EditorGUILayout.Space();
                        SubGroupTitle("Stages Info");
						
						for (int i = 0; i < globalInfo.stages.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
                                    globalInfo.stages[i].stageName = EditorGUILayout.TextField("Name:", globalInfo.stages[i].stageName);
									if (GUILayout.Button("", "PaneOptions")){
										PaneOptions<StageOptions>(
											globalInfo.stages, 
											globalInfo.stages[i], 
											delegate (StageOptions[] newElement){
												globalInfo.stages = newElement;
												globalInfo.ValidateStoryModeInformation();
											}
										);
									}
								}EditorGUILayout.EndHorizontal();

                                if (globalInfo.stagePrefabStorage == StorageMode.Legacy) {
                                    globalInfo.stages[i].prefab = (GameObject)EditorGUILayout.ObjectField("Stage Prefab:", globalInfo.stages[i].prefab, typeof(UnityEngine.GameObject), true);
                                } else {
                                    globalInfo.stages[i].stageResourcePath = EditorGUILayout.TextField("Stage Resource Path:", globalInfo.stages[i].stageResourcePath);
                                }
                                if (globalInfo.stageMusicStorage == StorageMode.Legacy) {
                                    globalInfo.stages[i].music = (AudioClip)EditorGUILayout.ObjectField("Music File:", globalInfo.stages[i].music, typeof(UnityEngine.AudioClip), true);
                                } else {
                                    globalInfo.stages[i].musicResourcePath = EditorGUILayout.TextField("Music Resource Path:", globalInfo.stages[i].musicResourcePath);
                                }
								globalInfo.stages[i]._leftBoundary = EditorGUILayout.FloatField("Left Boundary:", (float)globalInfo.stages[i]._leftBoundary);
								globalInfo.stages[i]._rightBoundary = EditorGUILayout.FloatField("Right Boundary:", (float)globalInfo.stages[i]._rightBoundary);
								globalInfo.stages[i]._groundFriction = EditorGUILayout.FloatField("Ground Friction:", (float)globalInfo.stages[i]._groundFriction);
								globalInfo.stages[i]._groundHeight = EditorGUILayout.FloatField("Ground Height:", (float)globalInfo.stages[i]._groundHeight);
                                EditorGUILayout.LabelField("Screenshot:");
								globalInfo.stages[i].screenshot = (Texture2D) EditorGUILayout.ObjectField(globalInfo.stages[i].screenshot, typeof(Texture2D), false);

								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
						}
						EditorGUI.indentLevel -= 1;
						
						if (StyledButton("New Stage")){
							globalInfo.stages = AddElement<StageOptions>(globalInfo.stages, new StageOptions());
							globalInfo.ValidateStoryModeInformation();
						}
						
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Characters
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					characterOptions = EditorGUILayout.Foldout(characterOptions, "Characters ("+ globalInfo.characters.Length +")", foldStyle);
					helpButton("global:characters");
				}EditorGUILayout.EndHorizontal();

				if (characterOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						for (int i = 0; i < globalInfo.characters.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									globalInfo.characters[i] = (UFE3D.CharacterInfo)EditorGUILayout.ObjectField("Character File:", globalInfo.characters[i], typeof(UFE3D.CharacterInfo), false);
									if (GUILayout.Button("", "PaneOptions")){
										PaneOptions<UFE3D.CharacterInfo>(
											globalInfo.characters, 
											globalInfo.characters[i], 
											delegate (UFE3D.CharacterInfo[] newElement) { 
												globalInfo.characters = newElement; 
												globalInfo.ValidateStoryModeInformation();
											}
										);
									}
								}EditorGUILayout.EndHorizontal();
								
								if (GUILayout.Button("Open in the Character Editor")) {
									CharacterEditorWindow.sentCharacterInfo = globalInfo.characters[i];
									CharacterEditorWindow.Init();
								}
							}EditorGUILayout.EndVertical();
						}
						EditorGUI.indentLevel -= 1;
						
						EditorGUILayout.Space();
						if (StyledButton("New Character")){
							globalInfo.characters = AddElement<UFE3D.CharacterInfo>(globalInfo.characters, null);
							globalInfo.storyMode.selectableCharactersInStoryMode.Add(globalInfo.characters.Length - 1);
							globalInfo.storyMode.selectableCharactersInVersusMode.Add(globalInfo.characters.Length - 1);
							globalInfo.ValidateStoryModeInformation();
						}
						
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Screen Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					screenOptions = EditorGUILayout.Foldout(screenOptions, "GUI Options", foldStyle);
					helpButton("global:gui");
				}EditorGUILayout.EndHorizontal();
				
				if (screenOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						EditorGUIUtility.labelWidth = 180;

                        globalInfo.gameGUI.screenFadeDuration = Mathf.Max(0f, EditorGUILayout.FloatField("Screen Fade-in Duration:", (float)globalInfo.gameGUI.screenFadeDuration));
                        globalInfo.gameGUI.screenFadeColor = EditorGUILayout.ColorField("Screen Fade-in Color:", globalInfo.gameGUI.screenFadeColor);
                        globalInfo.gameGUI.gameFadeDuration = Mathf.Max(0f, EditorGUILayout.FloatField("Game Fade-in Duration:", (float)globalInfo.gameGUI.gameFadeDuration));
                        globalInfo.gameGUI.gameFadeColor = EditorGUILayout.ColorField("Game Fade-in Color:", globalInfo.gameGUI.gameFadeColor);
                        globalInfo.gameGUI.roundFadeDuration = Mathf.Max(0f, EditorGUILayout.FloatField("Round Fade-in Duration:", (float)globalInfo.gameGUI.roundFadeDuration));
                        globalInfo.gameGUI.roundFadeColor = EditorGUILayout.ColorField("Round Fade-in Color:", globalInfo.gameGUI.roundFadeColor);

                        EditorGUILayout.Space();
						globalInfo.gameGUI.hasGauge = EditorGUILayout.Toggle("Has Gauge/Meter", globalInfo.gameGUI.hasGauge);
						globalInfo.gameGUI.useCanvasScaler = EditorGUILayout.Toggle("Use Canvas Scaler", globalInfo.gameGUI.useCanvasScaler);
						
						if (globalInfo.gameGUI.useCanvasScaler){
							EditorGUILayout.BeginVertical(this.subGroupStyle);{
								EditorGUILayout.Space();
								globalInfo.gameGUI.canvasScaler.scaleMode = (CanvasScaler.ScaleMode)EditorGUILayout.EnumPopup("Scale Mode:", globalInfo.gameGUI.canvasScaler.scaleMode, enumStyle);
								if (globalInfo.gameGUI.canvasScaler.scaleMode == CanvasScaler.ScaleMode.ConstantPhysicalSize){
									globalInfo.gameGUI.canvasScaler.referencePixelsPerUnit = EditorGUILayout.FloatField("Reference Pixels Per Unit:", globalInfo.gameGUI.canvasScaler.referencePixelsPerUnit);
									globalInfo.gameGUI.canvasScaler.physicalUnit = (CanvasScaler.Unit) EditorGUILayout.EnumPopup("Physical Unit:", globalInfo.gameGUI.canvasScaler.physicalUnit, enumStyle);
									globalInfo.gameGUI.canvasScaler.fallbackScreenDPI = EditorGUILayout.FloatField("Fallback Screen DPI:", globalInfo.gameGUI.canvasScaler.fallbackScreenDPI);
									globalInfo.gameGUI.canvasScaler.defaultSpriteDPI = EditorGUILayout.FloatField("Default Sprite DPI:", globalInfo.gameGUI.canvasScaler.defaultSpriteDPI);
								}else if (globalInfo.gameGUI.canvasScaler.scaleMode == CanvasScaler.ScaleMode.ConstantPixelSize){
									globalInfo.gameGUI.canvasScaler.referencePixelsPerUnit = EditorGUILayout.FloatField("Reference Pixels Per Unit:", globalInfo.gameGUI.canvasScaler.referencePixelsPerUnit);
									globalInfo.gameGUI.canvasScaler.scaleFactor = EditorGUILayout.FloatField("Scale Factor:", globalInfo.gameGUI.canvasScaler.scaleFactor);
								}else if (globalInfo.gameGUI.canvasScaler.scaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize){
									globalInfo.gameGUI.canvasScaler.referencePixelsPerUnit = EditorGUILayout.FloatField("Reference Pixels Per Unit:", globalInfo.gameGUI.canvasScaler.referencePixelsPerUnit);
									globalInfo.gameGUI.canvasScaler.screenMatchMode = (CanvasScaler.ScreenMatchMode)EditorGUILayout.EnumPopup("Screen Match Mode:", globalInfo.gameGUI.canvasScaler.screenMatchMode, enumStyle);
									if (globalInfo.gameGUI.canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight){
										globalInfo.gameGUI.canvasScaler.matchWidthOrHeight = EditorGUILayout.Slider("Match Width or Height:", globalInfo.gameGUI.canvasScaler.matchWidthOrHeight, 0f, 1f);
									}
									globalInfo.gameGUI.canvasScaler.referenceResolution = EditorGUILayout.Vector2Field("Resolution:", globalInfo.gameGUI.canvasScaler.referenceResolution);
								}
                                EditorGUI.BeginDisabledGroup(canvasPreview == null);{
									if (GUILayout.Button("Update Canvas Preview")) UpdateGUICanvas();
                                } EditorGUI.EndDisabledGroup();
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
						}
						
						EditorGUILayout.Space();
						
						EditorGUILayout.BeginHorizontal();{
							guiScreensOptions = EditorGUILayout.Foldout(guiScreensOptions, "Screens", foldStyle);
						}EditorGUILayout.EndHorizontal();

                        if (guiScreensOptions) {
                            EditorGUIUtility.labelWidth = 150;
							EditorGUILayout.BeginVertical(this.subGroupStyle);{
								EditorGUILayout.Space();
								
                                SubGroupTitle("Main");
                                EditorGUILayout.BeginHorizontal();{
                                    globalInfo.gameGUI.mainMenuScreen = (MainMenuScreen)EditorGUILayout.ObjectField("Main Menu:", globalInfo.gameGUI.mainMenuScreen, typeof(MainMenuScreen), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.mainMenuScreen));{
                                       // if (GUILayout.Button("Open", GUILayout.Width(45))) OpenGUICanvas(globalInfo.gameGUI.mainMenuScreen);
                                        ScreenButton(globalInfo.gameGUI.mainMenuScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();{
                                    globalInfo.gameGUI.optionsScreen = (OptionsScreen)EditorGUILayout.ObjectField("Options:", globalInfo.gameGUI.optionsScreen, typeof(OptionsScreen), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.optionsScreen));{
                                        ScreenButton(globalInfo.gameGUI.optionsScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();
                                
								EditorGUILayout.BeginHorizontal();{
								    globalInfo.gameGUI.characterSelectionScreen = (CharacterSelectionScreen)EditorGUILayout.ObjectField("Character Selection:", globalInfo.gameGUI.characterSelectionScreen, typeof(CharacterSelectionScreen), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.characterSelectionScreen));{
                                        ScreenButton(globalInfo.gameGUI.characterSelectionScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();
                                
								EditorGUILayout.BeginHorizontal();{
								    globalInfo.gameGUI.stageSelectionScreen = (StageSelectionScreen)EditorGUILayout.ObjectField("Stage Selection:", globalInfo.gameGUI.stageSelectionScreen, typeof(StageSelectionScreen), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.stageSelectionScreen));{
                                        ScreenButton(globalInfo.gameGUI.stageSelectionScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();
                                
								EditorGUILayout.BeginHorizontal();{
								    globalInfo.gameGUI.loadingBattleScreen = (LoadingBattleScreen)EditorGUILayout.ObjectField("Loading Screen:", globalInfo.gameGUI.loadingBattleScreen, typeof(LoadingBattleScreen), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.loadingBattleScreen));{
                                        ScreenButton(globalInfo.gameGUI.loadingBattleScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();

                                
								EditorGUILayout.BeginHorizontal();{
								    globalInfo.gameGUI.battleGUI = (BattleGUI)EditorGUILayout.ObjectField("Battle GUI:", globalInfo.gameGUI.battleGUI, typeof(BattleGUI), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.battleGUI));{
                                        ScreenButton(globalInfo.gameGUI.battleGUI);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();

								EditorGUILayout.BeginHorizontal();{
									globalInfo.gameGUI.pauseScreen = (PauseScreen)EditorGUILayout.ObjectField("Pause Screen:", globalInfo.gameGUI.pauseScreen, typeof(PauseScreen), true);
									EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.pauseScreen));{
										ScreenButton(globalInfo.gameGUI.pauseScreen);
									} EditorGUI.EndDisabledGroup();
								} EditorGUILayout.EndHorizontal();

								EditorGUILayout.Space();


                                SubGroupTitle("Extras");
                                EditorGUILayout.BeginHorizontal();
                                {
                                    globalInfo.gameGUI.introScreen = (IntroScreen)EditorGUILayout.ObjectField("Intro Screen:", globalInfo.gameGUI.introScreen, typeof(IntroScreen), true, GUILayout.ExpandWidth(true));
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.introScreen));{
                                        ScreenButton(globalInfo.gameGUI.introScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();


                                EditorGUILayout.BeginHorizontal();
                                {
                                    globalInfo.gameGUI.creditsScreen = (CreditsScreen)EditorGUILayout.ObjectField("Credits:", globalInfo.gameGUI.creditsScreen, typeof(CreditsScreen), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.creditsScreen));{
                                        ScreenButton(globalInfo.gameGUI.creditsScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();

                                EditorGUILayout.Space();


								SubGroupTitle("Story Mode");
								EditorGUILayout.BeginHorizontal();{
								    globalInfo.gameGUI.storyModeContinueScreen = (StoryModeContinueScreen)EditorGUILayout.ObjectField("Continue?:", globalInfo.gameGUI.storyModeContinueScreen, typeof(StoryModeContinueScreen), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.storyModeContinueScreen));{
                                        ScreenButton(globalInfo.gameGUI.storyModeContinueScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();
                                
								EditorGUILayout.BeginHorizontal();{
								    globalInfo.gameGUI.storyModeGameOverScreen = (StoryModeScreen)EditorGUILayout.ObjectField("Game Over:", globalInfo.gameGUI.storyModeGameOverScreen, typeof(StoryModeScreen), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.storyModeGameOverScreen));{
                                        ScreenButton(globalInfo.gameGUI.storyModeGameOverScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();

								EditorGUILayout.BeginHorizontal();{
								    globalInfo.gameGUI.storyModeCongratulationsScreen = (StoryModeScreen)EditorGUILayout.ObjectField("Congratulations:", globalInfo.gameGUI.storyModeCongratulationsScreen, typeof(StoryModeScreen), true);
								    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.storyModeCongratulationsScreen));{
                                        ScreenButton(globalInfo.gameGUI.storyModeCongratulationsScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();

								EditorGUILayout.Space();
								

								SubGroupTitle("Versus Mode");
								EditorGUILayout.BeginHorizontal();{
								    globalInfo.gameGUI.versusModeScreen = (VersusModeScreen)EditorGUILayout.ObjectField("Options:", globalInfo.gameGUI.versusModeScreen, typeof(VersusModeScreen), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.versusModeScreen));{
                                        ScreenButton(globalInfo.gameGUI.versusModeScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();
                                
								EditorGUILayout.BeginHorizontal();{
								    globalInfo.gameGUI.versusModeAfterBattleScreen = (VersusModeAfterBattleScreen)EditorGUILayout.ObjectField("After Battle:", globalInfo.gameGUI.versusModeAfterBattleScreen, typeof(VersusModeAfterBattleScreen), true);
                                    EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.versusModeAfterBattleScreen));{
                                        ScreenButton(globalInfo.gameGUI.versusModeAfterBattleScreen);
                                    } EditorGUI.EndDisabledGroup();
                                } EditorGUILayout.EndHorizontal();
								
								if (UFE.isNetworkAddonInstalled){
                                    EditorGUILayout.Space();

                                    SubGroupTitle("Network Mode");

                                    EditorGUILayout.BeginHorizontal();{
                                        globalInfo.gameGUI.networkGameScreen = (NetworkGameScreen)EditorGUILayout.ObjectField("Network Screen:", globalInfo.gameGUI.networkGameScreen, typeof(NetworkGameScreen), true);
                                        EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.networkGameScreen));{
                                            ScreenButton(globalInfo.gameGUI.networkGameScreen);
                                        } EditorGUI.EndDisabledGroup();
                                    } EditorGUILayout.EndHorizontal();
                                    
								    EditorGUILayout.BeginHorizontal();{
									    globalInfo.gameGUI.hostGameScreen = (HostGameScreen)EditorGUILayout.ObjectField("Host Game:", globalInfo.gameGUI.hostGameScreen, typeof(HostGameScreen), true);
                                        EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.hostGameScreen));{
                                            ScreenButton(globalInfo.gameGUI.hostGameScreen);
                                        } EditorGUI.EndDisabledGroup();
                                    } EditorGUILayout.EndHorizontal();
                                    
								    EditorGUILayout.BeginHorizontal();{
									    globalInfo.gameGUI.joinGameScreen = (JoinGameScreen)EditorGUILayout.ObjectField("Join Game:", globalInfo.gameGUI.joinGameScreen, typeof(JoinGameScreen), true);
                                        EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.joinGameScreen));{
                                            ScreenButton(globalInfo.gameGUI.joinGameScreen);
                                        } EditorGUI.EndDisabledGroup();
                                    } EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();{
                                        globalInfo.gameGUI.searchMatchScreen = (SearchMatchScreen)EditorGUILayout.ObjectField("Search Match:", globalInfo.gameGUI.searchMatchScreen, typeof(SearchMatchScreen), true);
                                        EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.searchMatchScreen));{
                                            ScreenButton(globalInfo.gameGUI.searchMatchScreen);
                                        } EditorGUI.EndDisabledGroup();
                                    } EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();{
										globalInfo.gameGUI.bluetoothGameScreen = (BluetoothGameScreen)EditorGUILayout.ObjectField("Bluetooth Screen:", globalInfo.gameGUI.bluetoothGameScreen, typeof(BluetoothGameScreen), true);
										EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.bluetoothGameScreen));{
											ScreenButton(globalInfo.gameGUI.bluetoothGameScreen);
										} EditorGUI.EndDisabledGroup();
									} EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();{
									    globalInfo.gameGUI.connectionLostScreen = (ConnectionLostScreen)EditorGUILayout.ObjectField("Connection Lost:", globalInfo.gameGUI.connectionLostScreen, typeof(ConnectionLostScreen), true);
                                        EditorGUI.BeginDisabledGroup(DisableScreenButton(globalInfo.gameGUI.connectionLostScreen));{
                                            ScreenButton(globalInfo.gameGUI.connectionLostScreen);
                                        } EditorGUI.EndDisabledGroup();
                                    } EditorGUILayout.EndHorizontal();

								}
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
                        } else if (!storyModeOptions){
                            CloseGUICanvas();
                        }
						
						EditorGUILayout.Space();
						EditorGUIUtility.labelWidth = 150;
						EditorGUI.indentLevel -= 1;
						
					}EditorGUILayout.EndVertical();
                } else if (!storyModeOptions) {
                    CloseGUICanvas();
                }
			}EditorGUILayout.EndVertical();
			

			// Story Mode Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					storyModeOptions = EditorGUILayout.Foldout(storyModeOptions, "Story Mode Options", foldStyle);
					helpButton("global:storymode");
				}EditorGUILayout.EndHorizontal();
				
				if (storyModeOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						EditorGUILayout.Space();
						EditorGUIUtility.labelWidth = 180;

						EditorGUILayout.BeginVertical(subGroupStyle);{
							EditorGUILayout.Space();
							EditorGUILayout.BeginHorizontal();{
								storyModeSelectableCharactersInStoryModeOptions = EditorGUILayout.Foldout(storyModeSelectableCharactersInStoryModeOptions, "Selectable characters (Story Mode)", foldStyle);
							}EditorGUILayout.EndHorizontal();

							if (storyModeSelectableCharactersInStoryModeOptions){
								EditorGUI.indentLevel += 1;
								for (int i = 0; i < globalInfo.characters.Length; i ++){
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical(arrayElementStyle);{
										EditorGUILayout.Space();
										EditorGUILayout.BeginHorizontal();{
											string name;
											if (globalInfo.characters[i] != null){
												name = globalInfo.characters[i].characterName;
											}else{
												name = "Character " + i;
											}

											bool oldValue = globalInfo.storyMode.selectableCharactersInStoryMode.Contains(i);
											bool newValue = EditorGUILayout.Toggle(name, oldValue);
											
											if (oldValue != newValue){
												if (newValue){
													globalInfo.storyMode.selectableCharactersInStoryMode.Add(i);
													globalInfo.ValidateStoryModeInformation();
												}else{
													int index = globalInfo.storyMode.selectableCharactersInStoryMode.IndexOf(i);
													globalInfo.storyMode.selectableCharactersInStoryMode.RemoveAt(index);
													globalInfo.ValidateStoryModeInformation();
												}
											}
										}EditorGUILayout.EndHorizontal();
										EditorGUILayout.Space();
									}EditorGUILayout.EndVertical();
								}
								EditorGUI.indentLevel -= 1;
							}
							EditorGUILayout.Space();

						}EditorGUILayout.EndVertical();

						EditorGUILayout.Space();

						EditorGUILayout.BeginVertical(subGroupStyle);{
							EditorGUILayout.Space();
							EditorGUILayout.BeginHorizontal();{
								storyModeSelectableCharactersInVersusModeOptions = EditorGUILayout.Foldout(storyModeSelectableCharactersInVersusModeOptions, "Selectable characters (Versus Mode)", foldStyle);
							}EditorGUILayout.EndHorizontal();
							
							if (storyModeSelectableCharactersInVersusModeOptions){
								EditorGUI.indentLevel += 1;
								for (int i = 0; i < globalInfo.characters.Length; i ++){
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical(arrayElementStyle);{
										EditorGUILayout.Space();
										EditorGUILayout.BeginHorizontal();{
											string name;
											if (globalInfo.characters[i] != null){
												name = globalInfo.characters[i].characterName;
											}else{
												name = "Character " + i;
											}

											bool oldValue = globalInfo.storyMode.selectableCharactersInVersusMode.Contains(i);
											bool newValue = EditorGUILayout.Toggle(name, oldValue);
											
											if (oldValue != newValue){
												if (newValue){
													globalInfo.storyMode.selectableCharactersInVersusMode.Add(i);
												}else{
													int index = globalInfo.storyMode.selectableCharactersInVersusMode.IndexOf(i);
													globalInfo.storyMode.selectableCharactersInVersusMode.RemoveAt(index);
												}
											}
										}EditorGUILayout.EndHorizontal();
										EditorGUILayout.Space();
									}EditorGUILayout.EndVertical();
								}
								EditorGUI.indentLevel -= 1;
							}
							EditorGUILayout.Space();
							
						}EditorGUILayout.EndVertical();

						EditorGUILayout.Space();
						EditorGUILayout.Space();
						EditorGUIUtility.labelWidth = 250;

						globalInfo.storyMode.canCharactersFightAgainstThemselves = EditorGUILayout.Toggle("Allow mirror matches", globalInfo.storyMode.canCharactersFightAgainstThemselves);
						globalInfo.storyMode.useSameStoryForAllCharacters = EditorGUILayout.Toggle("Use the same story for all characters", globalInfo.storyMode.useSameStoryForAllCharacters);

						EditorGUIUtility.labelWidth = 180;
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						if (globalInfo.storyMode.useSameStoryForAllCharacters){
							this.EditCharacterStory(globalInfo.storyMode.defaultStory);
						}else{
							globalInfo.ValidateStoryModeInformation();

							for (int i = 0; i < globalInfo.characters.Length; i ++){
								if (globalInfo.storyMode.selectableCharactersInStoryMode.Contains(i)){
									string name;
									if (globalInfo.characters[i] != null){
										name = globalInfo.characters[i].characterName;
									}else{
										name = "Character " + i;
									}
									this.EditCharacterStory(globalInfo.storyMode.characterStories[i], name + "'s Story");
								}
							}
						}
						
						EditorGUIUtility.labelWidth = 150;
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


            // Training Mode Options
            EditorGUILayout.BeginVertical(rootGroupStyle);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    trainingModeOptions = EditorGUILayout.Foldout(trainingModeOptions, "Training Mode Options", foldStyle);
                    helpButton("global:trainingmode");
                } EditorGUILayout.EndHorizontal();

                if (trainingModeOptions) {
                    EditorGUILayout.BeginVertical(subGroupStyle);
                    {
                        EditorGUILayout.Space();
                        EditorGUI.indentLevel += 1;
                        EditorGUIUtility.labelWidth = 200;
                        globalInfo.trainingModeOptions.inputInfo = EditorGUILayout.Toggle("Display Input", globalInfo.trainingModeOptions.inputInfo);
                        globalInfo.trainingModeOptions.freezeTime = EditorGUILayout.Toggle("Freeze Timer", globalInfo.trainingModeOptions.freezeTime);
                        globalInfo.trainingModeOptions.p1StartingLife = EditorGUILayout.Slider("Player 1 Starting Life:", globalInfo.trainingModeOptions.p1StartingLife, 1, 100);
                        globalInfo.trainingModeOptions.p2StartingLife = EditorGUILayout.Slider("Player 2 Starting Life:", globalInfo.trainingModeOptions.p2StartingLife, 1, 100);
                        globalInfo.trainingModeOptions.p1StartingGauge = EditorGUILayout.Slider("Player 1 Starting Gauge:", globalInfo.trainingModeOptions.p1StartingGauge, 0, 100);
                        globalInfo.trainingModeOptions.p2StartingGauge = EditorGUILayout.Slider("Player 2 Starting Gauge:", globalInfo.trainingModeOptions.p2StartingGauge, 0, 100);
                        globalInfo.trainingModeOptions.p1Life = (LifeBarTrainingMode)EditorGUILayout.EnumPopup("Player 1 Life:", globalInfo.trainingModeOptions.p1Life, enumStyle);
                        globalInfo.trainingModeOptions.p2Life = (LifeBarTrainingMode)EditorGUILayout.EnumPopup("Player 2 Life:", globalInfo.trainingModeOptions.p2Life, enumStyle);
                        globalInfo.trainingModeOptions.p1Gauge = (LifeBarTrainingMode)EditorGUILayout.EnumPopup("Player 1 Gauge:", globalInfo.trainingModeOptions.p1Gauge, enumStyle);
                        globalInfo.trainingModeOptions.p2Gauge = (LifeBarTrainingMode)EditorGUILayout.EnumPopup("Player 2 Gauge:", globalInfo.trainingModeOptions.p2Gauge, enumStyle);
                        globalInfo.trainingModeOptions.refillTime = EditorGUILayout.FloatField("Refill Time (seconds)", globalInfo.trainingModeOptions.refillTime);
                        EditorGUIUtility.labelWidth = 150;

                        EditorGUI.indentLevel -= 1;
                        EditorGUILayout.Space();
                    } EditorGUILayout.EndVertical();
                }
            } EditorGUILayout.EndVertical();


            // Challenge Mode Options
            EditorGUILayout.BeginVertical(rootGroupStyle);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    challengeModeOptions = EditorGUILayout.Foldout(challengeModeOptions, "Challenge Mode (" + globalInfo.challengeModeOptions.Length + ")", foldStyle);
                    helpButton("global:challengeMode");
                } EditorGUILayout.EndHorizontal();

                if (challengeModeOptions) {
                    EditorGUILayout.BeginVertical(subGroupStyle);
                    {
                        EditorGUILayout.Space();
                        EditorGUI.indentLevel += 1;

                        for (int i = 0; i < globalInfo.challengeModeOptions.Length; i++) {
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginVertical(arrayElementStyle);
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                {
                                    if (globalInfo.challengeModeOptions[i].challengeName == "")
                                        globalInfo.challengeModeOptions[i].challengeName = "Challenge " + (i + 1);

                                    globalInfo.challengeModeOptions[i].challengeName = EditorGUILayout.TextField("Name:", globalInfo.challengeModeOptions[i].challengeName);
                                    if (GUILayout.Button("", "PaneOptions")) {
                                        PaneOptions<ChallengeModeOptions>(
                                            globalInfo.challengeModeOptions,
                                            globalInfo.challengeModeOptions[i],
                                            delegate(ChallengeModeOptions[] newElement) {
                                                globalInfo.challengeModeOptions = newElement;
                                            }
                                        );
                                    }
                                } EditorGUILayout.EndHorizontal();
                                globalInfo.challengeModeOptions[i].description = EditorGUILayout.TextField("Description:", globalInfo.challengeModeOptions[i].description);
                                globalInfo.challengeModeOptions[i].character = (UFE3D.CharacterInfo)EditorGUILayout.ObjectField("Character (P1):", globalInfo.challengeModeOptions[i].character, typeof(UFE3D.CharacterInfo), false);
                                globalInfo.challengeModeOptions[i].opCharacter = (UFE3D.CharacterInfo)EditorGUILayout.ObjectField("Opponent (P2):", globalInfo.challengeModeOptions[i].opCharacter, typeof(UFE3D.CharacterInfo), false);
                                globalInfo.challengeModeOptions[i].repeats = EditorGUILayout.IntField("Repeats", globalInfo.challengeModeOptions[i].repeats);
                                globalInfo.challengeModeOptions[i].challengeSequence = (ChallengeAutoSequence)EditorGUILayout.EnumPopup("On success:", globalInfo.challengeModeOptions[i].challengeSequence, enumStyle);

                                EditorGUIUtility.labelWidth = 200;
                                if (globalInfo.challengeModeOptions[i].challengeSequence == ChallengeAutoSequence.MoveToNext)
                                    globalInfo.challengeModeOptions[i].resetData = EditorGUILayout.Toggle("Reset round on success", globalInfo.challengeModeOptions[i].resetData);

                                globalInfo.challengeModeOptions[i].aiOpponent = EditorGUILayout.Toggle("AI Opponent", globalInfo.challengeModeOptions[i].aiOpponent);
                                if (globalInfo.challengeModeOptions[i].aiOpponent) globalInfo.challengeModeOptions[i].ai = (SimpleAIBehaviour)EditorGUILayout.ObjectField("Simple AI Behaviour:", globalInfo.challengeModeOptions[i].ai, typeof(SimpleAIBehaviour), false);
                                globalInfo.challengeModeOptions[i].isCombo = EditorGUILayout.Toggle("Is Combo", globalInfo.challengeModeOptions[i].isCombo);
                                EditorGUIUtility.labelWidth = 150;

                                globalInfo.challengeModeOptions[i].actionListToggle = EditorGUILayout.Foldout(globalInfo.challengeModeOptions[i].actionListToggle, "Action List (" + globalInfo.challengeModeOptions[i].actionSequence.Length + ")", foldStyle);
                                if (globalInfo.challengeModeOptions[i].actionListToggle) {
                                    EditorGUILayout.BeginVertical(subGroupStyle);
                                    {
                                        EditorGUI.indentLevel += 1;
                                        for (int k = 0; k < globalInfo.challengeModeOptions[i].actionSequence.Length; k++) {
                                            EditorGUILayout.Space();
                                            EditorGUILayout.BeginVertical(arrayElementStyle);
                                            {
                                                EditorGUILayout.Space();
                                                EditorGUILayout.BeginHorizontal();
                                                {
                                                    globalInfo.challengeModeOptions[i].actionSequence[k].actionType = (ActionType)EditorGUILayout.EnumPopup("Action Type:", globalInfo.challengeModeOptions[i].actionSequence[k].actionType, enumStyle);
                                                    if (GUILayout.Button("", "PaneOptions")) {
                                                        PaneOptions<ActionSequence>(globalInfo.challengeModeOptions[i].actionSequence, globalInfo.challengeModeOptions[i].actionSequence[k], delegate(ActionSequence[] newElement) { globalInfo.challengeModeOptions[i].actionSequence = newElement; });
                                                    }
                                                } EditorGUILayout.EndHorizontal();
                                                EditorGUILayout.Space();

                                                if (globalInfo.challengeModeOptions[i].actionSequence[k].actionType == ActionType.BasicMove) {
                                                    globalInfo.challengeModeOptions[i].actionSequence[k].basicMove = (BasicMoveReference)EditorGUILayout.EnumPopup("Basic Move:", globalInfo.challengeModeOptions[i].actionSequence[k].basicMove, enumStyle);

                                                } else if (globalInfo.challengeModeOptions[i].actionSequence[k].actionType == ActionType.ButtonPress) {
                                                    globalInfo.challengeModeOptions[i].actionSequence[k].button = (ButtonPress)EditorGUILayout.EnumPopup("Button:", globalInfo.challengeModeOptions[i].actionSequence[k].button, enumStyle);
                                                    EditorGUIUtility.labelWidth = 200;
                                                    globalInfo.challengeModeOptions[i].actionSequence[k].onlyAllowThisButton = EditorGUILayout.Toggle("Only allow this button", globalInfo.challengeModeOptions[i].resetData);
                                                    EditorGUIUtility.labelWidth = 150;

                                                } else if (globalInfo.challengeModeOptions[i].actionSequence[k].actionType == ActionType.SpecialMove) {
                                                    globalInfo.challengeModeOptions[i].actionSequence[k].specialMove = (MoveInfo)EditorGUILayout.ObjectField("Special Move:", globalInfo.challengeModeOptions[i].actionSequence[k].specialMove, typeof(MoveInfo), false);

                                                }

                                                EditorGUILayout.Space();
                                            } EditorGUILayout.EndVertical();
                                        }
                                        EditorGUILayout.Space();
                                        if (StyledButton("New Action"))
                                            globalInfo.challengeModeOptions[i].actionSequence = AddElement<ActionSequence>(globalInfo.challengeModeOptions[i].actionSequence, new ActionSequence());

                                        EditorGUI.indentLevel -= 1;
                                    } EditorGUILayout.EndVertical();
                                }

                                EditorGUILayout.Space();
                            } EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                        }
                        EditorGUI.indentLevel -= 1;

                        if (StyledButton("New Challenge")) {
                            globalInfo.challengeModeOptions = AddElement<ChallengeModeOptions>(globalInfo.challengeModeOptions, new ChallengeModeOptions());
                        }

                    } EditorGUILayout.EndVertical();
                }
            } EditorGUILayout.EndVertical();


            // Preload Options
            EditorGUILayout.BeginVertical(rootGroupStyle);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    preloadOptions = EditorGUILayout.Foldout(preloadOptions, "Preload Options", foldStyle);
                    helpButton("global:preload");
                } EditorGUILayout.EndHorizontal();

                if (preloadOptions) {
                    EditorGUILayout.BeginVertical(subGroupStyle);
                    {
                        EditorGUILayout.Space();
                        EditorGUI.indentLevel += 1;
                        EditorGUIUtility.labelWidth = 200;
                        globalInfo._preloadingTime = EditorGUILayout.FloatField("Preloading Time:", (float)globalInfo._preloadingTime);
                        globalInfo.preloadHitEffects = EditorGUILayout.Toggle("Hit Effects", globalInfo.preloadHitEffects);
                        globalInfo.preloadCharacter1 = EditorGUILayout.Toggle("Player 1 Character & Moves", globalInfo.preloadCharacter1);
                        globalInfo.preloadCharacter2 = EditorGUILayout.Toggle("Player 2 Character & Moves", globalInfo.preloadCharacter2);
                        globalInfo.preloadStage = EditorGUILayout.Toggle("Stage", globalInfo.preloadStage);
                        globalInfo.warmAllShaders = EditorGUILayout.Toggle("Warm All Shaders", globalInfo.warmAllShaders);

                        EditorGUIUtility.labelWidth = 150;
                        EditorGUI.indentLevel -= 1;
                        EditorGUILayout.Space();
                    } EditorGUILayout.EndVertical();
                }
            } EditorGUILayout.EndVertical();


			// Advanced Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					advancedOptions = EditorGUILayout.Foldout(advancedOptions, "Advanced Options", foldStyle);
					helpButton("global:advanced");
				}EditorGUILayout.EndHorizontal();

				if (advancedOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						EditorGUIUtility.labelWidth = 180;

						EditorGUILayout.Space();
						UFE.fps = EditorGUILayout.IntField("Frames Per Second:", UFE.fps);
						globalInfo.executionBufferType = (ExecutionBufferType)EditorGUILayout.EnumPopup("Execution Buffer Type:", globalInfo.executionBufferType, enumStyle);
						EditorGUI.BeginDisabledGroup(globalInfo.executionBufferType == ExecutionBufferType.NoBuffer);{
							globalInfo.executionBufferTime = EditorGUILayout.IntField("Execution Buffer (frames):", Mathf.Clamp(globalInfo.executionBufferTime, 1, int.MaxValue));
						}EditorGUI.EndDisabledGroup();
						globalInfo.plinkingDelay = EditorGUILayout.IntField("Plinking Delay (frames):", Mathf.Clamp(globalInfo.plinkingDelay, 1, int.MaxValue));
						globalInfo._gameSpeed = EditorGUILayout.Slider("Game Speed:", (float)globalInfo._gameSpeed, .01f, 2);
						globalInfo._gravity = EditorGUILayout.FloatField("Global Gravity:", (float)globalInfo._gravity);
                        globalInfo.detect3D_Hits = EditorGUILayout.Toggle("3D Hit Detection", globalInfo.detect3D_Hits);
                        globalInfo.runInBackground = EditorGUILayout.Toggle("Run in Background", globalInfo.runInBackground);
						
						EditorGUIUtility.labelWidth = 150;
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


            EditorGUI.BeginDisabledGroup(!UFE.isNetworkAddonInstalled);
            //if (UFE.isNetworkAddonInstalled){
				EditorGUILayout.BeginVertical(rootGroupStyle);{
					EditorGUILayout.BeginHorizontal();{
						networkOptions = EditorGUILayout.Foldout(networkOptions, "Network Options", foldStyle);
						helpButton("global:network");
					}EditorGUILayout.EndHorizontal();

					if (networkOptions){
                        EditorGUILayout.BeginVertical(subGroupStyle);
                        {
                            EditorGUILayout.Space();
                            EditorGUI.indentLevel += 1;

                            SubGroupTitle("Online Service");
                            EditorGUIUtility.labelWidth = 200;
                            globalInfo.networkOptions.networkService = (NetworkService)EditorGUILayout.EnumPopup("Network Service:", globalInfo.networkOptions.networkService, enumStyle);

                            if (globalInfo.networkOptions.networkService == NetworkService.Photon) {
                                if (UFE.isPhotonInstalled) {
                                    globalInfo.networkOptions.photonHostingService = (PhotonHostingService)EditorGUILayout.EnumPopup("Photon Service:", globalInfo.networkOptions.photonHostingService);
                                    /*globalInfo.networkOptions.photonApplicationId = EditorGUILayout.TextField("Photon Application ID:", globalInfo.networkOptions.photonApplicationId);
                                    if (globalInfo.networkOptions.photonHostingService == PhotonHostingService.PlayFab) {
                                        globalInfo.networkOptions.playFabTitleId = EditorGUILayout.TextField("PlayFab Title Id:", globalInfo.networkOptions.playFabTitleId);
                                    }*/
                                } else {
                                    GUILayout.BeginHorizontal("GroupBox");
                                    GUILayout.Label("Photon Unity Network Missing", "CN EntryWarn");
                                    GUILayout.EndHorizontal();
                                }
                            }
                            EditorGUILayout.Space();

                            if (UFE.isBluetoothAddonInstalled) {
                                EditorGUILayout.BeginVertical(subArrayElementStyle);{
                                    EditorGUILayout.LabelField("Bluetooth installed.");
                                } EditorGUILayout.EndVertical();
                            }

                            EditorGUILayout.Space();

                            EditorGUI.BeginDisabledGroup(globalInfo.networkOptions.networkService == NetworkService.Disabled);

                            SubGroupTitle("LAN Games");
                            EditorGUIUtility.labelWidth = 220;
                            globalInfo.networkOptions.port = EditorGUILayout.IntField("Network Port:", globalInfo.networkOptions.port);
                            globalInfo.networkOptions.lanDiscoveryPort = EditorGUILayout.IntField("LAN Discovery Port:", globalInfo.networkOptions.lanDiscoveryPort);
                            globalInfo.networkOptions.lanDiscoveryBroadcastInterval = Mathf.Max(0f, EditorGUILayout.FloatField("LAN Discovery Broadcast Interval", globalInfo.networkOptions.lanDiscoveryBroadcastInterval));
                            globalInfo.networkOptions.lanDiscoverySearchInterval = Mathf.Max(0f, EditorGUILayout.FloatField("LAN Discovery Search Interval", globalInfo.networkOptions.lanDiscoverySearchInterval));
                            globalInfo.networkOptions.lanDiscoverySearchTimeout = Mathf.Max(0f, EditorGUILayout.FloatField("LAN Discovery Search Timeout", globalInfo.networkOptions.lanDiscoverySearchTimeout));

                            EditorGUILayout.Space();

                            SubGroupTitle("Animation Control");
                            EditorGUIUtility.labelWidth = 200;
                            globalInfo.networkOptions.forceAnimationControl = EditorGUILayout.Toggle("Force UFE Animation Control", globalInfo.networkOptions.forceAnimationControl);
                            globalInfo.networkOptions.disableRootMotion = EditorGUILayout.Toggle("Disable Root Motion", globalInfo.networkOptions.disableRootMotion);
                            globalInfo.networkOptions.disableBlending = EditorGUILayout.Toggle("Disable Blending", globalInfo.networkOptions.disableBlending);
                            globalInfo.networkOptions.disableRotationBlend = EditorGUILayout.Toggle("Disable Rotation Blend", globalInfo.networkOptions.disableRotationBlend);

                            EditorGUILayout.Space();

                            SubGroupTitle("Package Options");
                            EditorGUIUtility.labelWidth = 200;
                            globalInfo.networkOptions.networkMessageSize = (NetworkMessageSize)EditorGUILayout.EnumPopup("Network Message Size", globalInfo.networkOptions.networkMessageSize);
                            globalInfo.networkOptions.inputMessageFrequency = (NetworkInputMessageFrequency)EditorGUILayout.EnumPopup("Send Input Message", globalInfo.networkOptions.inputMessageFrequency);
                            globalInfo.networkOptions.onlySendInputChanges = EditorGUILayout.Toggle("Only Send Input Changes", globalInfo.networkOptions.onlySendInputChanges);

                            EditorGUILayout.Space();

                            SubGroupTitle("Rollback Netcode");
                            EditorGUIUtility.labelWidth = 220;
#if UFE_LITE || UFE_BASIC || UFE_STANDARD
                            globalInfo.networkOptions.allowRollBacks = false;
#else
                            globalInfo.networkOptions.allowRollBacks = EditorGUILayout.Toggle("Enable Rollback", globalInfo.networkOptions.allowRollBacks);
#endif
                            EditorGUI.BeginDisabledGroup(!globalInfo.networkOptions.allowRollBacks);
                            {
                                globalInfo.networkOptions.ufeTrackers = EditorGUILayout.Toggle("Track UFE Variables: ", globalInfo.networkOptions.ufeTrackers);
                                globalInfo.networkOptions.maxFastForwards = Mathf.Max(0, EditorGUILayout.IntField("Max Fast-Forwards Per Frame", globalInfo.networkOptions.maxFastForwards));
                                globalInfo.networkOptions.maxBufferSize = EditorGUILayout.IntField("Input Buffer Size: ", globalInfo.networkOptions.maxBufferSize);
                                globalInfo.networkOptions.spawnBuffer = Mathf.Max(1, EditorGUILayout.IntField("Spawn Buffer Size: ", globalInfo.networkOptions.spawnBuffer));
                                globalInfo.networkOptions.rollbackBalancing = (NetworkRollbackBalancing)EditorGUILayout.EnumPopup("Rollback Balancing", globalInfo.networkOptions.rollbackBalancing);

                            } EditorGUI.EndDisabledGroup();

                            EditorGUILayout.Space();

                            SubGroupTitle("Frame Delay Netcode");
                            EditorGUIUtility.labelWidth = 200;
                            globalInfo.networkOptions.frameDelayType = (NetworkFrameDelay)EditorGUILayout.EnumPopup("Frame Delay Type", globalInfo.networkOptions.frameDelayType);

                            EditorGUI.BeginDisabledGroup(globalInfo.networkOptions.frameDelayType == NetworkFrameDelay.Disabled);
                            {
                                if (globalInfo.networkOptions.frameDelayType != NetworkFrameDelay.Auto) {
                                    globalInfo.networkOptions.defaultFrameDelay = EditorGUILayout.IntSlider(
                                        "Default Frame Delay: ",
                                        globalInfo.networkOptions.defaultFrameDelay,
                                        0,
                                        60
                                    );
                                }

                                if (globalInfo.networkOptions.frameDelayType != NetworkFrameDelay.Fixed) {
                                    globalInfo.networkOptions.minFrameDelay = Mathf.Clamp(
                                        EditorGUILayout.IntField("Min Frame Delay: ", globalInfo.networkOptions.minFrameDelay),
                                        0,
                                        globalInfo.networkOptions.maxFrameDelay
                                    );

                                    globalInfo.networkOptions.maxFrameDelay = Mathf.Clamp(
                                        EditorGUILayout.IntField("Max Frame Delay: ", globalInfo.networkOptions.maxFrameDelay),
                                        globalInfo.networkOptions.minFrameDelay,
                                        60
                                    );
                                }

                                globalInfo.networkOptions.applyFrameDelayOffline = EditorGUILayout.Toggle("Apply Frame Delay Offline", globalInfo.networkOptions.applyFrameDelayOffline);

                            } EditorGUI.EndDisabledGroup();

                            if (globalInfo.networkOptions.frameDelayType == NetworkFrameDelay.Disabled) {
                                globalInfo.networkOptions.defaultFrameDelay = 0;
                            }

                            EditorGUILayout.Space();
                            
                            /*
                            SubGroupTitle("Sync Handling");
                            EditorGUIUtility.labelWidth = 220;

                            globalInfo.networkOptions.synchronizationMessageFrequency = (NetworkSynchronizationMessageFrequency)EditorGUILayout.EnumPopup("Send Synchronization Message", globalInfo.networkOptions.synchronizationMessageFrequency);
                            EditorGUI.BeginDisabledGroup(globalInfo.networkOptions.synchronizationMessageFrequency == NetworkSynchronizationMessageFrequency.Disabled);
                            globalInfo.networkOptions.floatDesynchronizationThreshold = EditorGUILayout.Slider("Float Desync Threshold", globalInfo.networkOptions.floatDesynchronizationThreshold, 0f, 1f);
                            globalInfo.networkOptions.desynchronizationRecovery = EditorGUILayout.Toggle("Try to Recover from Desync", globalInfo.networkOptions.desynchronizationRecovery);
                            globalInfo.networkOptions.disconnectOnDesynchronization = EditorGUILayout.Toggle("Disconnect on Desync", globalInfo.networkOptions.disconnectOnDesynchronization);
                            EditorGUI.BeginDisabledGroup(!globalInfo.networkOptions.disconnectOnDesynchronization);
                            globalInfo.networkOptions.allowedDesynchronizations = Mathf.Max(0, EditorGUILayout.IntField("Max Allowed Desync", globalInfo.networkOptions.allowedDesynchronizations));
                            EditorGUI.EndDisabledGroup();
                            EditorGUI.EndDisabledGroup();
                            */
                        
                            EditorGUI.EndDisabledGroup();

                            EditorGUIUtility.labelWidth = 150;
                            EditorGUI.indentLevel -= 1;
                            EditorGUILayout.Space();
                        } EditorGUILayout.EndVertical();
					}
				}EditorGUILayout.EndVertical();
			//}
            EditorGUI.EndDisabledGroup();

			}
        EditorGUILayout.EndScrollView();

        if (SystemInfo.deviceUniqueIdentifier == "eb1bb2d8e99b3e170b1d91fc9b64348ff1bbc264" && 
            (Application.dataPath.Contains("Unity Projects/UFE 2 PRO/") || Application.dataPath.Contains("Unity Projects/UFE 2/"))) {

            pName = EditorGUILayout.TextField("Package Name:", pName);

            if (StyledButton("Export Package")) {
                string[] projectContent = new string[] {
                    "Assets/StreamingAssets",
                    "Assets/UFE",
                    "ProjectSettings/TagManager.asset",
                    "ProjectSettings/TimeManager.asset",
                    "ProjectSettings/InputManager.asset"
                };

                AssetDatabase.ExportPackage(projectContent, pName + ".unitypackage", ExportPackageOptions.Recurse);
            }
		}

		if (GUI.changed) {
			Undo.RecordObject(globalInfo, "Global Editor Modify");
			EditorUtility.SetDirty(globalInfo);
		}
	}


    public void NetworkUserDataBlock(string foldName, NetworkUserData[] userData, string valueName, ref bool toggle, Action<NetworkUserData[]> callback) {
        toggle = EditorGUILayout.Foldout(toggle, foldName + " (" + userData.Length + ")", foldStyle);
        if (toggle) {
            EditorGUILayout.BeginVertical(subGroupStyle);{
                //EditorGUI.indentLevel += 1;
                for (int i = 0; i < userData.Length; ++i) {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(subArrayElementStyle);{

                        EditorGUILayout.BeginHorizontal();{
                            userData[i].variableName = EditorGUILayout.TextField("Variable Name:", userData[i].variableName);

                            if (GUILayout.Button("", "PaneOptions")) {
                                PaneOptions<NetworkUserData>(userData, userData[i], delegate(NetworkUserData[] newElement) { if (callback != null)callback(newElement); });
                                return;
                            }
                        } EditorGUILayout.EndHorizontal();

                        if (valueName == "filter") {
                            userData[i].matchMakingFilterType = (MatchMakingFilterType)EditorGUILayout.EnumPopup("Variable Update Type:", userData[i].matchMakingFilterType, enumStyle);
                            userData[i].variableType = (ServerVariableType)EditorGUILayout.EnumPopup("Variable Type:", userData[i].variableType, enumStyle);

                            if (userData[i].variableType == ServerVariableType.Float) {
                                if (userData[i].matchMakingFilterType == MatchMakingFilterType.Different) {
                                    userData[i].floatValue = EditorGUILayout.FloatField("Different Then:", userData[i].floatValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.Equal) {
                                    userData[i].floatValue = EditorGUILayout.FloatField("Equal To:", userData[i].floatValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.HigherThen) {
                                    userData[i].floatValue = EditorGUILayout.FloatField("Higher Then:", userData[i].floatValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.LowerThen) {
                                    userData[i].floatValue = EditorGUILayout.FloatField("Lower Then:", userData[i].floatValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.Range) {
                                    userData[i].floatValue = EditorGUILayout.FloatField("Range:", userData[i].floatValue);
                                }
                            } else if (userData[i].variableType == ServerVariableType.Integer) {
                                if (userData[i].matchMakingFilterType == MatchMakingFilterType.Different) {
                                    userData[i].intValue = EditorGUILayout.IntField("Different Then:", userData[i].intValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.Equal) {
                                    userData[i].intValue = EditorGUILayout.IntField("Equal To:", userData[i].intValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.HigherThen) {
                                    userData[i].intValue = EditorGUILayout.IntField("Higher Then:", userData[i].intValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.LowerThen) {
                                    userData[i].intValue = EditorGUILayout.IntField("Lower Then:", userData[i].intValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.Range) {
                                    userData[i].intValue = EditorGUILayout.IntField("Range:", userData[i].intValue);
                                }
                            } else if (userData[i].variableType == ServerVariableType.String) {
                                if (userData[i].matchMakingFilterType == MatchMakingFilterType.Different) {
                                    userData[i].stringValue = EditorGUILayout.TextField("Different Then:", userData[i].stringValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.Equal) {
                                    userData[i].stringValue = EditorGUILayout.TextField("Equal To:", userData[i].stringValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.HigherThen) {
                                    userData[i].stringValue = EditorGUILayout.TextField("Higher Then:", userData[i].stringValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.LowerThen) {
                                    userData[i].stringValue = EditorGUILayout.TextField("Lower Then:", userData[i].stringValue);
                                } else if (userData[i].matchMakingFilterType == MatchMakingFilterType.Range) {
                                    userData[i].stringValue = EditorGUILayout.TextField("Range:", userData[i].stringValue);
                                }
                            } else {
                                userData[i].ufeBoolean = (UFEBoolean)EditorGUILayout.EnumPopup("New Value:", userData[i].ufeBoolean, enumStyle);
                                userData[i].boolValue = userData[i].ufeBoolean == UFEBoolean.TRUE ? true : false;
                            }

                        } else {
                            userData[i].variableUpdateType = (ServerVariableUpdateType)EditorGUILayout.EnumPopup("Variable Update Type:", userData[i].variableUpdateType, enumStyle);
                            if (userData[i].variableUpdateType == ServerVariableUpdateType.ELO) {
                                EditorGUI.BeginDisabledGroup(true);
                                EditorGUILayout.TextField("New Value:", "(Automatic)");
                                EditorGUI.EndDisabledGroup();
                            } else if (userData[i].variableUpdateType == ServerVariableUpdateType.Increment) {
                                userData[i].floatValue = EditorGUILayout.FloatField("Increment:", userData[i].floatValue);
                            } else {
                                userData[i].variableType = (ServerVariableType)EditorGUILayout.EnumPopup("Variable Type:", userData[i].variableType, enumStyle);
                                if (userData[i].variableType == ServerVariableType.Float) {
                                    userData[i].floatValue = EditorGUILayout.FloatField("New Value:", userData[i].floatValue);
                                } else if (userData[i].variableType == ServerVariableType.Integer) {
                                    userData[i].intValue = EditorGUILayout.IntField("New Value:", userData[i].intValue);
                                } else if (userData[i].variableType == ServerVariableType.String) {
                                    userData[i].stringValue = EditorGUILayout.TextField("New Value:", userData[i].stringValue);
                                } else {
                                    userData[i].ufeBoolean = (UFEBoolean)EditorGUILayout.EnumPopup("New Value:", userData[i].ufeBoolean, enumStyle);
                                    userData[i].boolValue = userData[i].ufeBoolean == UFEBoolean.TRUE ? true : false;
                                }
                            }
                        }

                    } EditorGUILayout.EndVertical();
                }
                //EditorGUI.indentLevel -= 1;

            EditorGUILayout.Space();
            if (StyledButton("New Variable")) {
                callback(AddElement<NetworkUserData>(userData, new NetworkUserData()));
                //userData = AddElement<NetworkUserData>(userData, new NetworkUserData());
            }

            EditorGUILayout.Space();

            } EditorGUILayout.EndVertical();
        }
    }


    public bool DisableScreenButton(UFEScreen screen) {
        if (screen == null || (screenPreview != null && screenPreview.name != screen.name)) {
            return true;
        }
        return false;
    }

    public void ScreenButton(UFEScreen screen) {
        if (screenPreview != null && screen != null && screenPreview.name == screen.name) {
            if (GUILayout.Button("Close", GUILayout.Width(45))) CloseGUICanvas();
        } else {
            if (GUILayout.Button("Open", GUILayout.Width(45))) OpenGUICanvas(screen);
        }
    }

	public void UpdateGUICanvas() {
		if (canvasPreview != null) {
			CanvasScaler cScaler = canvasPreview.GetComponent<CanvasScaler>();
			if (canvasPreview.GetComponent<Canvas>() == null) {
				cScaler = canvasPreview.AddComponent<CanvasScaler>();
			}
			cScaler.uiScaleMode = globalInfo.gameGUI.canvasScaler.scaleMode;
			cScaler.referencePixelsPerUnit = globalInfo.gameGUI.canvasScaler.referencePixelsPerUnit;
			cScaler.screenMatchMode	= globalInfo.gameGUI.canvasScaler.screenMatchMode;
			cScaler.matchWidthOrHeight = globalInfo.gameGUI.canvasScaler.matchWidthOrHeight;
			cScaler.referenceResolution = globalInfo.gameGUI.canvasScaler.referenceResolution;
		}
	}

    public void OpenGUICanvas(UFEScreen screen) {
        CloseGUICanvas();

        if (screen.canvasPreview) {
            canvasPreview = new GameObject("Canvas");
            canvasPreview.AddComponent<Canvas>();
            CanvasScaler cScaler = canvasPreview.AddComponent<CanvasScaler>();
            canvasPreview.AddComponent<GraphicRaycaster>();
            canvasPreview.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasPreview.layer = 5;

			cScaler.uiScaleMode = globalInfo.gameGUI.canvasScaler.scaleMode;
			cScaler.referencePixelsPerUnit = globalInfo.gameGUI.canvasScaler.referencePixelsPerUnit;
			cScaler.screenMatchMode	= globalInfo.gameGUI.canvasScaler.screenMatchMode;
			cScaler.matchWidthOrHeight = globalInfo.gameGUI.canvasScaler.matchWidthOrHeight;
			cScaler.referenceResolution = globalInfo.gameGUI.canvasScaler.referenceResolution;

            eventSystemPreview = new GameObject("EventSystem");
            eventSystemPreview.AddComponent<EventSystem>();
            eventSystemPreview.AddComponent<StandaloneInputModule>();
            //eventSystemPreview.AddComponent<TouchInputModule>();

            screenPreview = (UFEScreen)PrefabUtility.InstantiatePrefab(screen);
            (screenPreview.transform as RectTransform).SetParent(canvasPreview.transform);

            (screenPreview.transform as RectTransform).anchorMin = Vector2.zero;
            (screenPreview.transform as RectTransform).anchorMax = Vector2.one;
            (screenPreview.transform as RectTransform).offsetMin = Vector2.zero;
            (screenPreview.transform as RectTransform).offsetMax = Vector2.zero;

            EditorWindow.FocusWindowIfItsOpen<SceneView>();
            Selection.activeObject = screenPreview;
        } else {
            Selection.activeObject = screen;
        }

        System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
        System.Type type = assembly.GetType("UnityEditor.InspectorWindow");
        EditorWindow inspectorView = EditorWindow.GetWindow(type);
        inspectorView.Focus();

    }

    public void CloseGUICanvas() {
        if (screenPreview != null) {
            Editor.DestroyImmediate(screenPreview);
            screenPreview = null;
        }
        if (canvasPreview != null) {
            Editor.DestroyImmediate(canvasPreview);
            canvasPreview = null;
        }
        if (eventSystemPreview != null) {
            Editor.DestroyImmediate(eventSystemPreview);
            eventSystemPreview = null;
        }
    }

	public bool StyledButton (string label) {
		EditorGUILayout.Space();
		GUILayoutUtility.GetRect(1, 20);
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		bool clickResult = GUILayout.Button(label, addButtonStyle);
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		return clickResult;
	}

    public void CharacterDebugOptions(CharacterDebugInfo debugInfo, string label) {
        debugInfo.toggle = EditorGUILayout.Foldout(debugInfo.toggle, label, foldStyle);
        if (debugInfo.toggle) {
            EditorGUILayout.BeginVertical(subGroupStyle);
            {
                EditorGUI.indentLevel += 1;
                debugInfo.currentMove = EditorGUILayout.Toggle("Move Info", debugInfo.currentMove);
                debugInfo.position = EditorGUILayout.Toggle("Position", debugInfo.position);
                debugInfo.lifePoints = EditorGUILayout.Toggle("Life Points", debugInfo.lifePoints);
                debugInfo.currentState = EditorGUILayout.Toggle("State", debugInfo.currentState);
                debugInfo.currentSubState = EditorGUILayout.Toggle("SubState", debugInfo.currentSubState);
                debugInfo.stunTime = EditorGUILayout.Toggle("Stun Time", debugInfo.stunTime);
                debugInfo.comboHits = EditorGUILayout.Toggle("Combo Hits", debugInfo.comboHits);
                debugInfo.comboDamage = EditorGUILayout.Toggle("Combo Damage", debugInfo.comboDamage);
                debugInfo.inputs = EditorGUILayout.Toggle("Input Held Time", debugInfo.inputs);
                debugInfo.buttonSequence = EditorGUILayout.Toggle("Move Execution (Console)", debugInfo.buttonSequence);
                
                EditorGUI.BeginDisabledGroup(!UFE.isAiAddonInstalled);{
                    debugInfo.aiWeightList = EditorGUILayout.Toggle("[Fuzzy A.I.] Weight List", debugInfo.aiWeightList);
                }EditorGUI.EndDisabledGroup();

                EditorGUI.indentLevel -= 1;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }
    }

	public void HitOptionBlock(string label, HitTypeOptions hit){
		HitOptionBlock(label, hit, false);
	}

	public void HitOptionBlock(string label, HitTypeOptions hit, bool disableFreezingTime){
		hit.editorToggle = EditorGUILayout.Foldout(hit.editorToggle, label, foldStyle);
		if (hit.editorToggle){
			EditorGUILayout.BeginVertical(subGroupStyle);{
				EditorGUILayout.Space();
				EditorGUI.indentLevel += 1;

				hit.hitParticle = (GameObject) EditorGUILayout.ObjectField("Particle Effect:", hit.hitParticle, typeof(UnityEngine.GameObject), true);
                hit.spawnPoint = (HitEffectSpawnPoint) EditorGUILayout.EnumPopup("Spawn Point:", hit.spawnPoint, enumStyle);
				hit.killTime = EditorGUILayout.FloatField("Effect Duration:", hit.killTime);
				hit.hitSound = (AudioClip) EditorGUILayout.ObjectField("Sound Effect:", hit.hitSound, typeof(UnityEngine.AudioClip), true);
				
				if (disableFreezingTime){
					EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField("Freezing Time (seconds):", "(Automatic)");
					EditorGUILayout.TextField("Animation Speed", "(Automatic)");
					EditorGUI.EndDisabledGroup();
				}else{
                    hit._freezingTime = EditorGUILayout.FloatField("Freezing Time (seconds):", (float)hit._freezingTime);
					hit._animationSpeed = EditorGUILayout.FloatField("Animation Speed (%):", (float)hit._animationSpeed);
				}

                hit.autoHitStop = EditorGUILayout.Toggle("Auto Hit Stop", hit.autoHitStop);
                if (!hit.autoHitStop) {
                    hit._hitStop = EditorGUILayout.FloatField("Hit Stop (seconds):", (float)hit._hitStop);
                } else {
                    hit._hitStop = hit._freezingTime;
                }

                hit.mirrorOn2PSide = EditorGUILayout.Toggle("Mirror on Right Side", hit.mirrorOn2PSide);
                hit.shakeCharacterOnHit = EditorGUILayout.Toggle("Shake Character On Hit", hit.shakeCharacterOnHit);
                if (hit.shakeCharacterOnHit) hit._shakeDensity = EditorGUILayout.FloatField("- Shake Density:", (float)hit._shakeDensity);
                hit.shakeCameraOnHit = EditorGUILayout.Toggle("Shake Camera On Hit", hit.shakeCameraOnHit);
				if (hit.shakeCameraOnHit) hit._shakeCameraDensity = EditorGUILayout.FloatField("- Shake Density:", (float)hit._shakeCameraDensity);

				EditorGUI.indentLevel -= 1;
				EditorGUILayout.Space();
				
			}EditorGUILayout.EndVertical();
		}
	}
	
	public void KnockdownOptionsBlock(string label, SubKnockdownOptions option){
		option.editorToggle = EditorGUILayout.Foldout(option.editorToggle, label, foldStyle);
		if (option.editorToggle){
			EditorGUILayout.BeginVertical(subGroupStyle);{
				EditorGUILayout.Space();
				EditorGUI.indentLevel += 1;
                EditorGUIUtility.labelWidth = 200;


                if (label == "Crumple Knockdown") {
                    option._knockedOutTime = EditorGUILayout.FloatField("Knockout Time:", (float)option._knockedOutTime);
					option._standUpTime = EditorGUILayout.FloatField("Stand Up Time:", (float)option._standUpTime);
					option.hideHitBoxes = EditorGUILayout.Toggle("Hide Hit Boxes", option.hideHitBoxes);
				}else{
					option._knockedOutTime = EditorGUILayout.FloatField("Knockout Time:", (float)option._knockedOutTime);
					option._standUpTime = EditorGUILayout.FloatField("Stand Up Time:", (float)option._standUpTime);
					option.hideHitBoxes = EditorGUILayout.Toggle("Knockdown Hit Boxes", option.hideHitBoxes);

					option.hasQuickStand = EditorGUILayout.Toggle("Allow Quick Stand", option.hasQuickStand);
					if (option.hasQuickStand){
						EditorGUILayout.BeginVertical(subGroupStyle);{
							EditorGUI.indentLevel += 1;
							for (int i = 0; i < option.quickStandButtons.Length; i ++){
								EditorGUILayout.Space();
								EditorGUILayout.BeginVertical(arrayElementStyle);{
									EditorGUILayout.Space();
									EditorGUILayout.BeginHorizontal();{
										option.quickStandButtons[i] = (ButtonPress)EditorGUILayout.EnumPopup("Button:", option.quickStandButtons[i], enumStyle);
										if (GUILayout.Button("", "PaneOptions")){
											PaneOptions<ButtonPress>(option.quickStandButtons, option.quickStandButtons[i], delegate (ButtonPress[] newElement) { option.quickStandButtons = newElement; });
										}
									}EditorGUILayout.EndHorizontal();
									EditorGUILayout.Space();
								}EditorGUILayout.EndVertical();
							}
							EditorGUILayout.Space();
							if (StyledButton("New Button"))
								option.quickStandButtons = AddElement<ButtonPress>(option.quickStandButtons, ButtonPress.Button1);
							
							EditorGUI.indentLevel -= 1;
						}EditorGUILayout.EndVertical();
					}
					
					option.hasDelayedStand = EditorGUILayout.Toggle("Allow Delayed Stand", option.hasDelayedStand);
					if (option.hasDelayedStand){
						EditorGUILayout.BeginVertical(subGroupStyle);{
							EditorGUI.indentLevel += 1;
							for (int i = 0; i < option.delayedStandButtons.Length; i ++){
								EditorGUILayout.Space();
								EditorGUILayout.BeginVertical(arrayElementStyle);{
									EditorGUILayout.Space();
									EditorGUILayout.BeginHorizontal();{
										option.delayedStandButtons[i] = (ButtonPress)EditorGUILayout.EnumPopup("Button:", option.delayedStandButtons[i], enumStyle);
										if (GUILayout.Button("", "PaneOptions")){
											PaneOptions<ButtonPress>(option.delayedStandButtons, option.delayedStandButtons[i], delegate (ButtonPress[] newElement) { option.delayedStandButtons = newElement; });
										}
									}EditorGUILayout.EndHorizontal();
									EditorGUILayout.Space();
								}EditorGUILayout.EndVertical();
							}
							EditorGUILayout.Space();
							if (StyledButton("New Button"))
								option.delayedStandButtons = AddElement<ButtonPress>(option.delayedStandButtons, ButtonPress.Down);
							
							EditorGUI.indentLevel -= 1;
						}EditorGUILayout.EndVertical();
					}
					
					if (label != "Default Knockdown") option._predefinedPushForce = FPVector.ToFPVector(EditorGUILayout.Vector2Field("Predefined Push Force:", option._predefinedPushForce.ToVector2()));
                }
                EditorGUIUtility.labelWidth = 150;
                EditorGUI.indentLevel -= 1;
				EditorGUILayout.Space();
				
			}EditorGUILayout.EndVertical();
		}
	}

	public void EditCharacterStory(CharacterStory characterStory, string storyName = "Story"){
		EditorGUILayout.BeginVertical(subGroupStyle);{
			EditorGUILayout.Space();
			characterStory.showStoryInEditor = EditorGUILayout.Foldout(characterStory.showStoryInEditor, storyName, foldStyle);

			if (characterStory.showStoryInEditor){
                EditorGUI.indentLevel += 1;
                EditorGUIUtility.labelWidth = 130;
				EditorGUILayout.BeginHorizontal();{
					characterStory.opening = (StoryModeScreen) EditorGUILayout.ObjectField("Opening Scene:", characterStory.opening, typeof(StoryModeScreen), true);
					EditorGUI.BeginDisabledGroup(DisableScreenButton(characterStory.opening) || !guiScreensOptions);{
						ScreenButton(characterStory.opening);
					} EditorGUI.EndDisabledGroup();
				} EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();{
					characterStory.ending = (StoryModeScreen) EditorGUILayout.ObjectField("Ending Scene:", characterStory.ending, typeof(StoryModeScreen), true);
					EditorGUI.BeginDisabledGroup(DisableScreenButton(characterStory.ending) || !guiScreensOptions);{
						ScreenButton(characterStory.ending);
					} EditorGUI.EndDisabledGroup();
				} EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();

                SubGroupTitle("Fight Groups");

				this.FightGroupsBlock(characterStory.fightsGroups, delegate(FightsGroup[] obj) {
					int previousLength = characterStory.fightsGroups.Length;
					int currentLength = obj.Length;


					characterStory.fightsGroups = obj;


					if (previousLength == 0 && currentLength == 1){
						characterStory.fightsGroups[0].name = "Random Fights";
						characterStory.fightsGroups[0].mode = FightsGroupMode.FightAgainstAllOpponentsInTheGroupInRandomOrder;
					}else if (previousLength == 1 && currentLength == 2){
						characterStory.fightsGroups[1].name = "Boss Fights";
						characterStory.fightsGroups[1].mode = FightsGroupMode.FightAgainstAllOpponentsInTheGroupInTheDefinedOrder;
					}else if (currentLength == previousLength + 1){
						characterStory.fightsGroups[previousLength].name = "Group " + currentLength;
						characterStory.fightsGroups[previousLength].mode = FightsGroupMode.FightAgainstAllOpponentsInTheGroupInTheDefinedOrder;
					}
				});

                EditorGUIUtility.labelWidth = 150;
				EditorGUI.indentLevel -= 1;
			}
			EditorGUILayout.Space();
		}EditorGUILayout.EndVertical();
		EditorGUILayout.Space();
	}

	public void FightGroupsBlock(FightsGroup[] fights, Action<FightsGroup[]> callback){
		List<string> characterNames = new List<string>();
		for (int i = 0; i < globalInfo.characters.Length; ++i){
			if (globalInfo.characters[i] != null){
				characterNames.Add(globalInfo.characters[i].name);
			}else{
				characterNames.Add ("Character " + i);
			}
		}

		EditorGUILayout.BeginVertical();{
			//EditorGUI.indentLevel += 1;
			for (int i = 0; i < fights.Length; ++i){
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical(arrayElementStyle);{
					EditorGUILayout.Space();

                    EditorGUIUtility.labelWidth = 130;
					EditorGUILayout.BeginHorizontal();{
						fights[i].name = EditorGUILayout.TextField("Group Name:", fights[i].name);

						if (GUILayout.Button("", "PaneOptions")){
							PaneOptions<FightsGroup>(fights, fights[i], delegate (FightsGroup[] newElement) { if (callback != null)callback(newElement); });
							return;
						}
					}EditorGUILayout.EndHorizontal();


					fights[i].mode = (FightsGroupMode) EditorGUILayout.EnumPopup("Fight Mode:", fights[i].mode);

                    EditorGUIUtility.labelWidth = 150;

					if (fights[i].mode == FightsGroupMode.FightAgainstSeveralOpponentsInTheGroupInRandomOrder){
						fights[i].maxFights = EditorGUILayout.IntSlider("How many opponents?", fights[i].maxFights, 1, Mathf.Max(fights[i].opponents.Length, 1));
					}

					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						fights[i].showOpponentsInEditor = EditorGUILayout.Foldout(fights[i].showOpponentsInEditor, "Opponents", foldStyle);

						if (fights[i].showOpponentsInEditor){
							this.StoryModeBattleBlock(fights, i, characterNames.ToArray(), delegate(FightsGroup[] obj) {
								if (callback != null) callback(obj);
							});
						}
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
				}EditorGUILayout.EndVertical();
			}
			//EditorGUI.indentLevel -= 1;
			
			if (StyledButton("New Group")){
				fights = AddElement<FightsGroup>(fights, new FightsGroup());
			}
		}EditorGUILayout.EndVertical();

		if (callback != null) callback(fights);
	}

	public void StoryModeBattleBlock(FightsGroup[] fights, int fightIndex, string[] characterNames, Action<FightsGroup[]> callback){
		this.StoryModeBattleBlock(fights[fightIndex].opponents, characterNames, delegate(StoryModeBattle[] obj) {
			int previousLength = fights[fightIndex].opponents.Length;
			int currentLength = obj.Length;
			
			
			fights[fightIndex].opponents = obj;
			
			
			if (currentLength == previousLength + 1){
				fights[fightIndex].opponents[previousLength].opponentCharacterIndex = (previousLength % globalInfo.characters.Length);
				fights[fightIndex].opponents[previousLength].possibleStagesIndexes.Add(previousLength % globalInfo.stages.Length);
			}
			

			if (callback != null) callback(fights);
		});
	}
	
	public void StoryModeBattleBlock(StoryModeBattle[] battles, string[] characterNames, Action<StoryModeBattle[]> callback){
		EditorGUILayout.BeginVertical();{
			EditorGUI.indentLevel += 1;
			for (int i = 0; i < battles.Length; ++i){
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical(subArrayElementStyle);{
					EditorGUILayout.Space();
					EditorGUILayout.BeginHorizontal();{
						battles[i].opponentCharacterIndex = EditorGUILayout.Popup("Opponent:", battles[i].opponentCharacterIndex, characterNames);

						if (GUILayout.Button("", "PaneOptions")){
							PaneOptions<StoryModeBattle>(battles, battles[i], delegate (StoryModeBattle[] newElement) { if (callback != null)callback(newElement); });
							return;
						}
					}EditorGUILayout.EndHorizontal();

					string[] stageNames = new string[globalInfo.stages.Length];
					int stageFlags = 0;
					
					for (int j = 0; j < globalInfo.stages.Length; ++j){
						if (globalInfo.stages[j] != null && !string.IsNullOrEmpty(globalInfo.stages[j].stageName)){
							stageNames[j] = globalInfo.stages[j].stageName;
						}else{
							stageNames[j] = "Stage " + j;
						}

						if (battles[i].possibleStagesIndexes.Contains(j)){
							stageFlags |= 1<<j;
						}
					}
					
					stageFlags = EditorGUILayout.MaskField("Possible Stages:", stageFlags, stageNames);
					battles[i].possibleStagesIndexes.Clear();
					
					for (int j = 0; j < globalInfo.stages.Length; ++j){
						
						if ((stageFlags & (1<<j)) != 0){
							battles[i].possibleStagesIndexes.Add(j);
						}
					}

					EditorGUILayout.BeginHorizontal();{
						battles[i].conversationBeforeBattle = (StoryModeScreen) EditorGUILayout.ObjectField("Before the battle:", battles[i].conversationBeforeBattle, typeof(StoryModeScreen), true);
						EditorGUI.BeginDisabledGroup(DisableScreenButton(battles[i].conversationBeforeBattle) || !guiScreensOptions);{
							ScreenButton(battles[i].conversationBeforeBattle);
						} EditorGUI.EndDisabledGroup();
					} EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();{
						battles[i].conversationAfterBattle = (StoryModeScreen) EditorGUILayout.ObjectField("After the battle:", battles[i].conversationAfterBattle, typeof(StoryModeScreen), true);
						EditorGUI.BeginDisabledGroup(DisableScreenButton(battles[i].conversationAfterBattle) || !guiScreensOptions);{
							ScreenButton(battles[i].conversationAfterBattle);
						} EditorGUI.EndDisabledGroup();
					} EditorGUILayout.EndHorizontal();

//					battles[i].conversationBeforeBattle = (StoryModeScreen) EditorGUILayout.ObjectField("Before the battle:", battles[i].conversationBeforeBattle, typeof(StoryModeScreen), true);
//					battles[i].conversationAfterBattle = (StoryModeScreen) EditorGUILayout.ObjectField("After the battle:", battles[i].conversationAfterBattle, typeof(StoryModeScreen), true);

					EditorGUILayout.Space();
				}EditorGUILayout.EndVertical();
			}
			EditorGUI.indentLevel -= 1;
			
			if (StyledButton("New Opponent")){
				battles = AddElement<StoryModeBattle>(battles, new StoryModeBattle());
			}
		}EditorGUILayout.EndVertical();
		
		if (callback != null)callback(battles);
	}

	public InputReferences[] PlayerInputsBlock(InputReferences[] inputReferences){
		bool controlFreakInstalled = UFE.isControlFreakInstalled;
		bool cInputInstalled = UFE.isCInputInstalled;
		
		EditorGUIUtility.labelWidth = 180;
		EditorGUILayout.BeginVertical(subGroupStyle);{
			for (int i = 0; i < inputReferences.Length; i ++){
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical(arrayElementStyle);{
					EditorGUILayout.Space();
					EditorGUILayout.BeginHorizontal();{
						inputReferences[i].inputType = (InputType) EditorGUILayout.EnumPopup("Input Type:", inputReferences[i].inputType, enumStyle);
						if (GUILayout.Button("", "PaneOptions")){
							if (inputReferences.Equals(globalInfo.player1_Inputs)){
								PaneOptions<InputReferences>(globalInfo.player1_Inputs, globalInfo.player1_Inputs[i], delegate (InputReferences[] newElement) { globalInfo.player1_Inputs = newElement; });
							}else{
								PaneOptions<InputReferences>(globalInfo.player2_Inputs, globalInfo.player2_Inputs[i], delegate (InputReferences[] newElement) { globalInfo.player2_Inputs = newElement; });
							}
							//PaneOptions<InputReferences>(inputReferences, inputReferences[i], delegate (InputReferences[] newElement) { inputReferences = newElement; });
						}
					}EditorGUILayout.EndHorizontal();

					if (inputReferences[i].inputType == InputType.Button){
						inputReferences[i].engineRelatedButton = (ButtonPress) EditorGUILayout.EnumPopup("UFE Button Reference:", inputReferences[i].engineRelatedButton, enumStyle);
					}

					if (cInputInstalled && globalInfo.inputOptions.inputManagerType == InputManagerType.cInput){
						if (inputReferences[i].inputType == InputType.Button){
							inputReferences[i].inputButtonName = EditorGUILayout.TextField("cInput Button Name:", inputReferences[i].inputButtonName);
							inputReferences[i].cInputPositiveDefaultKey = EditorGUILayout.TextField("cInput Default Key:", inputReferences[i].cInputPositiveDefaultKey);
							inputReferences[i].cInputPositiveAlternativeKey = EditorGUILayout.TextField("cInput Alternative Key:", inputReferences[i].cInputPositiveAlternativeKey);
						}else{
							inputReferences[i].inputButtonName = EditorGUILayout.TextField("cInput Axis Name:", inputReferences[i].inputButtonName);
							EditorGUILayout.Space();
							EditorGUILayout.LabelField("cInput:");

							inputReferences[i].cInputPositiveKeyName = EditorGUILayout.TextField("Positive Value -> Button:", inputReferences[i].cInputPositiveKeyName);
							inputReferences[i].cInputPositiveDefaultKey = EditorGUILayout.TextField("Positive Value -> Default Key:", inputReferences[i].cInputPositiveDefaultKey);
							inputReferences[i].cInputPositiveAlternativeKey = EditorGUILayout.TextField("Positive Value -> Alt Key:", inputReferences[i].cInputPositiveAlternativeKey);
							EditorGUILayout.Space();
							inputReferences[i].cInputNegativeKeyName = EditorGUILayout.TextField("Negative Value -> Button:", inputReferences[i].cInputNegativeKeyName);
							inputReferences[i].cInputNegativeDefaultKey = EditorGUILayout.TextField("Negative Value -> Default Key:", inputReferences[i].cInputNegativeDefaultKey);
							inputReferences[i].cInputNegativeAlternativeKey = EditorGUILayout.TextField("Negative Value -> Alt Key:", inputReferences[i].cInputNegativeAlternativeKey);
						}
					}else if (controlFreakInstalled && globalInfo.inputOptions.inputManagerType == InputManagerType.ControlFreak){
						if (inputReferences[i].inputType == InputType.Button){
							inputReferences[i].inputButtonName = EditorGUILayout.TextField("CF Button Name:", inputReferences[i].inputButtonName);
						}else{
							inputReferences[i].inputButtonName = EditorGUILayout.TextField("CF Axis Name:", inputReferences[i].inputButtonName);
						}
					}else{
						if (inputReferences[i].inputType == InputType.Button){
							inputReferences[i].inputButtonName = EditorGUILayout.TextField("Input Manager Reference:", inputReferences[i].inputButtonName);
						}else{
							inputReferences[i].inputButtonName = EditorGUILayout.TextField("Keyboard Axis Reference:", inputReferences[i].inputButtonName);
							inputReferences[i].joystickAxisName = EditorGUILayout.TextField("Joystick Axis Reference:", inputReferences[i].joystickAxisName);
						}
					}
					
					if (inputReferences[i].engineRelatedButton != ButtonPress.Start){
						string label1 = null;
						string label2 = null;
						if (inputReferences[i].inputType == InputType.Button){
							label1 = "Button Icon:";
						}else if (inputReferences[i].inputType == InputType.HorizontalAxis){
							label1 = "Axis Right Icon:";
							label2 = "Axis Left Icon:";
						}else if (inputReferences[i].inputType == InputType.VerticalAxis){
							label1 = "Axis Up Icon:";
							label2 = "Axis Down Icon:";
						}

						EditorGUILayout.BeginHorizontal();{
							EditorGUILayout.LabelField(label1, GUILayout.Width(160));
							inputReferences[i].inputViewerIcon1 = (Texture2D)EditorGUILayout.ObjectField(inputReferences[i].inputViewerIcon1, typeof(Texture2D), true);
							inputReferences[i].activeIcon = inputReferences[i].inputViewerIcon1;
						}EditorGUILayout.EndHorizontal();

						if (label2 != null){
							EditorGUILayout.BeginHorizontal();{
								EditorGUILayout.LabelField(label2, GUILayout.Width(160));
								inputReferences[i].inputViewerIcon2 = (Texture2D)EditorGUILayout.ObjectField(inputReferences[i].inputViewerIcon2, typeof(Texture2D), true);
							}EditorGUILayout.EndHorizontal();
						}
					}

					EditorGUILayout.Space();
				}EditorGUILayout.EndVertical();
			}
			
			if (StyledButton("New Input"))
				inputReferences = AddElement<InputReferences>(inputReferences, new InputReferences());

		}EditorGUILayout.EndVertical();
		EditorGUILayout.Space();
		EditorGUIUtility.labelWidth = 150;

		return inputReferences;
	}


	private Vector2 ReturnRange(CharacterDistance distance, int size){
		if (distance == CharacterDistance.Any) return new Vector2(0, 100);
		if (size == 2){
			if ((int)distance < (int)CharacterDistance.Mid) return new Vector2(0, 50);
			if ((int)distance >= (int)CharacterDistance.Mid) return new Vector2(51, 100);
		}else if (size == 3){
			if ((int)distance < (int)CharacterDistance.Mid) return new Vector2(0, 30);
			if (distance == CharacterDistance.Mid) return new Vector2(31, 70);
			if ((int)distance > (int)CharacterDistance.Mid) return new Vector2(71, 100);
		}else if (size == 4){
			if ((int)distance == (int)CharacterDistance.VeryClose) return new Vector2(0, 25);
			if ((int)distance <= (int)CharacterDistance.Mid) return new Vector2(26, 50);
			if ((int)distance == (int)CharacterDistance.Far) return new Vector2(51, 75);
			if ((int)distance == (int)CharacterDistance.VeryFar) return new Vector2(76, 100);
		}else if (size > 4){
			if ((int)distance == (int)CharacterDistance.VeryClose) return new Vector2(0, 20);
			if ((int)distance == (int)CharacterDistance.Close) return new Vector2(21, 40);
			if ((int)distance == (int)CharacterDistance.Mid) return new Vector2(41, 60);
			if ((int)distance == (int)CharacterDistance.VeryFar) return new Vector2(61, 80);
			if ((int)distance == (int)CharacterDistance.VeryFar) return new Vector2(81, 100);
		}
		
		
		return Vector2.zero;
	}

	private string DistanceToString(int index, int size){
		if (size == 2){
			if (index == 0) return "Close";
			if (index == 1) return "Far";
		}else if (size == 3){
			if (index == 0) return "Close";
			if (index == 1) return "Mid";
			if (index == 2) return "Far";
		}else if (size == 4){
			if (index == 0) return "Very Close";
			if (index == 1) return "Close";
			if (index == 2) return "Mid";
			if (index == 3) return "Far";
		}else if (size > 4){
			float position = (float)(index + 1)/(float)size;
			if (position <= .2f) return "Very Close";
			if (position <= .4f) return "Close";
			if (position <= .6f) return "Mid";
			if (position <= .8f) return "Far";
			if (position > .8f) return "Very Far";
		}


		return "Any";
	}

	private void DisableableSlider(string description, ref bool toggle, ref float slider, float minValue, float maxValue){
		EditorGUILayout.BeginHorizontal();{
			toggle = EditorGUILayout.Toggle(toggle, GUILayout.Width(40));
			EditorGUI.BeginDisabledGroup(!toggle);{
				slider = EditorGUILayout.Slider(description, slider, minValue, maxValue);
			}EditorGUI.EndDisabledGroup();
		}EditorGUILayout.EndHorizontal();
	}

	private void SubGroupTitle(string _name){
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(_name);
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
	}

	public void CInputPreferences(){
		EditorGUILayout.BeginVertical(subGroupStyle);{
			EditorGUILayout.BeginVertical(arrayElementStyle);{
				EditorGUI.indentLevel += 1;
				EditorGUILayout.Space();
				globalInfo.inputOptions.cInputAllowDuplicates = EditorGUILayout.Toggle("Allow Duplicates:", globalInfo.inputOptions.cInputAllowDuplicates);
				globalInfo.inputOptions.cInputGravity = EditorGUILayout.FloatField("Gravity:", globalInfo.inputOptions.cInputGravity);
				globalInfo.inputOptions.cInputSensitivity = EditorGUILayout.FloatField("Sensibility:", globalInfo.inputOptions.cInputSensitivity);
				globalInfo.inputOptions.cInputDeadZone = EditorGUILayout.FloatField("Dead Zone:", globalInfo.inputOptions.cInputDeadZone);
				globalInfo.inputOptions.cInputSkin = EditorGUILayout.ObjectField("Skin:", globalInfo.inputOptions.cInputSkin, typeof(GUISkin), false) as GUISkin;
				EditorGUILayout.Space();
				EditorGUI.indentLevel -= 1;
			}EditorGUILayout.EndVertical();
		}EditorGUILayout.EndVertical();
	}

	public void ControlFreakPreferences(){
		EditorGUILayout.BeginVertical(subGroupStyle);{
			EditorGUILayout.BeginVertical(arrayElementStyle);{
				EditorGUI.indentLevel += 1;
				EditorGUILayout.Space();

                if (UFE.isControlFreak2Installed) {
                    globalInfo.inputOptions.controlFreak2Prefab = EditorGUILayout.ObjectField(new GUIContent("CF2 Prefab:",
                        "Prefab of Control Freak 2 Input Rig with \'UFE Bridge\' component."),
                        globalInfo.inputOptions.controlFreak2Prefab, typeof(InputTouchControllerBridge), false) as InputTouchControllerBridge;
                }

                if (UFE.isControlFreak1Installed) {
                    if (UFE.isControlFreak2Installed && (globalInfo.inputOptions.controlFreakPrefab != null))
                        EditorGUILayout.HelpBox("Please, switch to Control Freak 2.", MessageType.Info);

                    globalInfo.inputOptions.controlFreakPrefab = EditorGUILayout.ObjectField("CF1 Prefab:",
                        globalInfo.inputOptions.controlFreakPrefab, typeof(GameObject), false) as GameObject;

                    globalInfo.inputOptions.controlFreakDeadZone = EditorGUILayout.FloatField("Dead Zone:", globalInfo.inputOptions.controlFreakDeadZone);
                }

				EditorGUILayout.Space();
				EditorGUI.indentLevel -= 1;
			}EditorGUILayout.EndVertical();
		}EditorGUILayout.EndVertical();
	}
	

	public void StyledMinMaxSlider (string label, ref int minValue, ref int maxValue, int minLimit, int maxLimit, int indentLevel) {
		int indentSpacing = 25 * indentLevel;
		//indentSpacing += 30;
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		if (minValue < 0) minValue = 0;
		if (maxValue < 2) maxValue = 2;
		if (maxValue > maxLimit) maxValue = maxLimit;
		if (minValue == maxValue) minValue --;
		float minValueFloat = (float) minValue;
		float maxValueFloat = (float) maxValue;
		float minLimitFloat = (float) minLimit;
		float maxLimitFloat = (float) maxLimit;
		
		Rect tempRect = GUILayoutUtility.GetRect(1, 10);

        Rect rect = new Rect(indentSpacing, tempRect.y, position.width - indentSpacing - 100, 20);
        //Rect rect = new Rect(indentSpacing + 15,tempRect.y, position.width - indentSpacing - 70, 20);
		float fillLeftPos = ((rect.width/maxLimitFloat) * minValueFloat) + rect.x;
		float fillRightPos = ((rect.width/maxLimitFloat) * maxValueFloat) + rect.x;
		float fillWidth = fillRightPos - fillLeftPos;
		
		fillWidth += (rect.width/maxLimitFloat);
		fillLeftPos -= (rect.width/maxLimitFloat);
		
		// Border
		GUI.Box(rect, "", borderBarStyle);
		
		// Overlay
		GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle1);
		
		// Text
		//GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
		//centeredStyle.alignment = TextAnchor.UpperCenter;
		labelStyle.alignment = TextAnchor.UpperCenter;
		GUI.Label(rect, label + " between "+ Mathf.Floor(minValueFloat)+" and "+Mathf.Floor(maxValueFloat), labelStyle);
		labelStyle.alignment = TextAnchor.MiddleCenter;
		
		// Slider
		rect.y += 10;
		rect.x = indentLevel * 10;
        rect.width = (position.width - (indentLevel * 10) - 100);
		
		EditorGUI.MinMaxSlider(rect, ref minValueFloat, ref maxValueFloat, minLimitFloat, maxLimitFloat);
		minValue = (int) minValueFloat;
		maxValue = (int) maxValueFloat;
		
		tempRect = GUILayoutUtility.GetRect(1, 20);
	}

	public void PaneOptions<T> (T[] elements, T element, System.Action<T[]> callback){
		if (elements == null || elements.Length == 0) return;
		GenericMenu toolsMenu = new GenericMenu();
		
		if ((elements[0] != null && elements[0].Equals(element)) || (elements[0] == null && element == null) || elements.Length == 1){
			toolsMenu.AddDisabledItem(new GUIContent("Move Up"));
			toolsMenu.AddDisabledItem(new GUIContent("Move To Top"));
		}else {
			toolsMenu.AddItem(new GUIContent("Move Up"), false, delegate() {callback(MoveElement<T>(elements, element, -1));});
			toolsMenu.AddItem(new GUIContent("Move To Top"), false, delegate() {callback(MoveElement<T>(elements, element, -elements.Length));});
		}
		if ((elements[elements.Length - 1] != null && elements[elements.Length - 1].Equals(element)) || elements.Length == 1){
			toolsMenu.AddDisabledItem(new GUIContent("Move Down"));
			toolsMenu.AddDisabledItem(new GUIContent("Move To Bottom"));
		}else{
			toolsMenu.AddItem(new GUIContent("Move Down"), false, delegate() {callback(MoveElement<T>(elements, element, 1));});
			toolsMenu.AddItem(new GUIContent("Move To Bottom"), false, delegate() {callback(MoveElement<T>(elements, element, elements.Length));});
		}
		
		toolsMenu.AddSeparator("");
		
		if (element != null && element is System.ICloneable){
			toolsMenu.AddItem(new GUIContent("Copy"), false, delegate() {callback(CopyElement<T>(elements, element));});
		}else{
			toolsMenu.AddDisabledItem(new GUIContent("Copy"));
		}
		
		if (element != null && CloneObject.objCopy != null && CloneObject.objCopy.GetType() == typeof(T)){
			toolsMenu.AddItem(new GUIContent("Paste"), false, delegate() {callback(PasteElement<T>(elements, element));});
		}else{
			toolsMenu.AddDisabledItem(new GUIContent("Paste"));
		}
		
		toolsMenu.AddSeparator("");
		
		if (!(element is System.ICloneable)){
			toolsMenu.AddDisabledItem(new GUIContent("Duplicate"));
		}else{
			toolsMenu.AddItem(new GUIContent("Duplicate"), false, delegate() {callback(DuplicateElement<T>(elements, element));});
		}
		toolsMenu.AddItem(new GUIContent("Remove"), false, delegate() {callback(RemoveElement<T>(elements, element));});
		
		toolsMenu.ShowAsContext();
		EditorGUIUtility.ExitGUI();
	}
	
	public T[] RemoveElement<T> (T[] elements, T element) {
		List<T> elementsList = new List<T>(elements);
		elementsList.Remove(element);
		return elementsList.ToArray();
	}
	
	public T[] AddElement<T> (T[] elements, T element) {
		List<T> elementsList = new List<T>(elements);
		elementsList.Add(element);
		return elementsList.ToArray();
	}
	
	public T[] CopyElement<T> (T[] elements, T element) {
		CloneObject.objCopy = (object)(element as ICloneable).Clone();
		return elements;
	}
	
	public T[] PasteElement<T> (T[] elements, T element) {
		if (CloneObject.objCopy == null) return elements;
		List<T> elementsList = new List<T>(elements);
		elementsList.Insert(elementsList.IndexOf(element) + 1, (T)CloneObject.objCopy);
		CloneObject.objCopy = null;
		return elementsList.ToArray();
	}
	
	public T[] DuplicateElement<T> (T[] elements, T element) {
		List<T> elementsList = new List<T>(elements);
		elementsList.Insert(elementsList.IndexOf(element) + 1, (T)(element as ICloneable).Clone());
		return elementsList.ToArray();
	}
	
	public T[] MoveElement<T> (T[] elements, T element, int steps) {
		List<T> elementsList = new List<T>(elements);
		int newIndex = Mathf.Clamp(elementsList.IndexOf(element) + steps, 0, elements.Length - 1);
		elementsList.Remove(element);
		elementsList.Insert(newIndex, element);
		return elementsList.ToArray();
	}
}