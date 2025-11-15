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

    public bool IsPromotion { get; private set; }
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
    }

    public Move(FigureType figureToMove, (char, int) from, (char, int) to, bool isCastling = false)
    {
        FigureToMove = figureToMove;
        From = from;
        To = to;
        CapturedFigure = FigureType.Null;
        Score = -10;
        if (figureToMove == FigureType.King)
        {
            IsPromotion = false;
            IsCastling = isCastling;
        }
        else if (figureToMove == FigureType.Pawn)
        {
            IsPromotion = isCastling;
            IsCastling = false;
        } else
        {
            IsPromotion = false;
            IsCastling = false;
        }
        IsEnPassant = false;
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
    }
}     
