using System;

namespace CourseWork;

static public class GameSystem
{
    const string basicFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    static private int depth = 0;
    static public void PlayGame()
    {
        ChessBoard board = new ChessBoard(basicFen);
        Turn(board, true, false);
    }
    static public void PlayGame(string fen)
    {
        string[] parts = fen.Split(' ');
        bool isWhitePlayer = parts[1] == "w";
        PlayGame(isWhitePlayer, fen);
    }
    static public void PlayGame(bool isWhitePlayer, string fen = basicFen)
    {
        Console.Clear();
        Console.WriteLine("Оберіть складність:");
        Console.WriteLine("1. Легка");
        Console.WriteLine("2. Середня");
        Console.WriteLine("3. Важка");
        int choice = int.Parse(Console.ReadLine() ?? "2");
        depth = choice switch
        {
            1 => 2,
            2 => 4,
            3 => 6,
            _ => 4
        };
        Console.Clear();
        ChessBoard board = new ChessBoard(fen);
        Turn(board, isWhitePlayer);
    }

    static public void Turn(ChessBoard board, bool isWhitePlayer, bool withBot=true)
    {
        while (true)
        {
            Console.Clear();
            board.ShowBoard();
            if (board.IsWhiteTurn == isWhitePlayer || !withBot)
            {
                var possibleMoves = CalculateSystem.GenerateLegalMoves(board);
                if (possibleMoves.Count == 0)
                {
                    King kingToMove = board.IsWhiteTurn ? board.GetWhiteKing() : board.GetBlackKing();
                    string color = !board.IsWhiteTurn ? "Білі" : "Чорні";
                    if (kingToMove.IsChecking(board))
                        Console.WriteLine(color + " перемогли");
                    else
                        Console.WriteLine("Нічия - пат");
                    break;
                }
                Console.WriteLine("Ваш хід. Введіть хід у форматі 'e2 e4':");
                string input = Console.ReadLine() ?? "";
                if (input.ToLower() == "exit" || input.ToLower() == "quit")
                    return;
                if (input.ToLower() == "fen" || input.ToLower() == "getfen")
                {
                    board.PrintFen();
                }
                string[] parts = input.Split(' ');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Невірний формат ходу. Спробуйте ще раз.");
                    Program.Continue();
                    continue;
                }
                (char fromLetter, int fromNumber) = (parts[0][0], int.Parse(parts[0][1].ToString()));
                (char toLetter, int toNumber) = (parts[1][0], int.Parse(parts[1][1].ToString()));
                if (fromLetter < 'a' || fromLetter > 'h' || toLetter < 'a' || toLetter > 'h' ||
                    fromNumber < 1 || fromNumber > 8 || toNumber < 1 || toNumber > 8)
                {
                    Console.WriteLine("Невірні координати. Спробуйте ще раз.");
                    Program.Continue();
                    continue;
                }
                var figure = board.GetFigureAt(fromLetter, fromNumber);
                var target = board.GetFigureAt(toLetter, toNumber);
                if (figure == null || (figure.IsWhite != board.IsWhiteTurn))
                {
                    Console.WriteLine("Невірний хід. Спробуйте ще раз.");
                    Program.Continue();
                    continue;
                }

                Move move;
                if (figure.Type == FigureType.Pawn && (toNumber == 1 || toNumber == 8))
                {
                    move = new Move(figure.Type, (fromLetter, fromNumber), (toLetter, toNumber), FigureType.Null, FigureType.Queen);
                    if (!possibleMoves.Contains(move))
                    {
                        Console.WriteLine("Невірний хід. Спробуйте ще раз.");
                        Program.Continue();
                        continue;
                    }
                    Console.WriteLine("Підтвердіть перетворення пішака (Q, R, B, N):");
                    string promotionChoice = Console.ReadLine() ?? "Q";
                    FigureType promotionType = promotionChoice.ToUpper() switch
                    {
                        "Q" => FigureType.Queen,
                        "R" => FigureType.Rook,
                        "B" => FigureType.Bishop,
                        "N" => FigureType.Knight,
                        _ => FigureType.Queen
                    };
                    if (target == null)
                        move = new Move(figure.Type, (fromLetter, fromNumber), (toLetter, toNumber), FigureType.Null, promotionType);
                    else
                        move = new Move(figure.Type, (fromLetter, fromNumber), (toLetter, toNumber), target.Type, promotionType);

                    figure.Move(move, board);
                    continue;
                }
                if (figure.Type == FigureType.Pawn && board.EnPassantAvailable && board.EnPassantTarget == (toLetter, toNumber))
                {
                    move = new Move(figure.Type, (fromLetter, fromNumber), (toLetter, toNumber), board.EnPassantTarget);
                    if (!possibleMoves.Contains(move))
                    {
                        Console.WriteLine("Невірний хід. Спробуйте ще раз.");
                        Program.Continue();
                        continue;
                    }
                    figure.Move(move, board);
                    Program.Continue();
                    continue;
                }
                if (figure.Type == FigureType.Pawn)
                {
                    if (target == null)
                        move = new Move(figure.Type, (fromLetter, fromNumber), (toLetter, toNumber));
                    else
                        move = new Move(figure.Type, (fromLetter, fromNumber), (toLetter, toNumber), target.Type);
                    if (!possibleMoves.Contains(move))
                    {
                        Console.WriteLine("Невірний хід. Спробуйте ще раз.");
                        Program.Continue();
                        continue;
                    }
                    figure.Move(move, board);
                    Program.Continue();
                    continue;
                }
                if (figure.Type == FigureType.King && Math.Abs(toLetter - fromLetter) == 2)
                {
                    move = new Move(figure.Type, (fromLetter, fromNumber), (toLetter, toNumber), true);
                    if (!possibleMoves.Contains(move))
                    {
                        Console.WriteLine("Невірний хід. Спробуйте ще раз.");
                        Program.Continue();
                        continue;
                    }
                    figure.Move(move, board);
                    Program.Continue();
                    continue;
                }
                if (target == null)
                    move = new Move(figure.Type, (fromLetter, fromNumber), (toLetter, toNumber));
                else
                    move = new Move(figure.Type, (fromLetter, fromNumber), (toLetter, toNumber), target.Type);
                if (!possibleMoves.Contains(move))
                {
                    Console.WriteLine("Невірний хід. Спробуйте ще раз.");
                    Program.Continue();
                    continue;
                }
                figure.Move(move, board);
            }
            else
            {
                if (!withBot)
                {
                    Console.WriteLine("Хід супротивника. Натисніть Enter, щоб продовжити...");
                    Console.ReadLine();
                    continue;
                }
                Console.WriteLine("Хід комп'ютера...");
                CalculateSystem.CalculateBestMove(board, depth, false);
                var otherMoves = CalculateSystem.GenerateLegalMoves(board);
                if (otherMoves.Count == 0)
                    break;
            }
            Program.Continue();
        }
    }
}
