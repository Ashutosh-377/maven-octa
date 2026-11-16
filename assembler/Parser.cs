using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Http.Headers;

public class Parser
{
    public ParsedInstruction Parse(List<Token> tokens)
    {
        if (tokens.Count == 0)
        {
            throw new Exception("Empty instruction.");
        }

        if (tokens[0].Type != TokenType.Instruction)
        {
            throw new Exception("Expected instruction.");
        }

        string opcode = tokens[0].Value.ToUpper();

        switch (opcode)
        {
            case "RET":
            case "HALT":
                CheckOperandCount(tokens, 1);
                return new ParsedInstruction(opcode, "", "");

            case "INC":
            case "DEC":
            case "NOT":
            case "PUSH":
            case "POP":
                CheckRegisterInstruction(tokens);
                return new ParsedInstruction(opcode, tokens[1].Value, "");

            case "JMP":
            case "JZ":
            case "JNZ":
            case "CALL":
                CheckAddressInstruction(tokens);
                return new ParsedInstruction(opcode, tokens[1].Value, "");

            default:
                CheckTwoOperandInstruction(tokens);
                return new ParsedInstruction(opcode, tokens[1].Value, tokens[3].Value);
        }
    }

    private void CheckRegisterInstruction(List<Token> tokens)
    {
        if (tokens.Count != 2)
        {
            throw new Exception("Expected one register.");
        }

        if (tokens[1].Type != TokenType.Register)
        {
            throw new Exception("Expected register.");
        }
    }

    private void CheckAddressInstruction(List<Token> tokens)
    {
        if (tokens.Count != 2)
        {
            throw new Exception("Expected one address.");
        }

        if (tokens[1].Type != TokenType.Number)
        {
            throw new Exception("Expected number.");
        }
    }

    private void CheckTwoOperandInstruction(List<Token> tokens)
    {
        if (tokens.Count != 4)
        {
            throw new Exception("Expected two operands.");
        }

        if (tokens[1].Type != TokenType.Register)
        {
            throw new Exception("Expected first register.");
        }

        if (tokens[2].Type != TokenType.Comma)
        {
            throw new Exception("Expected comma.");
        }

        if (tokens[3].Type != TokenType.Register && tokens[3].Type != TokenType.Number)
        {
            throw new Exception("Expected register or number.");
        }
    }

    private void CheckOperandCount(List<Token> tokens, int expected)
    {
        if (tokens.Count != expected)
        {
            throw new Exception("Unexpected operands.");            
        }
    }
}