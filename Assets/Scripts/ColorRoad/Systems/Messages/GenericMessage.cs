namespace ColorRoad.Systems.Messages
{
    public enum GenericMessageType
    {
        PlayerFollowPathBegin,
        PlayerFollowPathEnd,
        PlayerObstacleCollision, // Obstacle obstacle
        GameOver,                // int score, int roadNumber
        PlayerColorChanged,      // GameColor newColor
        ScoreChanged,            // int score
        NewBestScore,            // int score
        NewBestRoad,             // int road
        StartGame,
        PauseGame,
        ContinueGame,
        ReturnToMainMenu, // bool game finished (not from pause menu)
        FinalScore,       // bool game finished, int score, int roadNumber
        OpenShop,
        OpenMainMenu,
        SkinChanged,
        Respawn,
        NewRoad,    // int road
        BallPickup, // int added score, Obstacle target
        CoinPickup, // int added coins, Obstacle target
    }

    public class GenericMessage
    {
        public readonly GenericMessageType MessageType;
        public readonly object[] Data;

        public GenericMessage(GenericMessageType messageType, params object[] data)
        {
            MessageType = messageType;
            Data = data;
        }
    }
}