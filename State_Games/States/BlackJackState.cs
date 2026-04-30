using System;
public class BlackJack:State
{
    // attributes for the total
    private int playerTotal;
    private int dealerTotal;
    // create the deck. for this we can just generate random cards. 
    private Deck deck = new Deck();

    // play the game
    public void Play()
    {
        string choice;

        // setup
        playerTotal = deck.GenerateRandomCard() + deck.GenerateRandomCard();
        dealerTotal = deck.GenerateRandomCard() + deck.GenerateRandomCard();

        Console.WriteLine("Welcome to Blackjack!");
        Console.WriteLine("Your starting total: " + playerTotal);
        Console.WriteLine("Dealer shows: " + dealerTotal);

        // PLAYER TURN
        while (true)
        {
            Console.WriteLine("Do you want to hit or stand?");
            choice = Console.ReadLine().ToLower();

            if (choice == "hit")
            {
                int newCard = deck.GenerateRandomCard();
                playerTotal += newCard;

                Console.WriteLine("You drew: " + newCard);
                Console.WriteLine("Your total is now: " + playerTotal);

                if (playerTotal > 21)
                {
                    Console.WriteLine("You busted! You lose.");
                    return;
                }
            }
            else if (choice == "stand")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Type hit or stand.");
            }
        }

        // DEALER TURN
        Console.WriteLine("Dealer's turn...");

        while (dealerTotal < 17)
        {
            int newCard = deck.GenerateRandomCard();
            dealerTotal += newCard;

            Console.WriteLine("Dealer drew: " + newCard);
        }

        Console.WriteLine("Dealer total: " + dealerTotal);

        // DECIDE WINNER
        if (dealerTotal > 21 || playerTotal > dealerTotal)
        {
            Console.WriteLine("You won BlackJack! Moving on...");
            _context.TransitionToNext();
        }
        else if (playerTotal == dealerTotal)
        {
            Console.WriteLine("It's a tie!");
        }
        else
        {
            Console.WriteLine("Dealer wins. You lose.");
        }
    }
}

public class Card
{
    private int value;
    public int Value
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = value;
        }
    }
}

public class Deck
{
    private Random random = new Random();

    public int GenerateRandomCard()
    {
        return random.Next(2, 12);
    }
}