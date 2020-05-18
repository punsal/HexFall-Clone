using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utility.Custom
{
    public static class Math
    {
        public static List<int> Combination(int setCount, int subsetCount)
        {
            var result = new List<int>();
            try
            {
                setCount.IsZero();
                subsetCount.IsZero();
                setCount.IsGreaterThanOrEqualsTo(subsetCount);
            }
#pragma warning disable 168
            catch (ArgumentException argumentException)
#pragma warning restore 168
            {
                Debug.LogError("Take a look at arguments. Something is zero(0).");
            }
#pragma warning disable 168
            catch (ArithmeticException arithmeticException)
#pragma warning restore 168
            {
                Debug.LogError($"'subsetCount({subsetCount})' is greater than 'setCount{setCount}'.");
                subsetCount = setCount;
            }

            finally
            {
                var items = new List<int>();
                for (var i = 0; i < setCount; i++) items.Add(i);

                for (var i = 0; i < subsetCount; i++)
                {
                    var randomIndex = Random.Range(0, items.Count);
                    result.Add(items[randomIndex]);
                    items.RemoveAt(randomIndex);
                }
            }

            return result;
        }

        public static bool IsZero(this int number)
        {
            return number == 0 ? throw new ArgumentException() : true;
        }

        public static bool IsGreaterThan(this int compared, int comparison)
        {
            return compared <= comparison ? throw new ArithmeticException() : true;
        }

        public static bool IsGreaterThanOrEqualsTo(this int compared, int comparison)
        {
            return compared < comparison ? throw new ArithmeticException() : true;
        }
    }
}