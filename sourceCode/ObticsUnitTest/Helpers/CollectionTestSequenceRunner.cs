using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obtics.Collections.Transformations;

namespace ObticsUnitTest.Helpers
{
    internal abstract class CollectionTestSequenceRunner<TSource>
    {
        public abstract List<TSource> SourceSequence
        { get; }

        public abstract List<TSource> FillerItems
        { get; }

        public delegate void Action(IFrameIEnumerable<TSource> f);

        public delegate Action PrepAction(List<TSource> target, List<TSource> fillerItems, out string description);

        #region Add actions
        public static Action PrepAddSingleAtBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "add single item at begin";

            if (target.Count < 1)
                return null;

            IList<TSource> range = target.GetRange(0, 1);
            target.RemoveAt(0);

            return delegate(IFrameIEnumerable<TSource> f) { f.AddItems(range, 0); };
        }

        public static Action PrepAddMultipleAtBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "add multiple items at begin";

            if (target.Count < 3)
                return null;

            IList<TSource> range = target.GetRange(0, 3);
            target.RemoveRange(0, 3);

            return delegate(IFrameIEnumerable<TSource> f) { f.AddItems(range, 0); };
        }

        public static Action PrepAddSingleAtMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "add single item in middle";

            if (target.Count < 3)
                return null;

            int index = target.Count / 2 ;
            IList<TSource> range = target.GetRange(index, 1);
            target.RemoveAt(index);

            return delegate(IFrameIEnumerable<TSource> f) { f.AddItems(range, index); };
        }

        public static Action PrepAddMultipleAtMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "add multiple items in middle";

            if (target.Count < 5)
                return null;

            int index = target.Count / 2 - 1;
            IList<TSource> range = target.GetRange(index, 3);
            target.RemoveRange(index, 3);

            return delegate(IFrameIEnumerable<TSource> f) { f.AddItems(range, index); };
        }

        public static Action PrepAddSingleAtEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "add single item at end";

            if (target.Count < 2)
                return null;

            int index = target.Count - 1;
            IList<TSource> range = target.GetRange(index, 1);
            target.RemoveAt(index);

            return delegate(IFrameIEnumerable<TSource> f) { f.AddItems(range, index); };
        }

        public static Action PrepAddMultipleAtEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "add multiple items at end";

            if (target.Count < 4)
                return null;

            int index = target.Count - 3;
            IList<TSource> range = target.GetRange(index, 3);
            target.RemoveRange(index, 3);

            return delegate(IFrameIEnumerable<TSource> f) { f.AddItems(range, index); };
        }
        #endregion

        #region Move actions

        public static Action PrepMoveSingleFromBeginToMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move single item from begin to middle";

            if (target.Count < 3)
                return null;

            int index = target.Count / 2 - 1;
            IList<TSource> range = target.GetRange(index, 1);
            target.RemoveRange(index, 1);
            target.InsertRange(0, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(1, index, 0); };
        }

        public static Action PrepMoveMultipleFromBeginToMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move multiple items from begin to middle";

            if (target.Count < 5)
                return null;

            int index = target.Count / 2 - 1;
            IList<TSource> range = target.GetRange(index, 3);
            target.RemoveRange(index, 3);
            target.InsertRange(0, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(3, index, 0); };
        }

        public static Action PrepMoveSingleFromBeginToEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move single item from begin to end";

            if (target.Count < 2)
                return null;

            int index = target.Count - 1;
            IList<TSource> range = target.GetRange(index, 1);
            target.RemoveRange(index, 1);
            target.InsertRange(0, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(1, index, 0); };
        }

        public static Action PrepMoveMultipleFromBeginToEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move multiple items from begin to end";

            if (target.Count < 4)
                return null;

            int index = target.Count - 3;
            IList<TSource> range = target.GetRange(index, 3);
            target.RemoveRange(index, 3);
            target.InsertRange(0, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(3, index, 0); };
        }

        public static Action PrepMoveSingleFromMiddleToBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move single item from middle to begin";

            if (target.Count < 4)
                return null;

            IList<TSource> range = target.GetRange(0, 1);
            target.RemoveRange(0, 1);
            int index = target.Count / 2 - 1;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(1, 0, index); };
        }

        public static Action PrepMoveMultipleFromMiddleToBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move multiple items from middle to begin";

            if (target.Count < 6)
                return null;

            IList<TSource> range = target.GetRange(0, 3);
            target.RemoveRange(0, 3);
            int index = target.Count / 2 - 1;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(3, 0, index); };
        }

        public static Action PrepMoveSingleFromMiddleToEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move single item from middle to end";

            if (target.Count < 4)
                return null;

            IList<TSource> range = target.GetRange(target.Count - 1, 1);
            target.RemoveRange(target.Count - 1, 1);
            int index = target.Count / 2 - 1;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(1, target.Count - 1, index); };
        }

        public static Action PrepMoveMultipleFromMiddleToEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move multiple items from middle to end";

            if (target.Count < 6)
                return null;

            IList<TSource> range = target.GetRange(target.Count - 3, 3);
            target.RemoveRange(target.Count - 3, 3);
            int index = target.Count / 2 - 1;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(3, target.Count - 3, index); };
        }

        public static Action PrepMoveSingleFromEndToBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move single item from end to begin";

            if (target.Count < 2)
                return null;

            IList<TSource> range = target.GetRange(0, 1);
            target.RemoveRange(0, 1);
            int index = target.Count;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(1, 0, index); };
        }

        public static Action PrepMoveMultipleFromEndToBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move multiple items from end to begin";

            if (target.Count < 4)
                return null;

            IList<TSource> range = target.GetRange(0, 3);
            target.RemoveRange(0, 3);
            int index = target.Count;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(3, 0, index); };
        }

        public static Action PrepMoveSingleFromEndToMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move single item from end to middle";

            if (target.Count < 4)
                return null;

            int newIndex = target.Count / 2 - 1;
            IList<TSource> range = target.GetRange(newIndex, 1);
            target.RemoveRange(newIndex, 1);
            int index = target.Count;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(1, newIndex, index); };
        }

        public static Action PrepMoveMultipleFromEndToMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "move multiple items from end to middle";

            if (target.Count < 6)
                return null;

            int newIndex = target.Count / 2 - 1;
            IList<TSource> range = target.GetRange(newIndex, 3);
            target.RemoveRange(newIndex, 3);
            int index = target.Count;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.MoveItems(3, newIndex, index); };
        }

        #endregion

        #region Remove actions

        public static Action PrepRemoveSingleFromBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "remove single item from begin";

            var range = fillerItems.GetRange(0, 1);
            fillerItems.RemoveRange(0, 1);
            target.InsertRange(0, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.RemoveItems(1, 0); };
        }

        public static Action PrepRemoveMultipleFromBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "remove multiple items from begin";

            var range = fillerItems.GetRange(0, 3);
            fillerItems.RemoveRange(0, 3);
            target.InsertRange(0, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.RemoveItems(3, 0); };
        }

        public static Action PrepRemoveSingleFromMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "remove single item from middle";

            if (target.Count < 4)
                return null;

            var range = fillerItems.GetRange(0, 1);
            fillerItems.RemoveRange(0, 1);
            int index = target.Count / 2 - 1;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.RemoveItems(1, index); };
        }

        public static Action PrepRemoveMultipleFromMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "remove multiple items from middle";

            if (target.Count < 4)
                return null;

            var range = fillerItems.GetRange(0, 3);
            fillerItems.RemoveRange(0, 3);
            int index = target.Count / 2 - 1;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.RemoveItems(3, index); };
        }

        public static Action PrepRemoveSingleFromEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "remove single item from end";

            if (target.Count < 1)
                return null;

            var range = fillerItems.GetRange(0, 1);
            fillerItems.RemoveRange(0, 1);
            int index = target.Count;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.RemoveItems(1, index); };
        }

        public static Action PrepRemoveMultipleFromEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "remove multiple items from end";

            if (target.Count < 1)
                return null;

            var range = fillerItems.GetRange(0, 3);
            fillerItems.RemoveRange(0, 3);
            int index = target.Count;
            target.InsertRange(index, range);

            return delegate(IFrameIEnumerable<TSource> f) { f.RemoveItems(3, index); };
        }

        #endregion

        #region Replace actions

        public static Action PrepReplaceSingleBySingleAtBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace single item with single item at begin";

            if (target.Count < 1)
                return null;

            var newRange = target.GetRange(0, 1);
            target.RemoveRange(0, 1);
            var oldRange = fillerItems.GetRange(0, 1);
            fillerItems.RemoveRange(0, 1);
            target.InsertRange(0, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 1, 0); };
        }

        public static Action PrepReplaceSingleByMultipleAtBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace single item with multiple items at begin";

            if (target.Count < 3)
                return null;

            var newRange = target.GetRange(0, 3);
            target.RemoveRange(0, 3);
            var oldRange = fillerItems.GetRange(0, 1);
            fillerItems.RemoveRange(0, 1);
            target.InsertRange(0, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 1, 0); };
        }

        public static Action PrepReplaceMultipleBySingleAtBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace multiple items with single item at begin";

            if (target.Count < 1)
                return null;

            var newRange = target.GetRange(0, 1);
            target.RemoveRange(0, 1);
            var oldRange = fillerItems.GetRange(0, 3);
            fillerItems.RemoveRange(0, 3);
            target.InsertRange(0, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 3, 0); };
        }

        public static Action PrepReplaceMultipleByMultipleAtBegin(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace multiple items with multiple items at begin";

            if (target.Count < 3)
                return null;

            var newRange = target.GetRange(0, 3);
            target.RemoveRange(0, 3);
            var oldRange = fillerItems.GetRange(0, 3);
            fillerItems.RemoveRange(0, 3);
            target.InsertRange(0, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 3, 0); };
        }

        public static Action PrepReplaceSingleBySingleAtMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace single item with single item in middle";

            if (target.Count < 4)
                return null;

            var index = target.Count / 2 - 1;
            var newRange = target.GetRange(index, 1);
            target.RemoveRange(index, 1);
            var oldRange = fillerItems.GetRange(0, 1);
            fillerItems.RemoveRange(0, 1);
            target.InsertRange(index, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 1, index); };
        }

        public static Action PrepReplaceSingleByMultipleAtMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace single item with multiple items in middle";

            if (target.Count < 6)
                return null;

            var index = target.Count / 2 - 1;
            var newRange = target.GetRange(index, 3);
            target.RemoveRange(index, 3);
            var oldRange = fillerItems.GetRange(0, 1);
            fillerItems.RemoveRange(0, 1);
            target.InsertRange(index,oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 1, index); };
        }

        public static Action PrepReplaceMultipleBySingleAtMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace multiple items with single item in middle";

            if (target.Count < 4)
                return null;

            var index = target.Count / 2 - 1;
            var newRange = target.GetRange(index, 1);
            target.RemoveRange(index, 1);
            var oldRange = fillerItems.GetRange(0, 3);
            fillerItems.RemoveRange(0, 3);
            target.InsertRange(index, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 3, index); };
        }

        public static Action PrepReplaceMultipleByMultipleAtMiddle(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace multiple items with multiple items in middle";

            if (target.Count < 6)
                return null;

            var index = target.Count / 2 - 1;
            var newRange = target.GetRange(index, 3);
            target.RemoveRange(index, 3);
            var oldRange = fillerItems.GetRange(0, 3);
            fillerItems.RemoveRange(0, 3);
            target.InsertRange(index, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 3, index); };
        }


        public static Action PrepReplaceSingleBySingleAtEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace single item with single item at end";

            if (target.Count < 2)
                return null;

            var index = target.Count - 1;
            var newRange = target.GetRange(index, 1);
            target.RemoveRange(index, 1);
            var oldRange = fillerItems.GetRange(0, 1);
            fillerItems.RemoveRange(0, 1);
            target.InsertRange(index, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 1, index); };
        }

        public static Action PrepReplaceSingleByMultipleAtEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace singe item with multiple items at end";

            if (target.Count < 4)
                return null;

            var index = target.Count - 3;
            var newRange = target.GetRange(index, 3);
            target.RemoveRange(index, 3);
            var oldRange = fillerItems.GetRange(0, 1);
            fillerItems.RemoveRange(0, 1);
            target.InsertRange(index, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 1, index); };
        }

        public static Action PrepReplaceMultipleBySingleAtEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace multiple items with single item at end";

            if (target.Count < 2)
                return null;

            var index = target.Count - 1;
            var newRange = target.GetRange(index, 1);
            target.RemoveRange(index, 1);
            var oldRange = fillerItems.GetRange(0, 3);
            fillerItems.RemoveRange(0, 3);
            target.InsertRange(index, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 3, index); };
        }

        public static Action PrepReplaceMultipleByMultipleAtEnd(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "replace multiple items with multiple items at end";

            if (target.Count < 4)
                return null;

            var index = target.Count - 3;
            var newRange = target.GetRange(index, 3);
            target.RemoveRange(index, 3);
            var oldRange = fillerItems.GetRange(0, 3);
            fillerItems.RemoveRange(0, 3);
            target.InsertRange(index, oldRange);

            return delegate(IFrameIEnumerable<TSource> f) { f.ReplaceItems(newRange, 3, index); };
        }

        #endregion

        #region Reset actions

        public static Action PrepResetOnNotEmpty(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "reset a not emtpy list";

            var range = target.GetRange(0, target.Count);
            target.Clear();
            target.AddRange(fillerItems.GetRange(0, 3));
            fillerItems.RemoveRange(0, 3);

            return delegate(IFrameIEnumerable<TSource> f) { f.ResetItems(range); };
        }

        public static Action PrepResetOnEmpty(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "reset an empty list";

            var range = target.GetRange(0, target.Count);
            target.Clear();

            return delegate(IFrameIEnumerable<TSource> f) { f.ResetItems(range); };
        }

        #endregion

        public static Action PrepNothing(List<TSource> target, List<TSource> fillerItems, out string description)
        {
            description = "do nothing";
            return delegate(IFrameIEnumerable<TSource> f) { };
        }



        public static PrepAction[] Preps = new PrepAction[] { 
            PrepNothing,
            PrepAddMultipleAtBegin,
            PrepAddMultipleAtEnd,
            PrepAddMultipleAtMiddle,
            PrepAddSingleAtBegin,
            PrepAddSingleAtEnd,
            PrepAddSingleAtMiddle,
            PrepMoveMultipleFromBeginToEnd,
            PrepMoveMultipleFromBeginToMiddle,
            PrepMoveMultipleFromEndToBegin,
            PrepMoveMultipleFromEndToBegin,
            PrepMoveMultipleFromEndToMiddle,
            PrepMoveMultipleFromMiddleToBegin,
            PrepMoveMultipleFromMiddleToEnd,
            PrepMoveSingleFromBeginToEnd,
            PrepMoveSingleFromBeginToMiddle,
            PrepMoveSingleFromEndToBegin,
            PrepMoveSingleFromEndToMiddle,
            PrepMoveSingleFromMiddleToBegin,
            PrepMoveSingleFromMiddleToEnd,
            PrepRemoveMultipleFromBegin,
            PrepRemoveMultipleFromEnd,
            PrepRemoveMultipleFromMiddle,
            PrepRemoveSingleFromBegin,
            PrepRemoveSingleFromEnd,
            PrepRemoveSingleFromMiddle,
            PrepReplaceMultipleByMultipleAtBegin,
            PrepReplaceMultipleByMultipleAtEnd,
            PrepReplaceMultipleByMultipleAtMiddle,
            PrepReplaceMultipleBySingleAtBegin,
            PrepReplaceMultipleBySingleAtEnd,
            PrepReplaceMultipleBySingleAtMiddle,
            PrepReplaceSingleByMultipleAtBegin,
            PrepReplaceSingleByMultipleAtEnd,
            PrepReplaceSingleByMultipleAtMiddle,
            PrepReplaceSingleBySingleAtBegin,
            PrepReplaceSingleBySingleAtEnd,
            PrepReplaceSingleBySingleAtMiddle,
            PrepResetOnEmpty,
            PrepResetOnNotEmpty
        };

        public bool RunAllPrepPairs()
        {
            int ctr = 0;

            List<TSource> target = this.SourceSequence;
            List<TSource> filler = this.FillerItems;

            foreach (PrepAction firstPrep in Preps)
            {
                var targetCopy1 = target.GetRange(0, target.Count);
                var fillerCopy1 = filler.GetRange(0, filler.Count);
                string description1;

                var firstAction = firstPrep(targetCopy1, fillerCopy1, out description1);

                if (firstAction != null)
                    foreach (PrepAction secondPrep in Preps)
                    {
                        var targetCopy2 = targetCopy1.GetRange(0, targetCopy1.Count);
                        var fillerCopy2 = fillerCopy1.GetRange(0, fillerCopy1.Count);
                        string description2;

                        var secondAction = secondPrep(targetCopy2, fillerCopy2, out description2);

                        if (secondAction != null)
                        {
                            string description = "first " + description2 + " then " + description1;

                            if (!RunActions(firstAction, secondAction, targetCopy2, description, ++ctr))
                                return false;
                        }
                    }
            }

            return true;
        }

        protected abstract bool RunActions(CollectionTestSequenceRunner<TSource>.Action firstAction, CollectionTestSequenceRunner<TSource>.Action secondAction, List<TSource> targetCopy2, string description, int count);
    }
}
