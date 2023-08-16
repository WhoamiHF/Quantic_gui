using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * Abstract ancestor of user player and computer player. Keeps track of available pieces and selects move to be played
     */
    internal abstract class Player
    {
        readonly List<Piece> _pieces;
        readonly Piece.PlayerID _player;

        /**
         * Constructor with given pieces and player id
         */
        public Player(List<Piece> pieces,Piece.PlayerID player)
        {
            this._pieces = pieces;
            this._player = player;
        }

        /**
         * Constructor only with given player id - pieces will be all starting eight
         */
        public Player(Piece.PlayerID player)
        {
            this._pieces = new List<Piece>();
            for(int i = 0; i < 2; i++)
            {
                _pieces.Add(new Piece(player, Piece.ShapeType.PYRAMID));
                _pieces.Add(new Piece(player, Piece.ShapeType.CUBE));
                _pieces.Add(new Piece(player, Piece.ShapeType.SPHERE));
                _pieces.Add(new Piece(player, Piece.ShapeType.CYLINDER));
            }
            this._player = player;
        }

        /**
         * Copy constructor
         */
        public Player(Player other)
        {
            this._player = other.Owner;
            this._pieces = new List<Piece>();

            foreach (Piece piece in other._pieces)
            {
                this._pieces.Add(piece);
            }
        }

        /**
         * Copy method used during minimax so pieces will be updated on copy
         */
        public abstract Player Copy();

        /**
         * Selects move or returns null if no move is available
         */
        public abstract Move? SelectMove(GameLogic logic,Board board,Player current, Player other);

        public List<Piece> Pieces { get
            {
                return _pieces;
            }
        }

        public Piece.PlayerID Owner
        {
            get
            {
                return _player;
            }
        }
    }
}
