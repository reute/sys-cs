using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace tennis
{
    public class Game
    {
        private Scores _scoreP1;
        private Scores _scoreP2;

        public Scores ScoreP1 
        {
            get
            {
                return _scoreP1;
            }
        }

        public Scores ScoreP2
        {
            get
            {
                return _scoreP2;
            }
        }

        public Game()
        {
            _scoreP1 = _scoreP2 = Scores.p0;
        }

        public Game(Scores p1, Scores p2)
        {
            _scoreP1 = p1;
            _scoreP2 = p2;
        } 

        public void P1Scores()
        {
            PlayerScores(ref _scoreP1, ref _scoreP2);
        }

        public void P2Scores()
        {
            PlayerScores(ref _scoreP2, ref _scoreP1);
        }

        private void PlayerScores(ref Scores playerScoring, ref Scores otherPlayer)
        {
            if (playerScoring < Scores.p40 && otherPlayer <= Scores.p40)
            {              
                playerScoring++;
                return;
            }

            if (playerScoring == Scores.p30 && otherPlayer == Scores.p40)
            {
                playerScoring = Scores.Deuce;
                otherPlayer = Scores.Deuce;
                return;
            }

            if (playerScoring == Scores.p40 && otherPlayer < Scores.p40)
            {              
                playerScoring = Scores.Win;
                otherPlayer = Scores.Lose; 
                return;
            }

            if (playerScoring == Scores.Deuce)
            {
                playerScoring = Scores.Adv;
                otherPlayer = Scores.DisAd;
                return;
            }

            if (otherPlayer == Scores.Adv)
            {
                playerScoring = Scores.Deuce;
                otherPlayer = Scores.Deuce;
                return;
            }           

            if (playerScoring == Scores.Adv)
            {
                playerScoring = Scores.Win;
                otherPlayer = Scores.Lose;
                return;
            }           
        }  
        
        public void Restart()
        {
            _scoreP1 = _scoreP2 = 0;
        }
    }

    public enum Scores
    {
        p0,
        p15,
        p30,
        p40,
        Deuce,
        Adv,
        Win,
        Lose,
        DisAd
    }
}
