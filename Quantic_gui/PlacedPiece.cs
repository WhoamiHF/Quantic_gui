using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * Used for compilation type checking ensuring no piece will be placed to board mistakenly
     */
    internal class PlacedPiece : Piece      
    {
        readonly int x;
        readonly int y;
        public PlacedPiece(PlayerID owner, ShapeType shape,int x, int y) : base(owner, shape)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "\t\t"+Shape + "["+ x + "," + y + "]";
        }
    }
}
