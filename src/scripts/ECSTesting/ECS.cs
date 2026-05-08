using System;
using System.Collections;

using EntityId = System.Int16;
using ComponentId = System.Int16;
using System.Collections.Generic;

struct Archetype
{
    public List<ComponentId> componentIds;
    public HashSet<ComponentId> componentIdSet;
}

class ECS
{
    private Dictionary<EntityId, Archetype> _entities = [];
    private Dictionary<List<ComponentId>, Archetype> _archetypes = [];

    public EntityId CreateEntity()
    {

        return 0;
    }

    public bool HasComponent(EntityId entityId, ComponentId componentId)
    {
        Archetype archetype = _entities[entityId];
        return archetype.componentIdSet.Contains(componentId);
    }

    public ComponentId AddComponent(EntityId entityId, Component component)
    {
        return 0;
    }
}