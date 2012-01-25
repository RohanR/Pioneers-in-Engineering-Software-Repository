using System;
using Microsoft.SPOT;

namespace PiEAPI
{
    class CircularArray
    {
        private int head;
        private int tail;
        private int[] array;
        private int size;

        public CircularArray(int len)
        {
            head = 0;
            tail = 0;
            size = len;
            array = new int[len];
        }

        public void Enqueue(int newNum)
        {
            array[tail] = newNum;
            tail++;
            if(tail == size) 
            {
                tail = 0;
            }
            if(tail == head) 
            {
                head++;
            }
        }

        public int Dequeue() 
        {
            if (head == tail)
            {
                return 0;
            }
            int ret = array[head];
            head++;
            if (head == size)
            {
                head = 0;
            }
            return ret;
        }
    }
}
