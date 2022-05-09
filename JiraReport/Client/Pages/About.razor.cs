using JiraReport.Shared;

namespace JiraReport.Client.Pages
{
    public partial class About
    {
        private IEnumerable<JiraIssue> Issues = new List<JiraIssue>();
        private HashSet<JiraIssue> SelectedIssues = new();
        private HashSet<Element> selectedItems1 = new HashSet<Element>();
        private IEnumerable<Element> Elements = new List<Element>();

        protected override async Task OnInitializedAsync()
        {
            Issues = new List<JiraIssue>()
            {
                new()
                {
                    Project = "A"
                },
                new()
                {
                    Project = "B"
                },
                new()
                {
                    Project = "C"
                }
            };
            SelectedIssues = Issues.ToHashSet();
            Elements = new List<Element>(){
            new() {Number= 1},
            new() {Number= 2},
            new() {Number= 3}
        };
            selectedItems1 = Elements.ToHashSet();
        }
    }
    public record Element
    {
        public string Group { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public bool Checked { get; set; }

        public string Sign { get; set; }
        public double Molar { get; set; }
        public IList<int> Electrons { get; set; }

        public override string ToString()
        {
            return $"{Sign} - {Name}";
        }
    }
}
