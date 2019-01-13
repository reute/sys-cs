using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace tennis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;   

        public Game Game = new Game();      

        public string ScoreP1String
        {
            get
            {
                return ScoresString[Game.ScoreP1];
            }
        }

        public string ScoreP2String
        {
            get
            {
                return ScoresString[Game.ScoreP2];
            }
        }

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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void P1Scores(object sender, RoutedEventArgs e)
        {
            Game.P1Scores();
            NotifyPropertyChanged(nameof(ScoreP1String));
            NotifyPropertyChanged(nameof(ScoreP2String));
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            Game.Restart();
            NotifyPropertyChanged(nameof(ScoreP1String));
            NotifyPropertyChanged(nameof(ScoreP2String));
        }

        private void P2Scores(object sender, RoutedEventArgs e)
        {
            Game.P2Scores();
            NotifyPropertyChanged(nameof(ScoreP1String));
            NotifyPropertyChanged(nameof(ScoreP2String));
        }
    }
}
