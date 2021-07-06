﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biza.CodeAnalysis.Syntaxt
{
    internal static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind) => kind switch
        {
            SyntaxKind.PlusToken or SyntaxKind.MinusToken or SyntaxKind.BangToken => 6,
            _ => 0
        };

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind) => kind switch
        {
            SyntaxKind.StarToken or SyntaxKind.SlashToken => 5,
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => 4,
            SyntaxKind.EqualsEqualsToken or SyntaxKind.BangEqualsToken => 3,
            SyntaxKind.AmpersandAmpersandToken => 2,
            SyntaxKind.PipePipeToken => 1,
            _ => 0
        };

        internal static SyntaxKind GetKeywordKind(string text) => text switch
        {
            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            _ => SyntaxKind.IdentifierToken,
        };
    }
}
