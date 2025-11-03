namespace VLSEdit
{
    public abstract class Node
    {
        private string _name;

        private string _description;

        public string Name { get { return _name; } set { _name = value; } }

        public string Description { get { return _description; } }

        protected Node(string name, string desc)
        {
            _name = name;
            _description = desc;
        }
    }

    public class ClientNode : Node
    {
        private ServerNode? _to;

        public ServerNode? To { get { return _to; } set { _to = value; } }

        public ClientNode(string name, string desc) : base(name, desc)
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

        public Box Box { get { return _box; } }

        public ServerNode(string name, string desc, Box box) : base(name, desc)
        {
            _box = box;
        }

        public Value InterpretBox(Value context)
        {
            return _box.Interpret(context, this);
        }
    }
}