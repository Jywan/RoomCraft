using RoomCraft.Data;
using RoomCraft.Furniture;
using UnityEngine;

namespace RoomCraft.UndoRedo
{
    public class CreateFurnitureCommand : ICommand
    {
        private readonly FurnitureInteraction interaction;
        private readonly FurnitureData data;
        private readonly Vector3 position;
        private readonly float rotationY;
        private FurnitureObject created;

        public CreateFurnitureCommand(FurnitureInteraction interaction, FurnitureData data, Vector3 position,
            float rotationY, FurnitureObject alreadyCreated = null)
        {
            this.interaction = interaction;
            this.data = data;
            this.position = position;
            this.rotationY = rotationY;
            this.created = alreadyCreated;
        }

        public void Execute()
        {
            created = interaction.SpawnFurniture(data);
            created.MoveTo(position);
            created.SetRotationY(rotationY);
        }

        public void Undo()
        {
            if (created != null)
            {
                interaction.ClearSelectionIfMatches(created);
                Object.Destroy(created.gameObject);
            }
        }
    }
}