using Microsoft.VisualStudio.TestTools.UnitTesting;
using tennis;

namespace tennisTest
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void PlayerScores__p1_0_p2_0_p1_scores__ScoreIncreases()
        {
            Game game = new Game();
            game.P1Scores();
            Assert.AreEqual(game.ScoreP1, Scores.p15);
        }

        [TestMethod]
        public void PlayerScores__p1_40_p2_0_p1_scores__p1_wins()
        {
            Game game = new Game(Scores.p40, Scores.p0);
            game.P1Scores();
            Assert.AreEqual(game.ScoreP1, Scores.Win);
        }

        [TestMethod]
        public void PlayerScores__p1_Ad_p2_DisAd_p1_scores__p1_wins()
        {
            Game game = new Game(Scores.Adv, Scores.DisAd);
            game.P1Scores();
            Assert.AreEqual(game.ScoreP1, Scores.Win);
        }
    }
}
