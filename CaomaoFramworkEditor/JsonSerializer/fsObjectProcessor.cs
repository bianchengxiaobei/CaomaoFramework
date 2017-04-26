using System;
using System.Collections.Generic;
public abstract class fsObjectProcessor
{
    /// <summary>
    /// Is the processor interested in objects of the given type?
    /// </summary>
    /// <param name="type">The given type.</param>
    /// <returns>True if the processor should be applied, false otherwise.</returns>
    public virtual bool CanProcess(Type type) { throw new NotImplementedException(); }

    /// <summary>
    /// Called before serialization.
    /// </summary>
    /// <param name="storageType">The field/property type that is storing the instance.</param>
    /// <param name="instance">The type of the instance.</param>
    public virtual void OnBeforeSerialize(Type storageType, object instance) { }

    /// <summary>
    /// Called after serialization.
    /// </summary>
    /// <param name="storageType">The field/property type that is storing the instance.</param>
    /// <param name="instance">The type of the instance.</param>
    /// <param name="data">The data that was serialized.</param>
    public virtual void OnAfterSerialize(Type storageType, object instance, ref fsData data) { }

    /// <summary>
    /// Called before deserialization.
    /// </summary>
    /// <param name="storageType">The field/property type that is storing the instance.</param>
    /// <param name="data">The data that will be used for deserialization.</param>
    public virtual void OnBeforeDeserialize(Type storageType, ref fsData data) { }

    /// <summary>
    /// Called before deserialization has begun but *after* the object instance has been created. This will get
    /// invoked even if the user passed in an existing instance.
    /// </summary>
    /// <remarks>
    /// **IMPORTANT**: The actual instance that gets passed here is *not* guaranteed to be an a subtype of storageType, since
    /// the value for instance is whatever the active converter returned for CreateInstance() - ie, some converters will return
    /// dummy types in CreateInstance() if instance creation cannot be separated from deserialization (ie, KeyValuePair).
    /// </remarks>
    /// <param name="storageType">The field/property type that is storing the instance.</param>
    /// <param name="instance">The created object instance. No deserialization has been applied to it.</param>
    /// <param name="data">The data that will be used for deserialization.</param>
    public virtual void OnBeforeDeserializeAfterInstanceCreation(Type storageType, object instance, ref fsData data) { }

    /// <summary>
    /// Called after deserialization.
    /// </summary>
    /// <param name="storageType">The field/property type that is storing the instance.</param>
    /// <param name="instance">The type of the instance.</param>
    public virtual void OnAfterDeserialize(Type storageType, object instance) { }
}