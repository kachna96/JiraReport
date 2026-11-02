namespace JiraReport.Client
{
	public static class HourRandomizer
	{
		public static List<(string key, double hours)> SplitIntoBuckets(double count, IEnumerable<(string key, double hours)> weights)
		{
			ArgumentNullException.ThrowIfNull(weights);

			var list = weights as IReadOnlyList<(string key, double hours)> ?? weights.ToList();
			if (list.Count == 0)
			{
				return [];
			}

			var sum = list.Sum(x => x.hours);
			if (sum <= 0)
			{
				return list.Select(x => (x.key, 0.0)).ToList();
			}

			var result = new List<(string key, double hours)>(list.Count);
			foreach (var item in list)
			{
				var v = count * item.hours / sum;
				result.Add((item.key, v));
			}

			return result;
		}
	}
}
