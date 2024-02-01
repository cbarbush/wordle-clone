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


namespace CSC102_Final_Project
{

    public partial class Form1 : Form
    {
        public Label[,] boardArray;
        
        
        public Form1()
        {
            InitializeComponent();
        }

        GameBoard gameBoard = new GameBoard(); // create instance of GameBoard

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            gameBoard.HandleInput(e); // pass input into the handle input method

        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            FileHandler.WriteFile(); // update txt file
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FileHandler fileTest = new FileHandler();
            fileTest.ReadFile(); // get words from file
            gameBoard.GenerateBoard(this); // add labels to array 
            randWordLabel.Text = Word.generateWord(); // add word to label for debugging
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            WordleGame.Load();
            label1.Focus(); // remove focus from button
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            WordleGame.Reset();
            WordleGame.ResetCounters();
            label1.Focus(); // remove focus from button
        }
    }

    // declare global variables
    public static class Globals
    {
        public static bool gameOver = false;
        public static int attempts = 0;
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

                string[] testRead = File.ReadAllLines("wordlist.txt"); // read file to array

                for (int i = 0; i < testRead.Length; i++)
                {
                    string[] splitLine = testRead[i].Split(','); // split each array item into sub array
                    Globals.wordDict.Add(splitLine[0], int.Parse(splitLine[1])); // add array items to dictionary as key and value pair
                }
                inputFile.Close(); // close file
            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void WriteFile()
        {
            try
            {
                StreamWriter outputFile;
                outputFile = File.CreateText("wordlist.txt");
                List<string> outputList = new List<string>(); 

                foreach(var word in Globals.wordDict) // loop through word
                {
                    string result = word.Key + ',' + word.Value; // join the word and count
                    outputList.Add(result);
                }
                foreach(string i in outputList)
                {
                    outputFile.WriteLine(i); // write new word and count to file
                }
                outputFile.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class GameBoard
    {
        public static int row = 0; // static variables to reference the row and column of the boardArray
        public static int col = 0;
        public static Label[,] boardArray;

        public void GenerateBoard(Form1 form)
        {
            boardArray = new Label[,] // multi-dimensional array to hold labels
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
            if (!Globals.gameOver) // check if game is over
            {
                if (Globals.attempts < 6) // check if user has attempts left
                {

                    if (e.KeyChar == (char)Keys.Back) // handle backspace input
                    {
                        if (col > 0)
                        {
                            col--;
                            Globals.inputList.RemoveAt(Globals.inputList.Count - 1);
                            boardArray[row, col].Text = "";
                        }
                    }

                    else if (e.KeyChar == (char)Keys.Enter) // handle enter input
                    {
                        if (col == 5)
                        {
                            Input input = new Input();
                            if (input.CheckWord())
                            {
                                Globals.attempts++;
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
                            MessageBox.Show("Input must be five characters.");
                        }
                    }

                    else if (col <= 4)
                    {
                        char pressedKey = e.KeyChar; // convert input to char
                        pressedKey = char.ToUpper(pressedKey);
                        if (char.IsLetter(pressedKey)) // validate if the key is a letter
                        {
                            Globals.inputList.Add(pressedKey);
                            boardArray[row, col].Text = pressedKey.ToString();
                            col++;
                        }
                    }
                }
            }
        }
    }


    public class Word
    {
        static List<string> wordList = new List<string>(); 
        public static string generateWord()
        {
            int minValue = Globals.wordDict.Values.Min(); // get the minimum value from the wordDict
            foreach (var item in Globals.wordDict) // loop to find all words with the minimum value as their count
            {
                if(item.Value == minValue)
                {
                    wordList.Add(item.Key);
                }
            }

            Random rand = new Random();

            Globals.wordToGuess = wordList[rand.Next(0, wordList.Count)]; // get random word
            Globals.wordDict[Globals.wordToGuess]++;
            return Globals.wordToGuess;
        }
    }

    public class Input
    {
        public List<char> tempInput = Globals.inputList; // temporary list to hold user input
        public List<char> tempWordToGuess = new List<char>(); // temporary list to hold each character of the word to guess

        public Input()
        {
            tempWordToGuess.AddRange(Globals.wordToGuess.ToCharArray()); // converts the word to guess to a char array and adds to the list
        }

        public bool CheckWord()
        {
            string userWord = string.Join("", Globals.inputList); // join the input into a string
            return Globals.wordDict.ContainsKey(userWord); // validate that word is in the list of valid words
        }

        public void ChangeColorGreen()  
        {
            if (Globals.wordToGuess == String.Join("", tempInput.ToArray())) // check if input is the word to guess
            {
                for (int i = 0; i < Globals.wordToGuess.Length; i++)
                {
                    GameBoard.boardArray[GameBoard.row, i].BackColor = Color.Green; // change all labels green and end game
                }
                MessageBox.Show("You won! The word was " + Globals.wordToGuess);
                Globals.gameOver = true;
            }
            else
            {
                for (int i = 0; i < Globals.wordToGuess.Length; i++)
                {
                    if (Globals.wordToGuess[i] == Globals.inputList[i]) // check each letter to see if they match
                    {
                        GameBoard.boardArray[GameBoard.row, i].BackColor = Color.Green;
                        tempWordToGuess[i] = ' '; // replace the letter in each temporary list
                        tempInput[i] = ' ';
                    }
                }
            }
        }

        public void ChangeColorYellow()
        {
            if (!Globals.gameOver)
            {
                for (int i = 0; i < Globals.wordToGuess.Length; i++)
                {
                    if (GameBoard.boardArray[GameBoard.row, i].BackColor != Color.Green && GameBoard.boardArray[GameBoard.row, i].BackColor != Color.Yellow) // check if the label has already been made green or yellow
                    {
                        if (tempWordToGuess.Contains(tempInput[i])) // check if the input contains a letter that is in the word but in the wrong position
                        {
                            GameBoard.boardArray[GameBoard.row, i].BackColor = Color.Yellow;
                            int index = tempWordToGuess.IndexOf(tempInput[i]); // get index of letter
                            tempWordToGuess[index] = ' '; // replace letter in each temporary list
                            tempInput[i] = ' ';
                        }
                    }
                }
            }
        }
        public void ChangeColorGray()
        {
            if (!Globals.gameOver) // check if game is already over
            {
                int i = 0;
                foreach (char c in tempInput) 
                {
                    if (c != ' ') // change labels to gray where letters were not replaced
                    {
                        GameBoard.boardArray[GameBoard.row, i].BackColor = Color.Gray;
                    }
                    i++;
                }
                if (Globals.attempts == 6) // check if user is out of guesses
                {
                    MessageBox.Show("You lose. The word was " + Globals.wordToGuess);
                }
            }
            
        }
    }


    public static class WordleGame
    {
        public static void ResetCounters()
        {
            foreach (var word in Globals.wordDict.Keys.ToList()) // loop through dictionary and set all values to 0
            {
                Globals.wordDict[word] = 0;
            }
        }
        public static void Reset() // reset all variables and clear board
        {
            Globals.gameOver = false;
            Globals.attempts = 0;
            GameBoard.row = 0;
            GameBoard.col = 0;
            foreach (Label label in GameBoard.boardArray)
            {
                label.Text = "";
                label.BackColor = Color.Empty;
            }
        }

        public static void Load() // call reset and generate new word
        {
            Reset();
            Globals.wordToGuess = Word.generateWord();
        }
    }

}
