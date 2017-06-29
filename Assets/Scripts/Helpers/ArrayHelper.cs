﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionsHelper {
    public static Agent GetValue2(this Agent[,] agents, int i, int j) {
        if (i >= 0 && j >= 0 && agents.GetLength(0) > i && agents.GetLength(1) > j) {
            return agents[i,j];
        }
        return null;
    }

    public static void AddIfNotNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if ((object)value != null)
            dictionary.Add(key, value);
    }
}
