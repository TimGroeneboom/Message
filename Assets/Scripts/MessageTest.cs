using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeFlow
{
    public class CustomMessage : Message
    {
        public string HelloWorldString = "Hello World";
    }

    public class OtherCustomMessage : Message
    {
        public int MyCustomInteger = -1;
    }

    public class MessageExample : MonoBehaviour
    {
        void Start()
        {
            Message.AddListener<CustomMessage>(OnCustomMessage);
            Message.AddListener<OtherCustomMessage>(OnOtherCustomMessage);

            var customMessage = new CustomMessage();
            Message.BroadCast(customMessage); // OnCustomMessage should be called
            customMessage.HelloWorldString = "Should not be received";
            Message.BroadCast(customMessage); // OnCustomMessage should not be called
            Message.BroadCast(new OtherCustomMessage()); // OnOtherCustomMessage should be called
            Message.AddListener<OtherCustomMessage>(OnOtherCustomMessage); // error should be thrown
            Message.BroadCast(new OtherCustomMessage 
            {
                MyCustomInteger = 666
            }); // OnOtherCustomMessage should be called

            customMessage.HelloWorldString = "Should be received";
            Action<CustomMessage> callbackAction = null;
            callbackAction = new Action<CustomMessage>(message =>
           {
               Debug.Log(message.HelloWorldString);
               Message.RemoveListener(callbackAction);
           });
            Message.AddListener<CustomMessage>(OnCustomMessage);
            Message.BroadCast(customMessage); // callbackAction should be called
            Message.RemoveListener(callbackAction);
            Message.BroadCast(customMessage); // callbackAction should not be called
        }

        private void OnOtherCustomMessage(OtherCustomMessage message)
        {
            Debug.Log("my custom integer : " + message.MyCustomInteger);
        }

        private void OnCustomMessage(CustomMessage message)
        {
            Debug.Log(message.HelloWorldString);
            Message.RemoveListener<CustomMessage>(OnCustomMessage);
        }
    }
}
