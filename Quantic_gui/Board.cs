using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * This class is used for storing information about board state. 
     * The board contains 16 squares, each of them may or may not 
     * contain a piece placed by user.
     */
    internal class Board
    {
       
        //Squares to store references to pieces
        private Square[][] _squares;

        public Board()
        {
            CreateSquares();
        }

        /**
         * Copy constructor
         */
        public Board(Board other)
        {
            _squares = new Square[4][];
            for (int x = 0; x < 4; x++)
            {
                _squares[x] = new Square[4];
                for (int y = 0; y < 4; y++)
                {
                    _squares[x][y] = new Square();
                    _squares[x][y].SetPiece(other.Squares[x][y].Piece);
                }
            }
        }

        /**
         * Method enabling to perform moves on copies of board during minimax
         */
        public Board Copy()
        {
            return new Board(this);
        }

      
        public Square[][] Squares
        {
            get
            {
                return _squares;
            }
        }

        /**
         * Method to create four by four grid of squares
         */
        private void CreateSquares()
        {
            _squares = new Square[4][];
            for (int x  = 0; x < 4; x++)
            {
                _squares[x] = new Square[4];
                for (int y = 0; y < 4; y++)
                {
                    _squares[x][y] = new Square();
                }
            }
        }
    }
}
