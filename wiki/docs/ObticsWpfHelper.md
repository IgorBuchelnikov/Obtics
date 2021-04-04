# ObticsWpfHelper

This is a small project dedicated to solutions for challenges that arrise when combining Obtics with WPF.

## Unitialized field problem

When child elements of a user control bind directly to a transformation property that itself is directly dependent on properties of other child elements, the transformation may not work. For example; consider the following XAML and Codebehind of a user control:
{{
        <!--XAML-->
        <Label Height="28" Margin="104,80,146,0" Name="label5" VerticalAlignment="Top" Content="{Binding TotalManipulations.Value}"></Label>
        <TextBox Height="23" Margin="104,46,146,0" Name="textBox1" Text="1000" VerticalAlignment="Top" />
        <TextBox Height="23" Margin="104,12,146,0" Name="textBox2" Text="500" VerticalAlignment="Top" />
}}
{{
        //Code behind

        static int? Parse(string text)
        {
            int res;
            var valid = int.TryParse(text, out res);
            return valid ? (int?)res : null;
        }

        public IValueProvider<int?> TotalManipulations
        {
            get
            {
                return ExpressionObserver.Execute(
                    () => Parse(textBox1.Text) + Parse(textBox2.Text)
                );
            }
        }
}}

The Label is bound to the TotalManipulations property. This property is directly dependent on the textBox1 and textBox2 **fields**. These fields are generated by the development environment and it does not generate change notification for these fields. On top of that, the XAML engine assigns fields and avaluates bindings in the order they occur. That means that the getter of TotalManipulations is called before the textBox1 and textBox2 have a value. The Value property of TotalManipulations **will always be null**.

Note that if the label element has appeared **after** the TextBox elements, the fields would have been assigned before the binding was evaluated and everything would have worked.

ObticsWpfHelper provides a solution that is not as pretty as the original code but workable. It does this via the Obtics.WpfHelper.WhenInitialized method. This method takes a FrameworkElement as argument and return an [IValueProvider](IValueProvider)(IValueProvider) as result. This Value property of this [IValueProvider](IValueProvider)(IValueProvider) will have the given FrameworkElement as value but **only if its IsInitialized property is true**. It listens to the Initialized event of the FrameworkElement and changes the Value poperty accordingly.

{{
        //Code behind
        //using Obtics;

        static int? Parse(string text)
        {
            int res;
            var valid = int.TryParse(text, out res);
            return valid ? (int?)res : null;
        }

        public IValueProvider<int?> TotalManipulations
        {
            get
            {
                return ExpressionObserver.Execute(
                    () => Parse(this.WhenInitialized().Value.textBox1.Text) + Parse(this.WhenInitialized().Value.textBox2.Text)
                );
            }
        }
}}

So; when the user control has been initialized the transformation will be notified and the Value of TotalManipulations will be updated. The Label will display the proper value.