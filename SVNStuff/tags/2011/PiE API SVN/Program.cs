using System;
using Microsoft.SPOT;
using System.Threading;
namespace PiEProject1
{
    public class Program
    {
        public static void Main()
        {
            Robot robot = new Robot();

            while (true)
            {
                Debug.Print("sleeping");
                Thread.Sleep(500);
            }
        }

    }
}
