using System;
using System.ComponentModel;

namespace CourseWork;

public struct Move
{
    public FigureType FigureToMove { get; private set; }
    public (char, int) From { get; private set; }
    public (char, int) To { get; private set; }
    public FigureType CapturedFigure { get; private set; }
    public int Score { get; private set; }
    public Move(FigureType figureToMove, (char, int) from, (char, int) to, FigureType capturedFigure)
    {
        FigureToMove = figureToMove;
        From = from;
        To = to;
        CapturedFigure = capturedFigure;
        Score = 0;
        CalculateScore();
    }

    public Move(FigureType figureToMove, (char, int) from, (char, int) to)
    {
        FigureToMove = figureToMove;
        From = from;
        To = to;
        CapturedFigure = FigureType.Null;
        Score = 0;
        CalculateScore();
    }

    private void CalculateScore()
    {
        if (CapturedFigure != FigureType.Null)
            Score += Value(CapturedFigure) * 10 - Value(FigureToMove);
        else
            Score += 0;
    }

    private int Value(FigureType type)
    {
        switch (type)
        {
            case (FigureType.King): return 2; break;
            case (FigureType.Queen): return 9; break;
            case (FigureType.Rook): return 5; break;
            case (FigureType.Bishop): return 3; break;
            case (FigureType.Knight): return 3; break;
            case (FigureType.Pawn): return 1; break;
        }
        return 0;
    }
}     
