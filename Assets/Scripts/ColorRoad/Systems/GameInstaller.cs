using ColorRoad.Gameplay;
using ColorRoad.Systems.Audio;
using ColorRoad.Systems.Shop;
using UnityEngine;
using Zenject;

namespace ColorRoad.Systems
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject gameManager;
        [SerializeField] private GameObject roadGenerator;
        [SerializeField] private GameObject splinePointsGenerator;
        [SerializeField] private GameObject obstacleGenerator;
        [SerializeField] private GameObject buildingGenerator;
        [SerializeField] private GameObject vibrationController;
        [SerializeField] private InputControls inputControls;

        public override void InstallBindings()
        {
            BindNewInstanceFromPrefab<PlayerLogic>(playerPrefab);
            BindNewInstanceFromPrefab<GameManager>(gameManager, transform);
            BindNewInstanceFromPrefab<RoadGenerator>(roadGenerator, transform);
            BindNewInstanceFromPrefab<SplinePointsGenerator>(splinePointsGenerator, transform);
            BindNewInstanceFromPrefab<ObstacleGenerator>(obstacleGenerator, transform);
            BindNewInstanceFromPrefab<BuildingGenerator>(buildingGenerator, transform);
            BindInstance(inputControls);
            BindInstance(this);
            Container.BindInterfacesAndSelfTo<PlayerAccount>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameSettings>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ShopManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AchievementManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<VibrationController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AudioManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<VFXManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneLoadingManager>().AsSingle().NonLazy();
        }

        private void BindInstance<T>(T instance)
        {
            Container.Bind<T>().FromInstance(instance).AsSingle().NonLazy();
        }
        
        private void BindNewInstanceFromPrefab<T>(GameObject prefab, Transform parent = null)
        {
            Container.Bind<T>().FromComponentInNewPrefab(prefab).AsSingle().NonLazy()
                .BindInfo.InstantiatedCallback += (context, o) => ((MonoBehaviour) o).transform.SetParent(parent);
        }

        public GameObject InstantiateAndInject(GameObject obj)
        {
            return Container.InstantiatePrefab(obj);
        }
    }
}