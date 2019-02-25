using UnityEngine;

namespace UtilityKit
{
    public class EventData
    {
        public string axis = null;
        public string button = null;
        public KeyCode keyCode = KeyCode.None;
        public bool used = false;
        public float value = 0f;

        public EventData(KeyCode keyCode) { this.keyCode = keyCode; }
        public EventData(string axis, float value) { this.axis = axis; this.value = value; }
        public EventData(string button) { this.button = button; }
    }
}
