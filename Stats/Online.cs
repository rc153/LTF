using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Indicators;

namespace Toolkit.Stats
{
    // todo windowed as well
    public class OnlineUnivariateStat : IResetable
    {
        uint n = 0;
        double mean = 0;
        double M2 = 0;

        public void Add(double x)
        {
            n++;
            double delta = x - mean;
            mean += delta / n;
            M2 += delta * (x - mean);
        }

        public void Reset()
        {
            n = 0;
            mean = 0;
            M2 = 0;
        }

        public double Mean
        {
            get { return mean; }
        }

        public double Variance
        {
            get { return M2 / n; }
        }

        public double StandardDeviation
        {
            get { return Math.Sqrt(Variance); }
        }

        public double GetZScore(double x)
        {
            return (x - Mean) / StandardDeviation;
        }
    }

    // There is a cool approximate Theil-Sen online version (cf "Deterministic Sampling and Range Counting in Geometric Data Streams")
    // todo see also here for other algos http://en.wikipedia.org/wiki/Streaming_algorithm
    // and http://code.google.com/p/szl/source/browse/#svn%2Ftrunk%2Fsrc%2Femitters%253Fstate%253Dclosed
    public class OnlineBivariateStat : IResetable
    {
        uint n = 0;
        double meanX = 0;
        double meanY = 0;
        double M2X = 0;
        double M2Y = 0;
        double M2XY = 0;

        public void Add(double x, double y)
        {
            n++;
            double nInv = 1.0 / n;
            double deltaX = x - meanX;
            meanX += nInv * deltaX;
            M2X += deltaX * (x - meanX);
            double deltaY = y - meanY;
            meanY += nInv * deltaY;
            M2Y += deltaY * (y - meanY);
            M2XY += (deltaX) * (y - meanY);
        }

        public void Reset()
        {
            n = 0;
            meanX = 0;
            meanY = 0;
            M2X = 0;
            M2Y = 0;
            M2XY = 0;
        }

        public double MeanX
        {
            get { return meanX; }
        }

        public double MeanY
        {
            get { return meanY; }
        }

        public double VarianceX
        {
            get { return M2X / n; }
        }

        public double VarianceY
        {
            get { return M2Y / n; }
        }

        public double StandardDeviationX
        {
            get { return Math.Sqrt(VarianceX); }
        }

        public double StandardDeviationY
        {
            get { return Math.Sqrt(VarianceY); }
        }

        public double Covariance
        {
            get { return M2XY / n; }
        }

        public double Correlation
        {
            get { return Covariance / Math.Sqrt(VarianceX * VarianceY); }
        }

        public double BetaX     // residuals are measured verticaly
        {
            get { return Covariance / VarianceX; }
        }

        public double BetaY     // residuals are measured horizontaly
        {
            get { return VarianceY / Covariance; }
        }

        public double BetaPerp  // residuals are measured perpendicularly (total least square)
        {
            get
            {
                double r = (VarianceY - VarianceX) / Covariance;
                double result = r - Math.Sqrt(r * r + 4);
                if (result * Covariance > 0) return result / 2;
                return (r + Math.Sqrt(r * r + 4)) / 2;
            }
        }

        public double BetaTri  // residuals are measured as tha area of the triangle (reduced major axis)
        {
            get { return Math.Sign(Covariance) * Math.Sqrt(VarianceY / VarianceX); }
        }

        public double getAlpha(double beta)
        {
            return MeanY - beta * MeanX;
        }

    }
}
