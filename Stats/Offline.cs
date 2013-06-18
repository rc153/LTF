using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Indicators;

namespace Toolkit.Stats
{
    // todo tests
    public class OfflineUnivariateStat : IResetable
    {
        private bool sorted = false;
        private List<double> data = new List<double>();

        public void Add(IEnumerable<double> xs)
        {
            sorted = false;
            data.AddRange(xs);
        }

        public void Add(double x)
        {
            sorted = false;
            data.Add(x);
        }

        public void Reset()
        {
            sorted = false;
            data.Clear();
        }

        private void Sort()
        {
            if (!sorted)
            {
                data.Sort();
                sorted = true;
            }
        }

        // see introselect http://en.wikipedia.org/wiki/Selection_algorithm#Introselect and impl of the sort in arraysorthelper

        // replacing given parts of a sample at the high and low end with the most extreme remaining values
        public OfflineUnivariateStat Winsorize(double percent = 0.25)
        {
            OfflineUnivariateStat newStat = new OfflineUnivariateStat();
            int sizeToCut = (int)(data.Count * percent);
            for (int i = 0; i < sizeToCut; i++)
            {
                newStat.Add(data[sizeToCut]);
            }
            for (int i = sizeToCut; i < data.Count - sizeToCut; i++)
            {
                newStat.Add(data[i]);
            }
            for (int i = data.Count - sizeToCut; i < data.Count; i++)
            {
                newStat.Add(data[data.Count - sizeToCut - 1]);
            }
            return newStat;
        }

        // remove parts of a sample at the high and low end
        public OfflineUnivariateStat Trim(double percent = 0.25)
        {
            OfflineUnivariateStat newStat = new OfflineUnivariateStat();
            int sizeToCut = (int)(data.Count * percent);
            for (int i = sizeToCut; i < data.Count - sizeToCut; i++)
            {
                newStat.Add(data[i]);
            }
            return newStat;
        }

        // *** central tendency ***
        // todo quadratic mean and other stuff listed in http://en.wikipedia.org/wiki/Category:Means

        // breakdown point: 0
        public double Mean
        {
            get
            {
                return data.Average();
            }
        }

        // breakdown point: 0.5
        public double Median
        {
            get
            {
                Sort();
                return data.Count % 2 == 1 ? data[data.Count / 2] : 0.5 * (data[data.Count / 2] + data[data.Count / 2 - 1]);
            }
        }

        // breakdown point: 0.29
        public double GetHodgesLehmann()
        {
            OfflineUnivariateStat newStat = new OfflineUnivariateStat();
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = i; j < data.Count; j++)
                {
                    newStat.Add(data[i] + data[j]);
                }
            }
            return 0.5 * newStat.Median;
        }

        // quantize so that Q1 ~ Q3 is 10 buckets, and find the bucket with the most values in it
        // then recurse if there's still a lot of points in our largest bucket
        // if there are 2 possible answer, we return the lowest one
        public double GetMode()
        {
            double bucketSize = 0.1 * (GetQuantile(.75) - GetQuantile(.25));
            if (bucketSize <= 0) return Median;
            double result = 0;
            int maxNbPoints = 0;
            int i = 0;
            int oldI = 0;
            for (double d = Minimum + bucketSize; i < data.Count; d += bucketSize)
            {
                while (i < data.Count && data[i] < d) { i++; }
                if (i - oldI > maxNbPoints) { maxNbPoints = i - oldI; result = d - bucketSize / 2; }
                oldI = i;
            }

            OfflineUnivariateStat newStat = new OfflineUnivariateStat();
            for (int j = 0; j < data.Count; j++)
            {
                if (data[j] > result + 3 / 2 * bucketSize)
                    break;
                if (data[j] >= result - 3 / 2 * bucketSize)
                    newStat.Add(data[j]);
            }

            if (maxNbPoints > 1000)
                return newStat.GetMode();
            else
                return newStat.Median;
        }

        // mean of values between Q1 and Q3
        // breakdown point: 0.25
        public double InterquartileMean { get { return GetTruncatedMean(0.25); } }

        // breakdown point: percent
        public double GetWinsorizedMean(double percent = 0.25) { return Winsorize(percent).Mean; }

        // breakdown point: percent
        public double GetTruncatedMean(double percent = 0.25) { return Trim(percent).Mean; }

        // breakdown point: 0.25
        // 0.25* (Q1 + 2Q2 + Q3)
        public double TriMean { get { return 0.25 * (GetQuantile(.25) + 2 * Median + GetQuantile(.75)); } }

        // breakdown point: 0.25
        // 0.5 * (Q1 + Q3)
        public double MidHinge { get { return 0.5 * (GetQuantile(.25) + GetQuantile(.75)); } }

        // breakdown point: 0
        // 0.5 * (min + max)
        public double MidRange { get { return 0.5 * (Minimum + Maximum); } }

        // 2nd mom
        // the range, the interquartile range, the mean absolute deviation, and the median absolute deviation

        public double Variance
        {
            get
            {
                double sum1 = 0;
                foreach (double x in data)
                {
                    sum1 += x;
                }
                double mean = sum1 / data.Count;

                double sum2 = 0;
                double sum3 = 0;
                foreach (double x in data)
                {
                    double deltaX = x - mean;
                    sum2 = sum2 + deltaX * deltaX;
                    sum3 = sum3 + deltaX;
                }

                return (sum2 - sum3 * sum3 / data.Count) / (data.Count - 1);
            }
        }

        public double StandardDeviation
        {
            get
            {
                return Math.Sqrt(Variance);
            }
        }

        public double Minimum
        {
            get
            {
                Sort();
                return data[0];
            }
        }

        public double Maximum
        {
            get
            {
                Sort();
                return data[data.Count - 1];
            }
        }

        // todo could overflow + everage 2 values if we are not spot on one
        public double GetQuantile(double percent)
        {
            Sort();
            return data[(int)(data.Count * percent)];
        }
    }

    public class OfflineBivariateStat : IResetable
    {
        private List<double> dataX = new List<double>();
        private List<double> dataY = new List<double>();

        // todo different reg methods (find them back...)
        // todo Theil–Sen estimator & variants http://en.wikipedia.org/wiki/Theil%E2%80%93Sen_estimator (copy breakdown points)
        // and models on the right here: http://en.wikipedia.org/wiki/Regression_analysis

        public void Add(double x, double y)
        {
            dataX.Add(x);
            dataY.Add(y);
        }

        public void Add(IEnumerable<double> xs, IEnumerable<double> ys)
        {
            dataX.AddRange(xs);
            dataY.AddRange(xs);
        }

        public void Reset()
        {
            //sorted = false;
            dataX.Clear();
            dataY.Clear();
        }

        // breakdown: 29.3%
        // this is the dumb O(n^2) version, there are n log n versions
        double BetaTheilSen
        {
            get
            {
                OfflineUnivariateStat slopes = new OfflineUnivariateStat();
                int n = dataX.Count;
                for (int i = 0; i < n; i++)
                    for (int j = i + 1; j < n; j++)
                        slopes.Add((dataY[j] - dataY[i]) / (dataX[j] - dataX[i]));
                return slopes.Median;
            }
        }
    }

}
