using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka
{
    public class cmdFloat : Command
    {


        public float num;

        public cmdFloat(string newName, float newNum, cmdType newType = cmdType.Float)
        {

            name = newName;
            num = newNum;
            cmdData = newType;

        }

        public override float getCommandFloat()
        {
            return num;
        }

    }
}