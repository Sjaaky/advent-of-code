using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AoC2019Test
{
    public class IntCodeComputer
    {
        public int[] Memory;
        public List<int> Output;

        int Ip;
        int InputPointer;

        public IntCodeComputer(int[] program, bool returnOnOutput = false)
        {
            Memory = program.ToArray();
            Ip = 0;
            InputPointer = 0;
            Output = new List<int>();
            ReturnOnOutput = returnOnOutput;
        }

        public void SetNounAndVerb(int noun, int verb)
        {
            Memory[1] = noun;
            Memory[2] = verb;
        }

        public int Execute(List<int> input = null)
        {
            int opcode;
            do
            {
                opcode = Memory[Ip] % 100;

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
            while (opcode != 99 && (!ReturnOnOutput || opcode != 4));
            if (opcode == 99) IsHalted = true;
            return Memory[0];
        }

        public bool IsHalted;

        public bool ReturnOnOutput { get; }

        public void Add()
        {
            Memory[GetArgImmediate(3)] = GetArg(1) + GetArg(2);
            Ip += 4;
        }

        public void Mul()
        {
            Memory[GetArgImmediate(3)] = GetArg(1) * GetArg(2);
            Ip += 4;
        }

        public void In(List<int> input)
        {
            if (input == null || InputPointer >= input.Count) throw new Exception($"Input is empty {string.Join(",", input)}, {InputPointer}");
         
            Memory[GetArgImmediate(1)] = input[InputPointer];
            InputPointer++;
            Ip += 2;
        }

        public void Out()
        {
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
