using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AoC2019Test
{
    public class IntCodeComputer
    {
        public int[] Memory;
        public int[] Program;
        public List<int> Output;

        int Ip;
        int InputPointer;

        public IntCodeComputer(int[] program)
        {
            Program = program;
        }

        public void SetNounAndVerb(int noun, int verb)
        {
            Program[1] = noun;
            Program[2] = verb;
        }

        public int Execute(int[] input = null)
        {
            Memory = Program.ToArray();
            Ip = 0;
            InputPointer = 0;
            Output = new List<int>();

            int opcode;
            do
            {
                opcode = Memory[Ip] % 100;
                Debug.WriteLine($"ip = {Ip} opcode={Memory[Ip]}");

                switch (opcode)
                {
                    case 1: Add();
                        break;
                    case 2: Mul();
                        break;
                    case 3: In(input);
                        break;
                    case 4: Out();
                        break;
                    case 5: JumpIfTrue();
                        break;
                    case 6: JumpIfFalse();
                        break;
                    case 7: LessThen();
                        break;
                    case 8: Eq();
                        break;
                    case 99: 
                        break;
                    default:
                        throw new Exception($"unknown opcode {opcode} @{Ip}");
                }
            }
            while (opcode != 99);

            return Memory[0];
        }

        public void Add()
        {
            Debug.WriteLine($"Add {GetArg(1)}+{GetArg(2)} = { GetArg(1) + GetArg(2)} => {GetArgImmediate(3)}");
            
            Memory[GetArgImmediate(3)] = GetArg(1) + GetArg(2);
            Ip += 4;
        }

        public void Mul()
        {
            Debug.WriteLine($"Mul {GetArg(1)}x{GetArg(2)} = { GetArg(1) * GetArg(2)} => {GetArgImmediate(3)}");
            
            Memory[GetArgImmediate(3)] = GetArg(1) * GetArg(2);
            Ip += 4;
        }

        public void In(int[] input)
        {
            if (input == null || InputPointer >= input.Length) throw new Exception("Input is empty");
            Debug.WriteLine($"In {input[InputPointer]} => {GetArgImmediate(1)}");
         
            Memory[GetArgImmediate(1)] = input[InputPointer];
            InputPointer++;
            Ip += 2;
        }

        public void Out()
        {
            Debug.WriteLine($"Out {GetArg(1)}");
            
            Output.Add(GetArg(1));
            Ip += 2;
        }

        private void JumpIfTrue()
        {
            if (GetArg(1) != 0)
            {
                Ip = GetArg(2);
            }
            else
            {
                Ip += 3;
            }
        }

        private void JumpIfFalse()
        {
            if (GetArg(1) == 0)
            {
                Ip = GetArg(2);
            }
            else
            {
                Ip += 3;
            }
        }

        private void LessThen()
        {
            Memory[GetArgImmediate(3)] = (GetArg(1) < GetArg(2)) ? 1 : 0;
            Ip += 4;
        }

        private void Eq()
        {
            Memory[GetArgImmediate(3)] = (GetArg(1) == GetArg(2)) ? 1 : 0;
            Ip += 4;
        }

        public int GetArgImmediate(int offset)
        {
            var value = Memory[Ip + offset];
            return value;
        }
        
        public int GetArg(int offset)
        {
            var value = Memory[Ip + offset];

            var modeDiv = new[] { 100, 1000, 10000, 100000 };
            var paramMode = (Memory[Ip] / modeDiv[offset-1]) % 10;
            switch (paramMode)
            {
                case 0:
                    return Memory[value];
                case 1:
                    return value;
                default:
                    throw new Exception($"unknown parammode opcode={Memory[Ip]} offset={offset} paramMode={paramMode}");
            }
        }
    }
}
