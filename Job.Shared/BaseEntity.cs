namespace Job.Shared
{
    /**
     * base class that will be inhrent by all
     * classes that my be need to access datastore
     */
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    }
}
