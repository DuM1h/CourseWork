using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CourseWork;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Введіть FEN-нотацію: ");
        string fen = Console.ReadLine();
        ChessBoard chessBoard = new ChessBoard(fen);
        string choice;
        do
        {
            Console.Clear();
            Console.WriteLine("1. Показати шахівницю");
            Console.WriteLine("2. Прорахувати найкращий хід");
            Console.WriteLine("3. Вивести FEN-нотацію");
            Console.WriteLine("0. Вийти");

            choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    chessBoard.ShowBoard();
                    Continue(); break;
                case "2":
                    Console.WriteLine("Введіть глибину прорахунку (рекомендовано 3-4): ");
                    int depth;
                    while(!int.TryParse(Console.ReadLine(), out depth) || depth < 1)
                    {
                        Console.WriteLine("Невірне значення. Спробуйте ще раз.");
                    }
                    CalculateSystem.CalculateBestMove(chessBoard, depth);
                    Continue(); break;
                case "3":
                    chessBoard.PrintFen();
                    Continue(); break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Невірний вибір.");
                    Continue(); break;
            }
        } while(choice!="0");
    }

    static void Continue()
    {
        Console.WriteLine("Для продовження натисніть Enter");
        Console.ReadLine();
    }
}