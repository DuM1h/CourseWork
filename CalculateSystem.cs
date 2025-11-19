using System;

namespace CourseWork;

static public class CalculateSystem
{
    static public long nodesSearched;
    private const int MAX_DEPTH = 7;
    static private Move[,] killerMoves = new Move[MAX_DEPTH, 2];

    static private int[,] historyScore = new int[64, 64];
    static public void CalculateBestMove(ChessBoard board, int depth, bool confirm = true)
    {
        nodesSearched = 0;

        float bestResult = float.MinValue;
        float beta = float.MaxValue;
        Figure bestFigure = null;
        Move bestMove = new Move();

        King blackKing = board.GetBlackKing();
        King whiteKing = board.GetWhiteKing();

        List<Move> allMoves = GenerateLegalMoves(board);
        if (allMoves.Count == 0)
        {
            King kingToMove = board.IsWhiteTurn ? board.GetWhiteKing() : board.GetBlackKing();
            string color = !board.IsWhiteTurn ? "Білі" : "Чорні";
            if (kingToMove.IsChecking(board))
                Console.WriteLine(color+" перемогли");
            else
                Console.WriteLine("Нічия - пат");
        }

        ScoreMoves(allMoves, depth);
        allMoves.Sort((a, b) => b.Score.CompareTo(a.Score));

        ChessBoard boardCopy = new(board);
        foreach (var move in allMoves)
        {
            var figureCopy = boardCopy.GetFigureAt(move.From.Item1, move.From.Item2);
            var target = boardCopy.GetFigureAt(move.To.Item1, move.To.Item2);
            figureCopy.Move(move, boardCopy);
            float result = -FindBestScore(boardCopy, depth-1, -beta, -bestResult);
            figureCopy.Unmove(boardCopy, move, target);
            if (result > bestResult)
            {
                bestResult = result;
                bestFigure = board.GetFigureAt(move.From.Item1, move.From.Item2);
                bestMove = move;
            }
        }     
        PrintResult(bestFigure, bestMove, depth);
        string choice = "0";
        if (confirm)
        {
            Console.WriteLine("Оновити позицію?");
            Console.WriteLine("1. Так");
            choice = Console.ReadLine();
        }
        if ((choice == "1" && bestFigure != null)||!confirm)
        {
            var figureToMove = board.GetFigureAt(bestMove.From.Item1, bestMove.From.Item2);
            figureToMove.Move(bestMove, board);
        }
        for (int i = 0; i < historyScore.GetLength(0); i++)
        {
            for (int j = 0; j < historyScore.GetLength(1); j++)
            {
                historyScore[i, j] /= 2;
            }
        }
    }

    static void PrintResult(Figure bestFigure, Move move, int depth)
    {
        Console.WriteLine($"---");
        if (bestFigure != null)
        {
            Console.WriteLine("Найкращий хід:");
            Console.Write($"{bestFigure.Symbol} з {move.From.Item1}{move.From.Item2} на {move.To.Item1}{move.To.Item2}");
            if (move.IsPromoting)
            {
                Console.Write($" з перетворенням на {move.GetSymbol(bestFigure)}");
            }
        }
        else
        {
            Console.WriteLine("Немає можливих ходів для покращення позиції.");
        }
        Console.WriteLine($"\n---");
        Console.WriteLine($"Глибина пошуку: {depth}");
        Console.WriteLine($"Всього проаналізовано вузлів: {nodesSearched:N0}"); // :N0 для форматування (напр. 1,234,567)
        Console.WriteLine($"---");
    }

    static float FindBestScore(ChessBoard board, int depth, float alpha, float beta)
    {
        nodesSearched++;

        if (depth == 0)
        {
            return board.CalculateAdvantage();
        }

        List<Move> allMoves = GenerateLegalMoves(board);
        if (allMoves.Count == 0)
        {
            King kingToMove = board.IsWhiteTurn ? board.GetWhiteKing() : board.GetBlackKing();
            if (kingToMove.IsChecking(board))
                return float.MinValue;
            else
                return 0.0f;
        }
        
        ScoreMoves(allMoves, depth);
        allMoves.Sort((a, b) => b.Score.CompareTo(a.Score));

        ChessBoard boardCopy = new(board);
        foreach (var move in allMoves)
        {
            var figureCopy = boardCopy.GetFigureAt(move.From.Item1, move.From.Item2);
            var target = boardCopy.GetFigureAt(move.To.Item1, move.To.Item2);
            figureCopy.Move(move, boardCopy);
            float score = -FindBestScore(boardCopy, depth - 1, -beta, -alpha);
            figureCopy.Unmove(boardCopy, move, target);
            if (score >= beta)
            {
                if (move.CapturedFigure == FigureType.Null)
                {
                    StoreKillerMove(move, depth);
                    historyScore[move.FromIndex, move.ToIndex] += depth*depth;
                }
                return beta;
            }

            if (score > alpha)
                alpha = score;
        }
        return alpha;
    }

    static public List<Move> GenerateLegalMoves(ChessBoard board)
    {
        var moves = new List<Move>();   
        ChessBoard boardCopy = new(board);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var figure = board.GetFigureAt((char)('h' - j), 8 - i);
                if (figure == null || figure.IsWhite != board.IsWhiteTurn)
                    continue;

                var possibleMoves = figure.GetPossibleMoves(board);
                foreach (var move in possibleMoves)
                {
                    if (move.CapturedFigure == FigureType.King)
                        continue;
                    var figureCopy = boardCopy.GetFigureAt(move.From.Item1, move.From.Item2);
                    var target = boardCopy.GetFigureAt(move.To.Item1, move.To.Item2);
                    var kingCopy = board.IsWhiteTurn ? boardCopy.GetWhiteKing() : boardCopy.GetBlackKing();
                    figureCopy.Move(move, boardCopy);
                    if (!kingCopy.IsChecking(boardCopy))
                    {
                        moves.Add(move);
                    }
                    figureCopy.Unmove(boardCopy, move, target);
                }
            }
        }
        return moves;
    }

    static private int Value(FigureType type)
    {
        switch (type)
        {
            case (FigureType.King): return 2;
            case (FigureType.Queen): return 9;
            case (FigureType.Rook): return 5;
            case (FigureType.Bishop): return 3;
            case (FigureType.Knight): return 3;
            case (FigureType.Pawn): return 1;
        }
        return 0;
    }

    static private void ScoreMoves(List<Move> moves, int depth)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            Move move = moves[i];

            if (move.CapturedFigure != FigureType.Null)
            {
                move.Score =10000 + 10 * Value(move.CapturedFigure) - Value(move.FigureToMove);
            }
            else
            {
                if (move.Equals(killerMoves[depth, 0]))
                {
                    move.Score = 9000;
                }
                else if (move.Equals(killerMoves[depth, 1]))
                {
                    move.Score = 8000;
                }
                else
                {
                    move.Score = historyScore[move.FromIndex,move.ToIndex];
                }
            }
            moves[i] = move;
        }
    }

    static private void StoreKillerMove(Move move, int depth)
    {
        if (!killerMoves[depth, 0].Equals(move))
        {
            killerMoves[depth, 1] = killerMoves[depth, 0];
            killerMoves[depth, 0] = move;
        }
    }
}
