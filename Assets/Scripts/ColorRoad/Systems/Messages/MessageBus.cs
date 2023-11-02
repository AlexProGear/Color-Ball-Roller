using System;
using System.Collections.Generic;
using System.Linq;

namespace ColorRoad.Systems.Messages
{
    public interface IMessageReceiver {}
    public interface IMessageReceiver<in T> : IMessageReceiver where T : class
    {
        void OnMessageReceived(T message);
    }

    public static class MessageBus
    {
        private static readonly Dictionary<Type, HashSet<IMessageReceiver>> Subscribers = new Dictionary<Type, HashSet<IMessageReceiver>>();

        public static void Subscribe<T>(IMessageReceiver<T> receiver) where T : class
        {
            if (!Subscribers.TryGetValue(typeof(T), out HashSet<IMessageReceiver> collection))
                Subscribers.Add(typeof(T), collection = new HashSet<IMessageReceiver>());
            collection.Add(receiver);
        }

        public static void Unsubscribe<T>(IMessageReceiver<T> receiver) where T : class
        {
            if (Subscribers.TryGetValue(typeof(T), out HashSet<IMessageReceiver> collection))
                collection.Remove(receiver);
        }

        public static void Post<T>(T message) where T : class
        {
            if (Subscribers.TryGetValue(typeof(T), out HashSet<IMessageReceiver> collection))
            {
                foreach (IMessageReceiver<T> receiver in collection.Cast<IMessageReceiver<T>>().ToArray())
                {
                    receiver.OnMessageReceived(message);
                }
            }
        }
    }
}