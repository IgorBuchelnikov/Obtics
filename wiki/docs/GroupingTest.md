# GroupingTest

This is a small sample/test application that demonstrates some obtics capabilities.

It fills an ObservableCollection<int> with a given number of random integers and then does a given number of random manipulations on that collection. It filters in only those numbers where n%3 == 1 and then groups them by number of bits set. It creates an overview of those groeps (32 in total) and displays a score of the count in each group. All work is done on a background thread.

Interesting features demonstrated are Grouping, performance gain by sorting, Synchronization by using Async() and ObticsWPFHelper.WhenInitialized method.