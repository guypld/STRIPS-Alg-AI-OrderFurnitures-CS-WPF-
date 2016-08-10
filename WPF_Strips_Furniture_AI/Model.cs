using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;
using WPF_Strips_Furniture_AI.Heuristics;
using WPF_Strips_Furniture_AI.Heuristics.Actions;
using WPF_Strips_Furniture_AI.Heuristics.Predicates;
using WPF_Strips_Furniture_AI.Tools;
namespace WPF_Strips_Furniture_AI
{
    public class Model
    {
        #region Singleton
        private static Model m_instance;

        public static Model Instance
        {
            get 
            {
                if (m_instance == null)
                {
                    m_instance = new Model();
                }
                return m_instance; 
            }
        }
        #endregion

        #region MainBoard Matrix
        /// <summary>
        /// 0   -   Free Space
        /// 1   -   Wall
        /// 2   -   Door
        /// </summary>
        static int[,] m_MainBoard = new int[,] { 
            { -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }, 
            { -1,0,0,0,0,0,0,0,0,0,0,0,-1,0,0,0,0,0,0,0,0,0,0,0,-1 },  
            { -1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-1 },   
            { -1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-1 }, 
            { -1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-1 }, 
            { -1,0,0,0,0,0,0,0,0,0,0,0,-1,0,0,0,0,0,0,0,0,0,0,0,-1 }, 
            { -1,0,0,0,0,0,0,0,0,0,0,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 },
            { -1,0,0,0,0,0,0,0,0,0,0,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }, 
            { -1,0,0,0,0,0,0,0,0,0,0,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }, 
            { -1,0,0,0,0,0,0,0,0,0,0,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }, 
            { -1,0,0,0,0,0,0,0,0,0,0,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }, 
            { -1,0,0,0,0,0,0,0,0,0,0,0,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }, 
            { -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 }
        };

        public static int[,] MainBoard
        {
            get { return m_MainBoard; }
            //set { m_MainBoard = value; }
        }
        #endregion

        #region Furnitures - Furniture List (Original Source & Target)
        List<Furniture> m_Furnitures = new List<Furniture>();
        public List<Furniture> Furnitures
        {
            get { return m_Furnitures; }
            set { m_Furnitures = value; }
        }
        #endregion

        #region NextID
        private int m_NextID = 1;

        public int NextID
        {
            get { return m_NextID; }
            set { m_NextID = value; }
        }
        #endregion

        #region CurrentState
        private State m_CurrentState = new State();

        public State CurrentState
        {
            get { return m_CurrentState; }
            set { m_CurrentState = value; }
        }

        // For GUI after finding the paths
        public State BackUp_CurrentState;
        #endregion CurrentState

        #region Stack
        /// <summary>
        /// This is the main stack of the alg
        /// each item in stack is a LIST of BaseItem(Action\Predicate)
        /// LIST represent an AND predicate
        /// </summary>
        private GUIStack<List<BaseItem>> m_Stack = new GUIStack<List<BaseItem>>();

        public GUIStack<List<BaseItem>> Stack
        {
            get { return m_Stack; }
            set { m_Stack = value; }
        }

        //For GUI:
        private ObservableCollection<ActionDescription> m_ObserveStack = new ObservableCollection<ActionDescription>();

        public ObservableCollection<ActionDescription> ObserveStack
        {
            get { return m_ObserveStack; }
            set { m_ObserveStack = value; }
        }

        public void PushToGUIStack(ActionDescription act)
        {
            App.Current.Dispatcher.Invoke((Action)delegate 
            {
                ObserveStack.Insert(0, act);
            });
        }
        public void PopFromGUIStack()
        {
            App.Current.Dispatcher.Invoke((Action)delegate 
            {
                ObserveStack.RemoveAt(0);
            });
        }
        #endregion

        public List<BaseItem> actionList = new List<BaseItem>();

        public int totalMovesCounter = 0;

        public Tools.ActionDescription LastMove { get; set; }

        #region AnimationStatus
        private AnimationEnum m_AnimationStatus = AnimationEnum.Play;

        public AnimationEnum AnimationStatus
        {
            get { return m_AnimationStatus; }
            set { m_AnimationStatus = value; }
        }
        #endregion

        /// Clone New Empty Board
        public static int[,] CloneNewBoard()
        {
            return (int[,])MainBoard.Clone();
        }

        /// <summary>
        /// Fill Board Matrix (Rooms) with Furniture List (Base Furnitures)
        /// furnitures can be on top of each other..
        /// (Assume all furnitures fits)
        /// </summary>
        /// <param name="board">Board to fill</param>
        /// <param name="furnitures">Furnitures List</param>
        public static void FillBoardWithFurnitures(int[,] board, List<BaseFurniture> furnitures)
        {
            foreach (var f in furnitures)   // loop over all furnitures
            {
                for (int i = f.I; i < (f.I + f.Height); i++)
                {
                    for (int j = f.J; j < (f.J + f.Width); j++)
                    {
                        board[i, j] = f.ID;
                    }
                }
            }
        }


        

        /// <summary>
        /// Get Current Board Clone with Current Source Furniture (from Current State)
        /// </summary>
        /// <returns></returns>
        public int[,] GetCurrentBoard()
        {
            int[,] tmpBoard = CloneNewBoard();

            FillBoardWithFurnitures(tmpBoard, Model.Instance.CurrentState.FurnitureList);

            return tmpBoard;
        }

        /// <summary>
        /// Check if new furniture can fit 
        /// </summary>
        /// <param name="Furnitures">Exist Furnitures (can be Source and target, just check if can fit anyway)</param>
        /// <param name="f">new furniture</param>
        /// <returns></returns>
        public static Boolean CheckIfFurnitureCanFit(List<BaseFurniture> Furnitures, BaseFurniture f)
        {
            // not in board
            if (f.I < 0 || f.J < 0 ||
                f.I + f.Height >= MainBoard.GetLength(0) || f.J + f.Width >= MainBoard.GetLength(1))
            {
                return false;
            }

            // clone board, and put furniture in it
            int[,] tmpBoard = (int[,])Model.MainBoard.Clone();
            FillBoardWithFurnitures(tmpBoard, Furnitures);

            // check if all spots for the new furniture is Free Spots
            for (int i = f.I; i < f.I + f.Height; i++)
            {
                for (int j = f.J; j < f.J+f.Width; j++)
                {
                    if (tmpBoard[i, j] != Consts.BOARD_FREE_SPOT)
                    {
                        return false;
                    }
                }
            }
            //else
            return true;
        }



        public Boolean isFinished()
        {
            // foreach Source List Item
            foreach (var f in CurrentState.FurnitureList)
            {
                // get the target of the item
                BaseFurniture target = Furnitures.Where(t => t.Target.ID == f.ID).ToList().First().Target;

                if ( ! f.Equals(target))
                {
                    return false;
                }
            }
            // otherwise :)
            return true;
        }


        public BaseAction GetNextAction()
        {
            BaseAction lastAction = null;   // this will represent the last action to perform

            // while the stack is not empty AND there was NO lastAction
            while (lastAction == null && m_Stack.Count > 0)
            {
                var head = m_Stack.Peek();  //head of the stack

                //check if all the head items are Predicate
                if ( head.All(p => p is Predicate) )
                {
                    // if all predicates conditions achieved
                    if (isPredicatesAchieved(head))
                    {
                        // Movable\At Predicate Achieved
                        Stack.Pop();
                        continue;   // lastAction still null
                    }
                    else if (head.Count > 1)
                    {
                        // Handle multiple predicates in one BaseItem(in stack)
                        // get a list with new predicates to move multiple furnitures
                        var newOrder = Heuristics.Heuristics.GetNewPredicatesOrder(head);

                        // push all AtPreds and MovablePreds to stack by new order
                        // but now each predicate in one item, not AND between them
                        newOrder.Reverse();
                        foreach (var item in newOrder)
                        {
                            Stack.Push( new List<BaseItem>() { item }); 
                        }
                          
                        continue; // lastAction still null
                    }
                    else if(head.Count == 1)
                    {
                        // only one goal to achieve
                        // get the action to achieve this goal
                        //BaseAction act = Heuristics.GetBestAction();
                        BaseAction action = Heuristics.Heuristics.GetBestAction(GetCurrentBoard(), head.First() as Predicate);

                        if (action == null)
                        {
                            continue;
                        }

                        // insert this action to new Stack item
                        var newItem = new List<BaseItem>() { action };
                        Stack.Push(newItem);

                        // now put Movable Predicate to stack (precondition before the action)
                        MovablePred movePred = new MovablePred() { EmptyAreasNeeded = action.getEmptyArea() , SourceFurniture = action.CurrentFurniture };
                        newItem = new List<BaseItem>() { movePred };
                        Stack.Push(newItem);
                        continue;
                    }
                }
                else if (head.Count == 1) // && item is Action
                {
                    (head.First() as BaseAction).Commit();
                    Stack.Pop();
                    return head.First() as BaseAction;

                }
                lastAction = null;
            }

            return lastAction;
        }

        /// <summary>
        /// Check if a list of predicates achieved
        /// </summary>
        /// <param name="head">list of predicates</param>
        /// <returns>True\False if achieved</returns>
        private bool isPredicatesAchieved(List<BaseItem> head)
        {
            foreach (Predicate pred in head)
            {
                //if source is NOT on target(AtPred) OR cant move to next position(MovablePred)
                if (! pred.isAchieved())
                {
                    return false;
                }
            }
            return true; // Achieved !
        } 
    }
}
