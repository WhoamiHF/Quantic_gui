using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * Class containing info about move - coordinates, which player wants to play it and which shape is to be played
     */ 
    internal class Move
    {
        int _x;
        int _y;
        Piece.PlayerID _player;
        Piece.ShapeType _shape;

        public Move(int x, int y, Piece.PlayerID player, Piece.ShapeType shape)
        {
            this._x = x;
            this._y = y;
            this._player = player;
            this._shape = shape;
        }

        public int X { get { return _x; } }
        public int Y { get { return _y;} }
        public Piece.PlayerID Player
        {
            get
            {
                return _player;
            }
        }
        public Piece.ShapeType Shape
        {
            get
            {
                return _shape;
            }
        }

        /**
         * Used for logging
         */
        public override string ToString()
        {
            string player = _player == Piece.PlayerID.PLAYER_ONE ? "one" : "two";
            return "[Player " + player + "] " + Shape + " to [" + X + "," + Y + "]"; 
        }
    }

}
