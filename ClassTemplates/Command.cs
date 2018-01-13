using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka
{

    public enum cmdType
    {

        Move, Action, Toggle, Float

    }

    public enum Action
    {

        None, Walk, Run, Sprint, Attack, Stop, Defend, Jump, Crouch
        
    }

    public class Command
    {

        public string name;

        public cmdType cmdData;

        public cmdType actionType;

        public bool waitForFinish;

        public virtual Object getCommandData()
        {

            return null;

        }

        public virtual Vector3 getCommandVector()
        {

            return Vector3.zero;

        }

        public virtual float getCommandFloat()
        {

            return 0.0f;

        }

        public virtual string getCommandString()
        {

            return "";

        }

        public virtual bool getCommandToggle()
        {

            return false;

        }

        public virtual Action getAction()
        {

            return 0;

        }

        public cmdType getActionType()
        {

            return actionType;

        }

    }
}