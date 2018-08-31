#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

namespace SourceCode.Clay.Algorithms
{
    /// <summary>
    /// 
    /// </summary>
    public static class HuffmanOrigOpt
    {
        // TODO: this can be constructed from _decodingTable
        private static readonly (uint code, int bitLength)[] s_encodingTable = new (uint code, int bitLength)[]
        {
            // 0
            (0b11111111_11000000_00000000_00000000, 13),

            // 1
            (0b11111111_11111111_10110000_00000000, 23),
            (0b11111111_11111111_11111110_00100000, 28),
            (0b11111111_11111111_11111110_00110000, 28),
            (0b11111111_11111111_11111110_01000000, 28),
            (0b11111111_11111111_11111110_01010000, 28),
            (0b11111111_11111111_11111110_01100000, 28),
            (0b11111111_11111111_11111110_01110000, 28),
            (0b11111111_11111111_11111110_10000000, 28),
            (0b11111111_11111111_11101010_00000000, 24),
            (0b11111111_11111111_11111111_11110000, 30),

            // 11
            (0b11111111_11111111_11111110_10010000, 28),
            (0b11111111_11111111_11111110_10100000, 28),
            (0b11111111_11111111_11111111_11110100, 30),
            (0b11111111_11111111_11111110_10110000, 28),
            (0b11111111_11111111_11111110_11000000, 28),
            (0b11111111_11111111_11111110_11010000, 28),
            (0b11111111_11111111_11111110_11100000, 28),
            (0b11111111_11111111_11111110_11110000, 28),
            (0b11111111_11111111_11111111_00000000, 28),
            (0b11111111_11111111_11111111_00010000, 28),

            // 21
            (0b11111111_11111111_11111111_00100000, 28),
            (0b11111111_11111111_11111111_11111000, 30),
            (0b11111111_11111111_11111111_00110000, 28),
            (0b11111111_11111111_11111111_01000000, 28),
            (0b11111111_11111111_11111111_01010000, 28),
            (0b11111111_11111111_11111111_01100000, 28),
            (0b11111111_11111111_11111111_01110000, 28),
            (0b11111111_11111111_11111111_10000000, 28),
            (0b11111111_11111111_11111111_10010000, 28),
            (0b11111111_11111111_11111111_10100000, 28),

            // 31
            (0b11111111_11111111_11111111_10110000, 28),
            (0b01010000_00000000_00000000_00000000, 6),  // <space>
            (0b11111110_00000000_00000000_00000000, 10), // !
            (0b11111110_01000000_00000000_00000000, 10), // "
            (0b11111111_10100000_00000000_00000000, 12), // #
            (0b11111111_11001000_00000000_00000000, 13), // $
            (0b01010100_00000000_00000000_00000000, 6),  // %
            (0b11111000_00000000_00000000_00000000, 8),  // &
            (0b11111111_01000000_00000000_00000000, 11), // '
            (0b11111110_10000000_00000000_00000000, 10), // (

            // 41
            (0b11111110_11000000_00000000_00000000, 10), // )
            (0b11111001_00000000_00000000_00000000, 8),  // *
            (0b11111111_01100000_00000000_00000000, 11), // +
            (0b11111010_00000000_00000000_00000000, 8),  // ,
            (0b01011000_00000000_00000000_00000000, 6),  // -
            (0b01011100_00000000_00000000_00000000, 6),  // .
            (0b01100000_00000000_00000000_00000000, 6),  // /
            (0b00000000_00000000_00000000_00000000, 5),  // 0
            (0b00001000_00000000_00000000_00000000, 5),  // 1
            (0b00010000_00000000_00000000_00000000, 5),  // 2

            // 51
            (0b01100100_00000000_00000000_00000000, 6),  // 3
            (0b01101000_00000000_00000000_00000000, 6),  // 4
            (0b01101100_00000000_00000000_00000000, 6),  // 5
            (0b01110000_00000000_00000000_00000000, 6),  // 6
            (0b01110100_00000000_00000000_00000000, 6),  // 7
            (0b01111000_00000000_00000000_00000000, 6),  // 8
            (0b01111100_00000000_00000000_00000000, 6),  // 9
            (0b10111000_00000000_00000000_00000000, 7),  // :
            (0b11111011_00000000_00000000_00000000, 8),  // ;
            (0b11111111_11111000_00000000_00000000, 15), // <

            // 61
            (0b10000000_00000000_00000000_00000000, 6),  // =
            (0b11111111_10110000_00000000_00000000, 12), // >
            (0b11111111_00000000_00000000_00000000, 10), // ?
            (0b11111111_11010000_00000000_00000000, 13), // @
            (0b10000100_00000000_00000000_00000000, 6),  // A
            (0b10111010_00000000_00000000_00000000, 7),  // B
            (0b10111100_00000000_00000000_00000000, 7),  // C
            (0b10111110_00000000_00000000_00000000, 7),  // D
            (0b11000000_00000000_00000000_00000000, 7),  // E
            (0b11000010_00000000_00000000_00000000, 7),  // F
                                                         
            // 71                                        
            (0b11000100_00000000_00000000_00000000, 7),  // G
            (0b11000110_00000000_00000000_00000000, 7),  // H
            (0b11001000_00000000_00000000_00000000, 7),  // I
            (0b11001010_00000000_00000000_00000000, 7),  // J
            (0b11001100_00000000_00000000_00000000, 7),  // K
            (0b11001110_00000000_00000000_00000000, 7),  // L
            (0b11010000_00000000_00000000_00000000, 7),  // M
            (0b11010010_00000000_00000000_00000000, 7),  // N
            (0b11010100_00000000_00000000_00000000, 7),  // O
            (0b11010110_00000000_00000000_00000000, 7),  // P
                                                         
            // 81                                        
            (0b11011000_00000000_00000000_00000000, 7),  // Q
            (0b11011010_00000000_00000000_00000000, 7),  // R
            (0b11011100_00000000_00000000_00000000, 7),  // S
            (0b11011110_00000000_00000000_00000000, 7),  // T
            (0b11100000_00000000_00000000_00000000, 7),  // U
            (0b11100010_00000000_00000000_00000000, 7),  // V
            (0b11100100_00000000_00000000_00000000, 7),  // W
            (0b11111100_00000000_00000000_00000000, 8),  // X
            (0b11100110_00000000_00000000_00000000, 7),  // Y
            (0b11111101_00000000_00000000_00000000, 8),  // Z 

            // 91
            (0b11111111_11011000_00000000_00000000, 13), // [
            (0b11111111_11111110_00000000_00000000, 19), // \
            (0b11111111_11100000_00000000_00000000, 13), // ]
            (0b11111111_11110000_00000000_00000000, 14), // ^
            (0b10001000_00000000_00000000_00000000, 6),  // _
            (0b11111111_11111010_00000000_00000000, 15), // `
            (0b00011000_00000000_00000000_00000000, 5),  // a
            (0b10001100_00000000_00000000_00000000, 6),  // b
            (0b00100000_00000000_00000000_00000000, 5),  // c
            (0b10010000_00000000_00000000_00000000, 6),  // d

            // 101
            (0b00101000_00000000_00000000_00000000, 5),  // e
            (0b10010100_00000000_00000000_00000000, 6),  // f
            (0b10011000_00000000_00000000_00000000, 6),  // g
            (0b10011100_00000000_00000000_00000000, 6),  // h
            (0b00110000_00000000_00000000_00000000, 5),  // i
            (0b11101000_00000000_00000000_00000000, 7),  // j
            (0b11101010_00000000_00000000_00000000, 7),  // k
            (0b10100000_00000000_00000000_00000000, 6),  // l
            (0b10100100_00000000_00000000_00000000, 6),  // m
            (0b10101000_00000000_00000000_00000000, 6),  // n

            // 111
            (0b00111000_00000000_00000000_00000000, 5),  // o
            (0b10101100_00000000_00000000_00000000, 6),  // p
            (0b11101100_00000000_00000000_00000000, 7),  // q
            (0b10110000_00000000_00000000_00000000, 6),  // r
            (0b01000000_00000000_00000000_00000000, 5),  // s
            (0b01001000_00000000_00000000_00000000, 5),  // t
            (0b10110100_00000000_00000000_00000000, 6),  // u
            (0b11101110_00000000_00000000_00000000, 7),  // v
            (0b11110000_00000000_00000000_00000000, 7),  // w
            (0b11110010_00000000_00000000_00000000, 7),  // x

            // 121
            (0b11110100_00000000_00000000_00000000, 7),  // y
            (0b11110110_00000000_00000000_00000000, 7),  // z
            (0b11111111_11111100_00000000_00000000, 15), // {
            (0b11111111_10000000_00000000_00000000, 11), // |
            (0b11111111_11110100_00000000_00000000, 14), // }
            (0b11111111_11101000_00000000_00000000, 13), // ~
            (0b11111111_11111111_11111111_11000000, 28),
            (0b11111111_11111110_01100000_00000000, 20),
            (0b11111111_11111111_01001000_00000000, 22),
            (0b11111111_11111110_01110000_00000000, 20),

            // 131
            (0b11111111_11111110_10000000_00000000, 20),
            (0b11111111_11111111_01001100_00000000, 22),
            (0b11111111_11111111_01010000_00000000, 22),
            (0b11111111_11111111_01010100_00000000, 22),
            (0b11111111_11111111_10110010_00000000, 23),
            (0b11111111_11111111_01011000_00000000, 22),
            (0b11111111_11111111_10110100_00000000, 23),
            (0b11111111_11111111_10110110_00000000, 23),
            (0b11111111_11111111_10111000_00000000, 23),
            (0b11111111_11111111_10111010_00000000, 23),

            // 141
            (0b11111111_11111111_10111100_00000000, 23),
            (0b11111111_11111111_11101011_00000000, 24),
            (0b11111111_11111111_10111110_00000000, 23),
            (0b11111111_11111111_11101100_00000000, 24),
            (0b11111111_11111111_11101101_00000000, 24),
            (0b11111111_11111111_01011100_00000000, 22),
            (0b11111111_11111111_11000000_00000000, 23),
            (0b11111111_11111111_11101110_00000000, 24),
            (0b11111111_11111111_11000010_00000000, 23),
            (0b11111111_11111111_11000100_00000000, 23),

            // 151
            (0b11111111_11111111_11000110_00000000, 23),
            (0b11111111_11111111_11001000_00000000, 23),
            (0b11111111_11111110_11100000_00000000, 21),
            (0b11111111_11111111_01100000_00000000, 22),
            (0b11111111_11111111_11001010_00000000, 23),
            (0b11111111_11111111_01100100_00000000, 22),
            (0b11111111_11111111_11001100_00000000, 23),
            (0b11111111_11111111_11001110_00000000, 23),
            (0b11111111_11111111_11101111_00000000, 24),
            (0b11111111_11111111_01101000_00000000, 22),

            // 161
            (0b11111111_11111110_11101000_00000000, 21),
            (0b11111111_11111110_10010000_00000000, 20),
            (0b11111111_11111111_01101100_00000000, 22),
            (0b11111111_11111111_01110000_00000000, 22),
            (0b11111111_11111111_11010000_00000000, 23),
            (0b11111111_11111111_11010010_00000000, 23),
            (0b11111111_11111110_11110000_00000000, 21),
            (0b11111111_11111111_11010100_00000000, 23),
            (0b11111111_11111111_01110100_00000000, 22),
            (0b11111111_11111111_01111000_00000000, 22),

            // 171
            (0b11111111_11111111_11110000_00000000, 24),
            (0b11111111_11111110_11111000_00000000, 21),
            (0b11111111_11111111_01111100_00000000, 22),
            (0b11111111_11111111_11010110_00000000, 23),
            (0b11111111_11111111_11011000_00000000, 23),
            (0b11111111_11111111_00000000_00000000, 21),
            (0b11111111_11111111_00001000_00000000, 21),
            (0b11111111_11111111_10000000_00000000, 22),
            (0b11111111_11111111_00010000_00000000, 21),
            (0b11111111_11111111_11011010_00000000, 23),

            // 181
            (0b11111111_11111111_10000100_00000000, 22),
            (0b11111111_11111111_11011100_00000000, 23),
            (0b11111111_11111111_11011110_00000000, 23),
            (0b11111111_11111110_10100000_00000000, 20),
            (0b11111111_11111111_10001000_00000000, 22),
            (0b11111111_11111111_10001100_00000000, 22),
            (0b11111111_11111111_10010000_00000000, 22),
            (0b11111111_11111111_11100000_00000000, 23),
            (0b11111111_11111111_10010100_00000000, 22),
            (0b11111111_11111111_10011000_00000000, 22),

            // 191
            (0b11111111_11111111_11100010_00000000, 23),
            (0b11111111_11111111_11111000_00000000, 26),
            (0b11111111_11111111_11111000_01000000, 26),
            (0b11111111_11111110_10110000_00000000, 20),
            (0b11111111_11111110_00100000_00000000, 19),
            (0b11111111_11111111_10011100_00000000, 22),
            (0b11111111_11111111_11100100_00000000, 23),
            (0b11111111_11111111_10100000_00000000, 22),
            (0b11111111_11111111_11110110_00000000, 25),
            (0b11111111_11111111_11111000_10000000, 26),

            // 201
            (0b11111111_11111111_11111000_11000000, 26),
            (0b11111111_11111111_11111001_00000000, 26),
            (0b11111111_11111111_11111011_11000000, 27),
            (0b11111111_11111111_11111011_11100000, 27),
            (0b11111111_11111111_11111001_01000000, 26),
            (0b11111111_11111111_11110001_00000000, 24),
            (0b11111111_11111111_11110110_10000000, 25),
            (0b11111111_11111110_01000000_00000000, 19),
            (0b11111111_11111111_00011000_00000000, 21),
            (0b11111111_11111111_11111001_10000000, 26),

            // 211
            (0b11111111_11111111_11111100_00000000, 27),
            (0b11111111_11111111_11111100_00100000, 27),
            (0b11111111_11111111_11111001_11000000, 26),
            (0b11111111_11111111_11111100_01000000, 27),
            (0b11111111_11111111_11110010_00000000, 24),
            (0b11111111_11111111_00100000_00000000, 21),
            (0b11111111_11111111_00101000_00000000, 21),
            (0b11111111_11111111_11111010_00000000, 26),
            (0b11111111_11111111_11111010_01000000, 26),
            (0b11111111_11111111_11111111_11010000, 28),

            // 221
            (0b11111111_11111111_11111100_01100000, 27),
            (0b11111111_11111111_11111100_10000000, 27),
            (0b11111111_11111111_11111100_10100000, 27),
            (0b11111111_11111110_11000000_00000000, 20),
            (0b11111111_11111111_11110011_00000000, 24),
            (0b11111111_11111110_11010000_00000000, 20),
            (0b11111111_11111111_00110000_00000000, 21),
            (0b11111111_11111111_10100100_00000000, 22),
            (0b11111111_11111111_00111000_00000000, 21),
            (0b11111111_11111111_01000000_00000000, 21),

            // 231
            (0b11111111_11111111_11100110_00000000, 23),
            (0b11111111_11111111_10101000_00000000, 22),
            (0b11111111_11111111_10101100_00000000, 22),
            (0b11111111_11111111_11110111_00000000, 25),
            (0b11111111_11111111_11110111_10000000, 25),
            (0b11111111_11111111_11110100_00000000, 24),
            (0b11111111_11111111_11110101_00000000, 24),
            (0b11111111_11111111_11111010_10000000, 26),
            (0b11111111_11111111_11101000_00000000, 23),
            (0b11111111_11111111_11111010_11000000, 26),

            // 241
            (0b11111111_11111111_11111100_11000000, 27),
            (0b11111111_11111111_11111011_00000000, 26),
            (0b11111111_11111111_11111011_01000000, 26),
            (0b11111111_11111111_11111100_11100000, 27),
            (0b11111111_11111111_11111101_00000000, 27),
            (0b11111111_11111111_11111101_00100000, 27),
            (0b11111111_11111111_11111101_01000000, 27),
            (0b11111111_11111111_11111101_01100000, 27),
            (0b11111111_11111111_11111111_11100000, 28),
            (0b11111111_11111111_11111101_10000000, 27),

            // 251
            (0b11111111_11111111_11111101_10100000, 27),
            (0b11111111_11111111_11111101_11000000, 27),
            (0b11111111_11111111_11111101_11100000, 27),
            (0b11111111_11111111_11111110_00000000, 27),
            (0b11111111_11111111_11111011_10000000, 26),

            // 256
            (0b11111111_11111111_11111111_11111100, 30)
        };

        private const int _rows = 21;
        private const int _last = 4;
        private static readonly (byte codeLength, int codeMax, int mask, byte[] codes)[] s_decodingTable = new (byte, int, int, byte[])[_rows]
        {
            (05, 00000000_10, int.MinValue >> 04, new byte[00_10] { 048, 049, 050, 097, 099, 101, 105, 111, 115, 116 }), // 10
            (06, 00000000_46, int.MinValue >> 05, new byte[00_26] { 032, 037, 045, 046, 047, 051, 052, 053, 054, 055, 056, 057, 061, 065, 095, 098, 100, 102, 103, 104, 108, 109, 110, 112, 114, 117 }), // 26
            (07, 0000000_124, int.MinValue >> 06, new byte[00_32] { 058, 066, 067, 068, 069, 070, 071, 072, 073, 074, 075, 076, 077, 078, 079, 080, 081, 082, 083, 084, 085, 086, 087, 089, 106, 107, 113, 118, 119, 120, 121, 122 }), // 32
            (08, 0000000_254, int.MinValue >> 07, new byte[000_6] { 038, 042, 044, 059, 088, 090 }), // 6
            (10, 000000_1021, int.MinValue >> 09, new byte[000_5] { 033, 034, 040, 041, 063 }), // 5
            (11, 000000_2045, int.MinValue >> 10, new byte[000_3] { 039, 043, 124 }), // 3
            (12, 000000_4092, int.MinValue >> 11, new byte[000_2] { 035, 062 }), // 2
            (13, 000000_8190, int.MinValue >> 12, new byte[000_6] { 000, 036, 064, 091, 093, 126 }), // 6
            (14, 00000_16382, int.MinValue >> 13, new byte[000_2] { 094, 125 }), // 2
            (15, 00000_32767, int.MinValue >> 14, new byte[000_3] { 060, 096, 123 }), // 3
            (19, 0000_524275, int.MinValue >> 18, new byte[000_3] { 092, 195, 208 }), // 3
            (20, 000_1048558, int.MinValue >> 19, new byte[000_8] { 128, 130, 131, 162, 184, 194, 224, 226 }), // 8
            (21, 000_2097129, int.MinValue >> 20, new byte[00_13] { 153, 161, 167, 172, 176, 177, 179, 209, 216, 217, 227, 229, 230 }), // 13
            (22, 000_4194284, int.MinValue >> 21, new byte[00_26] { 129, 132, 133, 134, 136, 146, 154, 156, 160, 163, 164, 169, 170, 173, 178, 181, 185, 186, 187, 189, 190, 196, 198, 228, 232, 233 }), // 26
            (23, 000_8388597, int.MinValue >> 22, new byte[00_29] { 001, 135, 137, 138, 139, 140, 141, 143, 147, 149, 150, 151, 152, 155, 157, 158, 165, 166, 168, 174, 175, 180, 182, 183, 188, 191, 197, 231, 239 }), // 29
            (24, 00_16777206, int.MinValue >> 23, new byte[00_12] { 009, 142, 144, 145, 148, 159, 171, 206, 215, 225, 236, 237 }), // 12
            (25, 00_33554416, int.MinValue >> 24, new byte[000_4] { 199, 207, 234, 235 }), // 4
            (26, 00_67108847, int.MinValue >> 25, new byte[00_15] { 192, 193, 200, 201, 202, 205, 210, 213, 218, 219, 238, 240, 242, 243, 255 }), // 15
            (27, 0_134217713, int.MinValue >> 26, new byte[00_19] { 203, 204, 211, 212, 214, 221, 222, 223, 241, 244, 245, 246, 247, 248, 250, 251, 252, 253, 254 }), // 19
            (28, 0_268435455, int.MinValue >> 27, new byte[00_29] { 002, 003, 004, 005, 006, 007, 008, 011, 012, 014, 015, 016, 017, 018, 019, 020, 021, 023, 024, 025, 026, 027, 028, 029, 030, 031, 127, 220, 249 }), // 29
            (30, 1_073741824, int.MinValue >> 29, new byte[_last] { 010, 013, 022, 0 /* 256: Special handling for last cell */ }) // 4  
        };

        static HuffmanOrigOpt()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static (uint encoded, int bitLength) Encode(int data) => s_encodingTable[data];

        /// <summary>
        /// Decodes a Huffman encoded string from a byte array.
        /// </summary>
        /// <param name="src">The source byte array containing the encoded data.</param>
        /// <param name="offset">The offset in the byte array where the coded data starts.</param>
        /// <param name="count">The number of bytes to decode.</param>
        /// <param name="dst">The destination byte array to store the decoded data.</param>
        /// <returns>The number of decoded symbols.</returns>
        public static int Decode(byte[] src, int offset, int count, byte[] dst)
        {
            var i = offset;
            var j = 0;
            var lastDecodedBits = 0;
            var edgeIndex = count - 1;

            while (i <= edgeIndex)
            {
                var next = (uint)(src[i] << 24 + lastDecodedBits);
                if (i + 1 < src.Length)
                {
                    next |= (uint)(src[i + 1] << 16 + lastDecodedBits);

                    if (i + 2 < src.Length)
                    {
                        next |= (uint)(src[i + 2] << 8 + lastDecodedBits);

                        if (i + 3 < src.Length)
                        {
                            next |= (uint)(src[i + 3] << lastDecodedBits);
                        }
                    }
                }

                var remainingBits = 8 - lastDecodedBits;

                // The remaining 7 or less bits are all 1, which is padding.
                // We specifically check that lastDecodedBits > 0 because padding
                // longer than 7 bits should be treated as a decoding error.
                // http://httpwg.org/specs/rfc7541.html#rfc.section.5.2
                if (i == edgeIndex && lastDecodedBits > 0)
                {
                    var ones = (uint)(int.MinValue >> remainingBits - 1);

                    if ((next & ones) == ones)
                        break;
                }

                if (j == dst.Length)
                {
                    // Destination is too small.
                    throw new HuffmanDecodingException();
                }

                // The longest possible symbol size is 30 bits. If we're at the last 4 bytes
                // of the input, we need to make sure we pass the correct number of valid bits
                // left, otherwise the trailing 0s in next may form a valid symbol.
                var validBits = remainingBits + (edgeIndex - i) * 8;
                if (validBits > 30)
                    validBits = 30; // Equivalent to Math.Min(30, validBits)

                var ch = Decode(next, validBits, out var decodedBits);

                if (ch == -1 || ch == 256)
                {
                    // -1: No valid symbol could be decoded with the bits in next.

                    // 256: A Huffman-encoded string literal containing the EOS symbol MUST be treated as a decoding error.
                    // http://httpwg.org/specs/rfc7541.html#rfc.section.5.2
                    throw new HuffmanDecodingException();
                }

                dst[j++] = (byte)ch;

                // If we crossed a byte boundary, advance i so we start at the next byte that's not fully decoded.
                lastDecodedBits += decodedBits;
                i += lastDecodedBits / 8;

                // Modulo 8 since we only care about how many bits were decoded in the last byte that we processed.
                lastDecodedBits %= 8;
            }

            return j;
        }

        /// <summary>
        /// Decodes a single symbol from a 32-bit word.
        /// </summary>
        /// <param name="data">A 32-bit word containing a Huffman encoded symbol.</param>
        /// <param name="validBits">
        /// The number of bits in <paramref name="data"/> that may contain an encoded symbol.
        /// This is not the exact number of bits that encode the symbol. Instead, it prevents
        /// decoding the lower bits of <paramref name="data"/> if they don't contain any
        /// encoded data.
        /// </param>
        /// <param name="decodedBits">The number of bits decoded from <paramref name="data"/>.</param>
        /// <returns>The decoded symbol.</returns>
        public static int Decode(uint data, int validBits, out int decodedBits)
        {
            // The code below implements the decoding logic for a canonical Huffman code.
            //
            // To decode a symbol, we scan the decoding table, which is sorted by ascending symbol bit length.
            // For each bit length b, we determine the maximum b-bit encoded value, plus one (that is codeMax).
            // This is done with the following logic:
            //
            // if we're at the first entry in the table,
            //    codeMax = the # of symbols encoded in b bits
            // else,
            //    left-shift codeMax by the difference between b and the previous entry's bit length,
            //    then increment codeMax by the # of symbols encoded in b bits
            //
            // Next, we look at the value v encoded in the highest b bits of data. If v is less than codeMax,
            // those bits correspond to a Huffman encoded symbol. We find the corresponding decoded
            // symbol in the list of values associated with bit length b in the decoding table by indexing it
            // with codeMax - v.

            var result = -1;
            decodedBits = 0;

            for (var i = 0; i < s_decodingTable.Length; i++)
            {
                var (codeLength, codeMax, mask, codes) = s_decodingTable[i];
                if (codeLength > validBits)
                    break;

                var masked = (data & mask) >> (32 - codeLength);

                if (masked < codeMax)
                {
                    decodedBits = codeLength;
                    var j = codes.Length - (codeMax - masked);

                    var is256 = (i == _rows - 1 && j == _last - 1); // 256: Special handling for last cell
                    result = is256 ? 256 : codes[j];

                    break;
                }
            }

            return result;
        }
    }
}