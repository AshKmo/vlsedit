namespace VLSEdit
{
    public abstract class Value
    {
        public abstract string StringRepresentation { get; }

        public abstract bool IsEqualTo(Value x);

        public virtual int Length { get { return 0; } }

        public static Value FromString(Type type, string value)
        {
            switch (type)
            {
                case Type t when t == typeof(StringValue):
                    return new StringValue(value);
                case Type t when t == typeof(IntegerValue):
                    return IntegerValue.SubFromString(value);
                case Type t when t == typeof(DoubleValue):
                    return DoubleValue.SubFromString(value);
                case Type t when t == typeof(BoolValue):
                    return BoolValue.SubFromString(value);
            }

            return new NullValue();
        }
    }

    public class NullValue : Value
    {
        public override string StringRepresentation { get { return "Null"; } }

        public NullValue()
        {
        }

        public override bool IsEqualTo(Value x)
        {
            return x is NullValue;
        }
    }

    public abstract class NumberValue : Value
    {
        abstract public NumberValue Add(NumberValue x);

        abstract public NumberValue Multiply(NumberValue x);

        abstract public NumberValue Divide(NumberValue x);

        abstract public NumberValue Negative();

        abstract public bool LessThan(NumberValue x);

        public static NumberValue FromNumber(int x)
        {
            return new IntegerValue(x);
        }

        public static NumberValue FromNumber(double x)
        {
            return new DoubleValue(x);
        }
    }

    public class IntegerValue : NumberValue
    {
        private int _value;

        public int Value { get { return _value; } }

        public override string StringRepresentation { get { return Value.ToString(); } }

        public IntegerValue(int value)
        {
            _value = value;
        }

        public override NumberValue Add(NumberValue x)
        {
            switch (x)
            {
                case IntegerValue xInt:
                    return new IntegerValue(xInt.Value + _value);

                case DoubleValue:
                    return x.Add(this);
            }

            throw new Exception("number subclass not recognised");
        }

        public override NumberValue Multiply(NumberValue x)
        {
            switch (x)
            {
                case IntegerValue xInt:
                    return new IntegerValue(xInt.Value * _value);

                case DoubleValue:
                    return x.Multiply(this);
            }

            throw new Exception("number subclass not recognised");
        }

        public override NumberValue Divide(NumberValue x)
        {
            switch (x)
            {
                case IntegerValue xInt:
                    if (_value % xInt.Value == 0 && xInt.Value != 0)
                    {
                        return new IntegerValue(xInt.Value / _value);
                    }
                    else
                    {
                        return new DoubleValue(_value).Divide(x);
                    }

                case DoubleValue:
                    return new DoubleValue(_value).Divide(x);
            }

            throw new Exception("number subclass not recognised");
        }

        public override NumberValue Negative()
        {
            return new IntegerValue(-_value);
        }

        public override bool IsEqualTo(Value x)
        {
            return x is IntegerValue value && _value == value.Value || x is DoubleValue doubleValue && _value == doubleValue.Value;
        }

        public static IntegerValue SubFromString(string value)
        {
            return new IntegerValue(Int32.Parse(value));
        }

        public override bool LessThan(NumberValue x)
        {
            switch (x)
            {
                case IntegerValue xInt:
                    return _value < xInt.Value;
                case DoubleValue xDbl:
                    return _value < xDbl.Value;
            }

            return false;
        }

        public IntegerValue Remainder(IntegerValue x)
        {
            return new IntegerValue(_value % x.Value);
        }
    }

    public class DoubleValue : NumberValue
    {
        private double _value;

        public double Value { get { return _value; } }

        public override string StringRepresentation { get { return Value.ToString(); } }

        public DoubleValue(double value)
        {
            _value = value;
        }

        public override NumberValue Add(NumberValue x)
        {
            switch (x)
            {
                case IntegerValue xInt:
                    return Add(new DoubleValue(xInt.Value));

                case DoubleValue xDbl:
                    return new DoubleValue(xDbl.Value + _value);
            }

            throw new Exception("number subclass not recognised");
        }

        public override NumberValue Multiply(NumberValue x)
        {
            switch (x)
            {
                case IntegerValue xInt:
                    return Multiply(new DoubleValue(xInt.Value));

                case DoubleValue xDbl:
                    return new DoubleValue(xDbl.Value * _value);
            }

            throw new Exception("number subclass not recognised");
        }

        public override NumberValue Divide(NumberValue x)
        {
            switch (x)
            {
                case IntegerValue xInt:
                    return Divide(new DoubleValue(xInt.Value));

                case DoubleValue xDbl:
                    return new DoubleValue(_value / xDbl.Value);
            }

            throw new Exception("number subclass not recognised");
        }

        public override NumberValue Negative()
        {
            return new DoubleValue(-_value);
        }

        public override bool IsEqualTo(Value x)
        {
            return x is IntegerValue value && _value == value.Value || x is DoubleValue doubleValue && _value == doubleValue.Value;
        }

        public static DoubleValue SubFromString(string value)
        {
            return new DoubleValue(Double.Parse(value));
        }

        public override bool LessThan(NumberValue x)
        {
            switch (x)
            {
                case IntegerValue xInt:
                    return _value < xInt.Value;
                case DoubleValue xDbl:
                    return _value < xDbl.Value;
            }

            return false;
        }
    }

    public class BoolValue : Value
    {
        private bool _value;

        public bool Value { get { return _value; } }

        public override string StringRepresentation { get { return Value.ToString(); } }

        public BoolValue(bool value)
        {
            _value = value;
        }

        public BoolValue And(BoolValue x)
        {
            return new BoolValue(_value && x.Value);
        }

        public BoolValue Or(BoolValue x)
        {
            return new BoolValue(_value || x.Value);
        }

        public BoolValue Not()
        {
            return new BoolValue(!_value);
        }

        public override bool IsEqualTo(Value x)
        {
            return x is BoolValue value && _value == value.Value;
        }

        public static BoolValue SubFromString(string value)
        {
            return new BoolValue(bool.Parse(value));
        }
    }

    public class StringValue : Value
    {
        private string _value;

        public override string StringRepresentation { get { return _value; } }

        public override int Length { get { return _value.Length; } }

        public StringValue(string value)
        {
            _value = value;
        }

        public override bool IsEqualTo(Value x)
        {
            return x is StringValue && x.StringRepresentation == StringRepresentation;
        }

        public static StringValue SubFromString(string value)
        {
            return new StringValue(value);
        }

        public StringValue Concat(StringValue x)
        {
            return new StringValue(_value + x.StringRepresentation);
        }
    }

    public class ListValue : Value
    {
        private List<Value> _value;

        public override int Length { get { return _value.Count; } }

        public List<Value> Value { get { return _value; } }

        public override string StringRepresentation { get { return "[" + String.Join(", ", _value.Select(v => v.StringRepresentation)) + "]"; } }

        public ListValue(List<Value> value)
        {
            _value = value;
        }

        public override bool IsEqualTo(Value x)
        {
            if (x is not ListValue listValue) return false;

            if (listValue.Length != Length) return false;

            for (int i = 0; i < Length; i++)
            {
                if (!_value[i].IsEqualTo(listValue.Value[i])) return false;
            }

            return true;
        }

        public ListValue Add(Value x)
        {
            return new ListValue([.. _value, x]);
        }

        public Value Index(IntegerValue x)
        {
            if (x.Value < 0 || x.Value >= _value.Count)
            {
                return new NullValue();
            }

            return _value[x.Value];
        }

        public ListValue Cdr()
        {
            return new ListValue(_value.Skip(1).ToList());
        }

        public ListValue Concat(ListValue x)
        {
            return new ListValue(_value.Concat(x.Value).ToList());
        }
    }
}