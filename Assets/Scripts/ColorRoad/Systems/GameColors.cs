namespace ColorRoad.Systems
{
    public enum GameColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }

    public static class GameColors
    {
        public static GameColor[] ActiveColors { get; set; }
        public static (GameColor? oldColor, GameColor? newColor) ColorReplacement { get; set; }

        public static GameColor GetNewColorAfterReskin(GameColor color)
        {
            return ColorReplacement.oldColor == color ? (GameColor) ColorReplacement.newColor : color;
        }
    }
}