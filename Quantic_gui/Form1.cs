using System.Security.Cryptography.Xml;
using System.Windows.Forms;
using System;
using Quantic_console;
using static System.Windows.Forms.AxHost;

namespace Quantic_gui
{
    public partial class Form1 : Form
    {

        Game game;
        GameLogic logic;
        GUIBoardViewer boardViewer;
        Logger logger;

        public Form1()
        {
            InitializeComponent();
        }

        public void SetPlayerXPlayer(object sender, EventArgs e)
        {
            game = new Game();
            logger = new Logger(this);
            logic = new GameLogic(logger);
            boardViewer = new GUIBoardViewer(game, logic, this, logic.Logger!);
            PlayerXComputer.Visible = false;
            PlayerXPlayer.Visible = false;
            pictureBox1.Visible = false;

            boardViewer.SetBoard(Controls);
            //game.SetComputerPlayer(logic.Logger!);
            game.State = Game.GameState.FIRST_PLAYER_TURN;
        }

        public void SetPlayerXComputer(object sender, EventArgs e)
        {
            game = new Game();
            logger = new Logger(this);
            logic = new GameLogic(logger);
            boardViewer = new GUIBoardViewer(game, logic, this, logic.Logger!);
            PlayerXComputer.Visible = false;
            PlayerXPlayer.Visible = false;
            pictureBox1.Visible = false;

            boardViewer.SetBoard(Controls);
            game.SetComputerPlayer(logic.Logger!);
            game.State = Game.GameState.FIRST_PLAYER_TURN;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }
    }
}