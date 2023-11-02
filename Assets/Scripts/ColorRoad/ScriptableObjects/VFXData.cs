using UnityEngine;

namespace ColorRoad.ScriptableObjects
{
    [CreateAssetMenu(order = 6)]
    public class VFXData : ScriptableObject
    {
        public GameObject pickupBallParticles;
        public GameObject changeColorParticles;
        public GameObject landingParticles;
        public GameObject gameOverParticles;
        public GameObject confettiParticles;
    }
}