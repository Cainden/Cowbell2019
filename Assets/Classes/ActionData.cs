// Data container class for a custom action item (e.g. used for avatar movement).

using System;

namespace MySpace
{
    public class ActionData
    {
        private Action _ActionItem;

        public Action ActionItem
        {
            get { return _ActionItem; }
            set { _ActionItem = value; }
        }

        public ActionData(Action actionItem)
        {
            _ActionItem = actionItem;
        }
    }
}

