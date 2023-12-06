


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using UnityEngine;


public class MessageManager : MonoBehaviour
// , MMEventListener<TopDownEngineEvent>
{




    public static MessageManager Instance { get; private set; }//考虑到线程安全，这里没有使用自动创建

    GameObject UVSEventSystem;

    public static void Create()
    {
        if (Instance != null) return;
        var gameObject = new GameObject("_MessageManger");
        DontDestroyOnLoad(gameObject);
        gameObject.AddComponent<MessageManager>();
    }

    private void Awake()
    {
        Instance = this;
        // this.MMEventStartListening();
    }

    private void OnDestroy()
    {
        // this.MMEventStopListening();
        dicMsgEvents.Clear();
        Instance = null;
    }

    class MessageEventData
    {
        public Action<Message> messageEvent;
        public bool autoRemove;
    }

    private Dictionary<string, List<MessageEventData>> dicMsgEvents = new Dictionary<string, List<MessageEventData>>();

    public void AddListener(string messageName, Action<Message> messageEvent, bool autoRemove = false)
    {
        MessageEventData messageEventData = new MessageEventData
        {
            messageEvent = messageEvent,
            autoRemove = autoRemove
        };

        List<MessageEventData> list;
        if (!dicMsgEvents.TryGetValue(messageName, out list))
        {
            list = new List<MessageEventData>();
            dicMsgEvents.Add(messageName, list);
        }
        list.Add(messageEventData);
    }

    public void RemoveListener(string messageName, Action<Message> messageEvent)
    {
        List<MessageEventData> list;
        if (dicMsgEvents.TryGetValue(messageName, out list))
        {
            var index = list.FindIndex(0, data => data.messageEvent == messageEvent);
            if (index >= 0)
            {
                list.RemoveAt(index);
                if (list.Count <= 0)
                {
                    dicMsgEvents.Remove(messageName);
                }
            }
        }
    }
    public void RemoveOneTypeListener(string messageName)
    {
        dicMsgEvents.Remove(messageName);
    }

    public void RemoveAllListener()
    {
        dicMsgEvents.Clear();
    }

    public void SendMessage(Message message)
    {
        DoMessageDispatcher(message);
    }

    public void Send(Message message)
    {
        DoMessageDispatcher(message);
    }

    public void SendMessage(string name, object content = null, params object[] dicParams)
    {
        SendMessage(new Message(name, content, dicParams));
    }

    public void Send(string name, object content)
    {
        SendMessage(new Message(name, content));
    }

    public void Send(string name, object[] Params)
    {
        SendMessage(new Message(name) { Params = Params });
    }

    public void Send(string name)
    {
        SendMessage(new Message(name));
    }

    private void DoMessageDispatcher(Message message)
    {
        if (dicMsgEvents.TryGetValue(message.Name, out List<MessageEventData> list))
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                list[i].messageEvent?.Invoke(message);
                if (list[i].autoRemove)
                {
                    list.RemoveAt(i);
                    if (list.Count == 0)
                    {
                        dicMsgEvents.Remove(message.Name);
                        break;
                    }
                }
            }
        }
    }



    ConcurrentQueue<Message> msgQueue = new ConcurrentQueue<Message>();

    //线程安全
    public void PostMessage(Message message)
    {
        msgQueue.Enqueue(message);
    }

    public void PostMessage(string name, object content = null, params object[] dicParams)
    {
        PostMessage(new Message(name, content, dicParams));
    }

    public void RunInMainThread(Action<object> action, object content = null)
    {
        PostMessage(new Message(null, null, content) { Action = action });
    }

    public void Update()
    {
        while (msgQueue.TryDequeue(out Message msg))
        {
            if (msg.Action != null)
            {
                msg.CallInMainThread();
            }
            else
                SendMessage(msg);
        }
    }
}