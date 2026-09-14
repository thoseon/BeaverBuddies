using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.TemplateAttachmentSystem;
using UnityEngine;

namespace Timberborn.Particles;

public class ParticlesCache : BaseComponent, IAwakableComponent, IDeletableEntity
{
	private static readonly string IdSeparator = ",";

	private readonly ParticlesFastForwarder _particlesFastForwarder;

	private ParticlesRunnerCreator _particlesRunnerCreator;

	private TemplateAttachments _templateAttachments;

	private readonly Dictionary<string, ParticlesRunner> _particlesRunnerCache = new Dictionary<string, ParticlesRunner>();

	public ParticlesCache(ParticlesFastForwarder particlesFastForwarder)
	{
		_particlesFastForwarder = particlesFastForwarder;
	}

	public void Awake()
	{
		_particlesRunnerCreator = GetComponent<ParticlesRunnerCreator>();
		_templateAttachments = GetComponent<TemplateAttachments>();
	}

	public void DeleteEntity()
	{
		foreach (ParticlesRunner value in _particlesRunnerCache.Values)
		{
			_particlesFastForwarder.Unregister(value);
		}
	}

	public ParticlesRunner GetParticlesRunner(string attachmentId)
	{
		return GetParticlesRunner(ImmutableArray.Create(attachmentId));
	}

	public ParticlesRunner GetParticlesRunner(IList<string> attachmentIds)
	{
		string cacheKey = GetCacheKey(attachmentIds);
		if (_particlesRunnerCache.TryGetValue(cacheKey, out var value))
		{
			return value;
		}
		ParticlesRunner particlesRunner = _particlesRunnerCreator.Create(CreateParticleAttachments(attachmentIds));
		_particlesFastForwarder.Register(particlesRunner);
		_particlesRunnerCache[cacheKey] = particlesRunner;
		return particlesRunner;
	}

	private static string GetCacheKey(IEnumerable<string> attachmentIds)
	{
		return string.Join(IdSeparator, attachmentIds.OrderBy((string attachmentId) => attachmentId));
	}

	private List<ParticleSystem> CreateParticleAttachments(IEnumerable<string> attachmentIds)
	{
		List<ParticleSystem> list = new List<ParticleSystem>();
		foreach (string attachmentId in attachmentIds)
		{
			list.AddRange(_templateAttachments.GetOrCreateAttachment(attachmentId).Transform.GetComponentsInChildren<ParticleSystem>(includeInactive: true));
		}
		return list;
	}
}
