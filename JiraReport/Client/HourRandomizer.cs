namespace JiraReport.Client
{
    public static class HourRandomizer
    {
        /// <summary>
        /// Proportionally distributes a target hour count across weighted buckets,
        /// rounded to the specified decimal places (default 2) using the Largest Remainder
        /// (Hamilton-Hare) apportionment method.
        /// 
        /// Guarantees that:
        /// 1. Each bucket's allocated hours is rounded to <paramref name="decimalPlaces"/>.
        /// 2. The sum of all bucket hours strictly equals <paramref name="count"/> (rounded to <paramref name="decimalPlaces"/>).
        /// 3. If all weights are 0, hours are distributed equally among all buckets.
        /// 4. No negative hours are allocated.
        /// </summary>
        public static List<(string key, double hours)> SplitIntoBuckets(
            double count,
            IEnumerable<(string key, double hours)> weights,
            int decimalPlaces = 2)
        {
            ArgumentNullException.ThrowIfNull(weights);

            var decimalBuckets = SplitIntoBuckets(
                (decimal)count,
                weights.Select(w => (w.key, (decimal)w.hours)),
                decimalPlaces);

            var result = new List<(string key, double hours)>(decimalBuckets.Count);
            for (int i = 0; i < decimalBuckets.Count; i++)
            {
                var (key, hours) = decimalBuckets[i];
                result.Add((key, (double)hours));
            }

            return result;
        }

        /// <summary>
        /// Proportionally distributes a target decimal hour count across weighted buckets
        /// using exact decimal arithmetic to avoid binary floating-point rounding inaccuracies.
        /// </summary>
        public static List<(string key, decimal hours)> SplitIntoBuckets(
            decimal count,
            IEnumerable<(string key, decimal hours)> weights,
            int decimalPlaces = 2)
        {
            ArgumentNullException.ThrowIfNull(weights);
            ArgumentOutOfRangeException.ThrowIfNegative(decimalPlaces);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(decimalPlaces, 15);

            var list = weights as IReadOnlyList<(string key, decimal hours)> ?? weights.ToList();
            if (list.Count == 0)
            {
                return [];
            }

            decimal roundedTarget = Math.Round(count, decimalPlaces, MidpointRounding.AwayFromZero);
            if (roundedTarget <= 0m)
            {
                return list.Select(x => (x.key, 0m)).ToList();
            }

            if (list.Count == 1)
            {
                return [(list[0].key, roundedTarget)];
            }

            decimal sumWeights = 0m;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].hours > 0m)
                {
                    sumWeights += list[i].hours;
                }
            }

            bool allZero = sumWeights <= 0m;
            decimal effectiveSum = allZero ? list.Count : sumWeights;

            decimal factor = 1m;
            for (int i = 0; i < decimalPlaces; i++)
            {
                factor *= 10m;
            }

            long totalUnits = (long)Math.Round(roundedTarget * factor, MidpointRounding.AwayFromZero);

            var baseUnits = new long[list.Count];
            var remainders = new (int index, decimal remainder)[list.Count];
            long allocatedUnits = 0;

            for (int i = 0; i < list.Count; i++)
            {
                decimal w = allZero ? 1m : Math.Max(0m, list[i].hours);

                decimal exactUnits = (totalUnits * w) / effectiveSum;
                long baseVal = (long)Math.Floor(exactUnits);

                baseUnits[i] = baseVal;
                allocatedUnits += baseVal;
                remainders[i] = (i, exactUnits - baseVal);
            }

            long deficit = totalUnits - allocatedUnits;

            if (deficit > 0)
            {
                Array.Sort(remainders, static (a, b) =>
                {
                    int cmp = b.remainder.CompareTo(a.remainder);
                    return cmp != 0 ? cmp : a.index.CompareTo(b.index);
                });

                int countToIncrement = (int)Math.Min(deficit, list.Count);
                for (int i = 0; i < countToIncrement; i++)
                {
                    baseUnits[remainders[i].index]++;
                }
            }

            var result = new List<(string key, decimal hours)>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                result.Add((list[i].key, Math.Round(baseUnits[i] / factor, decimalPlaces, MidpointRounding.AwayFromZero)));
            }

            return result;
        }
    }
}
