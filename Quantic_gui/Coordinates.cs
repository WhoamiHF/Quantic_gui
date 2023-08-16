using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * Class to store x and y coordinate
     */
    internal class Coordinates
    {
        readonly int x;
        readonly int y;
        public Coordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }
    }
}
