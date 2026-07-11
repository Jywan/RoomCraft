using RoomCraft.Data;
using RoomCraft.Furniture;
using UnityEngine;

namespace RoomCraft.UndoRedo
{
    public class DeleteFurnitureCommand : ICommand
    {
        private readonly FurnitureInteraction interaction;
        private readonly FurnitureData data;
        private readonly Vector3 position;
        private readonly float rotationY;
        private GameObject targetObject;
        private FurnitureObject targetFurniture;


        public DeleteFurnitureCommand(FurnitureInteraction interaction, FurnitureObject furniture)
        {
            this.interaction = interaction;
            this.data = furniture.GetData();
            this.position = furniture.transform.position;
            this.rotationY = furniture.GetRotationY();
            this.targetObject = furniture.gameObject;
            this.targetFurniture = furniture;
        }

        public void Execute()
        {
            interaction.ClearSelectionIfMatches(targetFurniture);
            Object.Destroy(targetObject);
        }

        public void Undo()
        {
            FurnitureObject recreated = interaction.SpawnFurniture(data);
            recreated.MoveTo(position);
            recreated.SetRotationY(rotationY);
            targetObject = recreated.gameObject;
            targetFurniture = recreated;
        }
    }
}