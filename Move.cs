using System;

namespace CourseWork;

public class Move
{
    public Figure FigureToMove { get; private set; }
    public (char, int) From { get; private set; }
    public (char, int) To { get; private set; }
    public Figure CapturedFigure { get; private set; }
    public int Score { get; set; }
    public Move(Figure figureToMove, (char, int) from, (char, int) to, Figure capturedFigure = null)
    {
        FigureToMove = figureToMove;
        From = from;
        To = to;
        CapturedFigure = capturedFigure;
        Score = 0;
        CalculateScore();
    }

    private void CalculateScore()
    {
        if (CapturedFigure != null)
            Score += FigureToMove.Value*10 - CapturedFigure.Value;
        else
            Score += 0;
    }
}     
