#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2007 No Hands Entertainment, All rights reserved.
//-----------------------------------------------------------------------------
// File Created: 23 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core.MassSpring.Verlet;
using System.Diagnostics;
using HBBB.Core;
using HBBB.Core.Graphics;

namespace HBBB.GameComponents.PlayerComponents
{
    /// <summary>
    /// A specalized circular verlet system with an inner and outer circle, and a midpoint
    /// </summary>
    class Bubble : VerletSystem, IPhysicalEntity
    {
        public enum BubbleDrawType { POINTS, LINES, TRIANGLES, TEXTURED };

        // The number of segements making up the skin of the bubble.
        public int Segments;

        // a poiter to the center point of the bubble
        public VerletPoint CenterPoint = new VerletPoint();

        // The radius of the outer skin of the bubble
        public float OuterRadius;
        
        // The radius of the inner skin of the bubble
        public float InnerRadius;

        // The maximum strectched size of the bubble, to be used for high level collision
        // detection.  This should be a number greater than the outer radius.
        public float MaxBoundingRadius;

        // A list of the points composing the outer circle
        public List<VerletPoint> OuterCircle = new List<VerletPoint>();
        
        // A list of the point composing the inner circle
        public List<VerletPoint> InnerCircle = new List<VerletPoint>();

        // The texture to be apples to the inner circle
        public Texture2D InnerCircleTexture;
        
        // The texture to be applied to the outer circle
        public Texture2D OuterCircleTexture;
        
        // Set the texture coordinates for the inner circle and the texture
        public void TextureInnerCircle(Texture2D texture, float radius)
        {
            if (texture == null) throw new Exception("TextureInnerCircle - texture is null");
            if (radius > texture.Width / 2) radius = texture.Width / 2;
            InnerCircleTexture = texture;
            CenterPoint.TextureCoords = new Vector2(texture.Width / 2, texture.Height / 2);
            float angle_step = 2.0f * MathHelper.Pi / (float)InnerCircle.Count;
            // create points around a circle
            for (int i = 0; i < InnerCircle.Count; i++)
            {
                float angle = i * angle_step;
                float x = radius + radius * (float)System.Math.Cos(angle); // inner
                float y = radius + radius * (float)System.Math.Sin(angle);
                InnerCircle[i].TextureCoords = new Vector2(x, y);
            }
        }

        // Set the texture coordinates for the outer circle and the texture
        public void TextureOuterCircle(Texture2D texture, float radius)
        {
            if (texture == null) throw new Exception("TextureOuterCircle - texture is null");
            if (radius > texture.Width / 2) radius = texture.Width / 2;
            OuterCircleTexture = texture;
            CenterPoint.TextureCoords = new Vector2(texture.Width / 2, texture.Height / 2);
            float angle_step = 2.0f * MathHelper.Pi / (float)OuterCircle.Count;
            // create points around a circle
            for (int i = 0; i < InnerCircle.Count; i++)
            {
                float angle = i * angle_step;
                float x = radius + radius * (float)System.Math.Cos(angle); // outer
                float y = radius + radius * (float)System.Math.Sin(angle);
                OuterCircle[i].TextureCoords = new Vector2(x, y);
            }
        }
        
        // draw point forces
        public void DrawDebugForces(PrimitiveBatch primitiveBatch) {
            foreach (VerletPoint p in PointsList)
            {
                primitiveBatch.Begin(PrimitiveType.LineList);
                primitiveBatch.AddVertex(p.Position, Color.Yellow);
                primitiveBatch.AddVertex(p.Position + p.Force, Color.Yellow);
                primitiveBatch.End();
            }
        }
        
        /// <summary> Create a bubble with a thick outer cross-braced double skin to support its structure</summary>
        /// <param name="segments">number of segments in the skin</param>
        /// <param name="x">x location</param>
        /// <param name="y">y location</param>
        /// <param name="inner">radius of the inner circle</param>
        /// <param name="outer">radius of the outer circle</param>
        /// <param name="force">force for constraints in the skin (between inner and outer circle)</param>
        /// <param name="innerForce">force for the constraints between inner circle and center point</param>
        /// <returns></returns>
        public Bubble(int segments, float x, float y, float inner, float outer, float force, float innerForce) 
        {
            float angle_step = 2.0f * MathHelper.Pi / (float) segments;
            float outer_segment_length = (float)2.0f * outer * (float)System.Math.Sin(angle_step / 2.0f);
            float inner_segment_length = (float)2.0f * inner * (float)System.Math.Sin(angle_step / 2.0f);
            float ring_gap = outer - inner;

            const int MAX_SEGMENTS = 500;
            VerletPoint[] pointArray = new VerletPoint[MAX_SEGMENTS];  // array to hold all the points

            VerletPoint midpoint = CreatePoint(new Vector2(x, y)); // Create midpoint
            CenterPoint = midpoint;
            
            Segments = segments;
            OuterRadius = outer;
            InnerRadius = inner;
            MaxBoundingRadius = outer*1.5f;  // guessing is good enough here

            midpoint.Mass = (float)segments;  // set midpoint mass to be a sum of the number of segments

            // create points around a circle
            for (int i = 0; i < segments; i++)
            {
                float angle = i * angle_step;
                float bx = (float)x + inner * (float)System.Math.Cos(angle);
                float by = (float)y + inner * (float)System.Math.Sin(angle);
                float cx = (float)x + outer * (float)System.Math.Cos(angle);
                float cy = (float)y + outer * (float)System.Math.Sin(angle);
                pointArray[i * 2] = CreatePoint(new Vector2(cx, cy), 1.0f); // i*2 is outer
                OuterCircle.Add(pointArray[i * 2]);
                pointArray[i * 2 + 1] = CreatePoint(new Vector2(bx, by), 1.0f); // i*2+1 is inner
                InnerCircle.Add(pointArray[i * 2 + 1]);
            }

            // link up with constraints
            for (int j = 0; j < segments; j++)
            {
                int next = (j + 1) % segments;  // modulo so that the last index will wrap around to the first
                // outer ring to outer ring (point beside)
                VerletPoint.AddSemiRigidConstraint(pointArray[j * 2], pointArray[next * 2], outer_segment_length * 0.9f, outer_segment_length, outer_segment_length * 1.1f, 0);
                // inner ring to inner ring (point beside)
                VerletPoint.AddSemiRigidConstraint(pointArray[j * 2 + 1], pointArray[next * 2 + 1], inner_segment_length * 0.9f, inner_segment_length, inner_segment_length * 1.1f, force);
                // join inner and outer rings
                VerletPoint.AddSemiRigidConstraint(pointArray[j * 2], pointArray[j * 2 + 1], ring_gap * 0.9f, ring_gap, ring_gap * 1.1f, force);
                // and a cross-brace between the inner and outer rings
                VerletPoint.AddSemiRigidConstraint(pointArray[j * 2], pointArray[next * 2 + 1], ring_gap * 0.9f, ring_gap, ring_gap * 1.1f, force);
                // connect inner ring to center point
                VerletPoint.AddDynamicConstraint(pointArray[j * 2 + 1], midpoint, inner * .2f, inner * 1.5f, inner * 2.1f, innerForce);
            }

        }

        #region IPhysicalEntity Members

        public void AddStaticForce(Vector2 force) // IPhysicalEntity
        {
            foreach (VerletPoint p in PointsList) 
            {
                p.AddForce(force * p.Mass);  // add player acceleration to the point
                p.GatherForces();  // gather the forces applied the constraints on this point
            }
        }
        
        public void ProcessPhysics(float timeStepSeconds) { // IPhysicalEntity
            // for each point: integrate to apply forces
            foreach (VerletPoint p in PointsList)
            {
                p.Integrate(timeStepSeconds);  // move this point
            }

            // constrain point to make sure outer ring never overlaps inner ring
            foreach (VerletPoint p in PointsList)
            {
                
            }

            // constrain point based on it's attachments to other verlet points
            foreach (VerletPoint p in PointsList)
            {
                p.SatisfyConstraints();
            }

            // constrain point based on collision with the world lines
            foreach (VerletPoint p in PointsList)
            {
                p.SatisfyCollisionConstraints();
            }
            // constrain point's line segment based on collision with world lines
            foreach (VerletPoint p in PointsList)
            {
                p.SatisfyCollisionLSConstraints();
            }

            foreach (VerletPoint p in PointsList) {
                p.Force = Vector2.Zero;
            }
        }

        #endregion
    }
}
