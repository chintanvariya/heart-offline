using System;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO3;

namespace HeartCardGame
{
    public class HT_SocketHandler : MonoBehaviour
    {
        [Header("===== Socket URL =====")]
        [SerializeField] private string socketURL;
        [SerializeField] private string baseURL, portNumber;
        public List<string> serverUrl;

        [Header("===== Server Type =====")]
        public ServerType serverType;
        [SerializeField] private SocketState socketState;

        [Header("===== Socket =====")]
        [SerializeField] private int socketErrorCount;
        public SocketManager socketManager;

        [Header("===== Script Object =====")]
        [SerializeField] private HT_SocketEventManager socketEventManager;
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_UiManager uiManager;

        [Header("===== Heart Beat Data =====")]
        public bool isHeartBeatReceived;
        private int missCount = 0;

        private void Start()
        {
            Application.runInBackground = false;
            //InternetCheckInitiate();
        }

        private void OnApplicationPause(bool pause)
        {
            Debug.Log($"HT_GameManager || OnApplicationPause || IsPause : {pause}");
            if (gameManager.tableState != TableState.DASHBOARD)
            {
                if (pause)
                {
                    uiManager.reconnectionPanel.SetActive(true);
                    ForcefullySocketDisconnect();
                    gameManager.GameReset?.Invoke();
                    gameManager.RoundReset?.Invoke();
                }
                else
                {
                    Debug.Log($"BACK IN GAME PLAY");
                    InternetCheckInitiate();
                }
            }
        }

        public void SocketConnecting()
        {
            Debug.Log($"SocketHandler || SocketConnecting || Socket Connection Start");

            socketURL = serverUrl[(int)serverType];

            if (!socketURL.Contains("socket.io"))
            {
                socketURL = socketURL + "/socket.io/";
                Debug.Log($"<color=Yellow>Socket Connection URL -> </color> {socketURL}");
            }

            Debug.Log($"socketURL => {socketURL}");

            socketManager = new SocketManager(new Uri(socketURL), OverridedSocketOptions());

            socketManager.Socket.On(SocketIOEventTypes.Connect, () => SocketConnected());

            socketManager.Socket.On(SocketIOEventTypes.Disconnect, () => SocketDisConnected());

            socketManager.Socket.On<Error>(SocketIOEventTypes.Error, (error) => { SocketError(error); });

            RegisterCustomEvents();
        }

        private void SocketError(Error error)
        {
            Debug.Log($"Socket Error :{error}");
            socketState = SocketState.Error;
            HeartBeatStop();
            ErrorCounting();
        }

        private void SocketDisConnected()
        {
            Debug.Log($"<color=red><b>SocketHandler || Socket DisConnected</b></color> {socketManager.Socket.Id}");
            ForcefullySocketDisconnect();
        }

        private void SocketConnected()
        {
            if (socketState != SocketState.Open)
            {
                Debug.Log($"<color=green><b>Socket Connected Successfully</b></color>");
                socketState = SocketState.Open;
                HeartBeatInitiate();
                NoInternetPanelOnOff(false);
                //if (missCount > 2)
                //resetGame.ResetAllGame(false);
                AckEventSend(SocketEvents.SIGNUP.ToString(), socketEventManager.SignUpRequest(), ReceiveData);
                //socketErrorCount = 3;
            }
        }

        private void ReceiveData(string data)
        {
            //if (!data.ToString().Contains("HEART_BEAT"))
            //Debug.Log("<color=red><b>On Res : </b></color>" + "\n" + data + " |||||| Res Time => " + DateTime.Now + " |||| " + Time.time);
            eventManager.InvokeEvent(SocketEvents.SIGNUP.ToString(), data);
        }

        private void RegisterCustomEvents()
        {
            SocketEvents[] socketEvents = (SocketEvents[])Enum.GetValues(typeof(SocketEvents));

            for (int i = 0; i < socketEvents.Length; i++)
            {
                string eventName = socketEvents[i].ToString();
                socketManager.Socket.On<string>(eventName, (response) =>
                {
                    if (string.IsNullOrEmpty(response)) return;
                    eventManager.InvokeEvent(eventName, response);
                    //socketEventReceiver.ReceiveData(eventName, response);
                    // Pass response to socket event receiver
                });
            }
        }

        public void AckEventSend(string eventName, string data, Action<string> responseAction)
        {
            if (!eventName.Contains("HEART_BEAT"))
            {
                Debug.Log("<color><b> TIME || " + DateTime.Now.ToString("hh:mm:ss fff") + " || </b></color><color=yellow><b>EVENT NAME :- </b></color> <b><color>" + eventName + "</color></b>");
                Debug.Log("<color><b> TIME || " + DateTime.Now.ToString("hh:mm:ss fff") + " || </b></color><color=yellow><b>DATA :- </b></color>" + data);
            }
            socketManager.Socket.ExpectAcknowledgement<string>(responseAction).Emit(eventName, data);
        }

        public void DataSendToSocket(string eventName, string data)
        {
            if (!eventName.Contains("HEART_BEAT"))
            {
                Debug.Log("<color><b> TIME || " + DateTime.Now.ToString("hh:mm:ss fff") + " || </b></color><color=yellow><b>EVENT NAME :- </b></color> <b><color>" + eventName + "</color></b>");
                Debug.Log("<color><b> TIME || " + DateTime.Now.ToString("hh:mm:ss fff") + " || </b></color><color=yellow><b>DATA :- </b></color>" + data);
            }
            socketManager.Socket.Emit(eventName, data);
        }

        private SocketOptions OverridedSocketOptions()
        {
            SocketOptions socketOption = new SocketOptions();
            socketOption.ConnectWith = BestHTTP.SocketIO3.Transports.TransportTypes.WebSocket;
            socketOption.Reconnection = false;
            socketOption.ReconnectionAttempts = int.MaxValue;
            socketOption.ReconnectionDelay = TimeSpan.FromMilliseconds(1000);
            socketOption.ReconnectionDelayMax = TimeSpan.FromMilliseconds(5000);
            socketOption.RandomizationFactor = 0.5f;
            socketOption.Timeout = TimeSpan.FromMilliseconds(3000);
            socketOption.AutoConnect = true;
            socketOption.QueryParamsOnlyForHandshake = true;
            return socketOption;
        }

        public void InternetCheckInitiate()
        {
            CancelInvoke(nameof(InternetChecking));
            InvokeRepeating(nameof(InternetChecking), 0f, 2f);
        }

        public void InternetChecking()
        {
            if (IsInternetConnectionAvailable())
            {
                if (socketState == SocketState.None || socketState == SocketState.Error || socketState == SocketState.Disconnect)
                {
                    Debug.Log($"Internet is available on your device");
                    socketState = SocketState.Connecting;
                    uiManager.reconnectionPanel.SetActive(true);
                    SocketConnecting();
                }
            }
            else if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                NoInternetPanelOnOff(true);
                if (socketState != SocketState.None)
                {
                    ForcefullySocketDisconnect();
                }
            }
        }

        private void NoInternetPanelOnOff(bool isOpen)
        {
            uiManager.NoInternetPanelOnOff(isOpen, false);
        }

        public bool IsInternetConnectionAvailable()
        {
            // return true when network available;
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public void ForcefullySocketDisconnect()
        {
            if (socketManager != null)
            {
                Debug.Log($"<color=red><b>ForcefullySocketDisconnect</b></color>");
                socketManager.Socket.Disconnect();
                HeartBeatStop();
                socketState = SocketState.Disconnect;
                gameManager.GameReset?.Invoke();
                gameManager.RoundReset?.Invoke();
                gameManager.tableState = TableState.NONE;
            }
        }

        private void HeartBeatInitiate()
        {
            CancelInvoke(nameof(HeartBeating));
            InvokeRepeating(nameof(HeartBeating), 0, 3f);
        }

        private void HeartBeatStop()
        {
            CancelInvoke(nameof(HeartBeating));
        }

        private void HeartBeating()
        {
            isHeartBeatReceived = false;
            AckEventSend(SocketEvents.HEART_BEAT.ToString(), socketEventManager.HeartbeatRequest(), HeartBeatReceived);
            Invoke(nameof(HeartBeatCount), 1f);
        }

        private void HeartBeatReceived(string data)
        {
            isHeartBeatReceived = true;
        }

        private void HeartBeatCount()
        {
            if (isHeartBeatReceived)
            {
                missCount = 0;
            }
            else
            {
                missCount++;
                CancelInvoke(nameof(HeartBeatCount));
                Debug.Log("HeartBeatCount || missCount ||  " + missCount);
                if (missCount > 2)
                {
                    Debug.Log("internet connection very slow : ");
                    //resetGame.ResetAllGame(false);
                    ForcefullySocketDisconnect();
                    // Show No Interent Popup //
                    NoInternetPanelOnOff(true);
                }
            }

            // Network Icon Sprite Set //
            //uIManager.SetNetworkIconSprite(missCount);
        }

        private void ErrorCounting()
        {
            socketErrorCount--;
            if (socketErrorCount == 0)
            {
                Debug.Log(" Oops Something Went Wrong ");
                //uIManager.ErrorMsgAlertPopupOpen(AlertMessageType.Error);
            }
        }
    }

    public enum ServerType
    {
        dev = 0,
        local = 1,
    }

    public enum SocketState
    {
        None,
        Connecting,
        Open,
        Error,
        Disconnect
    }
}
