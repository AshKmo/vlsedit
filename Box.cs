using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using SplashKitSDK;

namespace VLSEdit
{
    public enum BoxType
    {
        Start,
        Sequence,
        State,
        Call,
        CallValue,
        Null,
        True,
        False,
        Integer,
        Double,
        String,
        Negate,
        ToNumber,
        Add,
        Subtract,
        Multiply,
        Divide,
        Equal,
        If,
        Print,
        Write,
        Ask,
    }

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

        public abstract BoxType Type { get; }

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
            writer.WriteLine(Type);

            writer.WriteLine(X);
            writer.WriteLine(Y);
        }

        public static Box FromString(StringReader reader)
        {
            Box newBox = Create(Enum.Parse<BoxType>(reader.ReadLine()!));

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

        public static Box Create(BoxType boxType)
        {
            Box? newBox = null;

            switch (boxType)
            {
                case BoxType.Null:
                    newBox = new NullBox();
                    break;
                case BoxType.Add:
                    newBox = new AddBox();
                    break;
                case BoxType.Start:
                    newBox = new StartBox();
                    break;
                case BoxType.Print:
                    newBox = new PrintBox();
                    break;
                case BoxType.Integer:
                    newBox = new IntegerBox(new IntegerValue(0));
                    break;
                case BoxType.Double:
                    newBox = new DoubleBox(new DoubleValue(0));
                    break;
                case BoxType.Subtract:
                    newBox = new SubtractBox();
                    break;
                case BoxType.Multiply:
                    newBox = new MultiplyBox();
                    break;
                case BoxType.Divide:
                    newBox = new DivideBox();
                    break;
                case BoxType.Negate:
                    newBox = new NegateBox();
                    break;
                case BoxType.If:
                    newBox = new IfBox();
                    break;
                case BoxType.Call:
                    newBox = new CallBox();
                    break;
                case BoxType.Ask:
                    newBox = new AskBox();
                    break;
                case BoxType.CallValue:
                    newBox = new CallValueBox();
                    break;
                case BoxType.Equal:
                    newBox = new EqualBox();
                    break;
                case BoxType.String:
                    newBox = new StringBox(new StringValue(""));
                    break;
                case BoxType.ToNumber:
                    newBox = new ToNumberBox();
                    break;
                case BoxType.State:
                    newBox = new StateBox();
                    break;
                case BoxType.Sequence:
                    newBox = new SequenceBox();
                    break;
                case BoxType.True:
                    newBox = new TrueBox();
                    break;
                case BoxType.False:
                    newBox = new FalseBox();
                    break;
                case BoxType.Write:
                    newBox = new WriteBox();
                    break;
            }

            if (newBox == null)
            {
                throw new Exception("box not recognised: " + boxType);
            }

            return newBox;
        }
    }

    public abstract class ValueBox : Box
    {
        protected readonly Node _valueNode;

        public override Color Color { get { return Color.Cyan; } }

        public override List<Node> Nodes { get { return new List<Node> { _valueNode }; } }

        protected ValueBox()
        {
            _valueNode = new ServerNode(Name, this);
        }

        public override void Serialise(StringWriter writer)
        {
            base.Serialise(writer);
        }

        public override void Deserialise(StringReader reader)
        {
        }
    }

    public abstract class SettableValueBox : ValueBox
    {
        private Value _value;

        public Value Value { get { return _value; } }

        protected SettableValueBox()
        {
            _value = new NullValue();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return Value;
        }

        public void SetValue(Value value)
        {
            _value = value;

            if (Value.StringRepresentation.Length > 20)
            {
                _valueNode.Name = value.StringRepresentation.Substring(0, Math.Min(17, value.StringRepresentation.Length)) + "...";
            }
            else
            {
                _valueNode.Name = value.StringRepresentation;
            }
        }

        public override void Serialise(StringWriter writer)
        {
            base.Serialise(writer);

            writer.WriteLine(Value.StringRepresentation);
        }
    }

    public class IntegerBox : SettableValueBox
    {
        public override string Name { get { return "Integer"; } }

        public override BoxType Type { get { return BoxType.Integer; } }

        public IntegerBox(IntegerValue value)
        {
            SetValue(value);
        }

        public override IntegerBox Clone()
        {
            return new IntegerBox((IntegerValue)Value);
        }

        public override void Deserialise(StringReader reader)
        {
            SetValue(new IntegerValue(0).NewFromString(reader.ReadLine()!));
        }
    }

    public class DoubleBox : SettableValueBox
    {
        public override string Name { get { return "Double"; } }

        public override BoxType Type { get { return BoxType.Double; } }

        public DoubleBox(DoubleValue value)
        {
            SetValue(value);
        }

        public override DoubleBox Clone()
        {
            return new DoubleBox((DoubleValue)Value);
        }

        public override void Deserialise(StringReader reader)
        {
            SetValue(new DoubleValue(0).NewFromString(reader.ReadLine()!));
        }
    }

    public class StringBox : SettableValueBox
    {
        public override string Name { get { return "String"; } }

        public override BoxType Type { get { return BoxType.String; } }

        public StringBox(StringValue value)
        {
            SetValue(value);
        }

        public override StringBox Clone()
        {
            return new StringBox((StringValue)Value);
        }

        public override void Deserialise(StringReader reader)
        {
            SetValue(new StringValue(reader.ReadLine()!));
        }
    }

    public class NullBox : ValueBox
    {
        public override string Name { get { return "Null"; } }

        public override BoxType Type { get { return BoxType.Null; } }

        public NullBox()
        {
        }

        public override NullBox Clone()
        {
            return new NullBox();
        }
    }

    public abstract class BoolBox : ValueBox
    {
        public abstract bool Value { get; }

        public override Value Interpret(Value context, ServerNode node)
        {
            return new BoolValue(Value);
        }
    }

    public class TrueBox : BoolBox
    {
        public override string Name { get { return "True"; } }

        public override BoxType Type { get { return BoxType.True; } }

        public override bool Value { get { return true; } }

        public override TrueBox Clone()
        {
            return new TrueBox();
        }
    }

    public class FalseBox : BoolBox
    {
        public override string Name { get { return "False"; } }
        
        public override BoxType Type { get { return BoxType.False; } }

        public override bool Value { get { return false; } }

        public override FalseBox Clone()
        {
            return new FalseBox();
        }
    }

    public abstract class OperatorBox : Box
    {
        public override Color Color { get { return Color.DeepSkyBlue; } }
    }

    public abstract class BinaryOpBox : OperatorBox
    {
        private ClientNode _aNode;

        private ClientNode _bNode;

        private ServerNode _resultNode;

        protected BinaryOpBox(string resultName)
        {
            _aNode = new ClientNode("A");
            _bNode = new ClientNode("B");

            _resultNode = new ServerNode(resultName, this);
        }

        public override List<Node> Nodes { get { return new List<Node> { _aNode, _bNode, _resultNode }; } }

        public override Value Interpret(Value context, ServerNode node)
        {
            return Operate((NumberValue)_aNode.InterpretTarget(context), (NumberValue)_bNode.InterpretTarget(context));
        }

        public abstract NumberValue Operate(NumberValue a, NumberValue b);
    }

    public class AddBox : BinaryOpBox
    {
        public override string Name { get { return "Add"; } }

        public override BoxType Type { get { return BoxType.Add; } }

        public AddBox() : base("A + B")
        {
        }

        public override AddBox Clone()
        {
            return new AddBox();
        }

        public override NumberValue Operate(NumberValue a, NumberValue b)
        {
            return a.Add(b);
        }
    }

    public class MultiplyBox : BinaryOpBox
    {
        public override string Name { get { return "Multiply"; } }

        public override BoxType Type { get { return BoxType.Multiply; } }

        public MultiplyBox() : base("A * B")
        {
        }

        public override MultiplyBox Clone()
        {
            return new MultiplyBox();
        }

        public override NumberValue Operate(NumberValue a, NumberValue b)
        {
            return a.Multiply(b);
        }
    }

    public class DivideBox : BinaryOpBox
    {
        public override string Name { get { return "Divide"; } }

        public override BoxType Type { get { return BoxType.Divide; } }

        public DivideBox() : base("A / B")
        {
        }

        public override DivideBox Clone()
        {
            return new DivideBox();
        }

        public override NumberValue Operate(NumberValue a, NumberValue b)
        {
            return a.Divide(b);
        }
    }

    public class SubtractBox : BinaryOpBox
    {
        public override string Name { get { return "Subtract"; } }

        public override BoxType Type { get { return BoxType.Subtract; } }

        public SubtractBox() : base("A - B")
        {
        }

        public override SubtractBox Clone()
        {
            return new SubtractBox();
        }

        public override NumberValue Operate(NumberValue a, NumberValue b)
        {
            return a.Add(b.Negative());
        }
    }

    public class NegateBox : OperatorBox
    {
        private ClientNode _inputNode;

        private ServerNode _resultNode;

        public override string Name { get { return "Negate"; } }

        public override BoxType Type { get { return BoxType.Negate; } }

        public override List<Node> Nodes { get { return new List<Node> { _inputNode, _resultNode }; } }

        public NegateBox()
        {
            _inputNode = new ClientNode("X");

            _resultNode = new ServerNode("-X", this);
        }

        public override NegateBox Clone()
        {
            return new NegateBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return ((NumberValue)_inputNode.InterpretTarget(context)).Negative();
        }
    }

    public class ToNumberBox : OperatorBox
    {
        private ClientNode _inputNode;

        private ServerNode _resultNode;

        public override string Name { get { return "String To Number"; } }

        public override BoxType Type { get { return BoxType.ToNumber; } }

        public override List<Node> Nodes { get { return new List<Node> { _inputNode, _resultNode }; } }

        public ToNumberBox()
        {
            _inputNode = new ClientNode("String");

            _resultNode = new ServerNode("Number", this);
        }

        public override ToNumberBox Clone()
        {
            return new ToNumberBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            try
            {
                return new IntegerValue(0).NewFromString(((StringValue)_inputNode.InterpretTarget(context)).StringRepresentation);
            }
            catch
            {
                return new DoubleValue(0).NewFromString(((StringValue)_inputNode.InterpretTarget(context)).StringRepresentation);
            }
        }
    }

    public class EqualBox : OperatorBox
    {
        private ClientNode _aNode;

        private ClientNode _bNode;

        private ServerNode _resultNode;

        public override string Name { get { return "Are Equal?"; } }

        public override BoxType Type { get { return BoxType.Equal; } }

        public override List<Node> Nodes { get { return new List<Node> { _resultNode, _aNode, _bNode }; } }

        public EqualBox()
        {
            _aNode = new ClientNode("A");
            _bNode = new ClientNode("B");

            _resultNode = new ServerNode("A == B", this);
        }

        public override EqualBox Clone()
        {
            return new EqualBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return new BoolValue(_aNode.InterpretTarget(context).IsEqualTo(_bNode.InterpretTarget(context)));
        }
    }

    public class IfBox : OperatorBox
    {
        private ClientNode _trueNode;

        private ClientNode _falseNode;

        private ClientNode _conditionNode;

        private ServerNode _resultNode;

        public override string Name { get { return "If"; } }

        public override BoxType Type { get { return BoxType.If; } }

        public override List<Node> Nodes { get { return new List<Node> { _resultNode, _conditionNode, _trueNode, _falseNode }; } }

        public IfBox()
        {
            _trueNode = new ClientNode("If True");
            _falseNode = new ClientNode("Else");
            _conditionNode = new ClientNode("Condition");

            _resultNode = new ServerNode("Result", this);
        }

        public override IfBox Clone()
        {
            return new IfBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            if (((BoolValue)_conditionNode.InterpretTarget(context)).Value)
            {
                return _trueNode.InterpretTarget(context);
            }

            return _falseNode.InterpretTarget(context);
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

        public override BoxType Type { get { return BoxType.Start; } }

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

    public abstract class OutputBox : ActionBox
    {
        private ServerNode _eventNode;

        private ClientNode _outputNode;

        public override List<Node> Nodes { get { return new List<Node> { _eventNode, _outputNode }; } }

        public OutputBox()
        {
            _eventNode = new ServerNode("Value", this);

            _outputNode = new ClientNode("Output");
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            Value result = _outputNode.InterpretTarget(context);

            Output(result.StringRepresentation);

            return result;
        }

        public abstract void Output(string output);
    }

    public class PrintBox : OutputBox
    {
        public override string Name { get { return "Print"; } }

        public override BoxType Type { get { return BoxType.Print; } }

        public PrintBox()
        {
        }

        public override PrintBox Clone()
        {
            return new PrintBox();
        }

        public override void Output(string output)
        {
            Console.WriteLine(output);
        }
    }

    public class WriteBox : OutputBox
    {
        public override string Name { get { return "Write"; } }

        public override BoxType Type { get { return BoxType.Write; } }

        public WriteBox()
        {
        }

        public override WriteBox Clone()
        {
            return new WriteBox();
        }

        public override void Output(string output)
        {
            Console.Write(output);
        }
    }

    public class AskBox : ActionBox
    {
        private ServerNode _resultNode;

        private ClientNode _questionNode;

        public override string Name { get { return "Ask"; } }

        public override BoxType Type { get { return BoxType.Ask; } }

        public override List<Node> Nodes { get { return new List<Node> { _resultNode, _questionNode }; } }

        public AskBox()
        {
            _resultNode = new ServerNode("Answer", this);

            _questionNode = new ClientNode("Question");
        }

        public override AskBox Clone()
        {
            return new AskBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            Value question = _questionNode.InterpretTarget(context);

            Console.Write(question.StringRepresentation);

            StringValue result = new StringValue(Console.ReadLine() ?? "");

            return result;
        }
    }

    public abstract class PatchBox : Box
    {
        public override Color Color { get { return Color.Yellow; } }
    }

    public class CallBox : PatchBox
    {
        private ServerNode _resultNode;

        private ClientNode _calleeNode;

        private ClientNode _inputNode;

        public override string Name { get { return "Call"; } }

        public override BoxType Type { get { return BoxType.Call; } }

        public override List<Node> Nodes { get { return new List<Node> { _resultNode, _calleeNode, _inputNode }; } }

        public CallBox()
        {
            _resultNode = new ServerNode("Result", this);

            _calleeNode = new ClientNode("Target");

            _inputNode = new ClientNode("Argument");
        }

        public override CallBox Clone()
        {
            return new CallBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return _calleeNode.InterpretTarget(_inputNode.InterpretTarget(context));
        }
    }

    public class CallValueBox : PatchBox
    {
        private ServerNode _resultNode;

        public override string Name { get { return "Call Value"; } }

        public override BoxType Type { get { return BoxType.CallValue; } }

        public override List<Node> Nodes { get { return new List<Node> { _resultNode }; } }

        public CallValueBox()
        {
            _resultNode = new ServerNode("Call Value", this);
        }

        public override CallValueBox Clone()
        {
            return new CallValueBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return context;
        }
    }

    public class StateBox : PatchBox
    {
        private Value _state = new NullValue();

        private ServerNode _setNode;

        private ServerNode _getNode;

        public override string Name { get { return "State"; } }

        public override BoxType Type { get { return BoxType.State; } }

        public override List<Node> Nodes { get { return new List<Node> { _setNode, _getNode }; } }

        public StateBox()
        {
            _setNode = new ServerNode("Set", this);

            _getNode = new ServerNode("Get", this);
        }

        public override StateBox Clone()
        {
            return new StateBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            if (node == _setNode)
            {
                return _state = context;
            }

            return _state;
        }
    }

    public class SequenceBox : PatchBox
    {
        private ServerNode _triggerNode;

        private ClientNode _firstAction;

        private ClientNode _secondAction;

        public override string Name { get { return "Sequence"; } }

        public override BoxType Type { get { return BoxType.Sequence; } }

        public override List<Node> Nodes { get { return new List<Node> { _triggerNode, _firstAction, _secondAction }; } }

        public SequenceBox()
        {
            _triggerNode = new ServerNode("Value", this);

            _firstAction = new ClientNode("Discard");
            _secondAction = new ClientNode("Return");
        }

        public override SequenceBox Clone()
        {
            return new SequenceBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            _firstAction.InterpretTarget(context);

            return _secondAction.InterpretTarget(context);
        }
    }
}