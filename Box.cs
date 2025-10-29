using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using SplashKitSDK;

namespace VLSEdit
{
    public enum BoxType
    {
        Start,
        Subroutine,
        Sequence,
        State,
        Call,
        CallValue,
        Invoke,
        If,
        While,
        Print,
        Write,
        Ask,
        Null,
        True,
        False,
        Integer,
        Double,
        Random,
        String,
        List,
        Equal,
        TypesEqual,
        And,
        Or,
        Not,
        Negate,
        Length,
        LT,
        LTE,
        ToNumber,
        ToString,
        Add,
        Subtract,
        Multiply,
        Divide,
        Remainder,
        Concat,
        Substring,
        ListAdd,
        ListConcat,
        ListCdr,
        ListIndex,
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
                case BoxType.Remainder:
                    newBox = new RemainderBox();
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
                case BoxType.ToString:
                    newBox = new ToStringBox();
                    break;
                case BoxType.And:
                    newBox = new AndBox();
                    break;
                case BoxType.Or:
                    newBox = new OrBox();
                    break;
                case BoxType.Not:
                    newBox = new NotBox();
                    break;
                case BoxType.LT:
                    newBox = new LTBox();
                    break;
                case BoxType.LTE:
                    newBox = new LTEBox();
                    break;
                case BoxType.Concat:
                    newBox = new ConcatBox();
                    break;
                case BoxType.Substring:
                    newBox = new SubstringBox();
                    break;
                case BoxType.List:
                    newBox = new ListBox();
                    break;
                case BoxType.ListIndex:
                    newBox = new ListIndexBox();
                    break;
                case BoxType.ListAdd:
                    newBox = new ListAddBox();
                    break;
                case BoxType.ListCdr:
                    newBox = new ListCdrBox();
                    break;
                case BoxType.Length:
                    newBox = new LengthBox();
                    break;
                case BoxType.TypesEqual:
                    newBox = new TypesEqualBox();
                    break;
                case BoxType.Subroutine:
                    newBox = new SubroutineBox();
                    break;
                case BoxType.Invoke:
                    newBox = new InvokeBox();
                    break;
                case BoxType.ListConcat:
                    newBox = new ListConcatBox();
                    break;
                case BoxType.While:
                    newBox = new WhileBox();
                    break;
                case BoxType.Random:
                    newBox = new RandomBox();
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

    public interface IValueSettable
    {
        public Value Value { get; }

        public void SetValue(Value value);
    }

    public abstract class SettableValueBox : ValueBox, IValueSettable
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
            SetValue(IntegerValue.SubFromString(reader.ReadLine()!));
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
            SetValue(DoubleValue.SubFromString(reader.ReadLine()!));
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

    public class ListBox : ValueBox
    {
        public override string Name { get { return "Empty List"; } }

        public override BoxType Type { get { return BoxType.List; } }

        public ListBox()
        {
        }

        public override ListBox Clone()
        {
            return new ListBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return new ListValue(new List<Value>());
        }
    }

    public class RandomBox : ValueBox
    {
        public override BoxType Type { get { return BoxType.Random; } }

        public override string Name { get { return "Random"; } }

        public override Value Interpret(Value context, ServerNode node)
        {
            return new DoubleValue(new Random().NextDouble());
        }

        public override RandomBox Clone()
        {
            return new RandomBox();
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

        protected BinaryOpBox(string resultName, string inputNameA = "A", string inputNameB = "B")
        {
            _aNode = new ClientNode(inputNameA);
            _bNode = new ClientNode(inputNameB);

            _resultNode = new ServerNode(resultName, this);
        }

        public override List<Node> Nodes { get { return new List<Node> { _aNode, _bNode, _resultNode }; } }

        public override Value Interpret(Value context, ServerNode node)
        {
            return Operate((Value)_aNode.InterpretTarget(context), (Value)_bNode.InterpretTarget(context));
        }

        public abstract Value Operate(Value a, Value b);
    }

    public abstract class BinaryMathOpBox : BinaryOpBox
    {
        protected BinaryMathOpBox(string resultName) : base(resultName)
        {
        }

        public override Value Operate(Value a, Value b)
        {
            return Operate((NumberValue)a, (NumberValue)b);
        }

        public abstract Value Operate(NumberValue a, NumberValue b);
    }

    public class AddBox : BinaryMathOpBox
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

    public class RemainderBox : BinaryMathOpBox
    {
        public override string Name { get { return "Remainder"; } }

        public override BoxType Type { get { return BoxType.Remainder; } }

        public RemainderBox() : base("A % B")
        {
        }

        public override RemainderBox Clone()
        {
            return new RemainderBox();
        }

        public override NumberValue Operate(NumberValue a, NumberValue b)
        {
            return ((IntegerValue)a).Remainder((IntegerValue)b);
        }
    }

    public class ConcatBox : BinaryOpBox
    {
        public override string Name { get { return "Join Strings"; } }

        public override BoxType Type { get { return BoxType.Concat; } }

        public ConcatBox() : base("AB")
        {
        }

        public override ConcatBox Clone()
        {
            return new ConcatBox();
        }

        public override StringValue Operate(Value a, Value b)
        {
            return ((StringValue)a).Concat((StringValue)b);
        }
    }

    public class ListAddBox : BinaryOpBox
    {
        public override string Name { get { return "Add to List"; } }

        public override BoxType Type { get { return BoxType.ListAdd; } }

        public ListAddBox() : base("New list", "List", "Element")
        {
        }

        public override ListAddBox Clone()
        {
            return new ListAddBox();
        }

        public override ListValue Operate(Value a, Value b)
        {
            if (a is NullValue)
            {
                a = new ListValue(new List<Value>());
            }

            return ((ListValue)a).Add(b);
        }
    }

    public class ListConcatBox : BinaryOpBox
    {
        public override string Name { get { return "Combine Lists"; } }

        public override BoxType Type { get { return BoxType.ListConcat; } }

        public ListConcatBox() : base("Combined", "List A", "List B")
        {
        }

        public override ListConcatBox Clone()
        {
            return new ListConcatBox();
        }

        public override ListValue Operate(Value a, Value b)
        {
            return ((ListValue)a).Concat((ListValue)b);
        }
    }

    public class ListIndexBox : BinaryOpBox
    {
        public override string Name { get { return "Index List"; } }

        public override BoxType Type { get { return BoxType.ListIndex; } }

        public ListIndexBox() : base("Element", "List", "Index")
        {
        }

        public override ListIndexBox Clone()
        {
            return new ListIndexBox();
        }

        public override Value Operate(Value a, Value b)
        {
            if (b is not IntegerValue || a is not ListValue)
            {
                return new NullValue();
            }

            return ((ListValue)a).Index((IntegerValue)b);
        }
    }

    public class ListCdrBox : UnaryOpBox
    {
        public override string Name { get { return "List Tail"; } }

        public override BoxType Type { get { return BoxType.ListCdr; } }

        public ListCdrBox() : base("List", "Tail")
        {
        }

        public override ListCdrBox Clone()
        {
            return new ListCdrBox();
        }

        public override Value Operate(Value a)
        {
            return ((ListValue)a).Cdr();
        }
    }

    public class LTBox : BinaryMathOpBox
    {
        public override string Name { get { return "Less Than"; } }

        public override BoxType Type { get { return BoxType.LT; } }

        public LTBox() : base("A < B")
        {
        }

        public override LTBox Clone()
        {
            return new LTBox();
        }

        public override BoolValue Operate(NumberValue a, NumberValue b)
        {
            return new BoolValue(a.LessThan(b));
        }
    }

    public class LTEBox : BinaryMathOpBox
    {
        public override string Name { get { return "Less Than or Equal"; } }

        public override BoxType Type { get { return BoxType.LTE; } }

        public LTEBox() : base("A <= B")
        {
        }

        public override LTEBox Clone()
        {
            return new LTEBox();
        }

        public override BoolValue Operate(NumberValue a, NumberValue b)
        {
            return new BoolValue(a.LessThan(b) || a.IsEqualTo(b));
        }
    }

    public abstract class LogicBox : BinaryOpBox
    {
        protected LogicBox(string resultName) : base(resultName)
        {
        }

        public override Value Operate(Value a, Value b)
        {
            return Operate((BoolValue)a, (BoolValue)b);
        }

        public abstract BoolValue Operate(BoolValue a, BoolValue b);
    }

    public class AndBox : LogicBox
    {
        public override string Name { get { return "And"; } }

        public override BoxType Type { get { return BoxType.And; } }

        public AndBox() : base("A && B")
        {
        }

        public override AndBox Clone()
        {
            return new AndBox();
        }

        public override BoolValue Operate(BoolValue a, BoolValue b)
        {
            return new BoolValue(a.Value && b.Value);
        }
    }

    public class OrBox : LogicBox
    {
        public override string Name { get { return "Or"; } }

        public override BoxType Type { get { return BoxType.Or; } }

        public OrBox() : base("A || B")
        {
        }

        public override OrBox Clone()
        {
            return new OrBox();
        }

        public override BoolValue Operate(BoolValue a, BoolValue b)
        {
            return new BoolValue(a.Value || b.Value);
        }
    }

    public class MultiplyBox : BinaryMathOpBox
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

    public class DivideBox : BinaryMathOpBox
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

    public class SubtractBox : BinaryMathOpBox
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

    public abstract class UnaryOpBox : OperatorBox
    {
        private ClientNode _inputNode;

        private ServerNode _resultNode;

        public override List<Node> Nodes { get { return new List<Node> { _inputNode, _resultNode }; } }

        protected UnaryOpBox(string inputName, string outputName)
        {
            _inputNode = new ClientNode(inputName);

            _resultNode = new ServerNode(outputName, this);
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return Operate(_inputNode.InterpretTarget(context));
        }

        public abstract Value Operate(Value x);
    }

    public class NegateBox : UnaryOpBox
    {
        public override string Name { get { return "Negate"; } }

        public override BoxType Type { get { return BoxType.Negate; } }

        public NegateBox() : base("X", "-X")
        {
        }

        public override NegateBox Clone()
        {
            return new NegateBox();
        }

        public override Value Operate(Value x)
        {
            return ((NumberValue)x).Negative();
        }
    }

    public class LengthBox : UnaryOpBox
    {
        public override string Name { get { return "Length of"; } }

        public override BoxType Type { get { return BoxType.Length; } }

        public LengthBox() : base("Any", "Length")
        {
        }

        public override LengthBox Clone()
        {
            return new LengthBox();
        }

        public override IntegerValue Operate(Value x)
        {
            return new IntegerValue(x.Length);
        }
    }

    public class ToNumberBox : UnaryOpBox
    {
        public override string Name { get { return "String To Number"; } }

        public override BoxType Type { get { return BoxType.ToNumber; } }

        public ToNumberBox() : base("String", "Number")
        {
        }

        public override ToNumberBox Clone()
        {
            return new ToNumberBox();
        }

        public override Value Operate(Value x)
        {
            try
            {
                return IntegerValue.SubFromString(x.StringRepresentation);
            }
            catch
            {
                return DoubleValue.SubFromString(x.StringRepresentation);
            }
        }
    }

    public class ToStringBox : UnaryOpBox
    {
        public override string Name { get { return "Any To String"; } }

        public override BoxType Type { get { return BoxType.ToString; } }

        public ToStringBox() : base("Any", "String")
        {
        }

        public override ToStringBox Clone()
        {
            return new ToStringBox();
        }

        public override Value Operate(Value x)
        {
            return new StringValue(x.StringRepresentation);
        }
    }

    public class NotBox : UnaryOpBox
    {
        public override string Name { get { return "Not"; } }

        public override BoxType Type { get { return BoxType.Not; } }

        public NotBox() : base("X", "!X")
        {
        }

        public override NotBox Clone()
        {
            return new NotBox();
        }

        public override Value Operate(Value x)
        {
            return new BoolValue(!((BoolValue)x).Value);
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

    public class TypesEqualBox : OperatorBox
    {
        private ClientNode _aNode;

        private ClientNode _bNode;

        private ServerNode _resultNode;

        public override string Name { get { return "Types Match?"; } }

        public override BoxType Type { get { return BoxType.TypesEqual; } }

        public override List<Node> Nodes { get { return new List<Node> { _resultNode, _aNode, _bNode }; } }

        public TypesEqualBox()
        {
            _aNode = new ClientNode("A");
            _bNode = new ClientNode("B");

            _resultNode = new ServerNode("Result", this);
        }

        public override TypesEqualBox Clone()
        {
            return new TypesEqualBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            return new BoolValue(_aNode.InterpretTarget(context).GetType() == _bNode.InterpretTarget(context).GetType());
        }
    }

    public class SubstringBox : OperatorBox
    {
        private ClientNode _stringNode;

        private ClientNode _startNode;

        private ClientNode _lengthNode;

        private ServerNode _resultNode;

        public override string Name { get { return "Substring"; } }

        public override BoxType Type { get { return BoxType.Substring; } }

        public override List<Node> Nodes { get { return new List<Node> { _resultNode, _stringNode, _startNode, _lengthNode }; } }

        public SubstringBox()
        {
            _stringNode = new ClientNode("String");
            _startNode = new ClientNode("Start");
            _lengthNode = new ClientNode("Length");

            _resultNode = new ServerNode("Substring", this);
        }

        public override SubstringBox Clone()
        {
            return new SubstringBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            int start = ((IntegerValue)_startNode.InterpretTarget(context)).Value;
            int length = ((IntegerValue)_lengthNode.InterpretTarget(context)).Value;

            string str = ((StringValue)_stringNode.InterpretTarget(context)).StringRepresentation;

            return new StringValue(str.Substring(start, length));
        }
    }

    public abstract class BranchBox : Box
    {
        public override Color Color { get { return Color.Plum; } }
    }

    public class IfBox : BranchBox
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
            _node = new ClientNode("Event");
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

    public class SubroutineBox : EventBox, IValueSettable
    {
        private string _subName;

        public override string Name { get { return "Portal"; } }

        public override BoxType Type { get { return BoxType.Subroutine; } }

        public Value Value { get { return new StringValue(_subName); } }

        public SubroutineBox(string subName = "MyPortal")
        {
            _subName = subName;

            SetValue(new StringValue(subName));
        }

        public override Box Clone()
        {
            return new SubroutineBox(_subName);
        }

        public void SetValue(Value value)
        {
            _subName = ((StringValue)value).StringRepresentation;

            _node.Name = _subName;
        }

        public override Value Interpret(Value context, ServerNode? _)
        {
            return _node.InterpretTarget(context);
        }

        public override void Serialise(StringWriter writer)
        {
            base.Serialise(writer);

            writer.WriteLine(_subName);
        }

        public override void Deserialise(StringReader reader)
        {
            SetValue(new StringValue(reader.ReadLine()!));
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

        public override Value Interpret(Value context, ServerNode _)
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

    public class WhileBox : BranchBox
    {
        private ServerNode _resultNode;

        private ClientNode _conditionNode;

        private ClientNode _processNode;

        public override string Name { get { return "While"; } }

        public override BoxType Type { get { return BoxType.While; } }

        public override List<Node> Nodes { get { return new List<Node> { _resultNode, _conditionNode, _processNode }; } }

        public WhileBox()
        {
            _resultNode = new ServerNode("Last Value", this);

            _conditionNode = new ClientNode("Condition");

            _processNode = new ClientNode("Body");
        }

        public override WhileBox Clone()
        {
            return new WhileBox();
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            Value result = new NullValue();

            while (((BoolValue)_conditionNode.InterpretTarget(context)).Value)
            {
                result = _processNode.InterpretTarget(context);
            }

            return result;
        }
    }

    public class InvokeBox : PatchBox, IValueSettable
    {
        private string _subName;

        private ServerNode _resultNode;

        public override string Name { get { return "Send to Portal"; } }

        public override BoxType Type { get { return BoxType.Invoke; } }

        public override List<Node> Nodes { get { return new List<Node> { _resultNode }; } }

        public Value Value { get { return new StringValue(_subName); } }

        public InvokeBox(string subName = "MyPortal")
        {
            _subName = subName;

            _resultNode = new ServerNode(_subName, this);

            SetValue(new StringValue(subName));
        }

        public override InvokeBox Clone()
        {
            return new InvokeBox(_subName);
        }

        public override Value Interpret(Value context, ServerNode node)
        {
            foreach (Box box in Runner.Instance.Script.Boxes)
            {
                if (box is SubroutineBox subBox && subBox.Value.StringRepresentation == Value.StringRepresentation)
                {
                    return subBox.Interpret(context, null);
                }
            }

            throw new Exception($"cannot find portal '{_subName}'");
        }

        public void SetValue(Value value)
        {
            _subName = ((StringValue)value).StringRepresentation;

            _resultNode.Name = _subName;
        }

        public override void Serialise(StringWriter writer)
        {
            base.Serialise(writer);

            writer.WriteLine(_subName);
        }

        public override void Deserialise(StringReader reader)
        {
            SetValue(new StringValue(reader.ReadLine()!));
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