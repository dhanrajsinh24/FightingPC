#if UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_PRE_5_0
#endif

#if UNITY_PRE_5_0 || UNITY_5_0
#define UNITY_PRE_5_1
#endif

#if UNITY_PRE_5_1 || UNITY_5_1
#define UNITY_PRE_5_2
#endif

#if UNITY_PRE_5_2 || UNITY_5_2
#define UNITY_PRE_5_3
#endif

#if UNITY_PRE_5_3 || UNITY_5_3
#define UNITY_PRE_5_4
#endif


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using FPLibrary;
using UFENetcode;
using UFE3D;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EventSystem))]
public class UFE : MonoBehaviour, UFEInterface
{
	#region public instance enum
	public enum MultiplayerMode{
		Lan,
		Online,
		Bluetooth,
	}
	#endregion

	#region public instance properties
    public GlobalInfo UFE_Config;
	#endregion

	#region public event definitions
	public delegate void MeterHandler(float newFloat, UFE3D.CharacterInfo player);
	public static event MeterHandler OnLifePointsChange;

	public delegate void IntHandler(int newInt);
	public static event IntHandler OnRoundBegins;

	public delegate void StringHandler(string newString, UFE3D.CharacterInfo player);
	public static event StringHandler OnNewAlert;
	
	public delegate void HitHandler(HitBox strokeHitBox, MoveInfo move, UFE3D.CharacterInfo player);
    public static event HitHandler OnHit;
    public static event HitHandler OnBlock;
    public static event HitHandler OnParry;

	public delegate void MoveHandler(MoveInfo move, UFE3D.CharacterInfo player);
	public static event MoveHandler OnMove;

    public delegate void ButtonHandler(ButtonPress button, UFE3D.CharacterInfo player);
    public static event ButtonHandler OnButton;

    public delegate void BasicMoveHandler(BasicMoveReference basicMove, UFE3D.CharacterInfo player);
    public static event BasicMoveHandler OnBasicMove;

    public delegate void BodyVisibilityHandler(MoveInfo move, UFE3D.CharacterInfo player, BodyPartVisibilityChange bodyPartVisibilityChange, HitBox hitBox);
    public static event BodyVisibilityHandler OnBodyVisibilityChange;

    public delegate void ParticleEffectsHandler(MoveInfo move, UFE3D.CharacterInfo player, MoveParticleEffect particleEffects);
    public static event ParticleEffectsHandler OnParticleEffects;

    public delegate void SideSwitchHandler(int side, UFE3D.CharacterInfo player);
    public static event SideSwitchHandler OnSideSwitch;

	public delegate void GameBeginHandler(UFE3D.CharacterInfo player1, UFE3D.CharacterInfo player2, StageOptions stage);
	public static event GameBeginHandler OnGameBegin;

	public delegate void GameEndsHandler(UFE3D.CharacterInfo winner, UFE3D.CharacterInfo loser);
	public static event GameEndsHandler OnGameEnds;
	public static event GameEndsHandler OnRoundEnds;

	public delegate void GamePausedHandler(bool isPaused);
	public static event GamePausedHandler OnGamePaused;

	public delegate void ScreenChangedHandler(UFEScreen previousScreen, UFEScreen newScreen);
	public static event ScreenChangedHandler OnScreenChanged;

	public delegate void StoryModeHandler(UFE3D.CharacterInfo character);
	public static event StoryModeHandler OnStoryModeStarted;
	public static event StoryModeHandler OnStoryModeCompleted;

	public delegate void TimerHandler(Fix64 time);
	public static event TimerHandler OnTimer;

	public delegate void TimeOverHandler();
	public static event TimeOverHandler OnTimeOver;

	public delegate void InputHandler(InputReferences[] inputReferences, int player);
	public static event InputHandler OnInput;
	#endregion

	#region network definitions
    //-----------------------------------------------------------------------------------------------------------------
	// Network
	//-----------------------------------------------------------------------------------------------------------------
    public static MultiplayerAPI multiplayerAPI{
		get{
			if (multiplayerMode == MultiplayerMode.Bluetooth){
				return bluetoothMultiplayerAPI;
			}else if (multiplayerMode == MultiplayerMode.Lan){
				return lanMultiplayerAPI;
			}else{
				return onlineMultiplayerAPI;
			}
		}
	}

	public static MultiplayerMode multiplayerMode{
		get{
			return _multiplayerMode;
		}
		set{
			_multiplayerMode = value;

			if (value == MultiplayerMode.Bluetooth){
				bluetoothMultiplayerAPI.enabled = true;
				lanMultiplayerAPI.enabled = false;
				onlineMultiplayerAPI.enabled = false;
			}else if (value == MultiplayerMode.Lan){
				bluetoothMultiplayerAPI.enabled = false;
				lanMultiplayerAPI.enabled = true;
				onlineMultiplayerAPI.enabled = false;
			}else{
				bluetoothMultiplayerAPI.enabled = false;
				lanMultiplayerAPI.enabled = false;
				onlineMultiplayerAPI.enabled = true;
			}
		}
	}

	private static MultiplayerAPI bluetoothMultiplayerAPI;
	private static MultiplayerAPI lanMultiplayerAPI;
	private static MultiplayerAPI onlineMultiplayerAPI;

	private static MultiplayerMode _multiplayerMode = MultiplayerMode.Lan;
    #endregion
    
	#region gui definitions
    //-----------------------------------------------------------------------------------------------------------------
	// GUI
	//-----------------------------------------------------------------------------------------------------------------
	public static Canvas canvas{get; protected set;}
	public static CanvasGroup canvasGroup{get; protected set;}
	public static EventSystem eventSystem{get; protected set;}
	public static GraphicRaycaster graphicRaycaster{get; protected set;}
	public static StandaloneInputModule standaloneInputModule{get; protected set;}
	//-----------------------------------------------------------------------------------------------------------------
	protected static readonly string MusicEnabledKey = "MusicEnabled";
	protected static readonly string MusicVolumeKey = "MusicVolume";
	protected static readonly string SoundsEnabledKey = "SoundsEnabled";
	protected static readonly string SoundsVolumeKey = "SoundsVolume";
	protected static readonly string DifficultyLevelKey = "DifficultyLevel";
	protected static readonly string DebugModeKey = "DebugMode";
	//-----------------------------------------------------------------------------------------------------------------

	public static CameraScript cameraScript{get; set;}
    public static FluxCapacitor fluxCapacitor;
	public static GameMode gameMode = GameMode.None;
	public static GlobalInfo config;
	public static UFE UFEInstance;
	public static bool debug = true;
	public static Text debugger1;
	public static Text debugger2;
    #endregion

    #region addons definitions
    public static bool isAiAddonInstalled {get; set;}
    public static bool isCInputInstalled { get; set; }
    public static bool isControlFreakInstalled { get; set; }
    public static bool isControlFreak1Installed { get; set; }
    public static bool isControlFreak2Installed { get; set; }
    public static bool isRewiredInstalled { get; set; }
    public static bool isNetworkAddonInstalled {get; set; }
    public static bool isPhotonInstalled { get; set; }
    public static bool isBluetoothAddonInstalled { get; set; }
    public static GameObject controlFreakPrefab;
    public static InputTouchControllerBridge touchControllerBridge;
    #endregion
    
    #region screen definitions
    public static UFEScreen currentScreen{get; protected set;}
	public static UFEScreen battleGUI{get; protected set;}
	public static GameObject gameEngine{get; protected set;}
    #endregion

    #region trackable definitions
    public static bool freeCamera;
    public static bool freezePhysics;
    public static bool newRoundCasted;
    public static bool normalizedCam = true;
    public static bool pauseTimer;
    public static Fix64 timer;
    public static Fix64 timeScale;
    public static ControlsScript p1ControlsScript;
    public static ControlsScript p2ControlsScript;
    public static List<DelayedAction> delayedLocalActions = new List<DelayedAction>();
    public static List<DelayedAction> delayedSynchronizedActions = new List<DelayedAction>();
    public static List<InstantiatedGameObject> instantiatedObjects = new List<InstantiatedGameObject>();
    #endregion

    #region story mode definitions
    //-----------------------------------------------------------------------------------------------------------------
    // Required for the Story Mode: if the player lost its previous battle, 
    // he needs to fight the same opponent again, not the next opponent.
    //-----------------------------------------------------------------------------------------------------------------
    private static StoryModeInfo storyMode = new StoryModeInfo();
    private static List<string> unlockedCharactersInStoryMode = new List<string>();
    private static List<string> unlockedCharactersInVersusMode = new List<string>();
    private static bool player1WonLastBattle;
    private static int lastStageIndex;
    #endregion

    #region public definitions
    public static Fix64 fixedDeltaTime { get { return _fixedDeltaTime * timeScale; } set { _fixedDeltaTime = value; } }
    public static int intTimer;
    public static int fps = 60;
    public static long currentFrame { get; set; }
    public static bool gameRunning { get; protected set; }

    public static UFEController localPlayerController;
    public static UFEController remotePlayerController;
    #endregion

    #region private definitions
    private static Fix64 _fixedDeltaTime;
    private static AudioSource musicAudioSource;
	private static AudioSource soundsAudioSource;

	private static UFEController p1Controller;
	private static UFEController p2Controller;

	private static RandomAI p1RandomAI;
	private static RandomAI p2RandomAI;
	private static AbstractInputController p1FuzzyAI;
	private static AbstractInputController p2FuzzyAI;
	private static SimpleAI p1SimpleAI;
	private static SimpleAI p2SimpleAI;
    
    private static bool closing = false;
	private static bool disconnecting = false;
    private static List<object> memoryDump = new List<object>();
    #endregion


    #region public class methods: Delay the execution of a method maintaining synchronization between clients
    public static void DelayLocalAction(Action action, Fix64 seconds) {
        if (fixedDeltaTime > 0) {
            DelayLocalAction(action, (int)FPMath.Floor(seconds * config.fps));
		}else{
			DelayLocalAction(action, 1);
		}
	}

	public static void DelayLocalAction(Action action, int steps){
		DelayLocalAction(new DelayedAction(action, steps));
	}

	public static void DelayLocalAction(DelayedAction delayedAction){
		delayedLocalActions.Add(delayedAction);
	}

	public static void DelaySynchronizedAction(Action action, Fix64 seconds){
        if (fixedDeltaTime > 0) {
            DelaySynchronizedAction(action, (int)FPMath.Floor(seconds * config.fps));
		}else{
			DelaySynchronizedAction(action, 1);
		}
	}

	public static void DelaySynchronizedAction(Action action, int steps){
		DelaySynchronizedAction(new DelayedAction(action, steps));
	}

	public static void DelaySynchronizedAction(DelayedAction delayedAction){
		delayedSynchronizedActions.Add(delayedAction);
	}
	
	
	public static bool FindDelaySynchronizedAction(Action action){
		foreach (DelayedAction delayedAction in delayedSynchronizedActions){
			if (action == delayedAction.action) return true;
		}
		return false;
	}

    public static bool FindAndUpdateDelaySynchronizedAction(Action action, Fix64 seconds) {
		foreach (DelayedAction delayedAction in delayedSynchronizedActions){
			if (action == delayedAction.action) {
				delayedAction.steps = (int)FPMath.Floor(seconds * config.fps);
				return true;
			}
		}
		return false;
	}

    public static void FindAndRemoveDelaySynchronizedAction(Action action) {
        foreach (DelayedAction delayedAction in delayedSynchronizedActions) {
            if (action == delayedAction.action) {
                delayedSynchronizedActions.Remove(delayedAction);
                return;
            }
        }
    }

    public static void FindAndRemoveDelayLocalAction(Action action) {
        foreach (DelayedAction delayedAction in delayedLocalActions) {
            if (action == delayedAction.action) {
                delayedLocalActions.Remove(delayedAction);
                return;
            }
        }
    }

    public static GameObject SpawnGameObject(GameObject gameObject, Vector3 position, Quaternion rotation, long? destroyTimer = null) {
        if (gameObject == null) return null;

        GameObject goInstance = UnityEngine.Object.Instantiate(gameObject, position, rotation);
        goInstance.transform.SetParent(gameEngine.transform);
        MrFusion mrFusion = (MrFusion) goInstance.GetComponent(typeof(MrFusion));
        if (mrFusion == null) mrFusion = goInstance.AddComponent<MrFusion>();

        instantiatedObjects.Add(new InstantiatedGameObject(goInstance, mrFusion, currentFrame, currentFrame + destroyTimer));

        return goInstance;
    }

    public static void DestroyGameObject(GameObject gameObject, long? destroyTimer = null) {
        for (int i = 0; i < instantiatedObjects.Count; ++i) {
            if (instantiatedObjects[i].gameObject == gameObject) {
                instantiatedObjects[i].destructionFrame = destroyTimer == null? currentFrame : destroyTimer;
                break;
            }
        }
    }

	#endregion

	#region public class methods: Audio related methods
	public static bool GetMusic(){
		return config.music;
	}

	public static AudioClip GetMusicClip(){
		return musicAudioSource.clip;
	}
	
	public static bool GetSoundFX(){
		return config.soundfx;
	}

	public static float GetMusicVolume(){
		if (config != null) return config.musicVolume;
		return 1f;
	}

	public static float GetSoundFXVolume(){
		if (config != null) return config.soundfxVolume;
		return 1f;
	}

	public static void InitializeAudioSystem(){
		Camera cam = Camera.main;

		// Create the AudioSources required for the music and sound effects
		musicAudioSource = cam.GetComponent<AudioSource>();
		if (musicAudioSource == null){
			musicAudioSource = cam.gameObject.AddComponent<AudioSource>();
		}

		musicAudioSource.loop = true;
		musicAudioSource.playOnAwake = false;
		musicAudioSource.volume = config.musicVolume;


		soundsAudioSource = cam.gameObject.AddComponent<AudioSource>();
		soundsAudioSource.loop = false;
		soundsAudioSource.playOnAwake = false;
		soundsAudioSource.volume = 1f;
	}

	public static bool IsPlayingMusic(){
		if (musicAudioSource.clip != null) return musicAudioSource.isPlaying;
		return false;
	}

	public static bool IsMusicLooped(){
		return musicAudioSource.loop;
	}

	public static bool IsPlayingSoundFX(){
		return false;
	}

	public static void LoopMusic(bool loop){
		musicAudioSource.loop = loop;
	}

	public static void PlayMusic(){
		if (config.music && !IsPlayingMusic() && musicAudioSource.clip != null){
			musicAudioSource.Play();
		}
	}

	public static void PlayMusic(AudioClip music){
		if (music != null){
			AudioClip oldMusic = GetMusicClip();

			if (music != oldMusic){
				musicAudioSource.clip = music;
			}

			if (config.music && (music != oldMusic || !IsPlayingMusic())){
				musicAudioSource.Play();
			}
		}
	}

	public static void PlaySound(IList<AudioClip> sounds){
		if (sounds.Count > 0){
			PlaySound(sounds[UnityEngine.Random.Range(0, sounds.Count)]);
		}
	}
	
	public static void PlaySound(AudioClip soundFX){
		PlaySound(soundFX, GetSoundFXVolume());
	}

	public static void PlaySound(AudioClip soundFX, float volume){
		if (config.soundfx && soundFX != null && soundsAudioSource != null){
			soundsAudioSource.PlayOneShot(soundFX, volume);
		}
	}
	
	public static void SetMusic(bool on){
		bool isPlayingMusic = IsPlayingMusic();
		config.music = on;

		if (on && !isPlayingMusic)		PlayMusic();
		else if (!on && isPlayingMusic)	StopMusic();

		PlayerPrefs.SetInt(MusicEnabledKey, on ? 1 : 0);
		PlayerPrefs.Save();
	}
	
	public static void SetSoundFX(bool on){
		config.soundfx = on;
		PlayerPrefs.SetInt(SoundsEnabledKey, on ? 1 : 0);
		PlayerPrefs.Save();
	}
	
	public static void SetMusicVolume(float volume){
		if (config != null) config.musicVolume = volume;
		if (musicAudioSource != null) musicAudioSource.volume = volume;

		PlayerPrefs.SetFloat(MusicVolumeKey, volume);
		PlayerPrefs.Save();
	}

	public static void SetSoundFXVolume(float volume){
		if (config != null) config.soundfxVolume = volume;
		PlayerPrefs.SetFloat(SoundsVolumeKey, volume);
		PlayerPrefs.Save();
	}
    
    public static void StopMusic()
    {
        if (musicAudioSource.clip != null) musicAudioSource.Stop();
    }

    public static void StopSounds()
    {
        soundsAudioSource.Stop();
    }
    #endregion

    #region public class methods: AI related methods
    public static void SetAIEngine(AIEngine engine){
		config.aiOptions.engine = engine;
	}
	
	public static AIEngine GetAIEngine(){
		return config.aiOptions.engine;
	}

    public static ChallengeModeOptions GetChallenge(int challengeNum) {
        return config.challengeModeOptions[challengeNum];
    }
	
	public static void SetDebugMode(bool flag){
		config.debugOptions.debugMode = flag;
		if (debugger1 != null) debugger1.enabled = flag;
        if (debugger2 != null) debugger2.enabled = flag;
	}

	public static void SetAIDifficulty(AIDifficultyLevel difficulty){
		foreach(AIDifficultySettings difficultySettings in config.aiOptions.difficultySettings){
			if (difficultySettings.difficultyLevel == difficulty) {
				SetAIDifficulty(difficultySettings);
				break;
			}
		}
	}

	public static void SetAIDifficulty(AIDifficultySettings difficulty){
		config.aiOptions.selectedDifficulty = difficulty;
		config.aiOptions.selectedDifficultyLevel = difficulty.difficultyLevel;

		for (int i = 0; i < config.aiOptions.difficultySettings.Length; ++i){
			if (difficulty == config.aiOptions.difficultySettings[i]){
				PlayerPrefs.SetInt(DifficultyLevelKey, i);
				PlayerPrefs.Save();
				break;
			}
		}
	}

	public static void SetSimpleAI(int player, SimpleAIBehaviour behaviour){
		if (player == 1){
			p1SimpleAI.behaviour = behaviour;
			p1Controller.cpuController = p1SimpleAI;
		}else if (player == 2){
			p2SimpleAI.behaviour = behaviour;
			p2Controller.cpuController = p2SimpleAI;
		}
	}

	public static void SetRandomAI(int player){
		if (player == 1){
			p1Controller.cpuController = p1RandomAI;
		}else if (player == 2){
			p2Controller.cpuController = p2RandomAI;
		}
	}

	public static void SetFuzzyAI(int player, UFE3D.CharacterInfo character){
		SetFuzzyAI(player, character, config.aiOptions.selectedDifficulty);
	}

	public static void SetFuzzyAI(int player, UFE3D.CharacterInfo character, AIDifficultySettings difficulty){
		if (isAiAddonInstalled){
			if (player == 1){
				p1Controller.cpuController = p1FuzzyAI;
			}else if (player == 2){
				p2Controller.cpuController = p2FuzzyAI;
			}

			UFEController controller = GetController(player);
			if (controller != null && controller.isCPU){
				AbstractInputController cpu = controller.cpuController;

				if (cpu != null){
					MethodInfo method = cpu.GetType().GetMethod(
						"SetAIInformation", 
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy,
						null,
						new Type[]{typeof(ScriptableObject)},
						null
					);

					if (method != null){
						if (character != null && character.aiInstructionsSet != null && character.aiInstructionsSet.Length > 0){
							if (difficulty.startupBehavior == AIBehavior.Any){
								method.Invoke(cpu, new object[]{character.aiInstructionsSet[0].aiInfo});
							}else{
								ScriptableObject selectedAIInfo = character.aiInstructionsSet[0].aiInfo;
								foreach(AIInstructionsSet instructionSet in character.aiInstructionsSet){
									if (instructionSet.behavior == difficulty.startupBehavior){
										selectedAIInfo = instructionSet.aiInfo;
										break;
									}
								}
								method.Invoke(cpu, new object[]{selectedAIInfo});
							}
						}else{
							method.Invoke(cpu, new object[]{null});
						}
					}
				}
			}
		}
	}
	#endregion

	#region public class methods: Story Mode related methods
	public static CharacterStory GetCharacterStory(UFE3D.CharacterInfo character){
		if (!config.storyMode.useSameStoryForAllCharacters){
			StoryMode storyMode = config.storyMode;

			for (int i = 0; i < config.characters.Length; ++i){
				if (config.characters[i] == character && storyMode.selectableCharactersInStoryMode.Contains(i)){
					CharacterStory characterStory = null;

					if (storyMode.characterStories.TryGetValue(i, out characterStory) && characterStory != null){
						return characterStory;
					}
				}
			}
		}
		
		return config.storyMode.defaultStory;
	}
	

	public static AIDifficultySettings GetAIDifficulty(){
		return config.aiOptions.selectedDifficulty;
	}
	#endregion

	#region public class methods: GUI Related methods
	public static BattleGUI GetBattleGUI(){
		return config.gameGUI.battleGUI;
	}

	public static BluetoothGameScreen GetBluetoothGameScreen(){
		return config.gameGUI.bluetoothGameScreen;
	}

	public static CharacterSelectionScreen GetCharacterSelectionScreen(){
		return config.gameGUI.characterSelectionScreen;
	}

	public static ConnectionLostScreen GetConnectionLostScreen(){
		return config.gameGUI.connectionLostScreen;
	}

	public static CreditsScreen GetCreditsScreen(){
		return config.gameGUI.creditsScreen;
	}

	public static HostGameScreen GetHostGameScreen(){
		return config.gameGUI.hostGameScreen;
	}

	public static IntroScreen GetIntroScreen(){
		return config.gameGUI.introScreen;
	}

	public static JoinGameScreen GetJoinGameScreen(){
		return config.gameGUI.joinGameScreen;
	}

	public static LoadingBattleScreen GetLoadingBattleScreen(){
		return config.gameGUI.loadingBattleScreen;
	}

	public static MainMenuScreen GetMainMenuScreen(){
		return config.gameGUI.mainMenuScreen;
	}

	public static NetworkGameScreen GetNetworkGameScreen(){
		return config.gameGUI.networkGameScreen;
	}

	public static OptionsScreen GetOptionsScreen(){
		return config.gameGUI.optionsScreen;
	}

	public static StageSelectionScreen GetStageSelectionScreen(){
		return config.gameGUI.stageSelectionScreen;
	}

	public static StoryModeScreen GetStoryModeCongratulationsScreen(){
		return config.gameGUI.storyModeCongratulationsScreen;
	}

	public static StoryModeContinueScreen GetStoryModeContinueScreen(){
		return config.gameGUI.storyModeContinueScreen;
	}

	public static StoryModeScreen GetStoryModeGameOverScreen(){
		return config.gameGUI.storyModeGameOverScreen;
	}

	public static VersusModeAfterBattleScreen GetVersusModeAfterBattleScreen(){
		return config.gameGUI.versusModeAfterBattleScreen;
	}

	public static VersusModeScreen GetVersusModeScreen(){
		return config.gameGUI.versusModeScreen;
	}

	public static void HideScreen(UFEScreen screen){
		if (screen != null){
			screen.OnHide();
			Destroy(screen.gameObject);
            if (!gameRunning && gameEngine != null) EndGame();
		}
	}
	
	public static void ShowScreen(UFEScreen screen, Action nextScreenAction = null){
		if (screen != null){
			if (OnScreenChanged != null){
				OnScreenChanged(currentScreen, screen);
			}

			currentScreen = (UFEScreen) GameObject.Instantiate(screen);
			currentScreen.transform.SetParent(canvas != null ? canvas.transform : null, false);

			StoryModeScreen storyModeScreen = currentScreen as StoryModeScreen;
			if (storyModeScreen != null){
				storyModeScreen.nextScreenAction = nextScreenAction;
			}

			currentScreen.OnShow ();
		}
	}

	public static void Quit(){
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	public static void StartBluetoothGameScreen(){
		StartBluetoothGameScreen((float)config.gameGUI.screenFadeDuration);
	}
	
	public static void StartBluetoothGameScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartBluetoothGameScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartBluetoothGameScreen(fadeTime / 2f);
        }
	}

	public static void StartCharacterSelectionScreen(){
		StartCharacterSelectionScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartCharacterSelectionScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartCharacterSelectionScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartCharacterSelectionScreen(fadeTime / 2f);
        }
	}

	public static void StartCpuVersusCpu(){
		StartCpuVersusCpu((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartCpuVersusCpu(float fadeTime){
		gameMode = GameMode.VersusMode;
		SetCPU(1, true);
		SetCPU(2, true);
		StartCharacterSelectionScreen(fadeTime);
	}

	public static void StartConnectionLostScreenIfMainMenuNotLoaded(){
		StartConnectionLostScreenIfMainMenuNotLoaded((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartConnectionLostScreenIfMainMenuNotLoaded(float fadeTime){
		if ((currentScreen as MainMenuScreen) == null){
			StartConnectionLostScreen(fadeTime);
		}
	}

	public static void StartConnectionLostScreen(){
		StartConnectionLostScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartConnectionLostScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartConnectionLostScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartConnectionLostScreen(fadeTime / 2f);
        }
	}

	public static void StartCreditsScreen(){
		StartCreditsScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartCreditsScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartCreditsScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartCreditsScreen(fadeTime / 2f);
        }
	}

	public static void StartGame(){
		StartGame((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartGame(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.gameFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartGame(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartGame(fadeTime / 2f);
        }
	}

	public static void StartHostGameScreen(){
		StartHostGameScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartHostGameScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartHostGameScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartHostGameScreen(fadeTime / 2f);
        }
	}

	public static void StartIntroScreen(){
		StartIntroScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartIntroScreen(float fadeTime){
        if (currentScreen != null && currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartIntroScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartIntroScreen(fadeTime / 2f);
        }
	}

	public static void StartJoinGameScreen(){
		StartJoinGameScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartJoinGameScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartJoinGameScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartJoinGameScreen(fadeTime / 2f);
        }
	}

	public static void StartLoadingBattleScreen(){
		StartLoadingBattleScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartLoadingBattleScreen(float fadeTime){
        if (currentScreen != null && currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartLoadingBattleScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartLoadingBattleScreen(fadeTime / 2f);
        }
	}

	public static void StartMainMenuScreen(){
		StartMainMenuScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartMainMenuScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
	        //Debug.Log(currentScreen.name);
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartMainMenuScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
	        //Debug.Log(currentScreen.name + ", else");
            _StartMainMenuScreen(fadeTime / 2f);
        }
	}

	public static void StartSearchMatchScreen(){
		StartSearchMatchScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartSearchMatchScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartSearchMatchScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartSearchMatchScreen(fadeTime / 2f);
        }
	}

	public static void StartNetworkGameScreen(){
		StartNetworkGameScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartNetworkGameScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartNetworkGameScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartNetworkGameScreen(fadeTime / 2f);
        }
	}

	public static void StartOptionsScreen(){
		StartOptionsScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartOptionsScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartOptionsScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartOptionsScreen(fadeTime / 2f);
        }
	}

	public static void StartPlayerVersusPlayer(){
		StartPlayerVersusPlayer((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartPlayerVersusPlayer(float fadeTime){
		gameMode = GameMode.VersusMode;
		SetCPU(1, false);
		SetCPU(2, false);
		StartCharacterSelectionScreen(fadeTime);
	}

	public static void StartPlayerVersusCpu(){
		StartPlayerVersusCpu((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartPlayerVersusCpu(float fadeTime){
		gameMode = GameMode.VersusMode;
		SetCPU(1, false);
		SetCPU(2, true);
		StartCharacterSelectionScreen(fadeTime);
	}

	public static void StartNetworkGame(float fadeTime, int localPlayer, bool startImmediately){
		disconnecting = false;
		Application.runInBackground = true;

		localPlayerController.Initialize(p1Controller.inputReferences);
		localPlayerController.humanController = p1Controller.humanController;
		localPlayerController.cpuController = p1Controller.cpuController;
		remotePlayerController.Initialize(p2Controller.inputReferences);

		if (localPlayer == 1){
			localPlayerController.player = 1;
			remotePlayerController.player = 2;
		}else{
			localPlayerController.player = 2;
			remotePlayerController.player = 1;
		}

		fluxCapacitor.Initialize();
		gameMode = GameMode.NetworkGame;
		SetCPU(1, false);
		SetCPU(2, false);

        if (startImmediately) {
            StartLoadingBattleScreen(fadeTime);
            //StartGame();
        } else {
            StartCharacterSelectionScreen(fadeTime);
        }
	}

	public static void StartStageSelectionScreen(){
		StartStageSelectionScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartStageSelectionScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartStageSelectionScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartStageSelectionScreen(fadeTime / 2f);
        }
	}

	public static void StartStoryMode(){
		StartStoryMode((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartStoryMode(float fadeTime){
		//-------------------------------------------------------------------------------------------------------------
		// Required for loading the first combat correctly.
		player1WonLastBattle = true; 
		//-------------------------------------------------------------------------------------------------------------
		gameMode = GameMode.StoryMode;
		SetCPU(1, false);
		SetCPU(2, true);
		storyMode.characterStory = null;
		storyMode.canFightAgainstHimself = config.storyMode.canCharactersFightAgainstThemselves;
		storyMode.currentGroup = -1;
		storyMode.currentBattle = -1;
		storyMode.currentBattleInformation = null;
		storyMode.defeatedOpponents.Clear();
		StartCharacterSelectionScreen(fadeTime);
	}

	public static void StartStoryModeBattle(){
		StartStoryModeBattle((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartStoryModeBattle(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartStoryModeBattle(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartStoryModeBattle(fadeTime / 2f);
        }
	}

	public static void StartStoryModeCongratulationsScreen(){
		StartStoryModeCongratulationsScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartStoryModeCongratulationsScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartStoryModeCongratulationsScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartStoryModeCongratulationsScreen(fadeTime / 2f);
        }
	}

	public static void StartStoryModeContinueScreen(){
		StartStoryModeContinueScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartStoryModeContinueScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartStoryModeContinueScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartStoryModeContinueScreen(fadeTime / 2f);
        }
	}

	public static void StartStoryModeConversationAfterBattleScreen(UFEScreen conversationScreen){
		StartStoryModeConversationAfterBattleScreen(conversationScreen, (float)config.gameGUI.screenFadeDuration);
	}

	public static void StartStoryModeConversationAfterBattleScreen(UFEScreen conversationScreen, float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartStoryModeConversationAfterBattleScreen(conversationScreen, fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartStoryModeConversationAfterBattleScreen(conversationScreen, fadeTime / 2f);
        }
	}

	public static void StartStoryModeConversationBeforeBattleScreen(UFEScreen conversationScreen){
		StartStoryModeConversationBeforeBattleScreen(conversationScreen, (float)config.gameGUI.screenFadeDuration);
	}

	public static void StartStoryModeConversationBeforeBattleScreen(UFEScreen conversationScreen, float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartStoryModeConversationBeforeBattleScreen(conversationScreen, fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartStoryModeConversationBeforeBattleScreen(conversationScreen, fadeTime / 2f);
        }
	}

	public static void StartStoryModeEndingScreen(){
		StartStoryModeEndingScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartStoryModeEndingScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartStoryModeEndingScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartStoryModeEndingScreen(fadeTime / 2f);
        }
	}

	public static void StartStoryModeGameOverScreen(){
		StartStoryModeGameOverScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartStoryModeGameOverScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartStoryModeGameOverScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartStoryModeGameOverScreen(fadeTime / 2f);
        }
	}

	public static void StartStoryModeOpeningScreen(){
		StartStoryModeOpeningScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartStoryModeOpeningScreen(float fadeTime){
		// First, retrieve the character story, so we can find the opening associated to this player
		storyMode.characterStory = GetCharacterStory(GetPlayer1());

        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartStoryModeOpeningScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartStoryModeOpeningScreen(fadeTime / 2f);
        }
	}

	public static void StartTrainingMode(){
		StartTrainingMode((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartTrainingMode(float fadeTime){
		gameMode = GameMode.TrainingRoom;
		SetCPU(1, false);
		SetCPU(2, false);
		StartCharacterSelectionScreen(fadeTime);
	}

	public static void StartVersusModeAfterBattleScreen(){
		StartVersusModeAfterBattleScreen(0f);
	}

	public static void StartVersusModeAfterBattleScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartVersusModeAfterBattleScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartVersusModeAfterBattleScreen(fadeTime / 2f);
        }
	}

	public static void StartVersusModeScreen(){
		StartVersusModeScreen((float)config.gameGUI.screenFadeDuration);
	}

	public static void StartVersusModeScreen(float fadeTime){
        if (currentScreen.hasFadeOut) {
            eventSystem.enabled = false;
            CameraFade.StartAlphaFade(
                config.gameGUI.screenFadeColor,
                false,
                fadeTime / 2f,
                0f
            );
            DelayLocalAction(() => { eventSystem.enabled = true; _StartVersusModeScreen(fadeTime / 2f); }, (Fix64)fadeTime / 2);
        } else {
            _StartVersusModeScreen(fadeTime / 2f);
        }
	}

	public static void WonStoryModeBattle(){
		WonStoryModeBattle((float)config.gameGUI.screenFadeDuration);
	}

	public static void WonStoryModeBattle(float fadeTime){
		storyMode.defeatedOpponents.Add(storyMode.currentBattleInformation.opponentCharacterIndex);
		StartStoryModeConversationAfterBattleScreen(storyMode.currentBattleInformation.conversationAfterBattle, fadeTime);
	}
	#endregion

	#region public class methods: Language
	public static void SetLanguage(){
		foreach(LanguageOptions languageOption in config.languages){
			if (languageOption.defaultSelection){
				config.selectedLanguage = languageOption;
				return;
			}
		}
	}

	public static void SetLanguage(string language){
		foreach(LanguageOptions languageOption in config.languages){
			if (language == languageOption.languageName){
				config.selectedLanguage = languageOption;
				return;
			}
		}
	}
	#endregion

	#region public class methods: Input Related methods
	public static bool GetCPU(int player){
		UFEController controller = GetController(player);
		if (controller != null){
			return controller.isCPU;
		}
		return false;
	}

	public static string GetInputReference(ButtonPress button, InputReferences[] inputReferences){
		foreach(InputReferences inputReference in inputReferences){
			if (inputReference.engineRelatedButton == button) return inputReference.inputButtonName;
		}
		return null;
	}
	
	public static string GetInputReference(InputType inputType, InputReferences[] inputReferences){
		foreach(InputReferences inputReference in inputReferences){
			if (inputReference.inputType == inputType) return inputReference.inputButtonName;
		}
		return null;
	}

	public static UFEController GetPlayer1Controller(){
		if (isNetworkAddonInstalled && isConnected){
			if (multiplayerAPI.IsServer()){
				return localPlayerController;
			}else{
				return remotePlayerController;
			}
		}
		return p1Controller;
	}
	
	public static UFEController GetPlayer2Controller(){
		if (isNetworkAddonInstalled && isConnected){
			if (multiplayerAPI.IsServer()){
				return remotePlayerController;
			}else{
				return localPlayerController;
			}
		}
		return p2Controller;
	}
	
	public static UFEController GetController(int player){
		if		(player == 1)	return GetPlayer1Controller();
		else if (player == 2)	return GetPlayer2Controller();
		else					return null;
	}
	
	public static int GetLocalPlayer(){
		if		(localPlayerController == GetPlayer1Controller())	return 1;
		else if	(localPlayerController == GetPlayer2Controller())	return 2;
		else																return -1;
	}
	
	public static int GetRemotePlayer(){
		if		(remotePlayerController == GetPlayer1Controller())	return 1;
		else if	(remotePlayerController == GetPlayer2Controller())	return 2;
		else																return -1;
	}

	public static void SetAI(int player, UFE3D.CharacterInfo character){
		if (isAiAddonInstalled){
			UFEController controller = GetController(player);
			
			if (controller != null && controller.isCPU){
				AbstractInputController cpu = controller.cpuController;
				
				if (cpu != null){
					MethodInfo method = cpu.GetType().GetMethod(
						"SetAIInformation", 
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy,
						null,
						new Type[]{typeof(ScriptableObject)},
					null
					);
					
					if (method != null){
						if (character != null && character.aiInstructionsSet != null && character.aiInstructionsSet.Length > 0){
							method.Invoke(cpu, new object[]{character.aiInstructionsSet[0].aiInfo});
						}else{
							method.Invoke(cpu, new object[]{null});
						}
					}
				}
			}
		}
	}

	public static void SetCPU(int player, bool cpuToggle){
		UFEController controller = GetController(player);
		if (controller != null){
			controller.isCPU = cpuToggle;
		}
	}
	#endregion

	#region public class methods: methods related to the character selection
	public static UFE3D.CharacterInfo GetPlayer(int player){
		if (player == 1){
			return GetPlayer1();
		}else if (player == 2){
			return GetPlayer2();
		}
		return null;
	}
	
	public static UFE3D.CharacterInfo GetPlayer1(){
		return config.player1Character;
	}
	
	public static UFE3D.CharacterInfo GetPlayer2(){
		return config.player2Character;
	}

	public static UFE3D.CharacterInfo[] GetStoryModeSelectableCharacters(){
		List<UFE3D.CharacterInfo> characters = new List<UFE3D.CharacterInfo>();

		for (int i = 0; i < config.characters.Length; ++i){
			if(
				config.characters[i] != null 
				&& 
				(
					config.storyMode.selectableCharactersInStoryMode.Contains(i) || 
					unlockedCharactersInStoryMode.Contains(config.characters[i].characterName)
				)
			){
				characters.Add(config.characters[i]);
			}
		}
		
		return characters.ToArray();
	}

	public static UFE3D.CharacterInfo[] GetTrainingRoomSelectableCharacters(){
		List<UFE3D.CharacterInfo> characters = new List<UFE3D.CharacterInfo>();
		
		for (int i = 0; i < config.characters.Length; ++i){
			// If the character is selectable on Story Mode or Versus Mode,
			// then the character should be selectable on Training Room...
			if(
				config.characters[i] != null 
				&& 
				(
					config.storyMode.selectableCharactersInStoryMode.Contains(i) || 
					config.storyMode.selectableCharactersInVersusMode.Contains(i) || 
					unlockedCharactersInStoryMode.Contains(config.characters[i].characterName) ||
					unlockedCharactersInVersusMode.Contains(config.characters[i].characterName)
				)
			){
				characters.Add(config.characters[i]);
			}
		}
		
		return characters.ToArray();
	}
	
	public static UFE3D.CharacterInfo[] GetVersusModeSelectableCharacters(){
		List<UFE3D.CharacterInfo> characters = new List<UFE3D.CharacterInfo>();
		
		for (int i = 0; i < config.characters.Length; ++i){
			if(
				config.characters[i] != null && 
				(
					config.storyMode.selectableCharactersInVersusMode.Contains(i) || 
					unlockedCharactersInVersusMode.Contains(config.characters[i].characterName)
				)
			){
				characters.Add(config.characters[i]);
			}
		}
		
		return characters.ToArray();
	}

	public static void SetPlayer(int player, UFE3D.CharacterInfo info){
		if (player == 1){
			config.player1Character = info;
		}else if (player == 2){
			config.player2Character = info;
		}
	}

	public static void SetPlayer1(UFE3D.CharacterInfo player1){
		config.player1Character = player1;
	}

	public static void SetPlayer2(UFE3D.CharacterInfo player2){
		config.player2Character = player2;
	}

	public static void LoadUnlockedCharacters(){
		unlockedCharactersInStoryMode.Clear();
		string value = PlayerPrefs.GetString("UCSM", null);

		if (!string.IsNullOrEmpty(value)){
			string[] characters = value.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string character in characters){
				unlockedCharactersInStoryMode.Add(character);
			}
		}


		unlockedCharactersInVersusMode.Clear();
		value = PlayerPrefs.GetString("UCVM", null);
		
		if (!string.IsNullOrEmpty(value)){
			string[] characters = value.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string character in characters){
				unlockedCharactersInVersusMode.Add(character);
			}
		}
	}

	public static void SaveUnlockedCharacters(){
		StringBuilder sb = new StringBuilder();
		foreach (string characterName in unlockedCharactersInStoryMode){
			if (!string.IsNullOrEmpty(characterName)){
				if (sb.Length > 0){
					sb.AppendLine();
				}
				sb.Append(characterName);
			}
		}
		PlayerPrefs.SetString("UCSM", sb.ToString());

		sb = new StringBuilder();
		foreach (string characterName in unlockedCharactersInVersusMode){
			if (!string.IsNullOrEmpty(characterName)){
				if (sb.Length > 0){
					sb.AppendLine();
				}
				sb.Append(characterName);
			}
		}
		PlayerPrefs.SetString("UCVM", sb.ToString());
		PlayerPrefs.Save();
	}

	public static void RemoveUnlockedCharacterInStoryMode(UFE3D.CharacterInfo character){
		if (character != null && !string.IsNullOrEmpty(character.characterName)){
			unlockedCharactersInStoryMode.Remove(character.characterName);
		}
		
		SaveUnlockedCharacters();
	}

	public static void RemoveUnlockedCharacterInVersusMode(UFE3D.CharacterInfo character){
		if (character != null && !string.IsNullOrEmpty(character.characterName)){
			unlockedCharactersInVersusMode.Remove(character.characterName);
		}
		
		SaveUnlockedCharacters();
	}

	public static void RemoveUnlockedCharactersInStoryMode(){
		unlockedCharactersInStoryMode.Clear();
		SaveUnlockedCharacters();
	}
	
	public static void RemoveUnlockedCharactersInVersusMode(){
		unlockedCharactersInVersusMode.Clear();
		SaveUnlockedCharacters();
	}

	public static void UnlockCharacterInStoryMode(UFE3D.CharacterInfo character){
		if(
			character != null && 
			!string.IsNullOrEmpty(character.characterName) &&
			!unlockedCharactersInStoryMode.Contains(character.characterName)
		){
			unlockedCharactersInStoryMode.Add(character.characterName);
		}

		SaveUnlockedCharacters();
	}

	public static void UnlockCharacterInVersusMode(UFE3D.CharacterInfo character){
		if(
			character != null && 
			!string.IsNullOrEmpty(character.characterName) &&
			!unlockedCharactersInVersusMode.Contains(character.characterName)
		){
			unlockedCharactersInVersusMode.Add(character.characterName);
		}

		SaveUnlockedCharacters();
	}
	#endregion

	#region public class methods: methods related to the stage selection
	public static void SetStage(StageOptions stage){
		config.selectedStage = stage;
	}

	public static void SetStage(string stageName){
		foreach(StageOptions stage in config.stages){
			if (stageName == stage.stageName){
				SetStage(stage);
				return;
			}
		}
	}
	
	public static StageOptions GetStage(){
		return config.selectedStage;
	}
	#endregion


	#region public class methods: methods related to the behaviour of the characters during the battle
	public static ControlsScript GetControlsScript(int player){
		if (player == 1){
			return GetPlayer1ControlsScript();
		}else if (player == 2){
			return GetPlayer2ControlsScript();
		}
		return null;
	}

	public static ControlsScript GetPlayer1ControlsScript(){
		return p1ControlsScript;
	}
	
	public static ControlsScript GetPlayer2ControlsScript(){
		return p2ControlsScript;
	}
	#endregion

	#region public class methods: methods that are used for raising events
	public static void SetLifePoints(Fix64 newValue, UFE3D.CharacterInfo player){
		if (OnLifePointsChange != null) OnLifePointsChange((float)newValue, player);
	}

	public static void FireAlert(string alertMessage, UFE3D.CharacterInfo player){
		if (OnNewAlert != null) OnNewAlert(alertMessage, player);
	}

	public static void FireHit(HitBox strokeHitBox, MoveInfo move, UFE3D.CharacterInfo player){
		if (OnHit != null) OnHit(strokeHitBox, move, player);
	}

    public static void FireBlock(HitBox strokeHitBox, MoveInfo move, UFE3D.CharacterInfo player) {
        if (OnBlock != null) OnBlock(strokeHitBox, move, player);
    }

    public static void FireParry(HitBox strokeHitBox, MoveInfo move, UFE3D.CharacterInfo player) {
        if (OnParry != null) OnParry(strokeHitBox, move, player);
    }
	
	public static void FireMove(MoveInfo move, UFE3D.CharacterInfo player){
		if (OnMove != null) OnMove(move, player);
	}

    public static void FireButton(ButtonPress button, UFE3D.CharacterInfo player) {
        if (OnButton != null) OnButton(button, player);
    }

    public static void FireBasicMove(BasicMoveReference basicMove, UFE3D.CharacterInfo player) {
        if (OnBasicMove != null) OnBasicMove(basicMove, player);
    }

    public static void FireBodyVisibilityChange(MoveInfo move, UFE3D.CharacterInfo player, BodyPartVisibilityChange bodyPartVisibilityChange, HitBox hitBox) {
        if (OnBodyVisibilityChange != null) OnBodyVisibilityChange(move, player, bodyPartVisibilityChange, hitBox);
    }

    public static void FireParticleEffects(MoveInfo move, UFE3D.CharacterInfo player, MoveParticleEffect particleEffects) {
        if (OnParticleEffects != null) OnParticleEffects(move, player, particleEffects);
    }

    public static void FireSideSwitch(int side, UFE3D.CharacterInfo player) {
        if (OnSideSwitch != null) OnSideSwitch(side, player);
    }

	public static void FireGameBegins(){
		if (OnGameBegin != null) {
			gameRunning = true;
			OnGameBegin(config.player1Character, config.player2Character, config.selectedStage);
		}
	}
	
	public static void FireGameEnds(UFE3D.CharacterInfo winner, UFE3D.CharacterInfo loser){
		// I've commented the next line because it worked with the old GUI, but not with the new one.
		//EndGame();

        timeScale = config._gameSpeed;
		gameRunning = false;
		newRoundCasted = false;
		player1WonLastBattle = (winner == GetPlayer1());

		/*if (fluxGameManager != null){
			fluxGameManager.Initialize();
		}*/

		if (OnGameEnds != null) {
			OnGameEnds(winner, loser);
		}
	}
	
	public static void FireRoundBegins(int currentRound){
		if (OnRoundBegins != null) OnRoundBegins(currentRound);
	}

	public static void FireRoundEnds(UFE3D.CharacterInfo winner, UFE3D.CharacterInfo loser){
		if (OnRoundEnds != null) OnRoundEnds(winner, loser);
	}

	public static void FireTimer(float timer){
		if (OnTimer != null) OnTimer(timer);
	}

	public static void FireTimeOver(){
		if (OnTimeOver != null) OnTimeOver();
	}
	#endregion

    
	#region public class methods: UFE CORE methods
	public static void PauseGame(bool pause){
        switch (pause)
        {
	        case true when timeScale == 0:
		        return;
	        case true:
		        timeScale = 0;
		        break;
	        default:
		        timeScale = config._gameSpeed;
		        break;
        }

        OnGamePaused?.Invoke(pause);
	}

	public static bool IsInstalled(string theClass){
		return SearchClass(theClass) != null;
	}
	
	public static bool isPaused(){
        return timeScale <= 0;
	}
	
	public static Fix64 GetTimer(){
		return timer;
	}
	
	public static void ResetTimer(){
		timer = config.roundOptions._timer;
		intTimer = (int)FPMath.Round(config.roundOptions._timer);
		if (OnTimer != null) OnTimer((float)timer);
	}
	
	public static Type SearchClass(string theClass){
		Type type = null;
		
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()){
			type = assembly.GetType(theClass);
			if (type != null){break;}
		}
		
		return type;
	}
	
	public static void SetTimer(Fix64 time){
		timer = time;
		intTimer = (int)FPMath.Round(time);
		if (OnTimer != null) OnTimer(timer);
	}
	
	public static void PlayTimer(){
		pauseTimer = false;
	}
	
	public static void PauseTimer(){
		pauseTimer = true;
	}
	
	public static bool IsTimerPaused(){
		return pauseTimer;
	}

    public static void EndGame(){
        /*timeScale = ToDouble(config.gameSpeed);
		gameRunning = false;
		newRoundCasted = false;*/

		if (battleGUI != null){
			battleGUI.OnHide();
            Destroy(battleGUI.gameObject);
            battleGUI = null;
		}
		
		if (gameEngine != null) {
			Destroy(gameEngine);
			gameEngine = null;
		}
	}

	public static void ResetRoundCast(){
		newRoundCasted = false;
	}
	
	public static void CastNewRound(){
		if (newRoundCasted) return;
		if (!p1ControlsScript.introPlayed || !p2ControlsScript.introPlayed) return;
		FireRoundBegins(config.currentRound);
		DelaySynchronizedAction(StartFight, (Fix64)2);
		newRoundCasted = true;
	}

    public static void StartFight() {
        if (gameMode != GameMode.ChallengeMode) 
            FireAlert(config.selectedLanguage.fight, null);
        config.lockInputs = false;
        config.lockMovements = false;
        PlayTimer();
    }

	public static void CastInput(InputReferences[] inputReferences, int player){
		if (OnInput != null) OnInput(inputReferences, player);
	}
	#endregion

	#region public class methods: Network Related methods
	public static void HostBluetoothGame(){
		if (isNetworkAddonInstalled){
			multiplayerMode = MultiplayerMode.Bluetooth;
			AddNetworkEventListeners();
			multiplayerAPI.CreateMatch(new MultiplayerAPI.MatchCreationRequest(config.networkOptions.port, null, 1, false, null));
		}
	}

	public static void HostGame(){
		if (isNetworkAddonInstalled){
			multiplayerMode = MultiplayerMode.Lan;

			AddNetworkEventListeners();
			multiplayerAPI.CreateMatch(new MultiplayerAPI.MatchCreationRequest(config.networkOptions.port, null, 1, false, null));
		}
	}

	public static void JoinBluetoothGame(){
		if (isNetworkAddonInstalled){
			multiplayerMode = MultiplayerMode.Bluetooth;

			multiplayerAPI.OnMatchesDiscovered += OnMatchesDiscovered;
			multiplayerAPI.OnMatchDiscoveryError += OnMatchDiscoveryError;
			multiplayerAPI.StartSearchingMatches();
		}
	}

	protected static void OnMatchesDiscovered(ReadOnlyCollection<MultiplayerAPI.MatchInformation> matches){
		multiplayerAPI.OnMatchesDiscovered -= OnMatchesDiscovered;
		multiplayerAPI.OnMatchDiscoveryError -= OnMatchDiscoveryError;
		AddNetworkEventListeners();

		if (matches != null && matches.Count > 0){
			// TODO: let the player choose the desired game
			multiplayerAPI.JoinMatch(matches[0]);
		}else{
			StartConnectionLostScreen();
		}
    }
    
	protected static void OnMatchDiscoveryError(){
		multiplayerAPI.OnMatchesDiscovered -= OnMatchesDiscovered;
		multiplayerAPI.OnMatchDiscoveryError -= OnMatchDiscoveryError;
		StartConnectionLostScreen();
    }

	public static void JoinGame(MultiplayerAPI.MatchInformation match){
		if (isNetworkAddonInstalled){
			multiplayerMode = MultiplayerMode.Lan;

			AddNetworkEventListeners();
			multiplayerAPI.JoinMatch(match);
		}
	}

	public static void DisconnectFromGame(){
		if (isNetworkAddonInstalled){
			NetworkState state = multiplayerAPI.GetConnectionState();
			if (state == NetworkState.Client){
				multiplayerAPI.DisconnectFromMatch();
			}else if (state == NetworkState.Server){
				multiplayerAPI.DestroyMatch();
			}
		}
	}
	#endregion
    

	#region protected instance methods: MonoBehaviour methods
	protected void Awake(){
        config = UFE_Config;
        UFEInstance = this;
        fixedDeltaTime = 1 / (Fix64)config.fps;

        FPRandom.Init();

        // Check which characters have been unlocked
        LoadUnlockedCharacters();

        // Check the installed Addons and supported 3rd party products
        isCInputInstalled = IsInstalled("cInput");
#if UFE_LITE
        isAiAddonInstalled = false;
#else
        isAiAddonInstalled = IsInstalled("RuleBasedAI");
#endif

#if UFE_LITE || UFE_BASIC
		isNetworkAddonInstalled = false;
		isPhotonInstalled = false;
        isBluetoothAddonInstalled = false;
#else
        isNetworkAddonInstalled = IsInstalled("UnetHighLevelMultiplayerAPI") && config.networkOptions.networkService != NetworkService.Disabled;
        isPhotonInstalled = IsInstalled("PhotonMultiplayerAPI") && config.networkOptions.networkService != NetworkService.Disabled;
        isBluetoothAddonInstalled = IsInstalled("BluetoothMultiplayerAPI") && config.networkOptions.networkService != NetworkService.Disabled;
#endif

        isControlFreak1Installed = IsInstalled("TouchController");				// [DGT]
        isControlFreak2Installed = IsInstalled("ControlFreak2.UFEBridge");
        isControlFreakInstalled = isControlFreak1Installed || isControlFreak2Installed;
        isRewiredInstalled = IsInstalled("Rewired.Integration.UniversalFightingEngine.RewiredUFEInputManager");

        // Check if we should run the application in background
        Application.runInBackground = config.runInBackground;

        // Check if cInput is installed and initialize the cInput GUI
		if (isCInputInstalled){
			Type t = SearchClass("cGUI");
			if (t != null) t.GetField("cSkin").SetValue(null, config.inputOptions.cInputSkin);
		}

        //-------------------------------------------------------------------------------------------------------------
        // Initialize the GUI
        //-------------------------------------------------------------------------------------------------------------
        GameObject goGroup = new GameObject("CanvasGroup");
        canvasGroup = goGroup.AddComponent<CanvasGroup>();


        GameObject go = new GameObject("Canvas");
        go.transform.SetParent(goGroup.transform);

        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        if(EventSystem.current != null) {
            // Use the current event system if one exists
            eventSystem = EventSystem.current;
        } else {
            eventSystem = go.AddComponent<EventSystem>();
        }
        //eventSystem = GameObject.FindObjectOfType<EventSystem>();
        //if (eventSystem == null) eventSystem = go.AddComponent<EventSystem>();

        graphicRaycaster = go.AddComponent<GraphicRaycaster>();

        standaloneInputModule = go.AddComponent<StandaloneInputModule>();
        standaloneInputModule.verticalAxis = "Mouse Wheel";
        standaloneInputModule.horizontalAxis = "Mouse Wheel";
        standaloneInputModule.forceModuleActive = true;

        if (config.gameGUI.useCanvasScaler) {
            CanvasScaler cs = go.AddComponent<CanvasScaler>();
            cs.defaultSpriteDPI = config.gameGUI.canvasScaler.defaultSpriteDPI;
            cs.fallbackScreenDPI = config.gameGUI.canvasScaler.fallbackScreenDPI;
            cs.matchWidthOrHeight = config.gameGUI.canvasScaler.matchWidthOrHeight;
            cs.physicalUnit = config.gameGUI.canvasScaler.physicalUnit;
            cs.referencePixelsPerUnit = config.gameGUI.canvasScaler.referencePixelsPerUnit;
            cs.referenceResolution = config.gameGUI.canvasScaler.referenceResolution;
            cs.scaleFactor = config.gameGUI.canvasScaler.scaleFactor;
            cs.screenMatchMode = config.gameGUI.canvasScaler.screenMatchMode;
            cs.uiScaleMode = config.gameGUI.canvasScaler.scaleMode;
            
            //Line commented because we use "Screen Space - Overlay" canvas and the "dynaicPixelsPerUnit" property is only used in "World Space" Canvas.
            //cs.dynamicPixelsPerUnit = config.gameGUI.canvasScaler.dynamicPixelsPerUnit; 
        }

        // Check if "Control Freak Virtual Controller" is installed and instantiate the prefab
        if (isControlFreakInstalled && config.inputOptions.inputManagerType == InputManagerType.ControlFreak)
        {
            if (isControlFreak2Installed && (config.inputOptions.controlFreak2Prefab != null)) {
                // Try to instantiate Control Freak 2 rig prefab...
                controlFreakPrefab = (GameObject)Instantiate(config.inputOptions.controlFreak2Prefab.gameObject);
                touchControllerBridge = (controlFreakPrefab != null) ? controlFreakPrefab.GetComponent<InputTouchControllerBridge>() : null;
                touchControllerBridge.Init();

            }
            else if (isControlFreak1Installed && (config.inputOptions.controlFreakPrefab != null)) {
                // ...or try to instantiate Control Freak 1.x controller prefab...
                controlFreakPrefab = (GameObject)Instantiate(config.inputOptions.controlFreakPrefab);
            }
        }

		// Check if the "network addon" is installed
		string uuid = (config.gameName ?? "UFE") /*+ "_" + Application.version*/;
        if (isNetworkAddonInstalled)
        {
            GameObject networkManager = new GameObject("Network Manager");
            networkManager.transform.SetParent(this.gameObject.transform);

            lanMultiplayerAPI = networkManager.AddComponent(SearchClass("UnetLanMultiplayerAPI")) as MultiplayerAPI;
			lanMultiplayerAPI.Initialize(uuid);

			if (config.networkOptions.networkService == NetworkService.Unity) {
				onlineMultiplayerAPI = networkManager.AddComponent(SearchClass("UnetOnlineMultiplayerAPI")) as MultiplayerAPI;
			} else if (config.networkOptions.networkService == NetworkService.Photon && isPhotonInstalled) {
				onlineMultiplayerAPI = networkManager.AddComponent(SearchClass("PhotonMultiplayerAPI")) as MultiplayerAPI;
			}else if (config.networkOptions.networkService == NetworkService.Photon && !isPhotonInstalled){
                Debug.LogError("You need 'Photon Unity Networking' installed in order to use Photon as a Network Service.");
            }
			onlineMultiplayerAPI.Initialize(uuid);

            if (Application.platform == RuntimePlatform.Android && isBluetoothAddonInstalled) {
                bluetoothMultiplayerAPI = networkManager.AddComponent(SearchClass("BluetoothMultiplayerAPI")) as MultiplayerAPI;
            } else {
                bluetoothMultiplayerAPI = networkManager.AddComponent<NullMultiplayerAPI>();
            }
            bluetoothMultiplayerAPI.Initialize(uuid);
			

			multiplayerAPI.SendRate = 1 / (float)config.fps;

			localPlayerController = gameObject.AddComponent<UFEController> ();
			remotePlayerController = gameObject.AddComponent<DummyInputController>();

			localPlayerController.isCPU = false;
			remotePlayerController.isCPU = false;

            // TODO deprecated
            //NetworkView network = this.gameObject.AddComponent<NetworkView>();
            //network.stateSynchronization = NetworkStateSynchronization.Off;
            //network.observed = remotePlayerController;
		}else{
			lanMultiplayerAPI = this.gameObject.AddComponent<NullMultiplayerAPI>();
			lanMultiplayerAPI.Initialize(uuid);

			onlineMultiplayerAPI = this.gameObject.AddComponent<NullMultiplayerAPI>();
			onlineMultiplayerAPI.Initialize(uuid);
			
			bluetoothMultiplayerAPI = this.gameObject.AddComponent<NullMultiplayerAPI>();
			bluetoothMultiplayerAPI.Initialize(uuid);
		}

		fluxCapacitor = new FluxCapacitor(currentFrame, config.networkOptions.maxBufferSize);
		_multiplayerMode = MultiplayerMode.Lan;


		// Initialize the input systems
        p1Controller = gameObject.AddComponent<UFEController>();
        if (config.inputOptions.inputManagerType == InputManagerType.ControlFreak) {
            p1Controller.humanController = gameObject.AddComponent<InputTouchController>();
        } else if (config.inputOptions.inputManagerType == InputManagerType.Rewired) {
            p1Controller.humanController = gameObject.AddComponent<RewiredInputController>();
            (p1Controller.humanController as RewiredInputController).rewiredPlayerId = 0;
        } else {
			p1Controller.humanController = gameObject.AddComponent<InputController>();
		}

        // Initialize AI
        p1SimpleAI = gameObject.AddComponent<SimpleAI>();
		p1SimpleAI.player = 1;

		p1RandomAI = gameObject.AddComponent<RandomAI>();
		p1RandomAI.player = 1;

		p1FuzzyAI = null;
		if (isAiAddonInstalled && config.aiOptions.engine == AIEngine.FuzzyAI){
			p1FuzzyAI = gameObject.AddComponent(SearchClass("RuleBasedAI")) as AbstractInputController;
			p1FuzzyAI.player = 1;
			p1Controller.cpuController = p1FuzzyAI;
		}else{
			p1Controller.cpuController = p1RandomAI;
		}

		p1Controller.isCPU = config.p1CPUControl;
		p1Controller.player = 1;

		p2Controller = gameObject.AddComponent<UFEController> ();
		p2Controller.humanController = gameObject.AddComponent<InputController>();

		p2SimpleAI = gameObject.AddComponent<SimpleAI>();
		p2SimpleAI.player = 2;

		p2RandomAI = gameObject.AddComponent<RandomAI>();
		p2RandomAI.player = 2;

		p2FuzzyAI = null;
		if (isAiAddonInstalled && config.aiOptions.engine == AIEngine.FuzzyAI){
			p2FuzzyAI = gameObject.AddComponent(SearchClass("RuleBasedAI")) as AbstractInputController;
			p2FuzzyAI.player = 2;
			p2Controller.cpuController = p2FuzzyAI;
		}else{
			p2Controller.cpuController = p2RandomAI;
		}

		p2Controller.isCPU = config.p2CPUControl;
		p2Controller.player = 2;


		p1Controller.Initialize(config.player1_Inputs);
		p2Controller.Initialize(config.player2_Inputs);

		if (config.fps > 0) {
            timeScale = config._gameSpeed;
			Application.targetFrameRate = config.fps;
		}

        SetLanguage();
        InitializeAudioSystem();
        SetAIDifficulty(config.aiOptions.selectedDifficultyLevel);
        SetDebugMode(config.debugOptions.debugMode);

        // Load the player settings from disk
        SetMusic(PlayerPrefs.GetInt(MusicEnabledKey, 1) > 0);
		SetMusicVolume(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
		SetSoundFX(PlayerPrefs.GetInt(SoundsEnabledKey, 1) > 0);
		SetSoundFXVolume(PlayerPrefs.GetFloat(SoundsVolumeKey, 1f));
        
        // Load the intro screen or the combat, depending on the UFE Config settings
        if (config.debugOptions.startGameImmediately){
            if (config.debugOptions.matchType == MatchType.Training) {
                gameMode = GameMode.TrainingRoom;
            } else if (config.debugOptions.matchType == MatchType.Challenge) {
                gameMode = GameMode.ChallengeMode;
            } else {
                gameMode = GameMode.VersusMode;
            }
			config.player1Character = config.p1CharStorage;
			config.player2Character = config.p2CharStorage;
			SetCPU(1, config.p1CPUControl);
			SetCPU(2, config.p2CPUControl);

            if (config.debugOptions.skipLoadingScreen)
            {
                _StartGame((float)config.gameGUI.gameFadeDuration);
            }
            else
            {
                _StartLoadingBattleScreen((float)config.gameGUI.screenFadeDuration);
            }
		}else{
			StartIntroScreen(0f);
        }
    }

	protected void Update(){
		GetPlayer1Controller().DoUpdate();
		GetPlayer2Controller().DoUpdate();

#if UNITY_EDITOR
        // Save and Load State
        if (fluxCapacitor != null && config.debugOptions.stateTrackerTest) {
			if (Input.GetKeyDown(KeyCode.F2)) { // Save State
				Debug.Log("Save (" + currentFrame + ")");
                fluxCapacitor.savedState = FluxStateTracker.SaveGameState(currentFrame);

                //dictionaryList.Add(RecordVar.SaveStateTrackers(this, new Dictionary<MemberInfo, object>()));
                //testRecording = !testRecording;
            }
			if (fluxCapacitor.savedState != null && Input.GetKeyDown(KeyCode.F3)) { // Load State
				Debug.Log("Load (" + fluxCapacitor.savedState.Value.networkFrame + ")");
                FluxStateTracker.LoadGameState(fluxCapacitor.savedState.Value);
                fluxCapacitor.PlayerManager.Initialize(fluxCapacitor.savedState.Value.networkFrame);

                //UFE ufeInstance = this;
                //ufeInstance = RecordVar.LoadStateTrackers(ufeInstance, dictionaryList[dictionaryList.Count - 1]) as UFE;
                //p1ControlsScript.MoveSet.MecanimControl.Refresh();
                //p2ControlsScript.MoveSet.MecanimControl.Refresh();
            }
        }
#endif
    }

#if UNITY_EDITOR
    private void OnGUI() {
        if (config.debugOptions.stateTrackerTest && gameRunning)
        {
            if (GUI.Button(new Rect(10, 10, 160, 40), "Save State"))
            {
                Debug.Log("Save (" + currentFrame + ")");
                fluxCapacitor.savedState = FluxStateTracker.SaveGameState(currentFrame);

                //Debug.Log(GetPlayer1ControlsScript().MoveSet.GetCurrentClipFrame());
            }

            if (GUI.Button(new Rect(10, 60, 160, 40), "Load State"))
            {
                Debug.Log("Load (" + fluxCapacitor.savedState.Value.networkFrame + ")");
                FluxStateTracker.LoadGameState(fluxCapacitor.savedState.Value);
                fluxCapacitor.PlayerManager.Initialize(fluxCapacitor.savedState.Value.networkFrame);

                //Debug.Log(GetPlayer1ControlsScript().MoveSet.GetCurrentClipFrame());
            }
        }
    }
#endif

    //public List<Dictionary<System.Reflection.MemberInfo, System.Object>> dictionaryList = new List<Dictionary<System.Reflection.MemberInfo, System.Object>>();
    //public bool testRecording = false;

    protected void FixedUpdate(){
		if (fluxCapacitor != null){
			fluxCapacitor.DoFixedUpdate();

            /*if (testRecording)
            {
                dictionaryList.Add(RecordVar.SaveStateTrackers(this, new Dictionary<MemberInfo, object>()));
                if (dictionaryList.Count > 30) dictionaryList.RemoveAt(0);
            }*/
        }
	}
    
	protected void OnApplicationQuit(){
		closing = true;
		EnsureNetworkDisconnection();
	}
#endregion

#region protected instance methods: Network Events
	public static bool isConnected{
		get{
			return multiplayerAPI.IsConnected() && multiplayerAPI.Connections > 0;
		}
	}

	public static void EnsureNetworkDisconnection(){
		if (!disconnecting){
			NetworkState state = multiplayerAPI.GetConnectionState();

			if (state == NetworkState.Client){
				RemoveNetworkEventListeners();
				multiplayerAPI.DisconnectFromMatch();
			}else if (state == NetworkState.Server){
				RemoveNetworkEventListeners();
				multiplayerAPI.DestroyMatch();
			}
		}
    }

	protected static void AddNetworkEventListeners(){
		multiplayerAPI.OnDisconnection -= OnDisconnectedFromServer;
		multiplayerAPI.OnJoined -= OnJoined;
		multiplayerAPI.OnJoinError -= OnJoinError;
		multiplayerAPI.OnPlayerConnectedToMatch -= OnPlayerConnectedToMatch;
		multiplayerAPI.OnPlayerDisconnectedFromMatch -= OnPlayerDisconnectedFromMatch;
		multiplayerAPI.OnMatchesDiscovered -= OnMatchesDiscovered;
		multiplayerAPI.OnMatchDiscoveryError -= OnMatchDiscoveryError;
		multiplayerAPI.OnMatchCreated -= OnMatchCreated;
		multiplayerAPI.OnMatchDestroyed -= OnMatchDestroyed;

		multiplayerAPI.OnDisconnection += OnDisconnectedFromServer;
		multiplayerAPI.OnJoined += OnJoined;
		multiplayerAPI.OnJoinError += OnJoinError;
		multiplayerAPI.OnPlayerConnectedToMatch += OnPlayerConnectedToMatch;
		multiplayerAPI.OnPlayerDisconnectedFromMatch += OnPlayerDisconnectedFromMatch;
		multiplayerAPI.OnMatchesDiscovered += OnMatchesDiscovered;
		multiplayerAPI.OnMatchDiscoveryError += OnMatchDiscoveryError;
		multiplayerAPI.OnMatchCreated += OnMatchCreated;
		multiplayerAPI.OnMatchDestroyed += OnMatchDestroyed;
	}

	protected static void RemoveNetworkEventListeners(){
		multiplayerAPI.OnDisconnection -= OnDisconnectedFromServer;
		multiplayerAPI.OnJoined -= OnJoined;
		multiplayerAPI.OnJoinError -= OnJoinError;
		multiplayerAPI.OnPlayerConnectedToMatch -= OnPlayerConnectedToMatch;
		multiplayerAPI.OnPlayerDisconnectedFromMatch -= OnPlayerDisconnectedFromMatch;
		multiplayerAPI.OnMatchesDiscovered -= OnMatchesDiscovered;
		multiplayerAPI.OnMatchDiscoveryError -= OnMatchDiscoveryError;
		multiplayerAPI.OnMatchCreated -= OnMatchCreated;
		multiplayerAPI.OnMatchDestroyed -= OnMatchDestroyed;
	}

	protected static void OnJoined(MultiplayerAPI.JoinedMatchInformation match){
		if (config.debugOptions.connectionLog) Debug.Log("Connected to server");
		StartNetworkGame(0.5f, 2, false);
	}

	protected static void OnDisconnectedFromServer() {
        if (config.debugOptions.connectionLog) Debug.Log("Disconnected from server");
		fluxCapacitor.Initialize(); // Return to single player controls

		if (!closing){
			disconnecting = true;
			Application.runInBackground = config.runInBackground;

			if (config.lockInputs && currentScreen == null){
				DelayLocalAction(StartConnectionLostScreenIfMainMenuNotLoaded, 1f);
			}else{
				StartConnectionLostScreen();
			}
		}
	}

	protected static void OnJoinError() {
        if (config.debugOptions.connectionLog) Debug.Log("Could not connect to server");
		Application.runInBackground = config.runInBackground;
		StartConnectionLostScreen();
	}

	protected static void OnMatchCreated(MultiplayerAPI.CreatedMatchInformation match){}

	protected static void OnMatchDestroyed(){}

	protected static void OnMatchJoined(JoinMatchResponse response){}

	protected static void OnMatchDropped(){}

	protected static void OnPlayerConnectedToMatch(MultiplayerAPI.PlayerInformation player) {
		if (config.debugOptions.connectionLog){
			if (player.networkIdentity != null){
				Debug.Log("Connection: " + player.networkIdentity.connectionToClient);
			}else{
				Debug.Log("Player connected: " + player.photonPlayer);
			}
		}

		StartNetworkGame(0.5f, 1, false);
	}

	protected static void OnPlayerDisconnectedFromMatch(MultiplayerAPI.PlayerInformation player) {
        if (config.debugOptions.connectionLog) Debug.Log("Clean up after player " + player);
		fluxCapacitor.Initialize(); // Return to single player controls

		if (!closing){
			disconnecting = true;
			Application.runInBackground = config.runInBackground;

			if (config.lockInputs && currentScreen == null){
				DelayLocalAction(StartConnectionLostScreenIfMainMenuNotLoaded, 1f);
			}else{
				StartConnectionLostScreen();
			}
		}
	}

	protected static void OnServerInitialized() {
        if (config.debugOptions.connectionLog) Debug.Log("Server initialized and ready");
		Application.runInBackground = true;
		disconnecting = false;
	}
#endregion

#region private class methods: GUI Related methods
    public static Text DebuggerText(string dName, string dText, Vector2 position, TextAnchor alignment)
    {
        GameObject debugger = new GameObject(dName);
        debugger.transform.SetParent(canvas.transform);

        RectTransform trans = debugger.AddComponent<RectTransform>();
        trans.anchoredPosition = position;

        Text debuggerText = debugger.AddComponent<Text>();
        debuggerText.text = dText;
        debuggerText.alignment = alignment;
        debuggerText.color = Color.black;
        debuggerText.fontStyle = FontStyle.Bold;

        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        debuggerText.font = ArialFont;
        debuggerText.fontSize = 24;
        debuggerText.verticalOverflow = VerticalWrapMode.Overflow;
        debuggerText.horizontalOverflow = HorizontalWrapMode.Overflow;
        debuggerText.material = ArialFont.material;

        //Outline debuggerTextOutline = debugger.AddComponent<Outline>();
        //debuggerTextOutline.effectColor = Color.white;

        return debuggerText;
    }

    public static void GoToNetworkGameScreen(){
		if (multiplayerMode == MultiplayerMode.Bluetooth){
			StartBluetoothGameScreen();
		}else{
			StartNetworkGameScreen();
		}
	}

	public static void GoToNetworkGameScreen(float fadeTime){
		if (multiplayerMode == MultiplayerMode.Bluetooth){
			StartBluetoothGameScreen(fadeTime);
		}else{
			StartNetworkGameScreen(fadeTime);
		}
    }

	private static void _StartBluetoothGameScreen(float fadeTime){
		EnsureNetworkDisconnection();

		HideScreen(currentScreen);
		if (config.gameGUI.bluetoothGameScreen == null){
			Debug.LogError("Bluetooth Game Screen not found! Make sure you have set the prefab correctly in the Global Editor");
		}else if (isNetworkAddonInstalled){
            ShowScreen(config.gameGUI.bluetoothGameScreen);
            if (!config.gameGUI.bluetoothGameScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}else{
			Debug.LogWarning("Network Addon not found!");
		}
	}

	private static void _StartCharacterSelectionScreen(float fadeTime){
		HideScreen(currentScreen);
		if (config.gameGUI.characterSelectionScreen == null){
			Debug.LogError("Character Selection Screen not found! Make sure you have set the prefab correctly in the Global Editor");
		}else{
            ShowScreen(config.gameGUI.characterSelectionScreen);
            if (!config.gameGUI.characterSelectionScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

	private static void _StartIntroScreen(float fadeTime){
		EnsureNetworkDisconnection();

		HideScreen(currentScreen);
		if (config.gameGUI.introScreen == null){
			//Debug.Log("Intro Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			_StartMainMenuScreen(fadeTime);
		}else{
            ShowScreen(config.gameGUI.introScreen);
            if (!config.gameGUI.introScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
        }
	}

	public static void DanMainScreen()
	{
		HideScreen(currentScreen);
		PauseGame(false);
		//StartMainMenuScreen();
		StartStoryModeGameOverScreen();
	}

	private static void _StartMainMenuScreen(float fadeTime){
		EnsureNetworkDisconnection();
		//Debug.Log(currentScreen.name);
		HideScreen(currentScreen);
		if (config.gameGUI.mainMenuScreen == null){
			Debug.LogError("Main Menu Screen not found! Make sure you have set the prefab correctly in the Global Editor");
		}else{
			Debug.Log("_StartMainMenuScreen: "+fadeTime);
            ShowScreen(config.gameGUI.mainMenuScreen);
            if (!config.gameGUI.mainMenuScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

	private static void _StartStageSelectionScreen(float fadeTime){
		HideScreen(currentScreen);
		if (config.gameGUI.stageSelectionScreen == null){
			Debug.LogError("Stage Selection Screen not found! Make sure you have set the prefab correctly in the Global Editor");
		}else{
            ShowScreen(config.gameGUI.stageSelectionScreen);
            if (!config.gameGUI.stageSelectionScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}
	
	private static void _StartCreditsScreen(float fadeTime){
		HideScreen(currentScreen);
		if (config.gameGUI.creditsScreen == null){
			Debug.Log("Credits screen not found! Make sure you have set the prefab correctly in the Global Editor");
		}else{
            ShowScreen(config.gameGUI.creditsScreen);
            if (!config.gameGUI.creditsScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

	private static void _StartConnectionLostScreen(float fadeTime){
		EnsureNetworkDisconnection();

		HideScreen(currentScreen);
		if (config.gameGUI.connectionLostScreen == null){
			Debug.LogError("Connection Lost Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			_StartMainMenuScreen(fadeTime);
		}else if (isNetworkAddonInstalled){
            ShowScreen(config.gameGUI.connectionLostScreen);
            if (!config.gameGUI.connectionLostScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}else{
			Debug.LogWarning("Network Addon not found!");
			_StartMainMenuScreen(fadeTime);
		}
	}

	private static void _StartGame(float fadeTime){
		HideScreen(currentScreen);
		if (config.gameGUI.battleGUI == null){
			Debug.LogError("Battle GUI not found! Make sure you have set the prefab correctly in the Global Editor");
			battleGUI = new GameObject("BattleGUI").AddComponent<UFEScreen>();
		}else{
			battleGUI = (UFEScreen) GameObject.Instantiate(config.gameGUI.battleGUI);
        }
        if (!battleGUI.hasFadeIn) fadeTime = 0;
        CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);

		battleGUI.transform.SetParent(canvas != null ? canvas.transform : null, false);
        battleGUI.OnShow();
        canvasGroup.alpha = 0;
		
		gameEngine = new GameObject("Game");
		cameraScript = gameEngine.AddComponent<CameraScript>();

		if (config.player1Character == null){
			Debug.LogError("No character selected for player 1.");
			return;
		}
		if (config.player2Character == null){
			Debug.LogError("No character selected for player 2.");
			return;
		}
		if (config.selectedStage == null){
			Debug.LogError("No stage selected.");
			return;
		}
		
		if (config.aiOptions.engine == AIEngine.FuzzyAI){
			SetFuzzyAI(1, config.player1Character);
            SetFuzzyAI(2, config.player2Character);
        } else {
            SetRandomAI(1);
            SetRandomAI(2);
        }
		
		config.player1Character.currentLifePoints = (Fix64)config.player1Character.lifePoints;
		config.player2Character.currentLifePoints = (Fix64)config.player2Character.lifePoints;
		config.player1Character.currentGaugePoints = 0;
		config.player2Character.currentGaugePoints = 0;

        GameObject stageInstance = null;
        if (config.stagePrefabStorage == StorageMode.Legacy) {
            if (config.selectedStage.prefab != null) {
                stageInstance = (GameObject)Instantiate(config.selectedStage.prefab);
                stageInstance.transform.parent = gameEngine.transform;
            } else {
                Debug.LogError("Stage prefab not found! Make sure you have set the prefab correctly in the Global Editor.");
            }
        } else {
            GameObject prefab = Resources.Load<GameObject>(config.selectedStage.stageResourcePath);

            if (prefab != null) {
                stageInstance = (GameObject)GameObject.Instantiate(prefab);
                stageInstance.transform.parent = gameEngine.transform;
            } else {
                Debug.LogError("Stage prefab not found! Make sure the prefab is correctly located under the Resources folder and the path is written correctly.");
            }
        }


        config.currentRound = 1;
		config.lockInputs = true;
		SetTimer(config.roundOptions._timer);
		PauseTimer();

        // Initialize Player 1 Character
		GameObject p1 = new GameObject("Player1");
		p1.transform.parent = gameEngine.transform;
        p1ControlsScript = p1.AddComponent<ControlsScript>();
        p1ControlsScript.Physics = p1.AddComponent<PhysicsScript>();
        p1ControlsScript.myInfo = (UFE3D.CharacterInfo)Instantiate(config.player1Character);

        config.player1Character = p1ControlsScript.myInfo;
        p1ControlsScript.myInfo.playerNum = 1;
        if (isControlFreak2Installed && p1ControlsScript.myInfo.customControls.overrideControlFreak && p1ControlsScript.myInfo.customControls.controlFreak2Prefab != null) {
            controlFreakPrefab = (GameObject)Instantiate(p1ControlsScript.myInfo.customControls.controlFreak2Prefab.gameObject);
            touchControllerBridge = (controlFreakPrefab != null) ? controlFreakPrefab.GetComponent<InputTouchControllerBridge>() : null;
            touchControllerBridge.Init();
        }

        // Initialize Player 2 Character
		GameObject p2 = new GameObject("Player2");
		p2.transform.parent = gameEngine.transform;
        p2ControlsScript = p2.AddComponent<ControlsScript>();
        p2ControlsScript.Physics = p2.AddComponent<PhysicsScript>();
        p2ControlsScript.myInfo = (UFE3D.CharacterInfo)Instantiate(config.player2Character);
        config.player2Character = p2ControlsScript.myInfo;
        p2ControlsScript.myInfo.playerNum = 2;


        // If the same character is selected, try loading the alt costume
        if (config.player1Character.name == config.player2Character.name) {
            if (config.player2Character.alternativeCostumes.Length > 0) {
                config.player2Character.isAlt = true;
                config.player2Character.selectedCostume = 0;
                
                if (config.player2Character.alternativeCostumes[0].characterPrefabStorage == StorageMode.Legacy) {
                    p2ControlsScript.myInfo.characterPrefab = config.player2Character.alternativeCostumes[0].prefab;
                } else {
                    p2ControlsScript.myInfo.characterPrefab = Resources.Load<GameObject>(config.player2Character.alternativeCostumes[0].prefabResourcePath);
                }
            }
        }

        // Initialize Debuggers
        debugger1 = DebuggerText("Debugger1", "", new Vector2(- Screen.width + 50, Screen.height - 180), TextAnchor.UpperLeft);
        debugger2 = DebuggerText("Debugger2", "", new Vector2(Screen.width - 50, Screen.height - 180), TextAnchor.UpperRight);
        p1ControlsScript.debugger = debugger1;
        p2ControlsScript.debugger = debugger2;
        debugger1.enabled = debugger2.enabled = config.debugOptions.debugMode;


        //fluxGameManager.Initialize(currentFrame);
        fluxCapacitor.savedState = null;
        PauseGame(false);
    }

    //Preloader
    public static void PreloadBattle() {
        PreloadBattle((float)config._preloadingTime);
    }

    public static void PreloadBattle(float warmTimer) {
        if (config.preloadHitEffects) {
            SearchAndCastGameObject(config.hitOptions, warmTimer);
            SearchAndCastGameObject(config.groundBounceOptions, warmTimer);
            SearchAndCastGameObject(config.wallBounceOptions, warmTimer);
            if (config.debugOptions.preloadedObjects) Debug.Log("Hit Effects Loaded");
        }
        if (config.preloadStage) {
            SearchAndCastGameObject(config.selectedStage, warmTimer);
            if (config.debugOptions.preloadedObjects) Debug.Log("Stage Loaded");
        }
        if (config.preloadCharacter1) {
            SearchAndCastGameObject(config.player1Character, warmTimer);
            if (config.debugOptions.preloadedObjects) Debug.Log("Character 1 Loaded");
        }
        if (config.preloadCharacter2) {
            SearchAndCastGameObject(config.player2Character, warmTimer);
            if (config.debugOptions.preloadedObjects) Debug.Log("Character 2 Loaded");
        }
        if (config.warmAllShaders) Shader.WarmupAllShaders();

        memoryDump.Clear();
    }

    public static void SearchAndCastGameObject(object target, float warmTimer) {
        if (target != null) {
            Type typeSource = target.GetType();
            FieldInfo[] fields = typeSource.GetFields();

            foreach (FieldInfo field in fields) {
                object fieldValue = field.GetValue(target);
                if (fieldValue == null || fieldValue.Equals(null)) continue;
                if (memoryDump.Contains(fieldValue)) continue;
                memoryDump.Add(fieldValue);

                if (field.FieldType.Equals(typeof(GameObject))) {
                    if (config.debugOptions.preloadedObjects) Debug.Log(fieldValue + " preloaded");
                    GameObject tempGO = (GameObject)Instantiate((GameObject)fieldValue);
                    tempGO.transform.position = new Vector2(-999, -999);

                    //Light lightComponent = tempGO.GetComponent<Light>();
                    //if (lightComponent != null) lightComponent.enabled = false;

                    Destroy(tempGO, warmTimer);

                } else if (field.FieldType.IsArray && !field.FieldType.GetElementType().IsEnum) {
                    object[] fieldValueArray = (object[])fieldValue;
                    foreach (object obj in fieldValueArray) {
                        SearchAndCastGameObject(obj, warmTimer);
                    }
                }
            }
        }
    }

	private static void _StartHostGameScreen(float fadeTime){
		EnsureNetworkDisconnection();

		HideScreen(currentScreen);
		if (config.gameGUI.hostGameScreen == null){
			Debug.LogError("Host Game Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			_StartMainMenuScreen(fadeTime);
		}else if (isNetworkAddonInstalled){
            ShowScreen(config.gameGUI.hostGameScreen);
            if (!config.gameGUI.hostGameScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}else{
			Debug.LogWarning("Network Addon not found!");
			_StartMainMenuScreen(fadeTime);
		}
	}

	private static void _StartJoinGameScreen(float fadeTime){
		EnsureNetworkDisconnection();

		HideScreen(currentScreen);
		if (config.gameGUI.joinGameScreen == null){
			Debug.LogError("Join To Game Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			_StartMainMenuScreen(fadeTime);
		}else if (isNetworkAddonInstalled){
            ShowScreen(config.gameGUI.joinGameScreen);
            if (!config.gameGUI.joinGameScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}else{
			Debug.LogWarning("Network Addon not found!");
			_StartMainMenuScreen(fadeTime);
		}
	}
	
	private static void _StartLoadingBattleScreen(float fadeTime){
        config.lockInputs = true;

		HideScreen(currentScreen);
		if (config.gameGUI.loadingBattleScreen == null){
			Debug.Log("Loading Battle Screen not found! Make sure you have set the prefab correctly in the Global Editor");
            _StartGame((float)config.gameGUI.gameFadeDuration);
		}else{
            ShowScreen(config.gameGUI.loadingBattleScreen);
            if (!config.gameGUI.loadingBattleScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

	private static void _StartSearchMatchScreen(float fadeTime){
		//EnsureNetworkDisconnection();

		HideScreen(currentScreen);
		if (config.gameGUI.searchMatchScreen == null){
			Debug.LogError("Random Match Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			_StartMainMenuScreen(fadeTime);
		}else if (isNetworkAddonInstalled){
			//AddNetworkEventListeners();
            ShowScreen(config.gameGUI.searchMatchScreen);
            if (!config.gameGUI.searchMatchScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}else{
			Debug.LogWarning("Network Addon not found!");
			_StartMainMenuScreen(fadeTime);
		}
	}

	private static void _StartNetworkGameScreen(float fadeTime){
		EnsureNetworkDisconnection();

		HideScreen(currentScreen);
		if (config.gameGUI.networkGameScreen == null){
			Debug.LogError("Network Game Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			_StartMainMenuScreen(fadeTime);
		}else if (isNetworkAddonInstalled){
            ShowScreen(config.gameGUI.networkGameScreen);
            if (!config.gameGUI.networkGameScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}else{
			Debug.LogWarning("Network Addon not found!");
			_StartMainMenuScreen(fadeTime);
		}
	}

	private static void _StartOptionsScreen(float fadeTime){

		HideScreen(currentScreen);
		if (config.gameGUI.optionsScreen == null){
			Debug.LogError("Options Screen not found! Make sure you have set the prefab correctly in the Global Editor");
		}else{
            ShowScreen(config.gameGUI.optionsScreen);
            if (!config.gameGUI.optionsScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

	public static void _StartStoryModeBattle(float fadeTime){
		// If the player 1 won the last battle, load the information of the next battle. 
		// Otherwise, repeat the last battle...
		UFE3D.CharacterInfo character = GetPlayer(1);

		if (player1WonLastBattle){
			// If the player 1 won the last battle...
			if (storyMode.currentGroup < 0){
				// If we haven't fought any battle, raise the "Story Mode Started" event...
				if (OnStoryModeStarted != null){
					OnStoryModeStarted(character);
				}

				// And start with the first battle of the first group
				storyMode.currentGroup = 0;
				storyMode.currentBattle = 0;
			}else if (storyMode.currentGroup >= 0 && storyMode.currentGroup < storyMode.characterStory.fightsGroups.Length){
				// Otherwise, check if there are more remaining battles in the current group
				FightsGroup currentGroup = storyMode.characterStory.fightsGroups[storyMode.currentGroup];
				int numberOfFights = currentGroup.maxFights;
				
				if (currentGroup.mode != FightsGroupMode.FightAgainstSeveralOpponentsInTheGroupInRandomOrder){
					numberOfFights = currentGroup.opponents.Length;
				}
				
				if (storyMode.currentBattle < numberOfFights - 1){
					// If there are more battles in the current group, go to the next battle...
					++storyMode.currentBattle;
				}else{
					// Otherwise, go to the next group of battles...
					++storyMode.currentGroup;
					storyMode.currentBattle = 0;
					storyMode.defeatedOpponents.Clear();
				}
			}

			// If the player hasn't finished the game...
			storyMode.currentBattleInformation = null;
			while (
				storyMode.currentBattleInformation == null &&
				storyMode.currentGroup >= 0 && 
				storyMode.currentGroup < storyMode.characterStory.fightsGroups.Length
			){
				// Try to retrieve the information of the next battle
				FightsGroup currentGroup = storyMode.characterStory.fightsGroups[storyMode.currentGroup];
				storyMode.currentBattleInformation = null;
				
				if (currentGroup.mode == FightsGroupMode.FightAgainstAllOpponentsInTheGroupInTheDefinedOrder){
					StoryModeBattle b = currentGroup.opponents[storyMode.currentBattle];
					UFE3D.CharacterInfo opponent = config.characters[b.opponentCharacterIndex];

					if (storyMode.canFightAgainstHimself || !character.characterName.Equals(opponent.characterName)){
						storyMode.currentBattleInformation = b;
					}else{
						// Otherwise, check if there are more remaining battles in the current group
						int numberOfFights = currentGroup.maxFights;
						
						if (currentGroup.mode != FightsGroupMode.FightAgainstSeveralOpponentsInTheGroupInRandomOrder){
							numberOfFights = currentGroup.opponents.Length;
						}
						
						if (storyMode.currentBattle < numberOfFights - 1){
							// If there are more battles in the current group, go to the next battle...
							++storyMode.currentBattle;
						}else{
							// Otherwise, go to the next group of battles...
							++storyMode.currentGroup;
							storyMode.currentBattle = 0;
							storyMode.defeatedOpponents.Clear();
						}
					}
				}else{
					List<StoryModeBattle> possibleBattles = new List<StoryModeBattle>();
					
					foreach (StoryModeBattle b in currentGroup.opponents){
						if (!storyMode.defeatedOpponents.Contains(b.opponentCharacterIndex)){
							UFE3D.CharacterInfo opponent = config.characters[b.opponentCharacterIndex];
							
							if (storyMode.canFightAgainstHimself || !character.characterName.Equals(opponent.characterName)){
								possibleBattles.Add(b);
							}
						}
					}
					
					if (possibleBattles.Count > 0){
						int index = UnityEngine.Random.Range(0, possibleBattles.Count);
						storyMode.currentBattleInformation = possibleBattles[index];
					}else{
						// If we can't find a valid battle in this group, try moving to the next group
						++storyMode.currentGroup;
					}
				}
			}
		}

		if (storyMode.currentBattleInformation != null){
			// If we could retrieve the battle information, load the opponent and the stage
			int characterIndex = storyMode.currentBattleInformation.opponentCharacterIndex;
			SetPlayer2(config.characters[characterIndex]);

			if (player1WonLastBattle){
				lastStageIndex = UnityEngine.Random.Range(0, storyMode.currentBattleInformation.possibleStagesIndexes.Count);
			}

			SetStage(config.stages[storyMode.currentBattleInformation.possibleStagesIndexes[lastStageIndex]]);
			
			// Finally, check if we should display any "Conversation Screen" before the battle
			_StartStoryModeConversationBeforeBattleScreen(storyMode.currentBattleInformation.conversationBeforeBattle, fadeTime);
		}else{
			// Otherwise, show the "Congratulations" Screen
			if (OnStoryModeCompleted != null){
				OnStoryModeCompleted(character);
			}

			_StartStoryModeCongratulationsScreen(fadeTime);
		}
	}

	private static void _StartStoryModeCongratulationsScreen(float fadeTime){
		HideScreen(currentScreen);
		if (config.gameGUI.storyModeCongratulationsScreen == null){
			Debug.Log("Congratulations Screen not found! Make sure you have set the prefab correctly in the Global Editor");
            _StartStoryModeEndingScreen(fadeTime);
		}else{
            ShowScreen(config.gameGUI.storyModeCongratulationsScreen, delegate() { StartStoryModeEndingScreen(fadeTime); });
            if (!config.gameGUI.storyModeCongratulationsScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

	private static void _StartStoryModeContinueScreen(float fadeTime){
		HideScreen(currentScreen);
		if (config.gameGUI.storyModeContinueScreen == null){
			Debug.Log("Continue Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			_StartMainMenuScreen(fadeTime);
		}else{
            ShowScreen(config.gameGUI.storyModeContinueScreen);
            if (!config.gameGUI.storyModeContinueScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

    private static void _StartStoryModeConversationAfterBattleScreen(UFEScreen conversationScreen, float fadeTime) {
        HideScreen(currentScreen);
		if (conversationScreen != null){
            ShowScreen(conversationScreen, delegate() { StartStoryModeBattle(fadeTime); });
            if (!conversationScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}else{
			_StartStoryModeBattle(fadeTime);
		}
	}

    private static void _StartStoryModeConversationBeforeBattleScreen(UFEScreen conversationScreen, float fadeTime) {
        HideScreen(currentScreen);
		if (conversationScreen != null){
            ShowScreen(conversationScreen, delegate() { StartLoadingBattleScreen(fadeTime); });
            if (!conversationScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}else{
			_StartLoadingBattleScreen(fadeTime);
		}
	}

	private static void _StartStoryModeEndingScreen(float fadeTime){
		HideScreen(currentScreen);
		if (storyMode.characterStory.ending == null){
			Debug.Log("Ending Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			_StartCreditsScreen(fadeTime);
		}else{
            ShowScreen(storyMode.characterStory.ending, delegate() { StartCreditsScreen(fadeTime); });
            if (!storyMode.characterStory.ending.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

	private static void _StartStoryModeGameOverScreen(float fadeTime){
		HideScreen(currentScreen);
		if (config.gameGUI.storyModeGameOverScreen == null){
			Debug.Log("Game Over Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			_StartMainMenuScreen(fadeTime);
		}else{
            ShowScreen(config.gameGUI.storyModeGameOverScreen, delegate() { StartMainMenuScreen(fadeTime); });
            if (!config.gameGUI.storyModeGameOverScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

	private static void _StartStoryModeOpeningScreen(float fadeTime){
		HideScreen(currentScreen);
		if (storyMode.characterStory.opening == null){
			Debug.Log("Opening Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			_StartStoryModeBattle(fadeTime);
		}else{
            ShowScreen(storyMode.characterStory.opening, delegate() { StartStoryModeBattle(fadeTime); });
            if (!storyMode.characterStory.opening.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

	private static void _StartVersusModeScreen(float fadeTime){
		HideScreen(currentScreen);
		if (config.gameGUI.versusModeScreen == null){
			Debug.Log("Versus Mode Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			StartPlayerVersusPlayer(fadeTime);
		}else{
            ShowScreen(config.gameGUI.versusModeScreen);
            if (!config.gameGUI.versusModeScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}

	private static void _StartVersusModeAfterBattleScreen(float fadeTime){
		HideScreen(currentScreen);
		if (config.gameGUI.versusModeAfterBattleScreen == null){
			Debug.Log("Versus Mode \"After Battle\" Screen not found! Make sure you have set the prefab correctly in the Global Editor");
			
			_StartMainMenuScreen(fadeTime);
		}else{
            ShowScreen(config.gameGUI.versusModeAfterBattleScreen);
            if (!config.gameGUI.versusModeAfterBattleScreen.hasFadeIn) fadeTime = 0;
            CameraFade.StartAlphaFade(config.gameGUI.screenFadeColor, true, fadeTime);
		}
	}
#endregion
}