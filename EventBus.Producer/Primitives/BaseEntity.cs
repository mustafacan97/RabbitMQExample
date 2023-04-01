namespace ProducerService.Primitives;

public class BaseEntity
{
    #region Public Properties and Indexers

    public Guid Id { get; private set; }

    #endregion

    #region Constructors and Destructors

    public BaseEntity(Guid id)
    {
        Id = id;
    }

    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }

    #endregion
}