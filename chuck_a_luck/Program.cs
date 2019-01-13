using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace chuck_a_luck
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Intro();
            var players = p.AddPlayers();
            p.StartMatch(players);
            p.ExitProgram();
        }

        private void Intro()
        {
            Console.WriteLine("**** Chuck-a-luck ****\nSie haben 1000 Geldeinheiten\nIn jeder Runde können Sie einen Teil davon auf eine der Zahlen 1 bis 6 setzen. Danach werden 3 Würfel geworfen. Falls Ihr Wert dabei ist, erhalten Sie Ihren Einsatz zurück und zusatzlich Ihren Einsatz fuer jeden Würfel,der die von Ihnen gesetzte Zahl zeigt.");
        }

        private void StartMatch(List<Player> players)
        {
            var m = new Match(players, new Dice());
            m.Start();
        }

        private List<Player> AddPlayers()
        {
            var players = new List<Player>();
            int i = 1;
            while (true)
            {
                Console.Write($"Name Player {i}: ");
                var name = Console.ReadLine();
                if (string.IsNullOrEmpty(name))
                    return players;
                players.Add(new Player(name));
                i++;
            }
        }

        public void ExitProgram()
        {
            Console.ReadLine();
            Environment.Exit(1);
        }
    }

    public class Player
    {
        public readonly string name;
        public readonly List<RoundData> gameHistory = new List<RoundData>();
        public int initialCredits, credits, stake, numberGuessed, correctGuess, result;
        public bool Quit
        {
            get
            {
                if (stake == 0 || credits <= 0)
                    return true;
                else
                    return false;
            }
        }

        public Player(string name, int credits = 100)
        {
            this.name = name;
            this.credits = initialCredits = credits;
        }

        public void SaveRound()
        {

        }

        public struct RoundData
        {
            public int CreditEnd, Result, NumberGuessed, Stake;

            public RoundData(int creditEnd, int stake, int numberGuessed, int result)
            {                
                CreditEnd = creditEnd;
                NumberGuessed = numberGuessed;
                Result = result;
                Stake = stake;
            }
        }
    }

    class Match
    {
        private readonly List<Player> allPlayers;
        private List<Player> activePlayers;
        Dice dice;

        public Match(List<Player> allPlayers, Dice dice)
        {
            this.allPlayers = activePlayers = allPlayers;
            this.dice = dice;
        }

        public void Start()
        {
            while (activePlayers.Count > 0)
            {
                new Round(activePlayers, dice.Numbers).Start();
                activePlayers = activePlayers.Where(p => p.Quit == false).ToList();
            }
            GameOver();
        }

        private void GameOver()
        {
            Console.WriteLine("No Player left, leaving Game\n");
        }
    }

    class Round
    {
        private readonly List<Player> players;
        private readonly List<int> numbers;

        public Round(List<Player> players, List<int> numbers)
        {
            this.players = players;
            this.numbers = numbers;
        }

        public void Start()
        {
            GetBets();
            CalcResult(numbers);
            OutputResult(numbers);
            SaveRound();
        }

        private void GetBets()
        {
            foreach (var p in players)
            {
                PrintStatus(p);
                InputStake(p);
                if (p.Quit)
                    continue;
                InputGuessedNumber(p);
            }           
        }

        public void PrintStatus(Player p)
        {
            Console.WriteLine($"{p.name}'s turn ! ");
            Console.WriteLine($"Sie haben {p.credits} Geldeinheiten");
        }

        public void InputStake(Player p)
        {
            Console.Write("Ihr Einsatz: ");
            var line = Console.ReadLine();
            p.stake = Int32.Parse(line);
            // Mit properties besser
            if (p.stake == 0)
            {
                Console.WriteLine($"{p.name} verläßt das Casino mit {p.credits} Geldeinheiten!!");
            }
        }

        public void InputGuessedNumber(Player p)
        {
            Console.Write("Ihre Zahl: ");
            string line = Console.ReadLine();
            p. = Int32.Parse(line);
        }

        public void CalcResult(List<int> numbers)
        {
            foreach (var number in numbers)
            {
                foreach (var p in players)
                {
                    if (p.numberGuessed == number)
                        p.correctGuess++;
                }
            }
            foreach (var p in players)
            {
                if (p.correctGuess > 0)                
                    p.result = p.correctGuess * p.stake; 
                else
                    p.result = -p.stake;

                p.credits += p.result;

                p.his.Add(new Round(MoneyBegin, Money, Bet, Number, Result));
            }

        }

        public void OutputResult(List<int> numbers)
        {
            Console.Write($"Die Würfel sind gefallen: ");
            Console.WriteLine(numbers.ToString());

            foreach (var p in players)
            {
                if (p.credits == 0)
                    Console.WriteLine($"Sorry {p.name}, sie haben kein Geld mehr und sind raus !!");

                if (p.correctGuess > 0)
                    Console.WriteLine($"Glückwunsch {p.name}, Sie erhalten {p.numberGuessed * p.stake} Geldeinheiten !!");
                else
                    Console.WriteLine($"Pech {p.name}, da war nichts für Sie dabei, sie verlieren {p.stake} Geldeinheiten !!");
            }
        }

        public void SaveRound()
        {
            
        }

      
    }

    class Dice
    {
        private Random rnd;
        private const int NumberThrows = 3;
        private const int Min = 0;
        private const int Max = 6;

        public Dice()
        {
            rnd = new Random();
        }   

        public List<int> Numbers
        {
            get
            {
                List<int> numbers = new List<int>();
                for (int i = 0; i < NumberThrows; i++)
                {
                    numbers.Add(rnd.Next(Min, Max));
                }
                return numbers;
            }
        }
    }
}
