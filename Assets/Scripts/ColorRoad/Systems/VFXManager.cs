using System;
using ColorRoad.Gameplay;
using ColorRoad.ScriptableObjects;
using ColorRoad.Systems.Messages;
using UnityEngine;
using Zenject;

namespace ColorRoad.Systems
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VFXManager : IInitializable, IDisposable, IMessageReceiver<VFXMessage>
    {
        [Inject] private PlayerLogic playerLogic;

        private VFXData vfxData;

        public void Initialize()
        {
            vfxData = CachedResources.Get<VFXData>("VFXData");
            MessageBus.Subscribe(this);
        }

        public void Dispose()
        {
            MessageBus.Unsubscribe(this);
        }

        public void OnMessageReceived(VFXMessage message)
        {
            switch (message.MessageType)
            {
                case VFXMessageType.PickupBall:
                    SpawnParticlesAtPlayer(vfxData.pickupBallParticles);
                    break;
                case VFXMessageType.ChangeColor:
                    SpawnParticlesAtPlayer(vfxData.changeColorParticles);
                    break;
                case VFXMessageType.Death:
                    SpawnParticlesAtPlayer(vfxData.gameOverParticles);
                    SpawnConfetti();
                    break;
                case VFXMessageType.Landing:
                    SpawnLandingParticles();
                    break;
            }
        }

        private void SpawnParticlesAtPlayer(GameObject particlesToSpawn)
        {
            MonoBehaviour.Instantiate(particlesToSpawn, playerLogic.ViewModelTransform);
        }
        
        private void SpawnLandingParticles()
        {
            GameObject particles = MonoBehaviour.Instantiate(vfxData.landingParticles, playerLogic.ViewModelTransform);
            particles.transform.rotation = Quaternion.LookRotation(Vector3.down);
            particles.transform.position += Vector3.down * 0.4f;
        }

        private void SpawnConfetti()
        {
            Vector3 position = Camera.main.transform.position + Vector3.up * 3 + Camera.main.transform.forward * 5;
            MonoBehaviour.Instantiate(vfxData.confettiParticles, position, Quaternion.identity);
        }
    }
}