using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Teleglib.Router.Patterns {
    public class RoutePatternParser {

        private readonly string _input;
        private ParserToken _currentToken;
        private StringBuilder _currentLeftPart;
        private StringBuilder _currentRightPart;

        public static IEnumerable<IRoutePatternPart> Parse(string input) {
            return new RoutePatternParser(input).Parse();
        }

        private RoutePatternParser(string input) {
            _input = input.Trim(' ').TrimStart('/');
            Reset();
        }

        private IEnumerable<IRoutePatternPart> Parse() {
            for (var i = 0; i <= _input.Length; i++) {
                var c = i == _input.Length ? '\0' : _input[i];

                switch (_currentToken) {
                    case ParserToken.BeginPart:
                        CannotBeEndOfString(c);
                        switch (c) {
                            case '\\':
                                _currentToken = ParserToken.EscapeInExactPart;
                                break;
                            case '{':
                                _currentToken = ParserToken.NamedPartLeftBody;
                                break;
                            default:
                                _currentLeftPart.Append(c);
                                _currentToken = ParserToken.ExactPartBody;
                                break;
                        }
                        break;
                    case ParserToken.ExactPartBody:
                        switch (c) {
                            case '\\':
                                _currentToken = ParserToken.EscapeInExactPart;
                                break;
                            case ':':
                            case '\0':
                                yield return new ExactRoutePatternPart(_currentLeftPart.ToString());
                                Reset();
                                break;
                            default:
                                _currentLeftPart.Append(c);
                                break;
                        }
                        break;
                    case ParserToken.NamedPartLeftBody:
                        CannotBeEndOfString(c);
                        switch (c) {
                            case '\\':
                                _currentToken = ParserToken.EscapeInNamedLeftPart;
                                break;
                            case ':':
                                _currentToken = ParserToken.NamedPartRigthBody;
                                break;
                            case '}':
                                _currentToken = ParserToken.CompleteNamedPart;
                                break;
                            default:
                                _currentLeftPart.Append(c);
                                break;
                        }
                        break;
                    case ParserToken.NamedPartRigthBody:
                        CannotBeEndOfString(c);
                        switch (c) {
                            case '\\':
                                _currentToken = ParserToken.EscapeInNamedRightPart;
                                break;
                            case '}':
                                _currentToken = ParserToken.CompleteNamedPart;
                                break;
                            default:
                                _currentRightPart.Append(c);
                                break;
                        }
                        break;
                    case ParserToken.CompleteNamedPart:
                        if (c == ':' || c == '\0') {
                            yield return CreateNamedPart();
                            Reset();
                        }
                        else {
                            throw new AggregateException($"Illegal symbol {c} after named part");
                        }
                        break;
                    case ParserToken.EscapeInExactPart:
                        CannotBeEndOfString(c);
                        _currentLeftPart.Append(c);
                        _currentToken = ParserToken.ExactPartBody;
                        break;
                    case ParserToken.EscapeInNamedLeftPart:
                        CannotBeEndOfString(c);
                        _currentLeftPart.Append(c);
                        _currentToken = ParserToken.NamedPartLeftBody;
                        break;
                    case ParserToken.EscapeInNamedRightPart:
                        CannotBeEndOfString(c);
                        _currentRightPart.Append(c);
                        _currentToken = ParserToken.NamedPartRigthBody;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown token " + _currentToken);
                }
            }
        }

        private void CannotBeEndOfString(char c) {
            if (c == '\0') {
                throw new ArgumentException("Unexpected end of string on token " + _currentToken);
            }
        }

        private IRoutePatternPart CreateNamedPart() {
            var name = _currentLeftPart.ToString();
            if (_currentRightPart.Length == 0) {
                return new NamedRoutePatternPart(name);
            }
            var regex = new Regex(_currentRightPart.ToString(), RegexOptions.Compiled);
            return new NamedRegexRoutePatternPart(name, regex);
        }

        private void Reset() {
            _currentLeftPart = new StringBuilder();
            _currentRightPart = new StringBuilder();
            _currentToken = ParserToken.BeginPart;
        }

        private enum ParserToken {
            BeginPart,
            ExactPartBody,

            NamedPartLeftBody,
            NamedPartRigthBody,
            CompleteNamedPart,

            EscapeInExactPart,
            EscapeInNamedLeftPart,
            EscapeInNamedRightPart
        }
    }
}