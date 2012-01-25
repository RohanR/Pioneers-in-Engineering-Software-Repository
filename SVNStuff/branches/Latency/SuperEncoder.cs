using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

namespace PiEAPI
{
    public abstract class SuperEncoder
    {
        private float dPerStep;
        private float stepsPerRev;
        private int steps;
        public float displacement
        {
            get
            {
                displacement = steps * dPerStep;
                return displacement;
            }
            set{displacement=value;}
        }
        public SuperEncoder(float displacementPerStep)
        {
            dPerStep=displacementPerStep;
        }
    }
}
