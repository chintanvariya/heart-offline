using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace FGSOfflineHeart
{
    public class HT_EventManager : MonoBehaviour
    {
        /// <summary>
        /// This is a custom UnityEvent that carries a payload of type JSONObject.
        /// </summary>
        public class SocketEvent : UnityEvent<string> { }

        /// <summary>
        /// Dictionary to store events dynamically
        /// </summary>
        private Dictionary<string, SocketEvent> eventDictionary = new Dictionary<string, SocketEvent>();

        /// <summary>
        /// This method register a listener for a specific event.
        /// If the event already exists, the listener is added. If not, a new event is created.
        /// </summary>
        /// /// <param name="eventName">The name of the event to register.</param>
        /// <param name="listener">The listener (UnityAction) to be added to the event.</param>
        public void RegisterEvent(SocketEvents eventName, UnityAction<string> listener)
        {
            if (eventDictionary.TryGetValue(eventName.ToString(), out var socketEvent))
            {
                // If the event already exists, add the listener
                socketEvent.AddListener(listener);
            }
            else
            {
                // If the event doesn't exist, create a new one and add the listener
                socketEvent = new SocketEvent();
                socketEvent.AddListener(listener);
                eventDictionary.Add(eventName.ToString(), socketEvent);
            }
        }

        /// <summary>
        /// This method unregisters a listener from a specific event.
        /// If the event exists, the listener is removed. If no listeners are left, the event is removed from the dictionary.
        /// </summary>
        /// <param name="eventName">The name of the event to unregister from.</param>
        /// <param name="listener">The listener (UnityAction) to be removed from the event.</param>
        public void UnregisterEvent(SocketEvents eventName, UnityAction<string> listener)
        {
            if (eventDictionary.TryGetValue(eventName.ToString(), out var socketEvent))
            {
                // Remove the listener from the event
                socketEvent.RemoveListener(listener);

                // If no listeners are left, remove the event from the dictionary
                if (socketEvent.GetPersistentEventCount() == 0)
                {
                    eventDictionary.Remove(eventName.ToString());
                }
            }
        }

        /// <summary>
        /// This method invokes a specific event with the provided data.
        /// If the event exists, it is invoked with the given event data.
        /// </summary>
        /// <param name="eventName">The name of the event to invoke.</param>
        /// <param name="eventData">The data to be passed to the event listeners.</param>
        public void InvokeEvent(string eventName, string eventData)
        {
            Debug.Log($"<color><b> TIME || {DateTime.Now.ToString("hh:mm:ss fff")}</b></color> || <color=cyan><b> <<< RECEIVED >>>  </b></color><color=white><b> { eventName}  </b></color> \n{eventData}");

            if (eventDictionary.TryGetValue(eventName, out var socketEvent))
            {
                // Invoke the event with the provided data
                socketEvent.Invoke(eventData);
            }
        }
    }

    public enum SocketEvents
    {
        SIGNUP,
        JOIN_TABLE,
        ROUND_TIMER_STARTED,
        LOCK_IN_PERIOD,
        COLLECT_BOOT_VALUE,
        SHOW_MY_CARDS,
        USER_TURN_STARTED,
        WINNER_DECLARE,
        LEAVE_TABLE,
        HEART_BEAT,
        USER_THROW_CARD,
        CARD_PASS,
        USER_CARD_PASS_TURN,
        CARD_MOVE,
        USER_THROW_CARD_SHOW,
        WIN_OF_ROUND,
        SHOW_POPUP,
        TIME_OUT_LEAVE_TABLE_POPUP,
        BACK_IN_GAME_PLAYING,
        PRE_CARD_PASS_SELECT,
        SHOW_SCORE_BOARD,
        SHOOTING_MOON
    }
}