using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.DataBase;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure.Installers;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.SaveSystem;
using TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData;
using TheProxor.SlimeSimulation.DecorationSystemModule;
using TheProxor.SlimeSimulation.DeformationSystemModule;
using TheProxor.SlimeSimulation.ViewModule;
using UnityEngine;


namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.Infrastructure
{
	public partial class SlimeFacade
	{
		private static readonly int baseColorId = Shader.PropertyToID("_BaseColor");


		public readonly Camera Camera;
		public readonly SlimeMetaGameInstaller.SlimeSettings SlimeSettings;
		public readonly SlimeMetaGameInstaller.SlimeCameraSettings SlimeCameraSettings;

		private readonly SlimeView view;
		private readonly SlimeDecorationSystem.Factory decorationSystemFactory;
		private readonly SlimeMesh slimeMesh;

		private readonly List<string> decorations = new();
		private readonly List<SlimeDecorationSystem> decorationSystems = new();

		private BasisStaticData basis;
		private ISlimeIngredientsManager<SlimeIngredientInfo> slimeIngredientsManager;

		public Transform Transform => view.RendererTransform;

		public SlimeMesh.Settings DeformationSettings
		{
			get => slimeMesh.DeformationSettings;
			set => slimeMesh.DeformationSettings = value;
		}

		public Color Color
		{
			get => Material.GetColor(baseColorId);
			set => Material.SetColor(baseColorId, value);
		}

		public BasisStaticData Basis
		{
			get => basis;

			set
			{
				basis = value;

				if (basis == null)
				{
					return;
				}

				Material.CopyPropertiesFromMaterial(basis.CachedMaterial);
				DeformationSettings = basis.SlimeSettings;
			}
		}

		public float SlimePlayProgress { get; set; }

		public float Size { get; }
		public float Radius { get; }
		public float Height { get; }

		public Material Material => view.Material;
		public Vector3 Position
		{
			get => view.Position;
			set => view.Position = value;
		}

		public Quaternion Rotation => view.Rotation;
		public int DecorationsCount => decorations.Count;


		public SlimeFacade(Camera camera,
						   SlimeMesh slimeMesh,
						   SlimeView view,
						   SlimeDecorationSystem.Factory decorationSystemFactory,
						   SlimeMetaGameInstaller.SlimeSettings slimeSettings,
						   SlimeMetaGameInstaller.SlimeCameraSettings slimeCameraSettings,
						   ISlimeIngredientsManager<SlimeIngredientInfo> slimeIngredientsManager
			)
		{
			this.view = view;
			this.decorationSystemFactory = decorationSystemFactory;
			this.slimeMesh = slimeMesh;
			this.slimeIngredientsManager = slimeIngredientsManager;

			Camera = camera;
			SlimeSettings = slimeSettings;
			SlimeCameraSettings = slimeCameraSettings;

			Size = view.Size.x;
			Height = view.Size.y;
			Radius = Size / 2;

			UpdateMesh();
		}


		public void DeInitialize()
		{
			RemoveDecorations();
			slimeMesh?.Dispose();
		}

		public void SetInteractable(bool value)
		{
			slimeMesh.IsInteractionEnabled = value;
		}


		public void AddDecoration(
			DecorationStaticData decorationData,
			Action<GameObject> onDecorationCreate = null
		)
		{
			decorations.Add(decorationData.Type);
			decorationSystems.Add(CreateDecorationSystem(decorationData, onDecorationCreate));
		}


		public IReadOnlyList<GameObject> GetDecorations(int group)
		{
			return decorationSystems[group].Decorations;
		}


		public void RemoveDecorationGroup(int index)
		{
			decorationSystems[index].Dispose();
			decorations.RemoveAt(index);
			decorationSystems.RemoveAt(index);
		}


		public void RemoveDecorations()
		{
			for (int i = decorations.Count - 1; i >= 0; i--)
			{
				RemoveDecorationGroup(i);
			}
		}


		public SlimeSaveData GetSaveData()
		{
			return new()
			{
				Id = DateTime.Now.GetHashCode().ToString(),
				Basis = basis.Type,
				Color = new ColorData(Color),
				Decorations = decorations.ToArray(),
				PlayProgress = SlimePlayProgress
			};
		}


		public void LoadFromData(
			SlimeSaveData saveData,
			BasisStaticDataProvider basisStaticDataProvider,
			DecorStaticDataProvider decorationStaticDataProvider
		)
		{
			RemoveDecorations();
			Basis = basisStaticDataProvider.GetStaticDataForType(saveData.Basis);
			Color = saveData.Color;
			SlimePlayProgress = saveData.PlayProgress;

			if (saveData.Decorations != null)
			{
				foreach (string decoration in saveData.Decorations)
				{
					AddDecoration(decorationStaticDataProvider.GetStaticDataForType(decoration));
				}
			}
		}


		public void Show()
		{
			view.Enabled = true;
			slimeMesh.Enabled = true;
		}


		public void Hide()
		{
			view.Enabled = false;
			slimeMesh.Enabled = false;
		}


		public void SetDeformationEnabled(bool isEnabled) =>
			slimeMesh.SetDeformationEnabled(isEnabled);


		public SlimeBasisInfo GetBasicsInfo() =>
			slimeIngredientsManager.GetIngredientsCollection<SlimeBasisInfo>()
				.First(x => x.BasisStaticData.Type == Basis.Type);


		public SlimeColorInfo GetColorInfo() =>
			slimeIngredientsManager.GetIngredientsCollection<SlimeColorInfo>()
				.First(x => x.ColorStaticData.Color == Color);


		public ICollection<SlimeDecorationInfo> GetDecorationInfos()
		{
			List<SlimeDecorationInfo> result = new(DecorationsCount);

			foreach (var decoration in decorations)
			{
				result.Add(slimeIngredientsManager.GetIngredientsCollection<SlimeDecorationInfo>()
					 .First(x => x.DecorationStaticData.Type == decoration));
			}

			return result;
		}


		private void UpdateMesh()
		{
			slimeMesh.Mesh = view.Mesh;
		}


		private SlimeDecorationSystem CreateDecorationSystem(
			DecorationStaticData decorationData,
			Action<GameObject> onDecorationCreate
		)
		{
			return decorationSystemFactory.Create(
				decorationData.DecorationGroup,
				onDecorationCreate
			);
		}
	}
}
