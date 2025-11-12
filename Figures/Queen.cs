using System;

namespace CourseWork;

public class Queen : Figure
{
    public Queen(char positionLetter, int positionNumber, bool isWhite, int value) : base(positionLetter, positionNumber, isWhite, value) { }

    public override List<(char, int)> GetPossibleMoves(ChessBoard board, bool includeAllies = false)
    {
        var moves = new List<(char, int)>();
        int[] dx = { -1, 0, 1 };
        int[] dy = { -1, 0, 1 };

        foreach (var x in dx)
        {
            foreach (var y in dy)
            {
                if ((Math.Abs(x) == Math.Abs(y) && x != 0) || (Math.Abs(x) != Math.Abs(y) && (x == 0 || y == 0)))
                {
                    char newLetter = (char)(PositionLetter + x);
                    int newNumber = PositionNumber + y;
                    while (newLetter >= 'a' && newLetter <= 'h' && newNumber >= 1 && newNumber <= 8)
                    {
                        var target = board.GetFigureAt(newLetter, newNumber);
                        if (target == null)
                        {
                            moves.Add((newLetter, newNumber));
                        }
                        else
                        {
                            if (target.IsWhite != IsWhite || includeAllies)
                            {
                                moves.Add((newLetter, newNumber));
                            }
                            break;
                        }
                        newLetter = (char)(newLetter + x);
                        newNumber = newNumber + y;
                    }
                }
            }
        }

        return moves;
    }
}
