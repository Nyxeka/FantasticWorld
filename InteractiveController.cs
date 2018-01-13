using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nyxeka
{
    public class InteractiveController : MonoBehaviour
    {

        public InputBehaviour[] input;

        public string ControllerID = "";

        public int defaultInputIndex = 0;

        private int currentIB = 0;

        private UnitCharacter unit;

        public UnitCharacter defaultUnit;

        private bool keepIBRunning = false;

        private bool busy = false;

        // Use this for initialization
        void Start()
        {

            if (defaultUnit != null)
            {

                unit = defaultUnit;

            }
            
            //run through and make sure they are all turned off.
            for(int i = 0; i < input.Length; i++)
            {
                if (input[i]) // null check
                    input[i].enabled = false;

            }

            currentIB = defaultInputIndex;

            RunIB();

        }

        void RunIB()
        {

            RunIB(currentIB);

        }

        public void ResetCurrentIB()
        {

            RunIB(currentIB);

        }

        void RunIB(string IBName)
        {

            for(int i = 0;i<input.Length;i++)
            {

                if (input[i].ID == IBName)
                {

                    RunIB(i);
                    return;

                }

            }

        }

        void RunIB(int IBindex)
        {
            InitIB(IBindex);
        }

        void SwitchIB(int newIBindex)
        {

            Debug.Log("IB killed. Running cleanup methods.");

            input[currentIB].ExitCamera();
            input[currentIB].ExitIB();

            input[currentIB].enabled = false;

            RunIB(newIBindex);

        }
        
        void InitIB(int IBindex)
        {

            if (input.Length == 0)
            {
                Debug.LogError("Warning! No input handlers detected!");
                StopAllCoroutines();
            }
            //yield return null;
            int index = (IBindex < input.Length ? IBindex : 0);

            if (input[index] == null)
            {

                if (input[index] = gameObject.GetComponent<InputBehaviour>())
                {
                    Debug.Log("Resetting to first input method found: " + input[index].ID);
                }
                else
                {
                    Debug.LogError("Input at index is null. Warning! No input handlers detected!");
                }

            }

            input[index].enabled = true;

            Debug.Log("Input Behaviour set to: " + input[index].ID + ", IB Index: " + index.ToString());

            currentIB = index;

            //sinces the input behaviour is a component, this turns into a sort of pointer
            InputBehaviour IB = input[index];

            //init the IB.

            //run some inits.

            IB.InitCamera();

            IB.Init(unit);

            keepIBRunning = true;

        }

        void HandleCurrentIB()
        {

            input[currentIB].RunBehaviour();
            input[currentIB].RunCamera();

        }

        // Update is called once per frame
        void Update()
        {
            // quick hack for now.
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("Switching Input Behaviours.");
                SwitchIB((currentIB + 1) % input.Length);

            }

        }

        private void FixedUpdate()
        {
            HandleCurrentIB();

        }
    }
}