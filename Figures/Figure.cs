using System;
using System.Collections.Generic;

namespace CourseWork;

public abstract class Figure
{
    public char PositionLetter { get; protected set; }
    public int PositionNumber { get; protected set; }
    public (char, int) Position { get; protected set; }
    public bool IsWhite { get; protected set; }
    public int Value { get; protected set; }
    public FigureType Type { get; protected set; }
    public char Initial
    {
        get
        {
            switch (Type)
            {
                case (FigureType.King): return IsWhite ? 'K' : 'k'; break;
                case (FigureType.Queen): return IsWhite ? 'Q': 'q'; break;
                case (FigureType.Rook): return IsWhite ? 'R': 'r'; break;
                case (FigureType.Bishop): return IsWhite ? 'B': 'b' ; break;
                case (FigureType.Knight): return IsWhite ? 'N': 'n' ; break;
                case (FigureType.Pawn): return IsWhite ? 'P': 'p'; break;
            }
            return ' ';
        }
    }
    public List<Figure> AttackingFigures
    { get; protected set; }
    public List<Figure> ProtectingFigures
    { get; protected set; }
    public char Symbol
    {
        get
        {
            switch (Type)
            {
                case (FigureType.King): return IsWhite ? '♔' : '♚';
                case (FigureType.Queen): return IsWhite ? '♕' : '♛';
                case (FigureType.Rook): return IsWhite ? '♖' : '♜';
                case (FigureType.Bishop): return IsWhite ? '♗' : '♝';
                case (FigureType.Knight): return IsWhite ? '♘' : '♞';
                case (FigureType.Pawn): return IsWhite ? '♙' : '♟';
          }
            return ' ';
        }
    }
    public Figure() { }
    public Figure(char positionLetter, int positionNumber, bool isWhite, int value)
    {
        PositionLetter = positionLetter;
        PositionNumber = positionNumber;
        IsWhite = isWhite;
        Value = value;
        Position = (positionLetter, positionNumber);
    }
    public Figure(char positionLetter, int positionNumber, bool isWhite)
    {
        PositionLetter = positionLetter;
        PositionNumber = positionNumber;
        IsWhite = isWhite;
        Position = (positionLetter, positionNumber);
    }

    public abstract List<Move> GetPossibleMoves(ChessBoard board, bool includeAllies = false);
    public void Move(char toLetter,int toNumber, ChessBoard board)
    {
        int fromLetter = PositionLetter;
        int fromNumber = PositionNumber;
        board.SetFigureAt(this, fromLetter, fromNumber, toLetter, toNumber);
        PositionNumber = toNumber;
        PositionLetter = toLetter;
    }

    public void Unmove(ChessBoard board, Move move, Figure target)
    {
        PositionLetter = move.From.Item1;
        PositionNumber = move.From.Item2;
        board.Unmove(this, move, target);
    }

    public bool IsAttacking(ChessBoard chessBoard)
    {
        bool isAttacked = false;
        AttackingFigures = new List<Figure>();
        Figure[,] opponentsFigures;
        (char, int) pos = (this.PositionLetter, this.PositionNumber);
        if (this.IsWhite)
            opponentsFigures = chessBoard.GetBlackFigures();
        else
            opponentsFigures = chessBoard.GetWhiteFigures();
        foreach (Figure figure in opponentsFigures)
        {
            if (figure == null) continue;
            foreach (Move move in figure.GetPossibleMoves(chessBoard))
            {
                if (figure is Pawn)
                {
                    if (Math.Abs((char)(this.PositionLetter - figure.PositionLetter)) == 1)
                    {
                        if (move.To == pos)
                        {
                            isAttacked = true;
                            this.AttackingFigures.Add(figure);
                        }
                    }
                }
                else if (figure is King && this is King)
                {
                    continue;
                }
                else
                {
                    if (move.To == Position)
                    {
                        isAttacked = true;
                        this.AttackingFigures.Add(figure);
                    }
                }
            }
        }
        return isAttacked;
    }

    public bool IsProtected(ChessBoard chessBoard)
    {
        bool isProtected = false;
        ProtectingFigures = new List<Figure>();
        Figure[,] opponentsFigures;
        if (this.IsWhite)
            opponentsFigures = chessBoard.GetWhiteFigures();
        else
            opponentsFigures = chessBoard.GetBlackFigures();
        foreach (Figure figure in opponentsFigures)
        {
            if (figure == null) continue;
            foreach (var move in figure.GetPossibleMoves(chessBoard, true))
            {
                if (figure is Pawn)
                {
                    if (Math.Abs((char)(this.PositionLetter - figure.PositionLetter)) == 1)
                    {
                        if (move.To == Position)
                        {
                            isProtected = true;
                            this.ProtectingFigures.Add(figure);
                        }
                    }
                }
                else if (figure is King && this is King)
                {
                    continue;
                }
                else
                {
                    if (move.To == Position)
                    {
                        isProtected = true;
                        this.ProtectingFigures.Add(figure);
                    }
                }
            }
        }
        return isProtected;
    }
}
