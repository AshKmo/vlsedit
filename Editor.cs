using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text.Encodings.Web;

namespace VLSEdit
{
    public class Editor
    {
        private static Editor? _instance = null;

        private KVMView _view = new KVMView();

        private KVMController _controller = new KVMController();

        private Script _script = new Script();

        private BoxWidget? _selectedBoxWidget;

        public BoxWidget? SelectedBoxWidget { get { return _selectedBoxWidget; } set { _selectedBoxWidget = value; } }

        public Script Script { get { return _script; } }

        public KVMView View { get { return _view; } }

        public KVMController Controller { get { return _controller; } }

        public static Editor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Editor();
                }

                return _instance;
            }
        }

        private Editor()
        {
            AddInitialBox(new NullBox());
            AddInitialBox(new AddBox());

            double newHeight = 20;

            foreach (BoxWidget boxWidget in _view.BoxWidgets)
            {
                boxWidget.X = 20;
                boxWidget.Y = newHeight;
                newHeight += boxWidget.Height + 15;
                boxWidget.Box.Mutable = false;
            }
        }

        public void OpenScript(string scriptPath)
        {
            _script = ScriptLoader.LoadScript(scriptPath);
        }

        public void Tick()
        {
            _controller.Tick();

            _view.Draw();
        }

        private void AddInitialBox(Box box)
        {
            _view.BoxWidgets.Add(new BoxWidget(box));
        }

        public void AddBox(Box box)
        {
            AddInitialBox(box);

            _script.Boxes.Add(box);
        }

        public void RemoveBox(BoxWidget boxWidget)
        {
            if (!_script.Boxes.Contains(boxWidget.Box)) return;

            foreach (Node node in boxWidget.Box.Nodes)
            {
                BreakLinksTo(node);
            }

            _view.BoxWidgets.Remove(boxWidget);
            _script.Boxes.Remove(boxWidget.Box);
        }

        public void BreakLinksTo(Node node)
        {
            foreach (Box box in _script.Boxes)
            {
                foreach (Node nodeB in box.Nodes)
                {
                    if (nodeB is not ClientNode) continue;

                    ClientNode clientNode = (ClientNode)nodeB;

                    if (clientNode.To == null) continue;

                    if (node == clientNode.To)
                    {
                        clientNode.To = null;
                    }
                }
            }
        }
    }
}