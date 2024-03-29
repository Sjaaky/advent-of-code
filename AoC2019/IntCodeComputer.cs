﻿using LanguageExt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AoC2019Test
{
    public class IntCodeComputer
    {
        public bool IsHalted;
        public bool ReturnOnOutput { get; }
        public Dictionary<int, bigint> Memory;
        public List<bigint> Output;
        public int InstructionsExecuted = 0;
        public int InputPointer;

        int Ip;
        int RelativeOffset;
        Func<string> InputFunc;

        public IntCodeComputer(bigint[] program, bool returnOnOutput = false, Func<string> input = null)
            :this(returnOnOutput)
        {
            Memory = program.Select((i, v) => (i, v)).ToDictionary(kv => kv.Item2, kv => kv.Item1);
            InputFunc = input;
        }

        public IntCodeComputer(int[] program, bool returnOnOutput = false)
            :this(returnOnOutput)
        {
            Memory = program.Select((i, v) => (i, v)).ToDictionary(kv => kv.Item2, kv => new bigint(kv.Item1));
        }

        private IntCodeComputer(bool returnOnOutput)
        {
            Ip = 0;
            InputPointer = 0;
            Output = new List<bigint>();
            ReturnOnOutput = returnOnOutput;
            RelativeOffset = 0;
        }

        public void SetNounAndVerb(int noun, int verb)
        {
            Memory[1] = noun;
            Memory[2] = verb;
        }

        public bigint Execute(List<bigint> input = null, int stepsToRun = -1)
        {
            int opcode;
            int steps = 0;
            do
            {
                if (input == null || InputPointer < input.Count)
                {
                    idleCounter = 0;
                }
                opcode = ((int)Memory[Ip]) % 100;
                InstructionsExecuted++;
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
                    case 9: SetRelativeOffset();
                        break;
                    case 99: 
                        break;
                    default:
                        throw new Exception($"unknown opcode {opcode} @{Ip}");
                }
                steps++;
            }
            while (opcode != 99 && (!ReturnOnOutput || opcode != 4) && (stepsToRun == -1 || steps < stepsToRun));
            if (opcode == 99) IsHalted = true;
            return GetMemory(0);
        }

        private void SetRelativeOffset()
        {
            RelativeOffset += (int)GetArg(1);
            Ip += 2;
        }

        public void Add()
        {
            SetMemory(GetArgImmediate(3), GetArg(1) + GetArg(2));
            Ip += 4;
        }

        public void Mul()
        {
            SetMemory(GetArgImmediate(3), GetArg(1) * GetArg(2));
            Ip += 4;
        }

        int idleCounter = 0;
        public bool IsIdle => idleCounter > 1;

        private string inputBuf = null;
        public void In(List<bigint> input)
        {
            if (InputFunc != null)
            {
                if (string.IsNullOrEmpty(inputBuf))
                {
                    inputBuf = InputFunc();
                }
                else
                {
                    SetMemory(GetArgImmediate(1), inputBuf[0]);
                    inputBuf = inputBuf.Substring(1);
                    idleCounter = 0;
                }
            }
            else
            {
                if (input == null || InputPointer >= input.Count)
                {
                    SetMemory(GetArgImmediate(1), -1);
                    idleCounter++;
                }
                else
                {
                    //throw new Exception($"Input is empty {string.Join(",", input)}, {InputPointer}");
                    SetMemory(GetArgImmediate(1), input[InputPointer]);
                    //Console.WriteLine("read input");
                    InputPointer++;
                    idleCounter = 0;
                }
            }
            Ip += 2;
        }

        public void Out()
        {
            Output.Add(GetArg(1));
            idleCounter = 0;
            Ip += 2;
        }

        private void JumpIfTrue()
        {
            if (GetArg(1) != 0)
            {
                Ip = (int)GetArg(2);
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
                Ip = (int)GetArg(2);
            }
            else
            {
                Ip += 3;
            }
        }

        private void LessThen()
        {
            SetMemory(GetArgImmediate(3), (GetArg(1) < GetArg(2)) ? 1 : 0);
            Ip += 4;
        }

        private void Eq()
        {
            SetMemory(GetArgImmediate(3), (GetArg(1) == GetArg(2)) ? 1 : 0);
            Ip += 4;
        }

        public bigint GetArgImmediate(int offset)
        {
            var modeDiv = new[] { 100, 1000, 10000, 100000 };
            var paramMode = (int)(Memory[Ip] / modeDiv[offset-1]) % 10;

            switch (paramMode)
            {
                case 0:
                    return GetMemory(Ip + offset);
                case 1:
                    throw new Exception($"other parammode {paramMode}");
                case 2:
                    var value = GetMemory(Ip + offset);
                    return RelativeOffset + value;
                default:
                    throw new Exception($"unknown parammode opcode={Memory[Ip]} offset={offset} paramMode={paramMode}");
            }
        }
        
        public bigint GetArg(int offset)
        {
            var value = GetMemory(Ip + offset);

            var modeDiv = new[] { 100, 1000, 10000, 100000 };
            var paramMode = (int)(Memory[Ip] / modeDiv[offset-1]) % 10;
            switch (paramMode)
            {
                case 0:
                    return GetMemory(value);
                case 1:
                    return value;
                case 2:
                    return GetMemory(RelativeOffset + value);
                default:
                    throw new Exception($"unknown parammode opcode={Memory[Ip]} offset={offset} paramMode={paramMode}");
            }
        }

        public bigint GetMemory(bigint absAddress)
        {
            if (absAddress > int.MaxValue) throw new IndexOutOfRangeException("addresses need to bigint as well");
            var a = (int)absAddress;
            if (!Memory.TryGetValue(a, out bigint value))
            {
                Memory[a] = 0;
            }
            return Memory[a];
        }

        public bigint SetMemory(bigint absAddress, bigint value)
        {
            if (absAddress > int.MaxValue) throw new IndexOutOfRangeException("addresses need to bigint as well");
            var a = (int)absAddress;
            return Memory[a] = value;
        }
    }
}
