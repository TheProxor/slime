using System.Collections.Generic;
using TheProxor.SlimeSimulation.DeformationSystemModule.Input.MeshDeformCamera;
using TheProxor.SlimeSimulation.InputSystem;
using TheProxor.SlimeSimulation.InputSystem.MultiTouchService;
using UnityEngine;

namespace TheProxor.SlimeSimulation.DeformationSystemModule.Input
{
	public class MeshDeformInput
	{
		private readonly IMeshDeformCamera camera;
		private readonly IMultiTouchService multiTouch;
		private readonly Interaction.Pool interactionPool;
		private readonly Dictionary<ITouch, Interaction> interactionByTouch;
		private readonly List<Interaction> interactions;

		public Vector3 Normal { get; private set; }
		public IReadOnlyList<Interaction> Interactions => interactions;

		public MeshDeformInput(IMeshDeformCamera camera,
							   IMultiTouchService multiTouch,
							   Interaction.Pool interactionPool)
		{
			this.camera = camera;
			this.multiTouch = multiTouch;
			this.interactionPool = interactionPool;

			interactionByTouch = new Dictionary<ITouch, Interaction>();
			interactions = new List<Interaction>();

			UpdateNormal();
			InitializeMultiTouch();
		}

		private void UpdateNormal()
		{
			Normal = camera.Normal;
		}

		private void InitializeMultiTouch()
		{
			multiTouch.OnTouchAdded += AddTouch;
			multiTouch.OnTouchRemoved += RemoveTouch;
		}

		private void AddTouch(ITouch touch)
		{
			UpdateNormal();
			Interaction interaction = interactionPool.Spawn(touch);
			interactionByTouch.Add(touch, interaction);
			interactions.Add(interaction);
		}

		private void RemoveTouch(ITouch touch)
		{
			Interaction interaction = interactionByTouch[touch];
			interactionPool.Despawn(interaction);
			interactions.Remove(interaction);
			interactionByTouch.Remove(touch);
		}
	}
}
