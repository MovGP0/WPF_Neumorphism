using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class DataGridsPage : UserControl
{
    public DataGridsPage()
    {
        InitializeComponent();
        Queue.ItemsSource = new QueueEntry[]
        {
            new("Review layouts", "Mina", "Active", 3),
            new("Check keyboard order", "Jon", "Ready", 2),
            new("Prepare release notes", "Tess", "Draft", 4),
            new("Verify contrast", "Mina", "Done", 1),
            new("Review examples", "Jon", "Active", 5),
            new("Update component tags", "Tess", "Done", 2)
        };
    }

    public sealed class QueueEntry(string task, string owner, string state, int hours)
    {
        public string Task { get; set; } = task;
        public string Owner { get; set; } = owner;
        public string State { get; set; } = state;
        public int Hours { get; set; } = hours;
    }
}
