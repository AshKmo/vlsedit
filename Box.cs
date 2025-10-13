using System.Reflection.Metadata.Ecma335;
using SplashKitSDK;

namespace VLSEdit
{
    public abstract class Box
    {
        private List<Node> _nodes = new List<Node>();

        private double _x;

        private double _y;

        private bool _mutable = true;

        public abstract List<Node> Nodes { get; }

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

        public virtual void Serialise(StringWriter writer)
        {
            writer.WriteLine(Name);

            writer.WriteLine(X);
            writer.WriteLine(Y);
        }

        public static Box FromString(StringReader reader)
        {
            Box? newBox = null;

            string boxName = reader.ReadLine()!;

            switch (boxName)
            {
                case "Null":
                    newBox = new NullBox();
                    break;
                case "Add":
                    newBox = new AddBox();
                    break;
                case "Program Start":
                    newBox = new StartBox();
                    break;
                case "Print":
                    newBox = new PrintBox();
                    break;
                case "Integer":
                    newBox = new IntegerBox(new IntegerValue(0));
                    break;
                case "Double":
                    newBox = new DoubleBox(new DoubleValue(0));
                    break;
            }

            if (newBox == null)
            {
                throw new Exception("box not recognised: " + boxName);
            }

            newBox.X = Double.Parse(reader.ReadLine()!);
            newBox.Y = Double.Parse(reader.ReadLine()!);

            newBox.Deserialise(reader);

            return newBox;
        }

        public virtual void Deserialise(StringReader reader)
        {
        }

        public virtual Value Interpret(Value context, ServerNode node)
        {
            return new NullValue();
        }
    }

    public abstract class ValueBox : Box
    {
        private Value _value;

        protected ServerNode _valueNode;

        public override List<Node> Nodes { get { return new List<Node> { _valueNode }; } }

        public override Color Color { get { return Color.Cyan; } }

        public Value Value { get { return _value; } }

        protected ValueBox()
        {
            _value = new NullValue();

            _valueNode = new ServerNode("", this);
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return Value;
        }

        public virtual void SetValue(Value value)
        {
            _value = value;

            _valueNode.Name = value.StringRepresentation;
        }
    }

    public class IntegerBox : ValueBox
    {
        public override string Name { get { return "Integer"; } }

        public IntegerBox(IntegerValue value)
        {
            SetValue(value);
        }

        public override IntegerBox Clone()
        {
            return new IntegerBox((IntegerValue)Value);
        }

        public override void Serialise(StringWriter writer)
        {
            base.Serialise(writer);

            writer.WriteLine(Value.StringRepresentation);
        }

        public override void Deserialise(StringReader reader)
        {
            SetValue(new IntegerValue(0).NewFromString(reader.ReadLine()!));
        }
    }

    public class DoubleBox : ValueBox
    {
        public override string Name { get { return "Double"; } }

        public DoubleBox(DoubleValue value)
        {
            SetValue(value);
        }

        public override DoubleBox Clone()
        {
            return new DoubleBox((DoubleValue)Value);
        }

        public override void Serialise(StringWriter writer)
        {
            base.Serialise(writer);

            writer.WriteLine(Value.StringRepresentation);
        }

        public override void Deserialise(StringReader reader)
        {
            SetValue(new DoubleValue(0).NewFromString(reader.ReadLine()!));
        }
    }

    public class NullBox : Box
    {
        private ServerNode _valueNode;

        public override string Name { get { return "Null"; } }

        public override List<Node> Nodes { get { return new List<Node> { _valueNode }; } }

        public override Color Color { get { return Color.Cyan; } }

        public NullBox()
        {
            _valueNode = new ServerNode("Null", this);
        }

        public override NullBox Clone()
        {
            return new NullBox();
        }

        public override void Serialise(StringWriter writer)
        {
            base.Serialise(writer);
        }

        public override void Deserialise(StringReader reader)
        {
        }
    }

    public class AddBox : Box
    {
        private ClientNode _aNode;

        private ClientNode _bNode;

        private ServerNode _resultNode;

        public override string Name { get { return "Add"; } }

        public override List<Node> Nodes { get { return new List<Node> { _aNode, _bNode, _resultNode }; } }

        public override Color Color { get { return Color.DeepSkyBlue; } }

        public AddBox()
        {
            _aNode = new ClientNode("A");
            _bNode = new ClientNode("B");

            _resultNode = new ServerNode("Sum", this);
        }

        public override AddBox Clone()
        {
            return new AddBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return ((NumberValue)_aNode.InterpretTarget(context)).Add((NumberValue)_bNode.InterpretTarget(context));
        }
    }

    public abstract class EventBox : Box
    {
        protected ClientNode _node;

        public override Color Color { get { return Color.LimeGreen; } }

        public override List<Node> Nodes { get { return new List<Node> { _node }; } }

        protected EventBox()
        {
            _node = new ClientNode("");
        }

        public void Trigger(Value context)
        {
            _node.InterpretTarget(context);
        }
    }

    public class StartBox : EventBox
    {
        public override string Name { get { return "Program Start"; } }

        public StartBox()
        {
        }

        public override StartBox Clone()
        {
            return new StartBox();
        }
    }

    public abstract class ActionBox : Box
    {
        public override Color Color { get { return SplashKit.RGBColor(255, 143, 143); } }
    }

    public class PrintBox : ActionBox
    {
        private ServerNode _eventNode;

        private ClientNode _outputNode;

        public override string Name { get { return "Print"; } }

        public override List<Node> Nodes { get { return new List<Node> { _eventNode, _outputNode }; } }

        public PrintBox()
        {
            _eventNode = new ServerNode("Event", this);

            _outputNode = new ClientNode("Output");
        }

        public override PrintBox Clone()
        {
            return new PrintBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            Value result = _outputNode.InterpretTarget(context);

            Console.WriteLine(result.StringRepresentation);

            return result;
        }
    }
}