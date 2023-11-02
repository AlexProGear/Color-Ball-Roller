namespace ColorRoad.Systems.Messages
{
    public enum Cheat
    {
        None,
        GodMode, // bool enabled
    }

    public class CheatMessage
    {
        public readonly Cheat CheatType;
        public readonly object[] Data;

        public CheatMessage(Cheat cheatType, params object[] data)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            CheatType = cheatType;
#else
            CheatType = Cheat.None;
#endif
            Data = data;
        }
    }
}