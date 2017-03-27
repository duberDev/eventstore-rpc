using System;

namespace EventStore.RPC.Server
{
    public static class PersistentSubscriptionSettingsExtensions
    {
        public static ClientAPI.PersistentSubscriptionSettings ToPersistentSubscriptionSettings(
            this PersistentSubscriptionSettings persistentSubscriptionSettings)
        {
            var settings = ClientAPI.PersistentSubscriptionSettings.Create();

            if (persistentSubscriptionSettings == null) return settings;

            if (persistentSubscriptionSettings.ResolveLinkTos)
            {
                settings.ResolveLinkTos();
            }
            else
            {
                settings.DoNotResolveLinkTos();
            }

            settings.StartFrom(persistentSubscriptionSettings.StartFrom);

            if (persistentSubscriptionSettings.ExtraStatistics)
            {
                settings.WithExtraStatistics();
            }

            settings.WithMessageTimeoutOf(TimeSpan.FromSeconds(persistentSubscriptionSettings.MessageTimeout));

            settings.WithMaxRetriesOf(persistentSubscriptionSettings.MaxRetryCount);

            settings.WithLiveBufferSizeOf(persistentSubscriptionSettings.LiveBufferSize);

            settings.WithReadBatchOf(persistentSubscriptionSettings.ReadBatchSize);

            settings.WithBufferSizeOf(persistentSubscriptionSettings.HistoryBufferSize);

            settings.CheckPointAfter(TimeSpan.FromSeconds(persistentSubscriptionSettings.CheckPointAfter));

            settings.MinimumCheckPointCountOf(persistentSubscriptionSettings.MinCheckPointCount);

            settings.MaximumCheckPointCountOf(persistentSubscriptionSettings.MaxCheckPointCount);

            settings.WithMaxSubscriberCountOf(persistentSubscriptionSettings.MaxSubscriberCount);

            settings.WithNamedConsumerStrategy(persistentSubscriptionSettings.NamedConsumerStrategy);

            return settings;
        }
    }
}