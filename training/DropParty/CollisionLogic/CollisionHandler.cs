using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace DropParty
{
    public static class CollisionHandler
    {
        static List<ICollidable> Collidables;
        static List<ICollidable> willRemove;
        static List<ICollidable> willAdd;

        static CollisionHandler()
        {
            Collidables = new List<ICollidable>();
            willRemove = new List<ICollidable>();
            willAdd = new List<ICollidable>();
        }

        public static void Clear()
        {
            Collidables.Clear();
            willRemove.Clear();
            willAdd.Clear();
        }

        public static void Add(ICollidable collidable)
        {
            if (!Collidables.Contains(collidable) && !willAdd.Contains(collidable))
            {
                willAdd.Add(collidable);
            }
        }

        public static bool IsValidPosition(Vector2f requestedPostion, ICollidable requestingCollidable)
        {

            foreach (ICollidable collideable in Collidables)
            {
                if (collideable != requestingCollidable && collideable.CollisionInfo.IsCollisionCheckNeeded(requestingCollidable))
                {
                    if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.Touching, requestingCollidable) || collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.InvertedTouching, requestingCollidable))
                    {
                        if (EdgeCollisionCheck(requestedPostion, requestingCollidable.Radius, collideable?.RequestedPosition ?? collideable.Position, collideable.Radius))
                        {
                            if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.Touching, requestingCollidable))
                            {
                                return false;
                            }
                        }
                        else if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.InvertedTouching, requestingCollidable))
                        {
                            return false;
                        }
                    }

                    if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.Enveloped, requestingCollidable) || collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.InvertedEnveloped, requestingCollidable))
                    {
                        if (FullCollisionCheck(requestedPostion, requestingCollidable.Radius, collideable?.RequestedPosition ?? collideable.Position, collideable.Radius))
                        {
                            if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.Enveloped, requestingCollidable))
                            {
                                return false;
                            }
                        }
                        else if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.InvertedEnveloped, requestingCollidable))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static void Update()
        {
            willRemove.ForEach(x => Collidables.Remove(x));
            willRemove.Clear();

            willAdd.ForEach(x => Collidables.Add(x));
            willAdd.Clear();


            foreach (ICollidable requestingCollidable in Collidables.Where(x => x.RequestedPosition != null).ToList())
            {
                if (!willRemove.Contains(requestingCollidable))
                {
                    List<ICollidable> collisions = new List<ICollidable>();
                    foreach (ICollidable collideable in Collidables)
                    {
                        if (collideable != requestingCollidable && collideable.CollisionInfo.IsCollisionCheckNeeded(requestingCollidable))
                        {
                            if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.Touching, requestingCollidable) || collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.InvertedTouching, requestingCollidable))
                            {
                                if (EdgeCollisionCheck((Vector2f)requestingCollidable.RequestedPosition, requestingCollidable.Radius, collideable?.RequestedPosition ?? collideable.Position, collideable.Radius))
                                {
                                    if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.Touching, requestingCollidable))
                                    {
                                        collisions.Add(collideable);
                                    }
                                }
                                else if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.InvertedTouching, requestingCollidable))
                                {
                                    collisions.Add(collideable);
                                }
                            }

                            if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.Enveloped, requestingCollidable) || collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.InvertedEnveloped, requestingCollidable))
                            {
                                if (FullCollisionCheck((Vector2f)requestingCollidable.RequestedPosition, requestingCollidable.Radius, collideable?.RequestedPosition ?? collideable.Position, collideable.Radius))
                                {
                                    if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.Enveloped, requestingCollidable))
                                    {
                                        collisions.Add(collideable);
                                    }
                                }
                                else if (collideable.CollisionInfo.IsCollisionCheckNeeded(CollisionType.InvertedEnveloped, requestingCollidable))
                                {
                                    collisions.Add(collideable);
                                }
                            }
                        }
                    }
                    requestingCollidable.OnCollide(collisions);
                }
            }

            Collidables.ForEach(x => x.RequestedPosition = null);
        }

        public static void Remove(ICollidable collidable)
        {
            if (Collidables.Contains(collidable) && !willRemove.Contains(collidable))
            {
                willRemove.Add(collidable);
            }
        }

        //public static Vector2f CoverToWall(Vector2f requestedPosition, ICollidable requestingCollidable, ICollidable collidable)
        //{
        //    return requestingCollidable.Position;
        //    Vector2f fullDistanceOfMovement = requestedPosition - requestingCollidable.Position;
        //    Vector2f subDistanceOfMovement = new Vector2f();
        //    for (double i = 1.02; i >= -0.02; i -= 0.01)
        //    {
        //        subDistanceOfMovement = new Vector2f((float)(fullDistanceOfMovement.X * i), (float)(fullDistanceOfMovement.Y * i));

        //        if (FullCollisionCheck((Vector2f)requestingCollidable.RequestedPosition, requestingCollidable.Radius, collidable?.RequestedPosition ?? collidable.Position, collidable.Radius))
        //        {
        //            subDistanceOfMovement = new Vector2f((float)(fullDistanceOfMovement.X * i), (float)(fullDistanceOfMovement.Y * i));
        //            break;
        //        }

        //    }

        //    return subDistanceOfMovement + requestingCollidable.Position;
        //}

        private static bool EdgeCollisionCheck(Vector2f movingPosition, float movingRadius, Vector2f stationaryPosition, float stationaryRadius)
        {
            return DistanceSqaured(movingPosition, stationaryPosition) < Math.Pow(movingRadius + stationaryRadius, 2);
        }

        private static bool FullCollisionCheck(Vector2f movingPosition, float movingRadius, Vector2f stationaryPosition, float stationaryRadius)
        {
            return DistanceSqaured(movingPosition, stationaryPosition) < Math.Pow(stationaryRadius - movingRadius, 2);
        }

        private static float DistanceSqaured(Vector2f point1, Vector2f point2)
        {
            return (float)(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }
    }
}