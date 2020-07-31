using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Class46Ext
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //表單背景圖片
            BackgroundImage = Properties.Resources.board;
            ////掛上表單事件連結
            //Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
            //設定表單長寬為棋盤圖檔的長寬
            Height = Properties.Resources.board.Height;
            Width = Properties.Resources.board.Width;
            //在表單上端安放提示現在該誰下的棋子
            Controls.Add(Board.PlaceCurrPlayerPiece());
            //設定表單出現在螢幕的正中央
            StartPosition = FormStartPosition.CenterScreen;
        }

        void marWinnerRedPiece()
        {
            //二維陣列中有些是null，沒有RedPiece存在，仍需用定義時的型別Piece才妥
            foreach (Piece redPiece in Game.PiecesMarkRedNode)
            {
                if (redPiece != null)
                {
                    Controls.Add(redPiece);
                    //在這裡最後加的控制項索引值是最大值：
                    Controls[Controls.Count - 1].BringToFront();
                }
            }
            Refresh();//這裡用Refresh才可以即時顯示
            Thread.Sleep(500);
            clearMarkedPieces();
        }

        int pieceCount = 1;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //當對表單（視窗）按下Esc鍵時，關閉表單
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void reBootGame()
        {
            Game.ReBootGame();
            Controls.Clear();
            //在表單上端安放提示現在該誰下的棋子
            Controls.Add(Board.PlaceCurrPlayerPiece());
            pieceCount = 1;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Piece piece = Game.PlaceAPiece(e.X, e.Y);
            if (piece != null)//如果可以下棋子
            {
                //移除頂端提示之棋子
                foreach (Piece item in Controls)
                {
                    if (item.Location.Y == 0)
                    {
                        Controls.Remove(item);
                        pieceCount--;
                    }
                }
                //加入現在下的棋子
                Controls.Add(piece);
                pieceCount++;

                ////●●以下自動下棋，不要自動下棋把此拿掉即可
                //autoPlay();

                //加入頂端提示的棋子
                Controls.Add(Board.PlaceCurrPlayerPiece());
                pieceCount++;
                //檢查是否有人獲勝，若有即出現提示訊息並重啟遊戲
                if (Game.Winner == PieceType.BLACK)
                {
                    marWinnerRedPiece();
                    MessageBox.Show("黑子獲勝！");
                    reBootGame();
                }
                else if (Game.Winner == PieceType.WHITE)
                {
                    marWinnerRedPiece();
                    MessageBox.Show("白子獲勝！");
                    reBootGame();
                }

                //檢查是否有人快贏了，若有即出現提示訊息

                if (Game.AlmostWinner == PieceType.BLACK)
                {
                    pieceCount += Game.MarkPieceCtr;
                    MessageBox.Show("黑子快要贏了！");
                    clearMarkedPieces();
                    Game.Reset_almostWinner();
                }//因為有自動下棋，故二者均有可能，都要檢查，故不能用 else
                if (Game.AlmostWinner == PieceType.WHITE)
                {
                    pieceCount += Game.MarkPieceCtr;
                    MessageBox.Show("白子快要贏了！");
                    clearMarkedPieces();
                    Game.Reset_almostWinner();
                }
            }
        }

        private void autoPlay()
        {
            //自動下棋功能還要再改良！！！           
            Piece pieceAuto = Game.autoPlay();
            this.Refresh();
            //要做個延遲
            Thread.Sleep(200);
            //自動下棋
            if (pieceAuto != null)
            {
                Controls.Add(pieceAuto);
                pieceCount++;
            }
            else
                MessageBox.Show("棋盤已滿！");
        }

        void clearMarkedPieces()
        {
            while (Controls[0].GetType().Name == "YellowPiece" ||
                Controls[0].GetType().Name == "RedPiece")
            {
                Controls.Remove(Controls[0]);//表單控制項最後加入的會在最下層，而索引值為0
                pieceCount--;
            }

            //讀取「Game.PiecesMarkYellow」其元素都是null，故不必以下這行來清空了
            //應該是被自動下棋的後者給覆寫了，不是讀不到
            Array.Clear(Game.PiecesMarkYellowNode, 0, Game.PiecesMarkYellowNode.Length);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //可見果然不必再用new做個執行個體才行(重構小山老師菩薩的)
            if (Game.CanBePlaced(e.X, e.Y))
                Cursor = Cursors.Hand;//可以下棋子則游標呈手形
            else
                Cursor = Cursors.Default;

        }


    }
}
