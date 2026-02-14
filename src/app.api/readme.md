Technical Specification: Transactional Outbox Pattern with EF Core

1. Objective
   Implement a Transactional Outbox Pattern using Entity Framework Core Interceptors. The goal is to ensure atomicity
   between domain state changes and the queuing of integration events, preventing "dual-write" inconsistencies.
2. Core Components
   A. Infrastructure Layer: Outbox Entity
   A persistence model to store serialized domain events within the primary database.
   Table Name: OutboxMessages
   Schema:
   Id (Guid): Primary Key.
   Type (string): The assembly-qualified name or short name of the event type.
   Content (string/NVARCHAR(MAX)): JSON-serialized payload of the event.
   OccurredOnUtc (DateTime): Timestamp of event creation.
   ProcessedOnUtc (DateTime?): Nullable; indicates when the background worker dispatched the message.
   Error (string?): Stores exception details if dispatch fails.
   B. Domain Layer: Event Tracking
   An abstraction to allow Domain Entities to hold events in memory during a single Unit of Work.
   IDomainEvent: Marker interface for all domain events (implemented as record for immutability).
   IHasDomainEvents: Interface exposing GetDomainEvents() and ClearDomainEvents().
   Entity (Base Class): Implements IHasDomainEvents and provides a protected void Raise(IDomainEvent) method for derived
   entities.
   C. Persistence Layer: SaveChangesInterceptor
   A custom SaveChangesInterceptor to automate the transition from Domain Events to Outbox Messages.
   Logic: Override SavingChangesAsync.
   Operation:
   Scan ChangeTracker for entities implementing IHasDomainEvents.
   Extract all pending events.
   Serialize events using System.Text.Json (explicitly passing event.GetType() to ensure polymorphic serialization).
   Append new OutboxMessage entities to the current DbContext change tracker.
   Clear events from the source entities to prevent duplicate processing.
   D. Application Layer: Background Processor
   A BackgroundService (IHostedService) to act as the "Message Relay."
   Polling: Queries OutboxMessages where ProcessedOnUtc is null.
   Execution:
   Fetch a batch (e.g., top 20).
   Dispatch to a message broker (RabbitMQ/Service Bus).
   Update ProcessedOnUtc upon success or log to Error on failure.
   Critical: Use a database-level lock or a library like Quartz.NET to prevent race conditions in multi-instance
   deployments.
3. Implementation Requirements (Prompt Instructions)
   When generating code based on this spec, ensure:
   Encapsulation: Domain Entities (e.g., Order) should have private set properties and use factory methods (e.g.,
   Create()).
   EF Core Configuration: Use modelBuilder.Ignore<IHasDomainEvents>() to prevent internal event lists from being mapped
   to database columns.
   DI Registration: The OutboxInterceptor must be registered and attached via .AddInterceptors() in the AddDbContext
   configuration.
