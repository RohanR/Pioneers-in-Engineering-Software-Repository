using System;
using Microsoft.SPOT;

namespace PiEAPI
{
    class PIDController
    {
        // Constants
        private int KP;
        private int KI;
        private int KD;

        public int kp 
        {
            get { return KP; }
            set { KP = value; }
        }
        public int ki 
        {
            get { return KI; }
            set { KI = value; }
        }
        public int kd
        {
            get { return KD; }
            set { KD = value; }
        }

        // Calculation memory
        private CircularArray integral;
        private int integralSum;
        static private int integratorSize = 20;

        // Error derivative fields
        private int prevError;
        private long prevTicks;

        // (optional) input reader
        private Func<int> reader = null;

        public PIDController(int kp, int ki, int kd)
        {
            KP = kp;
            KI = ki;
            KD = kd;
            integral = new CircularArray(integratorSize);
            for(int i = 0; i < integratorSize ; i++) {
                integral.Enqueue(0);
            }
            prevTicks = DateTime.Now.Ticks;
        }

        public PIDController(int kp, int ki, int kd, Func<int> input)
            : this(kp, ki, kd)
        {
            reader = input;
        }

        public int update(int reference, int reading)
        {            
            int currentError = reference - reading;
            // Update D term;
            long currentTicks = DateTime.Now.Ticks;
            int errorDerivative = (int) ((currentError - prevError) * 10000000 / (currentTicks - prevTicks));

            // Update I term;
            integralSum = integralSum - integral.Dequeue();
            integral.Enqueue(currentError * (currentTicks - prevTicks));

            prevError = currentError;
            prevTicks = currentTicks;

            return KP * currentError + KD * errorDerivative + KI * integralSum;
        }
        
        // add error code if update is called when reader is not set?
        public int update(int reference)
        {
            if (reader != null) 
            {
                int reading = reader();
                return update(reference, reading);
            }
        }

    }
}
