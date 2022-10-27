namespace BehaviourAPI.Core
{
    public static class Utils
    {
        public static Status Inverted(this Status status)
        {
            if (status == Status.Sucess) return Status.Failure;
            if (status == Status.Failure) return Status.Sucess;
            return status;
        }

        public static Status ToStatus(this bool check)
        {
            return check ? Status.Sucess : Status.Failure;
        }
    }
}
