using System;
using System.Collections.Generic;
using System.Text;
using FindInterfaceImplementation;

namespace FindInterfaceImplementation.World
{
    public class World :ITalkable
    {
        public void Talk()
        {
            Console.WriteLine("World!");
        }

        public void Append(List<string> str)
        {
            str.Add("World!");
        }
    }
}
