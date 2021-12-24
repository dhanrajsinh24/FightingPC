// Copyright (c) 2017 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#pragma warning disable 0649

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rewired.Integration.UniversalFightingEngine {
    using UFE3D;

    // Notes about convoluted EventSystem handling in this script:
    //
    // UFE's EventSystem handling is very complicated and non-standard.
    // UFE will create a StandaloneInputModule in Awake, and this in turn will create an EventSystem.
    // It will set its global UFE.eventSystem property in Start by setting it from EventSystem.current.
    // If some other EventSystem was added to the scene and it executes after UFE's Awake (this order cannot be known),
    // it will replace EventSystem.current and UFE will use that for its primary event system.
    // However, UFE will still have created an EventSystem in Awake, so Unity will spam warnings about having
    // multiple event systems in the scene.
    //
    // UFE uses a non-standard UI navigation system that gets the EventSystem.current.selectedGameObject directly and invokes
    // navigation and click/submit commands on these Selectables. This conflicts with the standard way of using the StandaloneInputModule
    // to handle navigation by creating events to be sent through the EventSystem and letting the Selectables determine navigation.
    // This has many implications, one of which is that as long as UFE's UFE.eventSystem is enabled, it will perform its own custom navigation commands.
    // This conflicts directly with Rewired. Rewired uses the standard methods of UI navigation, so when Control Mapper is open, if the same EventSystem
    // is being used for Control Mapper that is also set as the UFE.eventSystem, UFE will cause double navigation and button events
    // to happen because it's processing its own custom navigation while the InputModule is also creating its own navigation events.
    // Trying to get these two systems to work together is a huge problem. Further compounding the issues, for mouse and touch input to work,
    // a StandaloneInputModule must always exist, be enabled, and be on the same GameObject as the EventSystem that is currently enabled.
    // Because of UFE's custom navigation system, it is impossible to use a single EventSystem to handle both the needs of UFE and Rewired.
    //
    // Because of all these issues, it is required that multiple EventSystems exist in the scene for separate purposes.
    // UFE needs its own EventSystem which it can manage enabled state on and can do its non-standard navigation through.
    // Rewired needs its own EventSystem which it can use to control Control Mapper.
    // Mouse and touch input must be handled regardless of which EventSystem is currently in control, which means
    // there needs to be two InputModules (StandaloneInputModule created by UFE and RewiredStandaloneInputModule created by Rewired.)
    // These systems cannot run concurrently and must be switched on and off as needed. This also has the negative side effect that
    // The StandaloneInputModule used by UFE for navigation does not have the same capabilities of the RewiredStandaloneInputModule.
    // Some features of the RewiredStandaloneInputModule cannot be used with UFE such as PlayerMouse software mouse cursors.
    //
    // Eliminating these extremely roundabout workarounds would require a redesign of UFE's navigation handling to work in a more standard way.
    // At the very least, there needs to be a way to toggle on/off UFE navigation control that is accessible in a public variable so 
    // Rewired could disable UFE's navigation system when Control Mapper is open. This would make it possible to use a single EventSystem for the
    // project, though this would still require roundable workarounds to delete the EventSystem and StandaloneInput UFE creates on Awake and replace
    // them. UFE's initialization procedure should be reworked so it does not create the StandaloneInputModule (and implicitly the EventSystem) in Awake
    // so the detection of use of an external EventSystem prevents the creation of one by UFE.
    //
    // Fixing these issues is outside the scope of what an integration can do.
    // This integration script is prone to breakage with changes to UFE and/or the Unity UI code.

    /// <summary>
    /// This cannot be used with Universal Fighting Engine 1.x.
    /// This integration is compatible only with UFE2.
    /// </summary>
    public class RewiredUFEInputManager : MonoBehaviour, RewiredInputController.IInputSource, RewiredInputController.ITouchInputUI, RewiredInputController.IInputConfiguration {

        private const string className = "RewiredUFEInputManager";

        #region Variables

        [Tooltip("Link the Rewired Event System here.")]
        [SerializeField]
        private EventSystem _eventSystem;

        [Header("Control Mapper")]
        [Tooltip("Link Control Mapper here if you're using it.")]
        [SerializeField]
        private Rewired.UI.ControlMapper.ControlMapper _controlMapper;

        [Tooltip("Actions to disable while Control Mapper is open.")]
        [SerializeField]
        private string[] _actionsToDisableIfControlMapperOpen = new string[] {
            "Start"
        };

        [Header("Touch Controls")]
        [Tooltip("Link the touch controls GameObject here. This GameObject will be enabled and disabled when the touch UI is shown / hidden.")]
        [SerializeField]
        private GameObject _touchControls;

        [Tooltip("Should touch controls be hidden when a joystick is used?")]
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("_hideTouchControlsWhenJoysticksConnected")]
        private bool _hideTouchControlsWhenJoystickActive = true;

        [Tooltip("Show touch controls on the following platforms.")]
        [SerializeField]
        [Rewired.Utils.Attributes.BitmaskToggle(typeof(PlatformFlags))]
        private PlatformFlags _showTouchControlsOn = PlatformFlags.Android | PlatformFlags.IOS;

        [NonSerialized]
        private bool _initialized;
        [NonSerialized]
        private bool _autoTouchControlUI_isActive;
        [NonSerialized]
        private bool _touchControlsActiveExternal;
        [NonSerialized]
        private Action _inputConfigurationUIClosedCallback;
        [NonSerialized]
        private bool _ufeEventSystemEnabledPrev;

        #endregion

        #region RewiredInputController.IInputSource Implementation

        public float GetAxis(int playerId, string name) {
            if (!_initialized || !isEnabled) return 0f;
            return ReInput.players.GetPlayer(playerId).GetAxis(name);
        }

        public float GetAxisRaw(int playerId, string name) {
            if (!_initialized || !isEnabled) return 0f;
            return ReInput.players.GetPlayer(playerId).GetAxisRaw(name);
        }

        public bool GetButton(int playerId, string name) {
            if (!_initialized || !isEnabled) return false;
            return ReInput.players.GetPlayer(playerId).GetButton(name);
        }

        #endregion

        #region RewiredInputController.ITouchInputUI Implementation

        public bool showTouchControls {
            get {
                return _initialized &&
                    isEnabled &&
                    supportsTouchControls &&
                    _touchControlsActiveExternal &&
                    (_hideTouchControlsWhenJoystickActive ? _autoTouchControlUI_isActive : true);
            }
            set {
                _touchControlsActiveExternal = value;
                if (value && (!_initialized || !isEnabled || !supportsTouchControls)) return;
                EvaluateTouchControlsActive();
            }
        }

        #endregion

        #region RewiredInputController.IInputConfiguration Implementation

        public bool showInputConfigurationUI {
            get {
                if (_controlMapper == null) return false;
                return _controlMapper.isOpen;
            }
            set {
                if (_controlMapper == null) return;
                if (_controlMapper.isOpen == value) return;
                if (value) {
                    _controlMapper.Open();
                } else {
                    _controlMapper.Close(true);
                }
            }
        }

        public void ShowInputConfigurationUI(Action closedCallback) {
            if (_controlMapper == null) return;
            _inputConfigurationUIClosedCallback = closedCallback;
            showInputConfigurationUI = true;
        }

        #endregion

        #region Monobehaviour Callbacks

        private void Awake() {
            if (!ReInput.isReady) {
                Debug.LogError(className + ": Rewired is not initialized. You must have an active Rewired Input Manager in the scene.");
                return;
            }
        }

        private void Start() {
            if (_eventSystem == null) {
                //todo
                //_eventSystem = gameObject.GetComponentInChildren<EventSystem>(true);
                _eventSystem = UFE.eventSystem;
                if (_eventSystem == null) {
                    Debug.LogError(className + ": Event System must be linked in the inspector.");
                    return;
                }
            }

            // Rewired event system must start disabled so UFE doesn't see it and
            // try to use it as the primary event system for its custom navigation system.
            if (_eventSystem.enabled) {
                //Debug.LogError(className + ": Event System component must be disabled initially in the GameObject to prevent issues during UFE initialization.");
                return;
            }

            // Make sure navigation events are enabled because past releases defaulted to this being disabled in the inspector.
            _eventSystem.sendNavigationEvents = true;

            _initialized = true;
            if (!_initialized) return;

            // Show/hide the touch UI initially
            bool showTouchControls = supportsTouchControls;
            _touchControlsActiveExternal = showTouchControls;
            SetTouchControlsActive(showTouchControls);

            if (_hideTouchControlsWhenJoystickActive) {
                _autoTouchControlUI_isActive = showTouchControls;
                CheckHideTouchControlsWhenJoystickActive();
            }
        }

        private void OnEnable() {
            // Set up static references in UFE
            RewiredInputController.inputSource = this;
            if (_touchControls != null) RewiredInputController.touchInputUI = this;
            if (_controlMapper != null) RewiredInputController.inputConfiguration = this;

            // Subscribe to events
            ReInput.ControllerConnectedEvent += OnControllerConnected;
            ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
            if (_controlMapper != null) {
                _controlMapper.ScreenOpenedEvent += OnControlMapperOpened;
                _controlMapper.ScreenClosedEvent += OnControlMapperClosed;
            }
        }

        private void OnDisable() {
            // Remove static references in UFE
            if (RewiredInputController.inputSource == this as RewiredInputController.IInputSource) {
                RewiredInputController.inputSource = null;
            }
            if (RewiredInputController.touchInputUI == this as RewiredInputController.ITouchInputUI) {
                RewiredInputController.touchInputUI = null;
            }
            if (RewiredInputController.inputConfiguration == this as RewiredInputController.IInputConfiguration) {
                RewiredInputController.inputConfiguration = null;
            }

            // Unsubscribe from events
            ReInput.ControllerConnectedEvent -= OnControllerConnected;
            ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
            if (_controlMapper != null) {
                _controlMapper.ScreenOpenedEvent -= OnControlMapperOpened;
                _controlMapper.ScreenClosedEvent -= OnControlMapperClosed;
            }
        }

        void Update() {
            if (!_initialized) return;
            CheckHideTouchControlsWhenJoystickActive();
        }

        #endregion

        #region Private Properties

        private bool supportsTouchControls {
            get {
                if (_showTouchControlsOn == PlatformFlags.None) return false;
                if (_touchControls == null) return false;

                PlatformFlags p = _showTouchControlsOn;
                if (Rewired.Utils.UnityTools.isEditor && (p & PlatformFlags.Editor) != 0) return true;

                if (!UnityEngine.Input.touchSupported) return false;

                var platform = Rewired.Utils.UnityTools.platform;
                if (platform == Rewired.Platforms.Platform.Windows) return (p & PlatformFlags.Windows) != 0;
                if (platform == Rewired.Platforms.Platform.OSX) return (p & PlatformFlags.OSX) != 0;
                if (platform == Rewired.Platforms.Platform.Linux) return (p & PlatformFlags.Linux) != 0;
                if (platform == Rewired.Platforms.Platform.iOS) return (p & PlatformFlags.IOS) != 0;
                if (platform == Rewired.Platforms.Platform.tvOS) return (p & PlatformFlags.TVOS) != 0;
                if (platform == Rewired.Platforms.Platform.Android) return (p & PlatformFlags.Android) != 0;
                if (platform == Rewired.Platforms.Platform.AmazonFireTV) return (p & PlatformFlags.AmazonFireTV) != 0;
                if (platform == Rewired.Platforms.Platform.RazerForgeTV) return (p & PlatformFlags.RazerForgeTV) != 0;
                if (platform == Rewired.Platforms.Platform.Windows81Store || platform == Rewired.Platforms.Platform.WindowsAppStore) return (p & PlatformFlags.Windows8Store) != 0;
                if (platform == Rewired.Platforms.Platform.WindowsUWP) return (p & PlatformFlags.WindowsUWP10) != 0;
                if (platform == Rewired.Platforms.Platform.WebGL) return (p & PlatformFlags.WebGL) != 0;
                if (platform == Rewired.Platforms.Platform.PS4) return (p & PlatformFlags.PS4) != 0;
                if (platform == Rewired.Platforms.Platform.PSMobile || platform == Rewired.Platforms.Platform.PSVita) return (p & PlatformFlags.PSVita) != 0;
                if (platform == Rewired.Platforms.Platform.Xbox360) return (p & PlatformFlags.Xbox360) != 0;
                if (platform == Rewired.Platforms.Platform.XboxOne) return (p & PlatformFlags.XboxOne) != 0;
                if (platform == Rewired.Platforms.Platform.SamsungTV) return (p & PlatformFlags.SamsungTV) != 0;
                if (platform == Rewired.Platforms.Platform.WiiU) return (p & PlatformFlags.WiiU) != 0;
                if (platform == Rewired.Platforms.Platform.N3DS) return (p & PlatformFlags.Nintendo3DS) != 0;
                if (platform == Rewired.Platforms.Platform.Switch) return (p & PlatformFlags.Switch) != 0;
                if (platform == Rewired.Platforms.Platform.Stadia) return (p & PlatformFlags.Stadia) != 0;
                if (platform == Rewired.Platforms.Platform.GameCoreXboxOne) return (p & PlatformFlags.GameCoreXboxOne) != 0;
                if (platform == Rewired.Platforms.Platform.GameCoreScarlett) return (p & PlatformFlags.GameCoreScarlett) != 0;
                return (p & PlatformFlags.Unknown) != 0;
            }
        }

        private bool isEnabled { get { return this.isActiveAndEnabled; } }

        #endregion

        #region Private Methods

        private void CheckHideTouchControlsWhenJoystickActive() {
            if (!_hideTouchControlsWhenJoystickActive) return;

            Rewired.Player player = Rewired.ReInput.players.GetPlayer(0);

            if (_autoTouchControlUI_isActive) {
                // Disable touch controls if a joystick is active in Player 0
                for (int i = 0; i < player.controllers.joystickCount; i++) {
                    if (player.controllers.Joysticks[i].GetAnyButtonChanged()) {
                        _autoTouchControlUI_isActive = false;
                        break;
                    }
                }
            } else {
                // Enable touch controls if no joysticks are assigned to Player 0
                if (player.controllers.joystickCount == 0) {
                    _autoTouchControlUI_isActive = true;
                }
                // Enable touch controls if the screen is touched
                if (!_autoTouchControlUI_isActive) {
                    if (UnityEngine.Input.touchCount > 0) {
                        _autoTouchControlUI_isActive = true;
                    }
                }
            }

            EvaluateTouchControlsActive();
        }

        private void EvaluateTouchControlsActive() {
            // Set the active state on the touch controls based on the external state and the auto state
            // Only set active if both external and internal are active
            SetTouchControlsActive(showTouchControls);
        }

        private void SetTouchControlsActive(bool active) {
            if (_touchControls == null) return;
            if (_touchControls.activeInHierarchy == active) return;
            _touchControls.SetActive(active);
        }

        // Event Handlers

        private void OnControllerConnected(Rewired.ControllerStatusChangedEventArgs args) {
            if (!_initialized) return;
            if (args.controllerType != ControllerType.Joystick) return;
            CheckHideTouchControlsWhenJoystickActive();
        }

        private void OnControllerDisconnected(Rewired.ControllerStatusChangedEventArgs args) {
            if (!_initialized) return;
            if (args.controllerType != ControllerType.Joystick) return;
            CheckHideTouchControlsWhenJoystickActive();
        }

        private void OnControlMapperOpened() {
            if (!_initialized) return;

            // Disable the UFE event system because their custom UI navigation system causes tons of problems
            _ufeEventSystemEnabledPrev = UFE.eventSystem.enabled;
            UFE.eventSystem.enabled = false;

            // Enable Rewired event system so it can be used to by Control Mapper
            _eventSystem.enabled = true; // enable the Rewired Event System
            EventSystem.current = _eventSystem;

            // Disable Start action to prevent game from unpausing behind Control Mapper
            for (int i = 0; i < ReInput.players.playerCount; i++) {
                Player player = ReInput.players.Players[i];
                foreach (var map in player.controllers.maps.GetAllMaps()) {
                    foreach (var a in _actionsToDisableIfControlMapperOpen) {
                        map.ForEachElementMapMatch(x => x.actionId == ReInput.mapping.GetAction(a).id, x => x.enabled = false);
                    }
                }
            }
        }

        private void OnControlMapperClosed() {
            if (!_initialized) return;

            // Disable Rewired EventSystem
            _eventSystem.enabled = false;

            // Restore the UFE event system enabled state
            StartCoroutine(RestoreUFEEventSystemSettingsDelayed());

            // Invoke callback
            if (_inputConfigurationUIClosedCallback != null) {
                try {
                    _inputConfigurationUIClosedCallback();
                } catch (Exception ex) {
                    Debug.LogError(className + ": An exception occurred while invoking callback.\n" + ex);
                } finally {
                    _inputConfigurationUIClosedCallback = null; // clear the callback each time
                }
            }

            // Re-enable Start action
            for (int i = 0; i < ReInput.players.playerCount; i++) {
                Player player = ReInput.players.Players[i];
                foreach (var map in player.controllers.maps.GetAllMaps()) {
                    foreach (var a in _actionsToDisableIfControlMapperOpen) {
                        map.ForEachElementMapMatch(x => x.actionId == ReInput.mapping.GetAction(a).id, x => x.enabled = true);
                    }
                }
            }
            ReInput.userDataStore.Save(); // save data again so bindings are saved enabled
        }

        System.Collections.IEnumerator RestoreUFEEventSystemSettingsDelayed() {
            // Delay enabling the UFE Event System because it will pick up the key press
            // that closed Control Mapper and will re-open it again.
            // Wait two frames so button down event expires.
            yield return null;
            yield return null;
            UFE.eventSystem.enabled = _ufeEventSystemEnabledPrev;
        }

        #endregion

        #region Enums

        private enum PlatformFlags {
            None = 0,
            Editor = 1,
            Windows = 1 << 1,
            OSX = 1 << 2,
            Linux = 1 << 3,
            IOS = 1 << 4,
            TVOS = 1 << 5,
            Android = 1 << 6,
            Windows8Store = 1 << 7,
            WindowsUWP10 = 1 << 8,
            WebGL = 1 << 9,
            PS4 = 1 << 10,
            PSVita = 1 << 11,
            Xbox360 = 1 << 12,
            XboxOne = 1 << 13,
            SamsungTV = 1 << 14,
            WiiU = 1 << 15,
            Nintendo3DS = 1 << 16,
            Switch = 1 << 17,
            AmazonFireTV = 1 << 18,
            RazerForgeTV = 1 << 19,
            Stadia = 1 << 20,
            GameCoreXboxOne = 1 << 21,
            GameCoreScarlett = 1 << 22,
            Unknown = 1 << 31
        }

        #endregion
    }
}

namespace UFE3D {
    // This is a placeholder so the using statement doesn't throw an error when used with old versions of UFE which did not have the UFE3D namespace.
    // Added for UFE 2.4.1 where breaking change was made moving classes into namespaces.
}