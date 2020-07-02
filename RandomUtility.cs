using System;
using System.Collections.Generic;

namespace _2048_Solver
{
    public static class RandomUtility
    {
        private static Random rnd = new Random();

        public static T PickOne<T>(IList<T> items)
        {
            if (items.Count == 0)
                throw new Exception("Empty items list");

            int index = rnd.Next(items.Count);

            return items[index];
        }

        public static T PickOneAndRemove<T>(IList<T> items)
        {
            T item = PickOne(items);
            items.Remove(item);
            return item;
        }

        public static double Between(double min, double max)
        {
            return rnd.Next() * (max - min) + min;
        }
    }
}
