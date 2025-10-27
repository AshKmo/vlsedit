using SplashKitSDK;

namespace VLSEdit
{
    public class Editor
    {
        private static Editor? _instance = null;

        private string? _scriptPath = null;

        private KVMView _view = new KVMView();

        private KVMController _controller = new KVMController();

        private Script _script = new Script();

        private UndoStateManager _undoStateManager = new UndoStateManager();

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
            AddTemplateBoxes();
        }

        private void AddTemplateBoxes()
        {
            double newHeight = Constants.TOOLBAR_HEIGHT + 20;

            int column = 0;

            double biggestHeight = 0;

            foreach (BoxType boxType in Enum.GetValues<BoxType>())
            {
                Box newBox = Box.Create(boxType);
                newBox.X = 20 + column * (Constants.BOX_WIDTH + 20);
                newBox.Y = newHeight;
                newBox.Mutable = false;

                BoxWidget boxWidget = AddInitialBox(newBox);

                biggestHeight = Math.Max(biggestHeight, boxWidget.Height);

                column++;

                if (column == Constants.INITIAL_BOX_COLUMN_COUNT)
                {
                    newHeight += biggestHeight + 20;
                    column = 0;
                    biggestHeight = 0;
                }
            }
        }

        public void OpenScript(string scriptPath)
        {
            _scriptPath = scriptPath;

            try
            {
                _script = ScriptLoader.LoadScript(scriptPath);
            }
            catch
            {
                _script = new Script();
            }

            RegisterChange();
        }

        public void Tick()
        {
            _controller.Tick();

            _view.Draw();
        }

        private BoxWidget AddInitialBox(Box box)
        {
            BoxWidget boxWidget = new BoxWidget(box);
            _view.BoxWidgets.Add(boxWidget);
            return boxWidget;
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

        public void Save()
        {
            ScriptLoader.SaveScript(_scriptPath!, _script);
        }

        public void RegisterChange()
        {
            _undoStateManager.Change(ScriptLoader.UnparseScript(_script));
        }

        public bool ChangeState(bool redo)
        {
            string? oldState;

            if (redo)
            {
                oldState = _undoStateManager.Redo();
            }
            else
            {
                oldState = _undoStateManager.Undo();
            }

            if (oldState == null) return false;

            _script = new Script();

            _view.BoxWidgets = new List<BoxWidget>();

            AddTemplateBoxes();

            _script = ScriptLoader.ParseScript(oldState);

            return true;
        }
    }
}