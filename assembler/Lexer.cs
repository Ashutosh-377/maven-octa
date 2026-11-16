using System;
using System.Collections.Generic;

public class Lexer
{
    public List<Token> Tokenize(string line)
    {
        List<Token> tokens = new List<Token>();

        int i = 0;

        while (i < line.Length)
        {
            char current = line[i];

            if (char.IsWhiteSpace(current))
            {
                i++;
                continue;
            }

            if (current == ',')
            {
                tokens.Add(new Token(TokenType.Comma, ","));
                i++;
                continue;
            }

            int start = i;
            while (i< line.Length && !char.IsWhiteSpace(line[i]) && line[i] != ',')
            {
                i++;
            }

            string part = line.Substring(start, i - start);

            if (IsRegister(part))
            {
                tokens.Add(new Token(TokenType.Register, part));
            }
            else if (int.TryParse(part, out _))
            {
                tokens.Add(new Token(TokenType.Number, part));
            }
            else if (IsInstruction(part))
            {
                tokens.Add(new Token(TokenType.Instruction, part));
            }
            else
            {
                throw new Exception($"Unknown token: {part}");
            }
        }

        return tokens;
    }

    private bool IsInstruction(string text)
    {
        if (Enum.TryParse<Opcode>(text, true, out _))
        {
            return true;
        }

        if (Enum.TryParse<ExtendedOpcode>(text, true, out _))
        {
            return true;
        }

        return false;
    }

    private bool IsRegister(string text)
    {
        if (!text.StartsWith("R"))
        {
            return false;
        }

        if (!int.TryParse(text.Substring(1), out int number))
        {
            return false;
        }

        return number >= 0 && number <= 7;
    }
}