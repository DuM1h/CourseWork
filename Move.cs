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
    public Move(FigureType figureToMove, (char, int) from, (char, int) to, FigureType capturedFigure)
    {
        FigureToMove = figureToMove;
        From = from;
        To = to;
        CapturedFigure = capturedFigure;
        Score = 0;
    }

    public Move(FigureType figureToMove, (char, int) from, (char, int) to)
    {
        FigureToMove = figureToMove;
        From = from;
        To = to;
        CapturedFigure = FigureType.Null;
        Score = -10;
    }
}     
