namespace ColorRoad.Systems.Messages
{
    public enum AudioMessageType
    {
        BallPickup,        // int color index
        BallPickupSpecial, // int color index
        ChangeColor,       // int color index
        Death,
        NewRoad,
        Click
    }

    public class AudioMessage
    {
        public readonly AudioMessageType MessageType;
        public readonly object[] Data;
        
        public AudioMessage(AudioMessageType messageType, params object[] data)
        {
            MessageType = messageType;
            Data = data;
        }
    }
}