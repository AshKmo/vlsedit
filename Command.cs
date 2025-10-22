using SplashKitSDK;

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

            newBox.X = SplashKit.MouseX() - Editor.Instance.View.OffsetX;
            newBox.Y = SplashKit.MouseY() - Editor.Instance.View.OffsetY;

            Editor.Instance.AddBox(newBox);
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
                    }
                }
                catch
                {
                    Editor.Instance.View.Alert("Failed to parse input", 1500);
                }
            }
        }
    }
}