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

            var list = weights as IReadOnlyList<(string key, double hours)> ?? weights.ToList();
            if (list.Count == 0)
            {
                return [];
            }

            var roundedTarget = Math.Round(count, decimalPlaces);
            if (roundedTarget <= 0)
            {
                return list.Select(x => (x.key, 0.0)).ToList();
            }

            if (list.Count == 1)
            {
                return [(list[0].key, roundedTarget)];
            }

            var sumWeights = list.Sum(x => Math.Max(0.0, x.hours));
            bool allZero = sumWeights <= 0;

            var factor = Math.Pow(10, decimalPlaces);
            long totalUnits = (long)Math.Round(roundedTarget * factor);

            var baseUnits = new long[list.Count];
            var remainders = new (int index, double remainder)[list.Count];
            long allocatedUnits = 0;

            for (int i = 0; i < list.Count; i++)
            {
                double w = allZero ? 1.0 : Math.Max(0.0, list[i].hours);
                double effectiveSum = allZero ? list.Count : sumWeights;

                double exactUnits = (totalUnits * w) / effectiveSum;
                long baseVal = (long)Math.Floor(exactUnits);

                baseUnits[i] = baseVal;
                allocatedUnits += baseVal;
                remainders[i] = (i, exactUnits - baseVal);
            }

            long deficit = totalUnits - allocatedUnits;

            if (deficit > 0)
            {
                var sortedRemainders = remainders
                    .OrderByDescending(r => r.remainder)
                    .ThenBy(r => r.index)
                    .Take((int)Math.Min(deficit, list.Count));

                foreach (var (index, _) in sortedRemainders)
                {
                    baseUnits[index]++;
                }
            }

            var result = new List<(string key, double hours)>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                result.Add((list[i].key, Math.Round(baseUnits[i] / factor, decimalPlaces)));
            }

            return result;
        }
    }
}
