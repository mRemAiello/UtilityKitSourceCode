using System.Collections.Generic;
using UnityEngine;

namespace UtilityKit
{
    public class GameInputManager : Singleton<GameInputManager>
    {
        public const int MAX_PRIORITY = 10000;

        /// <summary>Register an axis as one of interest.</summary>
        public static void ObserveAxis(string axis)
        {
            if (!string.IsNullOrEmpty(axis))
                observedAxes.Add(axis);
        }

        /// <summary>Register a button as one of interest.</summary>
        public static void ObserveButton(string button)
        {
            if (!string.IsNullOrEmpty(button)) 
                observedButtons.Add(button);
        }

        /// <summary>Register a keycode as one of interest.</summary>
        public static void ObserveKeyCode(KeyCode keyCode)
        {
            if (keyCode != KeyCode.None) 
                observedKeycodes.Add(keyCode);
        }

        /// <summary>Register a handler method for hotkey event with one above currently highest priority.</summary>
        /// <param name="Action">Handler method that is called when hotkey event triggers. That method has one EventData parameter.</param>
        public static void Register(System.Action<EventData> Action)
        {
            if (Action != null) 
                GetBlock(HighestPriority + 1).Event += Action;
        }

        /// <summary>Register a handler method for hotkey event with the specified priority.</summary>
        /// <param name="Action">Handler method that is called when hotkey event triggers. That method has one EventData parameter.</param>
        /// <param name="priority">Callbacks are made in order of priority (from the highest to the lowest).</param>
        public static void Register(System.Action<EventData> Action, int priority)
        {
            if (Action != null) 
                GetBlock(priority).Event += Action;
        }

        /// <summary>Unregister a callback method from all Input events.</summary>
        public static void Unregister(System.Action<EventData> Action)
        {
            if (Action != null)
            {
                foreach (EventBlock b in eventBlocks)
                {
                    b.Event -= Action;
                }                   
            }
        }

        protected void Update()
        {
            foreach (string a in observedAxes)
            {
                SendEvent(new EventData(a, Input.GetAxis(a)));
            }
            foreach (string b in observedButtons)
            {
                if (Input.GetButtonDown(b)) SendEvent(new EventData(b));
            }
            foreach (KeyCode k in observedKeycodes)
            {
                if (Input.GetKeyDown(k)) SendEvent(new EventData(k));
            }
        }

        protected static List<EventBlock> eventBlocks = new List<EventBlock>();
        protected static HashSet<string> observedAxes = new HashSet<string>();
        protected static HashSet<string> observedButtons = new HashSet<string>();
        protected static HashSet<KeyCode> observedKeycodes = new HashSet<KeyCode>();

        protected static EventBlock GetBlock(int priority)
        {
            foreach (EventBlock b in eventBlocks) 
                if (b.priority == priority)
                    return b;
            
            EventBlock newBlock = new EventBlock(priority);
            eventBlocks.Add(newBlock);
            eventBlocks.Sort();
            
            return newBlock;
        }

        protected static int HighestPriority
        {
            get
            {
                // eventBlocks is always sorted in reversed priority order (i.e., highest to lowest), so first non-empty block is the correct result
                foreach (EventBlock b in eventBlocks) 
                    if (b.priority < MAX_PRIORITY && !b.IsEmpty) 
                        return b.priority;

                return 0;
            }
        }

        protected static void SendEvent(EventData data)
        {
            System.Action<EventData> callStack = null;
            foreach (EventBlock block in eventBlocks) 
                block.AppendTo(ref callStack);

            callStack?.Invoke(data);
        }
    }
}
