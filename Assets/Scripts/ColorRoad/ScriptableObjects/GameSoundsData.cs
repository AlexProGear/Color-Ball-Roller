using UnityEngine;

namespace ColorRoad.ScriptableObjects
{
    [CreateAssetMenu(order = 5)]
    public class GameSoundsData : ScriptableObject
    {
        public AudioClip[] pickupBall1;
        public AudioClip[] pickupBall2;
        public AudioClip[] pickupBall3;

        public AudioClip[] pickupBallSpecial;

        public AudioClip[] changeColor;

        public AudioClip[] death;
        public AudioClip[] newRoad;
        public AudioClip[] coin;
    }
}