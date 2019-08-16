using System.Collections.Generic;

namespace Assets.Scripts.Class
{
    public class NewSteps
    {
        public NewSteps(int stepId, List<ListObjects> listOfObjects)
        {
            StepId = stepId;
            ListOfObjects = listOfObjects;
        }

        public int StepId { set; get; }
        public List<ListObjects> ListOfObjects { set; get; }

    }
}
