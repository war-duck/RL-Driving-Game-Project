// using RLMatrix;
// using OneOf;

// public class RLModel : IEnvironment<int[]>
// {
//     public int stepCounter {get; set; }
//     public int maxSteps {get; set; }
//     public bool isDone {get; set; }
//     public OneOf<int, (int, int)> stateSize { get; set; }
//     public int[] actionSize {get; set; }

//     MyGameEnv myEnv;

//     public RLModel(MyGameEnv env)
//     {
//         myEnv = env;
//         Initialise();
//     }
//     public int[] GetCurrentState()
//     {
//         return myEnv.GetCurrentState();
//     }
//     public void Initialise()
//     {
//         stepCounter = 0;
//         maxSteps = 10000;
//         isDone = false;
//         stateSize = myEnv.stateSize;
//         actionSize = myEnv.actionSize;
//     }
//     public void Reset()
//     {
//         myEnv.Reset();
//         stepCounter = 0;
//         isDone = false;
//     }
//     public int Step(int[] actionId)
//     {
//         myEnv.Step(actionId);
//         stepCounter++;
//         if (stepCounter >= maxSteps)
//         {
//             isDone = true;
//         }
//         return 0;
//     }
// }