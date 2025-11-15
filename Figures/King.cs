using System;
using System.Security.Cryptography;

namespace CourseWork;

public class King : Figure
{
    public King(char positionLetter, int positionNumber, bool isWhite) : base(positionLetter, positionNumber, isWhite) { Type = FigureType.King; }

    public override List<Move> GetPossibleMoves(ChessBoard board, bool includeAllies = false)
    {
        var moves = new List<Move>();
        int[] dx = { -1, 0, 1 };
        int[] dy = { -1, 0, 1 };

        foreach (var x in dx)
        {
            foreach (var y in dy)
            {
                if (x == 0 && y == 0) continue;
                char newLetter = (char)(PositionLetter + x);
                int newNumber = PositionNumber + y;
                if (newLetter >= 'a' && newLetter <= 'h' && newNumber >= 1 && newNumber <= 8)
                {
                    var target = board.GetFigureAt(newLetter, newNumber);
                    if (target == null)
                        moves.Add(new Move(this.Type, Position, (newLetter, newNumber)));
                    else if (target.IsWhite != IsWhite || includeAllies)
                        moves.Add(new Move(this.Type, Position, (newLetter, newNumber), target.Type));
                }
            }
        }
        int[] kdx = {0, 1, 2};
        int[] qdx = {0, -1, -2};
        ChessBoard boardCopy = new ChessBoard(board);
        switch (IsWhite)
        {
            case true:
                if (board.CanWhiteCastleKingside)
                {
                    foreach (var k in kdx)
                    {
                        var target = board.GetFigureAt((char)(PositionLetter + k), PositionNumber);
                        if (target != null && k!=0)
                            break;
                        boardCopy = new ChessBoard(board);
                        King kingCopy = new King(PositionLetter, PositionNumber, IsWhite);
                        char newLetter = (char)(PositionLetter + k);
                        kingCopy.Move(new Move(this.Type, Position, (newLetter, PositionNumber)), boardCopy);
                        if (kingCopy.IsChecking(boardCopy))
                            break;
                        if (k == 2)
                            moves.Add(new Move(this.Type, Position, (newLetter, PositionNumber), true));
                    }
                }
                if (board.CanWhiteCastleQueenside)
                {
                    foreach (var q in qdx)
                    {
                        var target = board.GetFigureAt((char)(PositionLetter + q), PositionNumber);
                        if (target != null && q != 0)
                            break;
                        boardCopy = new ChessBoard(board);
                        King kingCopy = new King(PositionLetter, PositionNumber, IsWhite);
                        char newLetter = (char)(PositionLetter + q);
                        kingCopy.Move(new Move(this.Type, Position, (newLetter, PositionNumber)), boardCopy);
                        if (kingCopy.IsChecking(boardCopy))
                            break;
                        if (q == -2)
                            moves.Add(new Move(this.Type, Position, (newLetter, PositionNumber), true));
                    }
                }
            break;
            case false:
            if (board.CanBlackCastleKingside)
            {
                foreach (var k in kdx)
                {
                    var target = board.GetFigureAt((char)(PositionLetter + k), PositionNumber);
                    if (target != null && k != 0)
                        break;
                    boardCopy = new ChessBoard(board);
                    King kingCopy = new King(PositionLetter, PositionNumber, IsWhite);
                    char newLetter = (char)(PositionLetter + k);
                    kingCopy.Move(new Move(this.Type, Position, (newLetter, PositionNumber)), boardCopy);
                    if (kingCopy.IsChecking(boardCopy))
                        break;
                    if (k == 2)
                        moves.Add(new Move(this.Type, Position, (newLetter, PositionNumber), true));
                }
            }
            if (board.CanBlackCastleQueenside)
            {
                foreach (var q in qdx)
                {
                    var target = board.GetFigureAt((char)(PositionLetter + q), PositionNumber);
                    if (target != null && q!=0)
                        break;
                    boardCopy = new ChessBoard(board);
                    King kingCopy = new King(PositionLetter, PositionNumber, IsWhite);
                    char newLetter = (char)(PositionLetter + q);
                    kingCopy.Move(new Move(this.Type, Position, (newLetter, PositionNumber)), boardCopy);
                    if (kingCopy.IsChecking(boardCopy))
                        break;
                    if (q == -2)
                        moves.Add(new Move(this.Type, Position, (newLetter, PositionNumber), true));
                }
            }
            break;
        }
        return moves;
    }

    public bool IsChecking(ChessBoard board)
    {
        bool isChecked = false;
        AttackingFigures = new List<Figure>();

        char checkingPosLetter;
        int checkingPosNumber;

        int[] dx = { -2, -1, 1, 2 };
        int[] dy = { -2, -1, 1, 2 };
        int dir = IsWhite ? 1 : -1;

        foreach (var x in dx)
        {
            foreach (var y in dy)
            {
                if (Math.Abs(x) != Math.Abs(y))
                {
                    checkingPosLetter = (char)(PositionLetter + x);
                    checkingPosNumber = PositionNumber + y;
                    if (checkingPosLetter >= 'a' && checkingPosLetter <= 'h' && checkingPosNumber >= 1 && checkingPosNumber <= 8)
                    {
                        var target = board.GetFigureAt(checkingPosLetter, checkingPosNumber);
                        if (target != null && target.Type == FigureType.Knight && target.IsWhite != this.IsWhite)
                        {
                            isChecked = true;
                            AttackingFigures.Add(target);
                        }
                    }
                }
            }
        }

        dx = new int[]{ -1, 1};
        foreach (var x in dx)
        {
            checkingPosLetter = (char)(PositionLetter + x);
            checkingPosNumber = PositionNumber + dir;
            if (checkingPosLetter >= 'a' && checkingPosLetter <= 'h' && checkingPosNumber >=1 && checkingPosNumber <= 8)
            {
                var target = board.GetFigureAt(checkingPosLetter, checkingPosNumber);
                if (target != null && target.Type == FigureType.Pawn && target.IsWhite != this.IsWhite)
                {
                    isChecked = true;
                    AttackingFigures.Add(target);
                }
            }
        }

        dx = new int[] { -1, 0, 1 };
        dy = new int[] { -1, 0, 1 };
        foreach (var x in dx)
        {
            foreach (var y in dy)
            {
                if (x == 0 && y == 0)
                    continue;
                checkingPosLetter = (char)(PositionLetter + x);
                checkingPosNumber = PositionNumber + y;
                if (checkingPosLetter >= 'a' && checkingPosLetter <= 'h' && checkingPosNumber >= 1 && checkingPosNumber <= 8)
                {
                    var target = board.GetFigureAt(checkingPosLetter, checkingPosNumber);
                    if (target != null && target.Type == FigureType.King && target.IsWhite!=this.IsWhite)
                        isChecked = true;
                }
            }
        }

        
        foreach(var x in dx)
        {
            foreach(var y in dy)
            {
                if (x == 0 && y == 0)
                    continue;
                bool isDiagonal = (x!=0&&y!=0);
                checkingPosLetter = (char)(PositionLetter + x);
                checkingPosNumber = PositionNumber + y;
                while (checkingPosLetter >= 'a' && checkingPosLetter <= 'h' && checkingPosNumber >= 1 && checkingPosNumber <= 8)
                {
                    var target = board.GetFigureAt(checkingPosLetter, checkingPosNumber);
                    if (target == null)
                    {
                        checkingPosLetter = (char)(checkingPosLetter+x);
                        checkingPosNumber = checkingPosNumber + y;
                        continue;
                    }
                    if (target.IsWhite == this.IsWhite)
                        break;
                    if (isDiagonal && (target.Type == FigureType.Bishop || target.Type == FigureType.Queen))
                    { 
                        isChecked = true;
                        AttackingFigures.Add(target);
                    }
                    if (!isDiagonal && (target.Type == FigureType.Rook || target.Type == FigureType.Queen))
                    {
                        isChecked = true;
                        AttackingFigures.Add(target);
                    }
                    break;
                }
            }
        }

        return isChecked;
    }
    public bool IsMated(ChessBoard board)
    {
        if (this.GetPossibleMoves(board).Count == 0 && this.IsChecking(board))
            return true;
        return false;
    }
}
