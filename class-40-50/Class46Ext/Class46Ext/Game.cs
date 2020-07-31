using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Class46Ext
{
    public class Game
    {
        private static PieceType currentPlayer = PieceType.BLACK;
        //現在玩家是誰，是Game類別在管的，別的類別只能唯讀，故做get存取器回報
        internal static PieceType CurrentPlayer { get { return currentPlayer; } }
        //二維陣列，記錄棋盤上要標識的棋子及其位置
        private static Piece[,] piecesMarkYellowNode = new Piece[Board.NODE_COUNT_ONESIDE, Board.NODE_COUNT_ONESIDE];
        internal static Piece[,] PiecesMarkYellowNode { get { return piecesMarkYellowNode; } }
        private static Piece[,] piecesMarkRedNode = new Piece[Board.NODE_COUNT_ONESIDE, Board.NODE_COUNT_ONESIDE];
        internal static Piece[,] PiecesMarkRedNode { get { return piecesMarkRedNode; } }

        internal static int markAlmostWinPiecesYellow()
        {

            Form form1 = Application.OpenForms[0];
            Piece p = null; int i = 0;
            foreach (Piece piecesMarkItem in piecesMarkYellowNode)
            {
                if (piecesMarkItem != null)
                {

                    p = new YellowPiece(piecesMarkItem.Location.X, piecesMarkItem.Location.Y);
                    //p.BringToFront();//不能先執行此行，因為加入表單控制項Controls.Add是以複製的方式傳入Controls集合
                    form1.Controls.Add(p);
                    form1.Controls[form1.Controls.Count - 1].BringToFront();
                    i++;
                }
            }
            return i;
        }

        //記下要標識的棋子數
        static int markPieceCtr, markPieceCtrOpponent;
        internal static int MarkPieceCtr { get { return markPieceCtr; } }
        internal static int MarkPieceCtrOpponent { get { return markPieceCtrOpponent; } }
        //要給form1呼叫的
        internal static Piece PlaceAPiece(int x, int y)
        {
            Piece piece = Board.PlaceAPiece(x, y, currentPlayer);
            if (piece != null)
            {
                //下子之後，檢查有沒有贏家：
                checkWinner(Board.LastPlaceNode);
                //沒有贏家，則檢查有沒有快要贏的：
                checkAlmostWinner(Board.LastPlaceNode);
                if (almostWinner != PieceType.NONE)
                {
                    if (almostWinner == currentPlayer)
                    {
                        markPieceCtr = markAlmostWinPiecesYellow();
                    }
                    else
                    {
                        markPieceCtrOpponent = markAlmostWinPiecesYellow();
                    }
                }
                //換對手下棋
                if (currentPlayer == PieceType.BLACK)
                    currentPlayer = PieceType.WHITE;
                else if (currentPlayer == PieceType.WHITE)
                    currentPlayer = PieceType.BLACK;
            }
            return piece;
        }

        //要由表單form1 Controls.Add()呼叫
        internal static Piece autoPlay()
        {
            Point p = Board.autoPlayAvailable();
            if (p.X == -1)
                return null;
            else
                return PlaceAPiece(p.X, p.Y);
        }



        //要給form1呼叫的
        internal static bool CanBePlaced(int x, int y)
        {
            return Board.CanBePlaced(x, y);
        }

        private static PieceType almostWinner = PieceType.NONE;
        internal static PieceType AlmostWinner { get { return almostWinner; } }
        private static PieceType winner = PieceType.NONE;
        internal static PieceType Winner { get { return winner; } }
        internal static void Reset_almostWinner()
        {
            almostWinner = PieceType.NONE;
            Array.Clear(piecesMarkYellowNode, 0, piecesMarkYellowNode.Length);
        }

        //重啟遊戲
        internal static void ReBootGame()
        {
            //重設winner等值
            winner = PieceType.NONE;
            almostWinner = PieceType.NONE;
            currentPlayer = PieceType.BLACK;//均是黑子先下
            //清除棋盤上棋子的分布記錄
            Board.PiecesClear();
            Array.Clear(piecesMarkYellowNode, 0, piecesMarkYellowNode.Length);
        }

        private static void checkWinner(Point lastPlaceNode)
        {
            //count記下連成幾子-//記錄現在看到幾顆相同的棋子，「=1」表示在下的這子
            int count = 1, countReverse = 0;//反方向檢查是不包括自己，故初始值為0
            int targetX = 0, targetY = 0;
            for (int xDir = -1; xDir <= 1; xDir++)
            {
                for (int yDir = -1; yDir <= 1; yDir++)
                {
                    //略過中心點（自己位置）不檢查，當然要輻射出去八方
                    if (xDir == 0 && yDir == 0)
                        continue;//若是現在下的這一子則略過後面的程式碼，換下個方向檢查
                    while (count < 5)
                    {
                        //int centerX = lastPlaceNode.X;
                        //int centerY= lastPlaceNode.Y;

                        targetX = lastPlaceNode.X + xDir * count;
                        targetY = lastPlaceNode.Y + yDir * count;
                        //檢查是否有子、且顏色是否相同
                        if (targetX < 0 || targetY < 0 || targetX >= Board.NODE_COUNT_ONESIDE ||
                            targetY >= Board.NODE_COUNT_ONESIDE ||
                            Board.GetPieceType(targetX, targetY) == PieceType.NONE ||
                            Board.GetPieceType(targetX, targetY) != currentPlayer)
                        //如果此方向已沒有棋子或換色時，就調頭去看還有沒有同色棋子
                        {
                            countReverse = 0;
                            while (countReverse + count < 5)
                            {
                                countReverse++;
                                targetX = lastPlaceNode.X + xDir * countReverse * -1;//調頭即*-1，即可反方向
                                targetY = lastPlaceNode.Y + yDir * countReverse * -1;
                                if (targetX < 0 || targetY < 0 || targetX >= Board.NODE_COUNT_ONESIDE ||
                                    targetY >= Board.NODE_COUNT_ONESIDE ||
                                    Board.GetPieceType(targetX, targetY) == PieceType.NONE ||
                                    Board.GetPieceType(targetX, targetY) != currentPlayer)
                                //如果反方向已沒有棋子或換色時，就結束此方向的檢查
                                {
                                    goto nextdir;//若該位置無子，或連不到五子則跳出最接近這個break的封閉式迴圈while                            

                                }
                                switch (countReverse + count)
                                {
                                    case 5:
                                        winner = currentPlayer;
                                        return;//贏家確定了，就不用再找了
                                    //case 4:
                                    //    if (currentPlayer == PieceType.BLACK)
                                    //        almostWinner = PieceType.BLACK;
                                    //    else
                                    //        almostWinner = PieceType.WHITE;
                                    //    break;
                                    default:
                                        break;
                                }
                                //if (countReverse + count == 5)
                                //{
                                //    winner = currentPlayer;
                                //    return;//贏家確定了，就不用再找了
                                //}
                                piecesMarkRedNode[targetX, targetY] = new RedPiece(
                                Board.convertToFormPosition(new Point(targetX, targetY)));
                            }
                        }
                        //要五子連棋,找到同色子就加1
                        count++;
                        piecesMarkRedNode[targetX, targetY] = new RedPiece(
                            Board.convertToFormPosition(new Point(targetX, targetY)));
                    }
                    nextdir:
                    if (count == 5)
                    {
                        winner = currentPlayer;
                        return;//決定了贏家之後，就不用再找了
                    }
                    else
                        count = 1;
                    Array.Clear(piecesMarkRedNode, 0, piecesMarkRedNode.Length);
                }
            }

        }

        private static void checkAlmostWinner(Point lastPlaceNode)
        {
            //count記下從現在下的這子開始看到幾顆相同的棋子，「=1」表示在下的這子
            int count = 1, countReverse = 0;//反方向檢查是不包括自己，故初始值為0
            int targetX = 0, targetY = 0;//檢查目標的節點
            int pieceNoneCtr = 0;//記錄空子有幾個，因為快要贏，空位只能留一個
            Point pointYellowPos = new Point(-1, -1);
            for (int xDir = -1; xDir <= 1; xDir++)
            {
                for (int yDir = -1; yDir <= 1; yDir++)
                {
                    //略過中心點（自己位置）不檢查，要輻射出去八方
                    if (xDir == 0 && yDir == 0)
                        continue;//若是現在下的這一子則略過後面的程式碼，換下個方向檢查
                    while (count < 5)
                    {
                        //int centerX = lastPlaceNode.X;
                        //int centerY= lastPlaceNode.Y;

                        targetX = lastPlaceNode.X + xDir * count;
                        targetY = lastPlaceNode.Y + yDir * count;
                        //只會被邊界外與對手棋子給卡死，否則一直延伸5個位置，沒有被障礙到，都有機會贏
                        if (targetX < 0 || targetY < 0 || targetX >= Board.NODE_COUNT_ONESIDE ||
                            targetY >= Board.NODE_COUNT_ONESIDE ||
                            (Board.GetPieceType(targetX, targetY) != PieceType.NONE &
                            Board.GetPieceType(targetX, targetY) != currentPlayer))
                        //如果此方向已沒有棋子或換色時，就調頭去看還有沒有同色棋子
                        {
                            countReverse = 0;
                            while (countReverse + count < 5)
                            {
                                countReverse++;
                                targetX = lastPlaceNode.X + xDir * countReverse * -1;//調頭即*-1，即可反方向
                                targetY = lastPlaceNode.Y + yDir * countReverse * -1;
                                if (targetX < 0 || targetY < 0 || targetX >= Board.NODE_COUNT_ONESIDE ||
                                    targetY >= Board.NODE_COUNT_ONESIDE ||
                                    (Board.GetPieceType(targetX, targetY) != PieceType.NONE &
                                    Board.GetPieceType(targetX, targetY) != currentPlayer))
                                    //如果反方向已被堵死，就結束此方向的檢查                                
                                    goto nextdir;//跳出最接近這個break的封閉式迴圈while                            
                                else if (Board.GetPieceType(targetX, targetY) == PieceType.NONE)
                                    pieceNoneCtr++;
                                else
                                    pointYellowPos = Board.convertToFormPosition(new Point(targetX, targetY));
                                piecesMarkYellowNode[targetX, targetY] =
                                    new YellowPiece(new Point(
                                        pointYellowPos.X + Piece.IMAGE_WIDTH / 2,
                                        pointYellowPos.Y + Piece.IMAGE_WIDTH / 2));

                                if (countReverse + count == 5)
                                {
                                    if (pieceNoneCtr == 1)
                                    {
                                        almostWinner = currentPlayer;
                                        return;//快要贏的確定了，就不用再找了

                                    }
                                    Array.Clear(piecesMarkYellowNode, 0, piecesMarkYellowNode.Length);
                                    break;
                                }

                            }
                        }
                        else if (Board.GetPieceType(targetX, targetY) == PieceType.NONE)
                        {
                            pieceNoneCtr++;
                        }
                        //要五子連棋,找到同色子就加1
                        count++;
                        pointYellowPos = Board.convertToFormPosition(new Point(targetX, targetY));
                        piecesMarkYellowNode[targetX, targetY] =
                            new YellowPiece(new Point(
                                pointYellowPos.X + Piece.IMAGE_WIDTH / 2,
                                pointYellowPos.Y + Piece.IMAGE_WIDTH / 2));
                    }
                    nextdir:
                    if (count == 5)
                    {
                        if (pieceNoneCtr == 1)
                        {
                            almostWinner = currentPlayer;
                            return;//快要贏的確定了，就不用再找了

                        }

                    }
                    count = 1;
                    pieceNoneCtr = 0;
                    Array.Clear(piecesMarkYellowNode, 0, piecesMarkYellowNode.Length);
                }
            }

            if (count == 5)
            {
                if (pieceNoneCtr == 1)
                {
                    almostWinner = currentPlayer;
                    return;//快要贏的確定了，就不用再找了
                }
                //return;
            }
            //找到就會中途離開函式，故到此應是沒有
            //if (almostWinner != PieceType.NONE)
            //    almostWinner = PieceType.NONE;
        }

    }
}
