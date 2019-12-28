using System.Collections;
using System.Collections.Generic;


namespace Shooter.Core
{
    public static class Utility
    {
        public static T[] ShuffleArray<T>(T[] array, int seed)
        {
            System.Random randomNumGen = new System.Random(seed);
            for (int i = 0; i < array.Length - 1; i++)
            {
                int randIndex = randomNumGen.Next(i, array.Length);
                T tempItem = array[randIndex];
                array[randIndex] = array[i];
                array[i] = tempItem;
            }
            return array;
        }
    }
}
