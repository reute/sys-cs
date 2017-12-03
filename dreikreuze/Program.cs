using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dreikreuze
{
    class Program
    {
        static void Main(string[] args)
        {
            new Game().Start();
        }

        public static void Exit()
        {
            Console.ReadLine();
            Environment.Exit(1);
        }
    }

    public interface IPlayer
    {      
        string Name { get; set; }
        int Turn(PlayField field);
    }

    class Game
    {
        PlayField PlayField = new PlayField();
        IPlayer CurrentPlayer;
        IPlayer[] Players = new IPlayer[2];

        public Game()
        {
            Console.SetWindowSize(100, 50);
            Players[0] = new ComputerPlayer();
            Players[1] = new HumanPlayer();
        }

        public void Start()
        {                         
            Intro();
            Prepare();
            GameLoop();
        }

        private void Intro()
        {
            Console.WriteLine("*** Drei Kreuze ***\nGegeben ist eine Kette von 23 freien Feldern. In jedem Zug setzt jeder der Spieler ein X auf ein freies Feld. Wenn dadurch drei oder mehr X benachbart sind, hat der Spieler gewonnen.  ");           
        }


        private void Prepare()
        {
            Console.WriteLine("Wollen Sie anfangen? Ja=1 Nein=2 ");
            if (Console.ReadLine() == "j")
            {
                CurrentPlayer = Players[1];    
            }
            else
            {
                CurrentPlayer = Players[0];
            }
            PlayField.Draw();
        }

        private IPlayer SwitchPlayers()
        {
            if (CurrentPlayer == Players[0])
                return Players[1];
            else
                return Players[0];
        }


        private void GameLoop()
        {       
            do
            {               
                CurrentPlayer = SwitchPlayers();
                CurrentPlayer.Turn(PlayField);
                PlayField.Draw();
            } while (!PlayField.GameOver);
            Console.WriteLine($"{CurrentPlayer.Name} won !");
            Program.Exit();
        }        
    }

    public class HumanPlayer : IPlayer
    {
        public string Name { get; set; } = "Player";

        public int Turn(PlayField playField)
        {
            int number;
            while (true) {
                Console.WriteLine($"Bitte Zahl zwischen 0 und {PlayField.Size} eingeben: ");
                number = Int32.Parse(Console.ReadLine());
                if (playField.UseField(number))
                    break;
                else
                    Console.Write($"Nummer {number} bereits belegt !");
            }
            return number;
        }
    }

    public class ComputerPlayer : IPlayer
    {
        public string Name { get; set; } = "Computer";

        Dice dice = new Dice();

        public int Turn(PlayField playField)
        {       
            int number;
            while (true)
            {
                number = dice.Roll();
                if (playField.UseField(number))
                    break;               
            }
            Console.WriteLine($"{Name} sets X at position {number}");
            return number;
        }
    }

    public class PlayField
    {
        public const int Size = 23;

        private int FieldData;

        public bool FieldsLeft
        {
            get
            {
                return FieldData < (2 ^ Size);
            }
        }

        public bool GameOver
        {
            get
            {
                return CalcGameOver();
            }
        }

        private bool FieldUsed(int index)
        {
            var tmp = 1 << index;
            if ((tmp & FieldData) == tmp)
                return true;
            else
                return false;
        }

        public bool UseField(int index)
        {
            if (!FieldUsed(index)) 
            {
                var tmp = 1 << index;
                FieldData = FieldData | tmp;
                return true;
            }
            return false;            
        }

        private bool CalcGameOver()
        {
            int crosses = 0;
            for (int i = 0; i < Size; i++)
            {
                if (FieldUsed(i))
                {
                    crosses += 1;
                    if (crosses == 3)
                        return true;
                } 
                else
                {
                    crosses = 0;
                }
            }
            return false;
        }

        public void Draw()
        {
            Console.WriteLine();
            Console.WriteLine(CreateFieldString(" \\/"));
            Console.WriteLine(CreateFieldString(" /\\"));
            Console.WriteLine(CreateIndexString());
        }

        private string CreateFieldString(string stringUsed)
        {
            string stringFree = "   ";            
            StringBuilder line = new StringBuilder();
            for (int i = 0; i < Size; i++)
            {
                
                if (FieldUsed(i))
                {
                    line.Append(stringUsed);
                }
                else
                {
                    line.Append(stringFree);
                }     
            }
            return line.ToString();
        }

        private string CreateIndexString()
        {
            StringBuilder line = new StringBuilder();
            for (int i = 0; i < Size; i++)
            {
                line.Append(string.Format("{0,3:D}", i));
            }            
            return line.ToString();
        }
    }

    class Dice
    {         
        private Random rnd;
        private const int Min = 0;
        private const int Max = PlayField.Size;

        public Dice()
        {
            rnd = new Random();
        }

        public int Roll()
        {
            return rnd.Next(Min, Max);
        }   
    }
}
