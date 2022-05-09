namespace JiraReport.Client
{
	public static class HourRandomizer
	{
		public static List<(string key, double hours)> SplitIntoBuckets(double count, IEnumerable<(string key, double hours)> weights)
		{
			if (null == weights)
				throw new ArgumentNullException(nameof(weights));
			else if (!weights.Any())
				throw new ArgumentOutOfRangeException(nameof(weights), "Empty weights");

			var result = new List<(string key, double hours)>();

			var sum = weights.Sum(x => x.hours);

			if (sum == 0)
				throw new ArgumentException("Weights must not sum to 0", nameof(weights));

			static double round(double x) => x >= 0 ? x + 0.5 : x - 0.5;

			double diff = 0;

			for (var i = 0; i < weights.Count(); ++i)
			{
				double v = count * (weights.ElementAt(i).hours) / sum;
				double value = round(v);
				diff += v - value;

				if (diff >= 0.5)
				{
					value += 1;
					diff -= 1;
				}
				else if (diff <= -0.5)
				{
					value -= 1;
					diff += 1;
				}

				result.Add((weights.ElementAt(i).key, value));
			}

			return result;
		}
	}
}
