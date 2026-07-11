namespace RoomCraft.UndoRedo
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}