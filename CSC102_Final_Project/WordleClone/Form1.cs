using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace WordleClone
{

    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        Word word = new Word();
        GameBoard gameBoard = new GameBoard();

        

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            gameBoard.HandleInput(e);

        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FileHandler fileTest = new FileHandler();
            fileTest.ReadFile();
            gameBoard.GenerateBoard(this);
            randWordLabel.Text = word.generateWord();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            focusHolder.Focus();
        }
    }

    public static class Globals
    {
        public static string wordToGuess;
        public static List<char> inputList = new List<char>();
        public static Dictionary<string, int> wordDict = new Dictionary<string, int>(); // Create dictionary to hold word and count
    }




    public class FileHandler
    {
        public void ReadFile()
        {
            try
            {
                
                StreamReader inputFile;
                inputFile = File.OpenText("wordlist.txt");

                string[] testRead = File.ReadAllLines("wordlist.txt"); // Read file to array

                for (int i = 0; i < testRead.Length; i++)
                {
                    string[] splitLine = testRead[i].Split(','); // split each array item into sub array
                    Globals.wordDict.Add(splitLine[0], int.Parse(splitLine[1])); // add array items to dictionary as key and value pair
                }
            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class GameBoard
    {
        public static int row = 0;
        public static int col = 0;
        public static Label[,] boardArray;

        public void GenerateBoard(Form1 form)
        {
            boardArray = new Label[,]
            {
                {form.label1, form.label2, form.label3, form.label4, form.label5},
                {form.label6, form.label7, form.label8, form.label9, form.label10},
                {form.label11, form.label12, form.label13, form.label14, form.label15},
                {form.label16, form.label17, form.label18, form.label19, form.label20},
                {form.label21, form.label22, form.label23, form.label24, form.label25},
                {form.label26, form.label27, form.label28, form.label29, form.label30}
            };
        }

        public void HandleInput(KeyPressEventArgs e)
        {
           
            if (e.KeyChar == (char)Keys.Back)
            {
                if (col > 0)
                {
                    col--;
                    Globals.inputList.RemoveAt(Globals.inputList.Count - 1);
                    boardArray[row, col].Text = "";
                }
            }

            else if (e.KeyChar == (char)Keys.Enter)
            {
               if (col == 5)
                {
                    Input input = new Input();
                    if (input.CheckWord())
                    {
                        input.ChangeColorGreen();
                        input.ChangeColorYellow();
                        input.ChangeColorGray();
                        row++;
                        col = 0;
                        Globals.inputList.Clear();
                    }
                   else
                    {
                        MessageBox.Show("Invalid word.");
                    }
                }
               else
                {
                    MessageBox.Show("Moron");
                }
            }

            else if (col <= 4)
            { 
                char pressedKey = e.KeyChar;
                pressedKey = char.ToUpper(pressedKey);
                if (char.IsLetter(pressedKey))
                {
                    Globals.inputList.Add(pressedKey);
                    boardArray[row, col].Text = pressedKey.ToString();
                    col++;
                }
            }
        }

        public void ResetBoard()
        {
            foreach (Label label in boardArray)
            {
                label.Text = "";
            }
        }
    }


    public class Word
    {
        List<string> wordList = new List<string>(); 
        public string generateWord()
        {
            int minValue = Globals.wordDict.Values.Min();
            foreach (var item in Globals.wordDict)
            {
                if(item.Value == minValue)
                {
                    wordList.Add(item.Key);
                }
            }

            Random rand = new Random();

            Globals.wordToGuess = wordList[rand.Next(0, wordList.Count)];
            Globals.wordDict[Globals.wordToGuess]++;
            return Globals.wordToGuess;
        }
    }

    public class Input
    {
        public List<char> tempInput = Globals.inputList;
        public List<char> tempWordToGuess = new List<char>();

        public Input()
        {
            tempWordToGuess.AddRange(Globals.wordToGuess.ToCharArray());
        }

        public bool CheckWord()
        {
            string userWord = string.Join("", Globals.inputList);
            return Globals.wordDict.ContainsKey(userWord);
        }

        public void ChangeColorGreen()
        {
            for (int i = 0; i < Globals.wordToGuess.Length; i++)
            {
                if (Globals.wordToGuess[i] == Globals.inputList[i])
                {
                    GameBoard.boardArray[GameBoard.row, i].BackColor = Color.Green;
                    tempWordToGuess[i] = ' ';
                    tempInput[i] = ' ';
                }
            }
        }

        public void ChangeColorYellow()
        {
            for (int i = 0; i < Globals.wordToGuess.Length; i++)
            {
                if (GameBoard.boardArray[GameBoard.row, i].BackColor != Color.Green && GameBoard.boardArray[GameBoard.row, i].BackColor != Color.Yellow)
                {
                    if (tempWordToGuess.Contains(tempInput[i]))
                    {
                        GameBoard.boardArray[GameBoard.row, i].BackColor = Color.Yellow;
                        tempWordToGuess.Remove(tempInput[i]);
                        tempInput[i] = ' ';
                    }
                }
            }
        }

        public void ChangeColorGray()
        {
            int i = 0;
            foreach (char c in tempInput)
            {
                if (c != ' ')
                {
                    GameBoard.boardArray[GameBoard.row, i].BackColor = Color.Gray;
                }
                i++;
            }
        }
    }

}
