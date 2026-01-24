using MyChess.Core.Board;
using MyChess.Models;

namespace MyChess.Hashing;

using System;
using System.Security.Cryptography;

public static class ZobristHasher
{
    private static readonly ulong[,] PieceHashes;
    private static readonly ulong[] CastlingHashes;
    private static readonly ulong[] EnPassantHashes;
    private static readonly ulong BlackToMoveHash;

    static ZobristHasher()
    {
        PieceHashes = new ulong[64, 12];
        CastlingHashes = new ulong[16];
        EnPassantHashes = new ulong[8];
        BlackToMoveHash = GenerateRandomULong();

        InitializeHashes();
    }

    private static void InitializeHashes()
    {
        for (var square = 0; square < 64; square++)
        for (var pieceType = 0; pieceType < 12; pieceType++)
            PieceHashes[square, pieceType] = GenerateRandomULong();

        for (var i = 0; i < 16; i++)
            CastlingHashes[i] = GenerateRandomULong();

        for (var i = 0; i < 8; i++)
            EnPassantHashes[i] = GenerateRandomULong();
    }

    private static ulong GenerateRandomULong()
    {
        var bytes = new byte[8];
        RandomNumberGenerator.Fill(bytes);
        return BitConverter.ToUInt64(bytes, 0);
    }

    public static ulong CalculateInitialHash(ChessBoard board, BoardState state)
    {
        ulong hash = 0;

        for (var square = 0; square < 64; square++)
        {
            var piece = board.GetPiece(square);
            if (piece != null) hash ^= PieceHashes[square, piece.Index];
        }

        hash ^= CastlingHashes[(int)state.CastlingRights];

        if (state.EnPassantTarget is not null)
        {
            var file = state.EnPassantTarget % 8;
            hash ^= EnPassantHashes[file.Value];
        }

        if (state.CurrentMoveColor is ChessColor.Black) hash ^= BlackToMoveHash;

        return hash;
    }
}