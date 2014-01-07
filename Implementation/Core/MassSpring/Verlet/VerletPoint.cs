#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2007 Jason Dudash, GNU GPLv3.
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
// File Created: 21 August 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace HBBB.Core.MassSpring.Verlet
{
    /// <summary>
    /// One point that can be constrained to interact with other points in a VerletSystem.
    /// </summary>
    class VerletPoint
    {
        Vector2 textureCoords;
        public Vector2 TextureCoords
        {
            get { return textureCoords; }
            set { textureCoords = value; }
        }

            Vector2 position;
        public Vector2 Position
        {
            get { return position;}
        }

            Vector2 lastPosition;
        public Vector2 LastPosition
        {
            get { return lastPosition;}
        }

        Vector2 force;
        public Vector2 Force
        {
            get { return force;}
            set { force = value;}
        }

            float mass;
        public float Mass
        {
            get { return mass;}
            set { mass = value;}
        }

            // using a vector of constraints
        List<IVerletConstraint> constraints;
        public List<IVerletConstraint> Constraints { get { return constraints; } }
            // collision constraints can be applied separately
        List<IVerletConstraint> collisionConstraints;
        public List<IVerletConstraint> CollisionConstraints { get { return collisionConstraints; } }
            // line segment collisions can be applied separately
        List<IVerletConstraint> collisionLSConstraints;
        public List<IVerletConstraint> CollisionLSConstraints { get { return collisionLSConstraints; } }
        
        /// <summary>
        /// Construct
        /// </summary>
        public VerletPoint()
        {
            textureCoords = new Vector2();
            position = new Vector2();
            lastPosition = new Vector2();
            force = new Vector2();
            mass = 1.0f;

            constraints = new List<IVerletConstraint>();
            collisionConstraints = new List<IVerletConstraint>();
            collisionLSConstraints = new List<IVerletConstraint>();
        }

        /// <summary>
        /// Set the position and the last position to the argument point.  Use this
        /// to absolutely set a point position (deadening velocity), for simulation prefer
        ///  to add a force in the direction of movement.
        /// </summary>
        /// <param name="point"></param>
        public void SetPosition(Vector2 point)
        {
#if DEBUG
            if (float.IsNaN(point.X) || float.IsNaN(point.Y))
            {
                Debug.WriteLine("VerletPoint::SetPosition - new position has NaNs!");
            }
#endif
            position = point;
            lastPosition = position;
        }

        /// <summary>
        /// Set just the current position (creates a velocity based on the distance moved)
        /// </summary>
        /// <param name="point"></param>
        public void MoveTo(Vector2 point)
        {
            position = point;
        }

        /// <summary>
        /// Add the argument force components to the point's force
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddForce(float x, float y)
        {
            force.X += x;
            force.Y += y;
        }

        /// <summary>
        /// Add the argument vector force to the point's force
        /// </summary>
        /// <param name="value"></param>
            public void AddForce(Vector2 value)
        {
            force += value;
        }

        /// <summary>
        /// Set the force of this point
        /// </summary>
        /// <param name="value"></param>
        public void SetForce(float x, float y)
        {
            force.X = x;
            force.Y = y;
        }

        /// <summary>
        /// Set the force of this point
        /// </summary>
        /// <param name="value"></param>
        public void SetForce(Vector2 value)
        {
            force = value;
        }

        /// <summary>
        /// Add a constraint to the point
        /// </summary>
        /// <param name="constraint"></param>
        public void AddConstraint(IVerletConstraint constraint)
        {
            constraints.Add(constraint);
        }

        /// <summary>
        /// Add a collision constraint to the point
        /// </summary>
        /// <param name="constraint"></param>
        public void AddCollisionConstraint(IVerletConstraint constraint)
        {
            collisionConstraints.Add(constraint);
        }

        /// <summary>
        /// Add a line segment collision constraint to the point
        /// </summary>
        /// <param name="constraint"></param>
        public void AddCollisionLSConstraint(IVerletConstraint constraint)
        {
            collisionLSConstraints.Add(constraint);
        }

        /// <summary>
        /// Move point based on a step in time
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Integrate(float deltaTime)
        {
            float x = position.X;
            float y = position.Y;
            position += (position - lastPosition) + (force / mass) * deltaTime * deltaTime;
            lastPosition.X = x;
            lastPosition.Y = y;
        }

        /// <summary>
        /// MAke sure this point 
        /// </summary>
        public void SatisfyConstraints()
        {
            foreach (IVerletConstraint constraint in constraints)
            {
                constraint.Satisfy(this);
            }
        }

        /// <summary>
        /// Make sure this point isn't colliding
        /// </summary>
        public void SatisfyCollisionConstraints()
        {
            foreach (IVerletConstraint constraint in collisionConstraints)
            {
                constraint.Satisfy(this);
            }
        }

        /// <summary>
        /// Make sure the line segments this point is part of are not colliding
        /// </summary>
        public void SatisfyCollisionLSConstraints()
        {
            foreach (IVerletConstraint constraint in collisionLSConstraints)
            {
                constraint.Satisfy(this);
            }
        }

        /// <summary>
        /// Sum up all the forces acting upon this point
        /// </summary>
        public void GatherForces()
        {
            foreach (IVerletConstraint constraint in constraints)
            {
                force += constraint.GetForce(this);
            }
        }

        /// <summary>
        /// Setup a rigid constraint between 2 points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="radius"></param>
        public static void AddRigidConstraint(VerletPoint point1, VerletPoint point2, float radius)
        {
            RigidConstraint constraint1 = new RigidConstraint(point2, radius);
            point1.AddConstraint(constraint1);

            RigidConstraint constraint2 = new RigidConstraint(point1, radius);
            point2.AddConstraint(constraint2);
        }

        /// <summary>
        /// Setup a semi rigid constraint between 2 points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="min"></param>
        /// <param name="mid"></param>
        /// <param name="max"></param>
        /// <param name="force"></param>
        public static void AddSemiRigidConstraint(VerletPoint point1, VerletPoint point2,
            float min, float mid, float max, float force)
        {
            SemiRigidConstraint constraint1 = new SemiRigidConstraint(point2, min, mid, max, force);
            point1.AddConstraint(constraint1);

            SemiRigidConstraint constraint2 = new SemiRigidConstraint(point1, min, mid, max, force);
            point2.AddConstraint(constraint2);
        }

        /// <summary>
        /// Setup a dynamic constraint between two points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="min"></param>
        /// <param name="mid"></param>
        /// <param name="max"></param>
        /// <param name="force"></param>
        public static void AddDynamicConstraint(VerletPoint point1, VerletPoint point2,
            float min, float mid, float max, float force)
        {
            DynamicConstraint constraint1 = new DynamicConstraint(point2, min, mid, max, force);
            point1.AddConstraint(constraint1);

            DynamicConstraint constraint2 = new DynamicConstraint(point1, min, mid, max, force);
            point2.AddConstraint(constraint2);
        }
    }
}
