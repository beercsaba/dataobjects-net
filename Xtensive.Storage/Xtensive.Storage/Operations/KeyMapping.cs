// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexis Kochetov
// Created:    2009.11.19

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Xtensive.Core.Internals.DocTemplates;

namespace Xtensive.Storage.Operations
{
  /// <summary>
  /// Maps local ("disconnected") <see cref="Key"/> instances to actual (storage) <see cref="Key"/> instances.
  /// </summary>
  [Serializable]
  public sealed class KeyMapping : ISerializable
  {
    public Dictionary<Key, Key> Map { get; private set; }

    /// <summary>
    /// Tries to remaps the specified key;
    /// returns the original key, if there is no 
    /// remapped key in <see cref="Map"/> for it.
    /// </summary>
    /// <param name="key">The key to remap.</param>
    /// <returns>The mapped storage <see cref="Key"/>.</returns>
    public Key TryRemapKey(Key key)
    {
      Key remappedKey;
      if (key!=null && Map.TryGetValue(key, out remappedKey))
        return remappedKey;
      else
        return key;
    }


    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    public KeyMapping(Dictionary<Key,Key> map)
    {
      Map = map;
    }

    // Serialization

    /// <inheritdoc/>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      var serializedMapping = new Dictionary<SerializableKey, SerializableKey>();
      foreach (var pair in Map)
        serializedMapping.Add(pair.Key, pair.Value);

      info.AddValue("Map", serializedMapping, typeof(Dictionary<SerializableKey,SerializableKey>));
    }

    /// <see cref="SerializableDocTemplate.Ctor" copy="true"/>
    protected KeyMapping(SerializationInfo info, StreamingContext context)
    {
      var serializedMapping = (Dictionary<SerializableKey, SerializableKey>)
        info.GetValue("Map", typeof(Dictionary<SerializableKey, SerializableKey>));
      Map = new Dictionary<Key, Key>();
      foreach (var pair in serializedMapping)
        Map.Add((Key) pair.Key, (Key) pair.Value);
    }
  }
}