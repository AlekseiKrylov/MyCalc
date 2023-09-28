namespace MyCalc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter a formula or path to a file containing formulas (one line, one formula).");
            Console.WriteLine("The formula must not contain spaces but may include round brackets.");
            Console.WriteLine("Example: -2+2*3+(4.5-2/0.5)");
            Console.WriteLine();

            do
            {
                Console.Write("Formula or file path: ");
                string input = Console.ReadLine();

                try
                {
                    MyCalc calc = new(input);
                    Console.WriteLine(calc.GetResult());
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to calculate anoter formula. Press Esc to exit.");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;
            } while (true);
        }
    }
}