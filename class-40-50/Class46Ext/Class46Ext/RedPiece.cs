using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Class46Ext
{
    public class RedPiece:Piece
    {
        public RedPiece(int x,int y):base(x,y)
        {
            Image = Properties.Resources.red;
        }
        public RedPiece(Point point):base(point)
        {
            Image = Properties.Resources.red;
        }
        internal override PieceType GetPieceType() {
            return PieceType.RED;
        }
    }
}
