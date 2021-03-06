﻿using BattleShip.Logical.AI;
using BattleShip.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip.Logical
{
    [Serializable]
    /// <summary>
    /// Stores playgrids for the player and enemy and handles operations on them
    /// </summary>
    public class GameData : INotifyPropertyChanged
    {
        /// <summary>
        /// The Player's ship grid, also acts as their own view of their ship, with enemy activity shown
        /// </summary>
        public Playgrid PlayerShipsGrid { get; set; } = new Playgrid(10, 10);

        /// <summary>
        /// The Enemies ship grid, acting as their own view of the player's activity
        /// </summary>
        public Playgrid EnemyShipsGrid { get; set; } = new Playgrid(10, 10);

        /// <summary>
        /// A list of ships belonging to the player
        /// </summary>
        public List<Ship> PlayerShips { get; private set; } = new List<Ship>()
        {
            new Ship(2, "Destroyer",false),
            new Ship(3, "Cruiser", false),
            new Ship(3, "Submarine", false),
            new Ship(4, "Battleship", false),
            new Ship(5, "Carrier", false),
        };

        /// <summary>
        /// A list of ships belonging to the enemy
        /// </summary>
        public List<Ship> EnemyShips { get; private set; } = new List<Ship>()
        {
            new Ship(2, "Destroyer",false),
            new Ship(3, "Cruiser", false),
            new Ship(3, "Submarine", false),
            new Ship(4, "Battleship", false),
            new Ship(5, "Carrier", false),
        };

        /// <summary>
        /// The IAIModel instance that the player is going agains
        /// </summary>
        public IAIModel ActiveAI { get; set; }

        private string logInfo = "";
        public string LogInfo
        {
            get
            {
                return logInfo;
            }
            set
            {
                logInfo = value;
                FieldChanged();
            }
        }
        /// <summary>
        /// Places a ship on the respective grid at the respective x,y location.
        /// Ships are placed by setting their leftmost or top most point, then add on their length
        /// </summary>
        /// <param name="s">The Ship instance to place</param>
        /// <param name="pg">The Playgrid instance to place on</param>
        /// <param name="x">The x coordinate to place on</param>
        /// <param name="y">The y coordinate to place on</param>
        public void PlaceShip(Ship s, Playgrid pg, int x, int y)
        {
            if (IsValidPlacement(s,pg,x,y) && !IsShipHere(pg,x,y) && !IsCollisionPresent(pg))
            {
                s.Tiles = new Tile[s.Length];
                if (!s.IsVertical)
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        pg.ChangeTile(x + i, y, TileState.ShipHere);
                        s.Tiles[i] = pg.Grid[x + i, y];
                    }
                }
                else
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        pg.ChangeTile(x, y + i, TileState.ShipHere);
                        s.Tiles[i] = pg.Grid[x, y + i];
                    }
                }
            }
        }

        /// <summary>
        /// Changes tile states to preview states when view visually
        /// </summary>
        /// <param name="s">The ship to place</param>
        /// <param name="pg">The Playgrid instance to place</param>
        /// <param name="x">The x coordinate to place on</param>
        /// <param name="y">The y coordinate to place on</param>
        public void PreviewShipPlace(Ship s, Playgrid pg, int x, int y)
        {
            if(s == null)
            {
                return;
            }
            bool inBounds = (x >= 0 && y >= 0) && (x < pg.Width && y < pg.Height); //isValid limits the bounds further because of ship length, need a bool for 'normal' out of bounds
            if (inBounds)
            {
                if (s.IsVertical)
                {
                    if (IsValidPlacement(s, pg, x, y))
                    {
                        for (int i = 0; i < s.Length; i++)
                        {
                            if(pg.ValueAt(x,y+i) == TileState.ShipHere)
                            {
                                pg.ChangeTile(x, y + i, TileState.PreviewCollision);
                            }
                            else
                            {
                                pg.ChangeTile(x, y + i, TileState.PreviewOK);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < s.Length; i++)
                        {
                            if (pg.ValueAt(x, pg.Height - s.Length + i) == TileState.ShipHere)
                            {
                                pg.ChangeTile(x, pg.Height - s.Length + i, TileState.PreviewCollision);
                            }
                            else
                            {
                                pg.ChangeTile(x, pg.Height - s.Length + i, TileState.PreviewOK);
                            }
                        }
                    }
                }
                else
                {
                    if (IsValidPlacement(s, pg, x, y))
                    {
                        for (int i = 0; i < s.Length; i++)
                        {
                            if (pg.ValueAt(x + i, y) == TileState.ShipHere)
                            {
                                pg.ChangeTile(x + i, y, TileState.PreviewCollision);
                            }
                            else
                            {
                                pg.ChangeTile(x + i, y, TileState.PreviewOK);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < s.Length; i++)
                        {
                            if (pg.ValueAt(pg.Width - s.Length + i, y) == TileState.ShipHere)
                            {
                                pg.ChangeTile(pg.Width - s.Length + i,y , TileState.PreviewCollision);
                            }
                            else
                            {
                                pg.ChangeTile(pg.Width - s.Length + i, y, TileState.PreviewOK);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reverts the changed tiles states that previewing made
        /// </summary>
        /// <param name="pg">The affected Playgrid instance to revert on</param>
        public void RevertPreviewChanges(Playgrid pg)
        {
            for (int i = 0; i < pg.Width; i++)
            {
                for (int j = 0; j < pg.Height; j++)
                {
                    if (pg.ValueAt(i, j) == TileState.PreviewOK)
                    {
                        pg.ChangeTile(i, j, TileState.Normal);
                    }
                    else if (pg.ValueAt(i, j) == TileState.PreviewCollision)
                    {
                        pg.ChangeTile(i, j, TileState.ShipHere);
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if a given Ship instance can be placed on a given Playgrid without being out of bounds
        /// </summary>
        /// <param name="s">The ship instance to place</param>
        /// <param name="pg">Playgrid instance</param>
        /// <param name="x">The x coordinate to place the ship on</param>
        /// <param name="y">The y coordinate to place the ship on</param>
        /// <returns>A boolean if the placement spot is valid or not</returns>
        public bool IsValidPlacement(Ship s, Playgrid pg, int x, int y)
        {
            if ( (x > pg.Width-s.Length && !s.IsVertical) || (y > pg.Height-s.Length && s.IsVertical) || x < 0 || y < 0) //out of bounds check
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Checks to see if a ship is present at the given tile
        /// </summary>
        /// <param name="pg">The Playgrid instance to check on</param>
        /// <param name="x">The x coordinate to check</param>
        /// <param name="y">The y coordinate to check</param>
        /// <returns></returns>
        public bool IsShipHere(Playgrid pg, int x, int y)
        {
            if (pg.ValueAt(x, y) == TileState.ShipHere || pg.ValueAt(x, y) == TileState.PreviewCollision)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks to see if any placement collisions exist when previewing
        /// </summary>
        /// <param name="pg">The Playgrid instance to check</param>
        /// <returns>True if any collisions are found, false if non are present</returns>
        public bool IsCollisionPresent(Playgrid pg)
        {
            for (int i = 0; i < pg.Width; i++)
            {
                for (int j = 0; j < pg.Height; j++)
                {
                    if (pg.ValueAt(i,j) == TileState.PreviewCollision)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if a given ship is intersecting
        /// </summary>
        /// <param name="s">The ship to check if its intersecting anything</param>
        /// <param name="pg">The playgrid to check</param>
        /// <param name="x">The x coordinate of the ships top/left most point</param>
        /// <param name="y">The y coordinate of the ships top/left most point</param>
        /// <returns>True if the ship is intersecting another and false if it is not</returns>
        public bool IsShipIntersecting(Ship s, Playgrid pg, int x, int y)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if(s.IsVertical && IsShipHere(pg, x, y + i))
                {
                    return true;
                }
                else if (!s.IsVertical && IsShipHere(pg, x + i, y))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Fires at a given x,y coordinate
        /// </summary>
        /// <param name="x">The x coordinate of where to 'shoot'</param>
        /// <param name="y">The y coordinate of where to 'shoot'</param>
        /// <param name="target">The target to base off of</param>
        /// <returns>True if the shot was a hit, false if it missed</returns>
        public bool Shoot(int x, int y, Playgrid target)
        {
            bool hit = false;
            if (target.ValueAt(x,y) == TileState.ShipHere && target.ValueAt(x,y) != TileState.Hit)
            {
                target.ChangeTile(x, y, TileState.Hit);
                hit = true;
            }
            else if (target.ValueAt(x, y) != TileState.Hit) 
            {
                target.ChangeTile(x, y, TileState.Missed);
            }
            return hit;
        }

        /// <summary>
        /// Saves the Gamedata instance to a file on disk
        /// </summary>
        /// <param name="filepath">The complete filepath to save to, ie: c:/data/save.btl</param>
        public void SaveData(string filepath)
        {
            Stream stream = File.Open(filepath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Close();
        }

        /// <summary>
        /// Loads a Gamedata instance from a file on disk
        /// </summary>
        /// <param name="filepath">The complete file path to load from, ie: c:/data/save.btl</param>
        /// <returns>A Gamedata instance with the data from the file given</returns>
        public static GameData LoadData(string filepath)
        {
            Stream stream = File.Open(filepath, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            GameData loaded = (GameData)formatter.Deserialize(stream);
            stream.Close();
            return loaded;
        }

        /// <summary>
        /// Returns a Ship instance if a ship has just been sunken
        /// </summary>
        /// <param name="ships">A List of Ship instances to check in</param>
        /// <returns>A Ship instance if its JustSank property is true, or null if no ship has just sunken</returns>
        public Ship GetJustSank(List<Ship> ships)
        {
            foreach (Ship s in ships)
            {
                s.IsSunken();
                if (s.JustSank ?? false)
                {
                    return s;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the winner of the game
        /// </summary>
        /// <returns>1 if the player won, 2 if the enemy won, or 0 if neither won/the game is still in progress</returns>
        public int WhoIsWinner()
        {
            bool playerShipsAllDown = true;
            foreach (Ship ship in PlayerShips)
            {
                if (!ship.IsSunken())
                {
                    playerShipsAllDown = false;
                }
            }
            bool enemyShipsAllDown = true;
            foreach (Ship ship in EnemyShips)
            {
                if (!ship.IsSunken())
                {
                    enemyShipsAllDown = false;
                }
            }

            if (!playerShipsAllDown && !enemyShipsAllDown) //if both still have ships, the game is still in progress
            {
                return 0;
            }
            else
            {
                return playerShipsAllDown ? 2 : 1; //if the player has no ships left, the enemy won, otherwise the player won as it is implicitly known both values are not equivalent
            }
        }

        [field: NonSerialized]
        /// <summary>
        /// Field changing event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void FieldChanged(string field = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(field));
            }
        }
    }

    
}
