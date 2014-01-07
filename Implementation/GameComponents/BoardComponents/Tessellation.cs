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
// File Created: 20 November 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// A regular tiling of polygons in two dimensions.  This class can create
    /// and manage a grid of hexagon slots.
    /// </summary>
    class Tessellation : IEnumerable<Slot>
    {
        private SlotOrientation.Type orientation;
        private Rectangle playZone;
        private Rectangle tightPlayzone;
        private float sideLength;
        private int countHorizontal;
        public int CountHorizontal { get { return countHorizontal; } }
        private int countVertical;
        public int CountVertical { get { return countVertical; } }
        private int maxCountHorizontal;
        private int maxCountVertical;
        private List<Slot> slots;
        public List<Slot> Slots { get { return slots; } }

        /// <summary>
        /// Construct the grid with playzone dimensions and desired grid counts
        /// </summary>
        /// <param name="playZone"></param>
        /// <param name="countHorizontal"></param>
        /// <param name="countVertical"></param>
        /// <param name="orientation"></param>
        public Tessellation(Rectangle playZone, int countHorizontal, int countVertical, SlotOrientation.Type orientation)
        {
            this.playZone = playZone;
            this.tightPlayzone = playZone;
            this.orientation = orientation;
            this.countHorizontal = countHorizontal;
            this.countVertical = countVertical;
            this.maxCountHorizontal = countHorizontal;
            this.maxCountVertical = countVertical;

            // determine the size of the hexagon side that allows fitting all of the slots
            float hSideLength = 0.0f;
            if (orientation == SlotOrientation.Type.ORIENTED_POINTS_HORIZONTAL)
                hSideLength = (float)playZone.Width / (float)countHorizontal / 3;
            else if (orientation == SlotOrientation.Type.ORIENTED_POINTS_VERTICAL)
                hSideLength = (float)playZone.Width / (float)countHorizontal / 1.732f;

            float vSideLength = 0.0f;
            if (orientation == SlotOrientation.Type.ORIENTED_POINTS_HORIZONTAL)
                vSideLength = (float)playZone.Height / (float)countVertical / 3;
            else if (orientation == SlotOrientation.Type.ORIENTED_POINTS_VERTICAL)
                vSideLength = (float)playZone.Height / (float)countVertical / 1.732f;

            this.sideLength = Math.Min(hSideLength, vSideLength);

            BuildBoard();
        }

        /// <summary>
        /// Max out the playzone dimensions with hexagons of argument length
        /// </summary>
        /// <param name="playZone"></param>
        /// <param name="hexSideLength"></param>
        /// <param name="?"></param>
        /// <param name="orientation"></param>
        public Tessellation(Rectangle playZone, float sideLength, SlotOrientation.Type orientation)
        {
            this.playZone = playZone;
            this.sideLength = sideLength;
            this.orientation = orientation;

            BuildBoard();
        }

        /// <summary>
        /// Get a playzone tightened around the tesselation
        /// </summary>
        /// <returns></returns>
        public Rectangle GetTightPlayzone()
        {
            return tightPlayzone;
        }

        /// <summary>
        /// Routine to create all the slots
        /// </summary>
        private void BuildBoard()
        {
            // calc outer triangle side lengths
            float r = (0.866f * sideLength); // bigger arm, cos(30) ~= 0.866f
            float h = (0.5f * sideLength);  // smaller arm , sin(30) = 0.5

            float p2pWidth = (2 * h + sideLength);
            float s2sWidth = (2 * r);
            if (orientation == SlotOrientation.Type.ORIENTED_POINTS_HORIZONTAL)
            {
                // determine the horizontal count
                this.countHorizontal = (int)((float)playZone.Width / (p2pWidth + sideLength));
                // determine the vertical count
                this.countVertical = (int)((float)playZone.Height / r); // half s2sWidth
            }
            else if (orientation == SlotOrientation.Type.ORIENTED_POINTS_VERTICAL)
            {
                // determine the horizontal count
                this.countHorizontal = (int)((float)playZone.Width / s2sWidth);
                // determine the vertical count
                this.countVertical = (int)((float)playZone.Height / (sideLength + h)); // half p2pWidth
            }

            // make sure we didn't go over the max (but we ARE allowed to go under)
            if (countHorizontal > maxCountHorizontal) countHorizontal = maxCountHorizontal;
            if (countVertical > maxCountVertical) countVertical = maxCountVertical;

            // create slots and add them to the slot list
            slots = new List<Slot>();

            // center the board within the playzone and set the tightplayzone
            float xStartingOffset = 0;  // if these stay 0, no centering will occur
            float yStartingOffset = 0;
            if (orientation == SlotOrientation.Type.ORIENTED_POINTS_HORIZONTAL)
            {
                tightPlayzone.Width = (int)((sideLength + p2pWidth) * countHorizontal);
                tightPlayzone.Height = (int)(r * countVertical);
                xStartingOffset = (playZone.Width - tightPlayzone.Width) / 2.0f;
                yStartingOffset = (playZone.Height - tightPlayzone.Height) / 2.0f;
            }
            else if (orientation == SlotOrientation.Type.ORIENTED_POINTS_VERTICAL)
            {
                tightPlayzone.Width = (int)(s2sWidth * countHorizontal);
                tightPlayzone.Height = (int)((sideLength + h) * countVertical + h);
                xStartingOffset = (playZone.Width - tightPlayzone.Width) / 2.0f;
                yStartingOffset = (playZone.Height - tightPlayzone.Height) / 2.0f;
            }
            tightPlayzone.X = playZone.X + (int)xStartingOffset;
            tightPlayzone.Y = playZone.Y + (int)yStartingOffset;

            // fill the list (left to right, top to bottom)
            float currentX = playZone.Left + xStartingOffset;
            float currentY = playZone.Top + yStartingOffset;
            int sIndex = 0;
            bool makingShortRow = false; // keep track if the current row is a short one
            for (int yIndex = 0; yIndex < countVertical; yIndex++)
            {
                for (int xIndex = 0; xIndex < countHorizontal; xIndex++)
                {
                    if (yIndex % 2 == 1 && xIndex + 1 >= countHorizontal) continue; // on the odd indexes skip the last horizontal hex

                    // create a slot
                    Rectangle bounds = new Rectangle((int)currentX, (int)currentY, (int)p2pWidth, (int)s2sWidth); // default
                    if (orientation == SlotOrientation.Type.ORIENTED_POINTS_VERTICAL) bounds = new Rectangle((int)currentX, (int)currentY, (int)s2sWidth, (int)p2pWidth);
                    Slot s = new Slot(sIndex, bounds, orientation);
                    slots.Add(s);
                    s.IsOnShortRow = makingShortRow;
                    // identify if this slot is an edge
                    if (yIndex == 0 && xIndex == 0) s.Edge = Slot.EdgeType.TOP_LEFT_EDGE;
                    else if (yIndex + 1 == countVertical && xIndex == 0) s.Edge = Slot.EdgeType.BOTTOM_LEFT_EDGE;
                    else if (yIndex == 0 && xIndex + 1 == countHorizontal) s.Edge = Slot.EdgeType.TOP_RIGHT_EDGE;
                    else if (yIndex + 1 == countVertical && xIndex + 1 == countHorizontal) s.Edge = Slot.EdgeType.BOTTOM_RIGHT_EDGE;
                    else if (yIndex == 0) s.Edge = Slot.EdgeType.TOP_EDGE;
                    else if (yIndex + 1 == countVertical) s.Edge = Slot.EdgeType.BOTTOM_EDGE;
                    else if (xIndex == 0) s.Edge = Slot.EdgeType.LEFT_EDGE;
                    else if (yIndex % 2 == 1 && xIndex + 2 == countHorizontal) s.Edge = Slot.EdgeType.RIGHT_EDGE; // odd case
                    else if (xIndex + 1 == countHorizontal) s.Edge = Slot.EdgeType.RIGHT_EDGE;

                    // increment the x location
                    if (orientation == SlotOrientation.Type.ORIENTED_POINTS_HORIZONTAL) currentX += (p2pWidth + sideLength);
                    else if (orientation == SlotOrientation.Type.ORIENTED_POINTS_VERTICAL) currentX += s2sWidth;
                    // index of this slot
                    sIndex++;
                }

                // reset the x location
                if (yIndex % 2 == 0) // on the odd indexes the row is shorter
                {
                    makingShortRow = true;
                    if (orientation == SlotOrientation.Type.ORIENTED_POINTS_HORIZONTAL) currentX = xStartingOffset + playZone.Left + (sideLength + h);
                    else if (orientation == SlotOrientation.Type.ORIENTED_POINTS_VERTICAL) currentX = xStartingOffset + playZone.Left + r;
                }
                else
                {
                    makingShortRow = false;
                    currentX = playZone.Left + xStartingOffset;
                }

                // increment the y location
                if (orientation == SlotOrientation.Type.ORIENTED_POINTS_HORIZONTAL) currentY += r;
                else if (orientation == SlotOrientation.Type.ORIENTED_POINTS_VERTICAL) currentY += (sideLength + h);
            }

            BuildAdjacencyLists();
        }

        /// <summary>
        /// Lop through all slots and build a list of neighbors
        /// </summary>
        private void BuildAdjacencyLists()
        {
            foreach (Slot s in slots)
            {
                switch (s.Edge)
                {
                    case Slot.EdgeType.NONE:  // normal non edge slot
                        {
                            int above1 = s.Id - countHorizontal;
                            int above2 = s.Id - countHorizontal + 1;
                            int left = s.Id - 1;
                            int right = s.Id + 1;
                            int below1 = s.Id + countHorizontal - 1;
                            int below2 = s.Id + countHorizontal;
                            s.adjacentSlots.Add(slots[above1]);
                            s.adjacentSlots.Add(slots[above2]);
                            s.adjacentSlots.Add(slots[left]);
                            s.adjacentSlots.Add(slots[right]);
                            s.adjacentSlots.Add(slots[below1]);
                            s.adjacentSlots.Add(slots[below2]);
                        }
                        break;

                    case Slot.EdgeType.TOP_LEFT_EDGE:
                        {
                            int right = s.Id + 1;
                            int below2 = s.Id + countHorizontal;
                            s.adjacentSlots.Add(slots[right]);
                            s.adjacentSlots.Add(slots[below2]);
                        }
                        break;

                    case Slot.EdgeType.TOP_RIGHT_EDGE:
                        {
                            int left = s.Id - 1;
                            int below1 = s.Id + countHorizontal - 1;
                            s.adjacentSlots.Add(slots[left]);
                            s.adjacentSlots.Add(slots[below1]);
                        }
                        break;

                    case Slot.EdgeType.BOTTOM_LEFT_EDGE:
                        {
                            int above2 = s.Id - countHorizontal + 1;
                            int right = s.Id + 1;
                            s.adjacentSlots.Add(slots[above2]);
                            s.adjacentSlots.Add(slots[right]);
                        }
                        break;

                    case Slot.EdgeType.BOTTOM_RIGHT_EDGE:
                        {
                            int left = s.Id - 1;
                            int above1 = s.Id - countHorizontal;
                            s.adjacentSlots.Add(slots[left]);
                            s.adjacentSlots.Add(slots[above1]);
                        }
                        break;

                    case Slot.EdgeType.LEFT_EDGE:
                        if (s.IsOnShortRow)
                        {
                            int above1 = s.Id - countHorizontal;
                            int above2 = s.Id - countHorizontal + 1;
                            int right = s.Id + 1;
                            int below1 = s.Id + countHorizontal - 1;
                            int below2 = s.Id + countHorizontal;
                            s.adjacentSlots.Add(slots[above1]);
                            s.adjacentSlots.Add(slots[above2]);
                            s.adjacentSlots.Add(slots[right]);
                            s.adjacentSlots.Add(slots[below1]);
                            s.adjacentSlots.Add(slots[below2]);
                        }
                        else
                        {
                            int above2 = s.Id - countHorizontal + 1;
                            int right = s.Id + 1;
                            int below2 = s.Id + countHorizontal;
                            s.adjacentSlots.Add(slots[above2]);
                            s.adjacentSlots.Add(slots[right]);
                            s.adjacentSlots.Add(slots[below2]);
                        }
                        break;

                    case Slot.EdgeType.RIGHT_EDGE:
                        if (s.IsOnShortRow)
                        {
                            int above1 = s.Id - countHorizontal;
                            int above2 = s.Id - countHorizontal + 1;
                            int left = s.Id - 1;
                            int below1 = s.Id + countHorizontal - 1;
                            int below2 = s.Id + countHorizontal;
                            s.adjacentSlots.Add(slots[above1]);
                            s.adjacentSlots.Add(slots[above2]);
                            s.adjacentSlots.Add(slots[left]);
                            s.adjacentSlots.Add(slots[below1]);
                            s.adjacentSlots.Add(slots[below2]);
                        }
                        else
                        {
                            int above1 = s.Id - countHorizontal;
                            int left = s.Id - 1;
                            int below1 = s.Id + countHorizontal - 1;
                            s.adjacentSlots.Add(slots[above1]);
                            s.adjacentSlots.Add(slots[left]);
                            s.adjacentSlots.Add(slots[below1]);
                        }
                        break;

                    case Slot.EdgeType.TOP_EDGE:
                        {
                            int below1 = s.Id + countHorizontal - 1;
                            int below2 = s.Id + countHorizontal;
                            int left = s.Id - 1;
                            int right = s.Id + 1;
                            s.adjacentSlots.Add(slots[left]);
                            s.adjacentSlots.Add(slots[right]);
                            s.adjacentSlots.Add(slots[below1]);
                            s.adjacentSlots.Add(slots[below2]);
                        }
                        break;

                    case Slot.EdgeType.BOTTOM_EDGE:
                        {
                            int above1 = s.Id - countHorizontal;
                            int above2 = s.Id - countHorizontal + 1;
                            int left = s.Id - 1;
                            int right = s.Id + 1;
                            s.adjacentSlots.Add(slots[left]);
                            s.adjacentSlots.Add(slots[right]);
                            s.adjacentSlots.Add(slots[above1]);
                            s.adjacentSlots.Add(slots[above2]);
                        }
                        break;
                }
            }
        }

        #region Enumeration techniques
        /// <summary>
        /// Enumerate as a list of slots.  easy iteration from 0 to slots.count
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Slot> GetEnumerator()
        {
            for (int x = 0; x < slots.Count; x++)
            {
                yield return slots[x];
            }
        }

        /// <summary>
        /// just call the slot enumerator
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
