using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka
{
    public class cmdToggle : Command
    {

        public bool toggle;

        public cmdToggle(string newName, bool newToggle, cmdType newType = cmdType.Toggle)
        {
            cmdData = newType;
            name = newName;
            toggle = newToggle;

        }

        public override bool getCommandToggle()
        {
            return toggle;
        }

    }
}