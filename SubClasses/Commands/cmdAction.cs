using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace nyxeka
{
    public class cmdAction : Command
    {

        public Action actionData;

        public Vector3 vectorData;

        public float floatData;

        // Use this for initialization
        public cmdAction(Action newAction, cmdType newActionType = cmdType.Action)
        {
            actionData = newAction;
            actionType = newActionType;
        }

        public cmdAction(Action newAction, float newFloatData, cmdType newActionType = cmdType.Float)
        {
            actionData = newAction;
            actionType = newActionType;
            floatData = newFloatData;
        }

        public cmdAction(Action newAction, Vector3 newVectorData, cmdType newActionType = cmdType.Move) {

            actionData = newAction;
            actionType = newActionType;
            vectorData = newVectorData;

        }

        public override Action getAction()
        {
            return actionData;
        }

        public override Vector3 getCommandVector()
        {

            return vectorData;

        }

        public override float getCommandFloat()
        {
            return floatData;
        }

    }
}