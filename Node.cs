namespace VLSEdit
{
    public abstract class Node
    {
        private string _name;

        public string Name { get { return _name; } }

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
    }

    public class ServerNode : Node
    {
        private Guid _id = Guid.NewGuid();

        public Guid ID { get { return _id; } }

        public ServerNode(string name) : base(name)
        {
        }
    }
}