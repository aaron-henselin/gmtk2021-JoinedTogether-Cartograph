using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace JoinedTogetherGmtk2021.Game
{
    public class Room
    {
        public RoomType RoomType { get; }

        public Room(RoomType roomType)
        {
            RoomType = roomType;
        }

        public bool ConnectsTo(Room other,Direction direction)
        {
            if (other == null)
                return false;

            return this.RoomType.ConnectsTo(other.RoomType, direction);
        }
        

        public bool Locked { get; set; }

        public override string ToString()
        {
            return $"RoomType={RoomType}, Locked={Locked}";
        }
    }

    public class Floor
    {
        public int Depth
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                string floorName = "Ground Floor";
                if (Depth > 1)
                    floorName = "Basement " + (Depth - 1);

                if (Depth < 1)
                    floorName = "Floor " + (Depth + 1);

                return floorName;
            }
        }

        public Floor ParentFloor { get; set; }
            
        private int _width;
        private int _height;

        public Dictionary<FloorCoord, Room> Layout { get; set; } = new Dictionary<FloorCoord, Room>();
        public Floor(int width, int height)
        {
            for (int w = 0; w < width; w++)
                for (int h = 0; h < height; h++)
                    Layout[new FloorCoord {X = w, Y = h}] = null;

            _width = width;
            _height = height;
        }

        public int Width => _width;
        public int Height => _height;


        public Dictionary<GameObject, FloorCoord> Ghosts { get; set; } = new Dictionary<GameObject, FloorCoord>();

        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();
  
    }

    public class GameObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
        public FloorCoord Position { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsExit { get; set; }
    }

    public struct Direction
    {
        public string KeyMapping { get; set; }   
        public int X { get; set; }
        public int Y { get; set; }

    }

    public struct Directions
    {
        public static Direction Up => new Direction{KeyMapping = "ArrowUp",X=0,Y=-1};
        public static Direction Down => new Direction { KeyMapping = "ArrowDown", X = 0, Y = 1 };
        public static Direction Left => new Direction { KeyMapping = "ArrowLeft", X = -1, Y = 0 };
        public static Direction Right => new Direction { KeyMapping = "ArrowRight", X = 1, Y = 0 };

        public static Dictionary<string, Direction> DirectionsByKeyMapping => new Dictionary<string, Direction>
        {
            [Up.KeyMapping]=Up,
            [Down.KeyMapping] = Down,
            [Left.KeyMapping] = Left,
            [Right.KeyMapping] = Right
        };
    }


    public struct FloorCoord
    {
        public int X { get; set; }
        public int Y { get; set; }

        public FloorCoord Add(Direction direction)
        {
            return new FloorCoord
            {
                X = X+direction.X,
                Y = Y+direction.Y
            };
        }

        public bool IsBackpack => this.Equals(Backpack);

        public static FloorCoord Backpack => new FloorCoord {X = -1, Y = -1};

        public override string ToString()
        {
            return $"X={X},Y={Y}";
        }
    }

    public static class RoomTypes
    {
        public static RoomType PATHNE__ => new RoomType { N = true, S = false, E = true, W = false, Name = "PATHNE__" };
        public static RoomType PATHN__W => new RoomType { N = true, S = false, E = false, W = true, Name = "PATHN__W" };
        public static RoomType PATH_ES_ => new RoomType { N = false, S = true, E = true, W = false, Name= "PATH_ES_" };
        public static RoomType PATH_E_W => new RoomType { N = false, S = false, E = true, W = true, Name = "PATH_E_W" };
        public static RoomType PATH__SW => new RoomType { N = false, S = true, E = false, W = true, Name = "PATH__SW" };
        public static RoomType PATHN_S_ => new RoomType { N = true, S = true, E = false, W = false, Name = "PATHN_S_" };
        public static RoomType PATHN___ => new RoomType { N = true, S = false, E = false, W = false, Name = "PATHN___",HasLadderDesc = true};
        public static RoomType PATH__S_ => new RoomType { N = false, S = true, E = false, W = false, Name = "PATH__S_", HasLadderAsc = true };
        public static RoomType BUTTON => new RoomType { N = true, S = true, E = true, W = true, Name = "BUTTON", HasButton=true };
        public static RoomType SPIKES => new RoomType { N = true, S = true, E = true, W = true, Name = "SPIKES", HasSpikes=true };
    }


    public struct RoomType
    {
        public bool W { get; set; }
        public bool E { get; set; }
        public bool S { get; set; }
        public bool N { get; set; }
        public string Name { get; internal set; }
        public bool HasLadderDesc { get; set; }
        public bool HasLadderAsc { get; set; }
        public bool HasButton { get; internal set; }
        public bool HasSpikes { get; internal set; }

        public bool ConnectsTo(RoomType other, Direction direction)
        {
            if (direction.Equals(Directions.Up))
                return this.N && other.S;

            if (direction.Equals(Directions.Down))
                return this.S && other.N;

            if (direction.Equals(Directions.Left))
                return this.W && other.E;

            if (direction.Equals(Directions.Right))
                return this.E && other.W;

            throw new ArgumentException(direction.ToString());
        }

        public override string ToString()
        {
            return $"N={N} S={S} E={E} W={W}";
        }
    }


}
