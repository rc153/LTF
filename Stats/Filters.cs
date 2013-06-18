using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Stats
{

    // http://www.statistics4u.info/fundstat_eng/cc_filter_savgolay.html
    // todo see parag 13 of the art for the convolv function to apply this
    public class SavitzkyGolay
    {
        public static double[] getCoefficients(int nbLeft, int nbRight, int degree, int deriv = 0)
        {
            if (nbLeft < 0) throw new ArgumentException("nbLeft < 0");
            if (nbRight < 0) throw new ArgumentException("nbRight < 0");
            if (deriv > degree) throw new ArgumentException("deriv > degree");
            if (nbLeft + nbRight < degree) throw new ArgumentException("nbLeft + nbRight < degree");
            Contract.EndContractBlock();

            int nbPoints = nbLeft + nbRight + 1;
            int nbPoly = degree + 1;

            // A is the model matrix
            // setup the normal equations A' * A
            DenseMatrix ata = new DenseMatrix(nbPoly, nbPoly);
            for (int ipj = 0; ipj <= (degree << 1); ipj++)
            {
                double sum = (ipj != 0 ? 0.0 : 1.0);
                for (int k = 1; k <= nbRight; k++) sum += Math.Pow((double)k, (double)ipj);
                for (int k = 1; k <= nbLeft; k++) sum += Math.Pow((double)-k, (double)ipj);
                int mm = Math.Min(ipj, 2 * degree - ipj);
                for (int imj = -mm; imj <= mm; imj += 2) ata[(ipj + imj) / 2, (ipj - imj) / 2] = sum;
            }

            // solve
            DenseLU lu = new DenseLU(ata);
            Vector<double> b = new DenseVector(nbPoly, 0);
            b[deriv] = 1;
            b = lu.Solve(b);

            // Each Savitzky-Golay coefficient is the dot product of powers of an integer with the inverse matrix row
            DenseVector result = new DenseVector(nbPoints);
            double factorial = SpecialFunctions.Factorial(deriv);
            for (int k = -nbLeft; k <= nbRight; k++)
            {
                double sum = b[0];
                double fac = 1.0;
                for (int mm = 1; mm <= degree; mm++) sum += b[mm] * (fac *= k);
                int kk = k + nbLeft;//((nbPoints - k) % nbPoints);
                if (deriv > 1) sum *= factorial;
                result[kk] = sum;
            }

            return result;
        }
    }
}
