namespace KellyBenchmark
{
    public readonly struct RotateInput
    {
        public int Count { get; }
        public int K { get; }

        public RotateInput(int count, int k)
        {
            Count = count;
            K = k;
        }

        public override string ToString() => $"[count={Count}, k={K}]";
    }
}