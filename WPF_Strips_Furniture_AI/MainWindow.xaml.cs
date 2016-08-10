using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Strips_Furniture_AI.Base;
using WPF_Strips_Furniture_AI.Heuristics;
using WPF_Strips_Furniture_AI.Heuristics.Actions;
using WPF_Strips_Furniture_AI.Heuristics.Predicates;
using WPF_Strips_Furniture_AI.Tools;

namespace WPF_Strips_Furniture_AI
{
    enum MouseUpStatusEnum
    {
        DoNothing,
        DrawNewFurnitureSource,
        DrawNewFurnitureTarget,
        AlgIsRunning
    }

    public partial class MainWindow : Window
    {
        MainWindowVM _vm = new MainWindowVM();

        Model _model = Model.Instance;

        Label[,] m_MainBoardGUI;

        MouseUpStatusEnum CurrentMouseStatus = MouseUpStatusEnum.DoNothing;

        Boolean isNewFurnitureFit = false;
        BaseFurniture m_NewSourceFurinture;
        BaseFurniture m_NewTargetFurinture;

        List<SolidColorBrush> Colors = new List<SolidColorBrush>() 
        {
            Brushes.Blue,
            Brushes.Brown,
            Brushes.CadetBlue,
            Brushes.Crimson,
            Brushes.DarkGreen,
            Brushes.DeepSkyBlue,
            Brushes.Gold,
            Brushes.Pink,
            Brushes.Bisque,
            Brushes.Salmon,
            Brushes.Purple,
            Brushes.Orange,
            Brushes.Navy,
            Brushes.MediumSpringGreen
        };


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _vm;

            InitBoard();

            StackDataGrid.ItemsSource = _model.ObserveStack;
        }

        private void InitBoard()
        {
            m_MainBoardGUI = new Label[Model.MainBoard.GetLength(0), 
                                       Model.MainBoard.GetLength(1)];

            for (int i = 0; i < Model.MainBoard.GetLength(0) ; i++)
            {
                var horizon_stack = new StackPanel();
                horizon_stack.Orientation = Orientation.Horizontal;
                //horizon_stack.Background = Brushes.Red;
                horizon_stack.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                horizon_stack.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                for (int j = 0; j < Model.MainBoard.GetLength(1) ; j++)
                {
                    var spot = new Label();
                    
                    m_MainBoardGUI[i, j] = spot;
                    spot.Tag = new Point(i, j);

                    spot.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    spot.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                    spot.Width = 30;
                    spot.Height = 30;
                    spot.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;

                    spot.BorderThickness = new Thickness(1);
                    spot.BorderBrush = Brushes.DarkBlue;

                    // Events
                    spot.MouseMove += spot_MouseMove;
                    spot.PreviewMouseDown += spot_PreviewMouseDown;

                    horizon_stack.Children.Add(spot);
                }

                MainBoard.Children.Add(horizon_stack);
            }

            DrawBoard();
        }

        private void DrawBoard()
        {
            // Clear Board
            #region Clear Board
            for (int i = 0; i < Model.MainBoard.GetLength(0); i++)
            {
                for (int j = 0; j < Model.MainBoard.GetLength(1); j++)
                {
                    var spot = m_MainBoardGUI[i, j];

                    switch (Model.MainBoard[i, j])
                    {
                        case Consts.BOARD_WALL_SPOT:
                            spot.Background = Brushes.Black;
                            break;

                        case Consts.BOARD_FREE_SPOT:
                        default:
                            spot.Background = Brushes.WhiteSmoke;
                            break;
                    }

                    spot.Content = String.Empty;
                }
            }
            #endregion

            // draw Target Furnitures First (will be hidden by the Current State):
            #region Draw Targets
            List<BaseFurniture> targets = _model.Furnitures.Select(f => f.Target).ToList();

            foreach (var f in targets)
            {
                for (int i = f.I; i < (f.I + f.Height); i++)
                {
                    for (int j = f.J; j < (f.J + f.Width); j++)
                    {
                        var spot = m_MainBoardGUI[i, j];
                        spot.Content = "T " + f.ID.ToString();

                        try
                        {
                            Color color = Colors[f.ID].Color;
                            float correctionFactor = 0.5f;
                            float red = (255 - color.R) * correctionFactor + color.R;
                            float green = (255 - color.G) * correctionFactor + color.G;
                            float blue = (255 - color.B) * correctionFactor + color.B;
                            Color lighterColor = Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);

                            spot.Background = new SolidColorBrush(lighterColor);
                        }
                        catch
                        {
                            spot.Background = Brushes.Maroon;
                        }
                    }
                }
            }
            #endregion

            // now draw Source Furnitures From Current State in Model
            #region Draw Sources
            List<BaseFurniture> sources = _model.CurrentState.FurnitureList;

            foreach (var f in sources)
            {
                for (int i = f.I; i < (f.I + f.Height); i++)
                {
                    for (int j = f.J; j < (f.J + f.Width); j++)
                    {
                        var spot = m_MainBoardGUI[i, j];
                        spot.Content = f.ID.ToString();
                        try
                        {
                            spot.Background = Colors[f.ID];
                        }
                        catch
                        {
                            spot.Background = Brushes.Maroon;
                        }
                    }
                }
            }
            #endregion
        }

        // Board Spot Mouse Clicked
        void spot_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("spot_PreviewMouseDown");


            if (isNewFurnitureFit)
            {
                int[,] tmpBoard = Model.CloneNewBoard();

                if (CurrentMouseStatus == MouseUpStatusEnum.DrawNewFurnitureSource)
                {
                    // create new Source Furniture
                    m_NewSourceFurinture = new BaseFurniture()
                    {
                        Height = _vm.NewFurniture.Height,
                        Width = _vm.NewFurniture.Width,
                        I = _vm.NewFurniture.I,
                        J = _vm.NewFurniture.J,
                        ID = _model.NextID
                    };

                    _model.CurrentState.FurnitureList.Add(m_NewSourceFurinture);    //update the CurrentState (also to draw the board while editing)
                    _model.NextID ++;

                    //update Status
                    CurrentMouseStatus = MouseUpStatusEnum.DrawNewFurnitureTarget;
                }
                else if (CurrentMouseStatus == MouseUpStatusEnum.DrawNewFurnitureTarget)
                {
                    // create new Target Furniture
                    m_NewTargetFurinture = new BaseFurniture()
                    {
                        Height = _vm.NewFurniture.Height,
                        Width = _vm.NewFurniture.Width,
                        I = _vm.NewFurniture.I,
                        J = _vm.NewFurniture.J,
                        ID = m_NewSourceFurinture.ID
                    };

                    // create a new Furniture according to source and target
                    Furniture new_f = new Furniture();
                    new_f.Source = m_NewSourceFurinture;
                    new_f.Target = m_NewTargetFurinture;

                    //insert this new furniture
                    _model.Furnitures.Add(new_f);

                    //update status
                    CurrentMouseStatus = MouseUpStatusEnum.DoNothing;
                    //update GUI
                    btnDrawNewFurniture.Content = "Draw";
                    isDrawNewFurniture_Clicked = false;
                    _vm.CanPutNewFurnitureState = true;

                    DrawBoard();
                }
            }

         
        }

        // Board Spot Mouse Move
        void spot_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentMouseStatus == MouseUpStatusEnum.DrawNewFurnitureSource ||
                CurrentMouseStatus == MouseUpStatusEnum.DrawNewFurnitureTarget)
            {
                SolidColorBrush color = Brushes.BlueViolet; 

                var lbl = sender as Label;
                
                //show highlighted rectangle in the cursor center
                int startI = int.Parse(((Point)lbl.Tag).X.ToString()) - (_vm.NewFurniture.Height / 2);
                int startJ = int.Parse(((Point)lbl.Tag).Y.ToString()) - (_vm.NewFurniture.Width / 2);

                int endI = startI + _vm.NewFurniture.Height;
                int endJ = startJ +  _vm.NewFurniture.Width;


                _vm.NewFurniture.I = startI;
                _vm.NewFurniture.J = startJ;

                DrawBoard();

                #region check if fits, if not change the color of the mouse
                //Create the furniture list (for sources and for targets -> source can be on target & target can be on source)
                List<BaseFurniture> list_so_far = new List<BaseFurniture>();
                if (CurrentMouseStatus == MouseUpStatusEnum.DrawNewFurnitureSource)
                {
                    // DrawNewFurnitureSource
                    // get sources list
                    list_so_far.AddRange(_model.CurrentState.FurnitureList);
                }
                else
                {
                    // DrawNewFurnitureTarget
                    // get targets list
                    list_so_far.AddRange(_model.Furnitures.Select(f => f.Target).ToList());
                }

                if (!Model.CheckIfFurnitureCanFit(list_so_far, _vm.NewFurniture))
                {
                    isNewFurnitureFit = false;
                    color = Brushes.Maroon;
                }
                else
                {
                    isNewFurnitureFit = true;
                }
                #endregion

                // Draw The Mouse Move New Furniture Spot
                for (int i = startI; i < endI; i++)
                {
                    for (int j = startJ; j < endJ; j++)
                    {
                        if (!(i < 0 || j < 0 || i > Model.MainBoard.GetLength(0) - 1 || j > Model.MainBoard.GetLength(1) - 1))
                        {
                            m_MainBoardGUI[i, j].Background = color;
                        }
                    }
                }
            }
        }

        private Boolean isDrawNewFurniture_Clicked = false;
        private void btnDrawNewFurniture_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMouseStatus == MouseUpStatusEnum.AlgIsRunning)
            {
                return;
            }

            if (isDrawNewFurniture_Clicked)
            {
                CurrentMouseStatus = MouseUpStatusEnum.DoNothing;
                (sender as Button).Content = "Draw";
                isDrawNewFurniture_Clicked = false;
                return;
            }

            int h = _vm.NewFurniture.Height,
                w = _vm.NewFurniture.Width;

            if (h <= 0 || w <= 0)
            {
                MessageBox.Show("Size error, please try again.", "Furniture Size", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentMouseStatus = MouseUpStatusEnum.DrawNewFurnitureSource;
            (sender as Button).Content = "Cancel";
            isDrawNewFurniture_Clicked = true;
            _vm.CanPutNewFurnitureState = false;

            RotateBTN.Focus();
        }

        private void StartMoving(object sender, RoutedEventArgs e)
        {
            if (_model.Furnitures.Count < 1)
            {
                MessageBox.Show("Please add at least one furniture");
                return;
            }

            foreach (var f in Model.Instance.Furnitures)
            {
                //not in same room
                if (Utils.getRoomOfFurniture(f.Source) != Utils.getRoomOfFurniture(f.Target))
                {
                    if (f.Source.Height > 3 && f.Source.Width > 3)
                    {
                        MessageBox.Show("MAN, You want to move a large Furniture through the door. Not cool!");
                        return;
                    }
                }
            }

            _vm.CanPutNewFurnitureState = false;
            CurrentMouseStatus = MouseUpStatusEnum.AlgIsRunning;

            _vm.Moves.Clear();

            Thread alg = new Thread(new ThreadStart(RunSTRIPS));

            alg.Start();

        }

        private void ClearBoard_Clicked(object sender, RoutedEventArgs e)
        {
            _vm.Moves.Clear();

            _model.Stack.Clear();

            _model.ObserveStack.Clear();

            _model.BackUp_CurrentState = new State();

            _model.Furnitures.Clear();

            _model.CurrentState.FurnitureList.Clear();

            _model.CurrentState = new State();

            _model.NextID = 1;

            DrawBoard();
        }

        private void SwapSizesClicked(object sender, RoutedEventArgs e)
        {
            int tmp = _vm.NewFurniture.Height;
            _vm.NewFurniture.Height = _vm.NewFurniture.Width;
            _vm.NewFurniture.Width = tmp;
            //DrawBoard();
        }

        private void REMOVETHIS_test_rotate(object sender, RoutedEventArgs e)
        {
            int[,] tmpBoard = Model.CloneNewBoard();

            foreach (var item in _model.Furnitures)
            {
                var checkRotate = new Heuristics.Actions.RotateAction();
                checkRotate.CurrentFurniture = item.Source;
                checkRotate.TargetFurniture = item.Target;
                if (checkRotate.CanCommit())
                {
                    MessageBox.Show("Can Rotate!");
                    checkRotate.Commit();
                    DrawBoard();
                }
                else
                {
                    MessageBox.Show("Stupid");
                }
            }
        }
        private void REMOVETHIS_test_move(object sender, RoutedEventArgs e)
        {
            int[,] tmpBoard = _model.CurrentState.GetBoard();

            var move = new MoveAction() { CurrentFurniture = _model.Furnitures[0].Source };
            string str = (sender as Button).Content.ToString();

            switch (str)
            {
                case "U":
                    move.Direction = DirectionEnum.Up;
                    break;
                case "R":
                    move.Direction = DirectionEnum.Right;
                    break;
                case "L":
                    move.Direction = DirectionEnum.Left;
                    break;
                case "D":
                default:
                    move.Direction = DirectionEnum.Down;
                    break;
            }

            if (move.CanCommit())
            {
                move.Commit();
            }

            DrawBoard();

        }

        #region Clean Stupid Actions
        /// <summary>
        /// Cleans moves not needed
        /// </summary>
        private void cleanMoves(BaseAction nextAction)
        {
            if (Model.Instance.actionList.Count == 0)
            {
                Model.Instance.actionList.Add(nextAction);
                return;
            }
            else if (nextAction is MoveAction && Model.Instance.actionList.Last() is MoveAction)
            {
                //last one is same id
                if ((Model.Instance.actionList.Last() as MoveAction).CurrentFurniture.ID == (nextAction as MoveAction).CurrentFurniture.ID)
                {
                    //is opposite, remove mini-loop
                    if ((Model.Instance.actionList.Last() as MoveAction).Direction == Utils.GetOppositeDirection((nextAction as MoveAction).Direction))
                    {
                        Model.Instance.actionList.RemoveAt(Model.Instance.actionList.Count-1);
                        return;
                    }

                    //check for loops
                    for (int i = Model.Instance.actionList.Count - 1; i >= 0; i--)
                    {
                        if (Model.Instance.actionList[i] is MoveAction)
                        {
                            //same id
                            if ((Model.Instance.actionList[i] as MoveAction).CurrentFurniture.ID == (nextAction as MoveAction).CurrentFurniture.ID)
                            {
                                //found loop, remove it
                                if ((Model.Instance.actionList[i] as MoveAction).x == (nextAction as MoveAction).x &&
                                    (Model.Instance.actionList[i] as MoveAction).y == (nextAction as MoveAction).y)
                                {
                                    Model.Instance.actionList.RemoveRange(i, Model.Instance.actionList.Count - i);
                                    Model.Instance.actionList.Add(nextAction);
                                    return;
                                }
                            }
                            else
                            {
                                //not same id / not a loop: stop! enter lastAction
                                Model.Instance.actionList.Add(nextAction);
                                return;
                            }
                        }
                        else
                        {
                            //not MoveAction: stop! enter lastAction
                            Model.Instance.actionList.Add(nextAction);
                            return;
                        }
                    }
                    //not a loop: stop! enter lastAction
                    Model.Instance.actionList.Add(nextAction);
                    return;
                }
                else
                {
                    Model.Instance.actionList.Add(nextAction);
                    return;
                }
            }
            else
            {
                Model.Instance.actionList.Add(nextAction);
                return;
            }
        }

        #endregion

        /// <summary>
        /// Start the STRIPS Alg
        /// </summary>
        public void RunSTRIPS()
        {
            #region Backup Furnitures Init Status
            Model.Instance.BackUp_CurrentState = new State();
            Model.Instance.BackUp_CurrentState.FurnitureList = new List<BaseFurniture>();

            foreach (var f in Model.Instance.Furnitures)
            {
                BaseFurniture cur = f.Source.Clone() as BaseFurniture;

                Model.Instance.BackUp_CurrentState.FurnitureList.Add( cur );
            }
            #endregion

            #region Init Stack (insert AtPred for each  furniture)
            // Initialize Alg Goals (all furnitures in their target spots)
            var init_preds = new List<BaseItem>(); 
            foreach (Furniture f in _model.Furnitures)
            {
                var new_f = new Furniture() // clone furniture
                { 
                    Source = (BaseFurniture)f.Source , 
                    Target = (BaseFurniture)f.Target 
                };

                AtPred p = new AtPred() { Furniture = new_f }; // create new At Predicate
                init_preds.Add(p);
            }
            _model.Stack.Push(init_preds);  //push to STRIPS stack
            #endregion

            Model.Instance.actionList.Clear();

            while (!_model.isFinished() && Model.Instance.Stack.Count() < Consts.MAX_STACK_COUNT && Model.Instance.totalMovesCounter < Consts.MAX_MOVES_TO_FAIL_COUNT)    //while not all sources a targets
            {
                switch (Model.Instance.AnimationStatus)
                {
                    case AnimationEnum.Pause:
                        continue;
                    case AnimationEnum.Next:
                        Model.Instance.AnimationStatus = AnimationEnum.Pause;
                        break;
                    case AnimationEnum.Play:
                    default:
                        break;
                }
                // Get the next action to do
                var nextAction = _model.GetNextAction();
                Model.Instance.totalMovesCounter++;

                cleanMoves(nextAction);

                Model.Instance.LastMove = new ActionDescription() { ID = nextAction.CurrentFurniture.ID, Action = nextAction.ToString() };

                this.Dispatcher.BeginInvoke(
                   (Action)delegate()
                   {
                       if ((bool)chkAnimate.IsChecked)
                       {
                           DrawBoard();
                           Thread.Sleep(1);
                       }
                       //_vm.Moves.Insert(0, Model.Instance.LastMove);
                   });

                //Thread.Sleep(300);

                //DrawBoard();
                //nextAction.Commit();
                //show action in GUI
            }

            if (Model.Instance.Stack.Count() >= Consts.MAX_STACK_COUNT || Model.Instance.totalMovesCounter >= Consts.MAX_MOVES_TO_FAIL_COUNT)
            {
                this.Dispatcher.BeginInvoke(
                (Action)delegate()
                {
                    MessageBox.Show("Not Solved..");
                    AlgEnded();
                });
                return;
            }


            //Now Show in GUI the final action
            // reset current state to init

            this.Dispatcher.BeginInvoke(
                (Action)delegate()
                {
                    lblPathFound.Visibility = System.Windows.Visibility.Visible;
                });

            Model.Instance.CurrentState.FurnitureList.Clear();

            //foreach (var f in Model.Instance.Furnitures)
            //{
            //    Model.Instance.CurrentState.FurnitureList.Add(f.Source);
            //}

            Model.Instance.CurrentState.FurnitureList.Clear();
            foreach (var item in Model.Instance.BackUp_CurrentState.FurnitureList)
            {
                Model.Instance.CurrentState.FurnitureList.Add(item.Clone() as BaseFurniture);
            }
            //Model.Instance.CurrentState = Model.Instance.BackUp_CurrentState;

            foreach (BaseAction act in Model.Instance.actionList)
            {
                act.CurrentFurniture = Model.Instance.CurrentState.FurnitureList.Where(f => f.ID == act.CurrentFurniture.ID).First();
                act.Commit();
                this.Dispatcher.BeginInvoke(
                (Action)delegate()
                {
                    DrawBoard();
                    _vm.Moves.Insert(0, new ActionDescription() { Action = act.ToString(), ID = act.CurrentFurniture.ID });
                });
                Thread.Sleep(140);
            }

            this.Dispatcher.BeginInvoke(
                (Action)delegate()
                {
                    MessageBox.Show("Success!!!");
                    AlgEnded();
                    lblPathFound.Visibility = System.Windows.Visibility.Hidden;
                });
        }

        public void AlgEnded()
        {
            _vm.CanPutNewFurnitureState = true;
            CurrentMouseStatus = MouseUpStatusEnum.DoNothing;
        }

        private void AnimationButton_Clicked(object sender, RoutedEventArgs e)
        {
            switch ( (sender as Button).Content.ToString() )
            {
                case "Play":
                    Model.Instance.AnimationStatus = AnimationEnum.Play;
                    break;
                case "Pause":
                    Model.Instance.AnimationStatus = AnimationEnum.Pause;
                    break;
                case "Next":
                    Model.Instance.AnimationStatus = AnimationEnum.Next;
                    break;
                default:
                    break;
            }
        }
    }
}
