﻿// HyperealVR Controller|SDK_HyperealVR|
namespace VRTK
{
#if VRTK_DEFINE_SDK_HYPEREALVR
    using UnityEngine;
    using System.Collections.Generic;
    using Hypereal;
    using System;
#endif

    /// <summary>
    /// The HyperealVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_HyperealVRSystem))]
    public class SDK_HyperealVRController
#if VRTK_DEFINE_SDK_HYPEREALVR
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_SDK_HYPEREALVR
        private VRTK_TrackedController cachedLeftController;
        private VRTK_TrackedController cachedRightController;
        private VRTK_TrackedController cachedLeftTrackedObject;
        private VRTK_TrackedController cachedRightTrackedObject;

        private bool[] previousHairTriggerState = new bool[2];
        private bool[] currentHairTriggerState = new bool[2];

        private bool[] previousHairGripState = new bool[2];
        private bool[] currentHairGripState = new bool[2];
        //private Dictionary<GameObject, HyperealVR_TrackedObject> cachedTrackedObjectsByGameObject = new Dictionary<GameObject, HyperealVR_TrackedObject>();
        //private Dictionary<uint, HyperealVR_TrackedObject> cachedTrackedObjectsByIndex = new Dictionary<uint, HyperealVR_TrackedObject>();
        

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(VRTK_ControllerReference ctrl, Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(VRTK_ControllerReference ctrl, Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.
        /// </summary>
        /// <returns>The ControllerType based on the SDK and headset being used.</returns>
        public override ControllerType GetCurrentControllerType()
        {
            return ControllerType.Hypereal_Sens;
        }

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            return "ControllerColliders/Fallback";
        }

        /// <summary>
        /// The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.
        /// </summary>
        /// <param name="element">The controller element to look up.</param>
        /// <param name="hand">The controller hand to look up.</param>
        /// <param name="fullPath">Whether to get the initial path or the full path to the element.</param>
        /// <returns>A string containing the path to the game object that the controller element resides in.</returns>
        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetControllerIndex(GameObject controller)
        {
            var trackedObject = GetTrackedObject(controller);
            return (trackedObject ? (uint)trackedObject.index : uint.MaxValue);
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject of the controller</returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            SetTrackedControllerCaches();
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);
                }

                if (cachedRightController != null && cachedRightController.index == index)
                {
                    return (actual ? sdkManager.loadedSetup.actualRightController : sdkManager.scriptAliasRightController);
                }
            }
            return null;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controller">The controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(VRTK_ControllerReference controller)
        {
            return controller.actual.transform;
        }

        /// <summary>
        /// The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.
        /// </summary>
        /// <param name="parent">The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.</param>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the left hand controller.</returns>
        public override GameObject GetControllerLeftHand(bool actual = false)
        {
			if (actual) {
				HyTrackObjRig trackedObjRig = VRTK_SharedMethods.FindEvenInactiveGameObject<HyTrackObjRig> ().GetComponent<HyTrackObjRig>();
				if (trackedObjRig)
                    return trackedObjRig.leftController;
			} else {
				var sdkManager = VRTK_SDKManager.instance;
				if (sdkManager != null) {
					return sdkManager.scriptAliasLeftController;
				}
			}
            return null;
        }

        /// <summary>
        /// The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public override GameObject GetControllerRightHand(bool actual = false)
        {
			if (actual) {
				HyTrackObjRig trackedObjRig = VRTK_SharedMethods.FindEvenInactiveGameObject<HyTrackObjRig> ().GetComponent<HyTrackObjRig>();
				if (trackedObjRig)
                    return trackedObjRig.rightController;
			} else {
				var sdkManager = VRTK_SDKManager.instance;
				if (sdkManager != null) {
					return sdkManager.scriptAliasRightController;
				}
			}
            return null;
        }

        /// <summary>
        /// The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsLeftHand(controller);
        }

        /// <summary>
        /// The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsRightHand(controller);
        }

        /// <summary>
        /// The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return CheckControllerLeftHand(controller, actual);
        }

        /// <summary>
        /// The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return CheckControllerRightHand(controller, actual);
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given GameObject.
        /// </summary>
        /// <param name="controller">The GameObject to get the model alias for.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerModelFromController(controller);
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given controller hand.
        /// </summary>
        /// <param name="hand">The hand enum of which controller model to retrieve.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(ControllerHand hand)
        {
            var model = GetSDKManagerControllerModelForHand(hand);
            if (!model)
            {
                GameObject controller = null;
                switch (hand)
                {
                    case ControllerHand.Left:
                        controller = GetControllerLeftHand(true);
                        break;
                    case ControllerHand.Right:
                        controller = GetControllerRightHand(true);
                        break;
                }

                if (controller != null)
                {
                    model = controller.transform.Find("Model").gameObject;
                }
            }
            return model;
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(VRTK_ControllerReference controller)
        {
            //TODO: NOT IMPLEMENTED
            return null;
        }

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            
        }

        /// <summary>
        /// The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public override void HapticPulse(VRTK_ControllerReference ctrl, float strength = 0.5f)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(ctrl);
            HyDevice ctrlDevice = MappingIndex2HyDevice(index);

            if (ctrlDevice == HyDevice.Device_Unknown)
                return;

            HyperealVR.Instance.SetHapticFeedback(ctrlDevice, 0.5f, strength);
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="clip">The audio clip to use for the haptic pattern.</param>
        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            //TODO;
            return false;
        }

        /// <summary>
        /// The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            return new SDK_ControllerHapticModifiers();
        }

        /// <summary>
        /// The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocity(VRTK_ControllerReference ctrl)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(ctrl);
            SetTrackedControllerCaches();
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller0);
                    return controllerState.velocity;
                }

                if (cachedRightController != null && cachedRightController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller1);
                    return controllerState.velocity;
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference ctrl)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(ctrl);
            SetTrackedControllerCaches();
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller0);
                    return controllerState.angularVelocity;
                }

                if (cachedRightController != null && cachedRightController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller1);
                    return controllerState.angularVelocity;
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.
        /// </summary>
        /// <param name="currentAxisValues"></param>
        /// <param name="previousAxisValues"></param>
        /// <param name="compareFidelity"></param>
        /// <returns>Returns true if the touchpad is not currently being touched or moved.</returns>
        public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
        {
            //TODO: 
            return (!isTouched || VRTK_SharedMethods.Vector2ShallowCompare(currentAxisValues, previousAxisValues, compareFidelity));
        }

        /// <summary>
        /// The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the axis on.</param>
        /// <param name="controllerReference">The reference to the controller to check the button axis on.</param>
        /// <returns>A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.</returns>
        public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            HyDevice ctrlDevice = MappingIndex2HyDevice(index);
            if (ctrlDevice == HyDevice.Device_Unknown)
                return Vector2.zero;

            HyInput input = HyInputManager.Instance.GetInputDevice(ctrlDevice);

            switch (buttonType)
            {
                case ButtonTypes.Touchpad:
                    return input.GetTouchpadAxis();
                case ButtonTypes.Trigger:
                    return new Vector2(input.GetTriggerAxis(HyInputKey.IndexTrigger), 0f);
                case ButtonTypes.Grip:
                    return new Vector2(input.GetTriggerAxis(HyInputKey.SideTrigger), 0f);
            }
            return Vector2.zero;
        }

        /// <summary>
        /// The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.
        /// </summary>
        /// <param name="buttonType">The type of button to get the hairline delta for.</param>
        /// <param name="controllerReference">The reference to the controller to get the hairline delta for.</param>
        /// <returns>The delta between the button presses.</returns>
        public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            HyDevice ctrlDevice = MappingIndex2HyDevice(index);
            if (ctrlDevice == HyDevice.Device_Unknown)
                return 0f;

            HyInput input = HyInputManager.Instance.GetInputDevice(ctrlDevice);

            return (buttonType == ButtonTypes.Trigger ? 0.1f : 0f);
        }

        /// <summary>
        /// The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the state of.</param>
        /// <param name="pressType">The button state to check for.</param>
        /// <param name="controllerReference">The reference to the controller to check the button state on.</param>
        /// <returns>Returns true if the given button is in the state of the given press type on the given controller reference.</returns>
        public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            HyDevice ctrlDevice = MappingIndex2HyDevice(index);
            if (ctrlDevice == HyDevice.Device_Unknown)
                return false;

            HyInput input = HyInputManager.Instance.GetInputDevice(ctrlDevice);

            switch (buttonType)
            {
                case ButtonTypes.ButtonOne:
                    return false;
                case ButtonTypes.ButtonTwo:
                    return false;
                case ButtonTypes.Grip:
                    return IsButtonPressed(ctrlDevice, pressType, HyInputKey.SideTrigger);
                case ButtonTypes.GripHairline:
                    return false;
                case ButtonTypes.StartMenu:
                    return IsButtonPressed(ctrlDevice, pressType, HyInputKey.Menu);
                case ButtonTypes.Trigger:
                    return IsButtonPressed(ctrlDevice, pressType, HyInputKey.IndexTrigger);
                case ButtonTypes.TriggerHairline:
                    return false;
                case ButtonTypes.Touchpad:
                    return IsButtonPressed(ctrlDevice, pressType, HyInputKey.Touchpad);
            }
            return false;
        }

        private void OnTrackedDeviceRoleChanged<T>(T ignoredArgument)
        {
            SetTrackedControllerCaches(true);
        }

        private void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftTrackedObject = null;
                cachedRightTrackedObject = null;
            }

            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftTrackedObject == null && sdkManager.loadedSetup.actualLeftController)
                {
                    cachedLeftController = sdkManager.loadedSetup.actualLeftController.GetComponent<VRTK_TrackedController>();
					cachedLeftController.index = 0;
                }
                if (cachedRightTrackedObject == null && sdkManager.loadedSetup.actualRightController)
                {
                    cachedRightController = sdkManager.loadedSetup.actualRightController.GetComponent<VRTK_TrackedController>();
					cachedRightController.index = 1;
                }
            }
        }

        private VRTK_TrackedController GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();
            VRTK_TrackedController trackedObject = null;

            if (IsControllerLeftHand(controller))
            {
                trackedObject = cachedLeftController;
            }
            else if (IsControllerRightHand(controller))
            {
                trackedObject = cachedRightController;
            }
            return trackedObject;
        }

		private HyDevice MappingIndex2HyDevice(uint index) {
			switch (index) {
			case 0:
				return HyDevice.Device_Controller0;
			case 1:
				return HyDevice.Device_Controller1;
			default:
				break;
			}
			return HyDevice.Device_Unknown;
		}

        private bool IsButtonPressed(HyDevice index, ButtonPressTypes type, HyInputKey button)
        {
            //todo use index to input type
            var device = HyInputManager.Instance.GetInputDevice(index);
            switch (type)
            {
                case ButtonPressTypes.Press:
                    return device.GetPress((HyInputKey)button);
                case ButtonPressTypes.PressDown:
                    return device.GetPressDown((HyInputKey)button);
                case ButtonPressTypes.PressUp:
                    return device.GetPressUp((HyInputKey)button);
                case ButtonPressTypes.Touch:
                    return device.GetTouch((HyInputKey)button);
                case ButtonPressTypes.TouchDown:
                    return device.GetTouchDown((HyInputKey)button);
                case ButtonPressTypes.TouchUp:
                    return device.GetTouchUp((HyInputKey)button);
            }

            return false;
        }

        private string GetControllerGripPath(ControllerHand hand, string suffix, ControllerHand forceHand)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return (forceHand == ControllerHand.Left ? "lgrip" : "rgrip") + suffix;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return "grip" + suffix;
            }
            return null;
        }

        private string GetControllerTouchpadPath(ControllerHand hand, string suffix)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return "trackpad" + suffix;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return "thumbstick" + suffix;
            }
            return null;
        }

        private string GetControllerButtonOnePath(ControllerHand hand, string suffix)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return null;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return (hand == ControllerHand.Left ? "x_button" : "a_button") + suffix;
            }
            return null;
        }

        private string GetControllerButtonTwoPath(ControllerHand hand, string suffix)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return "button" + suffix;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return (hand == ControllerHand.Left ? "y_button" : "b_button") + suffix;
            }
            return null;
        }

        private string GetControllerSystemMenuPath(ControllerHand hand, string suffix)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return "sys_button" + suffix;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return (hand == ControllerHand.Left ? "enter_button" : "home_button") + suffix;
            }
            return null;
        }

        private string GetControllerStartMenuPath(ControllerHand hand, string suffix)
        {
            switch (VRTK_DeviceFinder.GetHeadsetType(true))
            {
                case VRTK_DeviceFinder.Headsets.Vive:
                    return null;
                case VRTK_DeviceFinder.Headsets.OculusRift:
                    return (hand == ControllerHand.Left ? "enter_button" : "home_button") + suffix;
            }
            return null;
        }
#endif
    }
}