using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NavigationSample.ViewModels;

namespace NavigationSample.DataModel
{
    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        /// <summary>
        /// All the available groups
        /// </summary>
        public IList<SampleDataGroupViewModel> Groups { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SampleDataSource()
        {
            Groups = new ObservableCollection<SampleDataGroupViewModel>();
            PopulateData();
        }

        private static readonly Random RNG = new Random();
        private string GenerateString(int size)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < size; i++)
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * RNG.NextDouble() + 65))));

            return builder.ToString();
        }

        private string GenerateParagraph(int wordCount)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < wordCount; i++)
            {
                builder.Append(GenerateString(RNG.Next(10)));
                builder.Append(" ");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Method to populate the data
        /// </summary>
        private void PopulateData()
        {
            string itemContent = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}", GenerateParagraph(50));

            for (int i = 0; i < 20; i++)
            {
                var group = new SampleDataGroupViewModel("Group" + i,
                                                         "Group # " + (i + 1),
                                                         "Group Subtitle: " + (i + 1),
                                                         "/Assets/SampleImages/Box" + (1+RNG.Next(10)) + ".png",
                                                         "Description: " + GenerateParagraph(10));
                for (int x = 0; x < (RNG.Next(50)+1); x++)
                {
                    group.Items.Add(new SampleDataItemViewModel(string.Format("Group-{0}-Item-{1}", i, x),
                            "Item # " + (x+1),
                            "Item Subtitle: " + (x+1),
                            "/Assets/SampleImages/Box" + (1 + RNG.Next(10)) + ".png",
                            "Description: " + GenerateParagraph(8),
                            itemContent,
                            group));
                }
                
                Groups.Add(group);
            }
        }
    }
}
