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
// File Created: 24 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace HBBB.Core.Graphics
{

    /// <summary>
    /// PrimitiveBatch is a class that handles primitive rendering in a similar way
    /// to SpriteBatch. PrimitiveBatch can render line lists, point lists, and triangle lists
    /// </summary>
    public class PrimitiveBatch : IDisposable
    {
        /// <summary>
        /// this constant controls how large the vertices buffer is. Larger buffers will
        /// require flushing less often, which can increase performance. However, having
        /// buffer that is unnecessarily large will waste memory.
        /// </summary>
        const int DefaultBufferSize = 500;

        /// <summary>
        /// a block of vertices that calling AddVertex will fill. Flush will draw using
        /// this array, and will determine how many primitives to draw from
        /// positionInBuffer.
        /// </summary>
        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[DefaultBufferSize];

        /// <summary>
        /// keeps track of how many vertices have been added. this value increases until
        /// we run out of space in the buffer, at which time Flush is automatically
        /// called.
        /// </summary>
        int positionInBuffer = 0;

        /// <summary>
        /// the vertex declaration that will be set on the device for drawing. this is 
        /// created automatically using VertexPositionColor's vertex elements.
        /// </summary>
        VertexDeclaration vertexDeclaration;

        /// <summary>
        /// a basic effect, which contains the shaders that we will use to draw our
        /// primitives.
        /// </summary>
        BasicEffect basicEffect;

        /// <summary>
        /// the device that we will issue draw calls to.
        /// </summary>
        GraphicsDevice device;

        /// <summary>
        /// this value is set by Begin, and is the type of primitives that we are
        // drawing.
        /// </summary>
        PrimitiveType primitiveType;

        /// <summary>
        /// how many verts does each of these primitives take up? points are 1,
        /// lines are 2, and triangles are 3.
        /// </summary>
        int numVertsPerPrimitive;

        /// <summary>
        /// hasBegun is flipped to true once Begin is called, and is used to make
        /// sure users don't call End before Begin is called.
        /// </summary>
        bool hasBegun = false;

        bool isDisposed = false;

        /// <summary>
        /// the constructor creates a new PrimitiveBatch and sets up all of the internals
        /// that PrimitiveBatch will need.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public PrimitiveBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null) throw new ArgumentNullException("graphicsDevice");
            device = graphicsDevice;

            // create a vertex declaration, which tells the graphics card what kind of
            // data to expect during a draw call. We're drawing using
            // VertexPositionColors, so we'll use those vertex elements.
            vertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionColor.VertexElements);

            // set up a new basic effect, and enable vertex colors.
            basicEffect = new BasicEffect(graphicsDevice, null);
            basicEffect.VertexColorEnabled = true;

            // projection uses CreateOrthographicOffCenter to create 2d projection matrix with 0,0 in the upper left.
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width,graphicsDevice.Viewport.Height, 0, 0, 1);
        }

        /// <summary>
        /// public dispose function
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose protected implementation
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                if (vertexDeclaration != null)
                    vertexDeclaration.Dispose();

                if (basicEffect != null)
                    basicEffect.Dispose();

                isDisposed = true;
            }
        }

        /// <summary>
        /// Set the transparency (alpha) of the primitive
        /// </summary>
        /// <param name="alpha"></param>
        public void SetAlpha(float alpha)
        {
            basicEffect.Alpha = alpha;
        }

        /// <summary>
        /// Set the texture applied to the primitive
        /// </summary>
        /// <param name="texture"></param>
        public void SetTexture(Texture2D texture)
        {
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = texture;
        }

        /// <summary>
        /// Begin is called to tell the PrimitiveBatch what kind of primitives will be
        /// drawn, and to prepare the graphics card to render those primitives.
        /// </summary>
        /// <param name="primitiveType"></param>
        public void Begin(PrimitiveType primitiveType)
        {
            if (hasBegun) throw new InvalidOperationException ("End must be called before Begin can be called again.");

            // these three types reuse vertices, so we can't flush properly without more
            // complex logic. Since that's a bit too complicated for this sample, we'll
            // simply disallow them.
            if (primitiveType == PrimitiveType.LineStrip ||
                primitiveType == PrimitiveType.TriangleFan ||
                primitiveType == PrimitiveType.TriangleStrip)
            {
                throw new NotSupportedException ("The specified primitiveType is not supported by PrimitiveBatch.");
            }

            this.primitiveType = primitiveType;

            // how many verts will each of these primitives require?
            this.numVertsPerPrimitive = NumVertsPerPrimitive(primitiveType);

            // prepare the graphics device for drawing by setting the vertex declaration
            // and telling our basic effect to begin.
            device.VertexDeclaration = vertexDeclaration;
            basicEffect.Begin();
            basicEffect.CurrentTechnique.Passes[0].Begin();

            // flip the error checking boolean. It's now ok to call AddVertex, Flush,
            // and End.
            hasBegun = true;
        }

        /// <summary>
        /// AddVertex is called to add another vertex to be rendered. To draw a point,
        /// AddVertex must be called once. for lines, twice, and for triangles 3 times.
        /// this function can only be called once begin has been called.
        /// if there is not enough room in the vertices buffer, Flush is called automatically.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="color"></param>
        public void AddVertex(Vector2 vertex, Color color)
        {
            if (!hasBegun) throw new InvalidOperationException ("Begin must be called before AddVertex can be called.");
            // are we starting a new primitive? if so, and there will not be enough room for a whole primitive, flush.
            bool newPrimitive = ((positionInBuffer % numVertsPerPrimitive) == 0);

            if (newPrimitive && (positionInBuffer + numVertsPerPrimitive) >= vertices.Length) Flush();

            // once we know there's enough room, set the vertex in the buffer, and increase position.
            vertices[positionInBuffer].Position = new Vector3(vertex, 0);
            vertices[positionInBuffer].Color = color;

            positionInBuffer++;
        }

        /// <summary>
        /// AddVertex is called to add another vertex to be rendered. To draw a point,
        /// AddVertex must be called once. for lines, twice, and for triangles 3 times.
        /// this function can only be called once begin has been called.
        /// if there is not enough room in the vertices buffer, Flush is called automatically. 
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="textureCoords"></param>
        public void AddVertex(Vector2 vertex, Vector2 textureCoords)
        {
            if (!hasBegun) throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
            // are we starting a new primitive? if so, and there will not be enough room for a whole primitive, flush.
            bool newPrimitive = ((positionInBuffer % numVertsPerPrimitive) == 0);

            if (newPrimitive && (positionInBuffer + numVertsPerPrimitive) >= vertices.Length) Flush();

            // once we know there's enough room, set the vertex in the buffer, and increase position.
            vertices[positionInBuffer].Position = new Vector3(vertex, 0);
            vertices[positionInBuffer].TextureCoordinate = textureCoords;
            vertices[positionInBuffer].Color = Color.White;

            positionInBuffer++;
        }

        /// <summary>
        /// End is called once all the primitives have been drawn using AddVertex.
        /// it will call Flush to actually submit the draw call to the graphics card, and
        /// then tell the basic effect to end.
        /// </summary>
        public void End()
        {
            if (!hasBegun) throw new InvalidOperationException("Begin must be called before End can be called.");

            // Draw whatever the user wanted us to draw
            Flush();

            // and then tell basic effect that we're done.
            basicEffect.CurrentTechnique.Passes[0].End();
            basicEffect.End();
            hasBegun = false;
        }

        /// <summary>
        /// Flush is called to issue the draw call to the graphics card. Once the draw
        /// call is made, positionInBuffer is reset, so that AddVertex can start over
        /// at the beginning. End will call this to draw the primitives that the user
        /// requested, and AddVertex will call this if there is not enough room in the
        /// buffer.
        /// </summary>
        private void Flush()
        {
            if (!hasBegun) throw new InvalidOperationException("Begin must be called before Flush can be called.");

            // no work to do
            if (positionInBuffer == 0) return;

            // how many primitives will we draw?
            int primitiveCount = positionInBuffer / numVertsPerPrimitive;

            // submit the draw call to the graphics card
            device.DrawUserPrimitives<VertexPositionColorTexture>(primitiveType, vertices, 0, primitiveCount);

            // now that we've drawn, it's ok to reset positionInBuffer back to zero,
            // and write over any vertices that may have been set previously.
            positionInBuffer = 0;
        }

        /// <summary>
        /// NumVertsPerPrimitive is a boring helper function that tells how many vertices
        /// it will take to draw each kind of primitive.
        /// </summary>
        /// <param name="primitive"></param>
        /// <returns></returns>
        static private int NumVertsPerPrimitive(PrimitiveType primitive)
        {
            int numVertsPerPrimitive;
            switch (primitive)
            {
                case PrimitiveType.PointList:
                    numVertsPerPrimitive = 1;
                    break;
                case PrimitiveType.LineList:
                    numVertsPerPrimitive = 2;
                    break;
                case PrimitiveType.TriangleList:
                    numVertsPerPrimitive = 3;
                    break;
                default:
                    throw new InvalidOperationException("primitive is not valid");
            }
            return numVertsPerPrimitive;
        }
    }
}
