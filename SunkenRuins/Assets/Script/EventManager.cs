using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class EventManager : MonoBehaviour
    {
        private Dictionary<EventType, Action<Dictionary<string, object>>> eventDictionary;

        private static EventManager eventManager;

        public static EventManager instance
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                    if (!eventManager)
                    {
                        Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventManager.Init();

                        //  Sets this to not be destroyed when reloading scene
                        DontDestroyOnLoad(eventManager);
                    }
                }
                return eventManager;
            }
        }

        void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<EventType, Action<Dictionary<string, object>>>();
            }
        }

        public static void StartListening(EventType eventType, Action<Dictionary<string, object>> listener)
        {
            Action<Dictionary<string, object>> thisEvent;

            if (instance.eventDictionary.TryGetValue(eventType, out thisEvent))
            {
                thisEvent += listener;
                instance.eventDictionary[eventType] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                instance.eventDictionary.Add(eventType, thisEvent);
            }
        }

        public static void StopListening(EventType eventType, Action<Dictionary<string, object>> listener)
        {
            if (eventManager == null) return;
            Action<Dictionary<string, object>> thisEvent;
            if (instance.eventDictionary.TryGetValue(eventType, out thisEvent))
            {
                thisEvent -= listener;
                instance.eventDictionary[eventType] = thisEvent;
            }
        }

        public static void TriggerEvent(EventType eventType, Dictionary<string, object> message)
        {
            Action<Dictionary<string, object>> thisEvent = null;
            if (instance.eventDictionary.TryGetValue(eventType, out thisEvent))
            {
                thisEvent.Invoke(message);
            }
        }
    }
}