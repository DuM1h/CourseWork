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
            Console.WriteLine("1. Показати шахівницю");
            Console.WriteLine("2. Прорахувати найкращий хід");
            Console.WriteLine("3. Вийти");

            choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    chessBoard.ShowBoard();
                    break;
                case "2":
                    CalculateBestMove(chessBoard);
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Невірний вибір.");
                    break;
            }
        } while(choice!="3");
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
}