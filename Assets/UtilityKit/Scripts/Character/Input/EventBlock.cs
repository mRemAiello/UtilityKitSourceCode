namespace MCFramework
{
    public class EventBlock : System.IComparable<EventBlock>
    {
        public int priority;
        public event System.Action<EventData> Event;

        public EventBlock(int p)
        {
            priority = p;
        }

        public void AppendTo(ref System.Action<EventData> deleg)
        {
            if (Event != null)
                deleg += Event;
        }

        // Order highest to lowest
        public int CompareTo(EventBlock other)
        {
            return - priority.CompareTo(other.priority);
        }

        public void Invoke(EventData eventData)
        {
            Event?.Invoke(eventData);
        }

        public bool IsEmpty
        {
            get
            {
                return Event == null;
            }
        }
    }
}

