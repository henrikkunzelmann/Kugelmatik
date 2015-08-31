using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using KugelmatikLibrary;

namespace KugelmatikControl.PingPong
{
    public class World
    {
        public const int Width = 15;
        public const int Height = 20;

        public const int PaddleWidth = 3;
        public const int PaddleHeight = 1;

        public const int BallSize = 1;

        public Game Game { get; private set; }
        public Kugelmatik Kugelmatik { get { return Game.Kugelmatik; } } 

        public Player PlayerTop { get; private set; }
        public Player PlayerBottom { get; private set; }

        private PointF ball;
        private PointF ballSpeed;

        private Random random = new Random();

        private int respawnTime = 0;

        public World(Game game)
        {
            if (game == null)
                throw new ArgumentNullException("game");

            this.Game = game;

            this.PlayerTop = new Player();
            this.PlayerBottom = new Player();

            int initPaddle = Width / 2 - PaddleWidth / 2;
            this.PlayerTop.Position = initPaddle;
            this.PlayerBottom.Position = initPaddle;

            ResetBall();
        }

        private void ResetBall()
        {
            ball = new PointF(Width / 2 - BallSize / 2, Height / 2 - BallSize / 2);

            const double ballSpeedMultiplier = 0.1;
            double ballDirection = (Math.PI / 2) * random.Next(0, 4) + Math.PI / 4;

            ballSpeed = new PointF((float)(ballSpeedMultiplier * Math.Cos(ballDirection)), (float)(ballSpeedMultiplier * Math.Sin(ballDirection)));

            respawnTime = 30;
        }

        private void MoveComputerPlayer(Player player)
        {
            int pos = player.Position + PaddleWidth / 2;
            int diff = pos - (int)Math.Round(ball.X, MidpointRounding.AwayFromZero);
            if (diff > 1 && player.Position > 0)
                player.Position--;
            else if (diff < 1 && player.Position < Width - PaddleWidth)
                player.Position++;
        }

        public void Update()
        {
            if (PlayerTop.IsComputer)
            {
                if (ball.Y < Height / 2 && ballSpeed.Y < 0)
                    MoveComputerPlayer(PlayerTop);
            }
            else
            {
                if (Game.IsKeyDown(Keys.A) && PlayerTop.Position > 0)
                    PlayerTop.Position -= 1;
                if (Game.IsKeyDown(Keys.D) && PlayerTop.Position < Width - PaddleWidth)
                    PlayerTop.Position += 1;
            }

            if (PlayerBottom.IsComputer)
            {
                if (ball.Y > Height / 2 && ballSpeed.Y > 0)
                    MoveComputerPlayer(PlayerBottom);
            }
            else
            {
                if (Game.IsKeyDown(Keys.J) && PlayerBottom.Position > 0)
                    PlayerBottom.Position -= 1;
                if (Game.IsKeyDown(Keys.L) && PlayerBottom.Position < Width - PaddleWidth)
                    PlayerBottom.Position += 1;
            }

            if (respawnTime-- > 0)
                return;

            ball.X += ballSpeed.X;
            ball.Y += ballSpeed.Y;
            if (ball.X < 0)
            {
                ball.X = 0;
                ballSpeed.X = -ballSpeed.X;
            }
            if (ball.X > Width)
            {
                ball.X = Width;
                ballSpeed.X = -ballSpeed.X;
            }

            if (ball.Y >= 0 && ball.Y <= PaddleHeight)
            {
                if (ball.X >= PlayerTop.Position && ball.X < PlayerTop.Position + PaddleWidth)
                {
                    ballSpeed.Y = -ballSpeed.Y;
                    ball.Y += ballSpeed.Y;
                }
            }
            else if (ball.Y < -1)
            {
                PlayerBottom.Score++;
                ResetBall();
            }

            if (ball.Y >= Height - PaddleHeight && ball.Y <= Height)
            {
                if (ball.X >= PlayerBottom.Position && ball.X < PlayerBottom.Position + PaddleWidth)
                {
                    ballSpeed.Y = -ballSpeed.Y;
                    ball.Y += ballSpeed.Y;
                }
            }
            else if (ball.Y > Height)
            {
                PlayerTop.Score++;
                ResetBall();
            }
        }

        public void DrawToKugelmatik()
        {
            const ushort minHeight = 100;
            const ushort maxHeight = 1000;

            const int offsetX = 2;
            const int offsetY = 2;

            Game.Kugelmatik.SetRectangle(offsetX - 1, offsetY - 1, Width + 2, Height + 2, maxHeight);
            Game.Kugelmatik.SetRectangle(offsetX, offsetY, Width, Height, minHeight);
            Game.Kugelmatik.SetRectangle(offsetX, offsetY - 1, Width, Height + 2, minHeight);

            Game.Kugelmatik.SetRectangle(offsetX + PlayerTop.Position, offsetY, PaddleWidth, PaddleHeight, maxHeight);
            Game.Kugelmatik.SetRectangle(offsetX + PlayerBottom.Position, offsetY + Height - PaddleHeight, PaddleWidth, PaddleHeight, maxHeight);

            int ballX = (int)Math.Round(ball.X, MidpointRounding.AwayFromZero);
            int ballY = (int)Math.Round(ball.Y, MidpointRounding.AwayFromZero);

            Game.Kugelmatik.SetRectangle(offsetX + ballX, offsetY + ballY, BallSize, BallSize, maxHeight);
            Game.Kugelmatik.SendData();
        }
    }
}
