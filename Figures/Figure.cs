using System;
using System.Collections.Generic;

namespace CourseWork;

public abstract class Figure
{
    public char PositionLetter { get; protected set; }
    public int PositionNumber { get; protected set; }
    public bool IsWhite { get; protected set; }
    public int Value { get; protected set; }
    public char Initial
    {
        get
        {
            if (IsWhite)
            {
                if (this is King) return 'K';
                if (this is Queen) return 'Q';
                if (this is Rook) return 'R';
                if (this is Bishop) return 'B';
                if (this is Knight) return 'N';
                if (this is Pawn) return 'P';
            }
            else
            {
                if (this is King) return 'k';
                if (this is Queen) return 'q';
                if (this is Rook) return 'r';
                if (this is Bishop) return 'b';
                if (this is Knight) return 'n';
                if (this is Pawn) return 'p';
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
            if (this is King) return IsWhite ? '♔' : '♚';
            if (this is Queen) return IsWhite ? '♕' : '♛';
            if (this is Rook) return IsWhite ? '♖' : '♜';
            if (this is Bishop) return IsWhite ? '♗' : '♝';
            if (this is Knight) return IsWhite ? '♘' : '♞';
            if (this is Pawn) return IsWhite ? '♙' : '♟';
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
    }
    public Figure(char positionLetter, int positionNumber, bool isWhite)
    {
        PositionLetter = positionLetter;
        PositionNumber = positionNumber;
        IsWhite = isWhite;
    }

    public abstract List<(char, int)> GetPossibleMoves(ChessBoard board, bool includeAllies = false);
    public void Move(char toLetter,int toNumber, ChessBoard board)
    {
        int fromLetter = PositionLetter;
        int fromNumber = PositionNumber;
        board.SetFigureAt(this, fromLetter, fromNumber, toLetter, toNumber);
        PositionNumber = toNumber;
        PositionLetter = toLetter;
    }

    public bool IsAttacking(ChessBoard chessBoard)
    {
        bool isAttacked = false;
        AttackingFigures = new List<Figure>();
        Figure[,] opponentsFigures;
        if (this.IsWhite)
            opponentsFigures = chessBoard.GetBlackFigures();
        else
            opponentsFigures = chessBoard.GetWhiteFigures();
        foreach (Figure figure in opponentsFigures)
        {
            if (figure == null) continue;
            foreach (var (toLetter, toNumber) in figure.GetPossibleMoves(chessBoard))
            {
                if (figure is Pawn)
                {
                    if (Math.Abs((char)(this.PositionLetter - figure.PositionLetter)) == 1)
                    {
                        if ((toLetter == this.PositionLetter) && (toNumber == this.PositionNumber))
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
                    if ((toLetter == PositionLetter) && (toNumber == PositionNumber))
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
            foreach (var (toLetter, toNumber) in figure.GetPossibleMoves(chessBoard, true))
            {
                if (figure is Pawn)
                {
                    if (Math.Abs((char)(this.PositionLetter - figure.PositionLetter)) == 1)
                    {
                        if ((toLetter == this.PositionLetter) && (toNumber == this.PositionNumber))
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
                    if ((toLetter == PositionLetter) && (toNumber == PositionNumber))
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
