using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// A structure used for weighted random selection
/// </summary>
/// <typeparam name="T">
/// The type of item in the selector
/// </typeparam>
public class ItemSelector<T> where T : notnull
{
    private readonly RandomNumberGenerator random = new();
    private readonly Dictionary<T, double> entries;
    private double totalWeight;

    public double TotalWeight { get { return totalWeight; } }

    public int Size { get { return entries.Count; } }

    public bool Empty { get { return Size == 0; } }

    public ItemSelector()
    {
        entries = new Dictionary<T, double>();
        totalWeight = 0;
    }

    /// <summary>
    /// Adds an item to the selector
    /// </summary>
    /// <param name="item">
    /// The item
    /// </param>
    /// <param name="weight">
    /// The item's weight
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the provided weight is less than or equal to 0
    /// </exception>
    public void AddItem(T item, double weight)
    {
        if (entries.ContainsKey(item))
        {
            RemoveItem(item);
        }

        if (weight <= 0)
        {
            throw new ArgumentException("Weight must be greater than 0");
        }

        entries[item] = weight;
        totalWeight += weight;
    }

    /// <summary>
    /// Removes an item from the selector
    /// </summary>
    /// <param name="item">
    /// The item to remove
    /// </param>
    public void RemoveItem(T item)
    {
        if (!entries.TryGetValue(item, out _))
            return;

        totalWeight -= entries[item];
        entries.Remove(item);
    }

    /// <summary>
    /// The weight of an item
    /// </summary>
    /// <param name="item">
    /// The item
    /// </param>
    /// <returns>
    /// The weight of the item
    /// </returns>
    public double Weight(T item)
    {
        if (!Contains(item))
            return -1;

        return entries[item];
    }

    /// <summary>
    /// If an item is in the selector
    /// </summary>
    /// <param name="item">
    /// The item
    /// </param>
    /// <returns>
    /// If the item is in the selector
    /// </returns>
    public bool Contains(T item)
    {
        return entries.ContainsKey(item);
    }

    /// <summary>
    /// Rolls the selector for a random item
    /// </summary>
    /// <returns>
    /// A random item from the selector
    /// </returns>
    /// <exception cref="Exception">
    /// If something went wrong with selection
    /// </exception>
    public T Roll()
    {
        double value = random.RandfRange(0, (float)totalWeight);

        foreach (var item in entries.Keys)
        {
            double weight = entries[item];
            value -= weight;

            if (value < 0)
                return item;
        }

        throw new Exception("Impossible state");
    }

    /// <summary>
    /// The percentage chance of an item being rolled
    /// </summary>
    /// <param name="item">
    /// The item
    /// </param>
    /// <returns>
    /// The percent chance of the item being rolled
    /// </returns>
    public double Chance(T item)
    {
        if (!Contains(item))
            return 0;

        return entries[item] / totalWeight;
    }        
}