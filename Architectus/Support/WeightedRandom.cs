namespace Architectus;

public class WeightedRandom<T>
{
    private struct Item
    {
        public T Value;

        public float Probability;

        public Item(T value, float probability)
        {
            this.Value = value;
            this.Probability = probability;
        }
    }

    private readonly Random _random;

    private readonly List<Item> _weightedValues;

    public WeightedRandom(int seed) : this(new System.Random(seed)) { }

    public WeightedRandom() : this(new System.Random()) { }

    public WeightedRandom(System.Random random)
    {
        this._random = random;
        this._weightedValues = new List<Item>();
    }

    public WeightedRandom<T> Clear()
    {
        this._weightedValues.Clear();
        this.SumOfProbabilities = 0f;

        return this;
    }

    public WeightedRandom<T> Add(T value, float probability)
    {
        if (probability <= 0f) return this;

        this._weightedValues.Add(new Item(value, probability));
        this.SumOfProbabilities += probability;

        return this;
    }

    public bool Remove(T value)
    {
        var item = this._weightedValues.FirstOrDefault(x => x.Value!.Equals(value));
        if (item.Value == null) return false;

        this.SumOfProbabilities -= item.Probability;
        this._weightedValues.Remove(item);

        return true;
    }

    public float SumOfProbabilities { get; private set; } = 0f;

    public int Count => this._weightedValues.Count;

    public T Next()
    {
        if (this.SumOfProbabilities <= 0f) return default!;

        double p = this._random.NextDouble() * this.SumOfProbabilities;

        foreach (var v in this._weightedValues)
        {
            p -= v.Probability;
            if (p <= 0) return v.Value;
        }

        return this._weightedValues[^1].Value;
    }
}