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
                    CalculateBestMove(chessBoard);
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

    static void CalculateBestMove(ChessBoard board)
    {
        float startAdvantage = board.CalсulateAdvantage();
        float currentAdvantage;
        float bestResult=-1000;
        float result;
        Figure bestFigure = null;
        (char, int) from = ('a', 1);
        (char, int) to = ('a', 1);
        ChessBoard boardCopy;
        King blackKing = board.GetBlackKing();
        King whiteKing = board.GetWhiteKing();
        if (blackKing.IsMated(board))
        {
            Console.WriteLine("Білі перемогли");
            return;
        }
        if (whiteKing.IsMated(board))
        {
            Console.WriteLine("Чорні перемогли");
            return;
        }
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board.GetFigureAt((char)('h' - j), 8 - i) == null)
                    continue;
                var figure = board.GetFigureAt((char)('h' - j), 8 - i);
                if (figure.IsWhite == board.IsWhiteTurn)
                {
                    var possibleMoves = figure.GetPossibleMoves(board);
                    foreach (var move in possibleMoves)
                    {
                        boardCopy = new(board);
                        var figureCopy = boardCopy.GetFigureAt(figure.PositionLetter, figure.PositionNumber);
                        figureCopy.Move(move.Item1, move.Item2, boardCopy);
                        var blackKingCopy = boardCopy.GetBlackKing();
                        var whiteKingCopy = boardCopy.GetWhiteKing();
                        if (board.IsWhiteTurn)
                        {
                            if (blackKingCopy.IsMated(boardCopy))
                            {
                                bestFigure = figure;
                                from = (figure.PositionLetter, figure.PositionNumber);
                                to = move;
                                PrintResult(bestFigure, from, to);
                                return;
                            }
                            if (whiteKingCopy.IsChecking(boardCopy))
                                continue;
                        }
                        else if (!board.IsWhiteTurn)
                        {
                            if (whiteKingCopy.IsMated(boardCopy))
                            {
                                bestFigure = figure;
                                from = (figure.PositionLetter, figure.PositionNumber);
                                to = move;
                                PrintResult(bestFigure, from, to);
                                return;
                            }
                            if (blackKingCopy.IsChecking(boardCopy))
                                continue;
                        }
                        currentAdvantage = boardCopy.CalсulateAdvantage();
                        result = currentAdvantage - startAdvantage;
                        if (result > bestResult)
                        {
                            bestResult = result;
                            bestFigure = figure;
                            from = (figure.PositionLetter, figure.PositionNumber);
                            to = move;
                        }
                    }
                }
            }
        }
        PrintResult(bestFigure, from, to);

        Console.WriteLine("Оновити позицію?");
        Console.WriteLine("1. Так");
        string choice;
        choice = Console.ReadLine();
        if (choice == "1" && bestFigure != null)
        {
            var figureToMove = board.GetFigureAt(from.Item1, from.Item2);
            figureToMove.Move(to.Item1, to.Item2, board);
        }

    }
    static void PrintResult(Figure bestFigure, (char, int) from, (char, int) to)
    {
        Console.WriteLine("Найкращий хід:");
        if (bestFigure != null)
        {
            Console.WriteLine($"{bestFigure.Symbol} з {from.Item1}{from.Item2} на {to.Item1}{to.Item2}");
        }
        else
        {
            Console.WriteLine("Немає можливих ходів для покращення позиції.");
        }
    }

    static void Continue()
    {
        Console.WriteLine("Для продовження натисніть Enter");
        Console.ReadLine();
    }
}