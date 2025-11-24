using System;
using System.Xml.Serialization;

namespace CourseWork;
// 5rk1/1Q3pp1/6np/p2pB3/3P4/P1n5/2P3PP/4R1K1 w - a6 0 23
// До сортування - 105s
// Після сортування - 29s
// Після зміни Move з class на struct (+правки) - 14s
// Додав метод Unmove() та допрацював оцінку ходів - 6s
class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        int choice;
        do
        {
            Console.Clear();
            Console.WriteLine("Оберіть, що ви бажаєте:");
            Console.WriteLine("1. Грати");
            Console.WriteLine("2. Аналізувати позицію");
            Console.WriteLine("0. Вийти");
            if (!int.TryParse(Console.ReadLine(), out choice))
                choice = 0;
            switch (choice)
            {
                case 1:
                    Console.Clear();
                    Play(); break;
                case 2:
                    Console.Clear();
                    Analyze(); break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Невірний вибір"); 
                    Continue();
                    break;
            }
        } while (choice != 0);
    }

    static public void Continue()
    {
        Console.WriteLine("Для продовження натисніть Enter");
        Console.ReadLine();
    }

    static void Play()
    {
        int choice;
        do
        { 
            Console.Clear();
            Console.WriteLine("Оберіть сторону.");
            Console.WriteLine("1. Білі");
            Console.WriteLine("2. Чорні");
            Console.WriteLine("3. Випадковий вибір");
            Console.WriteLine("4. Грати двома сторонами");
            Console.WriteLine("5. Зіграти з позиції");
            Console.WriteLine("0. Повернутися в головне меню");
            if (!int.TryParse (Console.ReadLine(), out choice))
                choice = 0;
            switch (choice)
            {
                case 1:
                    GameSystem.PlayGame(true);
                    Continue(); break;
                case 2:
                    GameSystem.PlayGame(false);
                    Continue(); break;
                case 3:
                    Random rand = new Random();
                    bool isWhite = rand.Next(0, 2) == 0;
                    GameSystem.PlayGame(isWhite);
                    Continue(); break;
                case 4:
                    GameSystem.PlayGame();
                    Continue(); break;
                case 5:
                    Console.Clear();
                    Console.WriteLine("Введіть FEN-нотацію: ");
                    string fen = Console.ReadLine();
                    GameSystem.PlayGame(fen);
                    Continue(); break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Невірний вибір");
                    Continue(); break;
            }
        } while (choice!=0);
    }
    static void Analyze()
    {
        Console.WriteLine("Введіть FEN-нотацію: ");
        string fen = Console.ReadLine();
        ChessBoard chessBoard = new ChessBoard(fen);
        int choice;
        do
        {
            Console.Clear();
            Console.WriteLine("1. Показати шахівницю");
            Console.WriteLine("2. Прорахувати найкращий хід");
            Console.WriteLine("3. Відмінити останній хід");
            Console.WriteLine("4. Вивести FEN-нотацію");
            Console.WriteLine("5. Ввести нову FEN-нотацію");
            Console.WriteLine("0. Вийти");

            if (!int.TryParse(Console.ReadLine(), out choice))
                choice = 0;
            switch (choice)
            {
                case 1:
                    chessBoard.ShowBoard();
                    Continue(); break;
                case 2:
                    Console.WriteLine("Введіть глибину прорахунку (рекомендовано 3-4), не більше 6: ");
                    int depth;
                    while (!int.TryParse(Console.ReadLine(), out depth) || depth < 1 || depth > 6)
                    {
                        Console.WriteLine("Невірне значення. Спробуйте ще раз.");
                    }
                    CalculateSystem.CalculateBestMove(chessBoard, depth);
                    Continue(); break;
                case 3:
                    if (chessBoard.Unmove() == 0)
                        Console.WriteLine("Хід відмінено успішно.");
                    else
                        Console.WriteLine("Або хід вже відмінено, або ходу ще не було.");
                    Continue(); break;
                case 4:
                    chessBoard.PrintFen();
                    Continue(); break;
                case 5:
                    Console.WriteLine("Введіть FEN-нотацію: ");
                    fen = Console.ReadLine();
                    chessBoard = new ChessBoard(fen);
                    Continue(); break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Невірний вибір.");
                    Continue(); break;
            }
        } while (choice != 0);
    }
}