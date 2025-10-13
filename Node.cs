namespace VLSEdit
{
    public abstract class Node
    {
        private string _name;

        public string Name { get { return _name; } set { _name = value; } }

        protected Node(string name)
        {
            _name = name;
        }
    }

    public class ClientNode : Node
    {
        private ServerNode? _to;

        public ServerNode? To { get { return _to; } set { _to = value; } }

        public ClientNode(string name) : base(name)
        {
        }

        public Value InterpretTarget(Value context)
        {
            if (_to == null)
            {
                return new NullValue();
            }

            return _to.InterpretBox(context);
        }
    }

    public class ServerNode : Node
    {
        private Box _box;

        private Guid _id = Guid.NewGuid();

        public Guid ID { get { return _id; } }

        public ServerNode(string name, Box box) : base(name)
        {
            _box = box;
        }

        public Value InterpretBox(Value context)
        {
            return _box.Interpret(context, this);
        }
    }
}