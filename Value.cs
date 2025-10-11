namespace VLSEdit
{
    public abstract class Value
    {
        public abstract bool IsEqualTo(Value x);
    }

    public class NullValue : Value
    {
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
            return x is IntegerValue && _value == ((IntegerValue)x).Value;
        }
    }

    public class DoubleValue : NumberValue
    {
        private double _value;

        public double Value { get { return _value; } }

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
            return x is DoubleValue && _value == ((DoubleValue)x).Value;
        }
    }

    class BoolValue : Value
    {
        private bool _value;

        public bool Value { get { return _value; } }

        public BoolValue(bool value)
        {
            _value = Value;
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
            return x is BoolValue && _value == ((BoolValue)x).Value;
        }
    }
}