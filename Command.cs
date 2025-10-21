using SplashKitSDK;

namespace VLSEdit
{
    public abstract class Command
    {
        public abstract void Execute();
    }

    public class CloneCommand : Command
    {
        public CloneCommand()
        {
        }

        public override void Execute()
        {
            if (Editor.Instance.SelectedBoxWidget == null) return;

            Box newBox = Editor.Instance.SelectedBoxWidget.Box.Clone();

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
}