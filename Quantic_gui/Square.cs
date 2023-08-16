using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * Represents one square on the board. May or may not containg a piece
     */
    internal class Square
    {
        PlacedPiece? _piece;
        public Square() {
            _piece = null;
        }

        public Square(PlacedPiece? piece)
        {
            this._piece = piece;
        }

        public void SetPiece(PlacedPiece piece)
        {
            this._piece = piece; 
        }

        /**
         * Gets symbol, used for board drawing
         */
        public char GetSymbol()
        {
            if(_piece == null)
            {
                return '.';
            }
            else
            {
                return _piece.GetSymbol();
            }
        }
        
        public PlacedPiece? Piece
        {
            get
            {
                return _piece;
            }
        }
    }
}
