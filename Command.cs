using System.Security.Cryptography.X509Certificates;
using SplashKitSDK;
using VLSEdit;

namespace VLSEdit
{
    public abstract class Command
    {
        public abstract void Execute();
    }

    public class CloneBoxCommand : Command
    {
        private readonly Box? _box;

        public CloneBoxCommand(Box? box = null)
        {
            _box = box;
        }

        public override void Execute()
        {
            Box box;

            if (_box == null)
            {
                if (Editor.Instance.SelectedBoxWidget == null) return;

                box = Editor.Instance.SelectedBoxWidget.Box;
            }
            else
            {
                box = _box;
            }

            Box newBox = box.Clone();

            newBox.X = (SplashKit.MouseX() - Editor.Instance.View.OffsetX) / Editor.Instance.View.Scale;
            newBox.Y = (SplashKit.MouseY() - Editor.Instance.View.OffsetY) / Editor.Instance.View.Scale;

            Editor.Instance.AddBox(newBox);

            Editor.Instance.RegisterChange();
        }
    }

    public class SaveCommand : Command
    {
        public SaveCommand()
        {
        }

        public override void Execute()
        {
            Editor.Instance.Save();

            Editor.Instance.View.Alert("Saved successfully", 2000);
        }
    }

    public class DeleteBoxCommand : Command
    {
        private readonly BoxWidget _boxWidget;

        public DeleteBoxCommand(BoxWidget boxWidget)
        {
            _boxWidget = boxWidget;
        }

        public override void Execute()
        {
            Editor.Instance.RemoveBox(_boxWidget);
            Editor.Instance.SelectedBoxWidget = null;
            Editor.Instance.RegisterChange();
        }
    }

    public class SetBoxValueCommand : Command
    {
        private readonly Box _box;

        public SetBoxValueCommand(Box box)
        {
            _box = box;
        }

        public override void Execute()
        {
            if (_box is IValueSettable valueBox && _box.Mutable)
            {
                try
                {
                    string? result = GUIPrompt.Ask("Enter the new value for this box:");

                    if (result != null)
                    {
                        valueBox.SetValue(Value.FromString(valueBox.Value.GetType(), result));

                        Editor.Instance.RegisterChange();
                    }
                }
                catch
                {
                    Editor.Instance.View.Alert("Failed to parse input", 1500);
                }
            }
        }
    }

    public class ChangeStateCommand : Command
    {
        private bool _redo;

        public ChangeStateCommand(bool redo)
        {
            _redo = redo;
        }

        public override void Execute()
        {
            Editor.Instance.ChangeState(_redo);
        }
    }

    public class AutoCleanCommand : Command
    {
        public AutoCleanCommand()
        {
        }

        public override void Execute()
        {
            List<Box> boxList = new List<Box>();

            foreach (Box box in Editor.Instance.Script.Boxes)
            {
                if (box is not EventBox) continue;

                if (!box.Mutable) continue;

                CollectUseful(boxList, box);
            }

            int count = 0;

            for (int i = 0; i < Editor.Instance.View.BoxWidgets.Count; i++)
            {
                BoxWidget boxWidget = Editor.Instance.View.BoxWidgets[i];

                if (!boxWidget.Box.Mutable) continue;

                if (boxList.Contains(boxWidget.Box)) continue;

                new DeleteBoxCommand(boxWidget).Execute();

                i--;

                count++;
            }

            string es = "es";

            if (count == 1)
            {
                es = "";
            }

            Editor.Instance.View.Alert($"Removed {count} box{es}", 1500);
        }

        private void CollectUseful(List<Box> boxList, Box box)
        {
            if (boxList.Contains(box)) return;

            boxList.Add(box);

            if (box is InvokeBox invokeBox)
            {
                foreach (Box boxB in Editor.Instance.Script.Boxes)
                {
                    if (boxB is SubroutineBox subBox && subBox.Value.StringRepresentation == invokeBox.Value.StringRepresentation)
                    {
                        CollectUseful(boxList, boxB);
                        break;
                    }
                }
            }

            bool uselessEvent = box is EventBox;

            foreach (Node node in box.Nodes)
            {
                if (node is not ClientNode clientNode) continue;

                if (clientNode.To == null) continue;

                CollectUseful(boxList, clientNode.To.Box);

                uselessEvent = false;
            }

            if (uselessEvent)
            {
                boxList.Remove(box);
            }
        }
    }

    public class ScaleCommand : Command
    {
        private double _amount;

        private bool _fromCentre;

        public ScaleCommand(double amount, bool fromCentre = false)
        {
            _amount = amount;
            _fromCentre = fromCentre;
        }

        public override void Execute()
        {
            if (_amount == 0 || (Editor.Instance.Controller.DragState is not KVMControllerDragStateNone && Editor.Instance.Controller.DragState is not KVMControllerDragStateButton)) return;

            double newScale = Math.Min(Editor.Instance.View.Scale / (1 - Editor.Instance.View.Scale * _amount * 0.4), 1);

            if (newScale < 0 || newScale == double.NaN) return;

            double originX = SplashKit.MouseX();
            double originY = SplashKit.MouseY();

            if (_fromCentre)
            {
                originX = Constants.WINDOW_WIDTH / 2;
                originY = Constants.WINDOW_HEIGHT / 2;
            }

            Editor.Instance.View.OffsetX += (originX - Editor.Instance.View.OffsetX) * (1 - newScale / Editor.Instance.View.Scale);
            Editor.Instance.View.OffsetY += (originY - Editor.Instance.View.OffsetY) * (1 - newScale / Editor.Instance.View.Scale);

            Editor.Instance.View.Scale = newScale;
        }
    }
}