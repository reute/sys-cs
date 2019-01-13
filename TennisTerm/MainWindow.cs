using System;
using System.Collections.Generic;
using System.Text;
using Terminal.Gui;

namespace TennisTerm
{
    public class MainWindow
    {
        public Game Game = new Game();       

        private readonly Dictionary<Scores, string> ScoresString = new Dictionary<Scores, string>
        {
            {Scores.p0, "0" },
            {Scores.p15, "15" },
            {Scores.p30, "30" },
            {Scores.p40, "40" },
            {Scores.Deuce, "Deuce" },
            {Scores.Adv, "Advantage" },
            {Scores.DisAd, "" },
            {Scores.Win, "Winner" },
            {Scores.Lose, "Loser" },
        }; 

        Label labelP1, labelP2;
        Button btnP1, btnP2, btnRestart;
        Window win;

        public void Init()
        {
            Application.Init();

            win = new Window("Hello")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            Application.Top.Add(win);

            labelP1 = new Label("P1")
            {
                X = Pos.At(5),
                Y = Pos.At(5),
                Height = 2,
            };
            win.Add(labelP1);

            labelP2 = new Label("P2")
            {
                X = Pos.At(20),
                Y = Pos.At(5),
                Height = 2,
            };
            win.Add(labelP2);

            btnP1 = new Button("P1 Score")
            {
                X = Pos.At(5),
                Y = Pos.At(10),
                Height = 2,
                Clicked = P1Scores
             };

            win.Add(btnP1);        

            btnP2 = new Button("P2 Score")
            {
                X = Pos.At(20),
                Y = Pos.At(10),
                Height = 2,
                Clicked = P2Scores
            };
            win.Add(btnP2);

            btnRestart = new Button("Restart")
            {
                X = Pos.At(35),
                Y = Pos.At(10),
                Height = 2,
                Clicked = Restart
            };
            win.Add(btnRestart);           

            Application.Run();
        }

        private void P1Scores()
        {
            Game.P1Scores();         
            labelP1.Text = "P1 " + ScoresString[Game.ScoreP1];
            labelP2.Text = "P2 " + ScoresString[Game.ScoreP2];
        }

        private void Restart()
        {
            Game.Restart();
            labelP1.Text = "P1 " + ScoresString[Game.ScoreP1];
            labelP2.Text = "P2 " + ScoresString[Game.ScoreP2];
        }

        private void P2Scores()
        {
            Game.P2Scores();
            labelP1.Text = "P1 " + ScoresString[Game.ScoreP1];
            labelP2.Text = "P2 " + ScoresString[Game.ScoreP2];
        }
    }
}
