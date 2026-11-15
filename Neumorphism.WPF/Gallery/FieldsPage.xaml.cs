using System.Globalization;
using System.Net.Mail;
using System.Windows.Controls;

namespace Neumorphism.WPF.Gallery;

public partial class FieldsPage : UserControl
{
    public FieldsPage()
    {
        InitializeComponent();
    }

    public string Address { get; set; } = "design@example.com";
}


public sealed class EmailAddressRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo) =>
        value is string text && MailAddress.TryCreate(text, out MailAddress? address) && address.Address == text
            ? ValidationResult.ValidResult
            : new ValidationResult(false, "Enter a valid email address.");
}
