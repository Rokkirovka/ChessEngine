using MyChess.Core;
using MyChess.Models.Pieces;

namespace MyChessEngine.Utils;

public static class PieceSquareTables
{
    public const int DoublePawnPenalty = -20;
    public const int IsolatedPawnPenalty = -20;
    public const int OpenFileScore = 20;
    public const int SemiOpenFileScore = 10;
    public const int KingShieldBonus = 5;
    public const int MobilityBonus = 1;

    public static readonly int[] PassedPawnBonus = [0, 10, 30, 50, 75, 100, 150, 200];

    static PieceSquareTables()
    {
        for (var i = 0; i < 64; i++)
        {
            IsolatedMasks[i] = new BitBoard(0UL);
            WhitePassedMasks[i] = new BitBoard(0UL);
            BlackPassedMasks[i] = new BitBoard(0UL);
            FileMasks[i] = new BitBoard(0UL);
            RankMasks[i] = new BitBoard(0UL);
        }

        for (var rank = 0; rank < 8; rank++)
        for (var file = 0; file < 8; file++)
        {
            RankMasks[rank * 8 + file] = GetFileAndRankMask(-1, rank);
            FileMasks[rank * 8 + file] = GetFileAndRankMask(file, -1);
        }

        for (var rank = 0; rank < 8; rank++)
        for (var file = 0; file < 8; file++)
        {
            if (file != 0) IsolatedMasks[rank * 8 + file] |= GetFileAndRankMask(file - 1, -1);
            if (file != 7) IsolatedMasks[rank * 8 + file] |= GetFileAndRankMask(file + 1, -1);
        }

        for (var rank = 0; rank < 8; rank++)
        {
            for (var file = 0; file < 8; file++)
            {
                var square = rank * 8 + file;
                WhitePassedMasks[square] |= GetFileAndRankMask(file - 1, -1);
                WhitePassedMasks[square] |= GetFileAndRankMask(file, -1);
                WhitePassedMasks[square] |= GetFileAndRankMask(file + 1, -1);
                for (var i = 0; i < 8 - rank; i++)
                    WhitePassedMasks[square] &= ~RankMasks[(7 - i) * 8 + file];
            }
        }

        for (var rank = 0; rank < 8; rank++)
        {
            for (var file = 0; file < 8; file++)
            {
                var square = rank * 8 + file;

                BlackPassedMasks[square] |= GetFileAndRankMask(file - 1, -1);
                BlackPassedMasks[square] |= GetFileAndRankMask(file, -1);
                BlackPassedMasks[square] |= GetFileAndRankMask(file + 1, -1);
                for (var i = 0; i < rank + 1; i++)
                    BlackPassedMasks[square] &= ~RankMasks[i * 8 + file];
            }
        }
    }

    private static readonly int[] PawnTable =
    [
        90, 90, 90, 90, 90, 90, 90, 90,
        30, 30, 30, 40, 40, 30, 30, 30,
        20, 20, 20, 30, 30, 30, 20, 20,
        10, 10, 10, 20, 20, 10, 10, 10,
        5, 5, 10, 20, 20, 5, 5, 5,
        0, 0, 0, 5, 5, 0, 0, 0,
        0, 0, 0, -10, -10, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0
    ];

    private static readonly int[] KnightTable =
    [
        -5, 0, 0, 0, 0, 0, 0, -5,
        -5, 0, 0, 10, 10, 0, 0, -5,
        -5, 5, 20, 20, 20, 20, 5, -5,
        -5, 10, 20, 30, 30, 20, 10, -5,
        -5, 10, 20, 30, 30, 20, 10, -5,
        -5, 5, 20, 10, 10, 20, 5, -5,
        -5, 0, 0, 0, 0, 0, 0, -5,
        -5, -10, 0, 0, 0, 0, -10, -5
    ];

    private static readonly int[] BishopTable =
    [
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 10, 10, 0, 0, 0,
        0, 0, 10, 20, 20, 10, 0, 0,
        0, 0, 10, 20, 20, 10, 0, 0,
        0, 10, 0, 0, 0, 0, 10, 0,
        0, 30, 0, 0, 0, 0, 30, 0,
        0, 0, -10, 0, 0, -10, 0, 0
    ];

    private static readonly int[] RookTable =
    [
        50, 50, 50, 50, 50, 50, 50, 50,
        50, 50, 50, 50, 50, 50, 50, 50,
        0, 0, 10, 20, 20, 10, 0, 0,
        0, 0, 10, 20, 20, 10, 0, 0,
        0, 0, 10, 20, 20, 10, 0, 0,
        0, 0, 10, 20, 20, 10, 0, 0,
        0, 0, 10, 20, 20, 10, 0, 0,
        0, 0, 0, 20, 20, 0, 0, 0
    ];

    private static readonly int[] QueenTable =
    [
        -20, -10, -10, -5, -5, -10, -10, -20,
        -10, 0, 0, 0, 0, 0, 0, -10,
        -10, 0, 5, 5, 5, 5, 0, -10,
        -5, 0, 5, 5, 5, 5, 0, -5,
        0, 0, 5, 5, 5, 5, 0, -5,
        -10, 5, 5, 5, 5, 5, 0, -10,
        -10, 0, 5, 0, 0, 0, 0, -10,
        -20, -10, -10, -5, -5, -10, -10, -20
    ];

    private static readonly int[] KingTable =
    [
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 5, 5, 5, 5, 0, 0,
        0, 5, 5, 10, 10, 5, 5, 0,
        0, 5, 10, 20, 20, 10, 5, 0,
        0, 5, 10, 20, 20, 10, 5, 0,
        0, 0, 5, 10, 10, 5, 0, 0,
        0, 5, 5, -5, -5, 0, 5, 0,
        0, 0, 5, 0, -15, 0, 10, 0
    ];


    public static int[] GetPieceTable(IChessPiece piece)
    {
        return piece switch
        {
            King => KingTable,
            Queen => QueenTable,
            Bishop => BishopTable,
            Rook => RookTable,
            Knight => KnightTable,
            Pawn => PawnTable,
            _ => throw new ArgumentException("Unknown piece type")
        };
    }

    public static int GetPiecePrice(IChessPiece piece)
    {
        return piece switch
        {
            King => 10000,
            Queen => 1000,
            Bishop => 350,
            Rook => 500,
            Knight => 300,
            Pawn => 100,
            _ => throw new ArgumentException("Unknown piece type")
        };
    }

    public static int MirrorSquare(int squareIndex)
    {
        var rank = squareIndex / 8;
        var file = squareIndex % 8;
        var mirroredRank = 7 - rank;
        return mirroredRank * 8 + file;
    }

    public static readonly BitBoard[] IsolatedMasks = new BitBoard[64];
    public static readonly BitBoard[] WhitePassedMasks = new BitBoard[64];
    public static readonly BitBoard[] BlackPassedMasks = new BitBoard[64];
    public static readonly BitBoard[] RankMasks = new BitBoard[64];
    public static readonly BitBoard[] FileMasks = new BitBoard[64];

    private static BitBoard GetFileAndRankMask(int file, int rank)
    {
        var mask = new BitBoard(0UL);
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            if (file != -1 && j == file) mask = mask.SetBit(i * 8 + j);
            if (rank != -1 && i == rank) mask = mask.SetBit(i * 8 + j);
        }

        return mask;
    }
}