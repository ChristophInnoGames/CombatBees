using System;
using System.Collections;
using System.Collections.Generic;
using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleManagerECS : MonoBehaviour {
	public GameObject ParticlePrefab;
	public float speedStretch;

	public static ParticleManagerECS instance;

	const int instancesPerBatch = 1023;
	const int maxParticleCount = 10*instancesPerBatch;

	private EntityManager manager;
	private Entity ParticleEntityPrefab;

	public static void SpawnParticle(Vector3 position,ParticleType type,Vector3 velocity,float velocityJitter=6f,int count=1) {
		for (int i = 0; i < count; i++) {
			instance._SpawnParticle(position,type,velocity,velocityJitter);
		}
	}
	void _SpawnParticle(Vector3 position, ParticleType type, Vector3 velocity, float velocityJitter) {
		// if (particles.Count==maxParticleCount) {
		// 	return;
		// }
		
		Entity particle;

		particle = manager.Instantiate(ParticleEntityPrefab);
		manager.SetComponentData(particle, new Translation {Value = position});
		manager.AddComponentData(particle, new ParticleTag());
		manager.AddComponentData(particle, new Life {Value = 1f});

		if (type==ParticleType.Blood) {
			manager.AddComponentData(particle, new BloodTag());
			manager.AddComponentData(particle, new Velocity {Value = velocity+ Random.insideUnitSphere * velocityJitter});
			manager.AddComponentData(particle, new LifeDuration{Value = Random.Range(3f,5f)});
			manager.SetComponentData(particle, new NonUniformScale {Value = Vector3.one*Random.Range(.1f,.2f)});
			// particle.color = Random.ColorHSV(-.05f,.05f,.75f,1f,.3f,.8f);
		} else if (type==ParticleType.SpawnFlash) {
			manager.AddComponentData(particle, new Velocity {Value =  Random.insideUnitSphere * 5f});
			manager.AddComponentData(particle, new LifeDuration{Value = Random.Range(.25f,.5f)});
			manager.SetComponentData(particle, new NonUniformScale {Value = Vector3.one*Random.Range(1f,2f)});
			// particle.color = Color.white;
		}

		
		// if (pooledParticles.Count == 0) {
		// 	particle = new BeeParticle();
		// } else {
		// 	particle = pooledParticles[pooledParticles.Count - 1];
		// 	pooledParticles.RemoveAt(pooledParticles.Count - 1);
		//
		// 	particle.stuck = false;
		// }
		// particle.type = type;
		//
	}

	void Awake() {
		instance = this;
		
		World world = World.DefaultGameObjectInjectionWorld;
		manager = world.EntityManager;
		GameObjectConversionSettings settings =
			new GameObjectConversionSettings(world, GameObjectConversionUtility.ConversionFlags.AssignName);
		ParticleEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ParticlePrefab, settings);
	}

	private void Start()
	{
		for (int i = 0; i < 500; i++)
		{
			SpawnParticle(float3.zero, ParticleType.Blood, new Vector3(1, 0, 0));
		}
	}

	void Update() {
		// for (int j = 0; j <= activeBatch; j++) {
		// 	int batchSize = instancesPerBatch;
		// 	if (j == activeBatch) {
		// 		batchSize = activeBatchSize;
		// 	}
		// 	int batchOffset = j * instancesPerBatch;
		// 	Matrix4x4[] batchMatrices = matrices[j];
		// 	Vector4[] batchColors = colors[j];
		// 	for (int i = 0; i < batchSize; i++) {
		// 		BeeParticle particle = particles[i + batchOffset];
		//
		// 		if (particle.stuck) {
		// 			batchMatrices[i] = particle.cachedMatrix;
		// 		} else {
		// 			Quaternion rotation = Quaternion.identity;
		// 			Vector3 scale = particle.size * particle.life;
		// 			if (particle.type == ParticleType.Blood) {
		// 				rotation = Quaternion.LookRotation(particle.velocity);
		// 				scale.z *= 1f + particle.velocity.magnitude * speedStretch;
		// 			}
		// 			batchMatrices[i] = Matrix4x4.TRS(particle.position,rotation,scale);
		// 		}
		//
		// 		Color color = particle.color;
		// 		color.a = particle.life;
		// 		batchColors[i] = color;
		// 	}
		// }
		//
		// for (int i = 0; i <= activeBatch; i++) {
		// 	int batchSize = instancesPerBatch;
		// 	if (i==activeBatch) {
		// 		batchSize = activeBatchSize;
		// 	}
		// 	if (batchSize > 0) {
		// 		matProps.SetVectorArray("_Color",colors[i]);
		// 		Graphics.DrawMeshInstanced(particleMesh,0,particleMaterial,matrices[i],batchSize,matProps);
		// 	}
		// }
	}
}
