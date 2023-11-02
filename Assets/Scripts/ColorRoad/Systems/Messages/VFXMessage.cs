namespace ColorRoad.Systems.Messages
{
    public enum VFXMessageType
    {
        PickupBall,
        ChangeColor,
        Landing,
        Death,
    }

    public class VFXMessage
    {
        public readonly VFXMessageType MessageType;
        public readonly object[] Data;
        
        public VFXMessage(VFXMessageType messageType, params object[] data)
        {
            MessageType = messageType;
            Data = data;
        }
    }
}