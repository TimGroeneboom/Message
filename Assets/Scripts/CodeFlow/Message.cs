using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CodeFlow
{
    internal abstract class MessageBroadCasterBase { }

    internal class MessageBroadCaster<T> : MessageBroadCasterBase where T : Message
    {
        protected List<Action<T>> _callbacks = new List<Action<T>>();
        protected List<Action<T>> _callbacksToRemove = new List<Action<T>>();
        protected bool _broadCasting = false;

        public void AddListener(Action<T> callback) 
        {
            if (!_callbacks.Contains(callback))
            {
                _callbacks.Add(callback);
            }
            else
            {
                Debug.LogError("MessageBroadCaster<" + typeof(T).ToString() + "> -> already contains this callback");
            }
        }

        public void RemoveListener(Action<T> callback)
        {
            if( _broadCasting )
            {
                _callbacksToRemove.Add(callback);
            }else
            {
                _callbacks.Remove(callback);
            }
        }

        public void BroadCast(T message)
        {
            _broadCasting = true;
            foreach(var callback in _callbacks)
            {
                if( callback != null )
                {
                    callback.Invoke(message);
                }else
                {
                    Debug.LogError("MessageBroadCaster<" + typeof(T).ToString() + "> -> callback == null");
                }  
            }
            _broadCasting = false;

            foreach (var callback in _callbacksToRemove)
            {
                _callbacks.Remove(callback);
            }
            _callbacksToRemove.Clear();
        }
    }

    public abstract class Message
    {
        static internal Dictionary<Type, MessageBroadCasterBase> s_instances = new Dictionary<Type, MessageBroadCasterBase>();

        static public void AddListener<T>(Action<T> callback) where T : Message
        {
            GetBroadcasterInstance<T>().AddListener(callback);
        }

        static public void RemoveListener<T>(Action<T> callback) where T : Message
        {
            GetBroadcasterInstance<T>().RemoveListener(callback);
        }

        static public void BroadCast<T>(T message) where T : Message
        {
            GetBroadcasterInstance<T>().BroadCast(message);
        }

        static private MessageBroadCaster<T> GetBroadcasterInstance<T>() where T : Message
        {
            var type = typeof(T);
            if (!s_instances.ContainsKey(type))
            {
                s_instances.Add(type, new MessageBroadCaster<T>());
            }

            return s_instances[type] as MessageBroadCaster<T>;
        }
    }
}
