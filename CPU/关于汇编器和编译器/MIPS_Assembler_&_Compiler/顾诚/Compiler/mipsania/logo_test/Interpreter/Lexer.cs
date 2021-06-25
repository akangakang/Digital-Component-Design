
using System;

// Lexical analyzer

namespace Interpreter
{
    namespace Inner
    {
        class Lexer
        {
            public Lexer(IStream source)
            {
                _source = source;
                _currentStringToken = "";
                _comment = false;
                _line = 1;
                _current = _source.next();
                _lastEOF = true;
                _currentToken = new Token();
            }
            private double readNumber()
            {
                double d = 0;
                double t = 1;
                while (Char.IsDigit(_current))
                {
                    d *= 10;
                    d = d + (_current - '0');
                    _current = _source.next();
                }
                if (_current == '.')
                {
                    _current = _source.next();
                    while (Char.IsDigit(_current))
                    {
                        t /= 10;
                        d += t * (_current - '0');
                        _current = _source.next();
                    }
                }
                return d;
            }
            private string readString()
            {
                string retVal = "";
                _current = _source.next();
                while (!_source.eof())
                {
                    switch (_current)
                    {
                        case '\\':
                            {
                                _current = _source.next();
                                switch (_current)
                                {
                                    case 'a': _current = '\a'; break;
                                    case 'b': _current = '\b'; break;
                                    case 'f': _current = '\f'; break;
                                    case 'n': _current = '\n'; break;
                                    case 'r': _current = '\r'; break;
                                    case 't': _current = '\t'; break;
                                    case 'v': _current = '\v'; break;
                                    case '\r': 
                                        _current = _source.next(); _line++;
                                        if (_current == '\n') _current = _source.next();
                                        continue;
                                    case '\n': _current = _source.next(); _line++; continue;
                                    default: break;
                                }
                                retVal += _current;
                                break;
                            }
                        case '\n':
                        case '\r':
                            throw new CompileException(_line, "Unfinished string");
                        case '"':
                            _current = _source.next();
                            return retVal;
                        default:
                            retVal += _current;
                            break;
                    }
                    _current = _source.next();
                }
                throw new CompileException(_line, "Unfinished string");
            }
            public Token next()
            {
                _lastToken = _currentToken;
                _currentToken = new Token();

                while (Char.IsWhiteSpace(_current) && !_source.eof() && _current != '\n')
                    _current = _source.next();

                if (_source.eof())
                {
                    if (_lastEOF)
                    {
                        _currentToken.TokenType = TokenType.TOKEN_EOF;
                        return _currentToken;
                    }
                    _lastEOF = true;
                }
                else
                    _lastEOF = false;

                while (!_source.eof())
                {
                    if (_comment)
                    {
                        _current = _source.next();
                        if (_current != '\n') continue;
                    }
                    switch (_current)
                    {
                        case '\n':
                            _comment = false;
                            _line++;
                            _current = _source.next();
                            break;
                        case '`':
                            _comment = true;
                            _current = _source.next();
                            break;
                        case '-':
                            _currentToken.TokenType = '-';
                            _current = _source.next();
                            return _currentToken;
                        case '!':
                            _currentToken.TokenType = _current;
                            _current = _source.next();
                            if (_current == '=')
                            {
                                _currentToken.TokenType = TokenType.TOKEN_NEQU;
                                _current = _source.next();
                            }
                            return _currentToken;
                        case '>':
                            _currentToken.TokenType = _current;
                            _current = _source.next();
                            if (_current == '=')
                            {
                                _currentToken.TokenType = TokenType.TOKEN_GEQ;
                                _current = _source.next();
                            }
                            return _currentToken;
                        case '<':
                            _currentToken.TokenType = _current;
                            _current = _source.next();
                            if (_current == '=')
                            {
                                _currentToken.TokenType = TokenType.TOKEN_LEQ;
                                _current = _source.next();
                            }
                            return _currentToken;
                        case '=':
                            _currentToken.TokenType = _current;
                            _current = _source.next();
                            if (_current == '=')
                            {
                                _currentToken.TokenType = TokenType.TOKEN_EQUAL;
                                _current = _source.next();
                            }
                            return _currentToken;
                        case '&':
                            _currentToken.TokenType = _current;
                            _current = _source.next();
                            if (_current == '&')
                            {
                                _currentToken.TokenType = TokenType.TOKEN_LOGICAL_AND;
                                _current = _source.next();
                            }
                            return _currentToken;
                        case '|':
                            _currentToken.TokenType = _current;
                            _current = _source.next();
                            if (_current == '|')
                            {
                                _currentToken.TokenType = TokenType.TOKEN_LOGICAL_OR;
                                _current = _source.next();
                            }
                            return _currentToken;
                        case '"':
                            _currentToken.TokenType = TokenType.TOKEN_STRING;
                            _currentToken.strToken = readString();
                            return _currentToken;
                        default:
                            if (Char.IsWhiteSpace(_current))
                            {
                                _current = _source.next();
                                continue;
                            }
                            if (Char.IsDigit(_current))
                            {
                                _currentToken.TokenType = TokenType.TOKEN_NUMBER;
                                _currentToken.numToken = readNumber();
                                return _currentToken;
                            }
                            if (Char.IsLetter(_current) || _current == '_')
                            {
                                do
                                {
                                    _currentStringToken += _current;
                                    _current = _source.next();
                                } while (Char.IsLetterOrDigit(_current) || _current == '_');
                                if (_currentStringToken == "if")
                                    _currentToken.TokenType = TokenType.TOKEN_IF;
                                else if (_currentStringToken == "else")
                                    _currentToken.TokenType = TokenType.TOKEN_ELSE;
                                else if (_currentStringToken == "while")
                                    _currentToken.TokenType = TokenType.TOKEN_WHILE;
                                else if (_currentStringToken == "function")
                                    _currentToken.TokenType = TokenType.TOKEN_FUNCTION;
                                else if (_currentStringToken == "return")
                                    _currentToken.TokenType = TokenType.TOKEN_RETURN;
                                else if (_currentStringToken == "break")
                                    _currentToken.TokenType = TokenType.TOKEN_BREAK;
                                else if (_currentStringToken == "continue")
                                    _currentToken.TokenType = TokenType.TOKEN_CONTINUE;
                                else if (_currentStringToken == "local")
                                    _currentToken.TokenType = TokenType.TOKEN_LOCAL;
                                else if (_currentStringToken == "for")
                                    _currentToken.TokenType = TokenType.TOKEN_FOR;
                                else
                                {
                                    _currentToken.TokenType = TokenType.TOKEN_ID;
                                    _currentToken.strToken = _currentStringToken;
                                }
                                _currentStringToken = "";
                                return _currentToken;
                            }
                            _currentToken.TokenType = _current;
                            _current = _source.next();
                            return _currentToken;
                    }

                }

                _currentToken.TokenType = _current;
                return _currentToken;
            }
            public bool eof()
            {
                return _source.eof();
            }
            private string _currentStringToken;
            private IStream _source;
            private bool _comment;
            private int _line;
            private Token _lastToken;
            private Token _currentToken;
            private bool _lastEOF;
            public char _current;
            public int currentLine
            {
                get
                {
                    return _line;
                }
            }
        }
    }
}
