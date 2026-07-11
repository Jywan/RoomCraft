using System.Collections.Generic;
using UnityEngine;

namespace RoomCraft.UndoRedo
{
    /// <summary>
    /// 커맨드 스택 기반 싱행 취소/다시 실행 관리자
    /// EditorScene에 하나만 존재
    /// </summary>
    public class UndoManager : MonoBehaviour
    {
        private readonly Stack<ICommand> undoStack = new Stack<ICommand>();
        private readonly Stack<ICommand> redoStack = new Stack<ICommand>();
        
        /// <summary>
        /// 지금 바로 실행하면서 기록 (예: 삭제)
        /// </summary>
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            undoStack.Push(command);
            redoStack.Clear();
        }
        
        /// <summary>
        /// 이미 실행된 종작을 기록만 함 (예: 드래그로 이미 옮겨진 가구)
        /// </summary>
        public void RecordCommand(ICommand command)
        {
            undoStack.Push(command);
            redoStack.Clear();
        }

        public void Undo()
        {
            if (undoStack.Count == 0) return;
            ICommand command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
        }

        public void Redo()
        {
            if (redoStack.Count == 0) return;
            ICommand command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }
    }
}