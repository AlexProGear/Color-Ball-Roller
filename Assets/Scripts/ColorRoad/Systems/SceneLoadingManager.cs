using System;
using ColorRoad.Systems.Messages;
using UnityEngine.SceneManagement;
using Zenject;

namespace ColorRoad.Systems
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SceneLoadingManager : IInitializable, IDisposable, IMessageReceiver<GenericMessage>
    {
        public void Initialize()
        {
            MessageBus.Subscribe(this);
        }

        public void Dispose()
        {
            MessageBus.Unsubscribe(this);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.ReturnToMainMenu:
                    SceneManager.LoadScene(0);
                    break;
            }
        }
    }
}