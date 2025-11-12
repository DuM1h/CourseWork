using System;

namespace CourseWork
{
    static public class CalculateSystem
    {
        static public void CalculateBestMove(ChessBoard board, int depth)
        {
            float bestResult = float.MinValue;
            Figure bestFigure = null;
            (char, int) from = ('a', 1);
            (char, int) to = ('a', 1);

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
                    var figure = board.GetFigureAt((char)('h' - j), 8 - i);
                    if (figure == null)
                        continue;
                    if (figure.IsWhite != board.IsWhiteTurn)
                        continue;
                    var possibleMoves = figure.GetPossibleMoves(board);
                    foreach (var move in possibleMoves)
                    {
                        ChessBoard boardCopy = new(board);
                        var figureCopy = boardCopy.GetFigureAt(figure.PositionLetter, figure.PositionNumber);
                        var kingCopy = board.IsWhiteTurn ? boardCopy.GetWhiteKing() : boardCopy.GetBlackKing();
                        figureCopy.Move(move.Item1, move.Item2, boardCopy);
                        if (kingCopy.IsChecking(boardCopy))
                            continue;

                        float result = -FindBestScore(boardCopy, depth-1);
                        if (result > bestResult)
                        {
                            bestResult = result;
                            bestFigure = board.GetFigureAt((char)('h' - j), 8 - i);
                            from = (bestFigure.PositionLetter, bestFigure.PositionNumber);
                            to = move;
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

        static float FindBestScore(ChessBoard board, int depth)
        {
            if (depth == 0)
            {
                return board.CalсulateAdvantage();
            }
            King blackKing = board.GetBlackKing();
            King whiteKing = board.GetWhiteKing();

            if (!board.IsWhiteTurn && blackKing.IsMated(board))
            {
                return float.MinValue;
            }
            if (board.IsWhiteTurn && whiteKing.IsMated(board))
            {
                return float.MinValue;
            }

            float bestScore = float.MinValue;
            
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var figure = board.GetFigureAt((char)('h' - j), 8 - i);
                    if (figure == null)
                        continue;
                    if (figure.IsWhite != board.IsWhiteTurn)
                        continue;
                    var possibleMoves = figure.GetPossibleMoves(board);
                    foreach (var move in possibleMoves)
                    {
                        ChessBoard boardCopy = new(board);
                        var figureCopy = boardCopy.GetFigureAt(figure.PositionLetter, figure.PositionNumber);
                        var kingCopy = board.IsWhiteTurn ? boardCopy.GetWhiteKing() : boardCopy.GetBlackKing();
                        figureCopy.Move(move.Item1, move.Item2, boardCopy);

                        if (kingCopy.IsChecking(boardCopy))
                            continue;
                        float score = -FindBestScore(boardCopy, depth - 1);
                        
                        if (score > bestScore)
                        {
                            bestScore = score;
                        }
                        
                    }
                }
            }
            return bestScore;
        }
    }
}
