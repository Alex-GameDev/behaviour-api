namespace BehaviourAPI.Core
{
    public static class Utils
    {
        public static Status Inverted(this Status status)
        {
            if (status == Status.Success) return Status.Failure;
            if (status == Status.Failure) return Status.Success;
            else return status;
        }

        public static Status ToStatus(this bool check)
        {
            return check ? Status.Success : Status.Failure;
        }

        public static void MoveAtFirst<T>(this List<T> list, T element)
        {
            if(list.Remove(element)) list.Insert(0, element);
        }
    }
}
