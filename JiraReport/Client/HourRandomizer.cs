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

			for (var i = 0; i < weights.Count(); ++i)
			{
				var v = count * (weights.ElementAt(i).hours) / sum;

				result.Add((weights.ElementAt(i).key, v));
			}

			return result;
		}
	}
}
