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
    public char[] EnPassantTarget { get; private set; }
    public int HalfmoveClock { get; private set; }
    public int FullmoveNumber { get; private set; }

    private int previousHalfmoveClockValue;
    private bool previousEnPassantAvailable;
    private char[] previousEnPassantTarget = new char[2];
    private int previousFullmoveNumber;


    public ChessBoard(string fen)
    {
        Fen = fen;
        InitializeBoard();
    }

    public void SetFigureAt(Figure figure, int fromLetter, int fromNumber, char toLetter, int toNumber)
    {
        int fromRow = 8 - fromNumber;
        int fromCol = fromLetter - 'a';

        int toRow = 8 - toNumber;
        int toCol = toLetter - 'a';

        board[toRow, toCol] = figure;
        board[fromRow, fromCol] = null;

        if (figure is King)
        {
            if (figure.IsWhite)
            {
                if (fromLetter == 'e' && fromNumber == 1 && toLetter == 'g')
                {
                    board[7, 5] = board[7, 7];
                    board[7, 7] = null;
                }
                else if (fromLetter == 'e' && fromNumber == 1 && toLetter == 'c')
                {
                    board[7, 3] = board[7, 0];
                    board[7, 0] = null;

                }
                CanWhiteCastleKingside = false;
                CanWhiteCastleQueenside = false;
            }
            else
            {
                if (fromLetter == 'e' && fromNumber == 8 && toLetter == 'g')
                {
                    board[0, 5] = board[0, 7];
                    board[0, 7] = null;
                }
                else if (fromLetter == 'e' && fromNumber == 8 && toLetter == 'c')
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
                if (fromLetter == 'a' && fromNumber == 1)
                    CanWhiteCastleQueenside = false;
                else if (fromLetter == 'h' && fromNumber == 1)
                    CanWhiteCastleKingside = false;
            }
            else
            {
                if (fromLetter == 'a' && fromNumber == 8)
                    CanBlackCastleQueenside = false;
                else if (fromLetter == 'h' && fromNumber == 8)
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
            previousEnPassantTarget[0] = EnPassantTarget[0];
            previousEnPassantTarget[1] = EnPassantTarget[1];
        }
        else
            previousEnPassantTarget = null;

        if (figure is Pawn && Math.Abs(toNumber - fromNumber) == 2)
        {
            EnPassantAvailable = true;
            EnPassantTarget = new char[] { toLetter, (char)((fromNumber + toNumber) / 2 + '0') };
        }
        else
        {
            EnPassantAvailable = false;
            EnPassantTarget = null;
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

        char[] EnPassantTargetFile = new char[2];

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
            EnPassantTargetFile[0] = fenParts[3][0];
            EnPassantTargetFile[1] = fenParts[3][1];
            EnPassantTarget = EnPassantTargetFile;
        }
        else
        {
            EnPassantAvailable = false;
            EnPassantTargetFile = null;
            EnPassantTarget = EnPassantTargetFile;
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
            fenParts[3] = new string(EnPassantTarget);
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
