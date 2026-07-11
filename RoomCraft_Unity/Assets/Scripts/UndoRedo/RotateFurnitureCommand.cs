using RoomCraft.Furniture;

namespace RoomCraft.UndoRedo
{
    public class RotateFurnitureCommand : ICommand
    {
        private readonly FurnitureObject furniture;
        private readonly float fromAngle;
        private readonly float toAngle;

        public RotateFurnitureCommand(FurnitureObject furniture, float fromAngle, float toAngle)
        {
            this.furniture = furniture;
            this.fromAngle = fromAngle;
            this.toAngle = toAngle;
        }

        public void Execute() => furniture.SetRotationY(toAngle);
        public void Undo() => furniture.SetRotationY(fromAngle);
    }
}