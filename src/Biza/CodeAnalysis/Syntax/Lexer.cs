using System.Collections.Generic;

namespace Biza.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private int _position;
        private readonly string _text;

        private readonly DiagnosticBag _diagnostics = new();

        public Lexer(string text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private char Current => Peek(0);

        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _text.Length)
                return '\0';

            return _text[index];
        }

        private void Next() => _position++;

        public SyntaxToken Lex()
        {
            if (_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            }

            var start = _position;

            if (char.IsDigit(Current))
            {
                while (char.IsDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                if (!int.TryParse(text, out var value))
                    _diagnostics.ReportInvalidNumber(new TextSpan(start, length), _text, typeof(int));

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }

            if (char.IsLetter(Current))
            {
                while (char.IsLetter(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);

                return new SyntaxToken(kind, start, text, null);
            }

            (var addPosition, var token) = Current switch
            {
                '+' => (0, new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null)),
                '-' => (0, new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null)),
                '*' => (0, new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null)),
                '/' => (0, new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null)),
                '(' => (0, new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null)),
                ')' => (0, new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null)),
                '&' when Lookahead == '&' => (2, new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, start, "&&", null)),
                '|' when Lookahead == '|' => (2, new SyntaxToken(SyntaxKind.PipePipeToken, start, "||", null)),
                '=' when Lookahead == '=' => (2, new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null)),
                '!' when Lookahead == '=' => (2, new SyntaxToken(SyntaxKind.BangEqualsToken, start, "!=", null)),
                '!' => (0, new SyntaxToken(SyntaxKind.BangToken, _position++, "!", null)),
                '=' => (1, new SyntaxToken(SyntaxKind.EqualsToken, start, "=", null)),
                _ => (0, null)
            };

            _position += addPosition;

            if (token is not null)
                return token;

            _diagnostics.ReportBadCharacter(_position, Current);
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}