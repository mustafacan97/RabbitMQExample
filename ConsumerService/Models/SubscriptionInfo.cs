namespace ConsumerService.Models;

public class SubscriptionInfo
{
    #region Constructors and Destructors

    public SubscriptionInfo(Type handlerType)
    {
        HandlerType = handlerType;
    }

    #endregion

    #region Public Properties

    public Type HandlerType { get; }

    #endregion

    #region Public Methods

    public static SubscriptionInfo Create(Type handlerType) => new(handlerType);

    #endregion
}
