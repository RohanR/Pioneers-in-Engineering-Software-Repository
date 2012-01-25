using System;
using Microsoft.SPOT;

namespace PiEAPI
{
    class PIDController
    {
        // Constants
        private int _KP;
        private int _KI;
        private int _KD;

        // Calculation memory
        private CircularArray integral;
        private int integralSum;
        static private int integratorSize = 20;

        // Error derivative fields
        private int prevError;
        private long prevTicks;

        // (optional) input reader
        private delegate int Reader();
        private Reader reader = null;

        public PIDController(int kp, int ki, int kd)
        {
            _KP = kp;
            _KI = ki;
            _KD = kd;
            integral = new CircularArray(integratorSize);
            for(int i = 0; i < integratorSize ; i++) {
                integral.Enqueue(0);
            }
            prevTicks = DateTime.Now.Ticks;
        }

        public PIDController(int kp, int ki, int kd, Reader input)
            : this(kp, ki, kd)
        {
            reader = input;
        }

        // maybe we should normalize these accessors for the constants?
        // CURRENTLY: i = _i
        public int KP
        {
            get { return _KP; }
            set { _KP = value; }
        }

        public int KI
        {
            get { return _KI; }
            set { _KI = value; }
        }

        public int KD       
        {
                get { return _KD; }
            set { _KD = value; }
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

            return _KP * currentError + _KD * errorDerivative + _KI * integralSum;
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
