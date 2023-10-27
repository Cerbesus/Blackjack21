using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Ingresa tu nombre: ");
        string playerName = Console.ReadLine();

        BlackjackGame game = new BlackjackGame(playerName);
        game.PlayGame();
    }
}


public enum Suit //Enumerador para los palos de las cartas
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public class Card //Clase para representar una carta con su palo y valor
{
    public Suit Suit { get; }
    public int Value { get; }

    public Card(Suit suit, int value) //Constructor de clase
    {
        Suit = suit;
        Value = value;
    }
}

public class Deck
{
    private List<Card> cards;
    private Random random;

    public Deck()
    {
        cards = new List<Card>();
        random = new Random();

        foreach (Suit suit in Enum.GetValues(typeof(Suit))) //Se crea la baraja en la lista "cards"
        {
            for (int value = 1; value <= 13; value++)
            {
                cards.Add(new Card(suit, value));
            }
        }
    }

    public void Shuffle() //Método que mezcla la baraja
    {
        int n = cards.Count; 
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            Card card = cards[k];
            cards[k] = cards[n];
            cards[n] = card;
        }
    }

    public Card DrawCard()
    {
        if (cards.Count > 0)
        {
            Card card = cards[0]; //Coge la primera carta de la baraja
            cards.RemoveAt(0); //La elimina de la lista
            return card;
        }
        else
        {
            throw new InvalidOperationException("No hay cartas en la baraja"); //En caso de no haber más cartas maneja el error
        }
    }
}

public class Player
{
    public string Name { get; }
    public List<Card> Hand { get; }

    public Player(string name)
    {
        Name = name;
        Hand = new List<Card>();
    }

    public void DrawCard(Deck deck) //Llama al método de Deck para coger una carta
    {
        Card card = deck.DrawCard();
        Hand.Add(card); //Añade la carta a su baraja
    }

    public void ShowHand() //Muestra las cartas que tiene
    {
        Console.WriteLine($"{Name}'s Hand:");
        foreach (Card card in Hand)
        {
            Console.WriteLine($"Suit: {card.Suit}, Value: {card.Value}");
        }
    }
}

public class BlackjackPlayer : Player
{
    public BlackjackPlayer(string name) : base(name) { } //base(name) = Llamada al constructor de la clase base Player 

    public int CalculateScore()
    {
        int score = 0;
        int aceCount = 0;

        foreach (Card card in Hand)
        {
            score += Math.Min(card.Value, 10); //Los ases valen 1 u 11, otras cartas valen su valor numérico
                                               //Y las letras valen 10, por eso aunque salga una letra elegirá un 10 como valor
            if (card.Value == 1) //As
            {
                aceCount++;
            }
        }

        return score;
    }

    public bool HasBlackjack() //Si con la primera mano tiene 21 puntos es blackjack
    {
        return Hand.Count == 2 && CalculateScore() == 21;
    }

    public bool IsBusted() //Si tiene más de 21 puntos pierde
    {
        return CalculateScore() > 21;
    }
}

public class BlackjackDealer : BlackjackPlayer
{
    public BlackjackDealer() : base("Crupier") { }

    public void PlayAsDealer(Deck deck)
    {
        while (CalculateScore() < 17)
        {
            DrawCard(deck);
        }
    }
}

public class BlackjackGame
{
    private BlackjackPlayer player;
    private BlackjackDealer dealer;
    private Deck deck;

    public BlackjackGame(string playerName)
    {
        player = new BlackjackPlayer(playerName);
        dealer = new BlackjackDealer();
        deck = new Deck();
        deck.Shuffle();
    }

    public int GetDistanceTo21(BlackjackPlayer player)
    {
        return Math.Abs(player.CalculateScore() - 21);
    }

    public void PrintGameStatus()
    {
        Console.WriteLine("\nEstado del juego:");
        Console.WriteLine($"Puntuación del jugador: {player.CalculateScore()}");
        Console.WriteLine($"Puntuación del crupier: {dealer.CalculateScore()}");
    }

    public string DetermineWinner()
    {
        if (player.IsBusted() && dealer.IsBusted())
        {
            return "Ambos jugadores se han pasado de 21. Es un empate.";
        }
        else if (player.IsBusted())
        {
            return "El jugador se ha pasado de 21. El crupier gana.";
        }
        else if (dealer.IsBusted())
        {
            return "El crupier se ha pasado de 21. El jugador gana.";
        }
        else if (player.CalculateScore() == dealer.CalculateScore())
        {
            return "Es un empate. Nadie gana.";
        }
        else if (GetDistanceTo21(player) < GetDistanceTo21(dealer))
        {
            return "¡Felicidades! ¡El jugador gana!";
        }
        else
        {
            return "El crupier gana. Mejor suerte la próxima vez.";
        }
    }

    public void PlayGame()
    {
        player.DrawCard(deck);
        dealer.DrawCard(deck);
        player.DrawCard(deck);
        dealer.DrawCard(deck);

        Console.WriteLine("¡Bienvenido al Blackjack!");
        while (true)
        {
            Console.WriteLine("\nTus cartas:");
            player.ShowHand();
            Console.WriteLine($"Puntuación: {player.CalculateScore()}");
            PrintGameStatus();

            if (player.IsBusted())
            {
                Console.WriteLine("El jugador se ha pasado de 21. ¡El crupier gana!");
                return;
            }

            Console.Write("\n¿Quieres otra carta? (s/n): ");
            string response = Console.ReadLine();

            if (response.ToLower() == "s")
            {
                player.DrawCard(deck);
            }
            else if (response.ToLower() == "n")
            {
                break;
            }
            else
            {
                Console.WriteLine("Respuesta inválida. Por favor, ingrese 's' para sí y 'n' para no.");
            }
        }

        // El crupier juega automáticamente
        dealer.PlayAsDealer(deck);

        Console.WriteLine("\nCartas del crupier:");
        dealer.ShowHand();
        Console.WriteLine($"Puntuación del crupier: {dealer.CalculateScore()}");

        Console.WriteLine(DetermineWinner());
    }
}


