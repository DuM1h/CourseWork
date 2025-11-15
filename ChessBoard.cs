using System;

namespace CourseWork;

public class ChessBoard
{
    public Figure[,] board = new Figure[8, 8];
    public string Fen { get; private set; }
    public bool IsWhiteTurn { get; private set; }
    public bool CanWhiteCastleKingside { get; private set; }
    public bool CanWhiteCastleQueenside { get; private set; }
    public bool CanBlackCastleKingside { get; private set; }
    public bool CanBlackCastleQueenside { get; private set; }
    public bool EnPassantAvailable { get; private set; }
    public (char, int) EnPassantTarget { get; private set; }
    public int HalfmoveClock { get; private set; }
    public int FullmoveNumber { get; private set; }

    private int previousHalfmoveClockValue;
    private bool previousEnPassantAvailable;
    private (char, int) previousEnPassantTarget;
    private int previousFullmoveNumber;
    private bool previousCanWhiteCastleKingside;
    private bool previousCanWhiteCastleQueenside;
    private bool previousCanBlackCastleKingside;
    private bool previousCanBlackCastleQueenside;
    private Figure enPassantCapturedPawn;


    public ChessBoard(string fen)
    {
        Fen = fen;
        InitializeBoard();
    }

    public void SetFigureAt(Figure figure, Move move)
    {
        int fromRow = 8 - move.From.Item2;
        int fromCol = move.From.Item1 - 'a';

        int toRow = 8 - move.To.Item2;
        int toCol = move.To.Item1 - 'a';

        board[toRow, toCol] = figure;
        board[fromRow, fromCol] = null;

        previousCanBlackCastleKingside = CanBlackCastleKingside;
        previousCanBlackCastleQueenside = CanBlackCastleQueenside;
        previousCanWhiteCastleKingside = CanWhiteCastleKingside;
        previousCanWhiteCastleQueenside = CanWhiteCastleQueenside;
        if (move.IsCastling)
        {
            if (figure.IsWhite)
            {
                if (move.From.Item1 == 'e' && move.From.Item2 == 1 && move.To.Item1 == 'g')
                {
                    board[7, 5] = board[7, 7];
                    board[7, 7] = null;
                }
                else if (move.From.Item1 == 'e' && move.From.Item2 == 1 && move.To.Item1 == 'c')
                {
                    board[7, 3] = board[7, 0];
                    board[7, 0] = null;

                }
                CanWhiteCastleKingside = false;
                CanWhiteCastleQueenside = false;
            }
            else
            {
                if (move.From.Item1 == 'e' && move.From.Item2 == 8 && move.To.Item1 == 'g')
                {
                    board[0, 5] = board[0, 7];
                    board[0, 7] = null;
                }
                else if (move.From.Item1 == 'e' && move.From.Item2 == 8 && move.To.Item1 == 'c')
                {
                    board[0, 3] = board[0, 0];
                    board[0, 0] = null;
                }
                CanBlackCastleKingside = false;
                CanBlackCastleQueenside = false;
            }
        }
        else if (figure is Rook)
        {
            if (figure.IsWhite)
            {
                if (move.From.Item1 == 'a' && move.From.Item2 == 1)
                    CanWhiteCastleQueenside = false;
                else if (move.From.Item1 == 'h' && move.From.Item2 == 1)
                    CanWhiteCastleKingside = false;
            }
            else
            {
                if (move.From.Item1 == 'a' && move.From.Item2 == 8)
                    CanBlackCastleQueenside = false;
                else if (move.From.Item1 == 'h' && move.From.Item2 == 8)
                    CanBlackCastleKingside = false;
            }
        }
        previousHalfmoveClockValue = HalfmoveClock;
        if (figure is Pawn || board[toRow, toCol] != null)
            HalfmoveClock = 0;
        else
            HalfmoveClock++;


        previousEnPassantAvailable = EnPassantAvailable;
        if (EnPassantAvailable)
        {
            previousEnPassantTarget = EnPassantTarget;
        }
        else
            previousEnPassantTarget = ('-',0);

        if (move.IsEnPassant)
        {
            int EnPassantLetter = move.EnPassantTargetPos.Item1 - '0';
            int EnPassantNumber = 8 - move.EnPassantTargetPos.Item2;
            enPassantCapturedPawn = board[EnPassantLetter, EnPassantNumber];
            board[EnPassantLetter, EnPassantNumber] = null;
        }

        if (figure is Pawn && Math.Abs(move.To.Item2 - move.From.Item2) == 2)
        {
            EnPassantAvailable = true;
            EnPassantTarget = (move.To.Item1, move.To.Item2 - 1);
        }
        else
        {
            EnPassantAvailable = false;
            EnPassantTarget = ('-', 0);
        }
        SwitchTurn();
        previousFullmoveNumber = FullmoveNumber;
        if (!IsWhiteTurn)
            FullmoveNumber++;
        UpdateFen();
    }

    public void Unmove(Figure figure, Move move, Figure target)
    {
        int fromRow = 8 - move.To.Item2;
        int fromCol = move.To.Item1 - 'a';

        int toRow = 8 - move.From.Item2;
        int toCol = move.From.Item1 - 'a';

        board[toRow, toCol] = figure;
        board[fromRow, fromCol] = target;

        if (move.IsEnPassant)
        {
            int EnPassantLetter = move.EnPassantTargetPos.Item1 - '0';
            int EnPassantNumber = 8 - move.EnPassantTargetPos.Item2;
            board[EnPassantLetter, EnPassantNumber] = enPassantCapturedPawn;
        }

        if (move.IsCastling)
        {
            if (figure.IsWhite)
            {
                if (move.From.Item1 == 'e' && move.From.Item2 == 1 && move.To.Item1 == 'g')
                {
                    board[7, 7] = board[7, 5];
                    board[7, 5] = null;                    
                }
                else if (move.From.Item1 == 'e' && move.From.Item2 == 1 && move.To.Item1 == 'c')
                {
                    board[7, 0] = board[7, 3];
                    board[7, 3] = null;

                }
            }
            else
            {
                if (move.From.Item1 == 'e' && move.From.Item2 == 8 && move.To.Item1 == 'g')
                {
                    board[0, 7] = board[0, 5];
                    board[0, 5] = null;
                }
                else if (move.From.Item1 == 'e' && move.From.Item2 == 8 && move.To.Item1 == 'c')
                {
                    board[0, 0] = board[0, 3];
                    board[0, 3] = null;
                }
            }
        }

        CanBlackCastleKingside = previousCanBlackCastleKingside;
        CanBlackCastleQueenside = previousCanBlackCastleQueenside;
        CanWhiteCastleKingside = previousCanWhiteCastleKingside;
        CanWhiteCastleQueenside = previousCanWhiteCastleQueenside;
        HalfmoveClock = previousHalfmoveClockValue;
        FullmoveNumber = previousFullmoveNumber;
        EnPassantAvailable = previousEnPassantAvailable;
        EnPassantTarget = previousEnPassantTarget;
        SwitchTurn();
        UpdateFen();
    }

    private void InitializeBoard()
    {
        // Ініціалізація шахівниці на основі FEN-нотації
        string[] fenParts;
        fenParts = Fen.Split(' ');
        string[] rows = fenParts[0].Split('/');

        if (fenParts[1] == "w")
            IsWhiteTurn = true;
        else
            IsWhiteTurn = false;

        if (fenParts[2].Contains("K"))
            CanWhiteCastleKingside = true;
        else
            CanWhiteCastleKingside = false;
        if (fenParts[2].Contains("Q"))
            CanWhiteCastleQueenside = true;
        else
            CanWhiteCastleQueenside = false;
        if (fenParts[2].Contains("k"))
            CanBlackCastleKingside = true;
        else
            CanBlackCastleKingside = false;
        if (fenParts[2].Contains("q"))
            CanBlackCastleQueenside = true;
        else
            CanBlackCastleQueenside = false;

        if (fenParts[3] != "-")
        {
            EnPassantAvailable = true;
            int num = fenParts[3][1]-'0';
            EnPassantTarget = (fenParts[3][0], num);
        }
        else
        {
            EnPassantAvailable = false;
            EnPassantTarget = ('-',0);
        }

        HalfmoveClock = int.Parse(fenParts[4]);
        FullmoveNumber = int.Parse(fenParts[5]);

        for (int i = 0; i < 8; i++)
        {
            int col = 0;
            for (int j = 0; j < rows[i].Length; j++)
            {
                char c = rows[i][j];

                if (char.IsDigit(c))
                {
                    int emptySquares = c - '0';
                    for (int k = 0; k < emptySquares; k++)
                        board[i, col++] = null;
                }
                else
                {
                    bool isWhite = char.IsUpper(c);
                    char lower = char.ToLower(c);

                    switch (lower)
                    {
                        case 'r': board[i, col] = new Rook((char)('a' + col), 8 - i, isWhite, 5); break;
                        case 'n': board[i, col] = new Knight((char)('a' + col), 8 - i, isWhite, 3); break;
                        case 'b': board[i, col] = new Bishop((char)('a' + col), 8 - i, isWhite, 3); break;
                        case 'q': board[i, col] = new Queen((char)('a' + col), 8 - i, isWhite, 9); break;
                        case 'k': board[i, col] = new King((char)('a' + col), 8 - i, isWhite); break;
                        case 'p': board[i, col] = new Pawn((char)('a' + col), 8 - i, isWhite, 1); break;
                    }
                    col++;
                }
            }
        }
    }

    public void ShowBoard()
    {
        for (int i = 0; i < 9; i++)
        {
            if (i != 8)
                Console.Write(8 - i + " ");         
            else
                Console.Write("  ");
            for (int j = 0; j < 8; j++)
            {
                if (i == 8)
                {
                    for (int k = 0; k < 8; k++)
                        Console.Write((char)('a' + k) + " ");
                    break;
                }
                if (board[i, j] != null)
                    Console.Write(board[i, j].Symbol + " ");
                else
                    Console.Write(". ");
            }
            Console.WriteLine();
        }
    }

    public Figure GetFigureAt(char letter, int number)
    {
        int row = 8 - number;
        int col = letter - 'a';
        return board[row, col];
    }

    public float CalсulateAdvantage()
    {
        return BoardEvaluator.CalсulateAdvantage(this);
    }

    public Figure[,] GetWhiteFigures()
    {
        Figure[,] opponentsFigures = new Figure[8,8];            
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null && board[i, j].IsWhite == true)
                {
                    opponentsFigures[i, j] = board[i, j];
                }
            }
        }
        return opponentsFigures;
    }

    public Figure[,] GetBlackFigures()
    {
        Figure[,] opponentsFigures = new Figure[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null && board[i, j].IsWhite == false)
                {
                    opponentsFigures[i, j] = board[i, j];
                }
            }
        }
        return opponentsFigures;
    }

    public King GetWhiteKing()
    {
        foreach (Figure f in board)
        {
            if (f != null && f.IsWhite && f is King)
                return (King)f;
        }
        return null;
    }

    public King GetBlackKing()
    {
        foreach (Figure f in board)
        {
            if (f != null && !f.IsWhite && f is King)
                return (King)f;
        }
        return null;
    }

    public ChessBoard(ChessBoard chessBoard)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var figure = chessBoard.board[i, j];
                if (figure == null)
                {
                    this.board[i, j] = null;
                }
                else
                {
                    if (figure is King)
                        this.board[i, j] = new King(figure.PositionLetter, figure.PositionNumber, figure.IsWhite);
                    else if (figure is Queen)
                        this.board[i, j] = new Queen(figure.PositionLetter, figure.PositionNumber, figure.IsWhite, figure.Value);
                    else if (figure is Rook)
                        this.board[i, j] = new Rook(figure.PositionLetter, figure.PositionNumber, figure.IsWhite, figure.Value);
                    else if (figure is Bishop)
                        this.board[i, j] = new Bishop(figure.PositionLetter, figure.PositionNumber, figure.IsWhite, figure.Value);
                    else if (figure is Knight)
                        this.board[i, j] = new Knight(figure.PositionLetter, figure.PositionNumber, figure.IsWhite, figure.Value);
                    else if (figure is Pawn)
                        this.board[i, j] = new Pawn(figure.PositionLetter, figure.PositionNumber, figure.IsWhite, figure.Value);
                }
            }
        }
        Fen = chessBoard.Fen;
        IsWhiteTurn = chessBoard.IsWhiteTurn;
        CanWhiteCastleKingside = chessBoard.CanWhiteCastleKingside;
        CanWhiteCastleQueenside = chessBoard.CanWhiteCastleQueenside;
        CanBlackCastleKingside = chessBoard.CanBlackCastleKingside;
        CanBlackCastleQueenside = chessBoard.CanBlackCastleQueenside;
        EnPassantAvailable = chessBoard.EnPassantAvailable;
        EnPassantTarget = chessBoard.EnPassantTarget;
        HalfmoveClock = chessBoard.HalfmoveClock;
        FullmoveNumber= chessBoard.FullmoveNumber;
    }
    private void UpdateFen()
    {
        string[] fenParts = new string[6];
        string[] rows = new string[8];

        if (IsWhiteTurn)
            fenParts[1] = "w";
        else
            fenParts[1] = "b";

        if (!CanBlackCastleKingside && !CanBlackCastleQueenside && !CanWhiteCastleKingside && !CanWhiteCastleQueenside)
            fenParts[2] = "-";
        else
        {
            if (CanWhiteCastleKingside)
                fenParts[2] += "K";
            if (CanWhiteCastleQueenside)
                fenParts[2] += "Q";
            if (CanBlackCastleKingside)
                fenParts[2] += "k";
            if (CanBlackCastleQueenside)
                fenParts[2] += "q";
        }

        if (EnPassantAvailable)
        {
            char letter = EnPassantTarget.Item1;
            char num = (char)(EnPassantTarget.Item2+'0');
            fenParts[3] = letter.ToString() + num.ToString();
        }
        else
            fenParts[3] = "-";

        fenParts[4] = HalfmoveClock.ToString();
        fenParts[5] = FullmoveNumber.ToString();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == null)
                {
                    int emptyCount = 0;
                    while (j < 8 && board[i, j] == null)
                    {
                        emptyCount++;
                        j++;
                    }
                    rows[i] += emptyCount.ToString();
                    j--;
                }
                else
                {
                    char symbol = board[i, j].Initial;
                    rows[i] += symbol;
                }
            }
        }
        fenParts[0] = string.Join("/", rows);
        Fen = string.Join(" ", fenParts);
    }

    private void SwitchTurn()
    {
        IsWhiteTurn = !IsWhiteTurn;
    }

    public void PrintFen()
    {
        Console.WriteLine("Fen-нотація для поточної позиції на дошці: "+Fen);
    }
}
