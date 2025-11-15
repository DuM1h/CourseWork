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
    public void Move(Move move, ChessBoard board)
    {
        int fromLetter = PositionLetter;
        int fromNumber = PositionNumber;
        board.SetFigureAt(this, move);
        PositionNumber = move.To.Item2;
        PositionLetter = move.To.Item1;
    }

    public void Unmove(ChessBoard board, Move move, Figure target)
    {
        PositionLetter = move.From.Item1;
        PositionNumber = move.From.Item2;
        board.Unmove(this, move, target);
    }
    public void FindAttackers(ChessBoard board)
    {
        AttackingFigures = new List<Figure>();
        ProtectingFigures = new List<Figure>();
        
        bool isAttacked = false;
        bool isProtected = false;

        char attackingPosLetter;
        int attackingPosNumber;

        int[] dx = { -2, -1, 1, 2 };
        int[] dy = { -2, -1, 1, 2 };
        int dir = IsWhite ? 1 : -1;

        foreach (var x in dx)
        {
            foreach (var y in dy)
            {
                if (Math.Abs(x) != Math.Abs(y))
                {
                    attackingPosLetter = (char)(PositionLetter + x);
                    attackingPosNumber = PositionNumber + y;
                    if (attackingPosLetter >= 'a' && attackingPosLetter <= 'h' && attackingPosNumber >= 1 && attackingPosNumber <= 8)
                    {
                        var target = board.GetFigureAt(attackingPosLetter, attackingPosNumber);
                        if (target != null && target.Type == FigureType.Knight)
                        {
                            if (target.IsWhite != this.IsWhite)
                            {
                                isAttacked = true;
                                AttackingFigures.Add(target);
                            } else
                            {
                                isProtected = true;
                                ProtectingFigures.Add(target);
                            }
                        }
                    }
                }
            }
        }

        dx = new int[] { -1, 1 };
        foreach (var x in dx)
        {
            attackingPosLetter = (char)(PositionLetter + x);
            attackingPosNumber = PositionNumber + dir;
            if (attackingPosLetter >= 'a' && attackingPosLetter <= 'h' && attackingPosNumber >= 1 && attackingPosNumber <= 8)
            {
                var target = board.GetFigureAt(attackingPosLetter, attackingPosNumber);
                if (target != null && target.Type == FigureType.Pawn)
                {
                    if (target.IsWhite != this.IsWhite)
                    {
                        isAttacked = true;
                        AttackingFigures.Add(target);
                    } else
                    {
                        isProtected = true;
                        ProtectingFigures.Add(target);
                    }
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
                attackingPosLetter = (char)(PositionLetter + x);
                attackingPosNumber = PositionNumber + y;
                if (attackingPosLetter >= 'a' && attackingPosLetter <= 'h' && attackingPosNumber >= 1 && attackingPosNumber <= 8)
                {
                    var target = board.GetFigureAt(attackingPosLetter, attackingPosNumber);
                    if (target != null && target.Type == FigureType.King)
                    {
                        if (target.IsWhite != this.IsWhite)
                        {
                            isAttacked = true;
                            AttackingFigures.Add(target);
                        } else
                        {
                            isProtected = true;
                            ProtectingFigures.Add(target);
                        }
                    }
                }
            }
        }


        foreach (var x in dx)
        {
            foreach (var y in dy)
            {
                if (x == 0 && y == 0)
                    continue;
                bool isDiagonal = (x != 0 && y != 0);
                attackingPosLetter = (char)(PositionLetter + x);
                attackingPosNumber = PositionNumber + y;
                while (attackingPosLetter >= 'a' && attackingPosLetter <= 'h' && attackingPosNumber >= 1 && attackingPosNumber <= 8)
                {
                    var target = board.GetFigureAt(attackingPosLetter, attackingPosNumber);
                    if (target == null)
                    {
                        attackingPosLetter = (char)(attackingPosLetter + x);
                        attackingPosNumber = attackingPosNumber + y;
                        continue;
                    }
                    if (isDiagonal && (target.Type == FigureType.Bishop || target.Type == FigureType.Queen))
                    {
                        if (target.IsWhite != this.IsWhite)
                        {
                            isAttacked = true;
                            AttackingFigures.Add(target);
                        } else
                        {
                            isProtected = true;
                            ProtectingFigures.Add(target);
                        }
                    }
                    if (!isDiagonal && (target.Type == FigureType.Rook || target.Type == FigureType.Queen))
                    {
                        if (target.IsWhite != this.IsWhite)
                        {
                            isAttacked = true;
                            AttackingFigures.Add(target);
                        } else
                        {
                            isProtected = true;
                            ProtectingFigures.Add(target);
                        }
                    }
                    break;
                }
            }
        }
    }
}
