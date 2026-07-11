using RoomCraft.Furniture;
using UnityEngine;

namespace RoomCraft.UndoRedo
{
    public class MoveFurnitureCommand : ICommand
    {
        private readonly FurnitureObject furniture;
        private readonly Vector3 fromPos;
        private readonly Vector3 toPos;

        public MoveFurnitureCommand(FurnitureObject furniture, Vector3 fromPos, Vector3 toPos)
        {
            this.furniture = furniture;
            this.fromPos = fromPos;
            this.toPos = toPos;
        }
        
        public void Execute() => furniture.MoveTo(toPos);
        public void Undo() => furniture.MoveTo(fromPos);
    }
}