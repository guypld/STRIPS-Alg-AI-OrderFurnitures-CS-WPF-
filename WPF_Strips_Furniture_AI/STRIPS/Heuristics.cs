using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Strips_Furniture_AI.Base;
using WPF_Strips_Furniture_AI.Heuristics.Actions;
using WPF_Strips_Furniture_AI.Heuristics.Predicates;
using WPF_Strips_Furniture_AI.Tools;

namespace WPF_Strips_Furniture_AI.Heuristics
{
    public class Heuristics
    {

        private static Random random = new Random();

        /// <summary>
        /// Return the best action that help achieved the given predicate
        /// </summary>
        /// <param name="board">Current Board</param>
        /// <param name="pred">Base Predicate</param>
        public static BaseAction GetBestAction(int[,] board, Predicate pred)
        {
            if (pred is AtPred)
            {
                AtPred p = pred as AtPred;

                int sourceRoom = Utils.getRoomOfFurniture(p.Furniture.Source);
                int targetRoom = Utils.getRoomOfFurniture(p.Furniture.Target);

                // if furniture Source and Target in the same room
                if (sourceRoom == targetRoom || (p as AtPred).Furniture.Source.J == Consts.DOOR_X_POS)
                {
                    var directionsToMove = getDirectionsInRoom(p.Furniture);

                    //check if need to rotate
                    #region Check If Need to rotate (and can)
                    if (p.Furniture.Source.Height != p.Furniture.Target.Height)
                    {
                        // need rotate
                        var checkRotate = new RotateAction()
                        {
                            CurrentFurniture = p.Furniture.Source,
                            TargetFurniture = p.Furniture.Target
                        };
                        if (checkRotate.CanCommit())
                        {
                            //can rotate
                            return checkRotate;
                        }
                        else if (directionsToMove.Count() == 0 &&
                                    (p.Furniture.Source.I == p.Furniture.Target.I && 
                                     p.Furniture.Source.J == p.Furniture.Target.J))
                        {
                            // Check if furniture is ON target I,J but need to rotate and CANT
                            // try to move furniture one spot down
                            MakeRandomMove();
                            return null;
                        }
                    }
                    #endregion
                    
                    //else // no need to rotate -> need to move
                    {
                        // trying to move back to problematic point
                        if (p.Furniture.Source.IsWrongWay.IsInWrongWay && directionsToMove.Count() == 1)
                        {
                            p.Furniture.Source.IsWrongWay.TryCount++;
                            
                            if (p.Furniture.Source.IsWrongWay.TryCount > Consts.TRY_COUNT)
                            {
                                p.Furniture.Source.IsWrongWay.IsInWrongWay = false;

                                if (p.Furniture.Source.MoveCount < Consts.MAX_MOVES)
                                {
                                    SwitchFurniture();  // We're stacked, try other furniture
                                }
                                else
                                {
                                    MakeRandomMove();
                                }
                                return null;
                            }

                            // as long we can move in the same direction
                            DirectionEnum tmp_direction;

                            tmp_direction = Utils.GetOppositeDirection(p.Furniture.Source.IsWrongWay.Direction);

                            // check if can move to this direction
                            var newMove = new MoveAction()
                            {
                                CurrentFurniture = p.Furniture.Source,
                                TargetFurniture = p.Furniture.Target,
                                Direction = tmp_direction 
                            };
                            if (newMove.CanCommit())
                            {
                                // can move
                                return newMove;
                            }
                            else //can't move anymore, try opposite direction (Ex: Up -> Down)
                            {
                                p.Furniture.Source.IsWrongWay.Direction = tmp_direction;
                                tmp_direction = Utils.GetOppositeDirection(tmp_direction);

                                newMove.Direction = tmp_direction;
                                if (newMove.CanCommit())
                                {
                                    // can move
                                    return newMove;
                                }

                            }
                        }

                        // Can't move -> try to move anyway while moving blocking furnitures
                        //if (directionsToMove.Count == 0 ||
                        //    (p.Furniture.Source.IsWrongWay.IsInWrongWay &&
                        //        directionsToMove.First().Equals(p.Furniture.Source.IsWrongWay.Direction)))
                        if (directionsToMove.Count == 0)
                        {
                            // other furniture block us , check where we wish to move
                            directionsToMove = getDirectionsInRoom(p.Furniture, CheckIfNeedToCommitEnum.No);

                            // Wall is Blocking
                            if (directionsToMove.Count == 0)
                            {
                                // TODO
                                SwitchFurniture();
                                return null;
                            }
                            // now we have the direction we want to move, try to bypass the obstacle
                            // the list will always has ONKY ONE item
                            DirectionEnum d = directionsToMove.First();

                            //mark furniture in WrongWay
                            p.Furniture.Source.IsWrongWay.IsInWrongWay = true;

                            //p.Furniture.Source.IsWrongWay.Direction = d;

                            directionsToMove.Clear();

                            int randomNumber = random.Next(0, 2);   //choose random direction (UP\DOWN or LEFT\RIGHT)

                            

                            List<DirectionEnum> tmpDirList;

                            //now look for alternative direction
                            switch (d)
                            {
                                case DirectionEnum.Left:
                                case DirectionEnum.Right:
                                    tmpDirList = new List<DirectionEnum>() { DirectionEnum.Up, DirectionEnum.Down };
                                    break;
                                case DirectionEnum.Up:
                                case DirectionEnum.Down:
                                default:
                                    tmpDirList = new List<DirectionEnum>() { DirectionEnum.Left, DirectionEnum.Right };
                                    break;
                            }

                            //set to random direction
                            DirectionEnum move_to = tmpDirList[randomNumber];
                            tmpDirList.RemoveAt(randomNumber);
                            directionsToMove.Add(move_to); // put our chosen way (random)
                            // next turn don't allow to move back to this spot (UNTILL CAN'T MOVE THIS DIRECTION (BLOCK\WALL))
                            p.Furniture.Source.IsWrongWay.Direction = tmpDirList.First(); // put the opposite way
                        }
                        else
                        {
                            p.Furniture.Source.IsWrongWay.IsInWrongWay = false;
                        }
                        

                        //can move:
                        
                        //check if in wrong way

                        //take random direction from list
                        int rnd = random.Next(0, directionsToMove.Count());

                        var checkMove = new MoveAction()
                        {
                            CurrentFurniture = p.Furniture.Source,
                            TargetFurniture = p.Furniture.Target,
                            Direction = directionsToMove[rnd] // take the fist direction
                        };
                        if (checkMove.CanCommit())
                        {
                            return checkMove;
                        }
                        else
                        {
                            //Can't move!
                            if (p.Furniture.Source.MoveCount < Consts.MAX_MOVES)
                            {
                                SwitchFurniture();
                                return null;
                            }
                            else
                            {
                                MakeRandomMove();
                                return null;
                            }
                        }

                    }


                }
                else //not in the same room
                {
                    var directionsToMove = getDirectionsToDoor(p.Furniture);

                    if (directionsToMove.Count() == 0)
                    {
                        //some furniture is blocking the door!
                        if (Model.Instance.Furnitures.Count > 1 && p.Furniture.Source.MoveCount < Consts.MAX_MOVES)
                        {
                            SwitchFurniture();  // We're stacked, try other furniture
                            p.Furniture.Source.MoveCount++;
                            // if we switched to furniture that is on target, but reached max moves, make him move!
                            if (Model.Instance.Stack.Peek().First() is AtPred)
                            {
                                var next_f = (Model.Instance.Stack.Peek().First() as AtPred).Furniture.Source;
                                if (next_f.MoveCount > Consts.MAX_MOVES)
                                {
                                    MakeRandomMove();
                                }
                            }
                        }
                        else
                        {
                            MakeRandomMove();
                        }

                        return null;
                    }

                    if (Utils.CanCrossDoor(p.Furniture.Source))
                    {
                        var checkMove = new MoveAction()
                        {
                            CurrentFurniture = p.Furniture.Source,
                            TargetFurniture = p.Furniture.Target,
                            Direction = directionsToMove.First() // take the fist direction
                        };
                        if (checkMove.CanCommit())
                        {
                            return checkMove;
                        }
                        
                    }
                    else
                    {
                        var checkRotate = new RotateAction()
                        {
                            CurrentFurniture = p.Furniture.Source,
                            TargetFurniture = p.Furniture.Target
                        };
                        if (checkRotate.CanCommit())
                        {
                            //can rotate
                            return checkRotate;
                        }
                        else
                        {
                            //Need to rotate but can't
                            // check if there was available moves do make to target before starting with random moves
                            if (directionsToMove.Count > 0)
                            {
                                var checkMove = new MoveAction()
                                {
                                    CurrentFurniture = p.Furniture.Source,
                                    TargetFurniture = p.Furniture.Target,
                                    Direction = directionsToMove.First() // take the fist direction
                                };
                                if (checkMove.CanCommit())
                                {
                                    return checkMove;
                                }
                                else
                                {
                                    //Can't move and rotate!
                                    if (p.Furniture.Source.MoveCount < Consts.MAX_MOVES)
                                    {
                                        SwitchFurniture();
                                        return null;
                                    }
                                    else
                                    {
                                        MakeRandomMove();
                                        return null;
                                    }
                                }
                            }
                            else
                            {
                                // No Moves (and can't rotate)
                                if (p.Furniture.Source.MoveCount < Consts.MAX_MOVES)
                                {
                                    SwitchFurniture();
                                    return null;
                                }
                                else
                                {
                                    MakeRandomMove();
                                    return null;
                                }
                            }
                        }
                    }
                }
            }
            else // pred is MovablePred but not achieved
            {
                // check which furniture block us to achieved current MovablePred (pred)


                // get the first furniture that block. that enough.
                // check in our predicate which other furniture is in this spots
                foreach (var area in (pred as MovablePred).EmptyAreasNeeded)
                {
                    // area is the block that need to be free
                    for (int i = area.I; i < area.Height; i++)
                    {
                        for (int j = area.J; j < area.Width; j++)
                        {
                            if (board[i, j] != Consts.BOARD_FREE_SPOT)
                            {
                                // now board[i,j] will has the ID of the blocking furniture
                                BaseFurniture block_furniture = Model.Instance.CurrentState.FurnitureList.Where(b => b.ID == board[i, j]).First();
                                // check where to move this block furniture according to i,j
                                
                                // if blocking us from UP
                                if (block_furniture.I < (pred as MovablePred).SourceFurniture.I)
                                {
                                    var newAction = new MoveAction() 
                                    {
                                        // direction to move the blocking furniture
                                        Direction = DirectionEnum.Up,
                                        // the blocking furniture ref
                                        CurrentFurniture = block_furniture,
                                        // the blocking furniture REAL target
                                        TargetFurniture = Model.Instance.Furnitures.Where( s => s.Source.ID == board[i, j]).First().Target
                                    };

                                    if (!newAction.IsBlockedByWall())
                                    {
                                        // Move!
                                        return newAction;
                                    }
                                    
                                }
                                // if blocking us from DOWN
                                if (block_furniture.I > (pred as MovablePred).SourceFurniture.I)
                                {
                                    var newAction = new MoveAction()
                                    {
                                        // direction to move the blocking furniture
                                        Direction = DirectionEnum.Down,
                                        // the blocking furniture ref
                                        CurrentFurniture = block_furniture,
                                        // the blocking furniture REAL target
                                        TargetFurniture = Model.Instance.Furnitures.Where(s => s.Source.ID == board[i, j]).First().Target
                                    };

                                    if (!newAction.IsBlockedByWall())
                                    {
                                        // Move!
                                        return newAction;
                                    }
                                    
                                }
                                // if blocking us from Left
                                if (block_furniture.J < (pred as MovablePred).SourceFurniture.J)
                                {
                                    var newAction = new MoveAction()
                                    {
                                        // direction to move the blocking furniture
                                        Direction = DirectionEnum.Left,
                                        // the blocking furniture ref
                                        CurrentFurniture = block_furniture,
                                        // the blocking furniture REAL target
                                        TargetFurniture = Model.Instance.Furnitures.Where(s => s.Source.ID == board[i, j]).First().Target
                                    };

                                    if (!newAction.IsBlockedByWall())
                                    {
                                        // Move!
                                        return newAction;
                                    }
                                    
                                }
                                // if blocking us from Right
                                if (block_furniture.J > (pred as MovablePred).SourceFurniture.J)
                                {
                                    var newAction = new MoveAction()
                                    {
                                        // direction to move the blocking furniture
                                        Direction = DirectionEnum.Right,
                                        // the blocking furniture ref
                                        CurrentFurniture = block_furniture,
                                        // the blocking furniture REAL target
                                        TargetFurniture = Model.Instance.Furnitures.Where(s => s.Source.ID == board[i, j]).First().Target
                                    };

                                    if (!newAction.IsBlockedByWall())
                                    {
                                        // Move!
                                        return newAction;
                                    }
                                    
                                }
                            }
                        }
                    }
                }

                // can move! nothing block us

                
            }

            return null; 
        }

        public enum CheckIfNeedToCommitEnum
        {
            Yes,
            No
        }

        public static List<DirectionEnum> getDirectionsInRoom(Furniture f, CheckIfNeedToCommitEnum CheckIfCommit = CheckIfNeedToCommitEnum.Yes)
        {
            // create temp dictionary
            // has the directions needed(and avail) to target
            // and the "distance" for each direction
            var directionList = new List<Tuple<DirectionEnum,int>>();
            int deltaI = f.Target.I - f.Source.I;
            int deltaJ = f.Target.J - f.Source.J;

            // create temp MoveAction to check for each direction if legal
            var tempMoveAction = new MoveAction()
            {
                CurrentFurniture = f.Source,
                TargetFurniture = f.Target,
            };

            if (deltaJ > 0)
            {
                int dist = Math.Abs(deltaJ) + 1 + Math.Abs(deltaI);
                tempMoveAction.Direction = DirectionEnum.Right;
                // Check if can commit OR if we asked to ignore blocking furniture BUT not blocking wall
                if (tempMoveAction.CanCommit() ||
                    (CheckIfCommit == CheckIfNeedToCommitEnum.No && !tempMoveAction.IsBlockedByWall() ) )
                {
                    directionList.Add(new Tuple<DirectionEnum, int>(tempMoveAction.Direction, dist));
                }
            }
            if (deltaJ < 0)
            {
                int dist = Math.Abs(deltaJ) - 1 + Math.Abs(deltaI);
                tempMoveAction.Direction = DirectionEnum.Left;
                // Check if can commit OR if we asked to ignore blocking furniture BUT not blocking wall
                if (tempMoveAction.CanCommit() ||
                    (CheckIfCommit == CheckIfNeedToCommitEnum.No && !tempMoveAction.IsBlockedByWall()))
                {
                    directionList.Add(new Tuple<DirectionEnum, int>(tempMoveAction.Direction, dist));
                }
            }
            if (deltaI > 0)
            {
                int dist = Math.Abs(deltaJ) + 1 + Math.Abs(deltaI);
                tempMoveAction.Direction = DirectionEnum.Down;
                // Check if can commit OR if we asked to ignore blocking furniture BUT not blocking wall
                if (tempMoveAction.CanCommit() ||
                    (CheckIfCommit == CheckIfNeedToCommitEnum.No && !tempMoveAction.IsBlockedByWall()))
                {
                    directionList.Add(new Tuple<DirectionEnum, int>(tempMoveAction.Direction, dist));
                }
            }
            if (deltaI < 0)
            {
                int dist = Math.Abs(deltaJ) - 1 + Math.Abs(deltaI);
                tempMoveAction.Direction = DirectionEnum.Up;
                // Check if can commit OR if we asked to ignore blocking furniture BUT not blocking wall
                if (tempMoveAction.CanCommit() ||
                    (CheckIfCommit == CheckIfNeedToCommitEnum.No && !tempMoveAction.IsBlockedByWall()))
                {
                    directionList.Add(new Tuple<DirectionEnum, int>(tempMoveAction.Direction, dist));
                }
            }

            return directionList.OrderBy(d => d.Item1).Select(i => i.Item1).ToList();
        }

        /// <summary>
        /// Create door furniture as Target while need to move between rooms
        /// </summary>
        public static List<DirectionEnum> getDirectionsToDoor(Furniture f)
        {
            BaseFurniture doorFurniture;

            // Check furniture height
            if (f.Source.Height == 1)
            {
                // move furniture to the center of the door
                doorFurniture = new BaseFurniture()
                {
                    I = Consts.UPPER_DOOR_SPOT + 1,
                    J = Consts.DOOR_X_POS,
                    Height = Consts.BOTTOM_DOOR_SPOT - Consts.UPPER_DOOR_SPOT + 1,
                    Width = 1
                };
            }
            else
            {
                // move furniture to the top of the door
                doorFurniture = new BaseFurniture()
                {
                    I = Consts.UPPER_DOOR_SPOT,
                    J = Consts.DOOR_X_POS,
                    Height = Consts.BOTTOM_DOOR_SPOT - Consts.UPPER_DOOR_SPOT + 1,
                    Width = 1
                };
            }
            

            return getDirectionsInRoom(new Furniture() { Source = f.Source, Target = doorFurniture } );
        }


        public static List<BaseItem> GetNewPredicatesOrder(List<BaseItem> predList)
        {
            // get AtPred List and MovablePred List (separately)
            List<BaseItem> AtPred_List = predList.Where(p => p is AtPred).ToList();
            List<BaseItem> MovablePred_List = predList.Where(p => p is MovablePred).ToList();

            if (MovablePred_List == null)
            {
                MovablePred_List = new List<BaseItem>();
            }

            // More than 1 furniture need to move to target
            if (AtPred_List.Count > 1)
            {
                //var newAtPredOrder;
                // order AtPreds by shortest to longest
                // so stack will handle first the most longest path to target

                AtPred_List.OrderBy(i =>
                    Math.Abs((i as AtPred).Furniture.Source.I - (i as AtPred).Furniture.Target.I) +
                    Math.Abs((i as AtPred).Furniture.Source.J - (i as AtPred).Furniture.Target.J)
                    );
            }

            // order them  that AtPred will be the last and the Movables condition First
            return MovablePred_List.Concat(AtPred_List).ToList();

        }

        /// <summary>
        /// When Furniture is stack, insert to the Stack other random furniture
        /// Maybe it will solved the blocking
        /// </summary>
        public static void SwitchFurniture()
        {
            //move the spotlight to random furniture:
            int r = random.Next(0, Model.Instance.CurrentState.FurnitureList.Count());

            Console.WriteLine("SwitchFurniture ID = " + (r + 1).ToString());

            var new_fur = new Furniture()
            {
                Source = Model.Instance.CurrentState.FurnitureList[r],
            };
            new_fur.Target = Model.Instance.Furnitures.Where(ff => ff.Source.ID == new_fur.Source.ID).First().Target;

            var newResetPredicate = new AtPred()
            {
                Furniture = new_fur
            };

            Model.Instance.Stack.Push(new List<BaseItem>() { newResetPredicate });

            new_fur.Source.MoveCount++;

            //init WrongDirection for all furnitures
            foreach (var f in Model.Instance.CurrentState.FurnitureList)
            {
                f.IsWrongWay.IsInWrongWay = false;
            }
        }


        public static void MakeRandomMove()
        {
            //move the spotlight to random furniture:
            int r = random.Next(0, Model.Instance.CurrentState.FurnitureList.Count());

            Console.WriteLine("MakeRandomMove ID = " + (r+1).ToString());

            var new_fur = new Furniture()
            {
                Source = Model.Instance.CurrentState.FurnitureList[r]
            };


            // Choose Random Target:

            int rndTarget = random.Next(0, 4);

            BaseFurniture new_target;


            switch (rndTarget)
            {
                case 0:
                    // Top Left corner + rotate just in case
                    new_target = new BaseFurniture()
                    {
                        I = 1,
                        J = 1,
                        Height = new_fur.Source.Width,
                        Width = new_fur.Source.Height
                    };
                    break;
                case 1:
                    // Bottom Left corner
                    new_target = new BaseFurniture()
                    {
                        I = Consts.ROOM_1_Y2 - new_fur.Source.Height - 1,
                        J = 1,
                        Height = new_fur.Source.Height,
                        Width = new_fur.Source.Width
                    };
                    break;
                case 2:
                    new_target = new BaseFurniture()
                    {
                        I = 1,
                        J = Consts.ROOM_2_X2 - 1 - new_fur.Source.Width,
                        Height = new_fur.Source.Height,
                        Width = new_fur.Source.Width
                    };
                    break;
                case 3:
                default:
                    new_target = new BaseFurniture()
                    {
                        I = Consts.ROOM_2_Y2 - 1 - new_fur.Source.Height,
                        J = Consts.ROOM_2_X2 - 1 - new_fur.Source.Width,
                        Height = new_fur.Source.Height,
                        Width = new_fur.Source.Width
                    };
                    break;
                
            }




            // create random target location
            if (Utils.getRoomOfFurniture(new_fur.Source) == 1)
            {
                // Room 1
                //create new target 
                // random from 2:

                if (new_fur.Source.MoveCount % 2 != 0)
                {

                }
                else
                {

                }
            }
            else
            {
                // Room 2
                //create new target 
                // random from 2:

                if (new_fur.Source.MoveCount % 2 != 0)
                {

                }
                else
                {

                }
            }













            new_fur.Target = new_target;

            var newResetPredicate = new AtPred()
            {
                Furniture = new_fur
            };

            Model.Instance.Stack.Push(new List<BaseItem>() { newResetPredicate });

            //init WrongDirection for all furnitures
            foreach (var f in Model.Instance.CurrentState.FurnitureList)
            {
                f.IsWrongWay.IsInWrongWay = false;
            }
        }
    }
}
