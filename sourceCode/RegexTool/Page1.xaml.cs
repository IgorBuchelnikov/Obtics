using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Obtics.Values;
using Obtics;
using System.Collections.ObjectModel;
using OE = Obtics.Collections.ObservableEnumerable;

/*
        In the XAML we refer to the Value property of IValueProvider objects by explicit property reference. This means that WPF doesn't
        have to search for public (which they are not) implementations of the Value property on the general object.
    
        WPF can now resolve the property at all and can do it more efficiently.
    
        In one case (MarkedString) the value provider is 'capped' with a concrete and public interface implementation. (using the Concrete extension method from BindingHelper)
        This allows for more convenient but less efficient property reference by name alone.
  
        Note that all lambda Expressions in this code behind refer to public members only! Most likely this program will run with
        limited Reflection permissions. Limited in such a way that only public members can be reflected so that only LambdaExpressions
        refering to public members can be compiled.
 */


namespace RegexTool
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();

            var ts = new TestString(this);
            ts.String.Value = "";
            _TestStrings.Add(ts);
        }

        public KeyValuePair<Regex, Exception> CreateRegex(string pattern, RegexOptions options)
        {
            try
            {
                return new KeyValuePair<Regex, Exception>(new Regex(pattern, options), null);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<Regex, Exception>(new Regex(@"[^\w\W]"), ex);
            }
        }

        /// <summary>
        /// <see cref="Obtics.Values.ExpressionObserver"/>
        /// </summary>
        public IValueProvider<bool> EcmaExcludedOptionsEnabled
        {
            get
            {                
                return
                    ExpressionObserver.Execute(
                        () =>
                            !IsChecked("ECMAScriptCheckBox").Value
                    );
            }
        }

        
        public IValueProvider<bool> IsChecked(string name)
        { return _IsCheckedF(this, name); }

        static Func<Page1, string, IValueProvider<bool>> _IsCheckedF =
            ExpressionObserver.Compile(
                (Page1 t, string name) =>
                    WpfHelper.WhenInitialized(t).Value.FindName<CheckBox>(name).IsChecked.GetValueOrDefault()
            );        
                

        public IValueProvider<bool> EcmaOptionEnabled
        {
            get
            {
                return
                    ExpressionObserver.Execute(
                        () =>
                            !(
                                IsChecked("ExplicitCaptureCheckBox").Value
                                || IsChecked("SinglelineCheckBox").Value
                                || IsChecked("IgnorePatternWhitespaceCheckBox").Value
                                || IsChecked("RightToLeftCheckBox").Value
                                || IsChecked("CultureInvariantCheckBox").Value
                            )
                    );
            }
        }


        public IValueProvider<KeyValuePair<Regex, Exception>> CurrentRegex
        {
            get
            {
                return ExpressionObserver.Execute(
                    () =>
                        CreateRegex(WpfHelper.WhenInitialized(this).Value.FindName<TextBox>("PatternTextBox").Text,
                            (IsChecked("IgnoreCaseCheckBox").Value ? RegexOptions.IgnoreCase : RegexOptions.None)
                            | (IsChecked("MultilineCheckBox").Value ? RegexOptions.Multiline : RegexOptions.None)
                            | (IsChecked("ECMAScriptCheckBox").Value ? RegexOptions.ECMAScript : RegexOptions.None)
                            | (IsChecked("CultureInvariantCheckBox").Value ? RegexOptions.CultureInvariant : RegexOptions.None)
                            | (IsChecked("ExplicitCaptureCheckBox").Value ? RegexOptions.ExplicitCapture : RegexOptions.None)
                            | (IsChecked("IgnorePatternWhitespaceCheckBox").Value ? RegexOptions.IgnorePatternWhitespace : RegexOptions.None)
                            | (IsChecked("RightToLeftCheckBox").Value ? RegexOptions.RightToLeft : RegexOptions.None)
                            | (IsChecked("SinglelineCheckBox").Value ? RegexOptions.Singleline : RegexOptions.None)
                        )
                );
            }
        }

        public class TestString
        {
            readonly IValueProvider<string> _String = ValueProvider.Dynamic("");

            public IValueProvider<string> String { get { return _String; } }

            readonly Page1 _Owner;

            public IValueProvider<Match> Match
            { get { return _Owner.CurrentRegex.Select(String, (kvp, s) => kvp.Key.Match(s)); } }

            static IEnumerable<Match> IterateMatches(Match match)
            {
                while (match.Success)
                {
                    yield return match;
                    match = match.NextMatch();
                }
            }

            public IEnumerable<Match> Matches
            { get { return Match.Select(mtch => IterateMatches(mtch)).Cascade(); } }

            static TextBlock GetMarkedString(string text, IEnumerable<Match> matches)
            {
                var matchEnum = matches.OrderBy(m => m.Index).GetEnumerator();
                var currentMatch = matchEnum.MoveNext() ? matchEnum.Current : null;
                bool currentBold = currentMatch != null && currentMatch.Index == 0 && currentMatch.Length > 0;
                var stringBuilder = new StringBuilder();
                var textBlock = new TextBlock();
                var inlines = textBlock.Inlines;

                for (int i = 0, end = text.Length; i < end; ++i)
                {
                    while (currentMatch != null && currentMatch.Index + currentMatch.Length <= i)
                        currentMatch = matchEnum.MoveNext() ? matchEnum.Current : null;

                    bool bold = currentMatch != null && currentMatch.Index <= i && currentMatch.Index + currentMatch.Length > i;

                    if (currentBold != bold)
                    {
                        AddRun(currentBold, stringBuilder, inlines);
                        stringBuilder.Length = 0;
                        currentBold = bold;
                    }

                    stringBuilder.Append(text[i]);
                }

                if (stringBuilder.Length != 0)
                    AddRun(currentBold, stringBuilder, inlines);

                return textBlock;
            }

            private static void AddRun(bool currentBold, StringBuilder stringBuilder, InlineCollection inlines)
            {
                var run = new Run(stringBuilder.ToString());

                if (currentBold)
                {
                    var b = new Bold(run);
                    b.Background = Brushes.Yellow;
                    inlines.Add(b);
                }
                else
                    inlines.Add(run);
            }

            public IValueProvider<TextBlock> MarkedString
            {
                get
                {
                    //Here Concrete is used to return a concrete interface implementation
                    return String.Select(Match, (s, m) => GetMarkedString(s, IterateMatches(m))).Async().Concrete();
                }
            }

            public TestString(Page1 owner)
            { _Owner = owner; }
        }

        readonly ObservableCollection<TestString> _TestStrings = new ObservableCollection<TestString>();

        public ObservableCollection<TestString> TestStrings
        { get { return _TestStrings; } }

        public IEnumerable<string> TotalUniqueMatches
        {
            get
            {
                return
                    OE.OrderBy(
                        ExpressionObserver.Execute(
                            () =>
                                TestStrings
                                    .SelectMany(ts => ts.Matches)
                                    .Select(grp => grp.Value)
                                    .Distinct()
                        )
                        .Cascade(),
                        str => str
                    );
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var testString = button.Tag as TestString;

                if (testString != null)
                    TestStrings.Remove(testString);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TestStrings.Add(new TestString(this));
        }        

    }
}
