using System;
using System.Collections.Generic;
using System.Linq;
using ColorRoad.ScriptableObjects;
using ColorRoad.Systems.Messages;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace ColorRoad.Systems
{
    [CreateAssetMenu(order = 4)]
    public class SkinHelper : SerializedScriptableObject
    {
        public BallSkin specialSkin;
        public BallSkin ballSkin;
        public RoadSkin roadSkin;
        public TailSkin tailSkin;
        [OdinSerialize] private Dictionary<GameColor, Material> ballMaterials;
        [OdinSerialize] private Dictionary<GameColor, Material> trampolineSmallMaterials;
        [OdinSerialize] private Dictionary<GameColor, Material> trampolineBigMaterials;

        [BoxGroup("Environment", centerLabel: true), SerializeField]
        private float transitionTime = 1f;
        [BoxGroup("Environment"), OdinSerialize]
        private Dictionary<GameColor, EnvironmentColors> environmentColors;
        [BoxGroup("Environment"), OdinSerialize]
        private Dictionary<GameColor, Material> buildingMaterials;

        [Serializable, InlineProperty]
        private struct EnvironmentColors
        {
            public Color ambient;
            public Color fog;
            public Color sky;
        }

        private static SkinHelper Instance => CachedResources.Get<SkinHelper>("SkinHelperInfo");

        public static void UpdateGameColorPalette(GameColor? oldColor = null, GameColor? newColor = null)
        {
            GameColors.ActiveColors = Instance.ballSkin.colors;
            GameColors.ColorReplacement = (oldColor, newColor);
        }

        public static void UpdateSkins(BallSkin ballSkin, RoadSkin roadSkin, TailSkin tailSkin)
        {
            var missingColorsEnumerable = Instance.ballSkin.colors.Except(ballSkin.colors);
            GameColor? missingColor = missingColorsEnumerable.Select(color => (GameColor?) color).SingleOrDefault();
            var newColorsEnumerable = ballSkin.colors.Except(Instance.ballSkin.colors);
            GameColor? newColor = newColorsEnumerable.Select(color => (GameColor?) color).SingleOrDefault();
            Instance.ballSkin = ballSkin;
            Instance.roadSkin = roadSkin;
            Instance.tailSkin = tailSkin;
            UpdateGameColorPalette(missingColor, newColor);
            MessageBus.Post(new GenericMessage(GenericMessageType.SkinChanged));
        }

        public static void ApplyBallSkin(MeshRenderer meshRenderer, GameColor color, bool isSpecial)
        {
            meshRenderer.material = Instance.ballMaterials[color];

            if (isSpecial)
            {
                // meshRenderer.material = Instance.specialSkin.material;
                meshRenderer.material.mainTexture = Instance.specialSkin.GetTextureByColor(color);
            }
            else
            {
                // meshRenderer.material = Instance.ballSkin.material;
                meshRenderer.material.mainTexture = Instance.ballSkin.GetTextureByColor(color);
            }
        }
        
        public static void ApplyTrampolineSkin(MeshRenderer meshRenderer, GameColor color, bool isSmall)
        {
            if (isSmall)
                meshRenderer.material = Instance.trampolineSmallMaterials[color];
            else
                meshRenderer.material = Instance.trampolineBigMaterials[color];
        }
        
        public static void ApplyRoadSkin(MeshRenderer meshRenderer, GameColor color)
        {
            meshRenderer.material = Instance.roadSkin.material;
            meshRenderer.material.mainTexture = Instance.roadSkin.GetTextureByColor(color);
        }

        public static void ApplyBuildingSkin(MeshRenderer meshRenderer, GameColor color)
        {
            meshRenderer.material = Instance.buildingMaterials[color];
        }

        public static void ApplyEnvironmentColors(GameColor color)
        {
            DOTween.To(() => RenderSettings.ambientLight, value => RenderSettings.ambientLight = value,
                Instance.environmentColors[color].ambient, Instance.transitionTime);
            DOTween.To(() => RenderSettings.fogColor, value => RenderSettings.fogColor = value,
                Instance.environmentColors[color].fog, Instance.transitionTime);
            Camera.main.DOColor(Instance.environmentColors[color].sky, Instance.transitionTime);
        }

        public static GameObject GetTailPrefab()
        {
            return Instance.tailSkin.effectPrefab;
        }
    }
}