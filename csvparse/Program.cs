using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;

namespace csvparse
{
    class Program
    {
        class InitialCostTotal
        {
            public double Sum { get; private set; }
            public long Count { get; private set; }
            public double Mean { get { return Sum / Count; } }

            public void Add(double cost)
            {
                Sum += cost;
                ++Count;
            }
            public void Add(InitialCostTotal costs)
            {
                Sum += costs.Sum;
                Count += costs.Count;
            }
        }

        static void Main(string[] args)
        {
            InitialCostTotal total = new InitialCostTotal();

            for (int i=0; i<args.Length; i++)
            {
                total.Add( FindCost(args[i]) );
            }
            Console.WriteLine("Sum: {0} Mean:{1}", total.Sum, total.Mean);

            //Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static InitialCostTotal FindCost(String path)
        {
            InitialCostTotal cost = new InitialCostTotal();

            using (TextFieldParser parser = new TextFieldParser(path))
            {
                parser.SetDelimiters(",");
                if (parser.EndOfData)
                {
                    throw new Exception("Empty file: "+path);
                }

                // Read header
                int costCol = -1;
                string[] fields = parser.ReadFields();
                for(int i=0; i<fields.Length; i++)
                {
                    if(fields[i].Equals("Cost, Initial"))
                    {
                        costCol = i;
                        break;
                    }
                }
                if (-1 == costCol)
                {
                    throw new Exception("File missing 'Cost, Initial' column: "+path);
                }

                while (!parser.EndOfData)
                {
                    fields = parser.ReadFields();
                    double result;
                    if(Double.TryParse(
                        fields[costCol].TrimStart('$', ' '), 
                        out result))
                    {
                        cost.Add(result);
                    }
                    else
                    {
                        string lineNum = (parser.LineNumber - 1) > 0 ? (parser.LineNumber - 1).ToString() : "last";
                        Debug.WriteLine("Error parsing line " + lineNum + " in file: " + path);
                    }
                }
            }

            return cost;
        }
    }
}
