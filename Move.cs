using System;
using System.ComponentModel;

namespace CourseWork;

public struct Move
{
    public FigureType FigureToMove { get; private set; }
    public (char, int) From { get; private set; }
    public (char, int) To { get; private set; }
    public int FromIndex => 8 * (8 - From.Item2) + ('h' - From.Item1);
    public int ToIndex => 8 * (8 - To.Item2) + ('h' - To.Item1);
    public FigureType CapturedFigure { get; private set; }
    public int Score { get; set; }

    public bool IsPromoting { get; private set; }
    public FigureType PromotingType { get; private set; }
    public bool IsCastling { get; private set; }
    public bool IsEnPassant { get; private set; }
    public (char, int) EnPassantTargetPos { get; private set; }
    public Move(FigureType figureToMove, (char, int) from, (char, int) to, FigureType capturedFigure)
    {
        FigureToMove = figureToMove;
        From = from;
        To = to;
        CapturedFigure = capturedFigure;
        Score = 0;
        IsEnPassant = false;
        IsCastling = false;
        IsPromoting = false;
    }

    public Move(FigureType figureToMove, (char, int) from, (char, int) to, bool isCastling = false)
    {
        FigureToMove = figureToMove;
        From = from;
        To = to;
        CapturedFigure = FigureType.Null;
        Score = -10;
        IsCastling = isCastling;
        IsEnPassant = false;
        IsPromoting = false;
    }

    public Move(FigureType figureToMove, (char, int) from, (char, int) to, FigureType capturedFigure, FigureType type)
    {
        FigureToMove = figureToMove;
        From = from;
        To = to;
        CapturedFigure = capturedFigure;
        Score = -10;
        IsCastling = false;
        IsEnPassant = false;
        IsPromoting = true;
        PromotingType = type;
    }

    public Move(FigureType figureToMove, (char, int) from, (char, int) to, (char, int) enPassantPos)
    {
        FigureToMove = figureToMove;
        From = from;
        To = to;
        CapturedFigure = FigureType.Null;
        Score = 0;
        IsEnPassant = true;
        IsCastling = false;
        EnPassantTargetPos = enPassantPos;
        IsPromoting = false;
    }

    public char GetSymbol(Figure fig)
    {
        switch (PromotingType)
        {
            case (FigureType.King): return fig.IsWhite ? '♔' : '♚';
            case (FigureType.Queen): return fig.IsWhite ? '♕' : '♛';
            case (FigureType.Rook): return fig.IsWhite ? '♖' : '♜';
            case (FigureType.Bishop): return fig.IsWhite ? '♗' : '♝';
            case (FigureType.Knight): return fig.IsWhite ? '♘' : '♞';
            case (FigureType.Pawn): return fig.IsWhite ? '♙' : '♟';
        }
        return ' ';
    }
}     
