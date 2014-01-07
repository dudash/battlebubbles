#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2008 Jason Dudash, GNU GPLv3.
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//-----------------------------------------------------------------------------
// File Created: 3 September 2008, GONZ
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using HBBB.GameComponents.PlayerComponents;

namespace HBBB.Core {

public interface IEntity { }

public class EntityContainer : IEntity {
    public delegate void EntityEventHandler(EntityContainer container, IEntity childEntity);
    public event EntityEventHandler AddingChildEntity;
    public event EntityEventHandler RemovingChildEntity;
    
    List<IEntity> ChildEntities = new List<IEntity>();
    
    public IEnumerable<IEntity> GetChildEntities() { return ChildEntities; }
    
    protected void AddChildEntity(IEntity child) {
        Debug.Assert(!ChildEntities.Contains(child));
        ChildEntities.Add(child);
        if (AddingChildEntity != null)
            AddingChildEntity(this, child);
    }

    protected void RemoveChildEntity(IEntity child) {
        Debug.Assert(ChildEntities.Contains(child));
        ChildEntities.Remove(child);
        if (RemovingChildEntity != null)
            RemovingChildEntity(this, child);
    }
}

public interface IPhysicalEntity : IEntity {
    void AddStaticForce(Vector2 force);
    void ProcessPhysics(float timeStepSeconds);
}

// This class coordinates common operations for sets of game entities, similar to how
// Microsoft's Game class calls Update() for each IUpdateable entity, and then Draw()
// for each IDrawable entity sorted according to the Z order.  We could directly continue
// Microsoft's pattern by subclassing the Game class, but BattleBubbles already has too
// much non-reusable clutter in its Game class.
public class EntityManager : IDisposable {
    GameComponentCollection ComponentCollection;

    readonly public List<IPhysicalEntity> PhysicalEntities = new List<IPhysicalEntity>();

    public EntityManager(GameComponentCollection collection) {
        ComponentCollection = collection;
        ComponentCollection.ComponentAdded += new EventHandler<GameComponentCollectionEventArgs>(ComponentCollection_ComponentAdded);
        ComponentCollection.ComponentRemoved += new EventHandler<GameComponentCollectionEventArgs>(ComponentCollection_ComponentRemoved);
    }

    public void Dispose() { // IDisposable
        if (ComponentCollection == null) return;
        ComponentCollection.ComponentRemoved -= new EventHandler<GameComponentCollectionEventArgs>(ComponentCollection_ComponentRemoved);
        ComponentCollection.ComponentAdded -= new EventHandler<GameComponentCollectionEventArgs>(ComponentCollection_ComponentAdded);
        ComponentCollection = null;
    }

    void ComponentCollection_ComponentAdded(object sender, GameComponentCollectionEventArgs e) {
        Debug.WriteLine("+++ COMPONENT: " + e.GameComponent.ToString());
        if (e.GameComponent is IEntity)
            AddEntity((IEntity)e.GameComponent);
    }

    void ComponentCollection_ComponentRemoved(object sender, GameComponentCollectionEventArgs e) {
        Debug.WriteLine("--- COMPONENT: " + e.GameComponent.ToString());
        if (e.GameComponent is IEntity)
            RemoveEntity((IEntity)e.GameComponent);
    }

    void EntityContainer_AddingChildEntity(EntityContainer container, IEntity childEntity) {
        AddEntity(childEntity);
    }

    void EntityContainer_RemovingChildEntity(EntityContainer container, IEntity childEntity) {
        RemoveEntity(childEntity);
    }

    public void AddEntity(IEntity entity) {
        Debug.Assert(entity != null);
        Debug.WriteLine("+++ ENTITY: " + entity.ToString());

        if (entity is EntityContainer) {
            // Recursively add the child entities
            EntityContainer container = (EntityContainer)entity;
            foreach (IEntity child in container.GetChildEntities())
                AddEntity(entity);
            container.AddingChildEntity += new EntityContainer.EntityEventHandler(EntityContainer_AddingChildEntity);
            container.RemovingChildEntity += new EntityContainer.EntityEventHandler(EntityContainer_RemovingChildEntity);
        }
        
        if (entity is IPhysicalEntity) 
            PhysicalEntities.Add((IPhysicalEntity)entity);
    }

    public void RemoveEntity(IEntity entity) {
        Debug.Assert(entity != null);
        Debug.WriteLine("--- ENTITY: " + entity.ToString());
        
        if (entity is EntityContainer) {
            // Recursively remove the child entities
            EntityContainer container = (EntityContainer)entity;
            container.AddingChildEntity -= new EntityContainer.EntityEventHandler(EntityContainer_AddingChildEntity);
            container.RemovingChildEntity -= new EntityContainer.EntityEventHandler(EntityContainer_RemovingChildEntity);
            foreach (IEntity child in container.GetChildEntities())
                RemoveEntity(entity);
        }
        
        if (entity is IPhysicalEntity) 
            PhysicalEntities.Remove((IPhysicalEntity)entity);
    }

    public void UpdatePhysics(GameTime gameTime) {
        const int STEPS_PER_FRAME = Player.AVOID_INVERSION_HACKER;
        
        float timeStepSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        timeStepSeconds /= STEPS_PER_FRAME;
        
        foreach (IPhysicalEntity entity in PhysicalEntities) {

            // process physics 6 times for every update call (avoids inversions of bubbles)
            for (int step = 0; step < STEPS_PER_FRAME; ++step) {
                // upadte the player bubble physics
                entity.ProcessPhysics(timeStepSeconds);
            }
        }
    }
}

}
