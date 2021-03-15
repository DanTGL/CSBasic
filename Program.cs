using System;

namespace CSBasic
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("Please enter file path argument.");
                return -1;
            }

            Interpreter interpreter = new Interpreter();
            interpreter.LoadFile(args[0]);
            interpreter.Execute();

            return 0;
        }
    }
}
