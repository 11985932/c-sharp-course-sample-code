﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Class46Ext
{
    public class BlackPiece:Piece
    {
        public BlackPiece(int x,int y) : base(x, y)
        {
            Image = Properties.Resources.black;
        }
        public BlackPiece(Point point) : base(point)
        {
            Image = Properties.Resources.black;
        }
        internal override PieceType GetPieceType()
        {
            return PieceType.BLACK;
        }
    }
}
