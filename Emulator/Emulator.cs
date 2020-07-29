using System;
using System.Collections.Generic;

namespace Emulator
{
    public class Emulator
    {
        public enum State
        {
            Ready,
            Running,
            Finished,
            Blocking
        }

        enum RunningMode
        {
            Nonblocking,
            Blocking
        }

        private Queue<int> inputs;
        private RunningMode emulatorRunningMode;
        private Queue<int> outputs;
        private State emulatorState;
        private int phaseSetting = 0;
        private int programRegister = 0;
        private List<int> intcode;
        private int AValue, BValue, CValue;
        private int aParamMode = 0, bParamMode = 0, cParamMode = 0;
        private bool ampPhaseSettingBeenRead = false;
        private int operation = 0;

        public State EmulatorState
        {
            get { return emulatorState; }
            set { emulatorState = value; }
        }

        public Queue<int> Inputs
        {
            set { inputs = value; }
        }
        
        public Emulator(List<int> intcode)
        {
            inputs = null;
            outputs = null;
            emulatorState = State.Ready;
            emulatorRunningMode = RunningMode.Nonblocking;
            programRegister = 0;
            this.intcode = intcode;
        }

        public Emulator(List<int> intcode, int phaseSetting)
        {
            inputs =  new Queue<int>();
            outputs = new Queue<int>();
            emulatorState = State.Ready;
            emulatorRunningMode = RunningMode.Nonblocking;
            programRegister = 0;
            this.intcode = intcode;
            this.phaseSetting = phaseSetting;
        }

        public Emulator(List<int> intcode, Queue<int> inputs, int runMode)
        {
            this.inputs = inputs;
            outputs = null;
            emulatorState = State.Ready;
            emulatorRunningMode = (RunningMode)runMode;
            phaseSetting = 0;
            programRegister = 0;
            this.intcode = intcode;
        }

        public Emulator(List<int> intcode, Queue<int> inputs, int runMode, int phaseSetting)
        {
            this.inputs = inputs;
            outputs = null;
            emulatorState = State.Ready;
            emulatorRunningMode = (RunningMode)runMode;
            this.phaseSetting = phaseSetting;
            programRegister = 0;
            this.intcode = intcode;
        }

        public Queue<int> ReadOutputs()
        {
            return outputs;
        }

        private void SetParameters()
        {
            if (aParamMode == 0)
            {

            }
            AValue = intcode[programRegister + 2];

            if (bParamMode == 0)
            {
                BValue = intcode[programRegister + 1];
            }
            else
            {
                BValue = programRegister + 1;
            }

            if (cParamMode == 0)
            {
                CValue = intcode[programRegister];
            }
            else
            {
                CValue = programRegister;
            }
        }

        public List<int> IterateThroughIntcode()
        {
            while (programRegister < intcode.Count)
            {
                this.emulatorState = State.Running;
                if (intcode[programRegister] >= 10000)
                {
                    aParamMode = intcode[programRegister] / 10000;
                    int remainder = intcode[programRegister] % 10000;
                    bParamMode = remainder / 1000;
                    remainder %= 1000;
                    cParamMode = remainder / 100;
                    operation = remainder % 100;
                }
                else if (intcode[programRegister] >= 1000)
                {
                    aParamMode = 0;
                    bParamMode = intcode[programRegister] / 1000;
                    int remainder = intcode[programRegister] % 1000;
                    cParamMode = remainder / 100;
                    operation = remainder % 100;
                }
                else if (intcode[programRegister] >= 100)
                {
                    aParamMode = 0;
                    bParamMode = 0;
                    cParamMode = intcode[programRegister] / 100;
                    operation = intcode[programRegister] % 100;
                }
                else
                {
                    aParamMode = 0;
                    bParamMode = 0;
                    cParamMode = 0;
                    operation = intcode[programRegister];
                }
                /*operation = int.Parse(textKeycode.Substring(0, 2)); // this one should always exist
                cParamMode = int.Parse(textKeycode.Substring(2,1) ?? "0"); 
                bParamMode = int.Parse(textKeycode.Substring(3,1) ?? "0");
                aParamMode = int.Parse(textKeycode.Substring(4,1) ?? "0");*/
                programRegister += 1;
                switch (operation)
                {
                    case (int)Operation.Add:
                        //c = 1st parameter
                        //b = 2nd parameter
                        //a = 3rd parameter
                        //Add things together
                        SetParameters();

                        intcode[AValue] = AddValues(BValue, CValue);
                        programRegister += 3;
                        break;
                    case (int)Operation.Multiply:
                        //multiply things together
                        SetParameters();

                        intcode[AValue] = MultiplyValues(BValue, CValue);
                        programRegister += 3;
                        break;
                    case (int)Operation.Input:
                        //take a single integer as input and saves it to the position given by its only paramter
                        //i.e, 3,50 = take an input value and store it at address 50
                        CValue = intcode[programRegister];
                        if (inputs.Count == 0 && ampPhaseSettingBeenRead)
                        {
                            programRegister -= 1;
                            emulatorState = State.Blocking;
                            return intcode;
                        }
                        else if (ampPhaseSettingBeenRead)
                        {
                            intcode[CValue] = inputs.Dequeue();
                        }
                        else
                        {
                            intcode[CValue] = phaseSetting;
                            ampPhaseSettingBeenRead = true;
                        }
                        //intcode[cParam] = InputValue();
                        programRegister += 1;
                        break;
                    case (int)Operation.Output:
                        //outputs the value of its only parameter.
                        //i.e, 4,50 = output the value at address 50
                        if (cParamMode == 0)
                        {
                            CValue = intcode[programRegister];
                        }
                        else
                        {
                            CValue = programRegister;
                        }

                        outputs.Enqueue(OutputValue(CValue));
                        programRegister += 1;
                        break;
                    case (int)Operation.JumpIfNotZero:
                        //if true, move pointer
                        //otherwise, increment pointer
                        //jump if true
                        //if first param is not zero, sets instruction pointer to value from second param
                        SetParameters();

                        if (intcode[CValue] != 0)
                        {
                            programRegister = intcode[BValue];
                        }
                        else
                        {
                            programRegister += 2;
                        }
                        break;
                    case (int)Operation.JumpIfZero:
                        //if first param is zero, sets instruction pointer to value from second param
                        SetParameters();

                        if (intcode[CValue] == 0)
                        {
                            programRegister = intcode[BValue];
                        }
                        else
                        {
                            programRegister += 2;
                        }
                        break;
                    case (int)Operation.IsLessThan:
                        //if the first param is less than the second param, store 1 in position given by third param
                        //otherwise store 0 in position of third param
                        SetParameters();

                        if (intcode[CValue] < intcode[BValue])
                        {
                            intcode[AValue] = 1;
                        }
                        else
                        {
                            intcode[AValue] = 0;
                        }

                        programRegister += 3;
                        break;
                    case (int)Operation.IsEqualTo:
                        //if the first param equals the second param, store 1 in position given by third param
                        //otherwise store 0 in position of third param
                        //SetParams
                        SetParameters();

                        if (intcode[CValue] == intcode[BValue])
                        {
                            intcode[AValue] = 1;
                        }
                        else
                        {
                            intcode[AValue] = 0;
                        }

                        programRegister += 3;
                        break;
                    case (int)Operation.Finished:
                        //break out of do while loop
                        emulatorState = State.Finished;
                        return intcode;
                    default:
                        break;
                }
            };
            emulatorState = State.Finished;
            return intcode;
        }

        private int AddValues(int num1, int num2)
        {
            return intcode[num2] + intcode[num1];
        }

        //This function needs a unit test, seems to be working though
        private int OutputValue(int location)
        {
            return intcode[location];
        }

        private int MultiplyValues(int num1, int num2)
        {
            return intcode[num1] * intcode[num2];
        }

        //This function needs a unit test
        private static int InputValue()
        {
            Console.Write("Please enter a value: ");
            int value = int.Parse(Console.ReadLine());
            return value;
        }
    }
}
