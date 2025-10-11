using SplashKitSDK;

namespace VLSEdit
{
    public abstract class Box
    {
        private List<Node> _nodes = new List<Node>();

        private double _x;

        private double _y;

        private bool _mutable = true;

        public List<Node> Nodes { get { return _nodes; } }

        public abstract string Name { get; }

        public abstract Color Color { get; }

        public bool Mutable { get { return _mutable; } set { _mutable = value; } }

        public double X
        {
            get { return _x; }

            set
            {
                if (_mutable)
                {
                    _x = value;
                }
            }
        }

        public double Y
        {
            get { return _y; }

            set
            {
                if (_mutable)
                {
                    _y = value;
                }
            }
        }

        public abstract Box Clone();

        public virtual void Serialise(StreamWriter writer)
        {
        }

        public static Box Deserialise(StreamReader reader)
        {
            return new NullBox();
        }

        public abstract Value Interpret(Value context, ServerNode node);
    }

    public class NullBox : Box
    {
        public override string Name { get { return "Null"; } }

        public override Color Color { get { return Color.Cyan; } }

        public NullBox()
        {
            Nodes.Add(new ServerNode("Null"));
        }

        public override NullBox Clone()
        {
            return new NullBox();
        }

        public override void Serialise(StreamWriter writer)
        {
            base.Serialise(writer);
        }

        public static void FromString(StreamReader reader)
        {
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return new NullValue();
        }
    }

    public class AddBox : Box
    {
        public override string Name { get { return "Add"; } }

        public override Color Color { get { return Color.DeepSkyBlue; } }

        public AddBox()
        {
            Nodes.Add(new ClientNode("A"));
            Nodes.Add(new ClientNode("B"));

            Nodes.Add(new ServerNode("Sum"));
        }

        public override AddBox Clone()
        {
            return new AddBox();
        }

        public override void Serialise(StreamWriter writer)
        {
            base.Serialise(writer);
        }

        public static void FromString(StreamReader reader)
        {
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return new NullValue();
        }
    }
}