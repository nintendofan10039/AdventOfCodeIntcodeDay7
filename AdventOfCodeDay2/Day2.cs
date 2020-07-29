using System;
using System.Collections.Generic;
using System.IO;
using Emulator;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

//Test code:
//brackets are just for readability
//(1,0,0,3,99) -> reads the values at position 0 and 0, adds them up and stores the value at position 3 and halts the program
//(1,10,20,30) -> reads the values at position 10 and 20, add them and stores the value at position 30
//(1,9,10,3,2,3,11,0,99,30,40,50) -> 30+40=70, (1, 9, 10, 70, 2, 3, 11, 0, 99, 30, 40, 50) -> 70*50=3500, (3500, 9, 10, 70, 2, 3, 11, 0, 99, 30, 40, 50)
//(1,0,0,0,99) -> (2, 0, 0, 0, 99)
//(2, 3, 0, 3, 99) -> (2, 3, 0, 6, 99)
//(2, 4, 4, 5, 99, 0) -> (2, 4, 4, 5, 99, 9801)
//(1, 1, 1, 4, 99, 5, 6, 0, 99) -> (30, 1, 1, 4, 2, 5, 6, 0, 99)

//Intcode program list of integers separated by commas
//position 0 contains opcode(1, 2 or 99)
//99 means that the program is finished and should stop
//unknown opcode (something other than 1, 2 or 99) something went wrong
//opcode 1 adds numbers from two positions and stores the result in a third position
//the three ints after the opcode tell the three position, first two indicate positions from where to read the input values
//third integer specifies the position where the result is stored
//opcode 2 multiplies the two inputs, following the rules as before with opcode 1
//the integers after the opcode specifies WHERE the inputs and outputs are, NOT the values
//once done processing an opcode, move forward 4 positions
//before running the program, replace position 1 with the value 12 and replace position 2 with the value 2,
//Q: What value is left at position 0 after the program halts?

// Add support for paramter modes:
// parameter mode 0 = position mode(the parameter is interpreted as a position)
// i.e, if parameter is 50, it is the value stored at address 50
// parameter mode 1 = immediate mode
// i.e, if parameter is 50, then the value is 50
// parameter modes are stored in the same value as the instruction's opcode
// 1002,4,3,4,33 => 2 = multiply opcode, 0(hundreds digit) = position, 1(thousands digit) = immediate, and 0 = position(not present, so 0)
// ABCDE format: DE = two-digit opcode, C = 1st param mode, B = 2nd param mode, A = 3rd param mode
// 1002,4,3,4,33 = addr(4)[33] * 3 = 99 stored at addr(4)
// parameters that an instruction write to will NEVER be in immediate mode
// instruction pointer increases by the number of values in the instruction(4 for the initial operations), 1 for the new ones
// integers can be negative if they're in immediate mode
namespace AdventOfCodeDay2
{
    public class Day2
    {
        public static void Main(string[] args)
        {
            string filePathDay2 = "D:\\Mykola\\Pictures\\inputDay2.txt";
            string filePathDay5 = "D:\\Mykola\\Pictures\\inputDay5.txt";
            string filePathDay7 = "D:\\Mykola\\Pictures\\inputDay7.txt";
            Day2FuncA(filePathDay2);
            Day2FuncB(filePathDay2);
            Day5FuncA(filePathDay5);
            Day5FuncB(filePathDay5);
            Day7FuncA(filePathDay7);
            Day7FuncB(filePathDay7);
            //string filePath = "D:\\Mykola\\Pictures\\inputDay2.txt";
            //FindVerbAndNoun(filePath, 19690720);
        }

        public static List<int> ParseInput(string filePath)
        {
            List<int> tempIntcode = new List<int>();
            string lines;
            string number = "";
            
            try
            {
                lines = File.ReadAllText(filePath);
                // string[] numbers = lines.Split(',');
                
                foreach(char letter in lines)
                {
                    if (letter == ',')
                    {
                        tempIntcode.Add(int.Parse(number));
                        number = "";
                    }
                    else
                    {
                        number += letter;
                    }
                }
                tempIntcode.Add(int.Parse(number));
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return tempIntcode;
        }

        public static void ResetMemory(List<int> intcode)
        {
            intcode.Clear();
        }

        public static bool OutputChecker(List<int> intcode, int output)
        {
            return intcode[0] == output;
        }

        public static void FindVerbAndNoun(string filePath, int output)
        {
            List<int> memory = ParseInput(filePath);

            for (int i = 0; i <= 1000000000; i++)
            {
                for (int noun = 0; noun <= 99; noun++)
                {
                    for (int verb = 0; verb <= 99; verb++)
                    {
                        List<int> intcode = new List<int>(memory);
                        intcode[1] = noun;
                        intcode[2] = verb;
                        Emulator.Emulator emulator = new Emulator.Emulator(intcode);
                        intcode = emulator.IterateThroughIntcode();
                        if (OutputChecker(intcode, output))
                        {
                            Console.WriteLine(100 * noun + verb);
                            return;
                        }
                        ResetMemory(intcode);
                    }
                }
            }

            Console.WriteLine("No noun and verb found for finding the output.");
        }

        public static void Day2FuncA(string filePath)
        {
            List<int> intcode = ParseInput(filePath);
            Queue<int> input = new Queue<int>();
            Queue<int> output = new Queue<int>();
            input.Enqueue(5);
            Emulator.Emulator emulator = new Emulator.Emulator(intcode);
            intcode = emulator.IterateThroughIntcode();
            Console.WriteLine(intcode[0]);
        }

        public static void Day2FuncB(string filePath)
        {
            FindVerbAndNoun(filePath, 19690720);
        }

        public static void Day5FuncA(string filePath)
        {
            List<int> intcode = ParseInput(filePath);
            Queue<int> output = new Queue<int>();
            Emulator.Emulator emulator = new Emulator.Emulator(intcode,1);
            intcode = emulator.IterateThroughIntcode();
            output = emulator.ReadOutputs();
            while (output.Count != 0)
            {
                Console.WriteLine(output.Dequeue());
            }
        }

        public static void Day5FuncB(string filePath)
        {
            List<int> intcode = ParseInput(filePath);
            Queue<int> output = new Queue<int>();
            Emulator.Emulator emulator = new Emulator.Emulator(intcode, 5);
            intcode = emulator.IterateThroughIntcode();
            output = emulator.ReadOutputs();
            while (output.Count != 0)
            {
                Console.WriteLine(output.Dequeue());
            }
        }

        public static void Day7FuncA(string filePath)
        {
            List<string> ampPhaseSettings = new List<string>();
            GetPermutations("01234", ampPhaseSettings, 0, 4);
            List<int> memory = ParseInput(filePath);
            List<int> intcode = new List<int>(memory);
            int highestThruster = int.MinValue;

            foreach (string ampPhaseSetting in ampPhaseSettings)
            {
                Queue<int> inputs = new Queue<int>();
                inputs.Enqueue(0);
                Emulator.Emulator amplifierA = new Emulator.Emulator(intcode, int.Parse(ampPhaseSetting[0].ToString()));
                amplifierA.Inputs = inputs;
                amplifierA.IterateThroughIntcode();

                Emulator.Emulator amplifierB = new Emulator.Emulator(intcode, int.Parse(ampPhaseSetting[1].ToString()));
                amplifierB.Inputs = amplifierA.ReadOutputs();
                amplifierB.IterateThroughIntcode();

                Emulator.Emulator amplifierC = new Emulator.Emulator(intcode, int.Parse(ampPhaseSetting[2].ToString()));
                amplifierC.Inputs = amplifierB.ReadOutputs();
                amplifierC.IterateThroughIntcode();

                Emulator.Emulator amplifierD = new Emulator.Emulator(intcode, int.Parse(ampPhaseSetting[3].ToString()));
                amplifierD.Inputs = amplifierC.ReadOutputs();
                amplifierD.IterateThroughIntcode();

                Emulator.Emulator amplifierE = new Emulator.Emulator(intcode, int.Parse(ampPhaseSetting[4].ToString()));
                amplifierE.Inputs = amplifierD.ReadOutputs();
                amplifierE.IterateThroughIntcode();
                int thrusterValue = amplifierE.ReadOutputs().Dequeue();
                
                if (thrusterValue > highestThruster)
                {
                    highestThruster = thrusterValue;
                }
            }

            Console.WriteLine("Highest thruster: " + highestThruster);
        }

        public static void Day7FuncB(string filePath)
        {
            List<string> ampPhaseSettings = new List<string>();
            GetPermutations("56789", ampPhaseSettings, 0, 4);
            List<int> memory = ParseInput(filePath);
            List<int> intcode = new List<int>(memory);
            int highestThruster = int.MinValue;

            foreach (string ampPhaseSetting in ampPhaseSettings)
            {
                Emulator.Emulator amplifierA = new Emulator.Emulator(intcode, int.Parse(ampPhaseSetting[0].ToString()));
                Emulator.Emulator amplifierB = new Emulator.Emulator(intcode, int.Parse(ampPhaseSetting[1].ToString()));
                Emulator.Emulator amplifierC = new Emulator.Emulator(intcode, int.Parse(ampPhaseSetting[2].ToString()));
                Emulator.Emulator amplifierD = new Emulator.Emulator(intcode, int.Parse(ampPhaseSetting[3].ToString()));
                Emulator.Emulator amplifierE = new Emulator.Emulator(intcode, int.Parse(ampPhaseSetting[4].ToString()));
                Queue<int> inputs = new Queue<int>();
                inputs.Enqueue(0);
                amplifierA.Inputs = inputs;
                do
                {
                    amplifierA.IterateThroughIntcode();

                    amplifierB.Inputs = amplifierA.ReadOutputs();
                    amplifierB.IterateThroughIntcode();

                    amplifierC.Inputs = amplifierB.ReadOutputs();
                    amplifierC.IterateThroughIntcode();

                    amplifierD.Inputs = amplifierC.ReadOutputs();
                    amplifierD.IterateThroughIntcode();

                    amplifierE.Inputs = amplifierD.ReadOutputs();
                    amplifierE.IterateThroughIntcode();
                    amplifierA.Inputs = amplifierE.ReadOutputs();
                } while (amplifierE.EmulatorState != Emulator.Emulator.State.Finished);
                
                int thrusterValue = amplifierE.ReadOutputs().Dequeue();

                if (thrusterValue > highestThruster)
                {
                    highestThruster = thrusterValue;
                }
            }

            Console.WriteLine("Highest thruster: " + highestThruster);
        }

        public static void GetPermutations(string permutation, List<string> permutations, int leftIndex, int rightIndex)
        {
            if (leftIndex == rightIndex)
            {
                permutations.Add(permutation);
            }
            else
            {
                for (int i = leftIndex; i <= rightIndex; i++)
                {
                    permutation = Swap(permutation, leftIndex, i);
                    GetPermutations(permutation, permutations, leftIndex + 1, rightIndex);
                    permutation = Swap(permutation, leftIndex, i);
                }
            }
        }

        public static string Swap(string a, int i, int j)
        {
            char temp;
            char[] charArray = a.ToCharArray();
            temp = charArray[i];
            charArray[i] = charArray[j];
            charArray[j] = temp;
            string s = new string(charArray);
            return s;
        }
    }
}