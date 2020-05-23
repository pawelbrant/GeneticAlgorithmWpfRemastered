using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class GA
    {
        public BitArray[,,] chromosomes { get; set; }

        public static int binary2Int(BitArray bitArray)
        {
            int result=0;
            for(int index=0; index<bitArray.Length; index++)
            {
                int value = bitArray[index] ? 1 : 0;
                result += (int)Math.Pow(2, bitArray.Length - 1 - index) * value;
            }
            return result;
        }

        public static BitArray int2Binary(int value)
        {
            return new BitArray(new int[] { value });
        }

        public static int generateRandomValue(int precision)
        {
            Random random = new Random();
            return random.Next(0, (int)Math.Pow(2, precision));
        }
    }
}
