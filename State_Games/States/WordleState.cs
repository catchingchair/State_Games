public class WordleState : State
{
    public override void Play()
    {
        setupGame();
    }

    //Micah Code
    string[] words = { "apple", "grape", "crane", "slate", "birth", "birds", "canes", "mines", "micah", "trent", "isaac" };
    // just setting a random so that it's cleaner in setupGame
    Random random = new Random();
    //maxGuesses nuff said
    int maxGuesses = 6;
    //what will be called by the state to start the wordle game
    public void setupGame()
    {
        Console.WriteLine("Welcome To Wordle!\nYou have 6 try's to guess the word!\nGoodluck!\nGreen = Correct letter in correct spot\nBlue = correct letter in wrong spot");
        string wordToGuess = words[random.Next(words.Length)];
        startGame(wordToGuess);
    }

    //starts the game 
    void startGame(string wordToGuess)
    {

        for (int i = 0; i < maxGuesses; i++)
        {
            //get the players guess and display which guess it is
            Console.WriteLine($"Guess {i + 1}/6");
            string playerGuess = Console.ReadLine()!.ToLower();

            //fail safe to make sure the player doesn't lose a guess because of a mistype
            if (playerGuess.Length != 5)
            {
                Console.WriteLine("Not a valid guess, must be 5 letters exactly.");
                i--;
                continue;
            }
            //they win
            //state change will go here
            if (playerGuess == wordToGuess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(wordToGuess);
                Console.WriteLine("You Got it!");
                Console.ForegroundColor = ConsoleColor.White;
//transition here
                AskToRestart();
                break;
            }
            checkGuess(playerGuess, wordToGuess);
        }
        Console.WriteLine("Nice Try! But you didn't get it. The wordle was " + wordToGuess);
        AskToRestart();
    }
    //check which letters the player got right
    void checkGuess(string guess, string wordToGuess)
    {
        //to avoid getting double letters
        //having a list of bools to see if the letter has been used up and can't make the letters be the wrong color
        //for example if the word is apple and someone types poppy (horrible guess btw)
        //if it was just checking for letters it would make the 2nd p green and the two others blue 
        //with the bool it makes it to where the first p is blue and the second is gray cause the first p is already using the blue if that makes sense

        //also storing the color of the letter so that at the end we can apply it.
        bool[] used = new bool[5];
        ConsoleColor[] colors = new ConsoleColor[5];

        //get all the correct letters first 
        for (int i = 0; i < 5; i++)
        {
            if (guess[i] == wordToGuess[i])
            {
                colors[i] = ConsoleColor.Green;
                used[i] = true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        //now get the correct letters that are in the wrong spots (blue because i'm colorblind and yellow is too close to green)
        for (int i = 0; i < 5; i++)
        {
            if (colors[i] == ConsoleColor.Green) continue;

            for (int j = 0; j < 5; j++)
            {
                if (!used[j] && guess[i] == wordToGuess[j])
                {
                    colors[i] = ConsoleColor.Blue;
                    used[j] = true;
                    break;
                }
            }

        }
        //Write out the players guess
        for (int i = 0; i < 5; i++)
        {
            Console.ForegroundColor = colors[i];
            Console.Write(guess[i]);
            Console.ResetColor();
        }

        Console.WriteLine();
    }
    //restart the game
    void Restart()
    {
        Console.WriteLine("Restarting Wordle... Goodluck!");
        setupGame();
    }
    //win or lose, they come here to see if they want to play again
    void AskToRestart()
    {
        Console.WriteLine("Want to try again?\n     [Y] Play again    [N] Quit");
        string playerAnswer = Console.ReadLine()!.ToLower();
        if (playerAnswer == "y")
        {
            Restart();
        }
        else if (playerAnswer == "n")
        {
            _context.TransitionToNext();
        }
        else
        {
            Console.WriteLine("Input 'Y' or 'N' (Yes or No)");
            AskToRestart();
        }
    }

}