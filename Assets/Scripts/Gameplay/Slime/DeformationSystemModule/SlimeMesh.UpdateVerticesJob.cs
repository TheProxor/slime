using TheProxor.SlimeSimulation.ViewModule;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TheProxor.SlimeSimulation.DeformationSystemModule
{
	public partial class SlimeMesh
	{
		[BurstCompile]
		public struct UpdateVerticesJob : IJobParallelFor
		{
			public readonly struct Interaction
			{
				public readonly float3 Origin;
				public readonly float3 Normal;
				public readonly float3 Direction;
				public readonly float3 Shift;
				public readonly float ShiftSpeed;
				public readonly float PressIntensity;

				public Interaction(Vector3 normal,
								   Input.Interaction interaction,
								   SlimeView view)
				{
					Matrix4x4 matrix = view.WorldToLocalMatrix;
					Origin = matrix.MultiplyPoint(interaction.Origin);
					Normal = matrix.MultiplyVector(normal);
					Direction = matrix.MultiplyVector(interaction.Direction);
					Shift = matrix.MultiplyVector(interaction.Shift);
					ShiftSpeed = interaction.ShiftSpeed;
					PressIntensity = interaction.PressIntensity;
				}
			}

			[ReadOnly]
			[DeallocateOnJobCompletion]
			private NativeArray<Interaction> interactions;
			private NativeArray<float3> vertices;
			private NativeArray<VertexData> verticesData;
			private float deltaTime;
			private Settings deformSettings;
			private bool allowDeformation;

			public NativeArray<Interaction> Interactions
			{
				set => interactions = value;
			}

			public float DeltaTime
			{
				set => deltaTime = value;
			}

			public Settings Settings
			{
				set => deformSettings = value;
			}

			public bool AllowDeformation
			{
				set => allowDeformation = value;
			}


			public UpdateVerticesJob(NativeArray<float3> vertices,
									 NativeArray<VertexData> verticesData,
									 Settings deformSettings)
			{
				this.vertices = vertices;
				this.verticesData = verticesData;
				this.deformSettings = deformSettings;
				deltaTime = default;
				interactions = default;
				allowDeformation = false;
			}


			public void Execute(int index)
			{
				float3 vertex = vertices[index];
				VertexData data = verticesData[index];
				float3 originalPosition = data.OriginalPosition;
				float3 velocity = data.Velocity;
				float accumulatedVelocity = data.AccumulatedVelocity;

				float3 force = float3.zero;
				float3 pressForce = float3.zero;

				foreach (Interaction interaction in interactions)
				{
					float3 normal = interaction.Normal;

					float3 pointToVertex = PointToVertex(vertex,
														 normal,
														 interaction.Origin,
														 interaction.Direction);

					float3 shiftForce = GetShiftForce(interaction.Shift,
													  accumulatedVelocity,
													  pointToVertex);

					accumulatedVelocity += AccumulateVelocity(shiftForce);
					force += shiftForce;

					pressForce = GetPressForce(
						vertex,
						originalPosition,
						pointToVertex,
						normal,
						interaction.ShiftSpeed,
						interaction.PressIntensity
					);

					force += pressForce;
				}

				velocity = UpdateVertexDataVelocity(velocity + force, vertex, originalPosition);

				accumulatedVelocity = UpdateAccumulatedVelocity(accumulatedVelocity);


				if (!allowDeformation)
				{
					vertices[index] = originalPosition;
					velocity = accumulatedVelocity = 0.0f;
					UpdateVertexData(index, data, velocity, accumulatedVelocity);
					return;
				}

				UpdateVertex(index, vertex, velocity);
				UpdateVertexData(index, data, velocity, accumulatedVelocity);
			}


			private static float3 PointToVertex(float3 position,
												float3 normal,
												float3 origin,
												float3 direction)
			{
				float3 forcePosition = GetForcePosition(position, normal, origin, direction);
				return position - forcePosition;
			}

			private static float3 GetForcePosition(float3 at,
												   float3 normal,
												   float3 origin,
												   float3 direction)
			{
				float3 planeNormal = normal;
				float3 linePoint = origin;
				float3 lineDirection = direction;

				float t = (math.dot(planeNormal, at) - math.dot(planeNormal, linePoint))
					/ math.dot(planeNormal, lineDirection);

				return linePoint + lineDirection * t;
			}

			private float3 GetShiftForce(float3 shift,
										 float accumulatedVelocity,
										 float3 pointToVertex)
			{
				float inverseSquare = InverseSquareLaw(pointToVertex);


				float3 force = shift * (deformSettings.ShiftForce / (1 + accumulatedVelocity));
				force /= deltaTime;
				force /= 100;

				float3 attenuatedForce = force
										 * inverseSquare
										 / (deformSettings.ShiftSmooth * (1 - inverseSquare) + 1);

				return attenuatedForce;
			}

			private static float InverseSquareLaw(float3 pointToVertex)
			{
				return 1 / (1 + math.lengthsq(pointToVertex));
			}

			private float AccumulateVelocity(float3 shiftVelocity)
			{
				return math.length(shiftVelocity) * deformSettings.ShiftAccumulationFactor;
			}

			private float UpdateAccumulatedVelocity(float accumulatedVelocity)
			{
				return MoveTowards(accumulatedVelocity,
								   0,
								   deltaTime * deformSettings.ShiftAccumulationDecreaseSpeed);
			}

			private static float MoveTowards(float current, float target, float maxDelta)
			{
				if (math.abs(target - current) <= maxDelta)
				{
					return target;
				}

				return current + math.sign(target - current) * maxDelta;
			}

			private float3 GetPressForce(float3 displacedPosition,
										 float3 originalPosition,
										 float3 pointToVertex,
										 float3 normal,
										 float shiftSpeed,
										 float pressIntensity)
			{
				float3 velocity = normal * deformSettings.PushForce;

				velocity = ApplyPressIntensity(velocity, pressIntensity);

				velocity = ApplyPressDistanceFactor(velocity, pointToVertex);

				velocity = ApplyPressDepthFactor(velocity,
												 displacedPosition,
												 originalPosition,
												 normal);

				velocity = ApplyPressSpeedFactor(velocity, shiftSpeed);

				velocity /= deltaTime;
				velocity /= 10000;
				//коэффициент 10000 изза того что заменил умножить на разделить на дельтатайм
				//иначе в пришлось бы менять настройки в инсталлерах в 10 тысяч раз

				return velocity;
			}

			private float3 ApplyPressIntensity(float3 velocity, float pressIntensity)
			{
				return velocity * pressIntensity;
			}

			private float3 ApplyPressDistanceFactor(float3 velocity, float3 pointToVertex)
			{
				return velocity * GetPressDistanceFactor(pointToVertex);
			}

			private float GetPressDistanceFactor(float3 pointToVertex)
			{
				return 1 - math.clamp(math.length(pointToVertex) / deformSettings.PushRadius, 0, 1);
			}

			private float3 ApplyPressDepthFactor(float3 velocity,
												 float3 displacedPosition,
												 float3 originalPosition,
												 float3 normal)
			{
				return velocity * GetPressDeepFactor(displacedPosition, originalPosition, normal);
			}

			private float GetPressDeepFactor(float3 displacedPosition,
											 float3 originalPosition,
											 float3 normal)
			{
				float vertexDataDepth =
					GetVertexDataDepth(displacedPosition, originalPosition, normal);

				return 1 / (1 + vertexDataDepth * deformSettings.PushDepthFactor);
			}

			private float GetVertexDataDepth(float3 displacedPosition,
											 float3 originalPosition,
											 float3 normal)
			{
				return math.dot(displacedPosition - originalPosition, normal);
			}

			private float3 ApplyPressSpeedFactor(float3 velocity, float shiftSpeed)
			{
				return velocity * GetPressSpeedFactor(shiftSpeed);
			}

			private float GetPressSpeedFactor(float shiftSpeed)
			{
				return 1 + shiftSpeed * deformSettings.PressSpeedFactor;
			}

			private float3 UpdateVertexDataVelocity(float3 velocity,
													float3 position,
													float3 originalPosition)
			{
				float3 displacement = position - originalPosition;
				velocity -= displacement * (deformSettings.SpringForce * 0.015f);
				velocity *= 1f - deformSettings.Damping * 0.015f;
				//заменил deltaTime на 0.015 (1/60) изза того что в этом месте вообще не должно быть
				//ни умножить ни разделить на дельтатайм, и чтобы не менять настройки в инсталлерах

				float vel = math.length(velocity);
				if (vel > deformSettings.MaxShiftVelocity)
					velocity = math.normalize(velocity) * deformSettings.MaxShiftVelocity;

				return velocity;
			}

			private void UpdateVertex(int index, float3 vertex, float3 velocity)
			{
				vertices[index] = ApplyVertexVelocity(vertex, velocity);
			}

			private float3 ApplyVertexVelocity(float3 position, float3 velocity)
			{
				return position + velocity * deltaTime;
			}

			private void UpdateVertexData(int index,
										  VertexData data,
										  float3 velocity,
										  float accumulatedVelocity)
			{
				data.Velocity = velocity;
				data.AccumulatedVelocity = accumulatedVelocity;
				verticesData[index] = data;
			}
		}
	}
}
