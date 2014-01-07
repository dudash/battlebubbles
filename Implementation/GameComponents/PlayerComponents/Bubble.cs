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
        class Spoke
        {
            public VerletPoint Point;
            
            // This is the spoke's angle when the bubble is in its initial relaxed condition
            // with orientation=0
            public double BaseAngle;

            // This is the initial radius for this point, either InnerRadius or OuterRadius
            public double BaseRadius;

            // This is the angle measured by CalculateOrientation().  If the bubble is not
            // deformed at all, then MeasuredAngle = BaseAngle + orientation
            public double MeasuredAngle;
        }

        public enum BubbleDrawType { POINTS, LINES, TRIANGLES, TEXTURED };

        // This controls the bounciness of the bubble against the walls.  It specifies how 
        // many seconds it takes for CenterPoint to be moved halfway to averagePosition 
        // along an exponential curve.  (The effect is essentially equivalent to simple
        // harmonic motion, but a little easier to think about.)
        public double CenterHalfLifeSecs = 0.4;  //0.02
        public const double CHALFLIFE_DEFAULT_BUB = 0.4;
        public const double CHALFLIFE_DEFAULT_SOLID = 0.1;

        // This controls how quickly the bubbles regain their original shape after being
        // deformed.  It specifies how many seconds it takes for a spoke to be moved halfway 
        // to its undistorted position, i.e. if you set it to 0 then the bubble will always
        // be a perfect circle.
        public double SpokeHalfLifeSecs = 0.07; //0.05;
        public const double SHALFLIFE_DEFAULT_BUB = 0.07;
        public const double SHALFLIFE_DEFAULT_SOLID = 0.02;

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

        // This is a sorted list of { OuterCircle[0], InnerCircle[0], OuterCircle[1], ... }
        List<Spoke> Spokes = new List<Spoke>();

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
        
        // Angles are not unique because e.g. -2*Pi =  0 = 2*Pi.   GetNormalizedAngle() returns an equivalent
        // angle that is closest to 0.  Since -Pi and Pi are equally close to 0, Pi is chosen arbitrarily. 
        static public double GetNormalizedAngle(double angle) {
            while (angle <= -Math.PI + 1e-5) 
                angle += Math.PI*2;
            while (angle > Math.PI) 
                angle -= Math.PI*2;
            return angle;
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
/*
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
*/

            Segments = segments;
            OuterRadius = outer;
            InnerRadius = inner;
            MaxBoundingRadius = outer*1.5f;  // guessing is good enough here

            CenterPoint = CreatePoint(new Vector2(x, y)); // Create midpoint
            CenterPoint.Mass = (float)segments;  // set midpoint mass to be a sum of the number of segments

            // The number of spokes = Segments*2 (for inner and outer)
            int numSpokes = Segments*2;

            for (int i = 0; i < numSpokes; ++i)
            {
                Spoke spoke = new Spoke();
                Spokes.Add(spoke);

                spoke.BaseAngle = (2*MathHelper.Pi) * i / numSpokes;
                spoke.Point = CreatePoint(Vector2.Zero, 1.0f);

                if ((i % 2) == 0) {
                    // add a point to the outer circle
                    spoke.BaseRadius = outer;
                    OuterCircle.Add(spoke.Point);
                } else {
                    // add a point to the inner circle
                    spoke.BaseRadius = inner;
                    InnerCircle.Add(spoke.Point);
                }

                // Set the initial position
                double pointX = x + spoke.BaseRadius * System.Math.Cos(spoke.BaseAngle);
                double pointY = y + spoke.BaseRadius * System.Math.Sin(spoke.BaseAngle);
                spoke.Point.SetPosition(new Vector2((float)pointX, (float)pointY));
            }

            // Check that the initial orientation is 0
            Debug.Assert(Math.Abs(CalculateOrientation()) < 0.01);

/*
            // NOTE: If you uncomment these, you need to uncomment the "p.SatisfyConstraints()" loop below

            // Add constraints between outer and inner circle
            for (int i=0; i<numSpokes; ++i) {
                VerletPoint left = Spokes[i].Point;
                VerletPoint right = Spokes[(i+1)%numSpokes].Point;

                float gap = (left.Position - right.Position).Length();

                VerletPoint.AddSemiRigidConstraint(left, right, gap * 0.9f, gap, gap * 1.1f, 5);
            }

            // Add constraints between outer circle spokes
            for (int i=0; i<OuterCircle.Count; ++i) {

                VerletPoint left = OuterCircle[i];
                VerletPoint right = OuterCircle[(i+1)%OuterCircle.Count];

                float gap = (left.Position - right.Position).Length();

                VerletPoint.AddSemiRigidConstraint(left, right, gap * 0.9f, gap, gap * 1.1f, 10);
            }

            // Add constraints between inner circle and center
            CenterPoint.Mass = InnerCircle.Count;
            for (int i=0; i<InnerCircle.Count; ++i) {
                VerletPoint left = InnerCircle[i];
                VerletPoint right = CenterPoint;

                float gap = (left.Position - right.Position).Length();

                VerletPoint.AddSemiRigidConstraint(left, right, gap * 0.9f, gap, gap * 1.1f, 5);
            }
*/
        }

        #region IPhysicalEntity Members

        public void AddStaticForce(Vector2 force) // IPhysicalEntity
        {
            foreach (VerletPoint p in PointsList)
            {
                p.AddForce(force * p.Mass);  // add player acceleration to the point
            }
        }

        // Measure the bubble's orientation as the average (across spokes) of the difference
        // from the base angle
        public double CalculateOrientation() {
            double averageAngleDiff = 0;
            for (int i=0; i<Spokes.Count; ++i) {
                Spoke spoke = Spokes[i];

                Vector2 spokeVector = spoke.Point.Position - CenterPoint.Position;

                // Measure the angle and radius of the spoke
                spoke.MeasuredAngle = Math.Atan2(spokeVector.Y, spokeVector.X);

                // Subtract the initial angle to get the difference
                double angleDiff = GetNormalizedAngle(spoke.MeasuredAngle - spoke.BaseAngle);

                // Tally the average
                averageAngleDiff += angleDiff;
            }
            averageAngleDiff /= Spokes.Count;

            return averageAngleDiff;
        }
        
        // Given a halfLife (in the same units as timeStep), this returns the alpha coefficient
        // used with GetLerped().  A half-life of 0 returns alpha of 1, i.e. the goal is attained
        // instantly.  A very large half-lfe returns an alpha of 0, i.e. the goal is never
        // attained.
        // @@ We could eliminate the Math.Exp() call and make this a constant by changing the
        // physics to run on a fixed time step (decoupled from rendering).  This would also 
        // reduce other variations of behavior caused by varying time steps.
        static double GetAlphaFromHalfLife(double halfLife, double timeStep) {
          return 1-Math.Exp(-timeStep / halfLife);
        }
        
        // If alpha=0 then the result is current.  If alpha=1 then the result is goal.
        static float GetLerped(float current, float goal, double alpha) {
            // i.e. (1-alpha)*current + alpha*goal
            return current + ((float)alpha)*(goal - current);
        }

        static Vector2 GetLerped(Vector2 current, Vector2 goal, double alpha) {
            return Vector2.Lerp(current, goal, (float)alpha);
        }

        public void ProcessPhysics(float timeStepSeconds) { // IPhysicalEntity
            foreach (VerletPoint p in PointsList)
            {
                p.GatherForces();  // gather the forces applied the constraints on this point
            }

            // for each point: integrate to apply forces
            foreach (VerletPoint p in PointsList)
            {
                p.Integrate(timeStepSeconds);  // move this point
            }
/*
            // constrain point based on it's attachments to other verlet points
            foreach (VerletPoint p in PointsList)
            {
                p.SatisfyConstraints();
            }
*/
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
  
            // Step 1:  Reposition the center point as the average position of all the spoke points
            Vector2 averagePosition = Vector2.Zero;

            for (int i=0; i<Spokes.Count; ++i) {
                averagePosition += Spokes[i].Point.Position;
            }
            averagePosition /= Spokes.Count;

            Vector2 newCenterPoint = GetLerped(CenterPoint.Position, averagePosition,
                GetAlphaFromHalfLife(CenterHalfLifeSecs, timeStepSeconds));

            //CenterPoint.SetPosition(newCenterPoint);
            CenterPoint.MoveTo(newCenterPoint);

            // Step 2: Approximate the orientation (i.e. angular position) of the bubble
            double orientation = CalculateOrientation();

            // 3. Calculate the ideal position for each spoke, and spring it in that direction

            // (moved this out of the inner loop)
            double spokeAlpha = GetAlphaFromHalfLife(SpokeHalfLifeSecs, timeStepSeconds);

            for (int i=0; i<Spokes.Count; ++i) {
                Spoke spoke = Spokes[i];

                // This is what the angle would be without any distortion of the bubble.
                // I experimented with GetLerped() here, but springy angles
                // are apparently dynamically unstable whereas springy positions work just fine.
                double targetAngle = spoke.BaseAngle + orientation;
                Vector2 targetPosition = new Vector2((float)System.Math.Cos(targetAngle),
                    (float)System.Math.Sin(targetAngle));

                // Calculate where spoke.Point would be if the bubble is not distorted
                targetPosition *= (float)spoke.BaseRadius;
                targetPosition += CenterPoint.Position;

                // spring the bubble towards targetPosition
                Vector2 newPosition = GetLerped(spoke.Point.Position, targetPosition, spokeAlpha);

                //spoke.Point.SetPosition(newPosition);
                spoke.Point.MoveTo(newPosition);
            }

            foreach (VerletPoint p in PointsList) {
                p.Force = Vector2.Zero;
            }
        }

        #endregion
    }
}
