namespace VLSEdit
{
    public class UndoStateManager
    {
        private List<string> _states = new List<string>();

        private int _stateIndex = 0;

        public bool DocumentChanged
        {
            get
            {
                return _states.Count != 1;
            }
        }

        public UndoStateManager()
        {
        }

        public string? Undo()
        {
            if (_stateIndex <= 0) return null;
            _stateIndex--;
            return _states[_stateIndex];
        }

        public string? Redo()
        {
            if (_stateIndex + 1 >= _states.Count) return null;
            _stateIndex++;
            return _states[_stateIndex];
        }

        public void Change(string newState)
        {
            if (_states.Count > _stateIndex + 1)
            {
                _states.RemoveRange(_stateIndex + 1, _states.Count - _stateIndex - 1);
            }

            _stateIndex = _states.Count;

            _states.Add(newState);

            if (_states.Count > 30)
            {
                _states.RemoveAt(0);
            }
        }
    }
}