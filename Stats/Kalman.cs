using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace Toolkit.Stats
{
    // measurement
    //  y_t = Z*a_t + eps_t with E(eps_t)=0 & Var(eps_t)=H_t
    // state
    //  a_t = T*a_t-1 + nu_t with E(nu_t)=0 & Var(nu_t)=Q_t

    // todo other simplifications like all constant to ewma
    public class KalmanUnivariate
    {
        private Vector<float> a;    // State
        private Matrix<float> P;    // Covariance of the estimation error
        private Matrix<float> T;    // Rolls the state forward
        private Vector<float> Z;    // Rolls actual to predicted
        private Matrix<float> Q;    // Covariance of the noise in the state process
        private float H;            // Covariance of the noise in the measurement

        public Vector<float> State { get { return a; } }
        public float Prediction { get { return Z * a; } }

        public void Predict()
        {
            a = T * a;                           // Rolls state forward in time
            P = T * P * T.Transpose() + Q;       // Rolls the uncertainty forward in time
        }

        public void Update(float y)
        {
            float F = Z * P * Z + H;          // Residual covariance
            Vector<float> K = P * Z / F;      // Kalman gain = variance / residual covariance
            float Y = y - Z * a;              // Innovation = measurement – state
            a = a + K * Y;                    // Update with gain the new measurement
            P = P - K * Z * P;                // Update covariance to this time
        }

        public static KalmanUnivariate Build(Vector<float> a0, Matrix<float> P0, Matrix<float> T, Vector<float> Z, Matrix<float> Q, float H)
        {
            return new KalmanUnivariate() { a = a0, P = P0, T = T, Z = Z, Q = Q, H = H };
        }

        // a is [position, speed] 
        public static KalmanUnivariate BuildPositionSpeed(float positionVar, float accVar)
        {
            return new KalmanUnivariate()
            {
                a = new DenseVector(new float[] { 0, 0 }),
                P = new DenseMatrix(new float[,] { { positionVar * 100, 0 }, { 0, positionVar * 100 } }),
                T = new DenseMatrix(new float[,] { { 1, 1 }, { 0, 1 } }),
                Z = new DenseVector(new float[] { 1, 0 }),
                Q = new DenseMatrix(new float[,] { { 1 / 4f, 1 / 2f }, { 1 / 2f, 1 } }) * accVar,
                H = positionVar
            };
        }

        // a is [position, speed, acc]
        public static KalmanUnivariate BuildPositionSpeedAcc(float positionVar, float accVar)
        {
            return new KalmanUnivariate()
            {
                a = new DenseVector(new float[] { 0, 0, 0 }),
                P = new DenseMatrix(new float[,] { { positionVar * 100, 0, 0 }, { 0, positionVar * 100, 0 }, { 0, 0, positionVar * 100 } }),
                T = new DenseMatrix(new float[,] { { 1, 1, 0.5f }, { 0, 1, 1 }, { 0, 0, 1 } }),
                Z = new DenseVector(new float[] { 1, 0, 0 }),
                Q = new DenseMatrix(new float[,] { { 1 / 36f, 1 / 12f, 1 / 6f }, { 1 / 12f, 1 / 4f, 1 / 2f }, { 1 / 6f, 1 / 2f, 1 } }) * accVar,
                H = positionVar
            };
        }
    }

    public class KalmanMultivariate
    {
        private Vector<float> a;    // State
        private Matrix<float> P;    // Covariance of the estimation error
        private Matrix<float> T;    // Rolls the state forward
        private Matrix<float> Z;    // Rolls actual to predicted
        private Matrix<float> Q;    // Covariance of the noise in the state process
        private Matrix<float> H;    // Covariance of the noise in the measurement

        public Vector<float> State { get { return a; } }
        public Vector<float> Prediction { get { return Z * a; } }

        public void Predict()
        {
            a = T * a;                           // Rolls state forward in time
            P = T * P * T.Transpose() + Q;       // Rolls the uncertainty forward in time
        }

        public void Update(Vector<float> y)
        {
            Matrix<float> F = Z * P * Z.Transpose() + H;            // Residual covariance
            Matrix<float> K = P * Z.Transpose() * F.Inverse();      // Kalman gain = variance / residual covariance
            Vector<float> Y = y - Z * a;                            // Innovation = measurement – state
            a = a + K * Y;                                          // Update with gain the new measurement
            P = P - K * Z * P;                                      // Update covariance to this time
        }

        public static KalmanMultivariate Build(Vector<float> a0, Matrix<float> P0, Matrix<float> T, Matrix<float> Z, Matrix<float> Q, Matrix<float> H)
        {
            return new KalmanMultivariate() { a = a0, P = P0, T = T, Z = Z, Q = Q, H = H };
        }
    }
}
