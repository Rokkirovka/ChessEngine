using System;

namespace MyChess.Models;

[Flags]
public enum CastlingRights
{
    WhiteKingSide = 1,
    WhiteQueenSide = 2,
    BlackKingSide = 4,
    BlackQueenSide = 8
}