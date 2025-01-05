using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace VeggieMarketDataProcessor
{
    public static class Interpolation
    {
        public static void LinSpaceArray(ref double?[] arr)
        {
            if (IsEmptyArray(arr)) return;

            int i = 0;
            bool foundAtLeastOneNonNull = false;
            while (i < arr.Length)
            {
                if (!arr[i].HasValue)
                {
                    if (!foundAtLeastOneNonNull)
                    {
                        arr[i] = FindFirstNonNullValue(arr, i + 1);
                    } 
                    else if (i == arr.Length - 1)
                    {
                        arr[i] = FindLastNonNullValue(arr, i);
                    }
                    else
                    {
                        int startIndex = (i == 0) ? 0 : i - 1;
                        int j = i + 1;
                        while (j < arr.Length && !arr[j].HasValue)
                        {
                            j++;
                        }

                        if (j == arr.Length)
                        {
                            double? lastValue = FindLastNonNullValue(arr, j - 1);
                            ReplaceAllValuesStartingFromIndex(ref arr, i, j - 1, lastValue);
                        }
                        else
                        {
                            int endIndex = j;
                            int numberOfPoints = endIndex - startIndex;
                            Vector3D startPoint = new Vector3D(arr[startIndex].Value, 0, 0);
                            Vector3D endPoint = new Vector3D(arr[endIndex].Value, 0, 0);
                            double[] linspace = Vector3DToDoubleArray(LinSpace(startPoint, endPoint, numberOfPoints).ToArray());
                            ReplaceRange(ref arr, linspace, startIndex + 1, endIndex - 1);
                            i = j + 1;
                        }
                    }
                }
                else
                {
                    foundAtLeastOneNonNull = true;
                    i++;
                }
            }
        }

        private static bool IsEmptyArray(double?[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].HasValue) return false;
            }
            return true;
        }

        private static double? FindFirstNonNullValue(double?[] arr, int startIndex)
        {
            for (int i = startIndex; i < arr.Length; i++)
            {
                if (arr[i].HasValue) return arr[i].Value;
            }
            return null;
        }

        private static double? FindLastNonNullValue(double?[] arr, int endIndex)
        {
            for (int i = endIndex; i >= 0; i--)
            {
                if (arr[i].HasValue) return arr[i].Value;
            }
            return null;
        }

        private static void ReplaceAllValuesStartingFromIndex(ref double?[] arr, int startIndex, int endIndex, double? value)
        {
            if (!value.HasValue) return;
            for (int i = startIndex; i <= endIndex; i++)
            {
                arr[i] = value;
            }
        }

        private static double[] Vector3DToDoubleArray(Vector3D[] vector3Ds)
        {
            double[] arr = new double[vector3Ds.Length];
            for (int i = 0; i < vector3Ds.Length; i++)
            {
                arr[i] = vector3Ds[i].X;
            }
            return arr;
        }

        private static void ReplaceRange(ref double?[] arr, double[] range, int startIndex, int endIndex)
        {
            int index = startIndex;
            for (int i = 1; i < range.Length - 1; i++)
            {
                arr[index] = range[i];
                index++;
            }
        }

        private static IEnumerable<Vector3D> LinSpace(Vector3D start, Vector3D end, int partitions) =>
        Enumerable.Range(0, partitions + 1)
        .Select(idx => idx != partitions
                ? start + (end - start) / partitions * idx
                : end);
    }
}
