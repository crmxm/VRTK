namespace VRTK
{
    using UnityEngine;
    using UnityEngine.VR;

    public class SDK_FallbackTracker : MonoBehaviour
    {
        public VRNode nodeType;
        public uint index;
        public string triggerAxisName = "";
        public string gripAxisName = "";
        public string touchpadHorizontalAxisName = "";
        public string touchpadVerticalAxisName = "";

        protected virtual void FixedUpdate()
        {
            transform.localPosition = InputTracking.GetLocalPosition(nodeType);
            transform.localRotation = InputTracking.GetLocalRotation(nodeType);
        }
    }
}