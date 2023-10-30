using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("+------------------------------------+");
        Console.WriteLine("| +-----+ ------------------         |");
        Console.WriteLine("| |A    | JUEGO: Blackjack 21        |");
        Console.WriteLine("| |  ♠  | LENGUAJE: C#               |");
        Console.WriteLine("| |    A| AUTOR: Brian Giraldo       |");
        Console.WriteLine("| +-----+ ------------------         |");
        Console.WriteLine("|         PROGRAMACIÓN Y MOTORES     |");
        Console.WriteLine("+------------------------------------+");
        Console.Write("Ingresa tu nombre: ");
        string playerName = Console.ReadLine();
        Console.Clear();
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

    public void DrawHandHorizontally()
    {
        foreach (Card card in Hand)
        {
            Console.Write("+-----+");
        }
        Console.WriteLine(); // Nueva línea para las cartas siguientes

        foreach (Card card in Hand)
        {
            Console.Write($"|{GetCardValueAsString(card),-4} |");
            //{GetCardValueAsString(card),-4}: Esta parte es un especificador de formato que controla
            //cómo se alinea la cadena dentro de un espacio de ancho fijo.
            //El número -4 indica que la cadena se ajustará a una longitud
            //de 4 caracteres y se alineará a la izquierda dentro de ese espacio.
            //Si la cadena tiene menos de 4 caracteres, se llenará con espacios en blanco
            //a la derecha para cumplir con la longitud de 4 caracteres.
        }
        Console.WriteLine(); // Nueva línea para las cartas siguientes

        foreach (Card card in Hand)
        {
            Console.Write("|  ");
            Console.Write($"{GetCardSuitSymbol(card.Suit)}");
            Console.Write("  |");
        }
        Console.WriteLine(); // Nueva línea para las cartas siguientes

        foreach (Card card in Hand)
        {
            Console.Write($"|   {GetCardValueAsString(card),-2}|");
        }
        Console.WriteLine(); // Nueva línea para las cartas siguientes

        foreach (Card card in Hand)
        {
            Console.Write("+-----+");
        }
        Console.WriteLine(); // Nueva línea al final
    }

    public string GetCardSuitSymbol(Suit suit)
    {
        switch (suit)
        {
            case Suit.Hearts:
                return "♥";
            case Suit.Diamonds:
                return "♦";
            case Suit.Clubs:
                return "♣";
            case Suit.Spades:
                return "♠";
            default:
                return "";
        }
    }

    public string GetCardValueAsString(Card card)
    {
        if (card.Value >= 2 && card.Value <= 10)
        {
            return card.Value.ToString();
        }
        else
        {
            switch (card.Value)
            {
                case 1:
                    return "A";
                case 11:
                    return "J";
                case 12:
                    return "Q";
                case 13:
                    return "K";
                default:
                    return ""; // Manejar otros valores si es necesario
            }
        }
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

    public void DrawHandUnkown()
    {
        // Dibujar la primera carta normalmente junto a la segunda
        Console.Write("+-----++-----+");
        Console.WriteLine();
        Console.Write($"|{GetCardValueAsString(Hand[0]),-4} ||     |");
        Console.WriteLine();
        Console.Write($"|  {GetCardSuitSymbol(Hand[0].Suit),-2} ||  ?  |");
        Console.WriteLine();
        Console.Write($"|   {GetCardValueAsString(Hand[0]),-2}||     |");
        Console.WriteLine();
        Console.Write("+-----++-----+");
        Console.WriteLine();
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

        while (true)
        {
            Cabecera();
            Console.WriteLine("-----------------");
            Console.WriteLine("MANO DEL CROUPIER:");
            Console.WriteLine("-----------------");
            dealer.DrawHandUnkown();
            Console.WriteLine();
            Console.WriteLine("-----------------");
            Console.WriteLine($"MANO DE {player.Name}:");
            Console.WriteLine("-----------------");
            player.DrawHandHorizontally();
            Console.WriteLine("-----------------");

            if (player.IsBusted())
            {
                Console.WriteLine("El jugador se ha pasado de 21. ¡El crupier gana!");
                return;
            }

            Console.Write("¿Quieres otra carta? (s/n): ");
            string response = Console.ReadLine();

            if (response.ToLower() == "s")
            {
                player.DrawCard(deck);
            }
            else if (response.ToLower() == "n")
            {
                dealer.PlayAsDealer(deck);

                if (player.IsBusted() || dealer.IsBusted())
                {
                    Console.Clear();
                    break;
                }
            }
            else
            {
                Console.WriteLine("Respuesta inválida. Por favor, ingrese 's' para sí y 'n' para no.");
            }
            Console.Clear();
        }

        // El crupier juega automáticamente
        Cabecera();
        Console.WriteLine("-----------------");
        Console.WriteLine("MANO DEL CROUPIER:");
        Console.WriteLine("-----------------");
        dealer.DrawHandHorizontally();
        Console.WriteLine("-----------------");
        Console.WriteLine($"MANO DE {player.Name}:");
        Console.WriteLine("-----------------");
        player.DrawHandHorizontally();
        Console.WriteLine("-----------------");
        Console.WriteLine($"Puntuación del croupier: {dealer.CalculateScore()}");
        Console.WriteLine($"Puntuación de {player.Name}: {dealer.CalculateScore()}");
        Console.WriteLine($"\n{DetermineWinner()}");
    }

    public void Cabecera()
    {
        Console.WriteLine("+------------------------------------+");
        Console.WriteLine("| +-----+ ------------------         |");
        Console.WriteLine("| |A    | JUEGO: Blackjack 21        |");
        Console.WriteLine("| |  ♠  | LENGUAJE: C#               |");
        Console.WriteLine("| |    A| AUTOR: Brian Giraldo       |");
        Console.WriteLine("| +-----+ ------------------         |");
        Console.WriteLine("|         PROGRAMACIÓN Y MOTORES     |");
        Console.WriteLine("+------------------------------------+");
    }
}