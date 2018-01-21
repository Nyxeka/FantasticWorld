using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka
{
    public class cmdMove : Command
    {

        public Vector3 velocityData;

        public cmdMove(Vector3 newVelData, cmdType newType = cmdType.Move)
        {

            name = "move";
            velocityData = newVelData;
            cmdData = newType;

        }

        public cmdMove(Vector3 newVelData, string newName, cmdType newType = cmdType.Move)
        {

            name = newName;
            velocityData = newVelData;

        }

        public override Vector3 getCommandVector()
        {
            if (velocityData == null)
            {
                return Vector3.zero; 
            }
            else
            {
                return velocityData;
            }
        }
    }
}