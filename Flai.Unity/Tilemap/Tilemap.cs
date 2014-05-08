using Flai.Diagnostics;
using Flai.Tilemap.Serializing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flai.Tilemap
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class Tilemap : FlaiScript
    {
        public Material MeshMaterial;
        public bool CreateColliders = true;

        public TilemapData TilemapData;
        public TilesetManager TilesetManager;

        public void CreateFrom(TilemapData tilemapData, TilesetManager tilesetManager)
        {
            this.TilemapData = tilemapData;
            this.TilesetManager = tilesetManager;
            this.Create();
        }

        public void Create()
        {
            if (this.TilemapData == null)
            {
                FlaiDebug.LogErrorWithTypeTag<Tilemap>("Error creating a Tilemap: TilemapData is null!");
                return;
            }

            if (this.TilesetManager == null)
            {
                FlaiDebug.LogErrorWithTypeTag<Tilemap>("Error creating a Tilemap: TilesetManager is null!");
                return;
            }

            try
            {
                this.CreateMesh();
            }
            catch
            {
                FlaiDebug.LogErrorWithTypeTag<Tilemap>("Error Creating Mesh - aborting");
                throw;
            }

            foreach (EdgeCollider2D collider in this.GetComponents<EdgeCollider2D>())
            {
                Component.Destroy(collider);
            }

            if (this.CreateColliders)
            {
                try
                {
                    this.CreateEdgeColliders();
                }
                catch
                {
                    FlaiDebug.LogErrorWithTypeTag<Tilemap>("Error Creating Colliders - aborting");
                    throw;
                }
            }
        }

        #region Create Mesh

        private void CreateMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int lastID = -1;
            for (int y = 0; y < this.TilemapData.Height; y++)
            {
                for (int x = 0; x < this.TilemapData.Width; x++)
                {
                    int tile = this.TilemapData[x, y];
                    if (tile == 0)
                    {
                        // tile is empty
                        continue;
                    }

                    int realY = this.TilemapData.Height - y - 1; // y is inverted (tiled y = 0 is down, unity y = 0 is up)
                    this.CreateTile(vertices, triangles, uvs, x, realY, tile, this.TilesetManager);
                    lastID = tile;
                }
            }

            this.Get<MeshFilter>().mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray(),
            };

            this.Get<MeshRenderer>().material = new Material(this.MeshMaterial)
            {
                mainTexture = (lastID == -1) ? null : TilemapSpriteManager.Instance.GetSprite(this.TilesetManager, lastID).texture
            };
        }

        private void CreateTile(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, int x, int y, int tile, TilesetManager tilesetManager)
        {
            // coordinates start from top-left, remember
            vertices.Add(new Vector2f(x, y)); // TL
            vertices.Add(new Vector2f(x + 1, y)); // TR
            vertices.Add(new Vector2f(x, y + 1)); // BL

            vertices.Add(new Vector2f(x, y + 1)); // BL
            vertices.Add(new Vector2f(x + 1, y)); // TR
            vertices.Add(new Vector2f(x + 1, y + 1)); // BR

            for (int i = 0; i < 6; i++)
            {
                triangles.Add(triangles.Count);
            }

            Sprite sprite = TilemapSpriteManager.Instance.GetSprite(tilesetManager, tile);
            if (sprite == null)
            {
                FlaiDebug.LogErrorWithTypeTag<Tilemap>("Error - No Sprite found in TilemapSpriteManager");
                return;
            }

            RectangleF uv = sprite.textureRect.ToRectangleF() * sprite.texture.texelSize;
            uvs.Add(uv.TopLeft);
            uvs.Add(uv.TopRight);
            uvs.Add(uv.BottomLeft);

            uvs.Add(uv.BottomLeft);
            uvs.Add(uv.TopRight);
            uvs.Add(uv.BottomRight);
        }

        #endregion

        #region Create Colliders

        private void CreateEdgeColliders()
        {
            var sides = this.CreateTileSides(this.TilemapData);

            // todo: right now, all colliders are straight lines. they dont have to be.
            // todo>> make one collider for each closed shape. thus the amount of colliders is reduced greatly
            while (sides.Count > 0)
            {
                TileSide startSide = sides.First();
                sides.Remove(startSide);

                Vector2f min = startSide.Min;
                Vector2f max = startSide.Max;

                TileSide previous = startSide.Previous;
                while (sides.Contains(previous))
                {
                    sides.Remove(previous);
                    min = previous.Min;
                    previous = previous.Previous;
                }

                TileSide next = startSide.Next;
                while (sides.Contains(next))
                {
                    sides.Remove(next);
                    max = next.Max;
                    next = next.Next;
                }

                this.Add<EdgeCollider2D>().points = new Vector2[] { min, max };
            }

            #region !! TODO !! This is the optimized version (combines the colliders) which doesn't work with Unity 4.3, BUT it should work with Unity 4.5 (there is a bug with EdgeColliders and Physics2D.OverlapArea which broke crate collisions!!)

            /*
            // create segments
            HashSet<Segment2Di> segments = new HashSet<Segment2Di>();
            Dictionary<Vector2i, HashSet<Segment2Di>> pointToSegment = new Dictionary<Vector2i, HashSet<Segment2Di>>();
            while (sides.Count > 0)
            {
                TileSide startSide = sides.First();
                sides.Remove(startSide);

                Vector2i min = startSide.MinIndex;
                Vector2i max = startSide.MaxIndex;

                TileSide previous = startSide.Previous;
                while (sides.Contains(previous))
                {
                    sides.Remove(previous);
                    min = previous.MinIndex;
                    previous = previous.Previous;
                }

                TileSide next = startSide.Next;
                while (sides.Contains(next))
                {
                    sides.Remove(next);
                    max = next.MaxIndex;
                    next = next.Next;
                }

                Segment2Di segment = new Segment2Di(min, max);
                segments.Add(segment);

                pointToSegment.GetOrAddDefault(min).Add(segment);
                pointToSegment.GetOrAddDefault(max).Add(segment);
            }

            //  FlaiDebug.LogWarning("S: " + string.Join(", ", segments.ToArray().Select(p => p.ToString()).ToArray()));
            // combine segments and create edge colliders
            while (segments.Count > 0)
            {
                Segment2Di current = segments.First();
                FlaiDebug.Log(current);
                segments.Remove(current);
                pointToSegment[current.Start].Remove(current);
                pointToSegment[current.End].Remove(current);

                List<Vector2> points = new List<Vector2>() { current.Start * Tile.Size, current.End * Tile.Size };
                while (pointToSegment[current.End].Count != 0)
                {
                    Segment2Di newSegment = pointToSegment[current.End].FirstOrDefault(segments.Contains);
                    if (newSegment == default(Segment2Di))
                    {
                        break;
                    }

                    segments.Remove(newSegment);
                    segments.Remove(newSegment.Flipped);
                    if (newSegment.End == current.End)
                    {
                        newSegment = newSegment.Flipped;
                    }

                    FlaiDebug.Log(newSegment);
                    pointToSegment[newSegment.Start].Remove(newSegment);
                    pointToSegment[newSegment.End].Remove(newSegment);
                    points.Add(newSegment.End * Tile.Size);
                    current = newSegment;
                }

                this.Add<EdgeCollider2D>().points = points.ToArray();
            }   */

            #endregion
        }

        private HashSet<TileSide> CreateTileSides(TilemapData tilemapData)
        {
            var sides = new HashSet<TileSide>();
            for (int y = 0; y < tilemapData.Height; y++)
            {
                for (int x = 0; x < tilemapData.Width; x++)
                {
                    int tile = tilemapData[x, y];
                    if (tile == 0)
                    {
                        // empty tile
                        continue;
                    }

                    int realY = tilemapData.Height - y - 1;
                    if (this.IsEmpty(tilemapData, x - 1, y))
                    {
                        sides.Add(new TileSide(new Vector2i(x, realY), Direction2D.Left));
                    }

                    if (this.IsEmpty(tilemapData, x + 1, y))
                    {
                        sides.Add(new TileSide(new Vector2i(x, realY), Direction2D.Right));
                    }

                    if (this.IsEmpty(tilemapData, x, y - 1))
                    {
                        sides.Add(new TileSide(new Vector2i(x, realY + 1), Direction2D.Up));
                    }

                    if (this.IsEmpty(tilemapData, x, y + 1))
                    {
                        sides.Add(new TileSide(new Vector2i(x, realY - 1), Direction2D.Down));
                    }
                }
            }

            return sides;
        }

        private bool IsEmpty(TilemapData tilemapData, int x, int y)
        {
            if (x < 0 || y < 0 || x >= tilemapData.Width || y >= tilemapData.Height)
            {
                return true;
            }

            return tilemapData[x, y] == 0;
        }

        #region TileSide

        private struct TileSide : IEquatable<TileSide>
        {
            public readonly Vector2i Index;
            public readonly Direction2D Side;

            public TileSide Previous
            {
                get { return new TileSide(this.Index - this.RealDirection.ToUnitVector(), this.Side); }
            }

            public TileSide Next
            {
                get { return new TileSide(this.Index + this.RealDirection.ToUnitVector(), this.Side); }
            }

            public TileSide NextLeft
            {
                get
                {
                    Direction2D newSide = this.Side.RotateLeft();
                    Vector2i newIndex = this.Index + this.Side.ToUnitVector() + this.Side.RotateRight().ToUnitVector();

                    return new TileSide(newIndex, newSide);
                }
            }

            public TileSide NextRight
            {
                get
                {
                    Direction2D newSide = this.Side.RotateRight();
                    Vector2i newIndex = this.Index + this.Side.ToUnitVector() + this.Side.RotateLeft().ToUnitVector();

                    return new TileSide(newIndex, newSide);
                }

            }

            public TileSide NextLeft2
            {
                get
                {
                    Direction2D newSide = this.Side.RotateLeft();
                    return new TileSide(this.Index, newSide);
                }
            }

            public TileSide NextRight2
            {
                get
                {
                    Direction2D newSide = this.Side.RotateRight();
                    return new TileSide(this.Index, newSide);
                }
            }

            public Vector2f Min
            {
                get { return this.MinIndex; }
            }

            public Vector2f Max
            {
                get { return (this.MinIndex + this.RealDirection.ToUnitVector()); }
            }

            private Direction2D RealDirection
            {
                get
                {
                    switch (this.Side)
                    {
                        case Direction2D.Left:
                            return Direction2D.Down;

                        case Direction2D.Up:
                            return Direction2D.Right;

                        case Direction2D.Right:
                            return Direction2D.Down;

                        case Direction2D.Down:
                            return Direction2D.Right;

                        default:
                            throw new ArgumentException("value");
                    }
                }
            }

            public Vector2i MinIndex
            {
                get
                {
                    switch (this.Side)
                    {
                        case Direction2D.Left:
                        case Direction2D.Up:
                            return this.Index;

                        case Direction2D.Right:
                            return this.Index + Vector2i.UnitX;

                        case Direction2D.Down:
                            return this.Index + Vector2i.UnitY;

                        default:
                            throw new ArgumentException("value");
                    }
                }
            }

            public Vector2i MaxIndex
            {
                get { return this.MinIndex + this.RealDirection.ToUnitVector(); }
            }


            public TileSide(Vector2i index, Direction2D side)
            {
                this.Index = index;
                this.Side = side;
            }

            public override int GetHashCode()
            {
                return this.Index.GetHashCode() ^ this.Side.GetHashCode();
            }

            public bool Equals(TileSide other)
            {
                return this.Index == other.Index && this.Side == other.Side;
            }

            public override bool Equals(object obj)
            {
                return obj is TileSide && this.Equals((TileSide)obj);
            }
        }

        #endregion

        #endregion
    }
}
